using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{

    /// <summary>
    /// The current rent position entity.
    /// </summary>
    [Table("SSGarage")]
    public class Garage
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
        /// Gets or sets the property type.
        /// </summary>
        [Column("PropertyType")]
        public string PropertyType { get; set; }

        /// <summary>
        /// Gets or sets the area office.
        /// </summary>
        [Column("AreaOffice")]
        public string AreaOffice { get; set; }

        /// <summary>
        /// Gets or sets the patch.
        /// </summary>
        [Column("Patch")]
        public string Patch { get; set; }

        /// <summary>
        /// Gets or sets the fund.
        /// </summary>
        [Column("Fund")]
        public string Fund { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [Column("Status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the void date.
        /// </summary>
        [Column("VoidDate")]
        public DateTime? VoidDate { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the tenancy date.
        /// </summary>
        [Column("TenancyDate")]
        public DateTime? TenancyDate { get; set; }

        /// <summary>
        /// Gets or sets the tenure.
        /// </summary>
        [Column("Tenure")]
        public string Tenure { get; set; }

        /// <summary>
        /// Gets or sets the address line 1.
        /// </summary>
        [Column("AddressLine1")]
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Gets or sets the address line 2.
        /// </summary>
        [Column("AddressLine2")]
        public string AddressLine2 { get; set; }

        /// <summary>
        /// Gets or sets the address line 3.
        /// </summary>
        [Column("AddressLine3")]
        public string AddressLine3 { get; set; }

        /// <summary>
        /// Gets or sets the Post Code.
        /// </summary>
        [Column("PostCode")]
        public string PostCode { get; set; }

        /// <summary>
        /// Gets or sets the rent account.
        /// </summary>
        [Column("RentAccount")]
        public string RentAccount { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        [Column("Comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the DD case.
        /// </summary>
        [Column("DdCase")]
        public string DdCase { get; set; }

        /// <summary>
        /// Gets or sets the tenancy agreement ref.
        /// </summary>
        [Column("TenancyAgreementRef")]
        public string TenancyAgreementRef { get; set; }

        /// <summary>
        /// Gets or sets the balance at 11 Oct 2020.
        /// </summary>
        [Column("Balance11Oct20")]
        public decimal? Balance11Oct20 { get; set; }

        /// <summary>
        /// Gets or sets the rent.
        /// </summary>
        [Column("Rent")]
        public decimal? Rent { get; set; }

        /// <summary>
        /// Gets or sets the VAT.
        /// </summary>
        [Column("VAT")]
        public decimal? VAT { get; set; }

        /// <summary>
        /// Gets or sets the current total.
        /// </summary>
        [Column("CurrentTotal")]
        public decimal? CurrentTotal { get; set; }

        /// <summary>
        /// Gets or sets the new rent.
        /// </summary>
        [Column("NewRent")]
        public decimal? NewRent { get; set; }

        /// <summary>
        /// Gets or sets the new VAT.
        /// </summary>
        [Column("NewVAT")]
        public decimal? NewVAT { get; set; }

        /// <summary>
        /// Gets or sets the new total.
        /// </summary>
        [Column("NewTotal")]
        public decimal? NewTotal { get; set; }

        /// <summary>
        /// Gets or sets the corr 1.
        /// </summary>
        [Column("Corr1")]
        public string Corr1 { get; set; }

        /// <summary>
        /// Gets or sets the corr 2.
        /// </summary>
        [Column("Corr2")]
        public string Corr2 { get; set; }

        /// <summary>
        /// Gets or sets the corr 3.
        /// </summary>
        [Column("Corr3")]
        public string Corr3 { get; set; }

        /// <summary>
        /// Gets or sets the corr postcode.
        /// </summary>
        [Column("CorrPostCode")]
        public string CorrPostCode { get; set; }

    }

}
