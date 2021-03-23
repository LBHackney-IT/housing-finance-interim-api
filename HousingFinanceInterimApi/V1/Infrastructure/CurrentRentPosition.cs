using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The current rent position entity.
    /// </summary>
    [Table("SSCurrentRentPosition")]
    public class CurrentRentPosition
    {

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the property reference.
        /// </summary>
        [Column("PropertyRef")]
        public string PropertyRef { get; set; }

        /// <summary>
        /// Gets or sets the payment reference.
        /// </summary>
        [Column("PaymentRef")]
        public string PaymentRef { get; set; }

        /// <summary>
        /// Gets or sets the week53 closing balance.
        /// </summary>
        [Column("Week53Year20ClosingBalance")]
        public decimal? Week53ClosingBalance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [universal credit].
        /// </summary>
        [Column("UniversalCredit")]
        public bool UniversalCredit { get; set; }

        /// <summary>
        /// Gets or sets the universal housing reference.
        /// </summary>
        [Column("TenancyAgreementRef")]
        public string UniversalHousingReference { get; set; }

        /// <summary>
        /// Gets or sets the hb claim reference.
        /// </summary>
        [Column("HBClaimRef")]
        public string HBClaimRef { get; set; }

        /// <summary>
        /// Gets or sets the week1.
        /// </summary>
        [Column("Week1Balance")]
        public decimal? Week1 { get; set; }

        /// <summary>
        /// Gets or sets the week27 balance.
        /// </summary>
        [Column("Week27Balance")]
        public decimal? Week27Balance { get; set; }

        /// <summary>
        /// Gets or sets the total rent.
        /// </summary>
        [Column("TotalRent")]
        public decimal? TotalRent { get; set; }

        /// <summary>
        /// Gets or sets the hackney borough for w C12 oct2020.
        /// </summary>
        [Column("HBwc12Oct20")]
        public decimal? HackneyBoroughForWC12Oct2020 { get; set; }

        /// <summary>
        /// Gets or sets the subsequent weekly hackney borough.
        /// </summary>
        [Column("SubsequentWeeklyHB")]
        public decimal? SubsequentWeeklyHackneyBorough { get; set; }

        /// <summary>
        /// Gets or sets the net rent.
        /// </summary>
        [Column("NetRent")]
        public decimal? NetRent { get; set; }

        /// <summary>
        /// Gets or sets the direct debit date.
        /// </summary>
        [Column("DirectDebitDate")]
        public int? DirectDebitDate { get; set; }

        /// <summary>
        /// Gets or sets the week28 payment.
        /// </summary>
        [Column("Week28Payment")]
        public decimal? Week28Payment { get; set; }

        /// <summary>
        /// Gets or sets the week29 payment.
        /// </summary>
        [Column("Week29Payment")]
        public decimal? Week29Payment { get; set; }

        /// <summary>
        /// Gets or sets the week30 payment.
        /// </summary>
        [Column("Week30Payment")]
        public decimal? Week30Payment { get; set; }

        /// <summary>
        /// Gets or sets the week31 payment.
        /// </summary>
        [Column("Week31Payment")]
        public decimal? Week31Payment { get; set; }

        /// <summary>
        /// Gets or sets the week32 payment.
        /// </summary>
        [Column("Week32Payment")]
        public decimal? Week32Payment { get; set; }

        /// <summary>
        /// Gets or sets the week33 payment.
        /// </summary>
        [Column("Week33Payment")]
        public decimal? Week33Payment { get; set; }

        /// <summary>
        /// Gets or sets the week34 payment.
        /// </summary>
        [Column("Week34Payment")]
        public decimal? Week34Payment { get; set; }

        /// <summary>
        /// Gets or sets the week35 payment.
        /// </summary>
        [Column("Week35Payment")]
        public decimal? Week35Payment { get; set; }

        /// <summary>
        /// Gets or sets the week36 payment.
        /// </summary>
        [Column("Week36Payment")]
        public decimal? Week36Payment { get; set; }

        /// <summary>
        /// Gets or sets the week37 payment.
        /// </summary>
        [Column("Week37Payment")]
        public decimal? Week37Payment { get; set; }

        /// <summary>
        /// Gets or sets the week38 payment.
        /// </summary>
        [Column("Week38Payment")]
        public decimal? Week38Payment { get; set; }

        /// <summary>
        /// Gets or sets the week39 payment.
        /// </summary>
        [Column("Week39Payment")]
        public decimal? Week39Payment { get; set; }

        /// <summary>
        /// Gets or sets the week40 payment.
        /// </summary>
        [Column("Week40Payment")]
        public decimal? Week40Payment { get; set; }

        /// <summary>
        /// Gets or sets the week41 payment.
        /// </summary>
        [Column("Week41Payment")]
        public decimal? Week41Payment { get; set; }

        /// <summary>
        /// Gets or sets the week42 payment.
        /// </summary>
        [Column("Week42Payment")]
        public decimal? Week42Payment { get; set; }

        /// <summary>
        /// Gets or sets the week43 payment.
        /// </summary>
        [Column("Week43Payment")]
        public decimal? Week43Payment { get; set; }

        /// <summary>
        /// Gets or sets the week44 payment.
        /// </summary>
        [Column("Week44Payment")]
        public decimal? Week44Payment { get; set; }

        /// <summary>
        /// Gets or sets the week45 payment.
        /// </summary>
        [Column("Week45Payment")]
        public decimal? Week45Payment { get; set; }

        /// <summary>
        /// Gets or sets the week46 payment.
        /// </summary>
        [Column("Week46Payment")]
        public decimal? Week46Payment { get; set; }

        /// <summary>
        /// Gets or sets the week47 payment.
        /// </summary>
        [Column("Week47Payment")]
        public decimal? Week47Payment { get; set; }

        /// <summary>
        /// Gets or sets the week48 payment.
        /// </summary>
        [Column("Week48Payment")]
        public decimal? Week48Payment { get; set; }

        /// <summary>
        /// Gets or sets the week49 payment.
        /// </summary>
        [Column("Week49Payment")]
        [NotMapped]
        public decimal? Week49Payment { get; set; }

        /// <summary>
        /// Gets or sets the week50 payment.
        /// </summary>
        [Column("Week50Payment")]
        public decimal? Week50Payment { get; set; }

        /// <summary>
        /// Gets or sets the week51 payment.
        /// </summary>
        [Column("Week51Payment")]
        public decimal? Week51Payment { get; set; }

        /// <summary>
        /// Gets or sets the week52 payment.
        /// </summary>
        [Column("Week52Payment")]
        public decimal? Week52Payment { get; set; }

        /// <summary>
        /// Gets or sets the estimated balance.
        /// </summary>
        [Column("EstimatedBalance")]
        public decimal EstimatedBalance { get; set; }

        /// <summary>
        /// Gets or sets the increase arrears since week27.
        /// </summary>
        [Column("IncreaseArrearsSinceWeek27")]
        public decimal? IncreaseArrearsSinceWeek27 { get; set; }

        /// <summary>
        /// Gets or sets the tenant.
        /// </summary>
        [Column("Tenant")]
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the tenancy start date.
        /// </summary>
        [Column("TenancyStartDate")]
        public DateTimeOffset? TenancyStartDate { get; set; }

        /// <summary>
        /// Gets or sets the type of the tenancy.
        /// </summary>
        [Column("TenancyType")]
        public string TenancyType { get; set; }

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        [Column("DateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the home telephone.
        /// </summary>
        [Column("HomeTel")]
        public string HomeTelephone { get; set; }

        /// <summary>
        /// Gets or sets the mobile.
        /// </summary>
        [Column("Mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets the address line1.
        /// </summary>
        [Column("AddressLine1")]
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Gets or sets the address line2.
        /// </summary>
        [Column("AddressLine2")]
        public string AddressLine2 { get; set; }

        /// <summary>
        /// Gets or sets the address line3.
        /// </summary>
        [Column("AddressLine3")]
        public string AddressLine3 { get; set; }

        /// <summary>
        /// Gets or sets the postcode.
        /// </summary>
        [Column("PostCode")]
        public string Postcode { get; set; }

    }

}
