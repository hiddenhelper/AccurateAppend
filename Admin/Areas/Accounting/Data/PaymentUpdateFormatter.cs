using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using AccurateAppend.Core;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Sales;
using AccurateAppend.Security;

namespace AccurateAppend.Websites.Admin.Areas.Accounting.Data
{
    /// <summary>
    /// Simple email content formatter for declined and expired payment methods.
    /// </summary>
    /// <remarks>
    /// While this type isn't part of the "usual" formatters for bill creation,
    /// it is easier to maintain by keeping everything in one logical namespace.
    /// </remarks>
    public class PaymentUpdateFormatter
    {
        #region Fields

        private readonly SiteCache.SiteInfo site;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentUpdateFormatter"/> class.
        /// </summary>
        /// <param name="site">The <see cref="SiteCache.SiteInfo"/> used to configure the email.</param>
        public PaymentUpdateFormatter(SiteCache.SiteInfo site)
        {
            if (site == null) throw new ArgumentNullException(nameof(site));

            this.site = site;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Crafts an email message for a declined <see cref="TransactionEvent"/>.
        /// </summary>
        /// <param name="declinedTransaction">The <see cref="TransactionEvent"/> describing the declined charge.</param>
        /// <param name="sendTo">The set of emails to send the declined payment notice to.</param>
        /// <returns>A personalized <see cref="MailMessage"/>.</returns>
        public virtual MailMessage Create(TransactionEvent declinedTransaction, IEnumerable<MailAddress> sendTo)
        {
            if (declinedTransaction == null) throw new ArgumentNullException(nameof(declinedTransaction));
            if (declinedTransaction.Status == TransactionResult.Refunded) throw new ArgumentOutOfRangeException(nameof(declinedTransaction), declinedTransaction, $"Declined charges cannot be type {TransactionResult.Refunded}");

            var order = declinedTransaction.Order;
            var dealTitle = order.Deal.Title;
            //var userId = order.Deal.Client.UserId;

            var sb = new StringBuilder();

            sb.AppendLine(DomainModel.EmailTemplates.AccurateAppend.CreateHeader());
            sb.AppendLine(DomainModel.EmailTemplates.AccurateAppend.CreateBodyStart());
            sb.AppendLine(String.Format(PaymentUpdateTemplate.ContentIntroBlockDeal, declinedTransaction.AmountCharged.ToString("c", CultureInfoHelper.SystemCulture), dealTitle, declinedTransaction.DisplayValue));
            sb.AppendLine(DomainModel.EmailTemplates.AccurateAppend.CreateBodyEnd(this.site));
            sb.AppendLine(DomainModel.EmailTemplates.AccurateAppend.CreateFooter(this.site));

            var email = new MailMessage();
            email.Subject = "Declined card";
            email.Body = sb.ToString();
            email.From = new MailAddress(this.site.MailboxSupport);
            email.IsBodyHtml = true;
            email.To.AddRange(sendTo);

            return email;
        }

        /// <summary>
        /// Crafts an email message for a <see cref="ClientRef"/>.
        /// </summary>
        /// <param name="user">The <see cref="ClientRef"/> to send payment update notifications to.</param>
        /// <param name="sendTo">The set of emails to send the payment notice to.</param>
        /// <returns>A personalized <see cref="MailMessage"/>.</returns>
        public virtual MailMessage Create(ClientRef user, IEnumerable<MailAddress> sendTo)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var sb = new StringBuilder();

            sb.AppendLine(DomainModel.EmailTemplates.AccurateAppend.CreateHeader());
            sb.AppendLine(DomainModel.EmailTemplates.AccurateAppend.CreateBodyStart());
            sb.AppendLine(String.Format(PaymentUpdateTemplate.ContentIntroBlockUser, user.UserId));
            sb.AppendLine(DomainModel.EmailTemplates.AccurateAppend.CreateBodyEnd(this.site));
            sb.AppendLine(DomainModel.EmailTemplates.AccurateAppend.CreateFooter(this.site));

            var email = new MailMessage();
            email.Subject = "Expired payment";
            email.Body = sb.ToString();
            email.From = new MailAddress(this.site.MailboxSupport);
            email.IsBodyHtml = true;
            email.To.AddRange(sendTo);

            return email;
        }

        #endregion
    }
}