using AutoFixture;
using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System;
using Xunit;
using FluentAssertions;
using HousingFinanceInterimApi.Tests.V1.Infrastructure.DatabaseContext;
using HousingFinanceInterimApi.V1.Factories;

namespace HousingFinanceInterimApi.Tests.V1.Infrastructure
{
    public class UpdateTADetailsTests : DatabaseTests
    {
        private readonly HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext _context;
        private readonly Fixture _fixture;
        private readonly List<Action> _cleanups;
        private readonly Action<string> _executeProcedure;
        private readonly BaseContextTest _contextTest;

        public UpdateTADetailsTests()
        {
            _context = _contextTest._context;
            _fixture = _contextTest._fixture;
            _cleanups = _contextTest._cleanups;
            _executeProcedure = _contextTest.ExecuteProcedure;
        }

        [Fact]
        public async void Should_Update_TADetails_With_Values_Requested()
        {
            var othertagRef = "01234/07";
            var tagRef = "0123/02";
            var request = new UpdateTARequest
            {
                TenureEndDate = DateTime.UtcNow.AddDays(-12),
            }.ToDomain();
            var testClass = new UpdateTAGateway(_context);

            // Arrange
            var newUHTARow = _fixture.Build<UHTenancyAgreement>()
                .With(table => table.TenancyAgreementRef, tagRef)
                .Create();

            var newMATARow = _fixture.Build<MATenancyAgreement>()
                .With(table => table.TenancyAgreementRef, tagRef)
                .Create();
            var anotherUHTRow = _fixture.Build<UHTenancyAgreement>()
                .With(table => table.TenancyAgreementRef, othertagRef)
                .Without(table => table.EndOfTenure)
                .Create();
            var anotherMATRow = _fixture.Build<MATenancyAgreement>()
                .With(table => table.TenancyAgreementRef, othertagRef)
                .Without(table => table.EndOfTenure)
                .Create();

            _context.UHTenancyAgreement.Add(newUHTARow);
            _context.MATenancyAgreement.Add(newMATARow);
            _context.UHTenancyAgreement.Add(anotherUHTRow);
            _context.MATenancyAgreement.Add(anotherMATRow);
            _context.SaveChanges();

            _cleanups.Add(() =>
            {
                _context.UHTenancyAgreement.Remove(newUHTARow);
                _context.MATenancyAgreement.Remove(newMATARow);
                _context.UHTenancyAgreement.Remove(anotherUHTRow);
                _context.MATenancyAgreement.Remove(anotherMATRow);
                _context.SaveChanges();
            });

            // Act
            await testClass.UpdateTADetails(tagRef, request).ConfigureAwait(false);

            // Assert
            var endDate = $"{request.TenureEndDate}";

            var newUHTenancyAgr = _context.UHTenancyAgreement.SingleOrDefault(p => p.TenancyAgreementRef == tagRef);
            Assert.NotNull(newUHTenancyAgr);
            Assert.Equal(newUHTenancyAgr.EndOfTenure, request.TenureEndDate);
            newUHTenancyAgr.IsPresent.Should().BeFalse();
            newUHTenancyAgr.IsTerminated.Should().BeTrue();

            var newMATenancyAgr = _context.MATenancyAgreement.SingleOrDefault(p => p.TenancyAgreementRef == tagRef);
            Assert.NotNull(newMATenancyAgr);
            Assert.Equal(newMATenancyAgr.EndOfTenure, request.TenureEndDate);
            newMATenancyAgr.IsPresent.Should().BeFalse();
            newMATenancyAgr.IsTerminated.Should().BeTrue();


            //data should be the same as before
            var noChangeUHT = _context.UHTenancyAgreement.SingleOrDefault(p => p.TenancyAgreementRef == othertagRef);
            Assert.NotNull(noChangeUHT);
            noChangeUHT.EndOfTenure.Should().BeNull();
            noChangeUHT.IsPresent.Should().BeTrue();
            noChangeUHT.IsTerminated.Should().BeFalse();

            var noChangeMAT = _context.MATenancyAgreement.SingleOrDefault(p => p.TenancyAgreementRef == othertagRef);
            Assert.NotNull(noChangeMAT);
            noChangeMAT.EndOfTenure.Should().BeNull();
            noChangeUHT.IsPresent.Should().BeTrue();
            noChangeUHT.IsTerminated.Should().BeFalse();

        }
    }
}
