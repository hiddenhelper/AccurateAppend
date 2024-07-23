using System;
using System.Text;
using System.Threading.Tasks;
using AccurateAppend.Core;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.ViewModels;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.Areas.Accounting.Data;

namespace AccurateAppend.Websites.Admin.Areas.Sales.CreateBill.Data
{
    /// <summary>
    /// Contains the common logic and markup routines for HTML content. Allows HTML formatters to only
    /// implement very specific members unique to their output.
    /// </summary>
    public abstract class HtmlBillFormatter : BillFormatter
    {
        /// <inheritdoc />
        /// <value>Always returns <see cref="BillFormatter.MarkupType.Html"/> value.</value>
        public sealed override MarkupType Markup => MarkupType.Html;

        /// <inheritdoc />
        public override Task<String> CreateHeader(BillModel bill)
        {
            return Task.FromResult(DomainModel.EmailTemplates.AccurateAppend.CreateHeader());
        }

        /// <inheritdoc />
        protected override String CreateBodyStart()
        {
            return DomainModel.EmailTemplates.AccurateAppend.CreateBodyStart();
        }

        /// <inheritdoc />
        /// <remarks>
        /// Includes download hyperlinks for only "core" attachments. NationBuilder attachments are ignored.
        /// </remarks>
        protected override String CreateDownloadLinks(CommonAttachments attachments)
        {
            if (attachments == null || !attachments.ContainsAttachments()) return String.Empty;

            var sb = new StringBuilder(6);
            sb.AppendLine(ReceiptTemplate.ContentAttachmentsBlockStart);

            const String Template = @"
<tr>
  <td class=""fs-18"" style=""font:bold 16px/24px Arial, Helvetica, sans-serif;color:#000;""><a href=""{0}"">{1}</a></td>
</tr>";

            if (attachments.CommonProcessingCodes) sb.AppendLine(String.Format(Template, "https://clients.accurateappend.com/Public/Documentation/ProcessingCodes", "Processing & Match Codes"));

            sb.AppendLine(ReceiptTemplate.ContentAttachmentsBlockEnd);

            return sb.ToString();
        }

        /// <summary>
        /// Used to created the formatted product line item row on a bill.
        /// </summary>
        /// <param name="productLine">The <see cref="ProductLine"/> to create the line for.</param>
        /// <returns>The product line detail content.</returns>
        protected override String CreateProductLineRow(ProductLine productLine)
        {
            var sb = new StringBuilder(2);
            sb.AppendLine(@"
     <tr>
      <td height=""1"" bgcolor=""#dddddd"" colspan=""7"" style=""font-size: 1px; line-height: 1px;""></td>
     </tr>");

            sb.AppendLine(String.Format(ReceiptTemplate.ContentLineItem, productLine.Description, productLine.Quantity, productLine.Price.ToString("C4", CultureInfoHelper.SystemCulture), productLine.Total().ToString("C2", CultureInfoHelper.SystemCulture)));
            return sb.ToString();
        }

        /// <summary>
        /// Used to created the formatted credit row on a bill.
        /// </summary>
        /// <param name="order">The <see cref="Order"/> to create the credit line for.</param>
        /// <returns>The order credit line detail content.</returns>
        protected override String CreateCreditRow(Order order)
        {
            var sb = new StringBuilder(2);
            sb.AppendLine(@"
     <tr>
      <td height=""1"" bgcolor=""#dddddd"" colspan=""7"" style=""font-size: 1px; line-height: 1px;""></td>
     </tr>");

            sb.AppendLine(String.Format(ReceiptTemplate.ContentLineItem, "[CREDIT]", String.Empty, String.Empty, order.OrderMinimum.RoundFractionalPennies().ToString("C2", CultureInfoHelper.SystemCulture)));
            return sb.ToString();
        }

        /// <summary>
        /// Used to created the formatted order minimum row on a bill.
        /// </summary>
        /// <param name="order">The <see cref="Order"/> to create the order minimum line for.</param>
        /// <returns>The order minimum line detail content.</returns>
        protected override String CreateOrderMinimumRow(Order order)
        {
            var sb = new StringBuilder(2);
            sb.AppendLine(@"
     <tr>
      <td height=""1"" bgcolor=""#dddddd"" colspan=""7"" style=""font-size: 1px; line-height: 1px;""></td>
     </tr>");

            sb.AppendLine(String.Format(ReceiptTemplate.ContentLineItem, "[ORDER MINIMUM]", String.Empty, String.Empty, order.OrderMinimum.RoundFractionalPennies().ToString("C2", CultureInfoHelper.SystemCulture)));
            return sb.ToString();
        }

        /// <summary>
        /// Used to created the formatted order total row on a bill.
        /// </summary>
        /// <param name="order">The <see cref="Order"/> to create the order total line for.</param>
        /// <returns>The order total line detail content.</returns>
        protected override String CreateOrderTotalRow(Order order)
        {
            var sb = new StringBuilder(2);
            sb.AppendLine(@"
        <tr>
         <td height=""1"" bgcolor=""#dddddd"" colspan=""7"" style=""font-size: 1px; line-height: 1px;""></td>
        </tr>");
            
            sb.AppendLine(String.Format(ReceiptTemplate.ContentLineItem, "TOTAL", String.Empty, String.Empty, order.Total().RoundFractionalPennies().ToString("C2", CultureInfoHelper.SystemCulture)));
            return sb.ToString();
        }

        /// <inheritdoc />
        protected override String CreateBodyEnd(SiteCache.SiteInfo site)
        {
            return DomainModel.EmailTemplates.AccurateAppend.CreateBodyEnd(site);
        }

        /// <inheritdoc />
        public override async Task<String> CreateFooter(BillModel bill)
        {
            var site = await this.GetSite(bill).ConfigureAwait(false);

            return DomainModel.EmailTemplates.AccurateAppend.CreateFooter(site);
        }
    }
}