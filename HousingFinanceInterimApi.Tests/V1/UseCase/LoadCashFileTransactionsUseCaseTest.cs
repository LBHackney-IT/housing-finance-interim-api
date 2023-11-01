using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using Moq;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.UseCase
{
    public class LoadCashFileTransactionsUseCaseTests
    {
        private readonly Mock<IBatchLogGateway> _mockBatchLogGateway;
        private readonly Mock<IBatchLogErrorGateway> _mockBatchLogErrorGateway;
        private readonly Mock<IUPCashLoadGateway> _mockUpCashLoadGateway;
        private readonly Mock<ITransactionGateway> _mockTransactionGateway;
        private readonly ILoadCashFileTransactionsUseCase _classUnderTest;

        public LoadCashFileTransactionsUseCaseTests()
        {
            _mockBatchLogGateway = new Mock<IBatchLogGateway>();
            _mockBatchLogErrorGateway = new Mock<IBatchLogErrorGateway>();
            _mockUpCashLoadGateway = new Mock<IUPCashLoadGateway>();
            _mockTransactionGateway = new Mock<ITransactionGateway>();
            _classUnderTest = new LoadCashFileTransactionsUseCase(
                _mockBatchLogGateway.Object,
                _mockBatchLogErrorGateway.Object,
                _mockUpCashLoadGateway.Object,
                _mockTransactionGateway.Object);

            var batchLog = new BatchLogDomain();
            _mockBatchLogGateway.Setup(x => x.CreateAsync(It.IsAny<string>(), false))
                                .ReturnsAsync(batchLog);
        }

        [Fact]
        public async Task SuccessfullyImportsCashFileTransactions()
        {
            // Act
            var response = await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // Assert
            response.Should().NotBeNull();
            response.Continue.Should().BeTrue();
            response.NextStepTime.Should().BeCloseTo(DateTime.Now);

            _mockBatchLogGateway.Verify(x => x.CreateAsync(It.IsAny<string>(), false), Times.Once);
            _mockUpCashLoadGateway.Verify(x => x.LoadCashFiles(), Times.Once);
            _mockTransactionGateway.Verify(x => x.LoadCashFilesTransactions(), Times.Once);
            _mockBatchLogGateway.Verify(x => x.SetToSuccessAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void LogsExceptionsAndReThrows()
        {
            // Arrange
            var exception = new Exception("Test exception");
            _mockUpCashLoadGateway.Setup(x => x.LoadCashFiles()).Throws(exception);

            // Act
            Func<Task> act = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(true);

            // Assert
            act.Should().Throw<Exception>().WithMessage(exception.Message);
            _mockBatchLogGateway.Verify(x => x.CreateAsync(It.IsAny<string>(), false), Times.Once);
            _mockUpCashLoadGateway.Verify(x => x.LoadCashFiles(), Times.Once);
            _mockTransactionGateway.Verify(x => x.LoadCashFilesTransactions(), Times.Never);
            _mockBatchLogGateway.Verify(x => x.SetToSuccessAsync(It.IsAny<int>()), Times.Never);
            // _mockBatchLogErrorGateway.Verify(x => x.CreateAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
