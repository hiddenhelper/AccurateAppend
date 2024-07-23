using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Data;
using AccurateAppend.Operations.Contracts;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Messages;
using AccurateAppend.Sales.DataAccess;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Messages.Sales
{
    /// <summary>
    /// Handler for the <see cref="DealCompletedEvent"/>and <see cref="DealRefundedEvent"/> bus messages.
    /// </summary>
    /// <remarks>
    /// Responds to a message by generating an operations tracked email command
    /// based on the deal's matching order bill content.
    /// </remarks>
    public class SendBillEmailForDealEventsHandler : IHandleMessages<DealCompletedEvent>, IHandleMessages<DealRefundedEvent>
    {
        #region Fields

        private readonly DefaultContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SendBillEmailForDealEventsHandler"/> class.
        /// </summary>
        /// <param name="dataContext">The <see cref="DefaultContext"/> component that provides data access.</param>
        public SendBillEmailForDealEventsHandler(DefaultContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));

            this.dataContext = dataContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Shared lookup routine for finding the <see cref="BillContent"/> for a
        /// specific <see cref="DealBinder"/> for the matching public key.
        /// </summary>
        /// <param name="publicKey">The public key for a single <see cref="AccurateAppend.Sales.Order"/> on the <see cref="DealBinder"/>.</param>
        /// <param name="dealId"></param>
        /// <returns>The matching <see cref="BillContent"/> instance.</returns>
        protected virtual Task<BillContent> LocateBill(Guid publicKey, Int32 dealId)
        {
            var messageKey = publicKey.ToString();

            return this.dataContext
                .SetOf<BillContent>()
                .Where(b => b.Order.PublicKey == messageKey)
                .Where(b => b.Order.Deal.Id == dealId)
                .SingleAsync();
        }

        /// <summary>
        /// Given a <see cref="BillContent"/> instance, create a <see cref="SendEmailCommand"/> correlated
        /// with the supplied <paramref name="publicKey"/>.
        /// </summary>
        /// <param name="publicKey">The public key for a single <see cref="AccurateAppend.Sales.Order"/> on the <see cref="DealBinder"/>.</param>
        /// <param name="bill">The <see cref="BillContent"/> to craft a tracked <see cref="SendEmailCommand"/> from.</param>
        /// <returns>The populated <see cref="SendEmailCommand"/>.</returns>
        protected virtual SendEmailCommand CreateEmail(Guid publicKey, BillContent bill)
        {
            var command = new SendEmailCommand()
            {
                Body = bill.Body,
                IsHtmlContent = bill.IsHtml,
                MessageKey = publicKey,
                SendFrom = bill.SendFrom,
                Subject = bill.Subject,
                Track = true
            };
            command.To.AddRange(bill.SendTo.Select(a => a.Address));
            command.Bcc.AddRange(bill.BccTo.Select(a => a.Address));

            command.FileAttachments = new XElement("files");
            foreach (var attachment in bill.Attachments.Select(a => a.ToXml()))
            {
                command.FileAttachments.Add(attachment);
            }

            return command;
        }

        #endregion

        #region IHandleMessages<DealCompletedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(DealCompletedEvent message, IMessageHandlerContext context)
        {
            var dealId = message.DealId;
            var messageKey = message.PublicKey;

            var bill = await this.LocateBill(messageKey, dealId).ConfigureAwait(false);

            var command = this.CreateEmail(messageKey, bill);

            await context.Send(command);
        }

        #endregion

        #region IHandleMessages<DealRefundedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(DealRefundedEvent message, IMessageHandlerContext context)
        {
            var dealId = message.DealId;
            var messageKey = message.PublicKey;

            var bill = await this.LocateBill(messageKey, dealId).ConfigureAwait(false);

            var command = this.CreateEmail(messageKey, bill);

            await context.Send(command);
        }

        #endregion
    }
}