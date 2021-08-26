using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Boundary.Response
{
    public class UPHousingCashLoadSuspenseAccountsResponse
    {
        public long Id { get; set; }

        public string AcademyClaimRef { get; set; }

        public string RentAccount { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }
    }
}
