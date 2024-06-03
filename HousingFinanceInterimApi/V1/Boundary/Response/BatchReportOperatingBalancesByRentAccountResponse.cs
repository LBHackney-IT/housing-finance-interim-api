using System;

namespace HousingFinanceInterimApi.V1.Boundary.Response
{
    public class BatchReportOperatingBalancesByRentAccountResponse
    {
        public int Id { get; set; }
        public string RentGroup { get; set; }
        public int FinancialYear { get; set; }
        public int StartWeekOrMonth { get; set; }
        public int EndWeekOrMonth { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public string Link { get; set; }
        public bool IsSuccess { get; set; }
    }
}
