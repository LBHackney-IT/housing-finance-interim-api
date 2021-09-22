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

        [JsonProperty("Tenancy Agreement Ref")]
        public string TenancyAgreementRef { get; set; }

        [JsonProperty("Year")]
        public int Year { get; set; }

        [JsonProperty("Period (Week/Month)")]
        public int Period { get; set; }

        [JsonProperty("Transaction Type")]
        public string TransactionType { get; set; }

        [JsonProperty("Transaction Source")]
        public string TransactionSource { get; set; }

        [JsonProperty("Amount")]
        public decimal Amount { get; set; }

        [JsonProperty("Transaction Date")]
        [JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy")]
        public DateTime TransactionDate { get; set; }

        public bool IsRead { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}
