using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    [Table("DirectDebitAux")]
    public class DirectDebitAux
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string RentAccount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset Timestamp { get; private set; }
    }
}
