using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HousingFinanceInterimApi.V1.Infrastructure.Postgres.Configuration
{
    public class AssetAuxConfiguration : IEntityTypeConfiguration<AssetAuxDbEntity>
    {
        public void Configure(EntityTypeBuilder<AssetAuxDbEntity> builder)
        {
            builder?.HasKey(x => x.Id);
        }
    }
}
