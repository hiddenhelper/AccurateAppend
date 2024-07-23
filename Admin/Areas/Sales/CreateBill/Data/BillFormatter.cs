using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Services;
using AccurateAppend.Sales.Contracts.ViewModels;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.Areas.Accounting.Data;

namespace AccurateAppend.Websites.Admin.Areas.Sales.CreateBill.Data
{
    /// <summary>
    /// Used to simplify the maintenance of the new bill formatters by combining all the shared common
    /// formatting and flow logic we use with between them while still keeping each individual step a
    /// simple overrideable atomic bit of logic. By default, this type
    /// -Creates/Opens the content (HTML or Text)
    /// -Creates the outlining table
    /// -Creates an Intro block explaining the order
    /// -Creates a block of public site download links for any common attachments specified.
    /// -For each product line on the order, create the appropriate row
    /// --If the order has a credit, create the appropriate row
    /// --If the order has a minimum, create the appropriate row
    /// -Format a total for the order
    /// -Close the outlining table
    /// -Close the content
    ///
    /// Each of these steps can be overridden, while only the credit row logic must be implemented.
    /// </summary>
    public abstract class BillFormatter : IBillFormatter
    {
        #region IBillFormatter Members

        /// <inheritdoc />
        /// <remarks>
        /// Implemented whether the <see cref="Markup"/> value is equal to <see cref="MarkupType.Html"/>
        /// </remarks>
        public Boolean IsHtml => this.Markup == MarkupType.Html;

        /// <inheritdoc />
        public virtual async Task<MailAddress> SendFrom(BillModel bill)
        {
            var site = await this.GetSite(bill).ConfigureAwait(false);

            return new MailAddress(site.MailboxSupport);
        }

        /// <inheritdoc />
        public abstract Task<IEnumerable<MailAddress>> CreateTo(BillModel bill);

        /// <inheritdoc />
        /// <remarks>
        /// By default returns an empty set.
        /// </remarks>
        public virtual Task<IEnumerable<MailAddress>> CreateBcc(BillModel bill)
        {
            return Task.FromResult(Enumerable.Empty<MailAddress>());
        }

        /// <inheritdoc />
        /// <remarks>
        /// The default logic is to return the interpolated value "Order Complete - OrderID: {bill.OrderId}".
        /// </remarks>
        public virtual Task<String> CreateSubject(BillModel bill)
        {
            var subject = $"Order Complete - OrderID: {bill.OrderId}";
            return Task.FromResult(subject);
        }

        /// <inheritdoc />
        /// <remarks>
        /// By default returns <see cref="String.Empty"/>.
        /// </remarks>
        public virtual Task<String> CreateHeader(BillModel bill)
        {
            return Task.FromResult(String.Empty);
        }

        /// <inheritdoc />
        public virtual async Task<String> CreateBody(Order order, CommonAttachments attachments = null)
        {
            var sb = new StringBuilder();

            sb.AppendLine(this.CreateBodyStart());

            sb.AppendLine(this.CreateContentIntroBlock(order));

            sb.AppendLine(this.CreateDownloadLinks(attachments));

            sb.AppendLine(this.BuildOrderLines(order));

            var site = await this.GetSite(order).ConfigureAwait(false);
            sb.AppendLine(this.CreateBodyEnd(site));

            return sb.ToString();
        }

        /// <inheritdoc />
        /// <remarks>
        /// By default returns <see cref="String.Empty"/>.
        /// </remarks>
        public virtual Task<String> CreateFooter(BillModel bill)
        {
            return Task.FromResult(String.Empty);
        }

        #endregion

        #region Common Routines

        /// <summary>
        /// Used to locate the cached site information for the provided <paramref name="bill"/>.
        /// </summary>
        /// <remarks>
        /// As the viewmodel doesn't contain the application the order owner is in, no default implementation
        /// can be determined.
        /// </remarks>
        /// <param name="bill">The <see cref="BillModel"/> to lookup the appropriate site information for.</param>
        /// <returns>The cached site information that matches the bill.</returns>
        protected abstract Task<SiteCache.SiteInfo> GetSite(BillModel bill);

