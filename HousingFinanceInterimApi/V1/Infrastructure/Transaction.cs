namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The operating balance entity.
    /// </summary>
    public class Transaction
    {
        public string RentGroup { get; set; }
        public int Year { get; set; }
        public decimal WeekMonth { get; set; }
        public decimal TotalCharged { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalHBPaid { get; set; }
        public decimal AssignmentSCTrans { get; set; }
        public decimal BasicRentNoVAT { get; set; }
        public decimal CProfessionalFees { get; set; }
        public decimal CAdministration { get; set; }
        public decimal CleaningBlock { get; set; }
        public decimal CourtCosts { get; set; }
        public decimal CleaningEstate { get; set; }
        public decimal ContentsInsurance { get; set; }
        public decimal Concierge { get; set; }
        public decimal CarPort { get; set; }
        public decimal CommunalDigitalTV { get; set; }
        public decimal GarageAttached { get; set; }
        public decimal GroundsMaintenance { get; set; }
        public decimal GroundRent { get; set; }
        public decimal HostAmenity { get; set; }
        public decimal Heating { get; set; }
        public decimal HeatingMaintenance { get; set; }
        public decimal Interest { get; set; }
        public decimal ArrangementInterest { get; set; }
        public decimal LostKeyFobs { get; set; }
        public decimal LandlordLighting { get; set; }
        public decimal LatePaymentCharge { get; set; }
        public decimal MajorWorksCapital { get; set; }
        public decimal MWJudgementTrans { get; set; }
        public decimal MajorWorksRevenue { get; set; }
        public decimal RAdministrationFee { get; set; }
        public decimal RechgRepairsNoVAT { get; set; }
        public decimal RechargeableRepairs { get; set; }
        public decimal SCAdjustment { get; set; }
        public decimal SCBalancingCharge { get; set; }
        public decimal ServiceCharges { get; set; }
        public decimal SCJudgementDebit { get; set; }
        public decimal SharedOwnersRent { get; set; }
        public decimal ReserveFund { get; set; }
        public decimal Storage { get; set; }
        public decimal BasicRentTempAcc { get; set; }
        public decimal TravellersCharge { get; set; }
        public decimal TenantsLevy { get; set; }
        public decimal TelevisionLicense { get; set; }
        public decimal VATCharge { get; set; }
        public decimal WaterRates { get; set; }
        public decimal WaterStandingChrg { get; set; }
        public decimal WatersureReduction { get; set; }
        public decimal RepCashIncentive { get; set; }
        public decimal PromptPayDiscount { get; set; }
        public decimal SCJudgementTrans { get; set; }
        public decimal TMOReversal { get; set; }
        public decimal Rentwaiver { get; set; }
        public decimal WriteOn { get; set; }
        public decimal BailiffPayment { get; set; }
        public decimal BankPayment { get; set; }
        public decimal PayPointPostOffice { get; set; }
        public decimal CashOfficePayments { get; set; }
        public decimal DebitCreditCard { get; set; }
        public decimal DirectDebit { get; set; }
        public decimal DirectDebitUnpaid { get; set; }
        public decimal BACSRefund { get; set; }
        public decimal DeductionSalary { get; set; }
        public decimal DSSTransfer { get; set; }
        public decimal InternalTransfer { get; set; }
        public decimal PayPointPostOffice2 { get; set; }
        public decimal ChequePayments { get; set; }
        public decimal ReturnedCheque { get; set; }
        public decimal HBAdjustment { get; set; }
        public decimal HousingBenefit { get; set; }
    }

}
