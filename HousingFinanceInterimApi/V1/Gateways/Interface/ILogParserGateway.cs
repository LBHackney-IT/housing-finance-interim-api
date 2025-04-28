using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.CloudWatchLogs.Model;

namespace HousingFinanceInterimApi.V1.Gateway.Interfaces
{
    public interface ILogParserGateway
    {
        Task UpdateDatabaseWithResults(string logGroupName, List<List<ResultField>> queryResults);
    }
}
