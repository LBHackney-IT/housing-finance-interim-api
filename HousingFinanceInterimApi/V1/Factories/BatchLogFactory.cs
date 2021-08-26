using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Factories
{
    public static class BatchLogFactory
    {
        public static BatchLogDomain ToDomain(this BatchLog batchLog)
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

        public static List<BatchLogDomain> ToDomain(
            this ICollection<BatchLog> batchLog)
        {
            return batchLog?.Select(b => b.ToDomain()).ToList();
        }

        public static BatchLogResponse ToResponse(this BatchLogDomain batchLog)
        {
            if (batchLog == null)
                return null;

            return new BatchLogResponse
            {
                BatchId = batchLog.Id,
                ProcessName = batchLog.Type,
                StartTime = batchLog.StartTime,
                EndTime = batchLog.EndTime,
                Errors = batchLog.BatchLogErrors.ToResponse()
            };
        }

        public static List<BatchLogResponse> ToResponse(
            this ICollection<BatchLogDomain> batchLogs)
        {
            return batchLogs?.Select(b => b.ToResponse()).ToList();
        }
    }
}
