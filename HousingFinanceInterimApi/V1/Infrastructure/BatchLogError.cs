using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    [Table("BatchLogError")]
    public class BatchLogError
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Type { get; set; }

        public string Message { get; set; }

        public long BatchLogId { get; set; }

        [ForeignKey(nameof(BatchLogId))]
        public BatchLog BatchLog { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset Timestamp { get; set; }

    }
}
