using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HousingFinanceInterimApi.V1.Infrastructure
{
    [Table("SSMiniTransaction")]
    public class SSMiniTransaction
    {
        [StringLength(11)]
        [Column("tag_ref")]
        public string TagRef { get; set; }

        [StringLength(12)]
        [Column("prop_ref")]
        public string PropRef { get; set; }

        [StringLength(3)]
        [Column("rentgroup")]
        public string RentGroup { get; set; }

        [Column("post_year")]
        public int PostYear { get; set; }

        [Column("post_prdno", TypeName = "decimal(3, 0)")]
        public decimal PostPrdno { get; set; }

        [StringLength(3)]
        [Column("tenure")]
        public string Tenure { get; set; }

        [StringLength(3)]
        [Column("trans_type")]
        public string TransType { get; set; }

        [StringLength(3)]
        [Column("trans_src")]
        public string TransSrc { get; set; }

        [Column("real_value", TypeName = "decimal(9, 2)")]
        public decimal RealValue { get; set; }

        [Column("post_date")]
        public DateTime PostDate { get; set; }

        [StringLength(12)]
        [Column("trans_ref")]
        public string TransRef { get; set; }

        [Column("origin")]
        public string Origin { get; set; }

        [Column("origin_desc")]
        public string OriginDesc { get; set; }
    }
}
