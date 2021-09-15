using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The database context class.
    /// </summary>
    /// <seealso cref="DbContext" />
    public class DatabaseContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OperatingBalance>().HasNoKey().ToView(null);
            modelBuilder.Entity<Payment>().HasNoKey().ToView(null);
            modelBuilder.Entity<Tenancy>().HasNoKey().ToView(null);
            modelBuilder.Entity<TenancyTransaction>().HasNoKey().ToView(null);
            modelBuilder.Entity<Transaction>().HasNoKey().ToView(null);
            modelBuilder.Entity<ChargesAux>().Property(x => x.TimeStamp).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<DirectDebitAux>().Property(x => x.Timestamp).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<ActionDiaryAux>().Property(x => x.Timestamp).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<TenancyAgreementAux>().Property(x => x.TimeStamp).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<UPCashDump>().Property(x => x.Timestamp).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<UPHousingCashDump>().Property(x => x.Timestamp).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Adjustment>().Property(x => x.Timestamp).HasDefaultValueSql("GETDATE()");
        }

        public DatabaseContext(DbContextOptions options)
            : base(options) { }
        public DbSet<GoogleFileSetting> GoogleFileSettings { get; set; }
        public DbSet<BatchLog> BatchLogs { get; set; }
        public DbSet<BatchLogError> BatchLogErrors { get; set; }
        public DbSet<ChargesAux> ChargesAux { get; set; }
        public DbSet<UPCashDumpFileName> UpCashDumpFileNames { get; set; }
        public DbSet<UPCashDump> UpCashDumps { get; set; }
        public DbSet<DirectDebitAux> DirectDebitsAux { get; set; }
        public DbSet<ActionDiaryAux> ActionsDiaryAux { get; set; }
        public DbSet<UPHousingCashDumpFileName> UpHousingCashDumpFileNames { get; set; }
        public DbSet<UPHousingCashDump> UpHousingCashDumps { get; set; }
        public DbSet<TenancyAgreementAux> TenancyAgreementsAux { get; set; }
        public DbSet<UPCashLoadSuspenseAccounts> UPCashLoadSuspenseAccounts { get; set; }
        public DbSet<UPHousingCashLoadSuspenseAccounts> UPHousingCashLoadSuspenseAccounts { get; set; }
        private DbSet<TenancyTransaction> TenancyTransactionValue { get; set; }
        private DbSet<Transaction> TransactionsValue { get; set; }
        private DbSet<OperatingBalance> OperatingBalancesValue { get; set; }
        private DbSet<Payment> PaymentsValue { get; set; }
        private DbSet<Tenancy> TenanciesValue { get; set; }
        private DbSet<Adjustment> Adjustments { get; set; }

        public async Task<IList<OperatingBalance>> GetOperatingBalancesAsync(DateTime? startDate, DateTime? endDate, int startWeek, int startYear, int endWeek, int endYear)
            => await OperatingBalancesValue
                .FromSqlInterpolated($"usp_GetOperatingBalancesByRentGroup {startDate:yyyy-MM-dd}, {endDate:yyyy-MM-dd}, {startWeek}, {startYear}, {endWeek}, {endYear}")
                .ToListAsync()
                .ConfigureAwait(false);

        public async Task<IList<Transaction>> GetTransactionsAsync(DateTime? startDate, DateTime? endDate)
            => await TransactionsValue
                .FromSqlInterpolated($"usp_GetTransactions {startDate:yyyy-MM-dd}, {endDate:yyyy-MM-dd}")
                .ToListAsync()
                .ConfigureAwait(false);

        public async Task<IList<Payment>> GetPaymentsAsync(string tenancyAgreementRef, string rentAccount, string householdRef, int count, string order)
              => await PaymentsValue
                  .FromSqlInterpolated($"usp_GetTenantLastPayments {tenancyAgreementRef}, {rentAccount}, {householdRef}, {count}, {order}")
                  .ToListAsync()
                  .ConfigureAwait(false);

        public async Task<IList<Tenancy>> GetTenanciesAsync(string tenancyAgreementRef, string rentAccount, string householdRef)
            => await TenanciesValue
                .FromSqlInterpolated($"usp_GetTenancyDetails {tenancyAgreementRef}, {rentAccount}, {householdRef}")
                .ToListAsync()
                .ConfigureAwait(false);

        public async Task<IList<TenancyTransaction>> GetTenancyTransactionsAsync(string tenancyAgreementRef, string rentAccount, string householdRef, int count, string order)
            => await TenancyTransactionValue
                .FromSqlInterpolated($"usp_GetTenancyTransactions {tenancyAgreementRef}, {rentAccount}, {householdRef}, {count}, {order}")
                .ToListAsync()
                .ConfigureAwait(false);

        public async Task<IList<TenancyTransaction>> GetTenancyTransactionsByDateAsync(string tenancyAgreementRef, string rentAccount, string householdRef, DateTime startDate, DateTime endDate)
            => await TenancyTransactionValue
                .FromSqlInterpolated($"usp_GetTenancyTransactionsByDate {tenancyAgreementRef}, {rentAccount}, {householdRef}, {startDate}, {endDate}")
                .ToListAsync()
                .ConfigureAwait(false);

        public async Task<List<string[]>> GetRentPosition()
        {
            var results = new List<string[]>();

            var dbConnection = Database.GetDbConnection() as SqlConnection;
            var command = new SqlCommand($"dbo.usp_GetRentPosition", dbConnection);
            command.CommandTimeout = 300;

            dbConnection.Open();
            await using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
            {
                var columnNames = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    columnNames[i] = reader.GetName(i);
                }

                while (reader.Read())
                {
                    var result = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        result[i] = reader[i].ToString();
                    }

                    results.Add(result);
                }
            }
            dbConnection.Close();

            return results;
        }

        public async Task LoadCashFiles()
            => await PerformTransaction("usp_LoadCashFile", 600).ConfigureAwait(false);

        public async Task LoadHousingFiles()
            => await PerformTransaction("usp_LoadHousingFile", 600).ConfigureAwait(false);

        public async Task LoadCashFileTransactions()
            => await PerformTransaction("usp_LoadTransactionsCashFile", 600).ConfigureAwait(false);

        public async Task LoadChargesTransactions()
            => await PerformTransaction("usp_LoadTransactionsCharges", 600).ConfigureAwait(false);

        public async Task LoadHousingFileTransactions()
            => await PerformTransaction("usp_LoadTransactionsHousingFile", 600).ConfigureAwait(false);

        public async Task LoadDirectDebitHistory(DateTime? processingDate)
            => await PerformInterpolatedTransaction($"usp_LoadDirectDebitHistory {processingDate:yyyy-MM-dd}", 600).ConfigureAwait(false);

        public async Task LoadChargesHistory(DateTime? processingDate)
            => await PerformInterpolatedTransaction($"usp_LoadChargesHistory {processingDate:yyyy-MM-dd}", 600).ConfigureAwait(false);

        public async Task LoadDirectDebitTransactions()
            => await PerformTransaction($"usp_LoadTransactionsDirectDebit", 600).ConfigureAwait(false);

        public async Task UpdateCurrentBalance()
            => await PerformTransaction("usp_UpdateCurrentBalance", 300).ConfigureAwait(false);

        public async Task RefreshManageArrearsTenancyAgreement()
            => await PerformTransaction("usp_RefreshManageArrearsTenancyAgreement", 300).ConfigureAwait(false);

        public async Task LoadDirectDebit(long batchLogId)
            => await PerformTransaction($"usp_LoadDirectDebit {batchLogId}", 300).ConfigureAwait(false);

        public async Task LoadCharges()
            => await PerformTransaction($"usp_LoadCharges", 300).ConfigureAwait(false);

        public async Task LoadActionDiary()
            => await PerformTransaction($"usp_LoadActionDiary", 300).ConfigureAwait(false);

        public async Task LoadAdjustmentTransactions()
            => await PerformTransaction("usp_LoadTransactionsAdjustment", 600).ConfigureAwait(false);

        public async Task RefreshTenancyAgreementTables(long batchLogId)
            => await PerformTransaction($"usp_RefreshTenancyAgreement {batchLogId}", 600).ConfigureAwait(false);

        public async Task GenerateOperatingBalance()
            => await PerformTransaction("usp_GenerateOperatingBalance", 600).ConfigureAwait(false);

        public async Task CreateCashFileSuspenseAccountTransaction(long id, string newRentAccount)
            => await PerformInterpolatedTransaction($"usp_UpdateCashFileSuspenseAccountResolved {id}, {newRentAccount}").ConfigureAwait(false);

        public async Task CreateHousingFileSuspenseAccountTransaction(long id, string newRentAccount)
            => await PerformInterpolatedTransaction($"usp_UpdateHousingCashFileSuspenseAccountResolved {id}, {newRentAccount}").ConfigureAwait(false);

        public async Task TruncateDirectDebitAuxiliary()
        {
            var sql = "DELETE FROM DirectDebitAux";
            await PerformTransaction(sql).ConfigureAwait(false);
        }

        public async Task TruncateChargesAuxiliary()
        {
            var sql = "DELETE FROM ChargesAux";
            await PerformTransaction(sql).ConfigureAwait(false);
        }

        public async Task TruncateTenancyAgreementAuxiliary()
        {
            var sql = "DELETE FROM TenancyAgreementAux";
            await PerformTransaction(sql).ConfigureAwait(false);
        }

        public async Task TruncateActionDiaryAuxiliary()
        {
            var sql = "DELETE FROM ActionDiaryAux";
            await PerformTransaction(sql).ConfigureAwait(false);
        }

        private async Task PerformTransaction(string sql, int timeout = 0)
        {
            await using var transaction = await Database.BeginTransactionAsync().ConfigureAwait(false);

            try
            {
                if (timeout != 0)
                    Database.SetCommandTimeout(timeout);
                await Database.ExecuteSqlRawAsync(sql).ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
            catch
            {
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        private async Task PerformInterpolatedTransaction(FormattableString sql, int timeout = 0)
        {
            await using var transaction = await Database.BeginTransactionAsync().ConfigureAwait(false);

            try
            {
                if (timeout != 0)
                    Database.SetCommandTimeout(timeout);
                await Database.ExecuteSqlInterpolatedAsync(sql).ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
            catch
            {
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }
    }
}
