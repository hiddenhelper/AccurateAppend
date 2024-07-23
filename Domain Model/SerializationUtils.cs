using System;
using System.IO;
using System.Web.UI;

namespace DomainModel
{
    public static class SerializationUtils
    {
        public static String Serialize(Object obj)
        {
            // Note: obj must be marked [Serializable] or implement ISerializable
            using (var writer = new StringWriter())
            {

                new LosFormatter().Serialize(writer, obj);
                return writer.ToString();
            }
        }

        public static T Deserialize<T>(String data) where T: class
        {
            if (data == null) return null;

            return (new LosFormatter()).Deserialize(data) as T;
        }
    }
}