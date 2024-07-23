using System.Text;
using System.Web;
using System.Web.Mvc;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Security;

namespace AccurateAppend.Websites.Clients.HtmlHelpers
{
    /// <summary>
    /// HTML Helpers related to client side ZenDesk integration.
    /// </summary>
    public static class ZenDeskHtmlHelper
    {
        #region Fields

        private static readonly ZenDeskHelper Helper = new ZenDeskHelper();

        #endregion

        #region Entry Point

        /// <summary>
        /// Provides access to the ZenDesk HTML helpers.
        /// </summary>
        /// <remarks>
        /// Works like a logical namespace for ZenDesk HTML helpers to reduce the API surface exposed by all
        /// <see cref="HtmlHelper"/> references. By reducing the intellisense complexity, we reduce the cognitive
        /// complexity budget for the developers.
        /// </remarks>
        /// <param name="html">The <see cref="HtmlHelper"/> for the current view.</param>
        /// <returns>A <see cref="ZenDeskHelper"/> for use by the current view.</returns>
        public static ZenDeskHelper ZenDesk(this HtmlHelper html)
        {
            return Helper;
        }

        #endregion

        #region Html Helper Logic

        /// <summary>
        /// HTML Helper class that collates all ZenDesk script logic.
        /// </summary>
        public sealed class ZenDeskHelper
        {
            /// <summary>
            /// Returns an HTML string based on the specified parameters for ZenDesk integration.
            /// </summary>
            /// <param name="httpContext">The current HTTP context for the request.</param>
            /// <returns>An <see cref="IHtmlString"/> containing the generated view content.</returns>
            public IHtmlString GetWidgetScript(HttpContextBase httpContext)
            {
                var identity = httpContext.User.Identity;
                //if (!identity.IsAuthenticated) return MvcHtmlString.Empty;
                if (identity.GetIdentifier() == UserExtensions.OperationsUserId) return MvcHtmlString.Empty;

                var sb = new StringBuilder();
                sb.AppendLine("<!-- Start of accurateappend Zendesk Widget script -->");
                sb.AppendLine("<script id=\"ze-snippet\" src =\"https://static.zdassets.com/ekr/snippet.js?key=4f945b7b-8a73-44f9-82c9-d35e65504b8f\"></script>");
                sb.AppendLine($@"
<script>
  zE(function() {{
    $zopim(function () {{
      $zopim.livechat.setName('{identity.Name}');
      $zopim.livechat.setEmail('{identity.Name}');
    }});
  }});
 </script>");
                sb.AppendLine("<!-- End of accurateappend Zendesk Widget script -->");

                return new MvcHtmlString(sb.ToString());
            }
        }

        #endregion
    }
}