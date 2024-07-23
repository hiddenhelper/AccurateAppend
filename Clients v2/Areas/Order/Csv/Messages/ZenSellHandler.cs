using System;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.ZenDesk.Configuration;
using AccurateAppend.ZenDesk.Sales.Contacts;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Csv.Messages
{
    /// <summary>
    /// Handler for the CSV Cart Events. Syncs to ZenSell.
    ///
    /// <see cref="CsvCartCreatedEvent"/> : Update ZenSell Contact to assign current AA sales rep
    /// <see cref="FileUploadedEvent"/> : Create new ZenSell Deal with List details
    /// <see cref="QuoteCreatedEvent"/> : Mark hot
    /// <see cref="CsvOrderPlacedEvent"/> : Order won
    /// <see cref="CsvOrderExpiredEvent"/> : Order lost
    /// </summary>
    public class ZenSellHandler : ZenSellCartBase,
        IHandleMessages<CsvCartCreatedEvent>,
        IHandleMessages<FileUploadedEvent>,
        IHandleMessages<QuoteCreatedEvent>,
        IHandleMessages<CsvOrderPlacedEvent>, 
        IHandleMessages<CsvOrderExpiredEvent>
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

        #region IHandleMessages<CsvCartCreatedEvent> Members

        /// <inheritdoc />
        public async Task Handle(CsvCartCreatedEvent message, IMessageHandlerContext context)
        {
            var contact = await this.contactsService.DetailAsync(message.UserId, CancellationToken.None).ConfigureAwait(false);
            if (contact != null)
            {
                contact.OwnerId = this.MapToZenUser(message.SalesRep);
                await this.contactsService.Upsert(contact).ConfigureAwait(false);
            }
        }

        #endregion

        #region IHandleMessages<FileUploadedEvent> Members

        /// <inheritdoc />
        public async Task Handle(FileUploadedEvent message, IMessageHandlerContext context)
        {
            var contact = await this.contactsService.DetailAsync(message.UserId, CancellationToken.None).ConfigureAwait(false);
            if (contact == null) throw new InvalidOperationException($"Can not find ZenSell Contact matching {message.UserId}"); // This means we're in a race condition at ZenSell. Fail and try again. The contact will shortly be created so OK.

            await this.ListSelected(message.CartId, contact.Id.Value, message.CustomerFileName, contact.OwnerId);
        }

        #endregion

        #region IHandleMessages<QuoteCreatedEvent>

        /// <inheritdoc />
        public Task Handle(QuoteCreatedEvent message, IMessageHandlerContext context)
        {
            return this.UpdateWithQuote(message.CartId, message.Quote);
        }

        #endregion

        #region IHandleMessages<CsvOrderPlacedEvent> Members

        /// <inheritdoc />
        public Task Handle(CsvOrderPlacedEvent message, IMessageHandlerContext context)
        {
            return this.Won(message.CartId, message.QuotedTotal);
        }

        #endregion

        #region IHandleMessages<CsvOrderExpiredEvent> Members

        /// <inheritdoc />
        public Task Handle(CsvOrderExpiredEvent message, IMessageHandlerContext context)
        {
            return this.Lost(message.CartId);
        }

        #endregion
    }
}