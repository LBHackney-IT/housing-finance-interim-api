using System;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    public class ReportAccountBalance
    {
        public string TenancyAgreementRef { get; set; }

        public string RentAccount { get; set; }

        public string RentGroup { get; set; }

        public DateTime? TenancyEndDate { get; set; }

        public decimal? Balance { get; set; }
    }
}
