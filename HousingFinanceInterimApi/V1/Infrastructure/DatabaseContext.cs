using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The database context class.
    /// </summary>
    /// <seealso cref="DbContext" />
    public class DatabaseContext : DbContext
    {

        /// <summary>
        /// Override this method to further configure the model that was discovered by convention from the entity types
        /// exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
        /// and re-used for subsequent instances of your derived context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically
        /// define extension methods on this object that allow you to configure aspects of the model that are specific
        /// to a given database.</param>
        /// <remarks>
        /// If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
        /// then this method will not be run.
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OperatingBalance>().HasNoKey().ToView(null);
            modelBuilder.Entity<Payment>().HasNoKey().ToView(null);
            modelBuilder.Entity<Tenancy>().HasNoKey().ToView(null);
            modelBuilder.Entity<TenancyTransaction>().HasNoKey().ToView(null);
            modelBuilder.Entity<Transaction>().HasNoKey().ToView(null);
            modelBuilder.Entity<ReportCashSuspenseAccount>().HasNoKey().ToView(null);
            modelBuilder.Entity<ChargesAux>().Property(x => x.TimeStamp).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<DirectDebitAux>().Property(x => x.Timestamp).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<ActionDiaryAux>().Property(x => x.Timestamp).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<TenancyAgreementAux>().Property(x => x.TimeStamp).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<UPCashDump>().Property(x => x.Timestamp).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<UPHousingCashDump>().Property(x => x.Timestamp).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<AdjustmentAux>().Property(x => x.Timestamp).HasDefaultValueSql("GETDATE()");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public DatabaseContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<BatchLog> BatchLogs { get; set; }
        public DbSet<BatchLogError> BatchLogErrors { get; set; }
        private DbSet<Transaction> Transactions { get; set; }
        public DbSet<ChargesBatchYear> ChargesBatchYears { get; set; }

        /// <summary>
        /// Gets or sets the google file settings.
        /// </summary>
        public DbSet<GoogleFileSetting> GoogleFileSettings { get; set; }

        /// <summary>
        /// Gets or sets the up cash dump file names.
        /// </summary>
        public DbSet<UPCashDumpFileName> UpCashDumpFileNames { get; set; }

        /// <summary>
        /// Gets or sets the up cash dumps.
        /// </summary>
        public DbSet<UPCashDump> UpCashDumps { get; set; }

        /// <summary>
        /// Gets or sets the up cash dump file names.
        /// </summary>
        public DbSet<UPHousingCashDumpFileName> UpHousingCashDumpFileNames { get; set; }

        /// <summary>
        /// Gets or sets the up cash dumps.
        /// </summary>
        public DbSet<UPHousingCashDump> UpHousingCashDumps { get; set; }

        /// <summary>
        /// Gets or sets the error logs.
        /// </summary>
        public DbSet<ErrorLog> ErrorLogs { get; set; }

        /// <summary>
        /// Gets or sets the rent breakdowns.
        /// </summary>
        public DbSet<RentBreakdown> RentBreakdowns { get; set; }

        /// <summary>
        /// Gets or sets the current rent positions.
        /// </summary>
        public DbSet<CurrentRentPosition> CurrentRentPositions { get; set; }

        /// <summary>
        /// Gets or sets the service charges payments received.
        /// </summary>
        public DbSet<ServiceChargePaymentsReceived> ServiceChargesPaymentsReceived { get; set; }

        /// <summary>
        /// Gets or sets the leasehold accounts.
        /// </summary>
        public DbSet<LeaseholdAccount> LeaseholdAccounts { get; set; }

        /// <summary>
        /// Gets or sets the garages.
        /// </summary>
        public DbSet<Garage> Garages { get; set; }

        /// <summary>
        /// Gets or sets the garages.
        /// </summary>
        public DbSet<OtherHRA> OtherHRA { get; set; }

        public DbSet<CashSuspenseTransaction> CashSuspenseTransactions { get; set; }
        public DbSet<CashSuspenseTransactionAux> CashSuspenseTransactionsAux { get; set; }
        public DbSet<ReportCashSuspenseAccount> ReportCashSuspenseAccounts { get; set; }

        /// <summary>
        /// Gets the operating balances.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="startWeek">The start week.</param>
        /// <param name="startYear">The start year.</param>
        /// <param name="endWeek">The end week.</param>
        /// <param name="endYear">The end year.</param>
        /// <returns>The operating balances.</returns>
        public async Task<IList<OperatingBalance>> GetOperatingBalancesAsync(DateTime? startDate, DateTime? endDate, int startWeek, int startYear, int endWeek, int endYear)
            => await OperatingBalancesValue
                .FromSqlInterpolated($"usp_GetOperatingBalancesByRentGroup {startDate:yyyy-MM-dd}, {endDate:yyyy-MM-dd}, {startWeek}, {startYear}, {endWeek}, {endYear}")
                .ToListAsync()
                .ConfigureAwait(false);

        /// <summary>
        /// Gets or sets the operating balances.
        /// </summary>
        private DbSet<OperatingBalance> OperatingBalancesValue { get; set; }

        /// <summary>
        /// Gets the payments.
        /// </summary>
        /// <param name="tenancyAgreementRef">The tenancy agreement reference.</param>
        /// <param name="rentAccount">The rent account number.</param>
        /// <param name="householdRef">The household reference.</param>
        /// <param name="count">Number of rows to return.</param>
        /// <param name="order">List order (ASC or DESC).</param>
        /// <returns>The operating balances.</returns>
        public async Task<IList<Payment>> GetPaymentsAsync(string tenancyAgreementRef, string rentAccount, string householdRef, int count, string order)
            => await PaymentsValue
                .FromSqlInterpolated($"usp_GetTenantLastPayments {tenancyAgreementRef}, {rentAccount}, {householdRef}, {count}, {order}")
                .ToListAsync()
                .ConfigureAwait(false);

        /// <summary>
        /// Gets or sets the payments.
        /// </summary>
        private DbSet<Payment> PaymentsValue { get; set; }

        /// <summary>
        /// Gets the tenancy details.
        /// </summary>
        /// <param name="tenancyAgreementRef">The tenancy agreement reference.</param>
        /// <param name="rentAccount">The rent account number.</param>
        /// <param name="householdRef">The household reference.</param>
        /// <returns>Tenancy information.</returns>
        public async Task<IList<Tenancy>> GetTenanciesAsync(string tenancyAgreementRef, string rentAccount, string householdRef)
            => await TenanciesValue
                .FromSqlInterpolated($"usp_GetTenancyDetails {tenancyAgreementRef}, {rentAccount}, {householdRef}")
                .ToListAsync()
                .ConfigureAwait(false);

        /// <summary>
        /// Gets or sets the tenancies.
        /// </summary>
        private DbSet<Tenancy> TenanciesValue { get; set; }

        /// <summary>
        /// Gets the tenancy details.
        /// </summary>
        /// <param name="tenancyAgreementRef">The tenancy agreement reference.</param>
        /// <param name="rentAccount">The rent account number.</param>
        /// <param name="householdRef">The household reference.</param>
        /// <param name="count">Number of rows to return.</param>
        /// <param name="order">List order (ASC or DESC).</param>
        /// <returns>Tenancy transactions.</returns>
        public async Task<IList<TenancyTransaction>> GetTenancyTransactionsAsync(string tenancyAgreementRef, string rentAccount, string householdRef, int count, string order)
            => await TenancyTransactionValue
                .FromSqlInterpolated($"usp_GetTenancyTransactions {tenancyAgreementRef}, {rentAccount}, {householdRef}, {count}, {order}")
                .ToListAsync()
                .ConfigureAwait(false);

        /// <summary>
        /// Gets the tenancy details.
        /// </summary>
        /// <param name="tenancyAgreementRef">The tenancy agreement reference.</param>
        /// <param name="rentAccount">The rent account number.</param>
        /// <param name="householdRef">The household reference.</param>
        /// <param name="startDate">Start date.</param>
        /// <param name="endDate">End date.</param>
        /// <returns>Tenancy transactions.</returns>
        public async Task<IList<TenancyTransaction>> GetTenancyTransactionsByDateAsync(string tenancyAgreementRef, string rentAccount, string householdRef, DateTime startDate, DateTime endDate)
            => await TenancyTransactionValue
                .FromSqlInterpolated($"usp_GetTenancyTransactionsByDate {tenancyAgreementRef}, {rentAccount}, {householdRef}, {startDate}, {endDate}")
                .ToListAsync()
                .ConfigureAwait(false);

        /// <summary>
        /// Gets or sets the tenancies.
        /// </summary>
        private DbSet<TenancyTransaction> TenancyTransactionValue { get; set; }

        /// <summary>
        /// Deletes the rent breakdowns.
        /// </summary>
        public async Task DeleteRentBreakdowns()
            => await PerformTransaction("usp_DeleteRentBreakdown").ConfigureAwait(false);

        /// <summary>
        /// Deletes the current rent positions.
        /// </summary>
        public async Task DeleteCurrentRentPositions()
            => await PerformTransaction("usp_DeleteCurrentRentPosition").ConfigureAwait(false);

        /// <summary>
        /// Deletes the current rent positions.
        /// </summary>
        public async Task DeleteServiceChargePaymentsReceived()
            => await PerformTransaction("usp_DeleteServiceChargePaymentsReceived").ConfigureAwait(false);

        /// <summary>
        /// Deletes the rent breakdowns.
        /// </summary>
        public async Task DeleteLeaseholdAccounts()
            => await PerformTransaction("usp_DeleteLeaseholdAccounts").ConfigureAwait(false);

        /// <summary>
        /// Deletes the garages.
        /// </summary>
        public async Task DeleteGarages()
            => await PerformTransaction("usp_DeleteGarages").ConfigureAwait(false);

        /// <summary>
        /// Deletes the temp accomm and garaged (Other HRA).
        /// </summary>
        public async Task DeleteOtherHRA()
            => await PerformTransaction("usp_DeleteOtherHRA").ConfigureAwait(false);

        /// <summary>
        /// Generate table SSMiniTransaction using SpreadSheets tables.
        /// </summary>
        public async Task GenerateSpreadsheetTransaction()
            => await PerformTransaction("usp_GenerateSpreadsheetTransaction").ConfigureAwait(false);

        /// <summary>
        /// Copy information to MAMember using UHProperty and SpreadSheets tables.
        /// </summary>
        public async Task RefreshManageArrearsMember()
            => await PerformTransaction("usp_RefreshManageArrearsMember").ConfigureAwait(false);

        /// <summary>
        /// Copy information to MAProperty using UHProperty and SpreadSheets tables.
        /// </summary>
        public async Task RefreshManageArrearsProperty()
            => await PerformTransaction("usp_RefreshManageArrearsProperty").ConfigureAwait(false);

        public async Task<List<CashSuspenseTransaction>> GetCashSuspenseTransactions()
            => await CashSuspenseTransactions.FromSqlRaw($"usp_GetCashSuspenseTransactions", 600).ToListAsync().ConfigureAwait(false);

        public async Task<IList<Transaction>> GetTransactionsAsync(DateTime? startDate, DateTime? endDate)
            => await Transactions
                .FromSqlInterpolated($"usp_GetTransactions {startDate:yyyy-MM-dd}, {endDate:yyyy-MM-dd}")
                .ToListAsync()
                .ConfigureAwait(false);

        public async Task RefreshTenancyAgreementTables(long batchLogId)
            => await PerformTransaction($"usp_RefreshTenancyAgreement {batchLogId}", 600).ConfigureAwait(false);

        public async Task LoadCashFiles()
            => await PerformTransaction("usp_LoadCashFile", 600).ConfigureAwait(false);

        public async Task LoadHousingFiles()
            => await PerformTransaction("usp_LoadHousingFile", 600).ConfigureAwait(false);

        public async Task LoadDirectDebit(long batchLogId)
            => await PerformTransaction($"usp_LoadDirectDebit {batchLogId}", 300).ConfigureAwait(false);

        public async Task LoadCharges()
            => await PerformTransaction($"usp_LoadCharges", 300).ConfigureAwait(false);

        public async Task LoadActionDiary()
            => await PerformTransaction($"usp_LoadActionDiary", 900).ConfigureAwait(false);

        public async Task LoadCashFileTransactions()
            => await PerformTransaction("usp_LoadTransactionsCashFile", 600).ConfigureAwait(false);

        public async Task LoadChargesTransactions(int @processingYear)
            => await PerformInterpolatedTransaction($"usp_LoadTransactionsCharges {@processingYear}", 600).ConfigureAwait(false);

        public async Task LoadHousingFileTransactions()
            => await PerformTransaction("usp_LoadTransactionsHousingFile", 600).ConfigureAwait(false);

        public async Task LoadDirectDebitTransactions()
            => await PerformTransaction($"usp_LoadTransactionsDirectDebit", 600).ConfigureAwait(false);

        public async Task LoadAdjustmentTransactions()
            => await PerformTransaction("usp_LoadTransactionsAdjustment", 600).ConfigureAwait(false);

        public async Task LoadDirectDebitHistory(DateTime? processingDate)
            => await PerformInterpolatedTransaction($"usp_LoadDirectDebitHistory {processingDate:yyyy-MM-dd}", 600).ConfigureAwait(false);

        public async Task LoadcashSuspenseTransactions()
            => await PerformInterpolatedTransaction($"usp_LoadcashSuspenseTransactions", 600).ConfigureAwait(false);

        public async Task LoadChargesHistory(int @processingYear)
            => await PerformInterpolatedTransaction($"usp_LoadChargesHistory {@processingYear}", 600).ConfigureAwait(false);

        public async Task CreateCashFileSuspenseAccountTransaction(long id, string newRentAccount)
            => await PerformInterpolatedTransaction($"usp_UpdateCashFileSuspenseAccountResolved {id}, {newRentAccount}").ConfigureAwait(false);

        public async Task CreateHousingFileSuspenseAccountTransaction(long id, string newRentAccount)
            => await PerformInterpolatedTransaction($"usp_UpdateHousingCashFileSuspenseAccountResolved {id}, {newRentAccount}").ConfigureAwait(false);

        public async Task TruncateTenancyAgreementAuxiliary()
        {
            var sql = "DELETE FROM TenancyAgreementAux";
            await PerformTransaction(sql).ConfigureAwait(false);
        }

        public async Task TruncateDirectDebitAuxiliary()
        {
            var sql = "DELETE FROM DirectDebitAux";
            await PerformTransaction(sql).ConfigureAwait(false);
        }

        public async Task TruncateCashSuspenseTransactionAuxiliary()
        {
            var sql = "DELETE FROM CashSuspenseTransactionAux";
            await PerformTransaction(sql).ConfigureAwait(false);
        }

        public async Task TruncateChargesAuxiliary()
        {
            var sql = "DELETE FROM ChargesAux";
            await PerformTransaction(sql).ConfigureAwait(false);
        }

        public async Task TruncateActionDiaryAuxiliary()
        {
            var sql = "DELETE FROM ActionDiaryAux";
            await PerformTransaction(sql).ConfigureAwait(false);
        }

        public async Task TruncateAdjustmentsAuxiliary()
        {
            var sql = "DELETE FROM AdjustmentAux";
            await PerformTransaction(sql).ConfigureAwait(false);
        }

        public async Task UpdateCurrentBalance()
            => await PerformTransaction("usp_UpdateCurrentBalance", 300).ConfigureAwait(false);

        public async Task GenerateOperatingBalance()
            => await PerformTransaction("usp_GenerateOperatingBalance", 600).ConfigureAwait(false);

        public async Task RefreshManageArrearsTenancyAgreement()
            => await PerformTransaction("usp_RefreshManageArrearsTenancyAgreement", 900).ConfigureAwait(false);

        public async Task<List<string[]>> GetRentPosition()
        {
            var results = new List<string[]>();

            var dbConnection = Database.GetDbConnection() as SqlConnection;
            var command = new SqlCommand($"dbo.usp_GetRentPosition", dbConnection);
            command.CommandTimeout = 900;

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

                results.Insert(0, columnNames);
            }
            dbConnection.Close();

            return results;
        }

        //REPORTS
        public async Task<IList<ReportCashSuspenseAccount>> GetCashSuspenseAccountByYearAsync(int year, string suspenseAccountType)
            => await ReportCashSuspenseAccounts
                .FromSqlInterpolated($"usp_GetCashSuspenseAccountByYear {year}, {suspenseAccountType}")
                .ToListAsync()
                .ConfigureAwait(false);

        public async Task<List<dynamic>> GetChargesByYearAndRentGroupAsync(int year, string rentGroup)
        {
            var results = new List<dynamic>();

            var dbConnection = Database.GetDbConnection() as SqlConnection;
            var command = new SqlCommand($"dbo.usp_GetChargesByYearAndRentGroup {year}, {rentGroup}", dbConnection)
            {
                CommandTimeout = 900
            };

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
                    dynamic result = new System.Dynamic.ExpandoObject();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        ((IDictionary<string, object>) result)[columnNames[i]] = reader[i] != DBNull.Value ? reader[i] : null;
                    }

                    results.Add(result);
                }

            }
            dbConnection.Close();

            return results;
        }

        public async Task<List<dynamic>> GetChargesByGroupType(int year, string type)
        {
            var results = new List<dynamic>();

            var dbConnection = Database.GetDbConnection() as SqlConnection;
            var command = new SqlCommand($"dbo.usp_GetChargesByGroupType {year}, {type}", dbConnection)
            {
                CommandTimeout = 900
            };

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
                    dynamic result = new System.Dynamic.ExpandoObject();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        ((IDictionary<string, object>) result)[columnNames[i]] = reader[i] != DBNull.Value ? reader[i] : null;
                    }

                    results.Add(result);
                }

            }
            dbConnection.Close();

            return results;
        }

        public async Task<List<dynamic>> GetChargesByYear(int year)
        {
            var results = new List<dynamic>();

            var dbConnection = Database.GetDbConnection() as SqlConnection;
            var command = new SqlCommand($"dbo.usp_GetChargesByYear {year}", dbConnection)
            {
                CommandTimeout = 900
            };

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
                    dynamic result = new System.Dynamic.ExpandoObject();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        ((IDictionary<string, object>) result)[columnNames[i]] = reader[i] != DBNull.Value ? reader[i] : null;
                    }

                    results.Add(result);
                }

            }
            dbConnection.Close();

            return results;
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
