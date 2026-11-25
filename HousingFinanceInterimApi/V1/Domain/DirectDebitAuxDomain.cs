using System;
using HousingFinanceInterimApi.JsonConverters;
using Newtonsoft.Json;

namespace HousingFinanceInterimApi.V1.Domain
{
    public class DirectDebitAuxDomain : ISpreadsheetImportRow
    {
        public long Id { get; set; }

        [JsonProperty("Rent Account")]
        public string RentAccount { get; set; }

        [JsonProperty("Date")]
        [JsonConverter(typeof(DateTimeFormat))]
        public DateTime? Date { get; set; }

        [JsonProperty("Amount")]
        public decimal Amount { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public bool IsValid(out string reason)
        {
            if (!Date.HasValue)
            {
                reason = "Date is missing or in an unrecognised format.";
                return false;
            }

            if (!SpreadsheetFieldRules.IsAccountReference(RentAccount))
            {
                reason = "Rent Account must be 4 to 20 characters.";
                return false;
            }

            reason = null;
            return true;
        }
    }
}
