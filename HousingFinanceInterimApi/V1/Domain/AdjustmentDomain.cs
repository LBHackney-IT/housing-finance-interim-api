using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HousingFinanceInterimApi.JsonConverters;
using Newtonsoft.Json;

namespace HousingFinanceInterimApi.V1.Domain
{
    public class AdjustmentDomain : ISpreadsheetImportRow
    {
        public int Id { get; set; }

        [JsonProperty("Payment Ref")]
        public string PaymentRef { get; set; }

        [JsonProperty("Transaction Type")]
        public string TransactionType { get; set; }

        [JsonProperty("Amount")]
        public decimal Amount { get; set; }

        [JsonProperty("Adjustment Date")]
        [JsonConverter(typeof(DateTimeFormat))]
        public DateTime? TransactionDate { get; set; }

        public bool IsRead { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public bool IsValid(out string reason)
        {
            if (!TransactionDate.HasValue)
            {
                reason = "Adjustment Date is missing or in an unrecognised format.";
                return false;
            }

            if (!SpreadsheetFieldRules.IsAccountReference(PaymentRef))
            {
                reason = "Payment Ref must be 4 to 20 characters.";
                return false;
            }

            if (!SpreadsheetFieldRules.IsTransactionType(TransactionType))
            {
                reason = "Transaction Type must be 1 to 3 characters.";
                return false;
            }

            reason = null;
            return true;
        }
    }
}
