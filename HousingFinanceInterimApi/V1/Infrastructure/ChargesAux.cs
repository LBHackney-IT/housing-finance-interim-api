using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingFinanceInterimApi.V1.Infrastructure
{
    [Table("ChargesAux")]
    public class ChargesAux
    {
        [Key]
        public long Id { get; set; }

        public string PropertyRef { get; set; }

        public string RentGroup { get; set; }

        public decimal DAT { get; set; }

        public decimal DBR { get; set; }

        public decimal DC4 { get; set; }

        public decimal DC5 { get; set; }

        public decimal DCB { get; set; }

        public decimal DCC { get; set; }

        public decimal DCE { get; set; }

        public decimal DCI { get; set; }

        public decimal DCO { get; set; }

        public decimal DCP { get; set; }

        public decimal DCT { get; set; }

        public decimal DGA { get; set; }

        public decimal DGM { get; set; }

        public decimal DGR { get; set; }

        public decimal DHA { get; set; }

        public decimal DHE { get; set; }

        public decimal DHM { get; set; }

        public decimal DIN { get; set; }

        public decimal DIT { get; set; }

        public decimal DKF { get; set; }

        public decimal DLL { get; set; }

        public decimal DLP { get; set; }

        public decimal DMC { get; set; }

        public decimal DMJ { get; set; }

        public decimal DMR { get; set; }

        public decimal DR5 { get; set; }

        public decimal DRP { get; set; }

        public decimal DRR { get; set; }

        public decimal DSA { get; set; }

        public decimal DSB { get; set; }

        public decimal DSC { get; set; }

        public decimal DSJ { get; set; }

        public decimal DSO { get; set; }

        public decimal DSR { get; set; }

        public decimal DST { get; set; }

        public decimal DTA { get; set; }

        public decimal DTC { get; set; }

        public decimal DTL { get; set; }

        public decimal DTV { get; set; }

        public decimal DVA { get; set; }

        public decimal DWR { get; set; }

        public decimal DWS { get; set; }

        public decimal DWW { get; set; }

        public decimal RCI { get; set; }

        public decimal RPD { get; set; }

        public decimal RSJ { get; set; }

        public decimal RTM { get; set; }

        public decimal RWA { get; set; }

        public decimal WON { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTimeOffset TimeStamp { get; set; }
    }
}
