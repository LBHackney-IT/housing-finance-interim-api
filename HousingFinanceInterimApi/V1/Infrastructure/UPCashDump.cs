using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// This represents a row in the cash .dat file loaded overnight
    /// </summary>
    [Table("UPCashDump")]
    public class UPCashDump
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long UPCashDumpFileNameId { get; set; }

        [ForeignKey(nameof(UPCashDumpFileNameId))]
        public UPCashDumpFileName UpCashDumpFileName { get; set; }

        [Required]
        public string FullText { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTimeOffset Timestamp { get; set; }

        public bool IsRead { get; set; } = false;
    }

}
