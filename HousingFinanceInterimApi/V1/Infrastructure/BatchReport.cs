using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    [Table("BatchReport")]
    public class BatchReport
    {
        [Key]
        public int Id { get; set; }

        public string ReportName { get; set; }

        public string RentGroup { get; set; }

        public string Group { get; set; }

        public string TransactionType { get; set; }

        public DateTime? ReportStartDate { get; set; }

        public DateTime? ReportEndDate { get; set; }

        public DateTime? ReportDate { get; set; }

        public int? ReportYear { get; set; }
        public int? ReportStartWeekOrMonth { get; set; }

        public int? ReportEndWeekOrMonth { get; set; }

        public string Link { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset? EndTime { get; set; }

        public bool IsSuccess { get; set; }
    }
}
