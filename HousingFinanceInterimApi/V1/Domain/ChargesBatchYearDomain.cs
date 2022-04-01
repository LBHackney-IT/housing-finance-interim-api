using System;
using System.Collections.Generic;
using HousingFinanceInterimApi.V1.Infrastructure;

namespace HousingFinanceInterimApi.V1.Domain
{
    public class ChargesBatchYearDomain
    {
        public long Id { get; set; }

        public DateTime ProcessingDate { get; set; }

        public int Year { get; set; }

        public bool IsRead { get; set; }
    }
}
