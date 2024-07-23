using System;
using System.Text;
using AccurateAppend.Core;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.ViewModels;
using AccurateAppend.Security;

namespace AccurateAppend.Websites.Admin.Areas.Sales.CreateBill.Data
{
    /// <summary>
    /// Contains the common logic and markup routines for text content. Allows TEXT formatters to only
    /// implement very specific members unique to their output.
    /// </summary>
    public abstract class TextBillFormatter : BillFormatter
    {
        #region Overrides

        /// <inheritdoc />
        /// <value>Always returns <see cref="BillFormatter.MarkupType.RawText"/> value.</value>
        public sealed override MarkupType Markup => MarkupType.RawText;

        /// <inheritdoc />
        protected override String CreateBodyStart()
        {
            return String.Empty;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Includes download hyperlinks for only "core" attachments. NationBuilder attachments are ignored.
        /// </remarks>
        protected override String CreateDownloadLinks(CommonAttachments attachments)
        {
            if (attachments == null || !attachments.ContainsAttachments()) return String.Empty;

            var sb = new StringBuilder(6);
            sb.AppendLine("File Downloads");

            const String Template = @"{1} : {0}";

            if (attachments.CommonProcessingCodes) sb.AppendLine(String.Format(Template, "https://clients.accurateappend.com/Public/Documentation/ProcessingCodes", "Processing & Match Codes"));

            return sb.ToString();
        }

        /// <inheritdoc />
        protected override String CreateProductLineRow(ProductLine productLine)
        {
            return (" " + productLine.Description).PadRight(80)
                   + $"{productLine.Quantity.ToString("0,0", CultureInfoHelper.SystemCulture)}".PadLeft(10)
                   + productLine.Price.ToString("C4", CultureInfoHelper.SystemCulture).PadLeft(10)
                   + productLine.Total().ToString("C", CultureInfoHelper.SystemCulture).PadLeft(10);
        }

        /// <inheritdoc />
        protected override String CreateCreditRow(Order order)
        {
            return " [CREDIT]".PadRight(80)
                   + String.Empty.PadLeft(10)
                   + String.Empty.PadLeft(10)
                   + order.OrderMinimum.ToString("C2", CultureInfoHelper.SystemCulture);
        }

        /// <inheritdoc />
        protected override String CreateOrderMinimumRow(Order order)
        {
            return " [ORDER MINIMUM]".PadRight(80)
                   + String.Empty.PadLeft(10)
                   + String.Empty.PadLeft(10)
                   + order.OrderMinimum.ToString("C2", CultureInfoHelper.SystemCulture);
        }

        /// <inheritdoc />
        protected override String CreateOrderTotalRow(Order order)
        {
            return "Total".PadRight(100) + order.Total().ToString("C", CultureInfoHelper.SystemCulture).PadLeft(10);
        }

        /// <inheritdoc />
        protected override String CreateBodyEnd(SiteCache.SiteInfo site)
        {
            var footer = new StringBuilder();

            var siteinfo = site;

            footer.AppendLine($"For any questions please contact your Account Executive or Customer Support at {siteinfo.PrimaryPhone}, or email {siteinfo.MailboxSupport}.");
            footer.AppendLine();
            footer.AppendLine(siteinfo.Title);
            footer.AppendLine(siteinfo.Address);
            footer.AppendLine($"{siteinfo.City}, {siteinfo.State} {siteinfo.Zip}");
            footer.AppendLine($"Toll free {siteinfo.PrimaryPhone}");
            footer.AppendLine(siteinfo.MailboxSupport);
            footer.AppendLine(siteinfo.Website);
            footer.AppendLine();
            footer.AppendLine();
            footer.AppendLine("TERMS AND CONDITIONS");
            footer.AppendLine();
            footer.AppendLine("1. CONFIDENTIALITY");
            footer.AppendLine($"{siteinfo.Title} and Customer mutually agree to keep all business dealings confidential, including but not limited to pricing, products, services, systems used by {siteinfo.Title} to perform or provide services to Customer. Customer further agrees to keep confidential {siteinfo.Title} as Customer's source for any services performed and provided to Customer. All Customer files, data, and other communication by Customer to {siteinfo.Title} will be kept strictly confidential by {siteinfo.Title}, except as necessary to perform or provide services ordered by Customer.");
            footer.AppendLine();
            footer.AppendLine("2. PRIVACY OF CUSTOMER DATA");
            footer.AppendLine($"{siteinfo.Title} does not cache any customer data and customer data is never made available to any third party or used for any purpose other than to complete the services ordered by Customer. All Customer files, both input and output are deleted 30 (thirty) days after files are completed, except Customer may request and {siteinfo.Title} shall set an automatic or manual deletion of all files after completion to comply with secure information or HIPPA information processing guidelines.");
            footer.AppendLine();
            footer.AppendLine("3. NO WARRANTY");
            footer.AppendLine($"{siteinfo.Title.ToUpperInvariant()} provides all data, content, information and services on an “as is” basis, with no warranties, including without limitation, merchantability and quality, fitness for a particular purpose, title and non-infringement. {siteinfo.Title} does not warrant that access to its data, content, information, and services will be uninterrupted or error-free, or that any errors will be corrected.");
            footer.AppendLine();
            footer.AppendLine("4. LIMITATION OF LIABILITY");
            footer.AppendLine($"In no event shall either party, its officers, directors, employees or agents be liable for any indirect, special, incidental, exemplary or consequential damages (including damages for loss of business, loss of profits, or the like), whether based on breach of contract, tort (including negligence), strict liability, breach of warranty or otherwise, even if the other party has been advised of the possibility of such damages and even if a remedy set forth herein is found to have failed of its essential purpose either party’s aggregate liability shall not exceed the total fee paid or payable to {siteinfo.Title}");
            footer.AppendLine();
            footer.AppendLine("5. INDEMNIFICATION");
            footer.AppendLine($"Customer shall indemnify and hold harmless {siteinfo.Title}, its officers, directors, employees and agents from any and all third party claims, liabilities, losses, damages, expenses or costs (including but not limited to defense costs and attorney’s fees) arising out of Customers use of any data, content, information or services provided to Customer by {siteinfo.Title}. {siteinfo.Title} shall indemnify and hold harmless Customer and its affiliates and suppliers against all claims, losses, damages, liabilities, costs and expenses, including reasonable attorneys’ fees, which Customer may incur as a result of claims relating to the infringement by {siteinfo.Title} of any third party patent, copyright, trademark or trade secret.");
            footer.AppendLine();
            footer.AppendLine("6. LAWFUL USE");
            footer.AppendLine($"Customer acknowledges their sole responsibility to comply with any and all laws or regulations, Federal, State or otherwise, for the lawful use of any data, content, information or services provided to Customer by {siteinfo.Title}, including but not limited to any Mobile telephone numbers provided to Customer.");
            footer.AppendLine();
            footer.AppendLine("7. DO NOT CALL/DO NOT MARKET");
            footer.AppendLine($"{siteinfo.Title} does not process its databases, appended data or other content provisioned or provided to Customer, including no processing of Customer data against the National Do Not Call Registry, State Phone Suppression files and DMA Phone Suppression files, herein collectively called “Telephone Number Suppression files” and any Do Not Market suppression files. Customer acknowledges their sole responsibility to comply with all Do Not Call and / or Do Not Market laws and regulations, including that Customer will either obtain access to the Do Not Call Registry and other Telephone Number Suppression files on its own behalf, or if Customer will not obtain access to Telephone Number Suppression files, Customer will only make calls for purposes permitted by law. Customer also acknowledges their sole responsibility to comply with the CAN SPAM ACT for all electronic messaging.");

            return footer.ToString();
        }

        #endregion
    }
}