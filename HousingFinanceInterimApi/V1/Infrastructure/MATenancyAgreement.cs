using Microsoft.VisualBasic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    [Table("MATenancyAgreement")]
    public class MATenancyAgreement
    {
        [Column("tag_ref")]
        public string TagRef { get; set; }

        [Column("prop_ref")]
        public string PropRef { get; set; }

        [Column("house_ref")]
        public string HouseRef { get; set; }

        [Column("tag_desc")]
        public string TagDesc { get; set; }

        [Column("cot")]
        public DateTime? Cot { get; set; }

        [Column("eot")]
        public DateTime? Eot { get; set; }

        [Column("tenure")]
        public string Tenure { get; set; }

        [Column("prd_code")]
        public string PrdCode { get; set; }

        [Column("present")]
        public bool Present { get; set; }

        [Column("terminated")]
        public bool Terminated { get; set; }

        [Column("rentgrp_ref")]
        public string RentGrpRef { get; set; }

        [Column("rent")]
        public int? Rent { get; set; }

        [Column("service")]
        public int? Service { get; set; }

        [Column("other_charge")]
        public int? OtherCharge { get; set; }

        [Column("tenancy_rent")]
        public int? TenancyRent { get; set; }

        [Column("tenancy_service")]
        public int? TenancyService { get; set; }

        [Column("tenancy_other")]
        public int? TenancyOtherCharge { get; set; }

        [Column("cur_bal")]
        public int? CurrentBalance { get; set; }

        [Column("cur_nr_bal")]
        public int? CurrentNrBalance { get; set; }

        [Column("occ_status")]
        public string OccStatus { get; set; }

        [Column("tenagree_sid")]
        public string TenagreeSid { get; set; }

        [Column("u_saff_rentacc")]
        public string USaffRentAcc { get; set; }

        [Column("high_action")]
        public string HighAction { get; set; }

        [Column("u_notice_served")]
        public DateTime? UNoticeServiced { get; set; }

        [Column("courtdate")]
        public DateTime? CourtDate { get; set; }

        [Column("u_court_outcome")]
        public string UCourtOutcome { get; set; }

        [Column("evictdate")]
        public DateTime? EvictDate { get; set; }

        [Column("agr_type")]
        public string AgrType { get; set; }

        [Column("rech_tag_ref")]
        public string RechTagRef { get; set; }

        [Column("master_tag_ref")]
        public string MasterTagRef { get; set; }
    }
}
