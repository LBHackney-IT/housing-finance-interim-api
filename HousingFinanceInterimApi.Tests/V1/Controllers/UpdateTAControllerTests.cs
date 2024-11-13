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
    public class UpdateTAControllerTests
    {
        private readonly UpdateTAController _classUnderTest;

        private readonly Mock<IUpdateTAGateway> _gateway;
        private readonly Fixture _fixture = new Fixture();

        public UpdateTAControllerTests()
        {
            _gateway = new Mock<IUpdateTAGateway>();

            _classUnderTest = new UpdateTAController(_gateway.Object);
        }

        [Fact]
        public async Task UpdateTA_WhenValidRequest_ReturnsNoContent()
        {
            // Arrange
            var query = _fixture.Create<UpdateTAQuery>();
            var request = _fixture.Create<UpdateTARequest>();

            // Act
            var response = await _classUnderTest.UpdateTA(query, request)
                .ConfigureAwait(false);

            // Assert
            _gateway
                .Verify(x => x.UpdateTADetails(query, request), Times.Once);

            response.Should().BeEquivalentTo(new NoContentResult());
        }
    }
}
