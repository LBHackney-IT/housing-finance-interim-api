using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using FluentAssertions;
using HousingFinanceInterimApi.V1.Gateway.Interfaces;
using HousingFinanceInterimApi.V1.Helpers;
using HousingFinanceInterimApi.V1.Infrastructure;
using HousingFinanceInterimApi.V1.UseCase;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.UseCase
{
    public class NightlyProcessLogUseCaseTests
    {
        private readonly Mock<IAmazonCloudWatchLogs> _mockCloudWatchLogsClient;
        private readonly Mock<INightlyProcessLogGateway> _mockGateway;
        private readonly Mock<ILogGroupProvider> _mockProvider;
        private NightlyProcessLogUseCase _nightlyProcessLogUseCase;

        public NightlyProcessLogUseCaseTests()
        {
            _mockGateway = new Mock<INightlyProcessLogGateway>();
            _mockCloudWatchLogsClient = new Mock<IAmazonCloudWatchLogs>();
            _mockProvider = new Mock<ILogGroupProvider>();
        }

        [Fact]
        public async Task QueryCloudWatchLogs_Should_Return_Results_For_Successful_Query()
        {
            // Arrange
            var logGroups = new List<string> { "/aws/lambda/log-group-function1" };
            _nightlyProcessLogUseCase = new NightlyProcessLogUseCase(_mockGateway.Object, _mockCloudWatchLogsClient.Object, _mockProvider.Object);

            var queryResults = new List<List<ResultField>> { new List<ResultField> { new ResultField { Field = "Test", Value = "Value" } } };

            _mockCloudWatchLogsClient
                .Setup(x => x.StartQueryAsync(It.IsAny<StartQueryRequest>(), default))
                .ReturnsAsync(new StartQueryResponse { QueryId = "test-query-id" });

            _mockCloudWatchLogsClient
                .Setup(x => x.GetQueryResultsAsync(It.IsAny<GetQueryResultsRequest>(), default))
                .ReturnsAsync(new GetQueryResultsResponse { Status = QueryStatus.Complete, Results = queryResults });

            _mockProvider
                .Setup(x => x.GetLogGroups(It.IsAny<string>()))
                .Returns(logGroups);

            // Act
            var results = await _nightlyProcessLogUseCase.QueryCloudWatchLogs(logGroups[0]).ConfigureAwait(false);

            // Assert
            results.Should().NotBeNull();
            results.Should().BeEquivalentTo(queryResults);
        }

        [Fact]
        public async Task QueryCloudWatchLogs_Should_Throw_Exception_For_Failed_Query()
        {
            // Arrange
            var logGroups = new List<string> { "/aws/lambda/log-group-function1" };
            _nightlyProcessLogUseCase = new NightlyProcessLogUseCase(_mockGateway.Object, _mockCloudWatchLogsClient.Object, _mockProvider.Object);

            _mockCloudWatchLogsClient
                .Setup(x => x.StartQueryAsync(It.IsAny<StartQueryRequest>(), default))
                .ReturnsAsync(new StartQueryResponse { QueryId = "test-query-id" });

            _mockCloudWatchLogsClient
                .Setup(x => x.GetQueryResultsAsync(It.IsAny<GetQueryResultsRequest>(), default))
                .ReturnsAsync(new GetQueryResultsResponse { Status = QueryStatus.Failed });

            _mockProvider
                .Setup(x => x.GetLogGroups(It.IsAny<string>()))
                .Returns(logGroups);

            // Act
            Func<Task> act = async () => await _nightlyProcessLogUseCase.QueryCloudWatchLogs(logGroups[0]).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Cloudwatch Insights Query failed. Status: Failed").ConfigureAwait(false);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Process_LogGroups_InSequence()
        {
            // Arrange
            _mockGateway
                .Setup(x => x.UpdateDatabaseWithResults(It.IsAny<string>(), It.IsAny<List<List<ResultField>>>()))
                .Returns(Task.CompletedTask);

            _mockCloudWatchLogsClient
                .Setup(x => x.StartQueryAsync(It.IsAny<StartQueryRequest>(), default))
                .ReturnsAsync(new StartQueryResponse { QueryId = "test-query-id" });

            var queryResults = new List<List<ResultField>> { new List<ResultField> { new ResultField { Field = "Test", Value = "Value" } } };
            _mockCloudWatchLogsClient
                .Setup(x => x.GetQueryResultsAsync(It.IsAny<GetQueryResultsRequest>(), default))
                .ReturnsAsync(new GetQueryResultsResponse { Status = QueryStatus.Complete, Results = queryResults });

            var logGroups = new List<string> { "/aws/lambda/log-group-function1", "/aws/lambda/log-group-function2" };
            _mockProvider
                .Setup(x => x.GetLogGroups(It.IsAny<string>()))
                .Returns(logGroups);

            _nightlyProcessLogUseCase = new NightlyProcessLogUseCase(_mockGateway.Object, _mockCloudWatchLogsClient.Object, _mockProvider.Object);

            // Act
            var response = await _nightlyProcessLogUseCase.ExecuteAsync().ConfigureAwait(false);

            // Assert
            response.Should().NotBeNull();
            response.Continue.Should().BeTrue();
            _mockGateway.Verify(
                x => x.UpdateDatabaseWithResults(It.IsAny<string>(), queryResults),
                Times.Exactly(logGroups.Count));
        }

        [Fact]
        public async Task ExecuteAsync_Should_Log_Any_Errors_And_Continues_Processing()
        {
            // Arrange
            _mockCloudWatchLogsClient
                .SetupSequence(x => x.StartQueryAsync(It.IsAny<StartQueryRequest>(), default))
                .ReturnsAsync(new StartQueryResponse { QueryId = "query-id-1" })
                .ThrowsAsync(new AmazonCloudWatchLogsException("AWS error"));

            _mockCloudWatchLogsClient
                .Setup(x => x.GetQueryResultsAsync(It.IsAny<GetQueryResultsRequest>(), default))
                .ReturnsAsync(new GetQueryResultsResponse
                {
                    Status = QueryStatus.Complete,
                    Results = new List<List<ResultField>>
                    {
                new List<ResultField> { new ResultField { Field = "Test", Value = "Value" } }
                    }
                });

            _mockGateway
                .Setup(x => x.UpdateDatabaseWithResults(It.IsAny<string>(), It.IsAny<List<List<ResultField>>>()))
                .Returns(Task.CompletedTask);

            var logGroups = new List<string> { "log-group-1", "log-group-2" };
            _mockProvider
                .Setup(x => x.GetLogGroups(It.IsAny<string>()))
                .Returns(logGroups);

            _nightlyProcessLogUseCase = new NightlyProcessLogUseCase(_mockGateway.Object, _mockCloudWatchLogsClient.Object, _mockProvider.Object);

            // Act
            var response = await _nightlyProcessLogUseCase.ExecuteAsync().ConfigureAwait(false);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Continue);
            _mockGateway.Verify(
                x => x.UpdateDatabaseWithResults("log-group-1", It.IsAny<List<List<ResultField>>>()),
                Times.Once);
            _mockGateway.Verify(
                x => x.UpdateDatabaseWithResults("log-group-2", It.IsAny<List<List<ResultField>>>()),
                Times.Once); // Not Twice, because the second log group throws an exception
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowArgumentException_WhenCreatedDateIsDefault()
        {
            // Arrange
            var createdDate = default(DateTime);
            var logGroups = new List<string> { "/aws/lambda/log-group-function1" };
            _nightlyProcessLogUseCase = new NightlyProcessLogUseCase(_mockGateway.Object, _mockCloudWatchLogsClient.Object, _mockProvider.Object);

            _mockProvider
                .Setup(x => x.GetLogGroups(It.IsAny<string>()))
                .Returns(logGroups);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _nightlyProcessLogUseCase.ExecuteAsync(createdDate)).ConfigureAwait(false);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnEmptyList_WhenNoLogsFound()
        {
            // Arrange
            var createdDate = DateTime.UtcNow;
            _mockGateway
                .Setup(x => x.GetByDateCreatedAsync(createdDate))
                .ReturnsAsync(new List<NightlyProcessLog>());

            var logGroups = new List<string> { "/aws/lambda/log-group-function1" };
            _mockProvider
                .Setup(x => x.GetLogGroups(It.IsAny<string>()))
                .Returns(logGroups);

            _nightlyProcessLogUseCase = new NightlyProcessLogUseCase(_mockGateway.Object, _mockCloudWatchLogsClient.Object, _mockProvider.Object);

            // Act
            var result = await _nightlyProcessLogUseCase.ExecuteAsync(createdDate).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }


        [Fact]
        public async Task ExecuteAsync_ShouldReturnLogs_WhenLogsExistForGivenDate()
        {
            // Arrange
            var createdDate = DateTime.UtcNow.Date;
            var logs = new List<NightlyProcessLog>
            {
                new NightlyProcessLog
                {
                    Id = 1,
                    LogGroupName = "TestLogGroup",
                    Timestamp = DateTime.UtcNow,
                    IsSuccess = true,
                    DateCreated = createdDate
                }
            };

            _nightlyProcessLogUseCase = new NightlyProcessLogUseCase(_mockGateway.Object, _mockCloudWatchLogsClient.Object, _mockProvider.Object);

            _mockGateway
                .Setup(x => x.GetByDateCreatedAsync(createdDate))
                .ReturnsAsync(logs);

            var logGroups = new List<string> { "/aws/lambda/log-group-function1" };
            _mockProvider
                .Setup(x => x.GetLogGroups(It.IsAny<string>()))
                .Returns(logGroups);

            // Act
            var result = await _nightlyProcessLogUseCase.ExecuteAsync(createdDate).ConfigureAwait(false);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("TestLogGroup", result.First().LogGroupName);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowInvalidOperationException_WhenDbUpdateExceptionOccurs()
        {
            // Arrange
            var createdDate = DateTime.UtcNow;
            var logGroups = new List<string> { "/aws/lambda/log-group-function1" };

            _mockGateway
                .Setup(x => x.GetByDateCreatedAsync(createdDate))
                .Throws(new DbUpdateException("Database error"));

            _mockProvider
                .Setup(x => x.GetLogGroups(It.IsAny<string>()))
                .Returns(logGroups);

            _nightlyProcessLogUseCase = new NightlyProcessLogUseCase(_mockGateway.Object, _mockCloudWatchLogsClient.Object, _mockProvider.Object);

            // Act & Assert
            await Assert.ThrowsAsync<System.InvalidOperationException>(() => _nightlyProcessLogUseCase.ExecuteAsync(createdDate)).ConfigureAwait(false);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowApplicationException_WhenUnexpectedExceptionOccurs()
        {
            // Arrange
            var createdDate = DateTime.UtcNow;
            var logGroups = new List<string> { "/aws/lambda/log-group-function1" };
            _nightlyProcessLogUseCase = new NightlyProcessLogUseCase(_mockGateway.Object, _mockCloudWatchLogsClient.Object, _mockProvider.Object);

            _mockGateway
                .Setup(x => x.GetByDateCreatedAsync(createdDate))
                .Throws(new Exception("Unexpected error"));

            _mockProvider
                .Setup(x => x.GetLogGroups(It.IsAny<string>()))
                .Returns(logGroups);

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationException>(() => _nightlyProcessLogUseCase.ExecuteAsync(createdDate)).ConfigureAwait(false);
        }

    }
}
