using System;
using Newtonsoft.Json;

namespace HousingFinanceInterimApi.V1.Domain
{
    public class ChargesAuxDomain
    {
        public long Id { get; set; }

        [JsonProperty("Property Ref")]
        public string PropertyRef { get; set; }

        [JsonProperty("Assignment SC Trans")]
        public decimal? DAT { get; set; }

        [JsonProperty("Basic Rent (No VAT)")]
        public decimal? DBR { get; set; }

        [JsonProperty("C Professional Fees")]
        public decimal? DC4 { get; set; }

        [JsonProperty("C Administration")]
        public decimal? DC5 { get; set; }

        [JsonProperty("Cleaning (Block)")]
        public decimal? DCB { get; set; }

        [JsonProperty("Court Costs")]
        public decimal? DCC { get; set; }

        [JsonProperty("Cleaning (Estate)")]
        public decimal? DCE { get; set; }

        [JsonProperty("Contents Insurance")]
        public decimal? DCI { get; set; }

        [JsonProperty("Concierge")]
        public decimal? DCO { get; set; }

        [JsonProperty("Car Port")]
        public decimal? DCP { get; set; }

        [JsonProperty("Communal Digital TV")]
        public decimal? DCT { get; set; }

        [JsonProperty("Garage (Attached)")]
        public decimal? DGA { get; set; }

        [JsonProperty("Grounds Maintenance")]
        public decimal? DGM { get; set; }

        [JsonProperty("Ground Rent")]
        public decimal? DGR { get; set; }

        [JsonProperty("Host Amenity")]
        public decimal? DHA { get; set; }

        [JsonProperty("Heating")]
        public decimal? DHE { get; set; }

        [JsonProperty("Heating Maintenance")]
        public decimal? DHM { get; set; }

        [JsonProperty("Interest")]
        public decimal? DIN { get; set; }

        [JsonProperty("Arrangement Interest")]
        public decimal? DIT { get; set; }

        [JsonProperty("Lost Key Fobs")]
        public decimal? DKF { get; set; }

        [JsonProperty("Landlord Lighting")]
        public decimal? DLL { get; set; }

        [JsonProperty("Late Payment Charge")]
        public decimal? DLP { get; set; }

        [JsonProperty("Major Works Capital")]
        public decimal? DMC { get; set; }

        [JsonProperty("MW Judgement Trans")]
        public decimal? DMJ { get; set; }

        [JsonProperty("Major Works Revenue")]
        public decimal? DMR { get; set; }

        [JsonProperty("R Administration Fee")]
        public decimal? DR5 { get; set; }

        [JsonProperty("Rechg Repairs no VAT")]
        public decimal? DRP { get; set; }

        [JsonProperty("Rechargeable Repairs")]
        public decimal? DRR { get; set; }

        [JsonProperty("SC Adjustment")]
        public decimal? DSA { get; set; }

        [JsonProperty("SC Balancing Charge")]
        public decimal? DSB { get; set; }

        [JsonProperty("Service Charges")]
        public decimal? DSC { get; set; }

        [JsonProperty("SC Judgement Debit")]
        public decimal? DSJ { get; set; }

        [JsonProperty("Shared Owners Rent")]
        public decimal? DSO { get; set; }

        [JsonProperty("Reserve Fund")]
        public decimal? DSR { get; set; }

        [JsonProperty("Storage")]
        public decimal? DST { get; set; }

        [JsonProperty("Basic Rent Temp Acc")]
        public decimal? DTA { get; set; }

        [JsonProperty("Travellers Charge")]
        public decimal? DTC { get; set; }

        [JsonProperty("Tenants Levy")]
        public decimal? DTL { get; set; }

        [JsonProperty("Television License")]
        public decimal? DTV { get; set; }

        [JsonProperty("VAT Charge")]
        public decimal? DVA { get; set; }

        [JsonProperty("Water Rates")]
        public decimal? DWR { get; set; }

        [JsonProperty("Water Standing Chrg")]
        public decimal? DWS { get; set; }

        [JsonProperty("Watersure Reduction")]
        public decimal? DWW { get; set; }

        [JsonProperty("Rep Cash Incentive")]
        public decimal? RCI { get; set; }

        [JsonProperty("Prompt Pay Discount")]
        public decimal? RPD { get; set; }

        [JsonProperty("SC Judgement Trans")]
        public decimal? RSJ { get; set; }

        [JsonProperty("TMO Reversal")]
        public decimal? RTM { get; set; }

        [JsonProperty("Rent waiver")]
        public decimal? RWA { get; set; }

        [JsonProperty("Write On")]
        public decimal? WON { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}
