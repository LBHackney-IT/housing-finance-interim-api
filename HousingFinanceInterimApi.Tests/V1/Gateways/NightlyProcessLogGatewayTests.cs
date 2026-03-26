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
        public async Task UpdateDatabaseWithResults_Case2_LogsExistButNoMatchForKeyword()
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
        public async Task UpdateDatabaseWithResults_Case3_NoLogsExistForLogGroup()
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
        public async Task UpdateDatabaseWithResults_Case4_LogsExistButMandatorySuccessMessageMissing()
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
        public async Task UpdateDatabaseWithResults_IgnoresOlderErrors_IfMostRecentIsSuccess()
        {
            // Arrange
            var logGroupName = "test-log-group";
            var today = DateTime.UtcNow;
            var sequence = new MockSequence();

            var errorResults = new List<List<ResultField>>
            { 
                new List<ResultField>
                {
                    new ResultField { Field = "@timestamp", Value = today.AddMinutes(-10).ToString("o") },
                    new ResultField { Field = "@message", Value = "Initial Critical Error" }
                }
            };

            var successResults = new List<List<ResultField>>();

            NightlyProcessLog lastLogSaved = null; 

            _mockDbSet.Setup(x => x.AddAsync(It.IsAny<NightlyProcessLog>(), It.IsAny<System.Threading.CancellationToken>()))
                .Callback<NightlyProcessLog, System.Threading.CancellationToken>((log, token) => 
                {
                    lastLogSaved = log;
                })
                .ReturnsAsync((NightlyProcessLog l, System.Threading.CancellationToken c) => null);

            // 2. Act
            await _gateway.UpdateDatabaseWithResults(logGroupName, errorResults).ConfigureAwait(false);
            await _gateway.UpdateDatabaseWithResults(logGroupName, successResults).ConfigureAwait(false);

            // 3. Assert
            Assert.NotNull(lastLogSaved);
        
            Assert.True(lastLogSaved.IsSuccess);
            
            Assert.Equal(logGroupName, lastLogSaved.LogGroupName);
        }

        [Fact]
        public async Task UpdateDatabaseWithResults_SavesError_IfMostRecentIsError()
        {
            // Arrange
            var logGroupName = "test-log-group";
            var today = DateTime.UtcNow;
            var sequence = new MockSequence();

            var errorResults = new List<List<ResultField>>
            { 
                new List<ResultField>
                {
                    new ResultField { Field = "@timestamp", Value = today.ToString("o") },
                    new ResultField { Field = "@message", Value = "Initial Critical Error" }
                }
            };

            var successResults = new List<List<ResultField>>();

            NightlyProcessLog lastLogSaved = null; 

            _mockDbSet.Setup(x => x.AddAsync(It.IsAny<NightlyProcessLog>(), It.IsAny<System.Threading.CancellationToken>()))
                .Callback<NightlyProcessLog, System.Threading.CancellationToken>((log, token) => 
                {
                    lastLogSaved = log;
                })
                .ReturnsAsync((NightlyProcessLog l, System.Threading.CancellationToken c) => null);

            // Act
            await _gateway.UpdateDatabaseWithResults(logGroupName, successResults).ConfigureAwait(false);
            await _gateway.UpdateDatabaseWithResults(logGroupName, errorResults).ConfigureAwait(false);

            // Assert
            Assert.NotNull(lastLogSaved);
        
            Assert.False(lastLogSaved.IsSuccess);
            
            Assert.Equal(logGroupName, lastLogSaved.LogGroupName);
        }

        [Fact]
        public async Task ProcessingMultipleLogGroups_SavesMostRecentResultPerGroup()
        {
            // Arrange
            var firstLogGroup = "log-group-1";
            var secondLogGroup = "log-group-2";
            var today = DateTime.UtcNow;
            var sequence = new MockSequence();
            var finalStates = new Dictionary<string, bool>();

            var errorResults = new List<List<ResultField>>
            { 
                new List<ResultField>
                {
                    new ResultField { Field = "@timestamp", Value = today.ToString("o") },
                    new ResultField { Field = "@message", Value = "Initial Critical Error" }
                }
            };

            var successResults = new List<List<ResultField>>();

            // SCRIPT STEP 1 & 2: Group 1 goes from Error to Success
            _mockDbSet.InSequence(sequence).Setup(x => x.AddAsync(It.Is<NightlyProcessLog>(l => l.LogGroupName == firstLogGroup && l.IsSuccess.GetValueOrDefault() == false), It.IsAny<System.Threading.CancellationToken>()))
                .Callback<NightlyProcessLog, System.Threading.CancellationToken>((l, t) => finalStates[firstLogGroup] = l.IsSuccess.GetValueOrDefault())
                .ReturnsAsync((NightlyProcessLog l, System.Threading.CancellationToken c) => null);

            _mockDbSet.InSequence(sequence).Setup(x => x.AddAsync(It.Is<NightlyProcessLog>(l => l.LogGroupName == firstLogGroup && l.IsSuccess.GetValueOrDefault() == true), It.IsAny<System.Threading.CancellationToken>()))
                .Callback<NightlyProcessLog, System.Threading.CancellationToken>((l, t) => finalStates[firstLogGroup] = l.IsSuccess.GetValueOrDefault())
                .ReturnsAsync((NightlyProcessLog l, System.Threading.CancellationToken c) => null);

            // SCRIPT STEP 3 & 4: Group 2 goes from Error to Success
            _mockDbSet.InSequence(sequence).Setup(x => x.AddAsync(It.Is<NightlyProcessLog>(l => l.LogGroupName == secondLogGroup && l.IsSuccess.GetValueOrDefault() == true), It.IsAny<System.Threading.CancellationToken>()))
                .Callback<NightlyProcessLog, System.Threading.CancellationToken>((l, t) => finalStates[secondLogGroup] = l.IsSuccess.GetValueOrDefault())
                .ReturnsAsync((NightlyProcessLog l, System.Threading.CancellationToken c) => null);

            _mockDbSet.InSequence(sequence).Setup(x => x.AddAsync(It.Is<NightlyProcessLog>(l => l.LogGroupName == secondLogGroup && l.IsSuccess.GetValueOrDefault() == false), It.IsAny<System.Threading.CancellationToken>()))
                .Callback<NightlyProcessLog, System.Threading.CancellationToken>((l, t) => finalStates[secondLogGroup] = l.IsSuccess.GetValueOrDefault())
                .ReturnsAsync((NightlyProcessLog l, System.Threading.CancellationToken c) => null);

            // Act
            await _gateway.UpdateDatabaseWithResults(firstLogGroup, errorResults).ConfigureAwait(false);
            await _gateway.UpdateDatabaseWithResults(firstLogGroup, successResults).ConfigureAwait(false);

            await _gateway.UpdateDatabaseWithResults(secondLogGroup, successResults).ConfigureAwait(false);
            await _gateway.UpdateDatabaseWithResults(secondLogGroup, errorResults).ConfigureAwait(false);

            // Assert
            _mockDbSet.Verify(x => x.AddAsync(It.IsAny<NightlyProcessLog>(), It.IsAny<System.Threading.CancellationToken>()), Times.Exactly(4));

            Assert.True(finalStates[firstLogGroup]);
            Assert.False(finalStates[secondLogGroup]);
        }
    }
}
