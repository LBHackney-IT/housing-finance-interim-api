using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using FluentAssertions;
using HousingFinanceInterimApi.V1.Gateway.Interfaces;
using HousingFinanceInterimApi.V1.UseCase;
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
            var logGroups = new List<string> { "/aws/lambda/log-group-function1", "/aws/lambda/log-group-function2" };
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
    }
}
