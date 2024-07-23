using System;
using Newtonsoft.Json;

namespace DomainModel.JsonNET
{
    public class JsonGuidConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Guid) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    return Guid.Empty;
                case JsonToken.String:
                    string str = reader.Value as string;
                    if (string.IsNullOrEmpty(str))
                    {
                        return Guid.Empty;
                    }
                    return new Guid(reader.Value as string);
                default:
                    throw new ArgumentException("Invalid token type");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (Guid.Empty.Equals(value))
            {
                writer.WriteValue("");
            }
            else
            {
                writer.WriteValue((Guid)value);
            }
        }
    }
}
