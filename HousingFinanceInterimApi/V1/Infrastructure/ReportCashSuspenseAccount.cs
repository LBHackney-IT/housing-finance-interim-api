using System;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    public class ReportCashSuspenseAccount
    {
        public string OriginalRentAccount { get; set; }

        public DateTime TransactionDate { get; set; }

        public decimal TransactionAmount { get; set; }

        public string TransactionType { get; set; }

        public string SuspenseAccountType { get; set; }
    }

}
