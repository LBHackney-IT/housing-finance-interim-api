using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Boundary.Response
{
    public class BatchLogErrorResponse
    {
        public string Type { get; set; }

        public string Message { get; set; }
    }
}
