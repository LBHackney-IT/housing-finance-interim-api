using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HousingFinanceInterimApi.V1.Infrastructure.Postgres.Configuration
{
    public class TenureInformationAuxConfiguration : IEntityTypeConfiguration<TenureInformationAuxDbEntity>
    {
        public void Configure(EntityTypeBuilder<TenureInformationAuxDbEntity> builder)
        {
            builder?.HasKey(x => x.Id);
        }
    }
}
