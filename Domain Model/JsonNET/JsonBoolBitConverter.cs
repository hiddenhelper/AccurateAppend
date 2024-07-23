using System;
using Newtonsoft.Json;

namespace DomainModel.JsonNET
{
    public class JsonBoolBitConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof (Boolean) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    return false;
                case JsonToken.String:
                    string str = reader.Value as string;
                    switch(str.ToLower())
                    {
                        case "true":
                            return true;
                        case "false":
                            return true;
                        default:
                            throw new ArgumentException("Invalid token type: Input=" + str);
                    }
                case JsonToken.Boolean:
                    return reader.Value;
                default:
                    throw new ArgumentException("Invalid token type");
            }
            
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var result = true.Equals(value) ? true : false;
            writer.WriteValue(result);
        }
    }
}

