using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.Messaging;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.Order.Messages
{
    /// <summary>
    /// Saga designed to process the NationBuilder list order sales process.
    /// </summary>
    /// <remarks>
    /// Responds to the request by
    /// 1. Determining if cart exists, if true: perform no work
    /// 2. Creating new cart if no existing cart exists
    /// 3. Raise the CsvCartCreatedEvent
    /// 4. Set the product selections
    /// 5. Confirm the order
    /// 6. Raise the NationBuilderOrderPlacedEvent
    /// </remarks>
    public class NationBuilderSalesSaga : Saga<NationBuilderCartData>,
        IAmStartedByMessages<CreateNationBuilderCartCommand>,
        IAmStartedByMessages<SelectListForCartCommand>,
        IHandleMessages<EnterQuoteForNationBuilderOrderCommand>,
        IHandleMessages<SubmitNationBuilderOrderCommand>,
        IHandleTimeouts<CartExpiredTimeout>
    {
        #region Fields

        private readonly Sales.DataAccess.DefaultContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NationBuilderSalesSaga"/> class.
        /// </summary>
        /// <param name="dataContext">The <see cref="Sales.DataAccess.DefaultContext"/> providing data access to the handler.</param>
        public NationBuilderSalesSaga(Sales.DataAccess.DefaultContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
        }

        #endregion

        #region IHandleMessages<CreateCsvCartCommand> Members

        /// <inheritdoc />
        public virtual async Task Handle(CreateNationBuilderCartCommand message, IMessageHandlerContext context)
        {
            var userId = message.UserId;
            var cartId = message.CartId;

            this.Data.UserId = userId;
            this.Data.CartId = cartId;

            using (context.Alias())
            {
                var client = await this.dataContext
                    .SetOf<ClientRef>()
                    .FirstAsync(c => c.UserId == userId)
                    .ConfigureAwait(false);

                // check for replay
                var cart = await this.dataContext.SetOf<Cart>().FirstOrDefaultAsync(c => c.Id == cartId).ConfigureAwait(false);
                if (cart != null)
                {
                    if (!cart.IsActive) this.MarkAsComplete(); // kill with fire

                    return;
                }

                // New request so normal case
                cart = Cart.ForNationBuilder(client, cartId);
                this.dataContext.SetOf<Cart>().Add(cart);

                await this.dataContext.SaveChangesAsync().ConfigureAwait(false);

                // 24 hours to finish up
                await this.RequestTimeout<CartExpiredTimeout>(context, DateTime.UtcNow.AddHours(8)).ConfigureAwait(false);

                var @event = new NationBuilderCartCreatedEvent
                {
                    CartId = cartId,
                    CreatedBy = context.InitiatingUserId(),
                    UserId = userId,
                    SalesRep = client.OwnerId
                };

                await context.Publish(@event).ConfigureAwait(false);
            }
        }

        #endregion

        #region IHandleMessages<SelectListForCartCommand> Members

        /// <inheritdoc />
        public async Task Handle(SelectListForCartCommand message, IMessageHandlerContext context)
        {
            var cartId = message.CartId;
            var registrationId = message.RegistrationId;
            var listId = message.ListId;
            var listName = message.ListName;
            var recordCount = message.RecordCount;

            using (context.Alias())
            {
                var cart = await this.dataContext
                    .SetOf<Cart>()
                    .ForNationBuilder(cartId)
                    .Include(c => c.Client)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (cart == null)
                {
                    await context.HandleCurrentMessageLater().ConfigureAwait(false);
                    return;
                }

                if (!cart.IsActive)
                {
                    if (!cart.IsActive) this.MarkAsComplete(); // kill with fire
                    return;
                }

                cart.UpdateFileInformation(registrationId, listId, listName, recordCount);

                await this.dataContext.SaveChangesAsync().ConfigureAwait(false);

                var @event = new ListSelectedEvent
                {
                    UserId = cart.Client.UserId,
                    CartId = cartId,
                    ListName = listName,
                    RecordCount = recordCount
                };

                await context.Publish(@event).ConfigureAwait(false);
            }
        }

        #endregion

        #region IHandleMessages<EnterQuoteForCsvOrderCommand> Members

        /// <inheritdoc />
        public async Task Handle(EnterQuoteForNationBuilderOrderCommand message, IMessageHandlerContext context)
        {
            using (context.Alias())
            {
                var cartId = message.CartId;
                var cart = await this.dataContext
                    .SetOf<Cart>()
                    .Where(c => c.Id == cartId)
                    .Where(c => c.IsActive)
                    .SingleAsync()
                    .ConfigureAwait(false);

                cart.Quote = new XElement("Quote");
                foreach (var product in message.Products)
                {
                    cart.EnterQuotedRate(product.Product.Convert(), product.EstimatedMatches, product.QuotedRate);
                }

                cart.QuotedTotal(message.Products.Select(p => p.EstimatedMatches * p.QuotedRate).Sum());
                cart.OrderMinimum(message.OrderMinimum);

                await this.dataContext.SaveChangesAsync();

                var @event = new QuoteCreatedEvent();
                @event.CartId = cartId;
                @event.QuotedTotal = cart.QuotedTotal();
                @event.Quote = cart.Quote;

                await context.Publish(@event);
                await this.dataContext.SaveChangesAsync();
            }
        }

        #endregion

        #region IHandleMessages<SubmitNationBuilderOrderCommand> Members

        /// <inheritdoc />
        public async Task Handle(SubmitNationBuilderOrderCommand message, IMessageHandlerContext context)
        {
            var cartId = message.CartId;

            using (context.Alias())
            {
                var cart = await this.dataContext
                    .SetOf<Cart>()
                    .ForNationBuilder(cartId)
                    .Where(c => c.IsActive)
                    .Include(c => c.Client)
                    .SingleAsync()
                    .ConfigureAwait(false);

                // This is ok to do. It's possible that the submit command runs out of order to the select list and product commands. Retry will deal with this rare scenario.
                if (!cart.QuotedProducts().Any() || cart.ExternalId == null) throw new InvalidOperationException($"Cart {cartId} has 0 selected products or has not selected a list. Cannot submit the order.");

                // Complete the order
                var po = cart.Complete();
                this.dataContext.SetOf<ProductOrder>().Add(po);

                await this.dataContext.SaveChangesAsync().ConfigureAwait(false);

                var @event = new NationBuilderOrderPlacedEvent
                {
                    UserId = cart.Client.UserId,
                    CartId = cart.Id,
                    OrderMinimum = cart.OrderMinimum(),
                    DateSubmitted = DateTime.UtcNow,
                    TotalRecords = cart.RecordCount,
                    ListId = cart.ExternalId.Value,
                    ListName = cart.Name,
                    IntegrationId = cart.IntegrationId.Value
                };

                var products = cart.QuotedProducts().Select(p => new Orderproduct()
                {
                    Title = p.Item1.Convert().GetDescription(),
                    EstimatedMatches = p.Item2,
                    PPU = p.Item3,
                    Product = p.Item1.Convert()
                });

                @event.Products.AddRange(products);

                await context.Publish(@event).ConfigureAwait(false);

                this.MarkAsComplete();
            }
        }

        #endregion

        #region TIMEOUT

        /// <inheritdoc />
        public async Task Timeout(CartExpiredTimeout state, IMessageHandlerContext context)
        {
            using (SecurityHelper.Alias(WellKnownIdentifiers.SystemIdentity))
            {
                var cart = await this.dataContext
                    .SetOf<Cart>()
                    .Where(c => c.Client.UserId == this.Data.UserId)
                    .Where(c => c.Id == this.Data.CartId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
                if (cart != null && cart.IsActive)
                {
                    // Cancel the order
                    cart.Cancel();
                    await this.dataContext.SaveChangesAsync().ConfigureAwait(false);

                    var @event = new NationBuilderOrderExpiredEvent {CartId = cart.Id, UserId = cart.Client.UserId};
                    await context.Publish(@event).ConfigureAwait(false);
                }

                // Send alert?
                this.MarkAsComplete();
            }
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<NationBuilderCartData> mapper)
        {
            mapper.ConfigureMapping<CreateNationBuilderCartCommand>(message => message.CartId).ToSaga(saga => saga.CartId);
            mapper.ConfigureMapping<SelectListForCartCommand>(message => message.CartId).ToSaga(saga => saga.CartId);
            mapper.ConfigureMapping<EnterQuoteForNationBuilderOrderCommand>(message => message.CartId).ToSaga(saga => saga.CartId);
            mapper.ConfigureMapping<SubmitNationBuilderOrderCommand>(message => message.CartId).ToSaga(saga => saga.CartId);
        }

        #endregion
    }
}