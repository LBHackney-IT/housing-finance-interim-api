using System.Collections.Generic;

namespace HousingFinanceInterimApi.V1.Helpers
{
    public class LogGroupProvider : ILogGroupProvider
    {
        public List<string> GetLogGroups(string environmentName)
        {
            var logGroups = LogGroupUtility.GetLogGroups(environmentName);
            return logGroups;
        }
    }
}
