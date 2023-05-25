using AutoFixture;
using AutoMapper;
using Castle.DynamicProxy.Generators;
using FluentAssertions;
using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Controllers;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.Controllers
{
    public class AssetControllerTests
    {
        private readonly AssetController _classUnderTest;

        private readonly Mock<IAssetGateway> _assetGatewayMock;
        private readonly Fixture _fixture = new Fixture();

        private readonly UpdateAssetDetailsQuery _query;

        public AssetControllerTests()
        {
            _assetGatewayMock = new Mock<IAssetGateway>();

            _classUnderTest = new AssetController(_assetGatewayMock.Object);

            _query = _fixture.Create<UpdateAssetDetailsQuery>();
        }

        [Fact]
        public async Task UpdateAssetDetails_WhenAddressLine1NullOrEmpty_ReturnsBadRequest()
        {
            // Arrange
            var request = _fixture.Build<UpdateAssetDetailsRequest>()
                .Without(x => x.AddressLine1)
                .Create();

            // Act
            var response = await _classUnderTest.UpdateAssetDetails(_query, request)
                .ConfigureAwait(false);

            // Assert
            response.Should().BeEquivalentTo(new BadRequestObjectResult($"The value for {nameof(request.AddressLine1)} cannot be empty"));
        }

        [Fact]
        public async Task UpdateAssetDetails_WhenPostCodeNullOrEmpty_ReturnsBadRequest()
        {
            // Arrange
            var request = _fixture.Build<UpdateAssetDetailsRequest>()
                .Without(x => x.PostCode)
                .Create();

            // Act
            var response = await _classUnderTest.UpdateAssetDetails(_query, request)
                .ConfigureAwait(false);

            // Assert
            response.Should().BeEquivalentTo(new BadRequestObjectResult($"The value for {nameof(request.PostCode)} cannot be empty"));
        }

        [Fact]
        public async Task UpdateAssetDetails_WhenValidRequest_ReturnsNoContent()
        {
            // Arrange
            var request = _fixture.Create<UpdateAssetDetailsRequest>();

            // Act
            var response = await _classUnderTest.UpdateAssetDetails(_query, request)
                .ConfigureAwait(false);

            // Assert
            _assetGatewayMock
                .Verify(x => x.UpdateAssetDetails(_query, request), Times.Once);

            response.Should().BeEquivalentTo(new NoContentResult());
        }
    }
}
