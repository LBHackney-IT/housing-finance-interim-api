using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Principal;
using HousingFinanceInterimApi.V1.Boundary.Request;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The database context class.
    /// </summary>
    /// <seealso cref="DbContext" />
    public class DatabaseContext : DbContext, IDatabaseContext
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
            modelBuilder.Entity<PRNTransactionEntity>().HasNoKey().ToView(null);
            modelBuilder.Entity<ReportCashSuspenseAccount>().HasNoKey().ToView(null);
            modelBuilder.Entity<ReportCashImport>().HasNoKey().ToView(null);
            modelBuilder.Entity<ReportAccountBalance>().HasNoKey().ToView(null);
            modelBuilder.Entity<ChargesAux>().Property(x => x.TimeStamp).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<DirectDebitAux>().Property(x => x.Timestamp).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<ActionDiaryAux>().Property(x => x.Timestamp).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<TenancyAgreementAux>().Property(x => x.TimeStamp).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<UPCashDump>().Property(x => x.Timestamp).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<UPCashLoad>().Property(x => x.Timestamp).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<UPHousingCashDump>().Property(x => x.Timestamp).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<AdjustmentAux>().Property(x => x.Timestamp).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<MAProperty>().HasKey(x => x.PropRef);
            modelBuilder.Entity<UHProperty>().HasKey(x => x.PropRef);
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
        private DbSet<PRNTransactionEntity> PRNTransaction { get; set; }
        public DbSet<ChargesBatchYear> ChargesBatchYears { get; set; }
        public DbSet<MAProperty> MAProperty { get; set; }
        public DbSet<UHProperty> UHProperty { get; set; }

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

        public DbSet<UPCashLoad> UpCashLoads { get; set; }


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

        public DbSet<SuspenseTransaction> SuspenseTransactions { get; set; }
        public DbSet<SuspenseTransactionAux> SuspenseTransactionsAux { get; set; }
        public DbSet<ReportCashSuspenseAccount> ReportCashSuspenseAccounts { get; set; }
        public DbSet<ReportCashImport> ReportCashImports { get; set; }
        public DbSet<BatchReport> BatchReports { get; set; }
        public DbSet<ReportAccountBalance> ReportAccountBalances { get; set; }

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


        public async Task UpdateAssetDetails(UpdateAssetDetailsQuery query, UpdateAssetDetailsRequest request)
        {
            await PerformInterpolatedTransaction($"usp_UpdateAssetDetails {query.PropertyReference}, {request.PostPreamble}, {request.AddressLine1}")
                .ConfigureAwait(false);
        }

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

        public async Task<List<SuspenseTransaction>> GetCashSuspenseTransactions()
        {
            var results = new List<SuspenseTransaction>();

            var dbConnection = Database.GetDbConnection() as SqlConnection;
            var command = new SqlCommand($"dbo.usp_GetCashSuspenseTransactions", dbConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 900;

            dbConnection.Open();
            await using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
            {

                while (reader.Read())
                {
                    results.Add(new SuspenseTransaction()
                    {
                        Id = Convert.ToInt64(reader["Id"]),
                        RentAccount = reader["RentAccount"].ToString(),
                        PaymentDate = Convert.ToDateTime(reader["PaymentDate"]),
                        Amount = Convert.ToDecimal(reader["Amount"]),
                        NewRentAccount = reader["NewRentAccount"].ToString()
                    });
                }

            }
            dbConnection.Close();

            return results;
        }

        public async Task<List<SuspenseTransaction>> GetHousingBenefitSuspenseTransactions()
        {
            var results = new List<SuspenseTransaction>();

            var dbConnection = Database.GetDbConnection() as SqlConnection;
            var command = new SqlCommand($"dbo.usp_GetHousingBenefitSuspenseTransactions", dbConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 900;

            dbConnection.Open();
            await using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
            {

                while (reader.Read())
                {
                    results.Add(new SuspenseTransaction()
                    {
                        Id = Convert.ToInt64(reader["Id"]),
                        RentAccount = reader["RentAccount"].ToString(),
                        PaymentDate = Convert.ToDateTime(reader["PaymentDate"]),
                        Amount = Convert.ToDecimal(reader["Amount"]),
                        NewRentAccount = reader["NewRentAccount"].ToString()
                    });
                }

            }
            dbConnection.Close();

            return results;
        }

        public async Task<IList<Transaction>> GetTransactionsAsync(DateTime? startDate, DateTime? endDate)
            => await Transactions
                .FromSqlInterpolated($"usp_GetTransactions {startDate:yyyy-MM-dd}, {endDate:yyyy-MM-dd}")
                .ToListAsync()
                .ConfigureAwait(false);

        public async Task<IList<PRNTransactionEntity>> GetPRNTransactionsByRentGroupAsync(string rentGroup, int financialYear, int startWeekOrMonth, int endWeekOrMonth)
        {
            Database.SetCommandTimeout(timeout: 900); // set it equal to lambda timeout
            return await PRNTransaction
                .FromSqlInterpolated(
                    $"usp_GenerateOperatingBalanceAccounts @rent_group={rentGroup}, @post_year={financialYear}, @start_period={startWeekOrMonth}, @end_period={endWeekOrMonth}")
                .ToListAsync()
                .ConfigureAwait(false);
        }

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
            => await PerformInterpolatedTransaction($"usp_LoadTransactionsCharges {@processingYear}", 900).ConfigureAwait(false);

        public async Task LoadHousingFileTransactions()
            => await PerformTransaction("usp_LoadTransactionsHousingFile", 600).ConfigureAwait(false);

        public async Task LoadDirectDebitTransactions()
            => await PerformTransaction($"usp_LoadTransactionsDirectDebit", 600).ConfigureAwait(false);

        public async Task LoadAdjustmentTransactions()
            => await PerformTransaction("usp_LoadTransactionsAdjustment", 600).ConfigureAwait(false);

        public async Task LoadDirectDebitHistory(DateTime? processingDate)
            => await PerformInterpolatedTransaction($"usp_LoadDirectDebitHistory {processingDate:yyyy-MM-dd}", 600).ConfigureAwait(false);

        public async Task LoadCashSuspenseTransactions()
            => await PerformInterpolatedTransaction($"usp_LoadCashSuspenseTransactions", 600).ConfigureAwait(false);

        public async Task LoadHousingBenefitSuspenseTransactions()
            => await PerformInterpolatedTransaction($"usp_LoadHousingBenefitSuspenseTransactions", 600).ConfigureAwait(false);

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

        public async Task TruncateSuspenseTransactionAuxiliary()
        {
            var sql = "DELETE FROM SuspenseTransactionAux";
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
            => await PerformTransaction("usp_UpdateCurrentBalance", 900).ConfigureAwait(false);

        public async Task GenerateOperatingBalance()
            => await PerformTransaction("usp_GenerateOperatingBalance", 900).ConfigureAwait(false);

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
        public async Task<List<string[]>> GetCashSuspenseAccountByYearAsync(int year, string suspenseAccountType)
        {
            var results = new List<string[]>();

            var dbConnection = Database.GetDbConnection() as SqlConnection;
            var command = new SqlCommand($"usp_GetCashSuspenseAccountByYear", dbConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@year", year));
            if (!string.IsNullOrEmpty(suspenseAccountType))
                command.Parameters.Add(new SqlParameter("@suspenseAccountType", suspenseAccountType));
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

        public async Task<List<string[]>> GetCashImportByDateAsync(DateTime startDate, DateTime endDate)
        {
            var results = new List<string[]>();

            var dbConnection = Database.GetDbConnection() as SqlConnection;
            var command = new SqlCommand($"usp_GetCashImportByDate", dbConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@startDate", startDate.ToString("yyyy-MM-dd")));
            command.Parameters.Add(new SqlParameter("@endDate", endDate.ToString("yyyy-MM-dd")));
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

        public async Task<List<string[]>> GetChargesByYearAndRentGroupAsync(int year, string rentGroup)
        {
            var results = new List<string[]>();

            var dbConnection = Database.GetDbConnection() as SqlConnection;
            var command = new SqlCommand($"usp_GetChargesByYearAndRentGroup", dbConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@year", year));
            command.Parameters.Add(new SqlParameter("@rentGroup", rentGroup));
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

        public async Task<List<string[]>> GetChargesByGroupType(int year, string type)
        {
            var results = new List<string[]>();

            var dbConnection = Database.GetDbConnection() as SqlConnection;
            var command = new SqlCommand($"usp_GetChargesByGroupType", dbConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@year", year));
            command.Parameters.Add(new SqlParameter("@type", type));
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

        public async Task<List<string[]>> GetChargesByYear(int year)
        {
            var results = new List<string[]>();

            var dbConnection = Database.GetDbConnection() as SqlConnection;
            var command = new SqlCommand($"usp_GetChargesByYear", dbConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@year", year));
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

        #region Itemised Transactions
        public async Task<List<string[]>> GetItemisedTransactionsByYearAndTransactionTypeAsync(int year, string transactionType)
        {
            var results = new List<string[]>();

            var dbConnection = Database.GetDbConnection() as SqlConnection;
            var command = new SqlCommand($"usp_GetChargesByYearAndTransactionType", dbConnection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 900
            };

            command.Parameters.Add(new SqlParameter("@year", year));
            command.Parameters.Add(new SqlParameter("@transaction_type", transactionType));

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
            command.Dispose();

            return results;
        }
        #endregion

        public async Task<List<string[]>> GetHousingBenefitAcademyByYear(int year)
        {
            var results = new List<string[]>();

            var dbConnection = Database.GetDbConnection() as SqlConnection;
            var command = new SqlCommand($"usp_GetHousingBenefitAcademyByYear", dbConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@year", year));
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

        public async Task<IList<string[]>> GetReportAccountBalance(DateTime reportDate, string rentGroup)
        {
            var results = new List<string[]>();

            var dbConnection = Database.GetDbConnection() as SqlConnection;
            var command = new SqlCommand($"dbo.usp_GetReportAccountBalance", dbConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@reportDate", reportDate.ToString("yyyy-MM-dd")));
            if (!string.IsNullOrEmpty(rentGroup))
                command.Parameters.Add(new SqlParameter("@rentGroup", rentGroup));
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
