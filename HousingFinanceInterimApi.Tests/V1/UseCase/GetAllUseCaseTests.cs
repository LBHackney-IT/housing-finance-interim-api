using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.UseCase;
using FluentAssertions;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using Moq;
using NUnit.Framework;

namespace HousingFinanceInterimApi.Tests.V1.UseCase
{
    public class GetAllUseCaseTests
    {
        private Mock<IExampleGateway> _mockGateway;
        private GetAllUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IExampleGateway>();
            _classUnderTest = new GetAllUseCase(_mockGateway.Object);
            _fixture = new Fixture();
        }

        [Test]
        public void GetsAllFromTheGateway()
        {
            List<Entity> stubbedEntities = _fixture.CreateMany<Entity>().ToList();
            _mockGateway.Setup(x => x.GetAll()).Returns(stubbedEntities);

            ResponseObjectList expectedResponse = new ResponseObjectList { ResponseObjects = stubbedEntities.ToResponse() };

            _classUnderTest.Execute().Should().BeEquivalentTo(expectedResponse);
        }

        //TODO: Add extra tests here for extra functionality added to the use case
    }
}
