using HousingFinanceInterimApi.Tests.V1.spike289.DatabaseContext;
using Xunit;

namespace HousingFinanceInterimApi.Tests.V1.spike289.DatabaseTests
{
    [CollectionDefinition("SqlServerTests")]
    public class SqlServerTestCollection : ICollectionFixture<SqlServerDockerContext>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
