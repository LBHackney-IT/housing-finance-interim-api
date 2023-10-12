using System.ComponentModel.DataAnnotations;

namespace HousingFinanceInterimApi.V1.Boundary.Request
{
    public class BatchReportItemisedTransactionRequest
    {
        [Required]
        public int Year { get; set; }

        [Required]
        public string TransactionType { get; set; }
    }
}
