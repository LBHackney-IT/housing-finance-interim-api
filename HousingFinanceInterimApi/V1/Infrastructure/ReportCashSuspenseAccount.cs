using System;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    public class ReportCashSuspenseAccount
    {
        public string RentReference { get; set; }

        public DateTime TransactionDate { get; set; }

        public decimal TransactionAmount { get; set; }

        public string TransactionType { get; set; }

        public string OriginalRentAccount { get; set; }

        public string RentGroup { get; set; }

        public string OriginId { get; set; }

        public string Description { get; set; }
    }
}
