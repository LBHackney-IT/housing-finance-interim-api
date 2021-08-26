using System;

namespace HousingFinanceInterimApi.V1.Domain
{
    public class BatchLogErrorDomain
    {
        public long Id { get; set; }

        public long BatchLogId { get; set; }

        public string Type { get; set; }

        public string Message { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}
