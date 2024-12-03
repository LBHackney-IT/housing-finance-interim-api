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
    public class CurrentRentPositionDomain
    {

        /// <summary>
        /// Gets or sets the property reference.
        /// </summary>
        [JsonProperty("Property Ref")]
        public string PropertyRef { get; set; }

        /// <summary>
        /// Gets or sets the payment reference.
        /// </summary>
        [JsonProperty("Payment Ref")]
        public string PaymentRef { get; set; }

        /// <summary>
        /// Gets or sets the week53 closing balance.
        /// </summary>
        [JsonProperty("Wk 53 closing balance")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week53ClosingBalance { get; set; }

        /// <summary>
        /// Sets the universal credit input.
        /// </summary>
        [JsonProperty("Universal Credit")]
        public string UniversalCreditInput
        {
            set => UniversalCredit = !string.IsNullOrWhiteSpace(value) &&
                                     value.Equals("Yes", StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Gets or sets a value indicating whether [universal credit].
        /// </summary>
        public bool UniversalCredit { get; set; }

        /// <summary>
        /// Gets or sets the universal housing reference.
        /// </summary>
        [JsonProperty("UH Ref")]
        public string UniversalHousingReference { get; set; }

        /// <summary>
        /// Gets or sets the hb claim reference.
        /// </summary>
        [JsonProperty("HB claim ref")]
        public string HBClaimRef { get; set; }

        /// <summary>
        /// Gets or sets the week1.
        /// </summary>
        [JsonProperty("Week: 1")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week1 { get; set; }

        /// <summary>
        /// Gets or sets the week27 balance.
        /// </summary>
        [JsonProperty("Week: 27 Balance")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week27Balance { get; set; }

        /// <summary>
        /// Gets or sets the total rent.
        /// </summary>
        [JsonProperty("Total Rent")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? TotalRent { get; set; }

        /// <summary>
        /// Gets or sets the hackney borough for w C12 oct2020.
        /// </summary>
        [JsonProperty("HB for w/c 12 Oct 2020")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? HackneyBoroughForWC12Oct2020 { get; set; }

        /// <summary>
        /// Gets or sets the subsequent weekly hackney borough.
        /// </summary>
        [JsonProperty("Susequent weekly HB")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? SubsequentWeeklyHackneyBorough { get; set; }

        /// <summary>
        /// Gets or sets the net rent.
        /// </summary>
        [JsonProperty("Net Rent")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? NetRent { get; set; }

        /// <summary>
        /// Sets the direct debit date input.
        /// </summary>
        [JsonProperty("Direct Debit Date")]
        public string DirectDebitDateInput
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    string tempValue = new string(value.Where(char.IsDigit).ToArray()).Trim().Replace(" ", string.Empty);

                    if (!string.IsNullOrWhiteSpace(tempValue) && int.TryParse(tempValue, out int directDebitDateOut) &&
                        directDebitDateOut <= 127)
                    {
                        DirectDebitDate = directDebitDateOut;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the direct debit date.
        /// </summary>
        public int? DirectDebitDate { get; set; }

        /// <summary>
        /// Gets or sets the week28 payment.
        /// </summary>
        [JsonProperty("Week 28 (w/e 18 Oct) Payment ")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week28Payment { get; set; }

        /// <summary>
        /// Gets or sets the week29 payment.
        /// </summary>
        [JsonProperty("Week 29 (w/e 25 Oct) Payment ")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week29Payment { get; set; }

        /// <summary>
        /// Gets or sets the week30 payment.
        /// </summary>
        [JsonProperty("Week 30 (w/e 1 Nov) Payment")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week30Payment { get; set; }

        /// <summary>
        /// Gets or sets the week31 payment.
        /// </summary>
        [JsonProperty("Week 31 (w/e 8 Nov) Payment")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week31Payment { get; set; }

        /// <summary>
        /// Gets or sets the week32 payment.
        /// </summary>
        [JsonProperty("Week 32 (w/e 15 Nov) Payment")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week32Payment { get; set; }

        /// <summary>
        /// Gets or sets the week33 payment.
        /// </summary>
        [JsonProperty("Week 33     (w/e 22 Nov) Payment")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week33Payment { get; set; }

        /// <summary>
        /// Gets or sets the week34 payment.
        /// </summary>
        [JsonProperty("Week 34     (w/e 29 Nov) Payment")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week34Payment { get; set; }

        /// <summary>
        /// Gets or sets the week35 payment.
        /// </summary>
        [JsonProperty("Week 35      (w/e 6 Dec) Payment ")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week35Payment { get; set; }

        /// <summary>
        /// Gets or sets the week36 payment.
        /// </summary>
        [JsonProperty("Week 36      (w/e 13 Dec) Payment ")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week36Payment { get; set; }

        /// <summary>
        /// Gets or sets the week37 payment.
        /// </summary>
        [JsonProperty("Week 37      (w/e 20 Dec) Payment ")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week37Payment { get; set; }

        /// <summary>
        /// Gets or sets the week38 payment.
        /// </summary>
        [JsonProperty("Week 38      (w/e 27 Dec) Payment ")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week38Payment { get; set; }

        /// <summary>
        /// Gets or sets the week39 payment.
        /// </summary>
        [JsonProperty("Week 39     (w/e 3 Jan) Payment ")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week39Payment { get; set; }

        /// <summary>
        /// Gets or sets the week40 payment.
        /// </summary>
        [JsonProperty("Week 40     (w/e 10 Jan) Payment ")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week40Payment { get; set; }

        /// <summary>
        /// Gets or sets the week41 payment.
        /// </summary>
        [JsonProperty("Week 41     (w/e 17 Jan) Payment ")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week41Payment { get; set; }

        /// <summary>
        /// Gets or sets the week42 payment.
        /// </summary>
        [JsonProperty("Week 42     (w/e 24 Jan) Payment ")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week42Payment { get; set; }

        [JsonProperty("Week 43     (w/e 31 Jan) Payment ")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week43Payment { get; set; }

        /// <summary>
        /// Gets or sets the week44 payment.
        /// </summary>
        [JsonProperty("Week 44     (w/e 7 Feb) Payment ")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week44Payment { get; set; }

        /// <summary>
        /// Gets or sets the week45 payment.
        /// </summary>
        [JsonProperty("Week 45     (w/e 14 Feb) Payment")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week45Payment { get; set; }

        /// <summary>
        /// Gets or sets the week46 payment.
        /// </summary>
        [JsonProperty("Week 46     (w/e 21 Feb) Payment ")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week46Payment { get; set; }

        /// <summary>
        /// Gets or sets the week47 payment.
        /// </summary>
        [JsonProperty("Week 47     (w/e 28 Feb) Payment ")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week47Payment { get; set; }

        /// <summary>
        /// Gets or sets the week48 payment.
        /// </summary>
        [JsonProperty("Week 48     (w/e 7 Mar) Payment ")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week48Payment { get; set; }

        /// <summary>
        /// Gets or sets the week49 payment.
        /// </summary>
        [JsonProperty("Week 49     (w/e 14 Mar) Payment ")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week49Payment { get; set; }

        /// <summary>
        /// Gets or sets the week50 payment.
        /// </summary>
        [JsonProperty(nameof(Week50Payment))]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week50Payment { get; set; }

        /// <summary>
        /// Gets or sets the week51 payment.
        /// </summary>
        [JsonProperty(nameof(Week51Payment))]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week51Payment { get; set; }

        /// <summary>
        /// Gets or sets the week52 payment.
        /// </summary>
        [JsonProperty(nameof(Week52Payment))]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? Week52Payment { get; set; }

        /// <summary>
        /// Gets or sets the estimated balance.
        /// </summary>
        [JsonProperty("Estimated Balance")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal EstimatedBalance { get; set; }

        /// <summary>
        /// Gets or sets the increase arrears since week27.
        /// </summary>
        [JsonProperty("Increase in arrears since wk 27")]
        [JsonConverter(typeof(NullPoundCurrencyConverter))]
        public decimal? IncreaseArrearsSinceWeek27 { get; set; }

        /// <summary>
        /// Gets or sets the tenant.
        /// </summary>
        [JsonProperty(nameof(Tenant))]
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the tenancy start date.
        /// </summary>
        [JsonProperty("Tenancy Start Date")]
        [JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy")]
        public DateTimeOffset? TenancyStartDate { get; set; }

        /// <summary>
        /// Gets or sets the type of the tenancy.
        /// </summary>
        [JsonProperty("Tenancy Type")]
        public string TenancyType { get; set; }

        /// <summary>
        /// Sets the date of birth input.
        /// </summary>
        [JsonProperty("Date of Birth")]
        public string DateOfBirthInput
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    DateOfBirth = DateTime.Parse(value, new CultureInfo("en-GB"));
                }
                else
                {
                    DateOfBirth = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the home telephone.
        /// </summary>
        [JsonProperty("Home Tel")]
        public string HomeTelephone { get; set; }

        /// <summary>
        /// Gets or sets the mobile.
        /// </summary>
        [JsonProperty(nameof(Mobile))]
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets the address line1.
        /// </summary>
        [JsonProperty("Address Line 1")]
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Gets or sets the address line2.
        /// </summary>
        [JsonProperty("Address Line 2")]
        public string AddressLine2 { get; set; }

        /// <summary>
        /// Gets or sets the address line3.
        /// </summary>
        [JsonProperty("Address Line 3")]
        public string AddressLine3 { get; set; }

        /// <summary>
        /// Gets or sets the postcode.
        /// </summary>
        [JsonProperty("Post Code")]
        public string Postcode { get; set; }

    }

}
