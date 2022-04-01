using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    [Table("ChargesBatchYears")]
    public class ChargesBatchYear
    {
        [Key]
        public long Id { get; set; }

        public DateTime ProcessingDate { get; set; }

        public int Year { get; set; }

        public bool IsRead { get; set; }
    }
}
