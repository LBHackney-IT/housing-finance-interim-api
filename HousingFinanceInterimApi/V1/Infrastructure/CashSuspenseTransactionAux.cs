using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    [Table("CashSuspenseTransactionAux")]
    public class CashSuspenseTransactionAux
    {
        public long Id { get; set; }

        public long IdSuspenseTransaction { get; set; }

        public string RentAccount { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

        public string NewRentAccount { get; set; }
    }
}
