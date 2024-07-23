using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using AccurateAppend.Core;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.ViewModels;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.Areas.Accounting.Data;
using Contact = AccurateAppend.Accounting.Contact;

namespace AccurateAppend.Websites.Admin.Areas.Sales.CreateBill.Data
{
    /// <summary>
    /// <see cref="BillFormatter"/> used to create refund bills in HTML format.
    /// </summary>
    public class RefundBillFormatterHtml : HtmlBillFormatter
    {
        #region Fields

        private readonly ISessionContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RefundBillFormatterHtml"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> providing data access.</param>
        public RefundBillFormatterHtml(ISessionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            this.context = context;
        }

        #endregion

        #region Overrides
        
        /// <inheritdoc />
        public override async Task<IEnumerable<MailAddress>> CreateTo(BillModel bill)
        {
            var userId = bill.UserId;

            var userQuery = this.context
                .SetOf<User>()
                .Where(u => u.Id == userId)
                .Select(u => u.UserName);

            var contactQuery = this.context
                .SetOf<Contact>()
                .Where(c => c.Client.Logon.Id == userId)
                .Where(c => c.Billing)
                .Select(c => c.EmailAddress);

            var emails = await userQuery.Concat(contactQuery)
                .Distinct()
                .ToArrayAsync()
                .ConfigureAwait(false);

            return emails.Select(e => new MailAddress(e));
        }

        /// <inheritdoc />
        public override Task<String> CreateSubject(BillModel bill)
        {
            var subject = $"Refund Complete - OrderID: {bill.OrderId}";

            return Task.FromResult(subject);
        }

        /// <inheritdoc />
        protected override async Task<SiteCache.SiteInfo> GetSite(BillModel bill)
        {
            var applicationId = await this.context
                .SetOf<User>()
                .Where(u => u.Id == bill.UserId)
                .Select(u => u.Application.Id)
                .FirstAsync()
                .ConfigureAwait(false);

            var site = SiteCache.Cache.FirstOrDefault(s => s.ApplicationId == applicationId) ??
                       SiteCache.Cache.First(s => s.ApplicationId == WellKnownIdentifiers.AccurateAppendId);

            return site;
        }

        /// <inheritdoc />
        protected override String CreateContentIntroBlock(Order order)
        {
            return RefundTemplate.ContentIntroBlock;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Never returns any downloads.
        /// </remarks>
        protected sealed override String CreateDownloadLinks(CommonAttachments attachments)
        {
            return String.Empty;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Functionally this is basically the same base algorithm but removed minor header elements and only outputs items
        /// with a quantity greater than 0, unlike the base implementation. Instead of introducing complexity across the
        /// implementations, it's easier to override once here and introduce the needed changes. In addition, no support
        /// for credit/order minimums are added to the bill content.
        /// </remarks>
        protected override String BuildOrderLines(Order order)
        {
            if (!order.Lines.Any()) return String.Empty;

            var sb = new StringBuilder();

            sb.AppendLine(RefundTemplate.ContentLineItemsBlockStart);

            foreach (var oi in order.Lines.Where(i => i.Quantity > 0))
            {
                sb.AppendLine(this.CreateProductLineRow(oi));
            }

            sb.AppendLine(this.CreateOrderTotalRow(order));

            sb.AppendLine(RefundTemplate.ContentLineItemsBlockEnd);

            return sb.ToString();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        protected sealed override String CreateCreditRow(Order order)
        {
            throw new NotSupportedException("Refund templates do not support creating bill credit output");
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        protected sealed override String CreateOrderMinimumRow(Order order)
        {
            throw new NotSupportedException("Refund templates do not support creating bill minimum output");
        }

        /// <summary>
        /// Used to created the formatted product line item row on a bill.
        /// </summary>
        /// <param name="productLine">The <see cref="ProductLine"/> to create the line for.</param>
        /// <returns>The product line detail content.</returns>
        protected override String CreateProductLineRow(ProductLine productLine)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"
     <tr>
      <td height=""1"" bgcolor=""#dddddd"" colspan=""7"" style=""font-size: 1px; line-height: 1px;""></td>
     </tr>");

            sb.AppendLine(String.Format(RefundTemplate.ContentLineItem, productLine.Description, productLine.Quantity, productLine.Price.ToString("C4", CultureInfoHelper.SystemCulture), productLine.Total().ToString("C2", CultureInfoHelper.SystemCulture)));
            return sb.ToString();
        }
        
        /// <inheritdoc />
        protected override String CreateOrderTotalRow(Order order)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"
        <tr>
         <td height=""1"" bgcolor=""#dddddd"" colspan=""7"" style=""font-size: 1px; line-height: 1px;""></td>
        </tr>");

            sb.AppendLine(String.Format(RefundTemplate.ContentLineItem, "TOTAL", String.Empty, String.Empty, order.Total().RoundFractionalPennies().ToString("C2", CultureInfoHelper.SystemCulture)));
            return sb.ToString();
        }

        /// <inheritdoc />
        public override async Task<String> CreateFooter(BillModel bill)
        {
            var site = await this.GetSite(bill).ConfigureAwait(false);

            return DomainModel.EmailTemplates.AccurateAppend.CreateFooter(site);
        }

        #endregion
    }
}