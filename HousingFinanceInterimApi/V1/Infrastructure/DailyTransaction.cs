using System;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    public class DailyTransaction
    {

        public DateTime? Date { get; set; }

        public string TransactionDescription { get; set; }

        public decimal? Amount { get; set; }
    }

}
