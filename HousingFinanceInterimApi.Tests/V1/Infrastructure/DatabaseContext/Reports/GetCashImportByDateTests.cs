using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Infrastructure;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.Infrastructure.DatabaseContext.Reports;

public class GetCashImportByDateTests : IClassFixture<BaseContextTest>
{
    private readonly HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext _context;
    private readonly Fixture _fixture;

    public GetCashImportByDateTests(BaseContextTest baseContextTest)
    {
        _context = baseContextTest._context;
        var cleanups = baseContextTest._cleanups;
        _fixture = new Fixture();

        _fixture.Customize<UPCashDumpFileName>(composer => composer
            .Without(x => x.Id)
            .Without(x => x.Timestamp)
        );

        var tablesToClear = new List<Type>
        {
            typeof(SSMiniTransaction), typeof(UPCashDumpFileName), typeof(UPCashDump), typeof(UPCashLoad)
        };
        ClearTable.ClearTables(_context, tablesToClear);
        cleanups.Add(() => ClearTable.ClearTables(_context, tablesToClear));
    }

    /*
     * Given a list of cash loads and linked SSMiniTransactions
     * When GetCashImportByDate is called with a date range
     * Then it should return a list of cash imports by date
     *     within the date range
     *     with the total value of transactions for each rent group
     *     with summed totals across all rent groups
     */
    [Fact]
    public async void ShouldGetCashImportByDate()
    {
        // Arrange
        var testClass = new ReportGateway(_context);

        var ReportStartDate = DateTime.Now - TimeSpan.FromDays(7);
        var ReportEndDate = DateTime.Now;

        // Within the report date range
        var testDatesThisWeek = new List<DateTime>
        {
            DateTime.Now.Date - TimeSpan.FromDays(4),
            DateTime.Now.Date - TimeSpan.FromDays(3)
        };

        var allCashLoads = new List<UPCashLoad>();
        var allSsminis = new List<SSMiniTransaction>();
        var rentGroups = new List<string> { "GPS", "HGF", "HRA", "LMW", "LSC", "TAG", "TAH", "TRA", "ZZZZZZ", "SSSSSS" };

        foreach (var testDate in testDatesThisWeek)
        {
            var cashDumpFileNameFileName = $"CashFile{testDate:yyyyMMdd}.dat";
            var cashDumpFileName = _fixture.Build<UPCashDumpFileName>()
                .Without(x => x.Id)
                .With(x => x.FileName, cashDumpFileNameFileName)
                .Create();
            _context.UpCashDumpFileNames.Add(cashDumpFileName);
            _context.SaveChanges();

            var cashDump = _fixture.Build<UPCashDump>()
                .Without(x => x.Id)
                .With(x => x.UpCashDumpFileName, cashDumpFileName)
                .With(x => x.FullText, CashDumpTestData.FullText())
                .Create();
            _context.Add(cashDump);
            _context.SaveChanges();

            var cashLoad = _fixture.Build<UPCashLoad>()
                .Without(x => x.Id)
                .With(x => x.UpCashDump, cashDump)
                .Create();
            _context.UpCashLoads.Add(cashLoad);
            _context.SaveChanges();
            allCashLoads.Add(cashLoad);

            foreach (var rentGroup in rentGroups)
                foreach (var isSuspense in new List<bool> { true, false })
                {
                    var ssminiPre = _fixture.Build<SSMiniTransaction>()
                        .With(x => x.PostDate, testDate)
                        .With(x => x.OriginDesc, "Cash File")
                        .With(x => x.RentGroup, rentGroup[..3]);
                    if (isSuspense)
                        ssminiPre.With(x => x.TagRef, "SSSSSS");

                    var ssmini = ssminiPre.Create();
                    _context.Add(ssmini);
                    allSsminis.Add(ssmini);
                }
            _context.SaveChanges();
        }

        // Act
        IList<string[]> reportCashImport = await testClass.GetCashImportByDateAsync(ReportStartDate, ReportEndDate).ConfigureAwait(false);

        // Assert
        Assert.NotNull(reportCashImport);

        // Parse list of lists table to list of dicts to allow key access by header
        var header = reportCashImport[0];
        var data = reportCashImport.Skip(1).ToList();
        var reportData = new List<Dictionary<string, object>>();
        foreach (var row in data)
        {
            var rowData = new Dictionary<string, object>();
            for (var i = 0; i < header.Length; i++)
                rowData.Add(header[i], row[i]);
            reportData.Add(rowData);
        }

        // One row created per cash load date
        Assert.Equal(testDatesThisWeek.Count, reportData.Count);
        foreach (var reportItem in reportData)
        {
            var relatedSsminis = allSsminis
                .Where(x => x.PostDate == DateTime.ParseExact(s: (string) reportItem["Date"], format: "dd/MM/yyyy", null))
                .ToList();

            var expectedIfsTotal = relatedSsminis
                .Sum(x => x.RealValue);
            Assert.Equal(expectedIfsTotal, decimal.Parse((string) reportItem["IFSTotal"]));

            // File total is set from the related cash load
            var matchingCashLoads = allCashLoads
                .Where(x => x.AmountPaid == -decimal.Parse((string) reportItem["FileTotal"]))
                .ToList();
            Assert.Single(matchingCashLoads);

            // Report should have one col per rent group with the total value of transactions for that group
            foreach (var rentGroup in rentGroups)
                Assert.Equal(
                    relatedSsminis.Where(x => x.RentGroup == rentGroup[..3] && x.TagRef != "SSSSSS")
                        .Sum(x => x.RealValue),
                    decimal.Parse((string) reportItem[rentGroup])
                    );
        }
    }
}
