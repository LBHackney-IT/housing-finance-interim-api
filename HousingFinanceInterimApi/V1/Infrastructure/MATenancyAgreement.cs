using Microsoft.VisualBasic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    [Table("MATenancyAgreement")]
    public class MATenancyAgreement
    {
        [Column("tag_ref")]
        public string TenancyAgreementRef { get; set; }

        [Column("prop_ref")]
        public string PropertyRef { get; set; }

        [Column("house_ref")]
        public string HouseRef { get; set; }

        [Column("tag_desc")]
        public string TagDesc { get; set; }

        [Column("cot")]
        public DateTime? StartOfTenure { get; set; }

        [Column("eot")]
        public DateTime? EndOfTenure { get; set; }

        [Column("tenure")]
        public string TenureType { get; set; }

        [Column("prd_code")]
        public string PrdCode { get; set; }
            
        [Column("present")]
        public bool IsPresent { get; set; }

        [Column("terminated")]
        public bool IsTerminated { get; set; }

        [Column("rentgrp_ref")]
        public string RentGroupRef { get; set; }

        [Column("rent")]
        public decimal? Rent { get; set; }

        [Column("service")]
        public decimal? Service { get; set; }

        [Column("other_charge")]
        public decimal? OtherCharge { get; set; }

        [Column("tenancy_rent")]
        public decimal? TenancyRent { get; set; }

        [Column("tenancy_service")]
        public decimal? TenancyServiceCharge { get; set; }

        [Column("tenancy_other")]
        public decimal? TenancyOtherCharge { get; set; }

        [Column("cur_bal")]
        public decimal? CurrentBalance { get; set; }

        [Column("cur_nr_bal")]
        public decimal? CurrentNrBalance { get; set; }

        [Column("occ_status")]
        public string OccStatus { get; set; }

        [Column("tenagree_sid")]
        public int TenancyAgreementNumberSid { get; set; }

        [Column("u_saff_rentacc")]
        public string RentAccountNumber { get; set; }

        [Column("high_action")]
        public string HighAction { get; set; }

        [Column("u_notice_served")]
        public DateTime? NoticeServiced { get; set; }

        [Column("courtdate")]
        public DateTime? CourtDate { get; set; }

        [Column("u_court_outcome")]
        public string CourtOutcome { get; set; }

        [Column("evictdate")]
        public DateTime? EvictDate { get; set; }

        [Column("agr_type")]
        public string AgrType { get; set; }
    }
}
