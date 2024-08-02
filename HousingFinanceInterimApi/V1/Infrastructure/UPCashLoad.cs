using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HousingFinanceInterimApi.V1.Infrastructure
{
    [Table("UPCashLoad")]
    public class UPCashLoad
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string RentAccount { get; set; }
        public string PaymentSource { get; set; }
        public string MethodOfPayment { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime DatePaid { get; set; }
        public string CivicaCode { get; set; }
        public DateTimeOffset Timestamp { get; private set; }
        public bool IsRead { get; set; }

        [ForeignKey(nameof(UPCashDumpId))]
        public long UPCashDumpId { get; set; }
        public UPCashDump UpCashDump { get; set; }
    }

}
