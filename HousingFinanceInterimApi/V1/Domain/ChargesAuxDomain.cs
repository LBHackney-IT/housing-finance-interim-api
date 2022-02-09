using System;
using HousingFinanceInterimApi.JsonConverters;
using Newtonsoft.Json;

namespace HousingFinanceInterimApi.V1.Domain
{
    public class ChargesAuxDomain
    {
        public long Id { get; set; }

        [JsonProperty("Property Ref")]
        public string PropertyRef { get; set; }

        [JsonProperty("Assignment SC Trans")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DAT { get; set; }

        [JsonProperty("Basic Rent (No VAT)")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DBR { get; set; }

        [JsonProperty("C Professional Fees")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DC4 { get; set; }

        [JsonProperty("C Administration")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DC5 { get; set; }

        [JsonProperty("Cleaning (Block)")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DCB { get; set; }

        [JsonProperty("Court Costs")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DCC { get; set; }

        [JsonProperty("Cleaning (Estate)")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DCE { get; set; }

        [JsonProperty("Contents Insurance")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DCI { get; set; }

        [JsonProperty("Concierge")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DCO { get; set; }

        [JsonProperty("Car Port")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DCP { get; set; }

        [JsonProperty("Communal Digital TV")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DCT { get; set; }

        [JsonProperty("Garage (Attached)")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DGA { get; set; }

        [JsonProperty("Grounds Maintenance")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DGM { get; set; }

        [JsonProperty("Ground Rent")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DGR { get; set; }

        [JsonProperty("Host Amenity")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DHA { get; set; }

        [JsonProperty("Heating")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DHE { get; set; }

        [JsonProperty("Heating Maintenance")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DHM { get; set; }

        [JsonProperty("Interest")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DIN { get; set; }

        [JsonProperty("Arrangement Interest")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DIT { get; set; }

        [JsonProperty("Lost Key Fobs")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DKF { get; set; }

        [JsonProperty("Landlord Lighting")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DLL { get; set; }

        [JsonProperty("Late Payment Charge")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DLP { get; set; }

        [JsonProperty("Major Works Capital")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DMC { get; set; }

        [JsonProperty("MW Judgement Trans")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DMJ { get; set; }

        [JsonProperty("Major Works Revenue")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DMR { get; set; }

        [JsonProperty("R Administration Fee")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DR5 { get; set; }

        [JsonProperty("Rechg Repairs no VAT")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DRP { get; set; }

        [JsonProperty("Rechargeable Repairs")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DRR { get; set; }

        [JsonProperty("SC Adjustment")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DSA { get; set; }

        [JsonProperty("SC Balancing Charge")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DSB { get; set; }

        [JsonProperty("Service Charges")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DSC { get; set; }

        [JsonProperty("SC Judgement Debit")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DSJ { get; set; }

        [JsonProperty("Shared Owners Rent")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DSO { get; set; }

        [JsonProperty("Reserve Fund")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DSR { get; set; }

        [JsonProperty("Storage")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DST { get; set; }

        [JsonProperty("Basic Rent Temp Acc")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DTA { get; set; }

        [JsonProperty("Travellers Charge")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DTC { get; set; }

        [JsonProperty("Tenants Levy")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DTL { get; set; }

        [JsonProperty("Television License")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DTV { get; set; }

        [JsonProperty("VAT Charge")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DVA { get; set; }

        [JsonProperty("Water Rates")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DWR { get; set; }

        [JsonProperty("Water Standing Chrg")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DWS { get; set; }

        [JsonProperty("Watersure Reduction")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? DWW { get; set; }

        [JsonProperty("Rep Cash Incentive")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? RCI { get; set; }

        [JsonProperty("Prompt Pay Discount")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? RPD { get; set; }

        [JsonProperty("SC Judgement Trans")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? RSJ { get; set; }

        [JsonProperty("TMO Reversal")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? RTM { get; set; }

        [JsonProperty("Rent waiver")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? RWA { get; set; }

        [JsonProperty("Write On")]
        [JsonConverter(typeof(DecimalOrNull))]
        public decimal? WON { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}
