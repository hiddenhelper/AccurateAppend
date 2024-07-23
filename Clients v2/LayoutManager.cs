using System;
using System.Web;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Provides a single location where the host to layout logic can reside.
    /// Since as a technical requirement, the _Layout.cshtml files need to be found in
    /// the path for each view location, we can use this do deduplicate any code.
    /// </summary>
    public static class LayoutManager
    {
        public static String DeterminePath()
        {
            var Layout = String.Empty;

            switch (HttpContext.Current.Request.Url.Host.ToLowerInvariant())
            {
                case "dev.clients.accurateappend.com":
                case "devclients.accurateappend.com":
                case "clients.accurateappend.com":
                    Layout = "~/Views/Shared/AccurateAppend_v7/_Layout.cshtml";
                    break;
                case "localhost":
                    Layout = "~/Views/Shared/AccurateAppend_v7/_Layout.cshtml";
                    break;
                case "devclients.2020connect.net":
                case "dev.clients.2020connect.net":
                case "clients.2020connect.net":
                    Layout = "~/Views/Shared/2020Connect/_Layout.cshtml";
                    break;
            }

            return Layout;
        }
    }
}