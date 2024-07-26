using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Infrastructure;
using Xunit;
using Bogus;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using System.Threading.Tasks;

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

            var tablesToClear = new List<Type> {
                typeof(UPCashDumpFileName), typeof(UPCashDump), typeof(UPCashLoad)
            };
            ClearTable.ClearTables(_context, tablesToClear);
            _cleanups.Add(() => ClearTable.ClearTables(_context, tablesToClear ));
        }

        // For data that's specific to this test
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

            public static string FullTextBuild(string rentAccount, string paymentSource, string amountPaid, string paymentDate, string transactionType, string civicaCode) =>
                $"{rentAccount}{paymentSource}".PadRight(30) + $"{transactionType}+{amountPaid}{paymentDate}{civicaCode}";

            public static string FullText() =>
                FullTextBuild(TestDataGenerator.RentAccount(), PaymentSource(), AmountPaid(), PaymentDate(), TransactionType(), CivicaCode());
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
            var fullText = DataGen.FullTextBuild(
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

        /// <summary>
        /// Given an invalid cash dump
        ///     which has an unsuccessful filename
        ///     or which is already read
        /// Then does not create UPCashLoad
        /// </summary>
        /// <param name="rentAccount"></param>
        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        public async void Given_An_Invalid_Cash_Dump__Does_Not_Create_UPCashLoad(bool fileNameIsSuccess, bool cashDumpIsRead)
        {
            // Arrange
            var testClass = new UPCashLoadGateway(_context);

            var cashDumpFileName = _fixture.Build<UPCashDumpFileName>()
                .Without(x => x.Id)
                .Without(x => x.Timestamp)
                .With(x => x.IsSuccess, fileNameIsSuccess)
                .Create();
            _context.UpCashDumpFileNames.Add(cashDumpFileName);
            _context.SaveChanges();

            var cashDump = _fixture.Build<UPCashDump>()
                .Without(x => x.Id)
                .With(x => x.UpCashDumpFileName, cashDumpFileName)
                .With(x => x.FullText, DataGen.FullText())
                .With(x => x.IsRead, cashDumpIsRead)
                .Create();
            _context.Add(cashDump);
            _context.SaveChanges();

            // Act
            await testClass.LoadCashFiles().ConfigureAwait(false);
            
            // Assert
            var matchingCashLoad = _context.UPCashLoads.Where(x => x.UPCashDumpId == cashDump.Id).FirstOrDefault();
            Assert.Null(matchingCashLoad);
        }
        [Fact]
        public async void Given_A_UPCashDump_With_Invalid_FullText__ThrowsError()
        {
            // Arrange
            var testClass = new UPCashLoadGateway(_context);

            var cashDumpFileName = _fixture.Create<UPCashDumpFileName>();
            _context.UpCashDumpFileNames.Add(cashDumpFileName);
            _context.SaveChanges();

            var cashDump = _fixture.Build<UPCashDump>()
                .Without(x => x.Id)
                .With(x => x.UpCashDumpFileName, cashDumpFileName)
                .With(x => x.FullText, "invalid full text")
                .Create();
            _context.Add(cashDump);
            _context.SaveChanges();

            // Act
            async Task Act() => await testClass.LoadCashFiles().ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<Microsoft.Data.SqlClient.SqlException>(Act).ConfigureAwait(false);
            
        }
    }

}
