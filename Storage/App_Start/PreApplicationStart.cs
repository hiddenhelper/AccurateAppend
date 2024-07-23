using System.Web;
using AccurateAppend.Websites.Storage;

[assembly: PreApplicationStartMethod(typeof(PreApplicationStart), nameof(PreApplicationStart.Start))]

namespace AccurateAppend.Websites.Storage
{
    /// <summary>
    /// Performs pre start initialization routines.
    /// </summary>
    public static class PreApplicationStart
    {
        /// <summary>
        /// Add pre application initialization routines here.
        /// </summary>
        public static void Start()
        {
            CorsHandlerAttribute.EnablePreflightChecks();
        }
    }
}