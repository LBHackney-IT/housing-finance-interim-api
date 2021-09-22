using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    [Table("Adjustment")]
    public class Adjustment
    {
        [Key]
        public int Id { get; set; }

        public string TenancyAgreementRef { get; set; }

        public int Year { get; set; }

        public int Period { get; set; }

        public string TransactionType { get; set; }

        public string TransactionSource { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        public bool IsRead { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset Timestamp { get; private set; }
    }
}
