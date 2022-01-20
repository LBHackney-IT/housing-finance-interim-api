using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    [Table("AdjustmentAux")]
    public class AdjustmentAux
    {
        [Key]
        public int Id { get; set; }

        public string PaymentRef { get; set; }

        public string TransactionType { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset Timestamp { get; private set; }
    }
}
