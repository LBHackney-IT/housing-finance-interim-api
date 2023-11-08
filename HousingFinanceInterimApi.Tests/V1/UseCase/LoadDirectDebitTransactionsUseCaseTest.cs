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
    public class LoadDirectDebitTransactionsUseCaseTest
    {
        private readonly Mock<IBatchLogGateway> _mockBatchLogGateway;
        private readonly Mock<IBatchLogErrorGateway> _mockBatchLogErrorGateway;
        private readonly Mock<IDirectDebitGateway> _mockDirectDebitGateway;
        private readonly Mock<ITransactionGateway> _mockTransactionGateway;

        private readonly ILoadDirectDebitTransactionsUseCase _classUnderTest;
        private readonly double _waitDuration = 30;

        public LoadDirectDebitTransactionsUseCaseTest()
        {
            Environment.SetEnvironmentVariable("WAIT_DURATION", _waitDuration.ToString());

            _mockBatchLogGateway = new Mock<IBatchLogGateway>();
            _mockBatchLogErrorGateway = new Mock<IBatchLogErrorGateway>();
            _mockDirectDebitGateway = new Mock<IDirectDebitGateway>();
            _mockTransactionGateway = new Mock<ITransactionGateway>();

            _classUnderTest = new LoadDirectDebitTransactionsUseCase(
                _mockBatchLogGateway.Object,
                _mockBatchLogErrorGateway.Object,
                _mockDirectDebitGateway.Object,
                _mockTransactionGateway.Object);

            _mockBatchLogGateway
                .Setup(x => x.CreateAsync(It.IsAny<string>(), false))
                .ReturnsAsync(new BatchLogDomain { Id = 1 });
        }

        [Fact]
        public async Task ExecuteAsyncShouldLoadDirectDebitTransactionsAndReturnStepResponse()
        {
            // Arrange
            var processingDate = DateTime.UtcNow;

            // Act
            var result = await _classUnderTest.ExecuteAsync(processingDate).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Continue.Should().BeTrue();
            result.NextStepTime.Should().BeCloseTo(DateTime.Now.AddSeconds(_waitDuration));

            _mockBatchLogGateway.Verify(x => x.CreateAsync(It.IsAny<string>(), false), Times.Once);
            _mockTransactionGateway.Verify(x => x.LoadDirectDebitTransactions(), Times.Once);
            _mockBatchLogGateway.Verify(x => x.SetToSuccessAsync(It.IsAny<long>()), Times.Once);
        }

        [Fact]
        public void ExecuteAsyncShouldCatchExceptionAndThrow()
        {
            // Arrange
            var processingDate = DateTime.UtcNow;
            var testException = new Exception("Test Exception");

            _mockTransactionGateway
                .Setup(x => x.LoadDirectDebitTransactions())
                .ThrowsAsync(testException);

            // Act + Assert
            _classUnderTest.Invoking(x => x.ExecuteAsync(processingDate))
                           .Should()
                           .Throw<Exception>()
                           .WithMessage(testException.Message);
            _mockBatchLogErrorGateway.Verify(x => x.CreateAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task ExecuteOnDemandAsyncShouldLoadDirectDebitTransactionsAndReturnStepResponse(int dayDifference)
        {
            // Arrange
            var startDate = DateTime.Now.Date;
            var endDate = startDate.AddDays(dayDifference);
            var loopCount = dayDifference + 1;  // +1 because it includes the endDate

            // Act
            var result = await _classUnderTest.ExecuteOnDemandAsync(startDate, endDate).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Continue.Should().BeTrue();
            result.NextStepTime.Should().BeCloseTo(DateTime.Now.AddSeconds(_waitDuration));

            _mockBatchLogGateway.Verify(x => x.CreateAsync(It.IsAny<string>(), false), Times.Once);
            _mockDirectDebitGateway.Verify(x => x.LoadDirectDebitHistory(It.IsAny<DateTime>()), Times.Exactly(loopCount));
            _mockTransactionGateway.Verify(x => x.LoadDirectDebitTransactions(), Times.Exactly(loopCount));
            _mockBatchLogGateway.Verify(x => x.SetToSuccessAsync(It.IsAny<long>()), Times.Once);
        }

        [Fact]
        public void ExecuteOnDemandAsyncShouldCatchExceptionAndThrow()
        {
            // Arrange
            var startDate = DateTime.UtcNow.Date;
            var endDate = startDate.AddDays(1);
            var testException = new Exception("Test Exception");

            _mockDirectDebitGateway
                .Setup(x => x.LoadDirectDebitHistory(startDate))
                .ThrowsAsync(testException);

            // Act
            _classUnderTest.Invoking(x => x.ExecuteOnDemandAsync(startDate, endDate))
                           .Should()
                           .Throw<Exception>()
                           .WithMessage(testException.Message);
            _mockBatchLogErrorGateway.Verify(x => x.CreateAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
