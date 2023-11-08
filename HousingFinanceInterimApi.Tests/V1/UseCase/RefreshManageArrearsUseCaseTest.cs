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
    public class RefreshManageArrearsUseCaseTest
    {
        private readonly Mock<IManageArrearsGateway> _mockManageArrearsGateway;

        private readonly IRefreshManageArrearsUseCase _classUnderTest;
        private readonly double _waitDuration = 30;

        public RefreshManageArrearsUseCaseTest()
        {
            Environment.SetEnvironmentVariable("WAIT_DURATION", _waitDuration.ToString());

            _mockManageArrearsGateway = new Mock<IManageArrearsGateway>();

            _classUnderTest = new RefreshManageArrearsUseCase(_mockManageArrearsGateway.Object);
        }

        [Fact]
        public async Task ExecuteAsyncShouldRefreshManageArrearsTenancyAgreementAndReturnStepResponse()
        {
            // Act
            var result = await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Continue.Should().BeTrue();
            result.NextStepTime.Should().BeCloseTo(DateTime.Now.AddSeconds(_waitDuration));

            _mockManageArrearsGateway.Verify(x => x.RefreshManageArrearsTenancyAgreement(), Times.Once);
        }

        [Fact]
        public void ExecuteAsyncShouldCatchExceptionAndThrow()
        {
            // Arrange
            var testException = new Exception("Test Exception");

            _mockManageArrearsGateway
                .Setup(x => x.RefreshManageArrearsTenancyAgreement())
                .ThrowsAsync(testException);

            // Act + Assert
            _classUnderTest.Invoking(x => x.ExecuteAsync())
                           .Should()
                           .Throw<Exception>()
                           .WithMessage(testException.Message);
        }
    }
}