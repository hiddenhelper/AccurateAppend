using System;
using Newtonsoft.Json;

namespace DomainModel.JsonNET
{
    public class JsonShortDateConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(DateTime) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Date:
                    return reader.Value;
                default:
                    throw new ArgumentException("Invalid token type");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var v = (DateTime) value;
            writer.WriteValue(v.ToShortDateString());
        }
    }
}
