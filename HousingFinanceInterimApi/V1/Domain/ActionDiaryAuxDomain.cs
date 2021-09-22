using System;
using HousingFinanceInterimApi.JsonConverters;
using Newtonsoft.Json;

namespace HousingFinanceInterimApi.V1.Domain
{
    public class ActionDiaryAuxDomain
    {
        public long Id { get; set; }

        [JsonProperty("Tenancy Agreement Ref")]
        public string TenancyAgreementRef { get; set; }

        [JsonProperty("Rent Account")]
        public string RentAccount { get; set; }

        [JsonProperty("Action Code (Don't need fill)")]
        public string ActionCode { get; set; }

        [JsonProperty("Action")]
        public string Action { get; set; }

        [JsonProperty("Action Date")]
        [JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy")]
        public DateTime ActionDate { get; set; }

        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("Balance")]
        public decimal Balance { get; set; }

        [JsonProperty("Action Comment")]
        public string ActionComment { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}