        /// <summary>
        /// Used to locate the cached site information for the provided <paramref name="order"/>.
        /// </summary>
        /// <remarks>
        /// The default logic is to simply traverse the order information to determine the <see cref="ClientRef.ApplicationId"/>
        /// value. This value is then matched to the <see cref="SiteCache"/> information. If not found, the
        /// <see cref="ApplicationExtensions.AccurateAppendId"/> value is defaulted to.
        /// </remarks>
        /// <param name="order">The <see cref="Order"/> to lookup the appropriate site information for.</param>
        /// <returns>The cached site information that matches the order.</returns>
        protected virtual Task<SiteCache.SiteInfo> GetSite(Order order)
        {
            var applicationId = order.Deal.Client.ApplicationId;
            var site = SiteCache.Cache.FirstOrDefault(s => s.ApplicationId == applicationId) ??
                       SiteCache.Cache.First(s => s.ApplicationId == ApplicationExtensions.AccurateAppendId);

            return Task.FromResult(site);
        }

        /// <summary>
        /// Created any content that needs to be in the start of the message such as html or text wrappers.
        /// </summary>
        protected abstract String CreateBodyStart();

        /// <summary>
        /// Creates the content introduction explaining the order.
        /// </summary>
        /// <param name="order">The <see cref="Order"/> to create the intro for.</param>
        /// <returns>The order introduction.</returns>
        protected abstract String CreateContentIntroBlock(Order order);

        /// <summary>
        /// Creates the content block providing download links to the common online files.
        /// </summary>
        /// <remarks>
        /// If the <paramref name="attachments"/> is null or all items are false, no content should be generated.
        /// </remarks>
        /// <param name="attachments">The <see cref="CommonAttachments"/> to create download links for.</param>
        /// <returns>The order introduction.</returns>
        protected abstract String CreateDownloadLinks(CommonAttachments attachments);

        /// <summary>
        /// Creates the order line item details.
        /// </summary>
        /// <param name="order">The <see cref="Order"/> to create line items for.</param>
        /// <returns>The order line detail content.</returns>
        protected virtual String BuildOrderLines(Order order)
        {
            if (!order.Lines.Any()) return String.Empty;

            var sb = new StringBuilder();

            sb.AppendLine(ReceiptTemplate.ContentLineItemsBlockStart);

            foreach (var oi in order.Lines)
            {
                sb.AppendLine(this.CreateProductLineRow(oi));
            }

            if (order.OrderMinimum < 0)
            {
                sb.AppendLine(this.CreateCreditRow(order));
            }
            else if (order.OrderMinimum > 0 && order.OrderMinimum > order.Lines.Total())
            {
                sb.AppendLine(this.CreateOrderMinimumRow(order));
            }

            sb.AppendLine(this.CreateOrderTotalRow(order));

            sb.AppendLine(ReceiptTemplate.ContentLineItemsBlockEnd);

            return sb.ToString();
        }

        /// <summary>
        /// Used to created the formatted product line item row on a bill.
        /// </summary>
        /// <param name="productLine">The <see cref="ProductLine"/> to create the line for.</param>
        /// <returns>The product line detail content.</returns>
        protected abstract String CreateProductLineRow(ProductLine productLine);

        /// <summary>
        /// Used to created the formatted credit row on a bill.
        /// </summary>
        /// <param name="order">The <see cref="Order"/> to create the credit line for.</param>
        /// <returns>The order credit line detail content.</returns>
        protected abstract String CreateCreditRow(Order order);

        /// <summary>
        /// Used to created the formatted order minimum row on a bill.
        /// </summary>
        /// <param name="order">The <see cref="Order"/> to create the order minimum line for.</param>
        /// <returns>The order minimum line detail content.</returns>
        protected abstract String CreateOrderMinimumRow(Order order);

        /// <summary>
        /// Used to created the formatted order total row on a bill.
        /// </summary>
        /// <param name="order">The <see cref="Order"/> to create the order total line for.</param>
        /// <returns>The order total line detail content.</returns>
        protected abstract String CreateOrderTotalRow(Order order);

        /// <summary>
        /// Created any content that needs to be in the end of the message such as html or text wrappers.
        /// </summary>
        protected abstract String CreateBodyEnd(SiteCache.SiteInfo site);

        /// <summary>
        /// Indicates the format output of the formatter.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="MarkupType.RawText"/>. Implementors can override this property to return another
        /// value. Changing this value may change the <see cref="IsHtml"/> property (see property for more information).
        /// </remarks>
        public virtual MarkupType Markup => MarkupType.RawText;

        /// <summary>
        /// Indicates the format type of the output.
        /// </summary>
        public enum MarkupType
        {
            /// <summary>
            /// Pure or plain text.
            /// </summary>
            RawText,

            /// <summary>
            /// HTML markup
            /// </summary>
            Html,

            /// <summary>
            /// PDF Markup Language (not supported yet)
            /// </summary>
            Pdfml
        }

        #endregion
    }
}