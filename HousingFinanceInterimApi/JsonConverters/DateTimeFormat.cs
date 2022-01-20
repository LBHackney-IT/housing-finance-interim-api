using Newtonsoft.Json;
using System;
using System.Globalization;

namespace HousingFinanceInterimApi.JsonConverters
{

    /// <summary>
    /// Ensures that the value deserialized will be taken as a decimal, or else null.
    /// This allows us to convert invalid values to null instead of throwing an exception.
    /// </summary>
    /// <seealso cref="JsonConverter" />
    public class DateTimeFormat : JsonConverter
    {

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            string value = reader.Value.ToString();

            string[] formats = { "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy", "dd/M/yyyy HH:mm:ss", "dd/M/yyyy", "d/M/yyyy HH:mm:ss", "d/M/yyyy" };
            DateTime dt;
            if (DateTime.TryParseExact(value, formats, CultureInfo.CurrentCulture,
                DateTimeStyles.None, out dt))
            {
                if (dt.Date.Year >= 1900 && dt <= new DateTime(2079, 6, 6))
                {
                    return dt;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

    }

}
