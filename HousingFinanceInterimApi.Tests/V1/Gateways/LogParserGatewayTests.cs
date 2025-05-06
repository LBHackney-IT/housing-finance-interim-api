using Amazon.CloudWatchLogs.Model;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.Gateways
{
    public class LogParserGatewayTests
    {
        private readonly Mock<IDatabaseContext> _mockContext;
        private readonly Mock<DbSet<NightlyProcessLog>> _mockDbSet;
        private readonly LogParserGateway _gateway;

        public LogParserGatewayTests()
        {
            _mockContext = new Mock<IDatabaseContext>();
            _mockDbSet = new Mock<DbSet<NightlyProcessLog>>();

            // Setup DbSet in the mock context
            _mockContext.Setup(c => c.NightlyProcessLogs).Returns(_mockDbSet.Object);

            // Initialize the gateway with the mocked context
            _gateway = new LogParserGateway(_mockContext.Object);
        }

        [Fact]
        public async Task UpdateDatabaseWithResults_ShouldSaveToDatabaseAsFail_WhenValidInputHasError()
        {
            // Arrange
            var logGroupName = "/aws/lambda/log-group-function1";
            var queryResults = new List<List<ResultField>>
            {
                new List<ResultField>
                {
                    new ResultField { Field = "@timestamp", Value = DateTime.UtcNow.ToString("o") },
                    new ResultField { Field = "@message", Value = "Test log message contains error" }
                }
            };

            // Act
            await _gateway.UpdateDatabaseWithResults(logGroupName, queryResults).ConfigureAwait(false);

            // Assert
            _mockDbSet.Verify(db => db.AddAsync(It.IsAny<NightlyProcessLog>(), default), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateDatabaseWithResults_ShouldSaveToDatabaseAsSuccess_WhenValidInputHasNoError()
        {
            // Arrange
            var logGroupName = "/aws/lambda/log-group-function1";
            var queryResults = new List<List<ResultField>>
            {
                new List<ResultField>
                {
                    new ResultField { Field = "@timestamp", Value = DateTime.UtcNow.ToString("o") },
                    new ResultField { Field = "@message", Value = "Test log message" }
                }
            };

            // Act
            await _gateway.UpdateDatabaseWithResults(logGroupName, queryResults).ConfigureAwait(false);

            // Assert
            _mockDbSet.Verify(db => db.AddAsync(It.IsAny<NightlyProcessLog>(), default), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateDatabaseWithResults_ShouldThrowArgumentNullException_WhenLogGroupNameIsNull()
        {
            // Arrange
            string logGroupName = null;
            var queryResults = new List<List<ResultField>>();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _gateway.UpdateDatabaseWithResults(logGroupName, queryResults)).ConfigureAwait(false);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task UpdateDatabaseWithResults_ShouldLogAndThrow_WhenDbUpdateException()
        {
            // Arrange
            var logGroupName = "/aws/lambda/log-group-function1";
            var queryResults = new List<List<ResultField>>
            {
                new List<ResultField>
                {
                    new ResultField { Field = "@timestamp", Value = DateTime.UtcNow.ToString("o") },
                    new ResultField { Field = "@message", Value = "Test log message contains error" }
                }
            };

            _mockContext.Setup(c => c.SaveChangesAsync(default))
                .ThrowsAsync(new DbUpdateException("Database update failed"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<DbUpdateException>(() => _gateway.UpdateDatabaseWithResults(logGroupName, queryResults)).ConfigureAwait(false);

            Assert.Equal("Database update failed", exception.Message);
        }

        [Fact]
        public async Task UpdateDatabaseWithResults_ShouldLogErrorAndContinue_WhenInvalidTimestamp()
        {
            // Arrange
            var logGroupName = "/aws/lambda/log-group-function1";
            var queryResults = new List<List<ResultField>>
            {
                new List<ResultField>
                {
                    new ResultField { Field = "@timestamp", Value = "invalid-timestamp" },
                    new ResultField { Field = "@message", Value = "Test log message" }
                }
            };

            // Act
            await _gateway.UpdateDatabaseWithResults(logGroupName, queryResults).ConfigureAwait(false);

            // Assert
            _mockDbSet.Verify(db => db.AddAsync(It.IsAny<NightlyProcessLog>(), default), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateDatabaseWithResults_ShouldAddFailure_WhenErrorExists()
        {
            // Arrange
            var logGroupName = "/aws/lambda/log-group-function1";
            var queryResults = new List<List<ResultField>>
            {
                new List<ResultField>
                {
                    new ResultField { Field = "@timestamp", Value = DateTime.UtcNow.ToString("o") },
                    new ResultField { Field = "@message", Value = "Test log message" }
                },
                new List<ResultField>
                {
                    new ResultField { Field = "@timestamp", Value = DateTime.UtcNow.ToString("o") },
                    new ResultField { Field = "@message", Value = "Error occurred" }
                }
            };

            // Act
            await _gateway.UpdateDatabaseWithResults(logGroupName, queryResults).ConfigureAwait(false);

            // Assert
            _mockDbSet.Verify(db => db.AddAsync(It.IsAny<NightlyProcessLog>(), default), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateDatabaseWithResults_ShouldStopProcessingOnFirstError()
        {
            // Arrange
            var logGroupName = "/aws/lambda/log-group-function1";
            var queryResults = new List<List<ResultField>>
            {
                new List<ResultField>
                {
                    new ResultField { Field = "@timestamp", Value = DateTime.UtcNow.ToString("o") },
                    new ResultField { Field = "@message", Value = "Error occurred" }
                },
                new List<ResultField>
                {
                    new ResultField { Field = "@timestamp", Value = DateTime.UtcNow.ToString("o") },
                    new ResultField { Field = "@message", Value = "Another log message" }
                }
            };

            // Act
            await _gateway.UpdateDatabaseWithResults(logGroupName, queryResults).ConfigureAwait(false);

            // Assert
            _mockDbSet.Verify(db => db.AddAsync(It.IsAny<NightlyProcessLog>(), default), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateDatabaseWithResults_Case1_ResultsExistForKeyword()
        {
            // Arrange
            var logGroupName = "test-log-group";
            var queryResults = new List<List<ResultField>>
            {
                new List<ResultField>
                {
                    new ResultField { Field = "@timestamp", Value = DateTime.UtcNow.ToString("o") },
                    new ResultField { Field = "@message", Value = "RequestId: 12345" }
                }
            };

            // Act
            await _gateway.UpdateDatabaseWithResults(logGroupName, queryResults);

            // Assert
            _mockContext.Verify(x => x.NightlyProcessLogs.AddAsync(It.Is<NightlyProcessLog>(log =>
                log.LogGroupName == logGroupName &&
                log.IsSuccess == false), default), Times.Once);
            _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateDatabaseWithResults_Case2_LogsExistButNoMatchForKeyword()
        {
            // Arrange
            var logGroupName = "test-log-group";
            var queryResults = new List<List<ResultField>>();

            // Act
            await _gateway.UpdateDatabaseWithResults(logGroupName, queryResults);

            // Assert
            _mockContext.Verify(x => x.NightlyProcessLogs.AddAsync(It.Is<NightlyProcessLog>(log =>
                log.LogGroupName == logGroupName &&
                log.IsSuccess == true), default), Times.Once);
            _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateDatabaseWithResults_Case3_NoLogsExistForLogGroup()
        {
            // Arrange
            var logGroupName = "test-log-group";
            List<List<ResultField>> queryResults = null;

            // Act
            await _gateway.UpdateDatabaseWithResults(logGroupName, queryResults);

            // Assert
            _mockContext.Verify(x => x.NightlyProcessLogs.AddAsync(It.Is<NightlyProcessLog>(log =>
                log.LogGroupName == logGroupName &&
                log.IsSuccess == null), default), Times.Once);
            _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }
    }
}
