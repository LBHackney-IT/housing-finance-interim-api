using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HousingFinanceInterimApi.V1.Controllers
{

    [Route("api/v1/[controller]")]
    public class BaseController : Controller
    {

        public BaseController()
        {
            ConfigureJsonSerializer();
        }

        public static void ConfigureJsonSerializer()
        {
            JsonConvert.DefaultSettings = () =>
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat
                };

                return settings;
            };
        }

    }

}
