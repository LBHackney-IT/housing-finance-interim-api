using AutoFixture;
using HousingFinanceInterimApi.Tests.V1.Helper;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Gateways;
using FluentAssertions;
using HousingFinanceInterimApi.V1.Infrastructure;
using NUnit.Framework;

namespace HousingFinanceInterimApi.Tests.V1.Gateways
{
    //TODO: Rename Tests to match gateway name
    //For instruction on how to run tests please see the wiki: https://github.com/LBHackney-IT/lbh-base-api/wiki/Running-the-test-suite.
    [TestFixture]
    public class ExampleGatewayTests : DatabaseTests
    {
        private readonly Fixture _fixture = new Fixture();
        private ExampleGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new ExampleGateway(DatabaseContext);
        }

        //[Test]
        public void GetEntityByIdReturnsNullIfEntityDoesntExist()
        {
            Entity response = _classUnderTest.GetEntityById(123);

            response.Should().BeNull();
        }

        //[Test]
        public void GetEntityByIdReturnsTheEntityIfItExists()
        {
            Entity entity = _fixture.Create<Entity>();
            DatabaseEntity databaseEntity = DatabaseEntityHelper.CreateDatabaseEntityFrom(entity);

            DatabaseContext.DatabaseEntities.Add(databaseEntity);
            DatabaseContext.SaveChanges();

            Entity response = _classUnderTest.GetEntityById(databaseEntity.Id);

            databaseEntity.Id.Should().Be(response.Id);
            databaseEntity.CreatedAt.Should().BeSameDateAs(response.CreatedAt);
        }

        //TODO: Add tests here for the get all method.
    }
}
