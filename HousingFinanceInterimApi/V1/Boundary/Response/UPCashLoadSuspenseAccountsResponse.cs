using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Boundary.Response
{
    public class UPCashLoadSuspenseAccountsResponse
    {
        public long Id { get; set; }

        public string RentAccount { get; set; }

        public string PaymentSource { get; set; }

        public string MethodOfPayment { get; set; }

        public decimal AmountPaid { get; set; }

        public DateTime DatePaid { get; set; }

        public string CivicaCode { get; set; }
    }
}
