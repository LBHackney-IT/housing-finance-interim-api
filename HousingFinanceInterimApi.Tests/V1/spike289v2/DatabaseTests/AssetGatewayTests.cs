using AutoFixture;
using HousingFinanceInterimApi.Tests.V1.spike289v2.DatabaseContext;
using System.Collections.Generic;
using System;
using Xunit;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Linq;

namespace HousingFinanceInterimApi.Tests.V1.spike289v2.DatabaseTests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "This is a test class")]
    public class AssetGatewayTests : IClassFixture<DatabaseFixtureFactory>
    {
        private readonly DatabaseFixtureFactory _databaseFixtureFactory;

        private HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext _context;
        private Fixture _fixture;
        private List<Action> _cleanups;

        public AssetGatewayTests(DatabaseFixtureFactory databaseFixtureFactory)
        {
            _databaseFixtureFactory = databaseFixtureFactory;
            _fixture = _databaseFixtureFactory.Fixture;
            _cleanups = _databaseFixtureFactory.Cleanups;
        }

        [Theory]
        [InlineData(ConstantsGen.POSTGRESRDS)]
        [InlineData(ConstantsGen.SQLSERVERRDS)]
        public async void UpdateSqlServer_AssetDetails(string database)
        {
            var _databaseFixture = _databaseFixtureFactory.CreateFixture(database);
            _context = _databaseFixture.CreateDbContext();

            var propRef = TestDataGenerator.PropRef;
            var query = new UpdateAssetDetailsQuery
            {
                PropertyReference = propRef
            };
            var request = new UpdateAssetDetailsRequest
            {
                AddressLine1 = "100 Acacia Avenue",
                PostPreamble = "33 Mountain View Court"
            };
            var testClass = new AssetGateway(_context);

            // Arrange
            var newMaPropertyTableRow = _fixture.Build<MAProperty>()
                .With(table => table.PropRef, propRef)
                .Create();

            var newUhPropertyTableRow = _fixture.Build<UHProperty>()
                .With(table => table.PropRef, propRef)
                .Create();

            _context.MAProperty.Add(newMaPropertyTableRow);
            _context.UHProperty.Add(newUhPropertyTableRow);
            _context.SaveChanges();

            _cleanups.Add(() =>
            {
                _context.MAProperty.Remove(newMaPropertyTableRow);
                _context.UHProperty.Remove(newUhPropertyTableRow);
                _context.SaveChanges();
            });

            // Act
            await testClass.UpdateAssetDetails(query, request).ConfigureAwait(false);

            // Assert
            var address1 = $"{request.PostPreamble} {request.AddressLine1}";

            var newMAProperty = _context.MAProperty.SingleOrDefault(p => p.PropRef == query.PropertyReference);
            Assert.NotNull(newMAProperty);
            Assert.Equal(newMAProperty.PostPreamble, request.PostPreamble);
            Assert.Equal(newMAProperty.Address1, address1);
            Assert.Equal(newMAProperty.ShortAddress, address1);

            var newUHProperty = _context.UHProperty.SingleOrDefault(p => p.PropRef == query.PropertyReference);
            Assert.NotNull(newUHProperty);
            Assert.Equal(newUHProperty.PostPreamble, request.PostPreamble);
            Assert.Equal(newUHProperty.Address1, address1);
            Assert.Equal(newUHProperty.ShortAddress, address1);
        }
    }
}
