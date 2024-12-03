using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using HousingFinanceInterimApi.JsonConverters;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Domain
{
    public class SuspenseTransactionAuxDomain
    {
        public long Id { get; set; }

        [JsonProperty("Original Payment Ref")]
        public string RentAccount { get; set; }

        [JsonProperty("Payment Date")]
        [JsonConverter(typeof(DateTimeFormat))]
        public DateTime Date { get; set; }

        [JsonProperty(nameof(Amount))]
        public decimal Amount { get; set; }

        [JsonProperty("New Payment Ref")]
        public string NewRentAccount { get; set; }

    }
}
