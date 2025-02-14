using HousingFinanceInterimApi.Tests.V1.spike289.DatabaseContext;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.spike289.DatabaseTests.PostgresTests
{
    /// <summary>
    /// Postgres RDS instance
    /// </summary>
    [Collection("PostgresTests")]
    public class PostgresGateway01Tests : Gateway01TestBase
    {
        private readonly PostgresRdsContext _context;

        public PostgresGateway01Tests(PostgresRdsContext context) : base(context)
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
        public void Status_Should_Be_False()
        {
            Assert.False(_context.IsOk);
        }
    }
}
