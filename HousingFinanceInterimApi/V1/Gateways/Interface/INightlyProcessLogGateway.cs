using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.CloudWatchLogs.Model;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Gateway.Interfaces
{
    public interface INightlyProcessLogGateway
    {
        Task UpdateDatabaseWithResults(string logGroupName, List<List<ResultField>> queryResults);

        Task<IList<NightlyProcessLog>> GetByDateCreatedAsync(DateTime createdDate);
    }
}
