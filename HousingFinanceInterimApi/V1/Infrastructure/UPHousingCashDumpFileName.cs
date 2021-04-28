using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The UP Housing Cash dump file name entity.
    /// </summary>
    [Table("UPHousingCashDumpFileName")]
    public class UPHousingCashDumpFileName
    {

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        [Required]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is success.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets up housing cash dumps against this file name.
        /// </summary>
        public ICollection<UPHousingCashDump> UpHousingCashDumps { get; private set; }

    }

}
