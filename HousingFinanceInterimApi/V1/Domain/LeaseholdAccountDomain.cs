using HousingFinanceInterimApi.JsonConverters;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;

namespace HousingFinanceInterimApi.V1.Domain
{
    /// <summary>
    /// The current rent position domain object.
    /// </summary>
    public class LeaseholdAccountDomain
    {
        /// <summary>
        /// Gets or sets the tenancy agreement reference.
        /// </summary>
        [JsonProperty("uh_acct_no")]
        public string TenancyAgreementRef { get; set; }

        /// <summary>
        /// Gets or sets the payment reference.
        /// </summary>
        [JsonProperty("payment_ref")]
        public string PaymentRef { get; set; }

        /// <summary>
        /// Gets or sets the property reference.
        /// </summary>
        [JsonProperty("prop_ref")]
        public string PropertyRef { get; set; }

        /// <summary>
        /// Gets or sets the rent group.
        /// </summary>
        [JsonProperty("rentgroup")]
        public string RentGroup { get; set; }

        /// <summary>
        /// Gets or sets the tenure.
        /// </summary>
        [JsonProperty("tenure")]
        public string Tenure { get; set; }

        /// <summary>
        /// Gets or sets the assignment start date.
        /// </summary>
        [JsonProperty("assignment_start")]
        [JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy")]
        public DateTime? AssignmentStartDate { get; set; }

        /// <summary>
        /// Gets or sets the assignment end date.
        /// </summary>
        [JsonProperty("assignment_end")]
        [JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy")]
        public DateTime? AssignmentEndDate { get; set; }

        /// <summary>
        /// Gets or sets the date sold leased.
        /// </summary>
        [JsonProperty("date_sold_leased")]
        [JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy")]
        public DateTime? SoldLeasedDate { get; set; }

        /// <summary>
        /// Gets or sets the account type.
        /// </summary>
        [JsonProperty("account_type")]
        public string AccountType { get; set; }

        /// <summary>
        /// Gets or sets the agreement_type.
        /// </summary>
        [JsonProperty("agreement_type")]
        public string AgreementType { get; set; }

        /// <summary>
        /// Gets or sets the balance.
        /// </summary>
        [JsonProperty("balance")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? Balance { get; set; }

        /// <summary>
        /// Gets or sets the lessee.
        /// </summary>
        [JsonProperty("lessee")]
        public string Lessee { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

    }

}
