using System;
using System.Text;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.HtmlHelpers
{
    /// <summary>
    /// HTML Helpers for interacting with Google Tracking Analytics
    /// </summary>
    public static class GoogleAnalytics
    {
        /// <summary>
        /// Creates a google analytics script element for the application.
        /// </summary>
        public static MvcHtmlString GetAnalyticsScript(this HtmlHelper helper, Boolean isTestAccount)
        {
            var sb = new StringBuilder();

            // ReSharper disable StringLiteralTypo
            sb.AppendLine("<script>");
            sb.AppendLine("(function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){");
            sb.AppendLine("(i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),");
            sb.AppendLine("m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)");
            sb.AppendLine("})(window,document,'script','//www.google-analytics.com/analytics.js','ga');");
            sb.AppendLine("ga('create', '" + Properties.Settings.Default.GoogleAnalyticsCode + "', 'auto');");
            if (!isTestAccount) sb.AppendLine("ga('send', 'pageview');");
            sb.AppendLine("ga('send', 'pageview');");
            sb.AppendLine("ga('require', 'GTM-NKTTH2V');");
            sb.AppendLine("</script>");
            sb.AppendLine("<style>.async-hide { opacity: 0 !important} </style>");
            sb.AppendLine("<script>(function(a,s,y,n,c,h,i,d,e){s.className+=' '+y;h.start=1*new Date;");
            sb.AppendLine("h.end=i=function(){s.className=s.className.replace(RegExp(' ?'+y),'')};");
            sb.AppendLine("(a[n]=a[n]||[]).hide=h;setTimeout(function(){i();h.end=null},c);h.timeout=c;");
            sb.AppendLine("})(window,document.documentElement,'async-hide','dataLayer',4000,");
            sb.AppendLine("{'GTM-NKTTH2V':true});</script>");
            // ReSharper restore StringLiteralTypo

            return MvcHtmlString.Create(sb.ToString());
        }
    }
}