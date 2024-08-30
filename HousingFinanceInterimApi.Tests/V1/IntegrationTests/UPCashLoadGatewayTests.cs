using AutoFixture;
using Bogus;
using System.Collections.Generic;
using System;
using HousingFinanceInterimApi.Tests.V1.DatabaseContextFixtures;
using HousingFinanceInterimApi.V1.Infrastructure;
using Xunit;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using HousingFinanceInterimApi.V1.Gateways;
using System.Linq;
using HousingFinanceInterimApi.V1.Exceptions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HousingFinanceInterimApi.Tests.V1.IntegrationTests
{
    public class UPCashLoadGatewayTests : IClassFixture<DatabaseFixtureFactory>
    {
        private readonly DatabaseFixtureFactory _databaseFixtureFactory;
        private DatabaseContext _context;
        private readonly Fixture _fixture;
        private readonly Faker _faker;
        private readonly List<Action> _cleanups;

        public UPCashLoadGatewayTests(DatabaseFixtureFactory databaseFixtureFactory)
        {
            _databaseFixtureFactory = databaseFixtureFactory;
            _fixture = _databaseFixtureFactory.Fixture;
            _faker = _databaseFixtureFactory.Faker;
            _cleanups = _databaseFixtureFactory.Cleanups;
        }

        [Theory]
        [InlineData(ConstantsGen.POSTGRESRDS)]
        [InlineData(ConstantsGen.SQLSERVERRDS)]
        public async void Given_A_Valid_UPCashDump_Creates_A_Matching_UPCashLoad(string database)
        {
            var _databaseFixture = _databaseFixtureFactory.CreateFixture(database);
            _context = _databaseFixture.CreateDbContext();

            // Arrange
            var rentAccount = TestDataGenerator.RentAccount();
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
            cashDumpFileName.Id = null;

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
        [InlineData(ConstantsGen.POSTGRESRDS, false, true)]
        //[InlineData(ConstantsGen.POSTGRESRDS, true, false)]
        //[InlineData(ConstantsGen.SQLSERVERRDS, false, true)]
        //[InlineData(ConstantsGen.SQLSERVERRDS, true, false)]
        public async void Given_An_Invalid_Cash_Dump__Does_Not_Create_UPCashLoad(
            string database, 
            bool fileNameIsSuccessful,
            bool cashDumpNotAlreadyRead)
        {
            var _databaseFixture = _databaseFixtureFactory.CreateFixture(database);
            _context = _databaseFixture.CreateDbContext();

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
        //[InlineData(ConstantsGen.POSTGRESRDS, "invalid rent account")]
        [InlineData(ConstantsGen.POSTGRESRDS, "invalid payment source")]
        //[InlineData(ConstantsGen.SQLSERVERRDS, "invalid rent account")]
        //[InlineData(ConstantsGen.SQLSERVERRDS, "invalid payment source")]
        public async void Given_A_UPCashDump_With_Invalid_FullText__ThrowsError(string database, string fullText)
        {
            var _databaseFixture = _databaseFixtureFactory.CreateFixture(database);
            _context = _databaseFixture.CreateDbContext();

            // Arrange
            var testClass = new UPCashLoadGateway(_context);

            var cashDumpFileName = _fixture.Create<UPCashDumpFileName>();
            cashDumpFileName.Id = null;
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
