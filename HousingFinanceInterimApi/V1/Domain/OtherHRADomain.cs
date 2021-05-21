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
    public class OtherHRADomain
    {
        /// <summary>
        /// Gets or sets the tenancy agreement ref.
        /// </summary>
        [JsonProperty("Tenancy Agreement Ref")]
        public string TenancyAgreementRef { get; set; }

        /// <summary>
        /// Gets or sets the property reference.
        /// </summary>
        [JsonProperty("Property Ref")]
        public string PropertyRef { get; set; }

        /// <summary>
        /// Gets or sets the property type.
        /// </summary>
        [JsonProperty("Rent Group")]
        public string RentGroup { get; set; }

        /// <summary>
        /// Gets the Total Charged.
        /// </summary>
        [JsonProperty("Total Charged")]
        public decimal? TotalCharged { get; set; }

        /// <summary>
        /// Gets the Basic Rent (No VAT).
        /// </summary>
        [JsonProperty("Basic Rent (No VAT)")]
        public decimal? BasicRent { get; set; }

        /// <summary>
        /// Gets the C Professional Fees.
        /// </summary>
        [JsonProperty("C Professional Fees")]
        public decimal? CProfessionalFees { get; set; }

        /// <summary>
        /// Gets the C Administration.
        /// </summary>
        [JsonProperty("C Administration")]
        public decimal? CAdministration { get; set; }

        /// <summary>
        /// Gets the cleaning block.
        /// </summary>
        [JsonProperty("Cleaning (Block)")]
        public decimal? CleaningBlock { get; set; }

        /// <summary>
        /// Gets the court costs.
        /// </summary>
        [JsonProperty("Court Costs")]
        public decimal? CourtCosts { get; set; }

        /// <summary>
        /// Gets the cleaning estate.
        /// </summary>
        [JsonProperty("Cleaning (Estate)")]
        public decimal? CleaningEstate { get; set; }

        /// <summary>
        /// Gets the contents insurance.
        /// </summary>
        [JsonProperty("Contents Insurance")]
        public decimal? ContentsInsurance { get; set; }

        /// <summary>
        /// Gets the concierge.
        /// </summary>
        [JsonProperty("Concierge")]
        public decimal? Concierge { get; set; }

        /// <summary>
        /// Gets the car port.
        /// </summary>
        [JsonProperty("Car Port")]
        public decimal? CarPort { get; set; }

        /// <summary>
        /// Gets the communal digital tv.
        /// </summary>
        [JsonProperty("Communal Digital TV")]
        public decimal? CommunalDigitalTV { get; set; }

        /// <summary>
        /// Gets the garage attached.
        /// </summary>
        [JsonProperty("Garage (Attached)")]
        public decimal? GarageAttached { get; set; }

        /// <summary>
        /// Gets the grounds maintenance.
        /// </summary>
        [JsonProperty("Grounds Maintenance")]
        public decimal? GroundsMaintenance { get; set; }

        /// <summary>
        /// Gets the grounds rent.
        /// </summary>
        [JsonProperty("Ground Rent")]
        public decimal? GroundRent { get; set; }

        /// <summary>
        /// Gets the host amenity.
        /// </summary>
        [JsonProperty("Host Amenity")]
        public decimal? HostAmenity { get; set; }

        /// <summary>
        /// Gets the heating.
        /// </summary>
        [JsonProperty("Heating")]
        public decimal? Heating { get; set; }

        /// <summary>
        /// Gets the heating maintenance.
        /// </summary>
        [JsonProperty("Heating Maintenance")]
        public decimal? HeatingMaintenance { get; set; }

        // <summary>
        /// Gets the Interest.
        /// </summary>
        [JsonProperty("Interest")]
        public decimal? Interest { get; set; }

        /// <summary>
        /// Gets the Arrangement Interest.
        /// </summary>
        [JsonProperty("Arrangement Interest")]
        public decimal? ArrangementInterest { get; set; }

        /// <summary>
        /// Gets the Lost Key Fobs.
        /// </summary>
        [JsonProperty("Lost Key Fobs")]
        public decimal? LostKeyFobs { get; set; }

        /// <summary>
        /// Gets the landlord lighting.
        /// </summary>
        [JsonProperty("Landlord Lighting")]
        public decimal? LandlordLighting { get; set; }

        /// <summary>
        /// Gets the Late Payment Charge.
        /// </summary>
        [JsonProperty("Late Payment Charge")]
        public decimal? LatePaymentCharge { get; set; }

        /// <summary>
        /// Gets the Major Works Capital
        /// </summary>
        [JsonProperty("Major Works Capital")]
        public decimal? MajorWorksCapital { get; set; }

        /// <summary>
        /// Gets the MW Judgement Trans
        /// </summary>
        [JsonProperty("MW Judgement Trans")]
        public decimal? MWJudgementTrans { get; set; }

        /// <summary>
        /// Gets the Major Works Revenue
        /// </summary>
        [JsonProperty("Major Works Revenue")]
        public decimal? MajorWorksRevenue { get; set; }

        /// <summary>
        /// Gets the R Administration Fee
        /// </summary>
        [JsonProperty("R Administration Fee")]
        public decimal? RAdministrationFee { get; set; }

        /// <summary>
        /// Gets the Rechg Repairs No VAT
        /// </summary>
        [JsonProperty("Rechg Repairs no VAT")]
        public decimal? RechgRepairsNoVAT { get; set; }

        /// <summary>
        /// Gets the Rechargeable Repairs
        /// </summary>
        [JsonProperty("Rechargeable Repairs")]
        public decimal? RechargeableRepairs { get; set; }

        /// <summary>
        /// Gets the SC Adjustment
        /// </summary>
        [JsonProperty("SC Adjustment")]
        public decimal? SCAdjustment { get; set; }

        /// <summary>
        /// Gets the SC Balancing Charge
        /// </summary>
        [JsonProperty("SC Balancing Charge")]
        public decimal? SCBalancingCharge { get; set; }

        /// <summary>
        /// Gets the Service Charges
        /// </summary>
        [JsonProperty("Service Charges")]
        public decimal? ServiceCharges { get; set; }

        /// <summary>
        /// Gets the SC Judgement Debit
        /// </summary>
        [JsonProperty("SC Judgement Debit")]
        public decimal? SCJudgementDebit { get; set; }

        /// <summary>
        /// Gets the Shared Owners Rent
        /// </summary>
        [JsonProperty("Shared Owners Rent")]
        public decimal? SharedOwnersRent { get; set; }

        /// <summary>
        /// Gets the Reserve Fund
        /// </summary>
        [JsonProperty("Reserve Fund")]
        public decimal? ReserveFund { get; set; }

        /// <summary>
        /// Gets the Storage
        /// </summary>
        [JsonProperty("Storage")]
        public decimal? Storage { get; set; }

        /// <summary>
        /// Gets the Basic Rent Temp Acc
        /// </summary>
        [JsonProperty("Basic Rent Temp Acc")]
        public decimal? BasicRentTempAcc { get; set; }

        /// <summary>
        /// Gets the Travellers Charge
        /// </summary>
        [JsonProperty("Travellers Charge")]
        public decimal? TravellersCharge { get; set; }

        /// <summary>
        /// Gets the Tenants Levy
        /// </summary>
        [JsonProperty("Tenants Levy")]
        public decimal? TenantsLevy { get; set; }

        /// <summary>
        /// Gets the Television License
        /// </summary>
        [JsonProperty("Television License")]
        public decimal? TelevisionLicense { get; set; }

        /// <summary>
        /// Gets the VAT Charge
        /// </summary>
        [JsonProperty("VAT Charge")]
        public decimal? VATCharge { get; set; }

        /// <summary>
        /// Gets the Water Rates
        /// </summary>
        [JsonProperty("Water Rates")]
        public decimal? WaterRates { get; set; }

        /// <summary>
        /// Gets the Water standing Chrg
        /// </summary>
        [JsonProperty("Water Standing Chrg.")]
        public decimal? WaterStandingChrg { get; set; }

        /// <summary>
        /// Gets the Watersure Reduction
        /// </summary>
        [JsonProperty("Watersure Reduction")]
        public decimal? WatersureReduction { get; set; }

        /// <summary>
        /// Gets the Rep Cash Incentive
        /// </summary>
        [JsonProperty("Rep. Cash Incentive")]
        public decimal? RepCashIncentive { get; set; }

        /// <summary>
        /// Gets the PromptPay Discount
        /// </summary>
        [JsonProperty("Prompt Pay. Discount")]
        public decimal? PromptPayDiscount { get; set; }

        /// <summary>
        /// Gets the SC Judgement Trans
        /// </summary>
        [JsonProperty("SC Judgement Trans")]
        public decimal? SCJudgementTrans { get; set; }

        /// <summary>
        /// Gets the TMO Reversal
        /// </summary>
        [JsonProperty("TMO Reversal")]
        public decimal? TMOReversal { get; set; }

        /// <summary>
        /// Gets the Rent Waiver
        /// </summary>
        [JsonProperty("Rent waiver")]
        public decimal? RentWaiver { get; set; }

        /// <summary>
        /// Gets the Write On
        /// </summary>
        [JsonProperty("Write On")]
        public decimal? WriteOn { get; set; }

    }

}
