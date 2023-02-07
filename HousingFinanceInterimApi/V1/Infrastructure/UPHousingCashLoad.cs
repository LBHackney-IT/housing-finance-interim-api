using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The UP Housing Cash Dump entity.
    /// This is a line entry against a file name.
    /// </summary>
    [Table("UPHousingCashLoad")]
    public class UPHousingCashLoad
    {
        [Key]
        public long Id { get; set; }

        public string AcademyClaimRef { get; set; }

        public string Column2 { get; set; }

        public string RentAccount { get; set; }

        public DateTime? Date { get; set; }

        public decimal? Value1 { get; set; }

        public decimal? Value2 { get; set; }

        public decimal? Value3 { get; set; }

        public decimal? Value4 { get; set; }

        public decimal? Value5 { get; set; }

        public long UPHousingCashDumpId { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public bool? IsRead { get; set; }
    }
}
