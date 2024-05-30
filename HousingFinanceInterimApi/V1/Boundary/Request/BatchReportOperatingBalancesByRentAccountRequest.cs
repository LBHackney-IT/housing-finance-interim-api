using System;
using System.ComponentModel.DataAnnotations;

namespace HousingFinanceInterimApi.V1.Boundary.Request
{
    public class BatchReportOperatingBalancesByRentAccountRequest
    {
        [Required]
        [StringLength(3)]
        public string RentGroup { get; set; }

        [Required]
        [Range(1900, 3000)]
        public int FinancialYear { get; set; }

        [Required]
        [Range(1, 53)]
        public int StartWeekOrMonth { get; set; }

        [Required]
        [Range(1, 53)]
        public int EndWeekOrMonth { get; set; }
    }
}
