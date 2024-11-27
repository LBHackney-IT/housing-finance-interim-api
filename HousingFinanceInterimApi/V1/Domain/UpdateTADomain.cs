using System;

namespace HousingFinanceInterimApi.V1.Domain
{
    public class UpdateTADomain
    {
        public DateTime? TenureEndDate { get; set; }
        public bool IsPresent { get; set; }
        public bool IsTerminated { get; set; }
    }
}
