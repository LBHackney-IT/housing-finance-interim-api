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
    public class GarageDomain
    {
        /// <summary>
        /// Gets or sets the property reference.
        /// </summary>
        [JsonProperty("prop_ref")]
        public string PropertyRef { get; set; }

        /// <summary>
        /// Gets or sets the property type.
        /// </summary>
        [JsonProperty("Prop Type")]
        public string PropertyType { get; set; }

        /// <summary>
        /// Gets or sets the area office.
        /// </summary>
        [JsonProperty("Area Office")]
        public string AreaOffice { get; set; }

        /// <summary>
        /// Gets or sets the patch.
        /// </summary>
        [JsonProperty("Patch")]
        public string Patch { get; set; }

        /// <summary>
        /// Gets or sets the fund.
        /// </summary>
        [JsonProperty("Fund")]
        public string Fund { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [JsonProperty("Status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the void date.
        /// </summary>
        [JsonProperty("Void date")]
        [JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy")]
        public DateTime? VoidDate { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the tenancy date.
        /// </summary>
        [JsonProperty("Tenancy Date")]
        [JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy")]
        public DateTime? TenancyDate { get; set; }

        /// <summary>
        /// Gets or sets the tenure.
        /// </summary>
        [JsonProperty("tenure")]
        public string Tenure { get; set; }

        /// <summary>
        /// Gets or sets the address line 1.
        /// </summary>
        [JsonProperty("Address Line 1")]
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Gets or sets the address line 2.
        /// </summary>
        [JsonProperty("Address Line 2")]
        public string AddressLine2 { get; set; }

        /// <summary>
        /// Gets or sets the address line 3.
        /// </summary>
        [JsonProperty("Address Line 3")]
        public string AddressLine3 { get; set; }

        /// <summary>
        /// Gets or sets the Post Code.
        /// </summary>
        [JsonProperty("Post Code")]
        public string PostCode { get; set; }

        /// <summary>
        /// Gets or sets the rent account.
        /// </summary>
        [JsonProperty("u_saff_rentacc")]
        public string RentAccount { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        [JsonProperty("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the DD case.
        /// </summary>
        [JsonProperty("DD case")]
        public string DdCase { get; set; }

        /// <summary>
        /// Gets or sets the tenancy agreement ref.
        /// </summary>
        [JsonProperty("tag_ref")]
        public string TenancyAgreementRef { get; set; }

        /// <summary>
        /// Gets or sets the balance at 11 Oct 2020.
        /// </summary>
        [JsonProperty("Balance @ 11 Oct 2020")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? Balance11Oct20 { get; set; }

        /// <summary>
        /// Gets or sets the rent.
        /// </summary>
        [JsonProperty("rent")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? Rent { get; set; }

        /// <summary>
        /// Gets or sets the VAT.
        /// </summary>
        [JsonProperty("VAT")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? VAT { get; set; }

        /// <summary>
        /// Gets or sets the current total.
        /// </summary>
        [JsonProperty("Current Total")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? CurrentTotal { get; set; }

        /// <summary>
        /// Gets or sets the new rent.
        /// </summary>
        [JsonProperty("New rent")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? NewRent { get; set; }

        /// <summary>
        /// Gets or sets the new VAT.
        /// </summary>
        [JsonProperty("New Vat")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? NewVAT { get; set; }

        /// <summary>
        /// Gets or sets the new total.
        /// </summary>
        [JsonProperty("New Total")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? NewTotal { get; set; }

        /// <summary>
        /// Gets or sets the corr 1.
        /// </summary>
        [JsonProperty("corr_1")]
        public string Corr1 { get; set; }

        /// <summary>
        /// Gets or sets the corr 2.
        /// </summary>
        [JsonProperty("corr_2")]
        public string Corr2 { get; set; }

        /// <summary>
        /// Gets or sets the corr 3.
        /// </summary>
        [JsonProperty("corr_3")]
        public string Corr3 { get; set; }

        /// <summary>
        /// Gets or sets the corr postcode.
        /// </summary>
        [JsonProperty("corr_postcode")]
        public string CorrPostCode { get; set; }

    }

}
