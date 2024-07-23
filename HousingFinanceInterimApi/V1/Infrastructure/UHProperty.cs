using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    [Table("UHProperty")]
    public class UHProperty
    {
        [Column("prop_ref")]
        public string PropRef { get; set; }

        [Column("major_ref")]
        public string MajorRef { get; set; }

        [Column("man_scheme")]
        public string ManScheme { get; set; }

        [Column("post_code")]
        public string PostCode { get; set; }

        [Column("short_address")]
        public string ShortAddress { get; set; }

        [Column("telephone")]
        public string Telephone { get; set; }

        [Column("ownership")]
        public string Ownership { get; set; }

        [Column("agent")]
        public string Agent { get; set; }

        [Column("area_office")]
        public string AreaOffice { get; set; }

        [Column("subtyp_code")]
        public string SubtypeCode { get; set; }

        [Column("letable")]
        public bool Letable { get; set; }

        [Column("cat_type")]
        public string CatType { get; set; }

        [Column("house_ref")]
        public string HouseRef { get; set; }

        [Column("occ_stat")]
        public string OccStatus { get; set; }

        [Column("post_preamble")]
        public string PostPreamble { get; set; }

        [Column("property_sid")]
        public int PropertySid { get; set; }

        [Column("arr_patch")]
        public string ArrPatch { get; set; }

        [Column("address1")]
        public string Address1 { get; set; }

        [Column("num_bedrooms")]
        public int NumBedrooms { get; set; }

        [Column("post_desig")]
        public string PostDesig { get; set; }
    }
}
