using System;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.ZenDesk.Configuration;
using AccurateAppend.ZenDesk.Sales.Contacts;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.Order.Messages
{
    /// <summary>
    /// Handler for the NB Cart Events. Syncs to ZenSell.
    ///
    /// <see cref="NationBuilderCartCreatedEvent"/> : Update ZenSell Contact to assign current AA sales rep
    /// <see cref="ListSelectedEvent"/> :  Create new ZenSell Deal with List details
    /// <see cref="QuoteCreatedEvent"/> : Mark hot
    /// <see cref="NationBuilderOrderPlacedEvent"/> : Order won
    /// <see cref="NationBuilderOrderExpiredEvent"/> : Order lost
    /// </summary>
    public class ZenSellHandler : ZenSellCartBase, 
        IHandleMessages<NationBuilderCartCreatedEvent>,
        IHandleMessages<ListSelectedEvent>, 
        IHandleMessages<QuoteCreatedEvent>, 
        IHandleMessages<NationBuilderOrderPlacedEvent>, 
        IHandleMessages<NationBuilderOrderExpiredEvent>
    {
        #region Fields

        private readonly IContactsService contactsService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ZenSellHandler"/> class.
        /// </summary>
        /// <param name="sales">The <see cref="SalesContext"/> providing access to ZenSell data.</param>
        public ZenSellHandler(SalesContext sales) : base(sales)
        {
            this.contactsService = sales.Contacts;
        }

        #endregion

        #region IHandleMessages<NationBuilderCartCreatedEvent> Members

        /// <inheritdoc />
        public async Task Handle(NationBuilderCartCreatedEvent message, IMessageHandlerContext context)
        {
            var contact = await this.contactsService.DetailAsync(message.UserId, CancellationToken.None).ConfigureAwait(false);
            if (contact != null)
            {
                contact.OwnerId = this.MapToZenUser(message.SalesRep);
                await this.contactsService.Upsert(contact).ConfigureAwait(false);
            }
        }

        #endregion

        #region IHandleMessages<ListSelectedEvent> Members

        /// <inheritdoc />
        public async Task Handle(ListSelectedEvent message, IMessageHandlerContext context)
        {
            var contact = await this.contactsService.DetailAsync(message.UserId, CancellationToken.None).ConfigureAwait(false);
            if (contact == null) throw new InvalidOperationException($"Can not find ZenSell Contact matching {message.UserId}"); // This means we're in a race condition at ZenSell. Fail and try again. The contact will shortly be created so OK

            await this.ListSelected(message.CartId, contact.Id.Value, message.ListName, contact.OwnerId).ConfigureAwait(false);
        }

        #endregion

        #region IHandleMessages<QuoteCreatedEvent>

        /// <inheritdoc />
        public Task Handle(QuoteCreatedEvent message, IMessageHandlerContext context)
        {
            return this.UpdateWithQuote(message.CartId, message.Quote);
        }

        #endregion

        #region IHandleMessages<NationBuilderOrderPlacedEvent> Members

        /// <inheritdoc />
        public Task Handle(NationBuilderOrderPlacedEvent message, IMessageHandlerContext context)
        {
            return this.Won(message.CartId, message.Total());
        }

        #endregion

        #region IHandleMessages<NationBuilderOrderExpiredEvent> Members

        /// <inheritdoc />
        public Task Handle(NationBuilderOrderExpiredEvent message, IMessageHandlerContext context)
        {
            return this.Lost(message.CartId);
        }

        #endregion
    }
}