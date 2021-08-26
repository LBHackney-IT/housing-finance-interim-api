using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    public class UPHousingCashLoadSuspenseAccounts
    {
        [Key]
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


        [ForeignKey(nameof(UPHousingCashDumpId))]
        public UPHousingCashDump UPHousingCashDump { get; set; }

        public bool IsResolved { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset Timestamp { get; set; }
    }
}
