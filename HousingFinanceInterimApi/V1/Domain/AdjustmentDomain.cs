using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HousingFinanceInterimApi.JsonConverters;
using Newtonsoft.Json;

namespace HousingFinanceInterimApi.V1.Domain
{
    public class AdjustmentDomain
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
        public DateTime TransactionDate { get; set; }

        public bool IsRead { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}
