using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
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
    /// <see cref="BillFormatter"/> used to create subscription bills in HTML format.
    /// </summary>
    public class SubscriptionBillFormatterHtml : HtmlBillFormatter
    {
        #region Fields

        private readonly ISessionContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionBillFormatterHtml"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> providing data access.</param>
        public SubscriptionBillFormatterHtml(ISessionContext context)
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
            var subject = $"Subscription - OrderID: {bill.OrderId}";
            return Task.FromResult(subject);
        }

        /// <inheritdoc />
        protected override String CreateContentIntroBlock(Order order)
        {
            return SubscriptionTemplate.ContentIntroBlock;
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
        public override async Task<String> CreateFooter(BillModel bill)
        {
            var site = await this.GetSite(bill).ConfigureAwait(false);

            return DomainModel.EmailTemplates.AccurateAppend.CreateFooter(site);
        }
        
        #endregion
    }
}