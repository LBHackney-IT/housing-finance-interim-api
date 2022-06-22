using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Boundary.Request
{
    public class BatchReportCashSuspenseRequest
    {
        public int Year { get; set; }

        public string SuspenseAccountType { get; set; }
    }
}
