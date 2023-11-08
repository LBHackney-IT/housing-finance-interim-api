using System;
using System.Threading.Tasks;
using FluentAssertions;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using Moq;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.UseCase
{
    public class LoadHousingFileTransactionsUseCaseTests
    {
        private readonly Mock<IBatchLogGateway> _mockBatchLogGateway = new();
        private readonly Mock<IBatchLogErrorGateway> _mockBatchLogErrorGateway = new();
        private readonly Mock<IUPHousingCashLoadGateway> _mockUpHousingCashLoadGateway = new();
        private readonly Mock<ITransactionGateway> _mockTransactionGateway = new();
        private readonly ILoadHousingFileTransactionsUseCase _classUnderTest;

        public LoadHousingFileTransactionsUseCaseTests()
        {
            _classUnderTest = new LoadHousingFileTransactionsUseCase(
                _mockBatchLogGateway.Object,
                _mockBatchLogErrorGateway.Object,
                _mockUpHousingCashLoadGateway.Object,
                _mockTransactionGateway.Object);
        }

        [Fact]
        public async Task ShouldLoadHousingFilesAndReturnStepResponse()
        {
            // Arrange
            var waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
            var batchId = 1;
            _mockBatchLogGateway.Setup(x => x.CreateAsync(It.IsAny<string>(), false))
                                .ReturnsAsync(new BatchLogDomain { Id = batchId });

            // Act
            var result = await _classUnderTest.ExecuteAsync().ConfigureAwait(true);

            // Assert
            result.Continue.Should().BeTrue();
            result.NextStepTime.Should().BeCloseTo(DateTime.Now.AddSeconds(int.Parse(waitDuration)));
            
            _mockBatchLogGateway.Verify(x => x.CreateAsync(It.IsAny<string>(), false), Times.Once);
            _mockUpHousingCashLoadGateway.Verify(x => x.LoadHousingFiles(), Times.Once);
            _mockTransactionGateway.Verify(x => x.LoadHousingFilesTransactions(), Times.Once);
            _mockBatchLogGateway.Verify(x => x.SetToSuccessAsync(batchId), Times.Once);
        }

        [Fact]
        public void ShouldLogErrorAndRethrowExceptionIfLoadHousingFilesThrowsException()
        {
            // Arrange
            var exception = new Exception("Test exception");
            _mockUpHousingCashLoadGateway.Setup(x => x.LoadHousingFiles()).ThrowsAsync(exception);

            // Act
            Func<Task> act = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(true);

            // Assert
            act.Should().Throw<Exception>().WithMessage(exception.Message);
            _mockBatchLogGateway.Verify(x => x.CreateAsync(It.IsAny<string>(), false), Times.Once);
            _mockBatchLogErrorGateway.Verify(x => x.CreateAsync(It.IsAny<long>(), "ERROR", "Application error. Not possible to load housing files transactions"),
                                             Times.Once);
        }
    }
}