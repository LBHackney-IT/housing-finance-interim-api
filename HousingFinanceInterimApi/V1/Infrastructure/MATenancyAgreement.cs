using Microsoft.VisualBasic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    [Table("MATenancyAgreement")]
    public class MATenancyAgreement
    {
        [Column("TagRef")]
        public string TagRef { get; set; }

        [Column("PropRef")]
        public string PropRef { get; set; }

        [Column("HouseRef")]
        public string HouseRef { get; set; }

        [Column("TagDesc")]
        public string TagDesc { get; set; }

        [Column("Cot")]
        public DateTime? Cot { get; set; }

        [Column("Eot")]
        public DateTime? Eot { get; set; }

        [Column("Tenure")]
        public string Tenure { get; set; }

        [Column("PrdCode")]
        public string PrdCode { get; set; }

        [Column("Present")]
        public bool Present { get; set; }

        [Column("Terminated")]
        public bool Terminated { get; set; }

        [Column("RentGrpRef")]
        public string RentGrpRef { get; set; }

        [Column("Rent")]
        public int? Rent { get; set; }

        [Column("Service")]
        public int? Service { get; set; }

        [Column("OtherCharge")]
        public int? OtherCharge { get; set; }

        [Column("TenancyRent")]
        public int? TenancyRent { get; set; }

        [Column("TenancyService")]
        public int? TenancyService { get; set; }

        [Column("TenancyOther")]
        public int? TenancyOtherCharge { get; set; }

        [Column("CurrentBalance")]
        public int? CurrentBalance { get; set; }

        [Column("CurrentNrBalance")]
        public int? CurrentNrBalance { get; set; }

        [Column("OccStatus")]
        public string OccStatus { get; set; }

        [Column("TenagreeSid")]
        public string TenagreeSid { get; set; }

        [Column("USaffRentAcc")]
        public string USaffRentAcc { get; set; }

        [Column("HighAction")]
        public string HighAction { get; set; }

        [Column("UNoticeServiced")]
        public DateTime? UNoticeServiced { get; set; }

        [Column("CourtDate")]
        public DateTime? CourtDate { get; set; }

        [Column("UCourtOutcome")]
        public string UCourtOutcome { get; set; }

        [Column("EvictDate")]
        public DateTime? EvictDate { get; set; }

        [Column("AgrType")]
        public string AgrType { get; set; }

        [Column("RechTagRef")]
        public string RechTagRef { get; set; }

        [Column("MasterTagRef")]
        public string MasterTagRef { get; set; }
    }
}
