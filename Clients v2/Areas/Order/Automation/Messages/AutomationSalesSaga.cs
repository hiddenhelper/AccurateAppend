using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing.Manifest.Xml;
using AccurateAppend.Messaging;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;
using Microsoft.AspNet.SignalR;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Automation.Messages
{
    /// <summary>
    /// Saga designed to process the Automation Rule order sales process.
    /// </summary>
    /// <remarks>
    /// Responds to the request by
    /// 1. Determining if cart exists, if true: perform no work
    /// 2. Creating new cart if no existing cart exists
    /// 3. Raise the CreateAutomationCartCreatedEvent
    /// 4. Store the selected Manifest
    /// 5. Analyze the uploaded file
    /// 6. Confirm the order
    /// 7. Raise the AutomationOrderPlacedEvent
    /// </remarks>
    public class AutomationSalesSaga : Saga<AutomationCartData>,
        IAmStartedByMessages<CreateAutomationCartCommand>,
        IAmStartedByMessages<SelectManifestForCartCommand>,
        IHandleMessages<AnalyzeManifestCsvFileCommand>,
        IHandleMessages<SubmitAutomationOrderCommand>,
        IHandleTimeouts<CartExpiredTimeout>
    {
        #region Fields

        private readonly DefaultContext dataContext;
        private readonly IFileLocation tempFolder;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationSalesSaga"/> class.
        /// </summary>
        /// <param name="dataContext">The <see cref="DefaultContext"/> providing data access to the handler.</param>
        /// <param name="locations">The <see cref="IFileLocation"/> providing access to the temp folder location.</param>
        public AutomationSalesSaga(DefaultContext dataContext, Plugin.Storage.StandardFileLocations locations)
        {
            this.dataContext = dataContext;
            this.tempFolder = locations.Temp;
        }

        #endregion

        #region IHandleMessages<CreateAutomationCartCommand> Members

        /// <inheritdoc />
        public virtual async Task Handle(CreateAutomationCartCommand message, IMessageHandlerContext context)
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
                var cart = await this.dataContext
                    .SetOf<Cart>()
                    .FirstOrDefaultAsync(c => c.Id == cartId)
                    .ConfigureAwait(false);
                if (cart != null)
                {
                    if (!cart.IsActive) this.MarkAsComplete(); // kill with fire

                    return;
                }

                // New request so normal case
                cart = Cart.ForClientFile(client, cartId);
                this.dataContext.SetOf<Cart>().Add(cart);
                await this.dataContext.SaveChangesAsync().ConfigureAwait(false);

                // 24 hours to finish up
                await this.RequestTimeout<CartExpiredTimeout>(context, DateTime.UtcNow.AddHours(8)).ConfigureAwait(false);

                var @event = new AutomationCartCreatedEvent
                {
                    CartId = cartId,
                    CreatedBy = context.InitiatingUserId(),
                    UserId = userId,
                    SalesRep = client.OwnerId
                };

                await context.Publish(@event);
            }
        }

        #endregion

        #region IHandleMessages<SelectManifestForCartCommand> Members

        /// <inheritdoc />
        public virtual async Task Handle(SelectManifestForCartCommand message, IMessageHandlerContext context)
        {
            var cartId = message.CartId;

            using (context.Alias())
            {
                var cart = await this.dataContext
                    .SetOf<Cart>()
                    .ForCsv(cartId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                if (cart == null)
                {
                    // Race condition. We'll try it again later
                    await context.HandleCurrentMessageLater().ConfigureAwait(false);
                    return;
                }

                // Kill it with fire
                if (!cart.IsActive)
                {
                    this.MarkAsComplete();
                    return;
                }

                cart.LoadManifest(message.Manifest);

                await this.dataContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        #endregion

        #region IHandleMessages<AnalyzeManifestCsvFileCommand> Members

        /// <inheritdoc />
        public async Task Handle(AnalyzeManifestCsvFileCommand message, IMessageHandlerContext context)
        {
            Validator.ValidateObject(message, new ValidationContext(message));

            var cartId = message.CartId;
            var systemFileName = message.SystemFileName;
            var connectionId = message.RequestId;

            var cart = await this.dataContext
                .SetOf<Cart>()
                .ForCsv(cartId)
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
                this.MarkAsComplete(); // kill with fire
                return;
            }

            if (cart.Manifest == null)
            {
                // Race condition. We'll try it again later
                await context.HandleCurrentMessageLater().ConfigureAwait(false);
                return;
            }

            var file = this.tempFolder.CreateInstance(systemFileName);

            // determine if file contents are ',' or '|' or '\t' delimited
            var delimiter =
                await CsvFileContent.DiscoverDelimiterAsync(file).ConfigureAwait(false) ??
                CsvFileContent.DefaultDelimiter;

            var csvFile = new Plugin.Storage.CsvFile(file);
            csvFile.Delimiter = delimiter;
            var totalRecords = csvFile.CountRecords();

            await cart.UpdateFileInformation(csvFile, message.ClientFileName);

            cart.Manifest.InputDelimiter(delimiter);

            var cost = new CustomerCostService(cart.Client, this.dataContext);

            foreach (var product in await this.AnalyizeFile(cart, csvFile))
            {
                var operation = product.Item1;
                var estimatedMatches = product.Item2;
                var pricingModel = await cost.DeterminePricingModel(operation).ConfigureAwait(false);

                var rate = await cost.Estimate(operation, pricingModel, pricingModel == PricingModel.Match
                    ? estimatedMatches
                    : totalRecords).ConfigureAwait(false);

                cart.EnterQuotedRate(product.Item1, estimatedMatches, rate);

                cart.QuotedTotal(cart.QuotedTotal() + (estimatedMatches * rate));
            }

            cart.OrderMinimum(75.00m);

            var @event = new QuoteCreatedEvent();
            @event.CartId = cartId;
            @event.QuotedTotal = cart.QuotedTotal();
            @event.Quote = cart.Quote;

            await context.Publish(@event).ConfigureAwait(false);
            await this.dataContext.SaveChangesAsync().ConfigureAwait(false);

#if DEBUG
            using (var suppression = new System.Transactions.TransactionScope(
                System.Transactions.TransactionScopeOption.Suppress,
                System.Transactions.TransactionScopeAsyncFlowOption.Enabled))
            {
                var callback = GlobalHost.ConnectionManager.GetHubContext<CallbackHub>();
                callback.Clients.Client(connectionId.ToString()).callbackComplete();

                suppression.Complete();
            }
#else
                var callback = GlobalHost.ConnectionManager.GetHubContext<CallbackHub>();
                callback.Clients.Client(connectionId.ToString()).callbackComplete();
#endif
        }

        #endregion

        #region IHandleMessages<SubmitAutomationOrderCommand> Members

        /// <inheritdoc />
        public async Task Handle(SubmitAutomationOrderCommand message, IMessageHandlerContext context)
        {
            var cartId = message.CartId;

            using (context.Alias())
            {
                var cart = await this.dataContext
                    .SetOf<Cart>()
                    .Where(c => c.Id == cartId)
                    .Where(c => c.IsActive)
                    .Include(c => c.Client)
                    .SingleAsync()
                    .ConfigureAwait(false);

                // This is ok to do. It's possible that the submit command runs out of order to the mapping and the products commands. Retry will deal with this rare scenario.
                if (!cart.QuotedProducts().Any()) throw new InvalidOperationException($"Cart {cartId} has 0 selected products or has no column mapping. Cannot submit the order.");

                // Complete the order
                var po = cart.Complete();
                this.dataContext.SetOf<ProductOrder>().Add(po);

                await this.dataContext.SaveChangesAsync().ConfigureAwait(false);

                var @event = new AutomationOrderPlacedEvent
                {
                    CartId = cartId,
                    UserId = cart.Client.UserId,
                    QuotedTotal = cart.QuotedTotal()
                };

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
                    .ForClient(this.Data.UserId)
                    .ForCsv(this.Data.CartId)
                    .Include(c => c.Client)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
                if (cart != null && cart.IsActive)
                {
                    // Cancel the order
                    cart.Cancel();
                    await this.dataContext.SaveChangesAsync().ConfigureAwait(false);

                    var @event = new AutomationOrderExpiredEvent { CartId = cart.Id, UserId = cart.Client.UserId };
                    await context.Publish(@event).ConfigureAwait(false);
                }

                this.MarkAsComplete();
            }
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<AutomationCartData> mapper)
        {
            mapper.ConfigureMapping<CreateAutomationCartCommand>(message => message.CartId).ToSaga(saga => saga.CartId);
            mapper.ConfigureMapping<SelectManifestForCartCommand>(message => message.CartId).ToSaga(saga => saga.CartId);
            mapper.ConfigureMapping<AnalyzeManifestCsvFileCommand>(message => message.CartId).ToSaga(saga => saga.CartId);
            mapper.ConfigureMapping<SubmitAutomationOrderCommand>(message => message.CartId).ToSaga(saga => saga.CartId);
        }

        #endregion

        #region Helpers

        protected virtual Task<List<Tuple<DataServiceOperation, Int32>>> AnalyizeFile(CsvCart cart, CsvFileContent source)
        {
            var totalRecords = source.CountRecords();
            var operations = cart.Manifest.Operations().Select(o => o.OperationName()).FilterPreferences().SelectMany(o => o.ExpandComposites()).ToArray();

            var analysis = operations.Select(o => Tuple.Create(o, Convert.ToInt32(GetMatchRateForOperation(o) * totalRecords))).ToList();

            return Task.FromResult(analysis);
        }

        protected virtual Decimal GetMatchRateForOperation(DataServiceOperation operation)
        {
            switch (operation)
            {
                case DataServiceOperation.PHONE_MOB: return 0.25m;
                case DataServiceOperation.PHONE_BUS_DA: return 0.19m;
                case DataServiceOperation.NCOA48: return 0.03m;
                case DataServiceOperation.PHONE_BUS_PREM:
                    return 0.139004497m;
                case DataServiceOperation.EMAIL_BASIC_NO_SUPRESSION_W_VERIFICATION:
                    return 0.36m;
                case DataServiceOperation.PHONE_PREM:
                    return 0.26m;
                case DataServiceOperation.PHONE_CCO_MIXED:
                    return 0.57m;
                case DataServiceOperation.PHONE_DA:
                    return 0.1m;
                case DataServiceOperation.EMAIL_VERIFICATION:
                case DataServiceOperation.EMAIL_VER_DELIVERABLE:
                    return .9m;
                default:
                    return .35m;
            }
        }

        #endregion
    }
}