using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
        /// Gets the operating balances.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns>The operating balances.</returns>
        public async Task<IList<OperatingBalance>> GetOperatingBalancesAsync(DateTime startDate, DateTime endDate)
            => await OperatingBalancesValue
                .FromSqlInterpolated($"usp_GetOperatingBalances {startDate:yyyy-MM-dd}, {endDate:yyyy-MM-dd}")
                .ToListAsync()
                .ConfigureAwait(false);

        /// <summary>
        /// Gets or sets the operating balances.
        /// </summary>
        private DbSet<OperatingBalance> OperatingBalancesValue { get; set; }

        /// <summary>
        /// Deletes the rent breakdowns.
        /// </summary>
        public async Task DeleteRentBreakdowns()
            => await PerformTransactionStoredProcedure("usp_DeleteCurrentRentPosition").ConfigureAwait(false);

        /// <summary>
        /// Deletes the current rent positions.
        /// </summary>
        public async Task DeleteCurrentRentPositions()
            => await PerformTransactionStoredProcedure("usp_DeleteCurrentRentPosition").ConfigureAwait(false);

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
