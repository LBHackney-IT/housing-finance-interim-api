using System;
using System.Collections.Generic;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Domain
{
    public class UPHousingCashLoadSuspenseAccountsDomain
    {
        public long Id { get; set; }

        public string AcademyClaimRef { get; set; }

        public string column2 { get; set; }

        public string RentAccount { get; set; }

        public string NewRentAccount { get; set; }

        public DateTime Date { get; set; }

        public decimal value1 { get; set; }

        public decimal value2 { get; set; }

        public decimal value3 { get; set; }

        public decimal value4 { get; set; }

        public decimal value5 { get; set; }

        public long UPHousingCashDumpId { get; set; }

        public bool IsResolved { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}
