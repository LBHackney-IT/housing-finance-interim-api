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
    public class RefreshOperatingBalanceUseCaseTests
    {
        private Mock<IOperatingBalanceGateway> _mockOperatingBalanceGateway;
        private IRefreshOperatingBalanceUseCase _classUnderTest;

        public RefreshOperatingBalanceUseCaseTests()
        {
            _mockOperatingBalanceGateway = new Mock<IOperatingBalanceGateway>();
            _classUnderTest = new RefreshOperatingBalanceUseCase(_mockOperatingBalanceGateway.Object);
        }

        [Fact]
        public async Task ShouldGenerateOperatingBalanceAndReturnStepResponse()
        {
            // Arrange
            var waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

            // Act
            var result = await _classUnderTest.ExecuteAsync().ConfigureAwait(true);

            // Assert
            result.Continue.Should().BeTrue();
            result.NextStepTime.Should().BeCloseTo(DateTime.Now.AddSeconds(int.Parse(waitDuration)));
            _mockOperatingBalanceGateway.Verify(x => x.GenerateOperatingBalance(), Times.Once);
        }

        [Fact]
        public void ShouldLogErrorAndRethrowExceptionIfGenerateOperatingBalanceThrowsException()
        {
            // Arrange
            var exception = new Exception("Test exception");
            _mockOperatingBalanceGateway.Setup(x => x.GenerateOperatingBalance()).ThrowsAsync(exception);

            // Act
            Func<Task> act = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(true);

            // Assert
            act.Should().Throw<Exception>().WithMessage(exception.Message);
            _mockOperatingBalanceGateway.Verify(x => x.GenerateOperatingBalance(), Times.Once);
        }
    }
}
