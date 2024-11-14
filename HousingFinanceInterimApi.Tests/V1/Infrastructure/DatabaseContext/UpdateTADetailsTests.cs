using AutoFixture;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.Infrastructure.DatabaseContext
{
    public class UpdateTADetailsTests : IClassFixture<BaseContextTest>
    {
        private readonly HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext _context;
        private readonly Fixture _fixture;
        private readonly List<Action> _cleanups;
        private readonly Action<string> _executeProcedure;

        public UpdateTADetailsTests(BaseContextTest baseContextTest)
        {
            _context = baseContextTest._context;
            _fixture = baseContextTest._fixture;
            _cleanups = baseContextTest._cleanups;
            _executeProcedure = baseContextTest.ExecuteProcedure;
        }

        [Fact]
        public async void Should_Update_TADetails_With_Values_Requested()
        {
            var propRef = TestDataGenerator.PropRef;
            var query = new UpdateTAQuery
            {
                PropertyReference = propRef
            };
            var request = new UpdateTARequest
            {
                TenureEndDate = DateTime.UtcNow,
            };
            var testClass = new UpdateTAGateway(_context);

            // Arrange
            var newUHTARow = _fixture.Build<UHTenancyAgreement>()
                .With(table => table.PropRef, propRef)
                .Create();

            var newMATARow = _fixture.Build<MATenancyAgreement>()
                .With(table => table.PropRef, propRef)
                .Create();

            _context.UHTenancyAgreement.Add(newUHTARow);
            _context.MATenancyAgreement.Add(newMATARow);
            _context.SaveChanges();

            _cleanups.Add(() =>
            {
                _context.UHTenancyAgreement.Remove(newUHTARow);
                _context.MATenancyAgreement.Remove(newMATARow);
                _context.SaveChanges();
            });

            // Act
            await testClass.UpdateTADetails(query, request).ConfigureAwait(false);

            // Assert
            var endDate = $"{request.TenureEndDate}";

            var newUHTenancyAgr = _context.UHTenancyAgreement.SingleOrDefault(p => p.PropRef == query.PropertyReference);
            Assert.NotNull(newUHTenancyAgr);
            Assert.Equal(newUHTenancyAgr.Eot, request.TenureEndDate);
            Assert.Equal(newUHTenancyAgr.Present, false);
            Assert.Equal(newUHTenancyAgr.Terminated, true);

            var newMATenancyAgr = _context.MATenancyAgreement.SingleOrDefault(p => p.PropRef == query.PropertyReference);
            Assert.NotNull(newMATenancyAgr);
            Assert.Equal(newMATenancyAgr.Eot, request.TenureEndDate);
            Assert.Equal(newMATenancyAgr.Present, true);
            Assert.Equal(newMATenancyAgr.Terminated, false);
        }
    }
}
