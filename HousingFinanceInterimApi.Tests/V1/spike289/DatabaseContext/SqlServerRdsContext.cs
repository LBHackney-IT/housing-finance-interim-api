using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.Tests.V1.spike289.DatabaseContext
{
    public class SqlServerRdsContext : BaseContextClass
    {
        public SqlServerRdsContext()
        {
        }

        public override bool IsOk => throw new System.NotImplementedException();

        public override IDatabaseContext CreateDbContext()
        {
            throw new System.NotImplementedException();
        }

        public override string GetConnectionString()
        {
            throw new System.NotImplementedException();
        }
    }
}
