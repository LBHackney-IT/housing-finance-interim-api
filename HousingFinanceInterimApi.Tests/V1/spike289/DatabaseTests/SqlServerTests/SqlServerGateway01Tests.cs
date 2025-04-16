using HousingFinanceInterimApi.Tests.V1.spike289.DatabaseContext;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.spike289.DatabaseTests.SqlServerTests
{
    /// <summary>
    /// Docker SQL Server instance
    /// </summary>
    [Collection("SqlServerTests")]
    public class SqlServerGateway01Tests : Gateway01TestBase
    {
        private readonly SqlServerDockerContext _context;

        public SqlServerGateway01Tests(SqlServerDockerContext context) : base(context)
        {
            _context = context;
        }

        [Fact]
        public override void ConnectionString_Good()
        {
            base.ConnectionString_Good();
            Assert.Equal(_connectionString, _context.GetConnectionString());
        }

        [Fact]
        public void Status_Should_Be_True()
        {
            Assert.True(_context.IsOk);
        }
    }
}
