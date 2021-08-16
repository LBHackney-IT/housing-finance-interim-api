using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Factories
{
    public static class BatchLogErrorFactory
    {
        public static BatchLogErrorDomain ToDomain(BatchLogError batchLogError)
        {
            if (batchLogError == null)
                return null;

            return new BatchLogErrorDomain
            {
                Id = batchLogError.Id,
                BatchLogId = batchLogError.BatchLogId,
                Type = batchLogError.Type,
                Message = batchLogError.Message,
                Timestamp = batchLogError.Timestamp
            };
        }
    }
}
