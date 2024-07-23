using System;
using AccurateAppend.Security;

namespace DomainModel.EmailTemplates
{
    public partial class AccurateAppend
    {
        /// <summary>
        /// Step 1.
        /// </summary>
        public static String CreateHeader()
        {
            var html = HtmlDeclaration
                       + Head
                       + BodyHeader;

            return $"{html}\r\n";
        }

        /// <summary>
        /// Step 2.
        /// </summary>
        public static String CreateBodyStart()
        {
            var html = ContentStart;

            return $"\r\n{html}";
        }

        /// <summary>
        /// Step 3.
        /// </summary>
        public static String CreateBodyEnd(SiteCache.SiteInfo siteinfo)
        {
            var html = String.Format(ContentCustomerCareBlock, siteinfo.PrimaryPhone, siteinfo.MailboxSupport, siteinfo.Title, siteinfo.Address, siteinfo.City, siteinfo.State, siteinfo.Zip, siteinfo.Website)
                       + String.Format(ContentTac, siteinfo.Title)
                       + ContentEnd;

            return $"\r\n{html}";
        }
        
        /// <summary>
        /// Step 4.
        /// </summary>
        public static String CreateFooter(SiteCache.SiteInfo siteinfo)
        {
            return $"\r\n{String.Format(BodyFooter, DateTime.Now.Year, siteinfo.Title, siteinfo.Address, siteinfo.City, siteinfo.State, siteinfo.Zip)}{HtmlEnd}";
        }
    }
}
