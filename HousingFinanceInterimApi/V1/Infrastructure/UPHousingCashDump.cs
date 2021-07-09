using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The UP Housing Cash Dump entity.
    /// This is a line entry against a file name.
    /// </summary>
    [Table("UPHousingCashDump")]
    public class UPHousingCashDump
    {

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets up housing cash dump file name identifier.
        /// </summary>
        public long UPHousingCashDumpFileNameId { get; set; }

        /// <summary>
        /// Gets or sets the name of up housing cash dump file.
        /// </summary>
        [ForeignKey(nameof(UPHousingCashDumpFileNameId))]
        public UPHousingCashDumpFileName UpHousingCashDumpFileName { get; set; }

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
