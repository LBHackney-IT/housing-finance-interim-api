using Newtonsoft.Json.Converters;

namespace HousingFinanceInterimApi.JsonConverters
{

    /// <summary>
    /// Allows us to determine a format to serialize/deserialize with.
    /// </summary>
    /// <seealso cref="IsoDateTimeConverter" />
    public class DateFormatConverter : IsoDateTimeConverter
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DateFormatConverter"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }

    }

}
