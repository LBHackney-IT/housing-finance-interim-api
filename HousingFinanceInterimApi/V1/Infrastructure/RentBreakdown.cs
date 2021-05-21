using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The rent breakdown entity.
    /// </summary>
    [Table("SSRentBreakdown")]
    public class RentBreakdown
    {

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// Gets the property reference.
        /// </summary>
        public string PropertyRef { get; set; }

        /// <summary>
        /// Gets the tenancy agreement reference.
        /// </summary>
        public string TenancyAgreementRef { get; set; }

        /// <summary>
        /// Gets the area office.
        /// </summary>
        public string AreaOffice { get; set; }

        /// <summary>
        /// Gets the patch.
        /// </summary>
        public string Patch { get; set; }

        /// <summary>
        /// Gets the uprn.
        /// </summary>
        public string UPRN { get; set; }

        /// <summary>
        /// Gets the payment reference.
        /// </summary>
        public string PaymentRef { get; set; }

        /// <summary>
        /// Gets the comment.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets the comment2.
        /// </summary>
        public string Comment2 { get; set; }

        /// <summary>
        /// Gets the occupied status.
        /// </summary>
        public string OccupiedStatus { get; set; }

        /// <summary>
        /// Gets the formula rent202021.
        /// </summary>
        public decimal? FormulaRent202021 { get; set; }

        /// <summary>
        /// Gets the actual rent202021.
        /// </summary>
        public decimal? ActualRent202021 { get; set; }

        /// <summary>
        /// Gets the bedrooms.
        /// </summary>
        public int Bedrooms { get; set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets the tenure.
        /// </summary>
        public string Tenure { get; set; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets the forename.
        /// </summary>
        public string Forename { get; set; }

        /// <summary>
        /// Gets the surname.
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Gets the tenancy start date.
        /// </summary>
        public DateTime? TenancyStartDate { get; set; }

        /// <summary>
        /// Gets the void date.
        /// </summary>
        public DateTime? VoidDate { get; set; }

        /// <summary>
        /// Gets the address line1.
        /// </summary>
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Gets the address line2.
        /// </summary>
        public string AddressLine2 { get; set; }

        /// <summary>
        /// Gets the address line3.
        /// </summary>
        public string AddressLine3 { get; set; }

        /// <summary>
        /// Gets the address line4.
        /// </summary>
        public string AddressLine4 { get; set; }

        /// <summary>
        /// Gets the postcode.
        /// </summary>
        public string Postcode { get; set; }

        /// <summary>
        /// Gets the formula.
        /// </summary>
        public decimal? Formula { get; set; }

        /// <summary>
        /// Gets the total rent.
        /// </summary>
        public decimal? TotalRent { get; set; }

        /// <summary>
        /// Gets the actual.
        /// </summary>
        public decimal? Actual { get; set; }

        /// <summary>
        /// Gets the water rates.
        /// </summary>
        public decimal? WaterRates { get; set; }

        /// <summary>
        /// Gets the water standing charge.
        /// </summary>
        public decimal? WaterStandingChrg { get; set; }

        /// <summary>
        /// Gets the watersure reduction.
        /// </summary>
        public decimal? WatersureReduction { get; set; }

        /// <summary>
        /// Gets the tenants levy.
        /// </summary>
        public decimal? TenantsLevy { get; set; }

        /// <summary>
        /// Gets the cleaning block.
        /// </summary>
        public decimal? CleaningBlock { get; set; }

        /// <summary>
        /// Gets the cleaning estate.
        /// </summary>
        public decimal? CleaningEstate { get; set; }

        /// <summary>
        /// Gets the landlord lighting.
        /// </summary>
        public decimal? LandlordLighting { get; set; }

        /// <summary>
        /// Gets the grounds maintenance.
        /// </summary>
        public decimal? GroundsMaintenance { get; set; }

        /// <summary>
        /// Gets the communal digital tv.
        /// </summary>
        public decimal? CommunalDigitalTV { get; set; }

        /// <summary>
        /// Gets the concierge.
        /// </summary>
        public decimal? Concierge { get; set; }

        /// <summary>
        /// Gets the heating.
        /// </summary>
        public decimal? Heating { get; set; }

        /// <summary>
        /// Gets the heating maintenance.
        /// </summary>
        public decimal? HeatingMaintenance { get; set; }

        /// <summary>
        /// Gets the television license.
        /// </summary>
        public decimal? TelevisionLicense { get; set; }

        /// <summary>
        /// Gets the contents insurance.
        /// </summary>
        public decimal? ContentsInsurance { get; set; }

        /// <summary>
        /// Gets the travellers charge.
        /// </summary>
        public decimal? TravellersCharge { get; set; }

        /// <summary>
        /// Gets the garage attached.
        /// </summary>
        public decimal? GarageAttached { get; set; }

        /// <summary>
        /// Gets the car port.
        /// </summary>
        public decimal? CarPort { get; set; }

        /// <summary>
        /// Gets the garage vat.
        /// </summary>
        public decimal? GarageVAT { get; set; }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset Timestamp { get; set; }

    }

}
