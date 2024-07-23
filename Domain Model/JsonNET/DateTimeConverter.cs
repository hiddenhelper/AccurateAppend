using System;
using Newtonsoft.Json;

namespace DomainModel.JsonNET
{
    public class DateTimeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof (DateTime) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    return new DateTime(1900, 1, 1);
                case JsonToken.Date:
                    return DateTime.Parse(reader.Value.ToString());
                default:
                    throw new ArgumentException("Invalid token type");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var result = Convert.ToDateTime(value).ToString();
            writer.WriteValue(result);
        }
    }
}

