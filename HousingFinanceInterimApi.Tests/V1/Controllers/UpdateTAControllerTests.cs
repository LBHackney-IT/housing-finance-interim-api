using AutoFixture;
using FluentAssertions;
using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Controllers;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.Controllers
{
    public class TenancyAgreementControllerTests
    {
        private readonly TenancyAgreementController _classUnderTest;

        private readonly Mock<IUpdateTAUseCase> _useCase;
        private readonly Fixture _fixture = new Fixture();

        public TenancyAgreementControllerTests()
        {
            _useCase = new Mock<IUpdateTAUseCase>();

            _classUnderTest = new TenancyAgreementController(_useCase.Object);
        }

        [Fact]
        public async Task UpdateTA_WhenValidRequest_ReturnsNoContent()
        {
            // Arrange
            var query = _fixture.Create<string>();
            var request = _fixture.Create<UpdateTARequest>();

            // Act
            var response = await _classUnderTest.DynamoDbStreamTrigger(query, request)
                .ConfigureAwait(false);

            // Assert
            _useCase.Verify(x => x.ExecuteAsync(query, request), Times.Once);

            response.Should().BeEquivalentTo(new NoContentResult());
        }
    }
}
