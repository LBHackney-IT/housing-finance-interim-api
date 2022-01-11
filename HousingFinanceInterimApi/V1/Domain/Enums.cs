using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace HousingFinanceInterimApi.V1.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RentGroup
    {
        [Description("Gar & Park HRA")]
        GarParkHRA,
        [Description("Housing Gen Fund")]
        HousingGenFund,
        [Description("Housing Revenue")]
        HousingRevenue,
        [Description("LH Major Works")]
        LHMajorWorks,
        [Description("LH Serv Charges")]
        LHServCharges,
        [Description("Temp Acc Gen Fun")]
        TempAccGenFun,
        [Description("Temp Accom HRA")]
        TempAccomHRA,
        [Description("Travel Gen Fund")]
        TravelGenFund
    }
}
