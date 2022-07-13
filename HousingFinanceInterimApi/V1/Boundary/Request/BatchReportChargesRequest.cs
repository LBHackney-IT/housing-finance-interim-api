using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Boundary.Request
{
    public class BatchReportChargesRequest
    {
        public int Year { get; set; }

        public string RentGroup { get; set; }

        public string Group { get; set; }
    }
}
