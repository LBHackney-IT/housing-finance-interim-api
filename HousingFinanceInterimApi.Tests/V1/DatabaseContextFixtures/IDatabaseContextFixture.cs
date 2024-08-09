namespace HousingFinanceInterimApi.Tests.V1.DatabaseContextFixtures
{
    public interface IDatabaseContextFixture
    {
        public HousingFinanceInterimApi.V1.Infrastructure.DatabaseContext CreateDbContext();
    }
}
