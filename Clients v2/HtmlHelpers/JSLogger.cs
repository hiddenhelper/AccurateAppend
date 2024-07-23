using System.Text;
//using System.Web;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.HtmlHelpers
{
    /// <summary>
    /// HTML Helpers related to client side JS logging integration.
    /// </summary>
    // ReSharper disable InconsistentNaming
    public static class JSLogger
    // ReSharper restore InconsistentNaming
    {
        /// <summary>
        /// Crafts the appropriate logging script declarations and initialization that should be used on the current host.
        /// </summary>
        /// <returns>The <see cref="MvcHtmlString"/> containing the logging script content.</returns>
        public static MvcHtmlString CreateLoggingScript(this HtmlHelper helper)
        {
            var sb = new StringBuilder(3);
            sb.AppendLine("<!-- jslogger code -->");
            //sb.AppendLine("<script type=\"text/javascript\" src=\"//jslogger.com/jslogger.js\"></script>");
            //if (HttpContext.Current?.Request.Url.IsLoopback ?? false)
            //{
            //    sb.AppendLine("<script type=\"text/javascript\">window.jslogger = new JSLogger();</script>");
            //}
            //else
            //{
            //    sb.AppendLine("<script type=\"text/javascript\">window.jslogger = new JSLogger({ apiKey: \"550c32efcfce61b75300009e\" });</script>");
            //}

            return new MvcHtmlString(sb.ToString());
        }
    }
}