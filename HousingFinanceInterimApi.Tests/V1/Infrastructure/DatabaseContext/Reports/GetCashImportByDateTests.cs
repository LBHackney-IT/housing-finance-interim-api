using System;
using System.Collections.Generic;
using AutoFixture;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Infrastructure;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.Infrastructure.DatabaseContext
{
    public class GetCashImportByDateTests : IClassFixture<BaseContextTest>
    {
        private readonly HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext _context;
        private readonly Fixture _fixture;
        private readonly List<Action> _cleanups;

        public GetCashImportByDateTests(BaseContextTest baseContextTest)
        {
            _context = baseContextTest._context;
            _cleanups = baseContextTest._cleanups;
            _fixture = new Fixture();

            _fixture.Customize<UPCashDumpFileName>(composer => composer
                .Without(x => x.Id)
                .Without(x => x.Timestamp)
            );

            var tablesToClear = new List<Type> {
                typeof(SSMiniTransaction), typeof(UPCashDumpFileName), typeof(UPCashDump), typeof(UPCashLoad)
            };
            ClearTable.ClearTables(_context, tablesToClear);
            _cleanups.Add(() => ClearTable.ClearTables(_context, tablesToClear));

        }

        [Fact]
        public async void ShouldGetCashImportByDate()
        {
            // Arrange
            var testClass = new ReportGateway(_context);

            var reportStartDate = DateTime.Now - TimeSpan.FromDays(1);
            var reportEndDate = DateTime.Now;

            // Act
            var result = await testClass.GetCashImportByDateAsync(reportStartDate, reportEndDate).ConfigureAwait(false);

            // Assert
            Assert.NotNull(result);
        }
    }
}
