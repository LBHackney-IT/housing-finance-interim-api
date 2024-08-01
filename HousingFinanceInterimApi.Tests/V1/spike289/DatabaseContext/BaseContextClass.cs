using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.Tests.V1.spike289.DatabaseContext
{
    public abstract class BaseContextClass
    {
        public abstract IDatabaseContext CreateDbContext();

        public abstract string GetConnectionString();

        public abstract bool IsOk { get; }
    }
}
