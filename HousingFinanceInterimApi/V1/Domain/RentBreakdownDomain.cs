using HousingFinanceInterimApi.JsonConverters;
using Newtonsoft.Json;
using System;

namespace HousingFinanceInterimApi.V1.Domain
{

    /// <summary>
    /// The rent breakdown domain object.
    /// </summary>
    public class RentBreakdownDomain
    {
        /// <summary>
        /// Gets or sets the property reference.
        /// </summary>
        [JsonProperty("Property Ref")]
        public string PropertyRef { get; set; }

        /// <summary>
        /// Gets or sets the tenancy agreement reference.
        /// </summary>
        [JsonProperty("UH Tenancy Ref")]
        public string TenancyAgreementRef { get; set; }

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
        /// Gets or sets the uprn.
        /// </summary>
        [JsonProperty("UPRN")]
        public string UPRN { get; set; }

        /// <summary>
        /// Gets or sets the payment reference.
        /// </summary>
        [JsonProperty("Payment Ref")]
        public string PaymentRef { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        [JsonProperty("Comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the comment2.
        /// </summary>
        [JsonProperty("Comment2")]
        public string Comment2 { get; set; }

        /// <summary>
        /// Gets or sets the occupied status.
        /// </summary>
        [JsonProperty("Occupied Status")]
        public string OccupiedStatus { get; set; }

        /// <summary>
        /// Gets or sets the formula rent202021.
        /// </summary>
        [JsonProperty("Formula Rent 2020/21")]
        public decimal? FormulaRent202021 { get; set; }

        /// <summary>
        /// Gets or sets the actual rent202021.
        /// </summary>
        [JsonProperty("ACTUAL RENT 2020/21 (Transitional Rent Constrained by Caps and Limits)")]
        public decimal? ActualRent202021 { get; set; }

        /// <summary>
        /// Gets or sets the bedrooms.
        /// </summary>
        [JsonProperty("Bedrooms")]
        public int Bedrooms { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [JsonProperty("Type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the tenure.
        /// </summary>
        [JsonProperty("Tenure")]
        public string Tenure { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [JsonProperty("Title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the forename.
        /// </summary>
        [JsonProperty("Forename")]
        public string Forename { get; set; }

        /// <summary>
        /// Gets or sets the surname.
        /// </summary>
        [JsonProperty("Surname")]
        public string Surname { get; set; }

        /// <summary>
        /// Gets or sets the tenancy start date.
        /// </summary>
        [JsonProperty("Tenancy Start Date")]
        [JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy")]
        public DateTime? TenancyStartDate { get; set; }

        /// <summary>
        /// Sets the void date input.
        /// </summary>
        [JsonProperty("Void Date")]
        [JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy")]
        public DateTime? VoidDateInput
        {
            set
            {
                if (value != null && value.Value.Date.Year >= 1900 && value.Value <= new DateTime(2079, 6, 6))
                {
                    VoidDate = value;
                }
                else
                {
                    VoidDate = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the void date.
        /// </summary>
        public DateTime? VoidDate { get; set; }

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
        /// Gets or sets the address line4.
        /// </summary>
        [JsonProperty("Address Line 4")]
        public string AddressLine4 { get; set; }

        /// <summary>
        /// Gets or sets the postcode.
        /// </summary>
        [JsonProperty("Post Code")]
        public string Postcode { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [JsonProperty("Formula")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? Formula { get; set; }

        /// <summary>
        /// Gets or sets the total rent.
        /// </summary>
        [JsonProperty("Total Rent")]
        public decimal? TotalRent { get; set; }

        /// <summary>
        /// Gets or sets the actual.
        /// </summary>
        [JsonProperty("Actual")]
        public decimal? Actual { get; set; }

        /// <summary>
        /// Gets or sets the water rates.
        /// </summary>
        [JsonProperty("Water Rates")]
        public decimal? WaterRates { get; set; }

        /// <summary>
        /// Gets or sets the water standing CHRG.
        /// </summary>
        [JsonProperty("Water Standing Chrg.")]
        public decimal? WaterStandingChrg { get; set; }

        /// <summary>
        /// Gets or sets the watersure reduction.
        /// </summary>
        [JsonProperty("Watersure Reduction")]
        public decimal? WatersureReduction { get; set; }

        /// <summary>
        /// Gets or sets the tenants levy.
        /// </summary>
        [JsonProperty("Tenants Levy")]
        public decimal? TenantsLevy { get; set; }

        /// <summary>
        /// Gets or sets the cleaning block.
        /// </summary>
        [JsonProperty("Cleaning (Block)")]
        public decimal? CleaningBlock { get; set; }

        /// <summary>
        /// Gets or sets the cleaning estate.
        /// </summary>
        [JsonProperty("Cleaning (Estate)")]
        public decimal? CleaningEstate { get; set; }

        /// <summary>
        /// Gets or sets the landlord lighting.
        /// </summary>
        [JsonProperty("Landlord Lighting")]
        public decimal? LandlordLighting { get; set; }

        /// <summary>
        /// Gets or sets the grounds maintenance.
        /// </summary>
        [JsonProperty("Grounds Maintenance")]
        public decimal? GroundsMaintenance { get; set; }

        /// <summary>
        /// Gets or sets the communal digital tv.
        /// </summary>
        [JsonProperty("Communal Digital TV")]
        public decimal? CommunalDigitalTV { get; set; }

        /// <summary>
        /// Gets or sets the concierge.
        /// </summary>
        [JsonProperty("Concierge")]
        public decimal? Concierge { get; set; }

        /// <summary>
        /// Gets or sets the heating.
        /// </summary>
        [JsonProperty("Heating")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? Heating { get; set; }

        /// <summary>
        /// Gets or sets the heating maintenance.
        /// </summary>
        [JsonProperty("Heating Maintenance")]
        public decimal? HeatingMaintenance { get; set; }

        /// <summary>
        /// Gets or sets the television license.
        /// </summary>
        [JsonProperty("Television License")]
        public decimal? TelevisionLicense { get; set; }

        /// <summary>
        /// Gets or sets the contents insurance.
        /// </summary>
        [JsonProperty("Contents Insurance")]
        public decimal? ContentsInsurance { get; set; }

        /// <summary>
        /// Gets or sets the travellers charge.
        /// </summary>
        [JsonProperty("Travellers Charge")]
        public decimal? TravellersCharge { get; set; }

        /// <summary>
        /// Gets or sets the garage attached.
        /// </summary>
        [JsonProperty("Garage (Attached)")]
        public decimal? GarageAttached { get; set; }

        /// <summary>
        /// Gets or sets the car port.
        /// </summary>
        [JsonProperty("Car Port")]
        public decimal? CarPort { get; set; }

        /// <summary>
        /// Gets or sets the garage vat.
        /// </summary>
        [JsonProperty("Garage VAT")]
        public decimal? GarageVAT { get; set; }

    }

}
