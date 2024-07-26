using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Infrastructure;
using Xunit;
using Bogus;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;

namespace HousingFinanceInterimApi.Tests.V1.Infrastructure.DatabaseContext
{
    public class LoadCashFileTests : IClassFixture<BaseContextTest>
    {
        private readonly HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext _context;
        private static readonly Fixture _fixture = new();
        private static readonly Faker _faker = new();
        private readonly List<Action> _cleanups;

        public LoadCashFileTests(BaseContextTest baseContextTest)
        {
            _context = baseContextTest._context;
            _cleanups = baseContextTest._cleanups;

            _fixture.Customize<UPCashDumpFileName>(composer => composer
                .Without(x => x.Id)
                .Without(x => x.Timestamp)
            );

            _context.RemoveRange(_context.UpCashDumps);
            _context.RemoveRange(_context.UpCashDumpFileNames);
            _cleanups.Add(() => _context.RemoveRange(_context.UpCashDumps));
            _cleanups.Add(() => _context.RemoveRange(_context.UpCashDumpFileNames));
            _cleanups.Add(() => _context.RemoveRange(_context.UPCashLoads));
        }

        private static class DataGen
        {
            public static string PaymentSource() =>
                _faker.Random.Word().Replace(" ", "").PadRight(10)[..10].ToUpper();

            public static string AmountPaid() =>
                _faker.Random.Decimal(0, 1000).ToString().PadLeft(9, '0')[..9];

            public static string PaymentDate() =>
                _faker.Date.Past().ToString("dd/MM/yyyy");

            public static string TransactionType() =>
                _faker.Random.AlphaNumeric(3).ToUpper();

            public static string CivicaCode() =>
                _faker.Random.Number(1, 99).ToString().PadLeft(2, '0');

            public static string FullText(string rentAccount, string paymentSource, string amountPaid, string paymentDate, string transactionType, string civicaCode) =>
                $"{rentAccount}{paymentSource}".PadRight(30) + $"{transactionType}+{amountPaid}{paymentDate}{civicaCode}";
        }


        [Fact]
        public async void Given_A_Valid_UPCashDump_Creates_A_Matching_UPCashLoad()
        {
            // Arrange
            var rentAccount = TestDataGenerator.RentAccount();
            var paymentSource = DataGen.PaymentSource();
            var amountPaid = DataGen.AmountPaid();
            var paymentDate = DataGen.PaymentDate();
            var transactionType = DataGen.TransactionType();
            var civicaCode = DataGen.CivicaCode();
            // var fullText = $"{rentAccount}{paymentSource}".PadRight(30) + $"{transactionType}+{amountPaid}{paymentDate}{civicaCode}";
            var fullText = DataGen.FullText(
                rentAccount, paymentSource, amountPaid, paymentDate, transactionType, civicaCode
            );

            var testClass = new UPCashLoadGateway(_context);

            var cashDumpFileName = _fixture.Create<UPCashDumpFileName>();
            _context.UpCashDumpFileNames.Add(cashDumpFileName);
            _context.SaveChanges();

            var cashDump = _fixture.Build<UPCashDump>()
                .Without(x => x.Id)
                .With(x => x.UpCashDumpFileName, cashDumpFileName)
                .With(x => x.FullText, fullText)
                .Create();
            _context.Add(cashDump);
            _context.SaveChanges();

            // Act
            await testClass.LoadCashFiles().ConfigureAwait(false);
            var matchingCashLoad = _context.UPCashLoads.Where(x => x.UPCashDumpId == cashDump.Id).First();

            // Assert
            Assert.NotNull(matchingCashLoad);
            Assert.Equal(rentAccount, matchingCashLoad.RentAccount);
            Assert.Equal(paymentSource.Trim(), matchingCashLoad.PaymentSource);
            Assert.Equal(transactionType, matchingCashLoad.MethodOfPayment);
            Assert.Equal(Math.Round(decimal.Parse(amountPaid), 2), matchingCashLoad.AmountPaid);
            Assert.Equal(DateTime.Parse(paymentDate).Date, matchingCashLoad.DatePaid.Date);
            Assert.Equal(civicaCode, matchingCashLoad.CivicaCode);
            Assert.Equal(cashDump.Id, matchingCashLoad.UPCashDumpId);
            Assert.Equal(DateTime.Today.Date, matchingCashLoad.Timestamp.Date);


            _context.Entry(cashDump).Reload();
            Assert.True(cashDump.IsRead);
        }
    }
}
