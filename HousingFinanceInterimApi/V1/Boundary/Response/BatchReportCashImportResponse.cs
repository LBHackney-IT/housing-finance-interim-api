using System;

namespace HousingFinanceInterimApi.V1.Boundary.Response
{
    public class BatchReportCashImportResponse
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Link { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset? EndTime { get; set; }

        public bool IsSuccess { get; set; }
    }
}
