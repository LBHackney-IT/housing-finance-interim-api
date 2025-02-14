using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.Tests.V1.spike289.DatabaseContext
{
    public class PostgresRdsContext : BaseContextClass
    {
        public override bool IsOk => false;

        public override IDatabaseContext CreateDbContext()
        {
            throw new System.NotImplementedException();
        }

        public override string GetConnectionString()
        {
            return "Postgres Connectionstring";
        }
    }
}
