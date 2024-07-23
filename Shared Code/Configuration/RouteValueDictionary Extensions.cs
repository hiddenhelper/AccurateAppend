using System;
using System.Web.Routing;

namespace AccurateAppend.Websites.Configuration
{
    internal static class RouteValueDictionaryExtensions
    {
        internal static String Area(this RouteValueDictionary route)
        {
            var areaName = route.ContainsKey("area") ? (String)route["area"] : String.Empty;
            return areaName;
        }
    }
}