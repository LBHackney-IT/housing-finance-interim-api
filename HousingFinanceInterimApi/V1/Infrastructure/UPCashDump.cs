using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// This represents a row in the cash .dat file loaded overnight
    /// </summary>
    [Table("upcashdump")]
    public class UPCashDump
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("upcashdumpfilenameid")]
        public long UPCashDumpFileNameId { get; set; }

        [ForeignKey(nameof(UPCashDumpFileNameId))]
        public UPCashDumpFileName UpCashDumpFileName { get; set; }

        [Required]
        [Column("fulltext")]
        public string FullText { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [Column("isread")]
        public bool IsRead { get; set; } = false;
    }

}
