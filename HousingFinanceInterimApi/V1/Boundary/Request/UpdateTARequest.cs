using System;
using System.Diagnostics.Eventing.Reader;

namespace HousingFinanceInterimApi.V1.Boundary.Request
{
    public class UpdateTARequest
    {
        public DateTime? TenureEndDate { get; set; }
        public bool IsPresent { get; set; }
        public bool IsTerminated { get; set; }
    }
}
