using System.Web.Mvc;
using Newtonsoft.Json;

namespace AccurateAppend.Websites.Admin.TempDataExtensions
{
    /// <summary>
    /// Extensions used to store complex types in TempData
    /// </summary>
    public static class TempDataExtensions
    {
        public static void Put<T>(this TempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this TempDataDictionary tempData, string key) where T : class
        {
            object o;
            tempData.TryGetValue(key, out o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }

        public static T Peek<T>(this TempDataDictionary tempData, string key) where T : class
        {
            object o = tempData.Peek(key);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }
    }
}