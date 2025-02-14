using HousingFinanceInterimApi.Tests.V1.spike289v2.DatabaseContext;
using HousingFinanceInterimApi.Tests.V1.spike289v2.DatabaseContextFixture;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.spike289v2.DatabaseTests
{
    public class Gateway01Tests : IClassFixture<DatabaseFixtureFactory>
    {
        private readonly DatabaseFixtureFactory _contextFactory;
        private IDatabaseContextFixture _context;

        public Gateway01Tests(DatabaseFixtureFactory contextFactory)
        {
            _contextFactory = contextFactory;

        }

        [Theory]
        [InlineData(ConstantsGen.POSTGRESRDS)]
        public void Postgres_ConnectionString_Good(string dbContext)
        {
            _context = _contextFactory.CreateFixture(dbContext);
            //Assert.Equal("Postgres Rds Connectionstring", _context.GetConnectionString());
        }


        [Theory]
        [InlineData(ConstantsGen.SQLSERVERDOCKER)]
        public void SQL_ConnectionString_Good(string dbContext)
        {
            _context = _contextFactory.CreateFixture(dbContext);
            //Assert.Equal("SQLServer Docker Connectionstring", _context.GetConnectionString());
        }

        [Theory]
        [InlineData(ConstantsGen.POSTGRESRDS)]
        [InlineData(ConstantsGen.SQLSERVERDOCKER)]
        public void Status_Check_Is_True(string dbContext)
        {
            _context = _contextFactory.CreateFixture(dbContext);
            //Assert.True(_context.GetStatus());
        }

    }
}
