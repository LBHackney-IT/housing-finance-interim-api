using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The database context class.
    /// </summary>
    /// <seealso cref="DbContext" />
    public class DatabaseContext : DbContext
    {

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
        /// Deletes the rent breakdowns.
        /// </summary>
        public async Task DeleteRentBreakdowns()
        {
            await using var transaction = await Database.BeginTransactionAsync().ConfigureAwait(false);

            try
            {
                await Database.ExecuteSqlRawAsync("DeleteRentbreakdowns").ConfigureAwait(false);
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
