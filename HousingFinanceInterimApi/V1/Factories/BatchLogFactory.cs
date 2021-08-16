using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Factories
{
    public static class BatchLogFactory
    {
        public static BatchLogDomain ToDomain(BatchLog batchLog)
        {
            if (batchLog == null)
                return null;

            return new BatchLogDomain
            {
                Id = batchLog.Id,
                Type = batchLog.Type,
                StartTime = batchLog.StartTime,
                EndTime = batchLog.EndTime,
                IsSuccess = batchLog.IsSuccess
            };
        }
    }
}
