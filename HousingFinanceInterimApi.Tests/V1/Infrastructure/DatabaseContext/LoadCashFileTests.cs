using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.Infrastructure.DatabaseContext
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "This is a test class")]
    public class LoadCashFileTests : IClassFixture<BaseContextTest>
    {
        private readonly HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext _context;
        private readonly Fixture _fixture;
        private readonly List<Action> _cleanups;

        public LoadCashFileTests(BaseContextTest baseContextTest)
        {
            _context = baseContextTest._context;
            _fixture = baseContextTest._fixture;
            _cleanups = baseContextTest._cleanups;
        }

        [Fact]
        public async void Should_Load_Cash_File()
        {

            // Arrange
            var testClass = new UPCashLoadGateway(_context);

            _fixture.Customize<UPCashDumpFileName>(composer => composer
                .Without(x => x.Id)
                .Without(x => x.Timestamp)
            );
            _fixture.Customize<UPCashDump>(composer => composer
                .Without(x => x.Id)
                .Without(x => x.Timestamp)
            );

            var cashDumpFileName = _fixture.Create<UPCashDumpFileName>();
            _context.UpCashDumpFileNames.Add(cashDumpFileName);
            _context.SaveChanges();
            _cleanups.Add(() => _context.UpCashDumpFileNames.Remove(cashDumpFileName));

            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT UPCashDump ON");
            var cashDump = _fixture.Build<UPCashDump>()
                .With(cashDump => cashDump.UpCashDumpFileName, cashDumpFileName)
                .Create();
            _context.UpCashDumps.Add(cashDump);
            _context.SaveChanges();
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT UPCashDump OFF");
            _cleanups.Add(() => _context.UpCashDumps.Remove(cashDump));

            // Act
            await testClass.LoadCashFiles().ConfigureAwait(false);
            var matchingCashLoad = _context.UPCashLoads.Where(x => x.UPCashDumpId == cashDump.Id).First();
            _cleanups.Add(() => _context.UPCashLoads.Remove(matchingCashLoad));

            // Assert
            Assert.NotNull(matchingCashLoad);


        }
    }
}
