using HousingFinanceInterimApi.Tests.V1.spike289.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.spike289.DatabaseTests
{
    public class Gateway01TestBase : DatabaseTestsBase
    {
        private readonly BaseContextClass _context;
        internal string _connectionString;

        public Gateway01TestBase(BaseContextClass context) : base(context)
        {
            _context = context;
        }

        public virtual void ConnectionString_Good()
        {
            _connectionString = _context.GetConnectionString();
        }
    }
}
