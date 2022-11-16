using HousingFinanceInterimApi.V1.Infrastructure.Postgres.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Infrastructure.Postgres
{
    public class HousingFinanceContext : DbContext
    {
        public HousingFinanceContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AssetAuxDbEntity> AssetAuxDbEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder?.HasDefaultSchema("housingfinance_dbo");
            modelBuilder?.ApplyConfiguration(new AssetAuxConfiguration());
            modelBuilder?.ApplyConfiguration(new TenureInformationAuxConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        private async Task PerformTransaction(string query, int timeout = 0)
        {
            await using var transaction = await Database.BeginTransactionAsync().ConfigureAwait(false);

            try
            {
                if (timeout != 0)
                    Database.SetCommandTimeout(timeout);

                await Database.ExecuteSqlRawAsync(query).ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
            }
            catch
            {
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public async Task TruncateAssetsAuxiliary()
            => await PerformTransaction("DELETE FROM housingfinance_dbo.assets_aux;").ConfigureAwait(false);

        public async Task MergeAssetsAuxiliary()
            => await PerformTransaction($"CALL housingfinance_dbo.usp_load_chargesid();", 300).ConfigureAwait(false);

        public async Task TruncateTenuresInformationAuxiliary()
            => await PerformTransaction("DELETE FROM housingfinance_dbo.tenures_information_aux;").ConfigureAwait(false);

        public async Task MergeTenuresInformationAuxiliary()
            => await PerformTransaction($"CALL housingfinance_dbo.usp_load_transactionsid();", 300).ConfigureAwait(false);
    }
}
