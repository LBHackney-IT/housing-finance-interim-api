using System;
using System.Threading.Tasks;
using FluentAssertions;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using Moq;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.UseCase
{
    public class RefreshCurrentBalanceUseCaseTest
    {
        private readonly Mock<ICurrentBalanceGateway> _mockCurrentBalanceGateway;

        private readonly IRefreshCurrentBalanceUseCase _classUnderTest;
        private readonly double _waitDuration = 30;

        public RefreshCurrentBalanceUseCaseTest()
        {
            Environment.SetEnvironmentVariable("WAIT_DURATION", _waitDuration.ToString());

            _mockCurrentBalanceGateway = new Mock<ICurrentBalanceGateway>();

            _classUnderTest = new RefreshCurrentBalanceUseCase(_mockCurrentBalanceGateway.Object);
        }

        [Fact]
        public async Task ExecuteAsyncShouldUpdateCurrentBalanceAndReturnStepResponse()
        {
            // Act
            var result = await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Continue.Should().BeTrue();
            result.NextStepTime.Should().BeCloseTo(DateTime.Now.AddSeconds(_waitDuration));

            _mockCurrentBalanceGateway.Verify(x => x.UpdateCurrentBalance(), Times.Once);
        }

        [Fact]
        public void ExecuteAsyncShouldCatchExceptionAndThrow()
        {
            // Arrange
            var testException = new Exception("Test Exception");

            _mockCurrentBalanceGateway
                .Setup(x => x.UpdateCurrentBalance())
                .ThrowsAsync(testException);

            // Act + Assert
            _classUnderTest.Invoking(x => x.ExecuteAsync())
                           .Should()
                           .Throw<Exception>()
                           .WithMessage(testException.Message);
        }
    }
}
