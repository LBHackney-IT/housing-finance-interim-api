using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The current rent position entity.
    /// </summary>
    [Table("SSServiceChargePaymentsReceived")]
    public class ServiceChargePaymentsReceived
    {

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }


        /// <summary>
        /// Gets or sets the patch.
        /// </summary>
        [Column("ArrearPatch")]
        public string ArrearPatch { get; set; }

        /// <summary>
        /// Gets or sets the patch.
        /// </summary>
        [Column("TenancyAgreementRef")]
        public string TenancyAgreementRef { get; set; }

        /// <summary>
        /// Gets or sets the payment reference.
        /// </summary>
        [Column("PaymentRef")]
        public string PaymentRef { get; set; }

        /// <summary>
        /// Gets or sets the property reference.
        /// </summary>
        [Column("PropertyRef")]
        public string PropertyRef { get; set; }

        /// <summary>
        /// Gets or sets the tenure.
        /// </summary>
        [Column("Tenancy")]
        public string Tenancy { get; set; }

        /// <summary>
        /// Gets or sets the tenure.
        /// </summary>
        [Column("Tenant")]
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the address line1.
        /// </summary>
        [Column("Address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the address line1.
        /// </summary>
        [Column("DirectDebitDate")]
        public string DirectDebitDate { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [Column("MonthlyDebit")]
        public decimal? MonthlyDebit { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [Column("Sep20DebitToIncludeActuals")]
        public decimal? Sep20DebitToIncludeActuals { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [Column("AdjustmentsToSCDebits")]
        public decimal? AdjustmentsToSCDebits { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [Column("DirectDebits15and23Nov20")]
        public decimal? DirectDebits15and23Nov20 { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [Column("DirectDebitDec20")]
        public decimal? DirectDebitDec20 { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [Column("Balance30Sep20")]
        public decimal? Balance30Sep20 { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [Column("OctoberMovedToJudgement")]
        public decimal? OctoberMovedToJudgement { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [Column("JanuaryMovedToJudgement")]
        public decimal? JanuaryMovedToJudgement { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [Column("FebruaryMovedToJudgement")]
        public decimal? FebruaryMovedToJudgement { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [Column("MarchMovedToJudgement")]
        public decimal? MarchMovedToJudgement { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [Column("OctoberSCandMWTransfers")]
        public decimal? OctoberSCandMWTransfers { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [Column("MarchSCandMWTransfers")]
        public decimal? MarchSCandMWTransfers { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [Column("OctoberPayments")]
        public decimal? OctoberPayments { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [Column("NovemberPayments")]
        public decimal? NovemberPayments { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [Column("DecemberPayments")]
        public decimal? DecemberPayments { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [Column("JanuaryPayments")]
        public decimal? JanuaryPayments { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [Column("FebruaryPayments")]
        public decimal? FebruaryPayments { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [Column("MarchPayments")]
        public decimal? MarchPayments { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [Column("DisputedAmount")]
        public decimal? DisputedAmount { get; set; }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        [Column("BalanceIncludingDisputedAmount")]
        public decimal? BalanceIncludingDisputedAmount { get; set; }

    }

}
