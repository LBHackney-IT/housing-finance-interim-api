using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    public class UPCashLoadSuspenseAccounts
    {
        [Key]
        public long Id { get; set; }

        public long UPCashDumpId { get; set; }

        [ForeignKey(nameof(UPCashDumpId))]
        public UPCashDump UPCashDump { get; set; }

        public string RentAccount { get; set; }

        public string NewRentAccount { get; set; }

        public string PaymentSource { get; set; }

        public string MethodOfPayment { get; set; }

        public decimal AmountPaid { get; set; }

        public DateTime DatePaid { get; set; }

        public string CivicaCode { get; set; }

        public bool IsResolved { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset Timestamp { get; set; }
    }
}
