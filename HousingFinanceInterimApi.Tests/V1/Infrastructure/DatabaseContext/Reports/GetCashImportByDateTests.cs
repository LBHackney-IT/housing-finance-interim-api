using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Bogus;
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

    [Fact]
    public async void ShouldGetCashImportByDate()
    {
        // Arrange
        var testClass = new ReportGateway(_context);

        var cashDumpFileName = _fixture.Build<UPCashDumpFileName>()
            .Without(x => x.Id)
            .With(x => x.FileName, "CashFile20240727.dat")
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

        var ssminiDict = new Dictionary<string, SSMiniTransaction>();
        var rentGroups = new List<string> { "GPS", "HGF", "HRA", "LMW", "LSC", "TAG", "TAH", "TRA", "ZZZZZZ", "SSSSSS" };

        foreach (var rentGroup in rentGroups)
        {
            var ssmini = _fixture.Build<SSMiniTransaction>()
                .With(x => x.PostDate, DateTime.Now.Date - TimeSpan.FromDays(4))
                .With(x => x.OriginDesc, "Cash File")
                .With(x => x.RentGroup, rentGroup[..3])
                .With(x => x.RealValue, _fixture.Create<decimal>())
                .Create();
            _context.Add(ssmini);
            ssminiDict.Add(rentGroup, ssmini);
        }
        _context.SaveChanges();

        var reportStartDate = DateTime.Now - TimeSpan.FromDays(7);
        var reportEndDate = DateTime.Now;

        // Act
        var reportCashImport = await testClass.GetCashImportByDateAsync(reportStartDate, reportEndDate).ConfigureAwait(false);

        // Assert
        Assert.NotNull(reportCashImport);

        // Parse list of lists csv to list of objects
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

        Assert.Single(reportData);
        var reportItem = reportData[0];

        var expectedDateString = (DateTime.Now.Date - TimeSpan.FromDays(4)).ToString("dd/MM/yyyy");
        Assert.Equal(expectedDateString, reportItem["Date"]);

        var expectedIfsTotal = ssminiDict.Values.Sum(x => x.RealValue);
        Assert.Equal(expectedIfsTotal, decimal.Parse((string) reportItem["IFSTotal"]));
        Assert.Equal(-cashLoad.AmountPaid, decimal.Parse((string) reportItem["FileTotal"]));

        foreach (var rentGroup in rentGroups)
            Assert.Equal(ssminiDict[rentGroup].RealValue, decimal.Parse((string) reportItem[rentGroup]));
    }
}
