using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Factories
{
    public static class BatchLogErrorFactory
    {
        public static BatchLogErrorDomain ToDomain(this BatchLogError batchLogError)
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

        public static List<BatchLogErrorDomain> ToDomain(
            this ICollection<BatchLogError> batchLogError)
        {
            return batchLogError?.Select(b => b.ToDomain()).ToList();
        }

        public static BatchLogErrorResponse ToResponse(this BatchLogErrorDomain batchLogError)
        {
            if (batchLogError == null)
                return null;

            return new BatchLogErrorResponse
            {
                Type = batchLogError.Type,
                Message = batchLogError.Message
            };
        }

        public static List<BatchLogErrorResponse> ToResponse(
            this ICollection<BatchLogErrorDomain> batchLogErrors)
        {
            return batchLogErrors?.Select(b => b.ToResponse()).ToList();
        }
    }
}
