using System;
using System.Collections.Generic;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Domain
{
    public class UPCashLoadSuspenseAccountsDomain
    {
        public long Id { get; set; }

        public long UPCashDumpId { get; set; }

        public string RentAccount { get; set; }

        public string NewRentAccount { get; set; }

        public string PaymentSource { get; set; }

        public string MethodOfPayment { get; set; }

        public decimal AmountPaid { get; set; }

        public DateTime DatePaid { get; set; }

        public string CivicaCode { get; set; }

        public bool IsResolved { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}
