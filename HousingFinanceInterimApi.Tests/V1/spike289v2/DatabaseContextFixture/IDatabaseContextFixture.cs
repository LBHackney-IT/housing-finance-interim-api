namespace HousingFinanceInterimApi.Tests.V1.spike289v2.DatabaseContextFixture
{
    public interface IDatabaseContextFixture
    {
        public HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext CreateDbContext();
    }
}
