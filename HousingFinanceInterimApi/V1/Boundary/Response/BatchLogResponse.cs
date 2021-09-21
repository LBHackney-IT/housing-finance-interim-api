using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Boundary.Response
{
    public class BatchLogResponse
    {
        public long BatchId { get; set; }

        public string ProcessName { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public IList<BatchLogErrorResponse> Errors { get; set; }
    }
}
