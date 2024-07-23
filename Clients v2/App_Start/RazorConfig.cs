using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// MVC razor engine bootstrap routines.
    /// </summary>
    /// <remarks>
    /// The current logic is that this utility simply confirms that the Accurate Append
    /// custom path format is included in the search paths, if not already.
    /// </remarks>
    public class RazorConfig
    {
        /// <summary>
        /// Performs registration of view engine discovery paths.
        /// </summary>
        /// <param name="engine">The <see cref="RazorViewEngine"/> to configure view paths for.</param>
        public static void RegisterPaths(RazorViewEngine engine)
        {
            var locations = new HashSet<String>(engine.AreaPartialViewLocationFormats, StringComparer.OrdinalIgnoreCase);
            locations.Add("~/Areas/{2}/{1}/Views/{0}.cshtml");
            engine.AreaPartialViewLocationFormats = locations.ToArray();

            locations = new HashSet<String>(engine.AreaViewLocationFormats, StringComparer.OrdinalIgnoreCase);
            locations.Add("~/Areas/{2}/{1}/Views/{0}.cshtml");
            engine.AreaViewLocationFormats = locations.ToArray();
        }
    }
}