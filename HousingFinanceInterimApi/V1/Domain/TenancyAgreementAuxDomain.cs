using System;
using HousingFinanceInterimApi.JsonConverters;
using Newtonsoft.Json;

namespace HousingFinanceInterimApi.V1.Domain
{
    public class TenancyAgreementAuxDomain
    {
        public long Id { get; set; }

        [JsonProperty("Tenancy Agreement Ref")]
        public string TenancyAgreementRef { get; set; }

        [JsonProperty("Rent Account")]
        public string RentAccount { get; set; }

        [JsonProperty("Rent Group")]
        public string RentGroup { get; set; }

        [JsonProperty("Tenure")]
        public string Tenure { get; set; }

        [JsonProperty("Start Date")]
        [JsonConverter(typeof(DateTimeFormat))]
        public DateTime? StartDate { get; set; }

        [JsonProperty("End Date")]
        [JsonConverter(typeof(DateTimeFormat))]
        public DateTime? EndDate { get; set; }

        [JsonProperty("Property Ref")]
        public string PropertyRef { get; set; }

        [JsonProperty("Short Address")]
        public string ShortAddress { get; set; }

        [JsonProperty("Address")]
        public string Address { get; set; }

        [JsonProperty("Post Code")]
        public string PostCode { get; set; }

        [JsonProperty("Num Bedrooms")]
        public int? NumBedrooms { get; set; }

        [JsonProperty("Household Ref")]
        public string HouseholdRef { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Forename")]
        public string Forename { get; set; }

        [JsonProperty("Surname")]
        public string Surname { get; set; }

        [JsonProperty("Age")]
        public int? Age { get; set; }

        [JsonProperty("Contact Name")]
        public string ContactName { get; set; }

        [JsonProperty("Contact Address")]
        public string ContactAddress { get; set; }

        [JsonProperty("Contact PostCode")]
        public string ContactPostCode { get; set; }

        [JsonProperty("Contact Phone")]
        public string ContactPhone { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}
