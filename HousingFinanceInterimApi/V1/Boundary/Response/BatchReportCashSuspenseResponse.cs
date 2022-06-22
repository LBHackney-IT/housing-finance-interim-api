using System;

namespace HousingFinanceInterimApi.V1.Boundary.Response
{
    public class BatchReportCashSuspenseResponse
    {
        public int Id { get; set; }

        public int Year { get; set; }

        public string SuspenseAccountType { get; set; }

        public string Link { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset? EndTime { get; set; }

        public bool IsSuccess { get; set; }
    }
}
