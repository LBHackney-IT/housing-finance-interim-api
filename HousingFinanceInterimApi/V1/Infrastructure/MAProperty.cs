using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    [Table("MAProperty")]
    public class MAProperty
    {
        [Column("prop_ref")]
        [StringLength(12)]
        public string PropRef { get; set; }

        [Column("major_ref")]
        [StringLength(12)]
        public string MajorRef { get; set; }

        [Column("man_scheme")]
        [StringLength(11)]
        public string ManScheme { get; set; }

        [Column("post_code")]
        [StringLength(10)]
        public string PostCode { get; set; }

        [Column("short_address")]
        [StringLength(200)]
        public string ShortAddress { get; set; }

        [Column("telephone")]
        [StringLength(21)]
        public string Telephone { get; set; }

        [Column("ownership")]
        [StringLength(10)]
        public string Ownership { get; set; }

        [Column("agent")]
        [StringLength(3)]
        public string Agent { get; set; }

        [Column("area_office")]
        [StringLength(3)]
        public string AreaOffice { get; set; }

        [Column("subtyp_code")]
        [StringLength(3)]
        public string SubtypeCode { get; set; }

        [Column("letable")]       
        public bool Letable { get; set; }

        [Column("cat_type")]
        [StringLength(3)]
        public string CatType { get; set; }

        [Column("house_ref")]
        [StringLength(10)]
        public string HouseRef { get; set; }

        [Column("occ_stat")]
        [StringLength(3)]
        public string OccStatus { get; set; }

        [Column("post_preamble")]
        [StringLength(60)]
        public string PostPreamble { get; set; }

        [Column("property_sid")]      
        public int PropertySid { get; set; }

        [Column("arr_patch")]
        [StringLength(3)]
        public string ArrPatch { get; set; }

        [Column("address1")]
        [StringLength(255)]
        public string Address1 { get; set; }

        [Column("num_bedrooms")]
        public int NumBedrooms { get; set; }

        [Column("post_desig")]
        [StringLength(60)]
        public string PostDesig { get; set; }
    }
}
