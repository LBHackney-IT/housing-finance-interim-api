using System;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    public class SuspenseTransaction
    {
        public long Id { get; set; }
        public string RentAccount { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string NewRentAccount { get; set; }
    }
}
