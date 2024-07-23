using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Infrastructure;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.Infrastructure.DatabaseContext
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "This is a test class")]
    public class AssetDetailsTests : IClassFixture<BaseContextTest>
    {
        private readonly HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext _context;
        private readonly Fixture _fixture;
        private readonly List<Action> _cleanups;
        private readonly Action<string> _executeProcedure;

        public AssetDetailsTests(BaseContextTest baseContextTest)
        {            
            _context = baseContextTest._context;
            _fixture = baseContextTest._fixture;
            _cleanups = baseContextTest._cleanups;
            _executeProcedure = baseContextTest.ExecuteProcedure;
        }

        [Fact]
        public async void Should_Update_AssetDetails_With_Values_Requested()
        {
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
