using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Boundary.Request
{
    public class BatchReportAccountBalanceRequest
    {
        public string RentGroup { get; set; }

        public DateTime ReportDate { get; set; }
    }
}
