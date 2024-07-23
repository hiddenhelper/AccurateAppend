using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Data;
using AccurateAppend.Messaging;
using AccurateAppend.Operations.Contracts;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Websites.Clients.Areas.Order.Automation.Messages;
using AccurateAppend.Websites.Clients.Areas.Order.Csv.Messages;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Messages.Csv
{
    /// <summary>
    /// Responds to the <see cref="CsvOrderPlacedEvent"/> and <see cref="AutomationOrderPlacedEvent"/> event messages by crafting an email outlining
    /// details of the order and enqueuing it as a <see cref="SendEmailCommand"/>.
    /// </summary>
    public class SendEmailForOrderPlacedEventHandler : IHandleMessages<CsvOrderPlacedEvent>, IHandleMessages<AutomationOrderPlacedEvent>
    {
        #region Fields

        private readonly DefaultContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SendEmailForOrderPlacedEventHandler"/> class.
        /// </summary>
        /// <param name="dataContext">The <see cref="DefaultContext"/> providing data access to the handler.</param>
        public SendEmailForOrderPlacedEventHandler(DefaultContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
        }

        #endregion

        #region IHandleMessages<CsvOrderPlacedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(CsvOrderPlacedEvent message, IMessageHandlerContext context)
        {
            var cartId = message.CartId;
            
            using (context.Alias())
            {
                var command = await this.CreateEmail(cartId).ConfigureAwait(false);
                
                await context.Send(command).ConfigureAwait(false);
            }
        }

        #endregion

        #region IHandleMessages<AutomationOrderPlacedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(AutomationOrderPlacedEvent message, IMessageHandlerContext context)
        {
            var cartId = message.CartId;

            using (context.Alias())
            {
                var command = await this.CreateEmail(cartId).ConfigureAwait(false);

                await context.Send(command).ConfigureAwait(false);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Factory method to generate an email content for a quoted <see cref="CsvCart"/>.
        /// </summary>
        /// <param name="cartId">The identifier of the cart to generate an order submitted email for.</param>
        /// <returns>The <see cref="SendEmailCommand"/> to enqueue the email with.</returns>
        protected virtual async Task<SendEmailCommand> CreateEmail(Guid cartId)
        {
            var cart = await this.dataContext
                .SetOf<Cart>()
                .Where(c => !c.IsActive)
                .ForCsv(cartId)
                .Include(c => c.Client)
                .SingleAsync()
                .ConfigureAwait(false);

                var email = await EmailHtml.OrderSubmitted(cart).ConfigureAwait(false);

                var command = new SendEmailCommand();
                command.Bcc.AddRange(email.Bcc.Select(e => e.ToString()));
                command.IsHtmlContent = email.IsBodyHtml;
                command.MessageKey = Guid.NewGuid();
                command.SendFrom = email.From.ToString();
                command.Subject = email.Subject;
                command.To.AddRange(email.To.Select(e => e.ToString()));
                command.Track = true;
                command.Body = email.Body;

            return command;
        }

        #endregion
    }
}