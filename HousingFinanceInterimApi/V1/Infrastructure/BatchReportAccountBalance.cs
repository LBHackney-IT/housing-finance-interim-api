using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    [Table("BatchReportAccountBalance")]
    public class BatchReportAccountBalance
    {
        [Key]
        public int Id { get; set; }

        public string RentGroup { get; set; }

        public DateTime ReportDate { get; set; }

        public string Link { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset? EndTime { get; set; }

        public bool IsSuccess { get; set; }
    }
}
