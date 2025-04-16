using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.Tests.V1.spike289.DatabaseContext
{
    public class SqlServerDockerContext : BaseContextClass
    {
        public SqlServerDockerContext()
        {
        }

        public override bool IsOk => 1 == 1;

        public override IDatabaseContext CreateDbContext()
        {
            throw new System.NotImplementedException();
        }

        public override string GetConnectionString()
        {
            return "SQLServer Connectionstring";
        }
    }
}
