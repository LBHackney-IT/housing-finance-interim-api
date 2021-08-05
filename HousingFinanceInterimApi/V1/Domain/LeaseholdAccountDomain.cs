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
        /// Sets the assignment start date input.
        /// </summary>
        [JsonProperty("assignment_start")]
        public string AssignmentStartDateInput
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    AssignmentStartDate = DateTime.Parse(value, new CultureInfo("en-GB"));
                }
                else
                {
                    AssignmentStartDate = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the assignment start date.
        /// </summary>
        public DateTime? AssignmentStartDate { get; set; }

        /// <summary>
        /// Sets the assignment end date input.
        /// </summary>
        [JsonProperty("assignment_end")]
        public string AssignmentEndDateInput
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    AssignmentEndDate = DateTime.Parse(value, new CultureInfo("en-GB"));
                }
                else
                {
                    AssignmentEndDate = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the assignment end date.
        /// </summary>
        public DateTime? AssignmentEndDate { get; set; }

        /// <summary>
        /// Sets the assignment start date input.
        /// </summary>
        [JsonProperty("date_sold_leased")]
        public string SoldLeasedDateInput
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    SoldLeasedDate = DateTime.Parse(value, new CultureInfo("en-GB"));
                }
                else
                {
                    SoldLeasedDate = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the date sold leased.
        /// </summary>
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
