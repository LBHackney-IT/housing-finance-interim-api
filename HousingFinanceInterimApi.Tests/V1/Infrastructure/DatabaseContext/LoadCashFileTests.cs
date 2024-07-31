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
using HousingFinanceInterimApi.V1.Exceptions;

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
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");
            _context = baseContextTest._context;
            _cleanups = baseContextTest._cleanups;

            _fixture.Customize<UPCashDumpFileName>(composer => composer
                .Without(x => x.Id)
                .Without(x => x.Timestamp)
            );

            var tablesToClear = new List<Type> { typeof(UPCashDumpFileName), typeof(UPCashDump), typeof(UPCashLoad) };
            ClearTable.ClearTables(_context, tablesToClear);
            _cleanups.Add(() => ClearTable.ClearTables(_context, tablesToClear));
        }



        [Fact]
        public async void Given_A_Valid_UPCashDump_Creates_A_Matching_UPCashLoad()
        {
            // Arrange
            var rentAccount = TestDataGen.RentAccount();
            var paymentSource = CashDumpTestData.PaymentSource();
            var amountPaid = CashDumpTestData.AmountPaid();
            var paymentDate = CashDumpTestData.PaymentDate();
            var transactionType = CashDumpTestData.TransactionType();
            var civicaCode = CashDumpTestData.CivicaCode();
            var fullText = CashDumpTestData.FullTextBuild(
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

            // Assert
            var matchingCashLoad = _context.UpCashLoads.Where(x => x.UPCashDumpId == cashDump.Id).First();
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
        [InlineData(false, true)]
        [InlineData(true, false)]
        public async void Given_An_Invalid_Cash_Dump__Does_Not_Create_UPCashLoad(bool fileNameIsSuccessful,
            bool cashDumpNotAlreadyRead)
        {
            // Arrange
            var testClass = new UPCashLoadGateway(_context);

            var cashDumpFileName = _fixture.Build<UPCashDumpFileName>()
                .Without(x => x.Id)
                .Without(x => x.Timestamp)
                .With(x => x.IsSuccess, fileNameIsSuccessful)
                .Create();
            _context.UpCashDumpFileNames.Add(cashDumpFileName);
            _context.SaveChanges();

            var cashDump = _fixture.Build<UPCashDump>()
                .Without(x => x.Id)
                .With(x => x.UpCashDumpFileName, cashDumpFileName)
                .With(x => x.FullText, CashDumpTestData.FullText())
                .With(x => x.IsRead, !cashDumpNotAlreadyRead)
                .Create();
            _context.Add(cashDump);
            _context.SaveChanges();

            // Act
            await testClass.LoadCashFiles().ConfigureAwait(false);

            // Assert
            var matchingCashLoad = _context.UpCashLoads.Where(x => x.UPCashDumpId == cashDump.Id).FirstOrDefault();
            Assert.Null(matchingCashLoad);
        }

        [Theory]
        [InlineData("invalid rent account")]
        [InlineData("invalid payment source")]
        public async void Given_A_UPCashDump_With_Invalid_FullText__ThrowsError(string fullText)
        {
            // Arrange
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
            async Task Act() => await testClass.LoadCashFiles().ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<InvalidCashFileTextException>(Act).ConfigureAwait(false);
        }
    }
}
