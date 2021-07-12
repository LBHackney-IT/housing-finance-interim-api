using Newtonsoft.Json;
using System;

namespace HousingFinanceInterimApi.JsonConverters
{

    /// <summary>
    /// The null pound currency converter.
    /// </summary>
    /// <seealso cref="JsonConverter" />
    public class NullPoundCurrencyConverter : JsonConverter
    {

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            decimal? decimalValue = (decimal?) value;

            writer.WriteValue(decimalValue.HasValue
                ? decimalValue.Value.ToString("C")
                : "");
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
            string value = reader.Value?.ToString();

            if (!string.IsNullOrWhiteSpace(value))
            {
                value = value.Replace("£", string.Empty).Replace(" ", string.Empty).Trim();

                if (decimal.TryParse(value, out decimal decimalValue))
                {
                    return decimalValue;
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
