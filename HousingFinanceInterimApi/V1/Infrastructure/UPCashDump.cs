using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The UP Cash Dump entity.
    /// This is a line entry against a file name.
    /// </summary>
    [Table("UPCashDump")]
    public class UPCashDump
    {

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets up cash dump file name identifier.
        /// </summary>
        public long UPCashDumpFileNameId { get; set; }

        /// <summary>
        /// Gets or sets the name of up cash dump file.
        /// </summary>
        [ForeignKey(nameof(UPCashDumpFileNameId))]
        public UPCashDumpFileName UpCashDumpFileName { get; set; }

        /// <summary>
        /// Gets or sets the full text.
        /// </summary>
        [Required]
        public string FullText { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset Timestamp { get; private set; }

    }

}
