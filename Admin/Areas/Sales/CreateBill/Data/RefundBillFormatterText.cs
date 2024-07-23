using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using AccurateAppend.Accounting;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.ViewModels;
using AccurateAppend.Security;
using Order = AccurateAppend.Sales.Order;

namespace AccurateAppend.Websites.Admin.Areas.Sales.CreateBill.Data
{
    /// <summary>
    /// <see cref="BillFormatter"/> used to create refund bills in HTML format.
    /// </summary>
    public class RefundBillFormatterText : TextBillFormatter
    {
        #region Fields

        private readonly ISessionContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RefundBillFormatterText"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> providing data access.</param>
        public RefundBillFormatterText(ISessionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            this.context = context;
        }

        #endregion

        #region Overrides

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
                       SiteCache.Cache.First(s => s.ApplicationId == ApplicationExtensions.AccurateAppendId);

            return site;
        }

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
        protected override String CreateContentIntroBlock(Order order)
        {
            var intro = "Information regarding your refund is detailed below.\r\n";
            return intro;
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
        protected override String BuildOrderLines(Order order)
        {
            var orderdetails = new StringBuilder();
            
            orderdetails.AppendLine("DETAILS");
            orderdetails.AppendLine();

            orderdetails.AppendLine("Description".PadRight(80) + "Qty".PadLeft(10) + "Unit".PadLeft(10) + "Total".PadLeft(10));
            orderdetails.AppendLine("".PadRight(80, '-') + " ".PadRight(10, '-') + " ".PadRight(10, '-') + " ".PadRight(10, '-'));

            foreach (var oi in order.Lines.FilterNonBillableOperations().OrderBy(i => i.Id))
            {
                orderdetails.AppendLine(this.CreateProductLineRow(oi));
            }

            orderdetails.AppendLine("".PadRight(80, '-') + " ".PadRight(10, '-') + " ".PadRight(10, '-') + " ".PadRight(10, '-'));
            orderdetails.AppendLine(this.CreateOrderTotalRow(order));
            
            return orderdetails.ToString();
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

        #endregion
    }
}