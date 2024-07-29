using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The UP Cash dump file name entity.
    /// </summary>
    [Table("UPCashDumpFileName")]
    public class UPCashDumpFileName
    {

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets up cash dumps against this file name.
        /// </summary>
        public ICollection<UPCashDump> UpCashDumps { get; private set; }

    }

}
