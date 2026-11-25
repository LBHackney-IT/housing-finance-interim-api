using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.JsonConverters;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Domain
{
    public class SuspenseTransactionAuxDomain : ISpreadsheetImportRow
    {
        public long Id { get; set; }

        [JsonProperty("Original Payment Ref")]
        public string RentAccount { get; set; }

        [JsonProperty("Payment Date")]
        [JsonConverter(typeof(DateTimeFormat))]
        public DateTime? Date { get; set; }

        [JsonProperty("Amount")]
        public decimal Amount { get; set; }

        [JsonProperty("New Payment Ref")]
        public string NewRentAccount { get; set; }

        public bool IsValid(out string reason)
        {
            if (!Date.HasValue)
            {
                reason = "Payment Date is missing or in an unrecognised format.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(NewRentAccount) && !SpreadsheetFieldRules.IsAccountReference(NewRentAccount))
            {
                reason = "New Payment Ref must be 4 to 20 characters.";
                return false;
            }

            reason = null;
            return true;
        }
    }
}
