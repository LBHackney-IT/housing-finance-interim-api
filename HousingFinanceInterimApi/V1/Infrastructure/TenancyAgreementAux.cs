using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    [Table("TenancyAgreementAux")]
    public class TenancyAgreementAux
    {
        [Key]
        public long Id { get; set; }

        public string PaymentRef { get; set; }

        public string RentGroup { get; set; }

        public string Tenure { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string PropertyRef { get; set; }

        public string ShortAddress { get; set; }

        public string Address { get; set; }

        public string PostCode { get; set; }

        public int? NumBedrooms { get; set; }

        public string Title { get; set; }

        public string Forename { get; set; }

        public string Surname { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset TimeStamp { get; set; }
    }
}
