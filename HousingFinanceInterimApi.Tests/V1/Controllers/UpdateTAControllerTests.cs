using AutoFixture;
using FluentAssertions;
using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Controllers;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.Controllers
{
    public class TenancyAgreementControllerTests
    {
        private readonly TenancyAgreementController _classUnderTest;

        private readonly Mock<IUpdateTAGateway> _gateway;
        private readonly Fixture _fixture = new Fixture();

        public TenancyAgreementControllerTests()
        {
            _gateway = new Mock<IUpdateTAGateway>();

            _classUnderTest = new TenancyAgreementController(_gateway.Object);
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
            _gateway
                .Verify(x => x.UpdateTADetails(query, request), Times.Once);

            response.Should().BeEquivalentTo(new NoContentResult());
        }
    }
}
