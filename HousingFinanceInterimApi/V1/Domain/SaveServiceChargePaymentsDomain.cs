using HousingFinanceInterimApi.JsonConverters;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;

namespace HousingFinanceInterimApi.V1.Domain
{
    /// <summary>
    /// The service charge payments received domain object.
    /// </summary>
    public class ServiceChargePaymentsReceivedDomain
    {
        /// <summary>
        /// Gets or sets the patch.
        /// </summary>
        [JsonProperty("Arrears Patch")]
        public string ArrearPatch { get; set; }

        /// <summary>
        /// Gets or sets the rent account.
        /// </summary>
        [JsonProperty("UH Rent Acct")]
        public string TenancyAgreementRef { get; set; }

        /// <summary>
        /// Gets or sets the payment reference.
        /// </summary>
        [JsonProperty("Payment Ref")]
        public string PaymentRef { get; set; }

        /// <summary>
        /// Gets or sets the property reference.
        /// </summary>
        [JsonProperty("Property No")]
        public string PropertyRef { get; set; }

        /// <summary>
        /// Gets or sets the tenancy.
        /// </summary>
        [JsonProperty("Tenancy")]
        public string Tenancy { get; set; }

        /// <summary>
        /// Gets or sets the tenant.
        /// </summary>
        [JsonProperty("Tenant")]
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the address line1.
        /// </summary>
        [JsonProperty("Property Address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the direct debit date.
        /// </summary>
        [JsonProperty("Direct Debit Date")]
        public string DirectDebitDate { get; set; }

        /// <summary>
        /// Gets or sets the monthly debit.
        /// </summary>
        [JsonProperty("Monthly Debit ")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? MonthlyDebit { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [JsonProperty("Sep 2020 Debit to include Actuals")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? Sep20DebitToIncludeActuals { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [JsonProperty("Adjustments to SC Debits")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? AdjustmentsToSCDebits { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [JsonProperty("Direct Debits 15 & 23 Nov 2020")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DirectDebits15and23Nov20 { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [JsonProperty("Direct Debit December 2020")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DirectDebitDec20 { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [JsonProperty("Balance at 30 September 2020")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? Balance30Sep20 { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [JsonProperty("October moved to Judgement")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? OctoberMovedToJudgement { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [JsonProperty("January moved to Judgement")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? JanuaryMovedToJudgement { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [JsonProperty("February moved to Judgement")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? FebruaryMovedToJudgement { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [JsonProperty("March moved to Judgement")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? MarchMovedToJudgement { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [JsonProperty("October SC and MW Transfers")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? OctoberSCandMWTransfers { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [JsonProperty("March SC and MW Transfers")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? MarchSCandMWTransfers { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [JsonProperty("October Payments")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? OctoberPayments { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [JsonProperty("November Payments")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? NovemberPayments { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [JsonProperty("December Payments")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DecemberPayments { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [JsonProperty("January Payments")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? JanuaryPayments { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [JsonProperty("February Payments")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? FebruaryPayments { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [JsonProperty("March Payments")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? MarchPayments { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [JsonProperty("Disputed Amount")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DisputedAmount { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [JsonProperty("Balance (including disputed amount)")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? BalanceIncludingDisputedAmount { get; set; }
    }

}
