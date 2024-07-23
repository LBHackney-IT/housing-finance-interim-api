using AutoFixture;
using System.Collections.Generic;
using System;
using Xunit;
using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Infrastructure;
using HousingFinanceInterimApi.V1.Gateways;
using System.Linq;

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
            var propRef = GeneratePropRef();
            var query = new UpdateAssetDetailsQuery();
            query.PropertyReference = propRef;

            var request = new UpdateAssetDetailsRequest();
            request.AddressLine1 = "100 Acacia Avenue";
            request.PostPreamble = "33 Mountain View Court";

            var testClass = new AssetGateway(_context);


            // Arrange
            var newMaPropertyTableRow = _fixture.Build<MAProperty>()
                .With(table => table.PropRef, propRef)
                .With(table => table.Address1, "Default Address 1 value")
                .With(table => table.Ownership, "DefaultOwn")
                .With(table => table.PostPreamble, "Default Postpreamble value")
                .With(table => table.Agent, "Age")
                .With(table => table.Agent, "Are")
                .With(table => table.AreaOffice, "Are")
                .With(table => table.CatType, "Cat")
                .With(table => table.OccStatus, "Occ")
                .With(table => table.ArrPatch, "Arr")
                .With(table => table.HouseRef, "1234567890")
                .With(table => table.MajorRef, "majorred00")
                .With(table => table.ManScheme, "manscheme8")
                .With(table => table.PostCode, "n115tg")
                .With(table => table.NumBedrooms, 3)
                .With(table => table.PropertySid, 5000)
                .With(table => table.SubtypeCode, "SC1")
                .With(table => table.Telephone, "898987845")
                .Create();           

            var newUhPropertyTableRow = _fixture.Build<UHProperty>()
                .With(table => table.PropRef, propRef)
                .With(table => table.Address1, "Default Address 1 value")
                .With(table => table.Ownership, "DefaultOwn")
                .With(table => table.PostPreamble, "Default Postpreamble value")
                .With(table => table.Agent, "Age")
                .With(table => table.Agent, "Are")
                .With(table => table.AreaOffice, "Are")
                .With(table => table.CatType, "Cat")
                .With(table => table.OccStatus, "Occ")
                .With(table => table.ArrPatch, "Arr")
                .With(table => table.HouseRef, "1234567890")
                .With(table => table.MajorRef, "majorred00")
                .With(table => table.ManScheme, "manscheme8")
                .With(table => table.PostCode, "n115tg")
                .With(table => table.NumBedrooms, 3)
                .With(table => table.PropertySid, 5000)
                .With(table => table.SubtypeCode, "SC1")
                .With(table => table.Telephone, "898987845")
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
            var newMAProperty = _context.MAProperty.SingleOrDefault(p => p.PropRef == query.PropertyReference);
            var newUHProperty = _context.UHProperty.SingleOrDefault(p => p.PropRef == query.PropertyReference);

            var address1 = $"{request.PostPreamble} {request.AddressLine1}";

            Assert.NotNull(newMAProperty);         
            Assert.Equal(newMAProperty.PostPreamble, request.PostPreamble);
            Assert.Equal(newMAProperty.Address1, address1);
            Assert.Equal(newMAProperty.ShortAddress, address1);

            Assert.NotNull(newUHProperty);
            Assert.Equal(newUHProperty.PostPreamble, request.PostPreamble);
            Assert.Equal(newUHProperty.Address1, address1);
            Assert.Equal(newUHProperty.ShortAddress, address1);
        }

        private string GeneratePropRef()
        {
            // 8 Digit random number that leads with 9 to prevent clashes e.g. "90023871"
            return string.Concat("9", _fixture.Create<int>().ToString().PadLeft(7, '0').AsSpan(0, 7));
        }
    }
}
