using Amazon.CloudWatchLogs.Model;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Infrastructure;
using HousingFinanceInterimApi.V1.Domain;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.Gateways
{
    public class NightlyProcessLogGatewayTests
    {
        private readonly Mock<IDatabaseContext> _mockContext;
        private readonly Mock<DbSet<NightlyProcessLog>> _mockDbSet;
        private readonly NightlyProcessLogGateway _gateway;

        public NightlyProcessLogGatewayTests()
        {
            _mockContext = new Mock<IDatabaseContext>();
            _mockDbSet = new Mock<DbSet<NightlyProcessLog>>();

            // Setup DbSet in the mock context
            _mockContext.Setup(c => c.NightlyProcessLogs).Returns(_mockDbSet.Object);

            // Initialize the gateway with the mocked context
            _gateway = new NightlyProcessLogGateway(_mockContext.Object);
        }

        [Fact]
        public async Task UpdateDatabaseWithResultsShouldSaveToDatabaseAsFailWhenValidInputHasError()
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
        public async Task UpdateDatabaseWithResultsShouldThrowArgumentNullExceptionWhenLogGroupNameIsNull()
        {
            // Arrange
            string logGroupName = null;
            var queryResults = new List<List<ResultField>>();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _gateway.UpdateDatabaseWithResults(logGroupName, queryResults)).ConfigureAwait(false);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task UpdateDatabaseWithResultsShouldLogAndThrowWhenDbUpdateException()
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
        public async Task UpdateDatabaseWithResultsShouldAddFailureWhenErrorExists()
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
        public async Task UpdateDatabaseWithResultsShouldStopProcessingOnFirstError()
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
        public async Task UpdateDatabaseWithResultsCase1ResultsExistForKeyword()
        {
            // Arrange
            var logGroupName = "test-log-group";
            var queryResults = new List<List<ResultField>>
            {
                new List<ResultField>
                {
                    new ResultField { Field = "@timestamp", Value = DateTime.UtcNow.ToString("o") },
                    new ResultField { Field = "@message", Value = "Error occurred" }
                }
            };

            // Act
            await _gateway.UpdateDatabaseWithResults(logGroupName, queryResults).ConfigureAwait(false);

            // Assert
            _mockContext.Verify(x => x.NightlyProcessLogs.AddAsync(It.Is<NightlyProcessLog>(log =>
                log.LogGroupName == logGroupName &&
                log.IsSuccess == false), default), Times.Once);
            _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateDatabaseWithResultsCase2LogsExistButNoMatchForKeyword()
        {
            // Arrange
            var logGroupName = "test-log-group";
            var queryResults = new List<List<ResultField>>();

            // Act
            await _gateway.UpdateDatabaseWithResults(logGroupName, queryResults).ConfigureAwait(false);

            // Assert
            _mockContext.Verify(x => x.NightlyProcessLogs.AddAsync(It.Is<NightlyProcessLog>(log =>
                log.LogGroupName == logGroupName &&
                log.IsSuccess == true), default), Times.Once);
            _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateDatabaseWithResultsCase3NoLogsExistForLogGroup()
        {
            // Arrange
            var logGroupName = "test-log-group";
            List<List<ResultField>> queryResults = null;

            // Act
            await _gateway.UpdateDatabaseWithResults(logGroupName, queryResults).ConfigureAwait(false);

            // Assert
            _mockContext.Verify(x => x.NightlyProcessLogs.AddAsync(It.Is<NightlyProcessLog>(log =>
                log.LogGroupName == logGroupName &&
                log.IsSuccess == null), default), Times.Once);
            _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateDatabaseWithResultsCase4LogsExistButMandatorySuccessMessageMissing()
        {
            // Arrange
            var logGroupName = "test-log-group";
            var queryResults = new List<List<ResultField>>
            {
                new List<ResultField>
                {
                    // This matches Case 4 in your Use Case
                    new ResultField { Field = "@timestamp", Value = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff") },
                    new ResultField { Field = "@message", Value = $"error: Mandatory success message '{Constants.ProcessCompletedSuccessfullyMessage}' not found." }
                }
            };

            // Act
            await _gateway.UpdateDatabaseWithResults(logGroupName, queryResults).ConfigureAwait(false);

            // Assert
            _mockContext.Verify(x => x.NightlyProcessLogs.AddAsync(
                It.Is<NightlyProcessLog>(log =>
                    log.LogGroupName == logGroupName &&
                    log.IsSuccess == false),
                It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ProcessingMultipleLogGroupsSavesOneResultPerGroup()
        {
            // Arrange
            var firstLogGroup = "log-group-1";
            var secondLogGroup = "log-group-2";

            var results1 = new List<List<ResultField>> {
                new List<ResultField> {
                    new ResultField { Field = "@timestamp", Value = DateTime.UtcNow.ToString("o") },
                    new ResultField { Field = "@message", Value = "Error in group 1" }
                }
            };

            var results2 = new List<List<ResultField>> {
                new List<ResultField> {
                    new ResultField { Field = "@timestamp", Value = DateTime.UtcNow.ToString("o") },
                    new ResultField { Field = "@message", Value = "Error in group 2" }
                }
            };

            // Act
            await _gateway.UpdateDatabaseWithResults(firstLogGroup, results1).ConfigureAwait(false);
            await _gateway.UpdateDatabaseWithResults(secondLogGroup, results2).ConfigureAwait(false);

            // Assert
            _mockContext.Verify(x => x.NightlyProcessLogs.AddAsync(
                It.Is<NightlyProcessLog>(log => log.LogGroupName == firstLogGroup),
                It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);

            _mockContext.Verify(x => x.NightlyProcessLogs.AddAsync(
                It.Is<NightlyProcessLog>(log => log.LogGroupName == secondLogGroup),
                It.IsAny<System.Threading.CancellationToken>()),
                Times.Once);

            _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Exactly(2));
        }
    }
}
