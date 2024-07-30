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

            var cashDumpFileName = _fixture.Create<UPCashDumpFileName>();
            _context.UpCashDumpFileNames.Add(cashDumpFileName);
            _context.SaveChanges();   

            var cashDump = _fixture.Build<UPCashDump>()
                .Without(x => x.Id)
                .With(x => x.UpCashDumpFileName, cashDumpFileName)
                .With(x => x.FullText, _fixture.Create<string>())
                .Create();
            _context.Add(cashDump);
            _context.SaveChanges();

            var cashLoad = _fixture.Build<UPCashLoad>()
                .Without(x => x.Id)
                .With(x => x.UpCashDump, cashDump)
                .Create();
            _context.UpCashLoads.Add(cashLoad);
            _context.SaveChanges();

            var reportStartDate = DateTime.Now - TimeSpan.FromDays(1);
            var reportEndDate = DateTime.Now;

            // Act
            var result = await testClass.GetCashImportByDateAsync(reportStartDate, reportEndDate).ConfigureAwait(false);

            // Assert
            Assert.NotNull(result);
        }
    }
}
