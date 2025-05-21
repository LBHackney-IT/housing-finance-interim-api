using System.Collections.Generic;

namespace HousingFinanceInterimApi.V1.Helpers
{
    public interface ILogGroupProvider
    {
        List<string> GetLogGroups(string environmentName);
    }
}
