using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The current rent position entity.
    /// </summary>
    [Table("SSLeaseholdAccount")]
    public class LeaseholdAccount
    {

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the tenancy agreement reference.
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
        /// Gets or sets the rent group.
        /// </summary>
        [Column("RentGroup")]
        public string RentGroup { get; set; }

        /// <summary>
        /// Gets or sets the tenure.
        /// </summary>
        [Column("Tenure")]
        public string Tenure { get; set; }

        /// <summary>
        /// Gets or sets the assignment start date.
        /// </summary>
        [Column("AssignmentStartDate")]
        public DateTime? AssignmentStartDate { get; set; }

        /// <summary>
        /// Gets or sets the assignment end date.
        /// </summary>
        [Column("AssignmentEndDate")]
        public DateTime? AssignmentEndDate { get; set; }

        /// <summary>
        /// Gets or sets the date sold leased.
        /// </summary>
        [Column("SoldLeasedDate")]
        public DateTime? SoldLeasedDate { get; set; }

        /// <summary>
        /// Gets or sets the account type.
        /// </summary>
        [Column("AccountType")]
        public string AccountType { get; set; }

        /// <summary>
        /// Gets or sets the agreement_type.
        /// </summary>
        [Column("AgreementType")]
        public string AgreementType { get; set; }

        /// <summary>
        /// Gets or sets the balance.
        /// </summary>
        [Column("Balance")]
        public decimal? Balance { get; set; }

        /// <summary>
        /// Gets or sets the lessee.
        /// </summary>
        [Column("Lessee")]
        public string Lessee { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        [Column("Address")]
        public string Address { get; set; }

    }

}
