using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HousingFinanceInterimApi.V1.Infrastructure
{
    [Table("SSMiniTransaction")]
    public class SSMiniTransaction
    {
        [StringLength(11)]
        public string TagRef { get; set; }

        [StringLength(12)]
        public string PropRef { get; set; }

        [StringLength(3)]
        public string RentGroup { get; set; }
        public int PostYear { get; set; }

        [Column(TypeName = "decimal(3, 0)")]
        public decimal PostPrdno { get; set; }

        [StringLength(3)]
        public string Tenure { get; set; }

        [StringLength(3)]
        public string TransType { get; set; }

        [StringLength(3)]
        public string TransSrc { get; set; }

        [Column(TypeName = "decimal(9, 2)")]
        public decimal RealValue { get; set; }

        public DateTime PostDate { get; set; }

        [StringLength(12)]
        public string TransRef { get; set; }

        public string Origin { get; set; }

        public string OriginDesc { get; set; }
    }
}
