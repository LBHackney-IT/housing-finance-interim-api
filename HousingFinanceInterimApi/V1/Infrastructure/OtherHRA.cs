using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The current rent position entity.
    /// </summary>
    [Table("SSOtherHRA")]
    public class OtherHRA
    {

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the tenancy agreement ref.
        /// </summary>
        [Column("TenancyAgreementRef")]
        public string TenancyAgreementRef { get; set; }

        /// <summary>
        /// Gets or sets the property reference.
        /// </summary>
        [Column("PropertyRef")]
        public string PropertyRef { get; set; }

        /// <summary>
        /// Gets or sets the property type.
        /// </summary>
        [Column("RentGroup")]
        public string RentGroup { get; set; }

        /// <summary>
        /// Gets the Total Charged.
        /// </summary>
        [Column("TotalCharged")]
        public decimal? TotalCharged { get; set; }

        /// <summary>
        /// Gets the Basic Rent.
        /// </summary>
        [Column("BasicRent")]
        public decimal? BasicRent { get; set; }


        /// <summary>
        /// Gets the C Professional Fees.
        /// </summary>
        [Column("CProfessionalFees")]
        public decimal? CProfessionalFees { get; set; }

        /// <summary>
        /// Gets the C Administration.
        /// </summary>
        [Column("CAdministration")]
        public decimal? CAdministration { get; set; }

        /// <summary>
        /// Gets the cleaning block.
        /// </summary>
        [Column("CleaningBlock")]
        public decimal? CleaningBlock { get; set; }

        /// <summary>
        /// Gets the court costs.
        /// </summary>
        [Column("CourtCosts")]
        public decimal? CourtCosts { get; set; }

        /// <summary>
        /// Gets the cleaning estate.
        /// </summary>
        [Column("CleaningEstate")]
        public decimal? CleaningEstate { get; set; }

        /// <summary>
        /// Gets the contents insurance.
        /// </summary>
        [Column("ContentsInsurance")]
        public decimal? ContentsInsurance { get; set; }

        /// <summary>
        /// Gets the concierge.
        /// </summary>
        [Column("Concierge")]
        public decimal? Concierge { get; set; }

        /// <summary>
        /// Gets the car port.
        /// </summary>
        [Column("CarPort")]
        public decimal? CarPort { get; set; }

        /// <summary>
        /// Gets the communal digital tv.
        /// </summary>
        [Column("CommunalDigitalTV")]
        public decimal? CommunalDigitalTV { get; set; }

        /// <summary>
        /// Gets the garage attached.
        /// </summary>
        [Column("GarageAttached")]
        public decimal? GarageAttached { get; set; }

        /// <summary>
        /// Gets the grounds maintenance.
        /// </summary>
        [Column("GroundsMaintenance")]
        public decimal? GroundsMaintenance { get; set; }

        /// <summary>
        /// Gets the grounds rent.
        /// </summary>
        [Column("GroundRent")]
        public decimal? GroundRent { get; set; }

        /// <summary>
        /// Gets the host amenity.
        /// </summary>
        [Column("HostAmenity")]
        public decimal? HostAmenity { get; set; }

        /// <summary>
        /// Gets the heating.
        /// </summary>
        [Column("Heating")]
        public decimal? Heating { get; set; }

        /// <summary>
        /// Gets the heating maintenance.
        /// </summary>
        [Column("HeatingMaintenance")]
        public decimal? HeatingMaintenance { get; set; }

        // <summary>
        /// Gets the Interest.
        /// </summary>
        [Column("Interest")]
        public decimal? Interest { get; set; }

        /// <summary>
        /// Gets the Arrangement Interest.
        /// </summary>
        [Column("ArrangementInterest")]
        public decimal? ArrangementInterest { get; set; }

        /// <summary>
        /// Gets the Lost Key Fobs.
        /// </summary>
        [Column("LostKeyFobs")]
        public decimal? LostKeyFobs { get; set; }

        /// <summary>
        /// Gets the landlord lighting.
        /// </summary>
        [Column("LandlordLighting")]
        public decimal? LandlordLighting { get; set; }

        /// <summary>
        /// Gets the Late Payment Charge.
        /// </summary>
        [Column("LatePaymentCharge")]
        public decimal? LatePaymentCharge { get; set; }

        /// <summary>
        /// Gets the Major Works Capital
        /// </summary>
        [Column("MajorWorksCapital")]
        public decimal? MajorWorksCapital { get; set; }

        /// <summary>
        /// Gets the MW Judgement Trans
        /// </summary>
        [Column("MWJudgementTrans")]
        public decimal? MWJudgementTrans { get; set; }

        /// <summary>
        /// Gets the Major Works Revenue
        /// </summary>
        [Column("MajorWorksRevenue")]
        public decimal? MajorWorksRevenue { get; set; }

        /// <summary>
        /// Gets the R Administration Fee
        /// </summary>
        [Column("RAdministrationFee")]
        public decimal? RAdministrationFee { get; set; }

        /// <summary>
        /// Gets the Rechg Repairs No VAT
        /// </summary>
        [Column("RechgRepairsNoVAT")]
        public decimal? RechgRepairsNoVAT { get; set; }

        /// <summary>
        /// Gets the Rechargeable Repairs
        /// </summary>
        [Column("RechargeableRepairs")]
        public decimal? RechargeableRepairs { get; set; }

        /// <summary>
        /// Gets the SC Adjustment
        /// </summary>
        [Column("SCAdjustment")]
        public decimal? SCAdjustment { get; set; }

        /// <summary>
        /// Gets the SC Balancing Charge
        /// </summary>
        [Column("SCBalancingCharge")]
        public decimal? SCBalancingCharge { get; set; }

        /// <summary>
        /// Gets the Service Charges
        /// </summary>
        [Column("ServiceCharges")]
        public decimal? ServiceCharges { get; set; }

        /// <summary>
        /// Gets the SC Judgement Debit
        /// </summary>
        [Column("SCJudgementDebit")]
        public decimal? SCJudgementDebit { get; set; }

        /// <summary>
        /// Gets the Shared Owners Rent
        /// </summary>
        [Column("SharedOwnersRent")]
        public decimal? SharedOwnersRent { get; set; }

        /// <summary>
        /// Gets the Reserve Fund
        /// </summary>
        [Column("ReserveFund")]
        public decimal? ReserveFund { get; set; }

        /// <summary>
        /// Gets the Storage
        /// </summary>
        [Column("Storage")]
        public decimal? Storage { get; set; }

        /// <summary>
        /// Gets the Basic Rent Temp Acc
        /// </summary>
        [Column("BasicRentTempAcc")]
        public decimal? BasicRentTempAcc { get; set; }

        /// <summary>
        /// Gets the Travellers Charge
        /// </summary>
        [Column("TravellersCharge")]
        public decimal? TravellersCharge { get; set; }

        /// <summary>
        /// Gets the Tenants Levy
        /// </summary>
        [Column("TenantsLevy")]
        public decimal? TenantsLevy { get; set; }

        /// <summary>
        /// Gets the Television License
        /// </summary>
        [Column("TelevisionLicense")]
        public decimal? TelevisionLicense { get; set; }

        /// <summary>
        /// Gets the VAT Charge
        /// </summary>
        [Column("VATCharge")]
        public decimal? VATCharge { get; set; }

        /// <summary>
        /// Gets the Water Rates
        /// </summary>
        [Column("WaterRates")]
        public decimal? WaterRates { get; set; }

        /// <summary>
        /// Gets the Water standing Chrg
        /// </summary>
        [Column("WaterStandingChrg")]
        public decimal? WaterStandingChrg { get; set; }

        /// <summary>
        /// Gets the Watersure Reduction
        /// </summary>
        [Column("WatersureReduction")]
        public decimal? WatersureReduction { get; set; }

        /// <summary>
        /// Gets the Rep Cash Incentive
        /// </summary>
        [Column("RepCashIncentive")]
        public decimal? RepCashIncentive { get; set; }

        /// <summary>
        /// Gets the PromptPay Discount
        /// </summary>
        [Column("PromptPayDiscount")]
        public decimal? PromptPayDiscount { get; set; }

        /// <summary>
        /// Gets the SC Judgement Trans
        /// </summary>
        [Column("SCJudgementTrans")]
        public decimal? SCJudgementTrans { get; set; }

        /// <summary>
        /// Gets the TMO Reversal
        /// </summary>
        [Column("TMOReversal")]
        public decimal? TMOReversal { get; set; }

        /// <summary>
        /// Gets the Rent Waiver
        /// </summary>
        [Column("RentWaiver")]
        public decimal? RentWaiver { get; set; }

        /// <summary>
        /// Gets the Write On
        /// </summary>
        [Column("WriteOn")]
        public decimal? WriteOn { get; set; }

    }

}
