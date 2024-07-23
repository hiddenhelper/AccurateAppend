using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using AccurateAppend.Security;

namespace AccurateAppend.Websites.Clients.HtmlHelpers
{
    /// <summary>
    /// Component able to output a VisiStat pixel tracking script
    /// </summary>
    public class VisiStatPixel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="VisiStatPixel"/> class.
        /// </summary>
        public VisiStatPixel()
        {
            this.ConvDesc = String.Empty;
            this.ConvSubTotal = String.Empty;
            this.ConvTax = String.Empty;
            this.ConvTotal = String.Empty;
            this.ConvMisc1 = String.Empty;
            this.ConvMisc2 = String.Empty;
            this.ConvMisc3 = String.Empty;
            this.ConvMisc4 = String.Empty;
            this.ConvMisc5 = String.Empty;
        }

        #endregion

        #region Properties

        public String Did { get; set; }

        public String MyPageName { get; set; }

        public String ConvName { get; set; }

        public String ConvDesc { get; set; }

        public String ConvSubTotal { get; set; }

        public String ConvTax { get; set; }

        public String ConvTotal { get; set; }

        public String ConvMisc1 { get; set; }

        public String ConvMisc2 { get; set; }

        public String ConvMisc3 { get; set; }

        public String ConvMisc4 { get; set; }

        public String ConvMisc5 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String MyId { get; set; }

        public String RequestIp { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Create the visistat tracking pixel generation script for the configured properties.
        /// </summary>
        /// <param name="identity">The <see cref="IIdentity"/> of the current user that represents the remote caller.</param>
        /// <returns>An <see cref="IHtmlString"/> containing the generated view content.</returns>
        public IHtmlString Sniffer(IIdentity identity)
        {
            if (Debugger.IsAttached) return MvcHtmlString.Empty;

            if (identity?.IsTestAccount() == false)
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("<script type=\"text/javascript\">");
                sb.AppendLine("    var DID = " + this.Did + ";");
                if (!String.IsNullOrEmpty(this.MyId)) sb.AppendLine("    var MyID = '" + this.MyId + "';");
                sb.AppendLine("    var MyPageName = '" + this.MyPageName + "';");
                sb.AppendLine("    var pcheck=(window.location.protocol == \"https:\") ? \"https://sniff.visistat.com/live.js\":\"http://stats.visistat.com/live.js\";");
                sb.AppendLine("    document.writeln('<scr'+'ipt src=\"'+pcheck+'\" type=\"text\\/javascript\"><\\/scr'+'ipt>');");
                sb.AppendLine("</script>");

                return new MvcHtmlString(sb.ToString());
            }

            return MvcHtmlString.Empty;
        }

        #endregion
    }
}