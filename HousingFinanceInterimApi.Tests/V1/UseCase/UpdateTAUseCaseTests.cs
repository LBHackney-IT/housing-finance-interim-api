using AutoFixture;
using FluentAssertions;
using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Factories;
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
                                  .Create();
            var domain = request.ToDomain();
            _mockGateway.Setup(x => x.UpdateTADetails("01234/02", domain));

            await _classUnderTest.ExecuteAsync("01234/01", request).ConfigureAwait(false);

            domain.IsTerminated.Should().BeTrue();
            domain.IsPresent.Should().BeFalse();
        }

        [Fact]
        public async Task DateIsNull()
        {
            var request = _fixture.Build<UpdateTARequest>()
                                  .Without(x => x.TenureEndDate)
                                  .Create();
            var domain = request.ToDomain();
            _mockGateway.Setup(x => x.UpdateTADetails("01234/02", domain));

            await _classUnderTest.ExecuteAsync("01234/01", request).ConfigureAwait(false);

            domain.IsTerminated.Should().BeFalse();
            domain.IsPresent.Should().BeTrue();
        }

        [Fact]
        public async Task DateInFuture()
        {
            var endDateFuture = DateTime.UtcNow.AddDays(3);
            var request = _fixture.Build<UpdateTARequest>()
                                  .With(x => x.TenureEndDate, endDateFuture)
                                  .Create();
            var domain = request.ToDomain();
            _mockGateway.Setup(x => x.UpdateTADetails("01234/02", domain));

            await _classUnderTest.ExecuteAsync("01234/01", request).ConfigureAwait(false);

            domain.IsTerminated.Should().BeFalse();
            domain.IsPresent.Should().BeTrue();
        }

        [Fact]
        public async Task DateIsDefaultValue()
        {
            var endDateFuture = "1900-01-01T00:00:00.0000000Z";
            var request = _fixture.Build<UpdateTARequest>()
                                  .With(x => x.TenureEndDate, DateTime.Parse(endDateFuture))
                                  .Create();
            var domain = request.ToDomain();
            _mockGateway.Setup(x => x.UpdateTADetails("01234/02", domain));

            await _classUnderTest.ExecuteAsync("01234/01", request).ConfigureAwait(false);

            domain.IsTerminated.Should().BeFalse();
            domain.IsPresent.Should().BeTrue();
        }
    }
}
