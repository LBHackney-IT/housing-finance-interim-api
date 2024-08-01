using HousingFinanceInterimApi.Tests.V1.spike289.DatabaseContext;

namespace HousingFinanceInterimApi.Tests.V1.spike289.DatabaseTests
{
    public abstract class DatabaseTestsBase
    {
        private readonly BaseContextClass _context;

        protected DatabaseTestsBase(BaseContextClass context)
        {
            _context = context;
        }
    }
}
