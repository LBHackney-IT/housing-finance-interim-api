using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The UP Cash dump file name entity.
    /// </summary>
    [Table("upcashdumpfilename")]
    public class UPCashDumpFileName
    {

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long? Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        [Required]
        [Column("filename")]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is success.
        /// </summary>
        [Column("issuccess")]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets up cash dumps against this file name.
        /// </summary>
        public ICollection<UPCashDump> UpCashDumps { get; private set; }

    }

}
