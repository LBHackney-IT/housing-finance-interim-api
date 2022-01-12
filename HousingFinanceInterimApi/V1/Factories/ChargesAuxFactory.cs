using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace HousingFinanceInterimApi.V1.Factories
{
    public static class ChargesAuxFactory
    {
        public static ChargesAuxDomain ToDomain(this ChargesAux chargesAux)
        {
            if (chargesAux == null)
                return null;

            return new ChargesAuxDomain
            {
                Id = chargesAux.Id,
                PropertyRef = chargesAux.PropertyRef,
                DAT = chargesAux.DAT,
                DBR = chargesAux.DBR,
                DC4 = chargesAux.DC4,
                DC5 = chargesAux.DC5,
                DCB = chargesAux.DCB,
                DCC = chargesAux.DCC,
                DCE = chargesAux.DCE,
                DCI = chargesAux.DCI,
                DCO = chargesAux.DCO,
                DCP = chargesAux.DCP,
                DCT = chargesAux.DCT,
                DGA = chargesAux.DGA,
                DGM = chargesAux.DGM,
                DGR = chargesAux.DGR,
                DHA = chargesAux.DHA,
                DHE = chargesAux.DHE,
                DHM = chargesAux.DHM,
                DIN = chargesAux.DIN,
                DIT = chargesAux.DIT,
                DKF = chargesAux.DKF,
                DLL = chargesAux.DLL,
                DLP = chargesAux.DLP,
                DMC = chargesAux.DMC,
                DMJ = chargesAux.DMJ,
                DMR = chargesAux.DMR,
                DR5 = chargesAux.DR5,
                DRP = chargesAux.DRP,
                DRR = chargesAux.DRR,
                DSA = chargesAux.DSA,
                DSB = chargesAux.DSB,
                DSC = chargesAux.DSC,
                DSJ = chargesAux.DSJ,
                DSO = chargesAux.DSO,
                DSR = chargesAux.DSR,
                DST = chargesAux.DST,
                DTA = chargesAux.DTA,
                DTC = chargesAux.DTC,
                DTL = chargesAux.DTL,
                DTV = chargesAux.DTV,
                DVA = chargesAux.DVA,
                DWR = chargesAux.DWR,
                DWS = chargesAux.DWS,
                DWW = chargesAux.DWW,
                RCI = chargesAux.RCI,
                RPD = chargesAux.RPD,
                RSJ = chargesAux.RSJ,
                RTM = chargesAux.RTM,
                RWA = chargesAux.RWA,
                WON = chargesAux.WON,
                TimeStamp = chargesAux.TimeStamp
            };
        }

        public static List<ChargesAuxDomain> ToDomain(
            this ICollection<ChargesAux> chargesAux)
        {
            return chargesAux?.Select(c => c.ToDomain()).ToList();
        }
    }
}
