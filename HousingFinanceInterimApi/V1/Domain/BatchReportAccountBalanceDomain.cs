using System;
using System.Collections.Generic;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Domain
{
    public class BatchReportAccountBalanceDomain
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
