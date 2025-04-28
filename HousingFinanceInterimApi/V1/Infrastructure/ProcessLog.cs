using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    [Table("ProcessLog")]
    public class ProcessLog
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string LogGroupName { get; set; }

        public DateTime Timestamp { get; set; }

        public bool IsSuccess { get; set; }
    }
}
