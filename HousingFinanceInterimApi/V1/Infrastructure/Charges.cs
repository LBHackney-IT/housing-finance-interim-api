using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HousingFinanceInterimApi.V1.Infrastructure
{
    [Table("Charges")]
    public class Charges
    {
        [Key]
        public long Id { get; set; }
        public string PropertyRef { get; set; }
        public string RentGroup { get; set; }
        public string ChargePeriod { get; set; }
        public string ChargeType { get; set; }
        public decimal Amount { get; set; }
        public bool Active { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public int Year { get; set; }
    }
}
