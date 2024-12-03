using System;
using HousingFinanceInterimApi.JsonConverters;
using Newtonsoft.Json;

namespace HousingFinanceInterimApi.V1.Domain
{
    public class DirectDebitAuxDomain
    {
        public long Id { get; set; }

        [JsonProperty("Rent Account")]
        public string RentAccount { get; set; }

        [JsonProperty(nameof(Date))]
        [JsonConverter(typeof(DateTimeFormat))]
        public DateTime Date { get; set; }

        [JsonProperty(nameof(Amount))]
        public decimal Amount { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}
