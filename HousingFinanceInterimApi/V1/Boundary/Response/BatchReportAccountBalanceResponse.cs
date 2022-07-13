using System;

namespace HousingFinanceInterimApi.V1.Boundary.Response
{
    public class BatchReportAccountBalanceResponse
    {
        public int Id { get; set; }

        public string RentGroup { get; set; }

        public DateTime ReportDate { get; set; }

        public string Link { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset? EndTime { get; set; }

        public bool IsSuccess { get; set; }
    }
}
