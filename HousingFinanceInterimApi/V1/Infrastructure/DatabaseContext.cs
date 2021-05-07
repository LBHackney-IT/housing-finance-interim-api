using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;

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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public DatabaseContext(DbContextOptions options)
            : base(options)
        {
        }

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
            => await PerformTransactionStoredProcedure("usp_DeleteRentBreakdown").ConfigureAwait(false);

        /// <summary>
        /// Deletes the current rent positions.
        /// </summary>
        public async Task DeleteCurrentRentPositions()
            => await PerformTransactionStoredProcedure("usp_DeleteCurrentRentPosition").ConfigureAwait(false);

        /// <summary>
        /// Deletes the current rent positions.
        /// </summary>
        public async Task DeleteServiceChargePaymentsReceived()
            => await PerformTransactionStoredProcedure("usp_DeleteServiceChargePaymentsReceived").ConfigureAwait(false);

        /// <summary>
        /// Deletes the rent breakdowns.
        /// </summary>
        public async Task DeleteLeaseholdAccounts()
            => await PerformTransactionStoredProcedure("usp_DeleteLeaseholdAccounts").ConfigureAwait(false);

        /// <summary>
        /// Deletes the garages.
        /// </summary>
        public async Task DeleteGarages()
            => await PerformTransactionStoredProcedure("usp_DeleteGarages").ConfigureAwait(false);

        /// <summary>
        /// Performs the transaction stored procedure execution.
        /// </summary>
        /// <param name="storedProc">The stored proc.</param>
        private async Task PerformTransactionStoredProcedure(string storedProc)
        {
            await using var transaction = await Database.BeginTransactionAsync().ConfigureAwait(false);

            try
            {
                await Database.ExecuteSqlRawAsync(storedProc).ConfigureAwait(false);
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
