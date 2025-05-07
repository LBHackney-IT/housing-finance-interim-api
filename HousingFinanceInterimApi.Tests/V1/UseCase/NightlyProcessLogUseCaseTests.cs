using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using FluentAssertions;
using HousingFinanceInterimApi.V1.Gateway.Interfaces;
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
        private NightlyProcessLogUseCase _nightlyProcessLogUseCase;

        public NightlyProcessLogUseCaseTests()
        {
            _mockGateway = new Mock<INightlyProcessLogGateway>();
            _mockCloudWatchLogsClient = new Mock<IAmazonCloudWatchLogs>();
        }

        [Fact]
        public async Task QueryCloudWatchLogs_Should_Return_Results_For_Successful_Query()
        {
            // Arrange
            var logGroups = new List<string> { "/aws/lambda/log-group-function1" };
            _nightlyProcessLogUseCase = new NightlyProcessLogUseCase(_mockGateway.Object, _mockCloudWatchLogsClient.Object, logGroups);

            var queryResults = new List<List<ResultField>> { new List<ResultField> { new ResultField { Field = "Test", Value = "Value" } } };

            _mockCloudWatchLogsClient
                .Setup(x => x.StartQueryAsync(It.IsAny<StartQueryRequest>(), default))
                .ReturnsAsync(new StartQueryResponse { QueryId = "test-query-id" });

            _mockCloudWatchLogsClient
                .Setup(x => x.GetQueryResultsAsync(It.IsAny<GetQueryResultsRequest>(), default))
                .ReturnsAsync(new GetQueryResultsResponse { Status = QueryStatus.Complete, Results = queryResults });

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
            _nightlyProcessLogUseCase = new NightlyProcessLogUseCase(_mockGateway.Object, _mockCloudWatchLogsClient.Object, logGroups);

            _mockCloudWatchLogsClient
                .Setup(x => x.StartQueryAsync(It.IsAny<StartQueryRequest>(), default))
                .ReturnsAsync(new StartQueryResponse { QueryId = "test-query-id" });

            _mockCloudWatchLogsClient
                .Setup(x => x.GetQueryResultsAsync(It.IsAny<GetQueryResultsRequest>(), default))
                .ReturnsAsync(new GetQueryResultsResponse { Status = QueryStatus.Failed });

            // Act
            Func<Task> act = async () => await _nightlyProcessLogUseCase.QueryCloudWatchLogs(logGroups[0]).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Cloudwatch Insights Query failed. Status: Failed").ConfigureAwait(false);
        }

        [Fact]
        public async Task ExecuteAsync_Should_Process_LogGroups_InSequence()
        {
            // Arrange
            var logGroups = new List<string> { "/aws/lambda/log-group-function1", "/aws/lambda/log-group-function2" };
            _nightlyProcessLogUseCase = new NightlyProcessLogUseCase(_mockGateway.Object, _mockCloudWatchLogsClient.Object, logGroups);

            var queryResults = new List<List<ResultField>> { new List<ResultField> { new ResultField { Field = "Test", Value = "Value" } } };

            _mockGateway
                .Setup(x => x.UpdateDatabaseWithResults(It.IsAny<string>(), It.IsAny<List<List<ResultField>>>()))
                .Returns(Task.CompletedTask);

            _mockCloudWatchLogsClient
                .Setup(x => x.StartQueryAsync(It.IsAny<StartQueryRequest>(), default))
                .ReturnsAsync(new StartQueryResponse { QueryId = "test-query-id" });

            _mockCloudWatchLogsClient
                .Setup(x => x.GetQueryResultsAsync(It.IsAny<GetQueryResultsRequest>(), default))
                .ReturnsAsync(new GetQueryResultsResponse { Status = QueryStatus.Complete, Results = queryResults });

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
            var logGroups = new List<string> {
                "/aws/lambda/log-group-function1",
                "/aws/lambda/log-group-function2"
            };
            _nightlyProcessLogUseCase = new NightlyProcessLogUseCase(_mockGateway.Object, _mockCloudWatchLogsClient.Object, logGroups);

            var queryResults = new List<List<ResultField>> { new List<ResultField> {
                    new ResultField { Field = "Test", Value = "Value" }
                }
            };

            _mockGateway
                .Setup(x => x.UpdateDatabaseWithResults(It.IsAny<string>(), It.IsAny<List<List<ResultField>>>()))
                .Returns(Task.CompletedTask);

            _mockCloudWatchLogsClient
                .SetupSequence(x => x.StartQueryAsync(It.IsAny<StartQueryRequest>(), default))
                .ReturnsAsync(new StartQueryResponse { QueryId = "test-query-id" })
                .ThrowsAsync(new Exception("CloudWatch error"));

            _mockCloudWatchLogsClient
                .Setup(x => x.GetQueryResultsAsync(It.IsAny<GetQueryResultsRequest>(), default))
                .ReturnsAsync(new GetQueryResultsResponse { Status = QueryStatus.Complete, Results = queryResults });

            // Act
            var response = await _nightlyProcessLogUseCase.ExecuteAsync().ConfigureAwait(false);

            // Assert
            response.Should().NotBeNull();
            response.Continue.Should().BeTrue();
            _mockGateway.Verify(
                x => x.UpdateDatabaseWithResults(It.IsAny<string>(), queryResults),
                Times.Once);
            _mockGateway.Verify(
                x => x.UpdateDatabaseWithResults(It.IsAny<string>(), It.Is<List<List<ResultField>>>(r => r[0][0].Field == "Error")),
                Times.Once);
        }


        [Fact]
        public async Task ExecuteAsync_ShouldThrowArgumentException_WhenCreatedDateIsDefault()
        {
            // Arrange
            var createdDate = default(DateTime);
            var logGroups = new List<string> { "/aws/lambda/log-group-function1" };
            _nightlyProcessLogUseCase = new NightlyProcessLogUseCase(_mockGateway.Object, _mockCloudWatchLogsClient.Object, logGroups);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _nightlyProcessLogUseCase.ExecuteAsync(createdDate)).ConfigureAwait(false);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnEmptyList_WhenNoLogsFound()
        {
            // Arrange
            var createdDate = DateTime.UtcNow;
            var logGroups = new List<string> { "/aws/lambda/log-group-function1" };
            _nightlyProcessLogUseCase = new NightlyProcessLogUseCase(_mockGateway.Object, _mockCloudWatchLogsClient.Object, logGroups);

            _mockGateway
                .Setup(x => x.GetByDateCreatedAsync(createdDate))
                .ReturnsAsync(new List<NightlyProcessLog>());

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
            var logGroups = new List<string> { "/aws/lambda/log-group-function1" };
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

            _nightlyProcessLogUseCase = new NightlyProcessLogUseCase(_mockGateway.Object, _mockCloudWatchLogsClient.Object, logGroups);

            _mockGateway
                .Setup(x => x.GetByDateCreatedAsync(createdDate))
                .ReturnsAsync(logs);

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
            _nightlyProcessLogUseCase = new NightlyProcessLogUseCase(_mockGateway.Object, _mockCloudWatchLogsClient.Object, logGroups);

            // Act & Assert
            await Assert.ThrowsAsync<System.InvalidOperationException>(() => _nightlyProcessLogUseCase.ExecuteAsync(createdDate)).ConfigureAwait(false);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowApplicationException_WhenUnexpectedExceptionOccurs()
        {
            // Arrange
            var createdDate = DateTime.UtcNow;
            var logGroups = new List<string> { "/aws/lambda/log-group-function1" };
            _nightlyProcessLogUseCase = new NightlyProcessLogUseCase(_mockGateway.Object, _mockCloudWatchLogsClient.Object, logGroups);

            _mockGateway
                .Setup(x => x.GetByDateCreatedAsync(createdDate))
                .Throws(new Exception("Unexpected error"));

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationException>(() => _nightlyProcessLogUseCase.ExecuteAsync(createdDate)).ConfigureAwait(false);
        }

    }
}
