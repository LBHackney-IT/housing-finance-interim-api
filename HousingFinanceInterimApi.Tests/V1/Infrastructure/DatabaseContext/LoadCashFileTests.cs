using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Infrastructure;
using Xunit;
using Bogus;

namespace HousingFinanceInterimApi.Tests.V1.Infrastructure.DatabaseContext
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "This is a test class")]
    public class LoadCashFileTests : IClassFixture<BaseContextTest>
    {
        private readonly HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext _context;
        private readonly Fixture _fixture;
        private readonly Faker _faker;
        private readonly List<Action> _cleanups;

        public LoadCashFileTests(BaseContextTest baseContextTest)
        {
            _context = baseContextTest._context;
            _fixture = baseContextTest._fixture;
            _faker = baseContextTest._faker;
            _cleanups = baseContextTest._cleanups;

            _fixture.Customize<UPCashDumpFileName>(composer => composer
                .Without(x => x.Id)
                .Without(x => x.Timestamp)
            );
        }

        [Fact]
        public async void Should_Load_Cash_File()
        {
            _context.RemoveRange(_context.UpCashDumpFileNames);
            _context.RemoveRange(_context.UpCashDumps);
            _cleanups.Add(() => _context.UPCashLoads.RemoveRange(_context.UPCashLoads));
            _cleanups.Add(() => _context.UpCashDumps.RemoveRange(_context.UpCashDumps));

            var rentAccount = _faker.Random.Long(0, 9_999_999_999).ToString().PadLeft(10, '0');
            var paymentSource = _faker.Random.Word().Replace(" ", "").PadRight(10)[..10].ToUpper();
            var amountPaid = _faker.Random.Decimal(0, 1000).ToString().PadLeft(9, '0')[..9];
            var paymentDate = _faker.Date.Past().ToString("dd/MM/yyyy");
            var transactionType = _faker.Random.AlphaNumeric(3).ToUpper();
            var civicaCode = _faker.Random.Number(1, 99).ToString().PadLeft(2, '0');
            var fullText = $"{rentAccount}{paymentSource}".PadRight(30) + $"{transactionType}+{amountPaid}{paymentDate}{civicaCode}";

            // Arrange
            var testClass = new UPCashLoadGateway(_context);

            var cashDumpFileName = _fixture.Create<UPCashDumpFileName>();
            _context.UpCashDumpFileNames.Add(cashDumpFileName);
            _context.SaveChanges();

            var cashDump = _fixture.Build<UPCashDump>()
                .Without(cashDump => cashDump.Id)
                .With(cashDump => cashDump.UpCashDumpFileName, cashDumpFileName)
                .With(x => x.FullText, fullText)
                .Create();
            _context.Add(cashDump);
            _context.SaveChanges();

            // Act
            await testClass.LoadCashFiles().ConfigureAwait(false);
            var matchingCashLoad = _context.UPCashLoads.Where(x => x.UPCashDumpId == cashDump.Id).First();

            // Assert
            Assert.NotNull(matchingCashLoad);


        }
    }
}
