using AutoFixture;
using FluentAssertions;
using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.UseCase
{
    public class UpdateTAUseCaseTests
    {
        private IUpdateTAUseCase _classUnderTest;
        private readonly Mock<IUpdateTAGateway> _mockGateway;
        private static Fixture _fixture = new Fixture();


        public UpdateTAUseCaseTests()
        {
            _mockGateway = new Mock<IUpdateTAGateway>();
            _classUnderTest = new UpdateTAUseCase(_mockGateway.Object);
        }
        
        [Fact]
        public async Task DateInPast()
        {
            var endDate = DateTime.UtcNow.AddDays(-1);
            var request = _fixture.Build<UpdateTARequest>()
                                  .With(x => x.TenureEndDate, endDate)
                                  .Without(x=> x.IsPresent)
                                  .Without(x=> x.IsTerminated)
                                  .Create();
            _mockGateway.Setup(x => x.UpdateTADetails("01234/02", request));

            await _classUnderTest.ExecuteAsync("01234/01", request).ConfigureAwait(false);

            request.IsTerminated.Should().BeTrue();
            request.IsPresent.Should().BeFalse();
        }

        [Fact]
        public async Task DateIsNull()
        {
            var request = _fixture.Build<UpdateTARequest>()
                                  .Without(x => x.TenureEndDate)
                                  .Without(x => x.IsPresent)
                                  .Without(x => x.IsTerminated)
                                  .Create();
            _mockGateway.Setup(x => x.UpdateTADetails("01234/02", request));

            await _classUnderTest.ExecuteAsync("01234/01", request).ConfigureAwait(false);

            request.IsTerminated.Should().BeFalse();
            request.IsPresent.Should().BeTrue();
        }

        [Fact]
        public async Task DateInFuture()
        {
            var endDateFuture = DateTime.UtcNow.AddDays(3);
            var request = _fixture.Build<UpdateTARequest>()
                                  .With(x => x.TenureEndDate, endDateFuture)
                                  .Without(x => x.IsPresent)
                                  .Without(x => x.IsTerminated)
                                  .Create();
            _mockGateway.Setup(x => x.UpdateTADetails("01234/02", request));

            await _classUnderTest.ExecuteAsync("01234/01", request).ConfigureAwait(false);

            request.IsTerminated.Should().BeFalse();
            request.IsPresent.Should().BeTrue();
        }

        [Fact]
        public async Task DateIsDefaultValue()
        {
            var endDateFuture = "1900-01-01 00:00:00.000";
            var request = _fixture.Build<UpdateTARequest>()
                                  .With(x => x.TenureEndDate, DateTime.Parse(endDateFuture))
                                  .Without(x => x.IsPresent)
                                  .Without(x => x.IsTerminated)
                                  .Create();
            _mockGateway.Setup(x => x.UpdateTADetails("01234/02", request));

            await _classUnderTest.ExecuteAsync("01234/01", request).ConfigureAwait(false);

            request.IsTerminated.Should().BeTrue();
            request.IsPresent.Should().BeFalse();
        }
    }
}
