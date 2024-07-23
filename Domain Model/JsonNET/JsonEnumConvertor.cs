using System;
using Newtonsoft.Json;

namespace DomainModel.JsonNET
{
    public class JsonEnumConvertor : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsEnum;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return System.Enum.Parse(objectType, reader.Value.ToString());
        }
    }
}
