using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Core.ComponentModel;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing.Manifest;
using AccurateAppend.Messaging;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Websites.Clients.Areas.Order.Analyzers;
using EventLogger;
using Microsoft.AspNet.SignalR;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Csv.Messages
{
    /// <summary>
    /// Saga designed to process the CSV file order sales process.
    /// </summary>
    /// <remarks>
    /// Responds to the request by
    /// 1. Determining if cart exists, if true: perform no work
    /// 2. Creating new cart if no existing cart exists
    /// 3. Raise the CsvCartCreatedEvent
    /// 4. Analyze the uploaded file
    /// 5. Set the product selections
    /// 6. Confirm the order
    /// 7. Raise the CsvOrderPlacedEvent
    /// </remarks>
    public class CsvSalesSaga : Saga<CsvCartData>, 
        IAmStartedByMessages<CreateCsvCartCommand>,
        IAmStartedByMessages<AnalyzeRawCsvFileCommand>,
        IHandleMessages<EnterQuoteForCsvOrderCommand>,
        IHandleMessages<ColumnMapCsvOrderCommand>,
        IHandleMessages<SubmitCsvOrderCommand>,
        IHandleTimeouts<CartExpiredTimeout>
    {
        #region Fields

        private readonly Sales.DataAccess.DefaultContext dataContext;
        private readonly IFileLocation tempFolder;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvSalesSaga"/> class.
        /// </summary>
        /// <param name="dataContext">The <see cref="Sales.DataAccess.DefaultContext"/> providing data access to the handler.</param>
        /// <param name="tempFolder">The <see cref="IFileLocation"/> providing access to the temp folder location.</param>
        public CsvSalesSaga(Sales.DataAccess.DefaultContext dataContext, IFileLocation tempFolder)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            if (tempFolder == null) throw new ArgumentNullException(nameof(tempFolder));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
            this.tempFolder = tempFolder;
        }

        #endregion

        #region IHandleMessages<CreateCsvCartCommand> Members

        /// <inheritdoc />
        /// <remarks>
        /// Normally this message starts the saga but due to possible race conditions, the <see cref="AnalyzeRawCsvFileCommand"/> message may start this
        /// instead.
        /// </remarks>
        public virtual async Task Handle(CreateCsvCartCommand message, IMessageHandlerContext context)
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
                cart = Cart.ForClientFile(client, cartId);
                this.dataContext.SetOf<Cart>().Add(cart);
                await this.dataContext.SaveChangesAsync().ConfigureAwait(false);

                // 24 hours to finish up
                await this.RequestTimeout<CartExpiredTimeout>(context, DateTime.UtcNow.AddHours(8)).ConfigureAwait(false);

                var @event = new CsvCartCreatedEvent
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

        #region IHandleMessages<AnalyzeCsvFileCommand> Members

        /// <inheritdoc />
        /// <remarks>
        /// Due to possible race conditions, the <see cref="AnalyzeRawCsvFileCommand"/> message may start this
        /// saga as well. This prevents the analysis from being lost.
        /// </remarks>
        public virtual async Task Handle(AnalyzeRawCsvFileCommand message, IMessageHandlerContext context)
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

            var file = this.tempFolder.CreateInstance(systemFileName);

            // determine if file contents are ',' or '|' or '\t' delimited
            this.Data.Delimiter =
                await CsvFileContent.DiscoverDelimiterAsync(file).ConfigureAwait(false) ??
                CsvFileContent.DefaultDelimiter;

            var csvFile = new Plugin.Storage.CsvFile(file);
            csvFile.Delimiter = this.Data.Delimiter.Value;

            await cart.UpdateFileInformation(csvFile, message.ClientFileName).ConfigureAwait(false);

            var results = await this.AnalyizeFile(csvFile).ConfigureAwait(false);
            foreach (var analysis in results)
            {
                cart.AnalyzedProduct(analysis.Item1.Convert(), analysis.Item2);
            }

            await this.dataContext.SaveChangesAsync().ConfigureAwait(false);

            using (new Correlation(cartId))
            {
                Logger.LogEvent($"Completing analysis on cart {cartId}", Severity.None, Application.Clients);
            }
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

        #region IHandleMessages<EnterQuoteForCsvOrderCommand> Members

        /// <inheritdoc />
        public async Task Handle(EnterQuoteForCsvOrderCommand message, IMessageHandlerContext context)
        {
            using (context.Alias())
            {
                var cartId = message.CartId;
                var cart = await this.dataContext
                    .SetOf<Cart>()
                    .ForCsv(cartId)
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

                cart.LoadManifest(message.Manifest);

                var @event = new QuoteCreatedEvent();
                @event.CartId = cartId;
                @event.QuotedTotal = cart.QuotedTotal();
                @event.Quote = cart.Quote;

                await context.Publish(@event).ConfigureAwait(false);
                await this.dataContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        #endregion

        #region IHandleMessages<ColumnMapCsvOrderCommand> Members

        /// <inheritdoc />
        public Task Handle(ColumnMapCsvOrderCommand message, IMessageHandlerContext context)
        {
            var hasHeaderRow = message.HasHeaderRow;
            var columnMap = message.ColumnMap;

            this.Data.HasHeaderRow = hasHeaderRow;
            this.Data.ColumnMap = new XDocument(new XElement("ColumnMap", columnMap)); 

            return Task.CompletedTask;
        }

        #endregion

        #region IHandleMessages<SubmitCsvOrderCommand> Members

        /// <inheritdoc />
        public async Task Handle(SubmitCsvOrderCommand message, IMessageHandlerContext context)
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
                if (!cart.QuotedProducts().Any() || this.Data.ColumnMap == null) throw new InvalidOperationException($"Cart {cartId} has 0 selected products or has no column mapping. Cannot submit the order.");

                // Complete the order
                var po = cart.Complete();
                this.dataContext.SetOf<ProductOrder>().Add(po);

                await this.dataContext.SaveChangesAsync().ConfigureAwait(false);

                var @event = new CsvOrderPlacedEvent
                {
                    CartId = cartId,
                    UserId = cart.Client.UserId,
                    HasHeaderRow = this.Data.HasHeaderRow,
                    ColumnMap = this.Data.ColumnMap.Root,
                    QuotedTotal = cart.QuotedTotal(),
                    Delimiter = this.Data.Delimiter ?? CsvFileContent.DefaultDelimiter
                };
                var products = cart.QuotedProducts().Select(p => p.Item1).Select(p => p.Convert());

                @event.Products.AddRange(products);

                await context.Publish(@event).ConfigureAwait(false);

                this.MarkAsComplete();
            }
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<CsvCartData> mapper)
        {
            mapper.ConfigureMapping<CreateCsvCartCommand>(message => message.CartId).ToSaga(saga => saga.CartId);
            mapper.ConfigureMapping<AnalyzeRawCsvFileCommand>(message => message.CartId).ToSaga(saga => saga.CartId);
            mapper.ConfigureMapping<EnterQuoteForCsvOrderCommand>(message => message.CartId).ToSaga(saga => saga.CartId);
            mapper.ConfigureMapping<ColumnMapCsvOrderCommand>(message => message.CartId).ToSaga(saga => saga.CartId);
            mapper.ConfigureMapping<SubmitCsvOrderCommand>(message => message.CartId).ToSaga(saga => saga.CartId);
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

                    var @event = new CsvOrderExpiredEvent {CartId = cart.Id, UserId = cart.Client.UserId};
                    await context.Publish(@event).ConfigureAwait(false);
                }

                this.MarkAsComplete();
            }
        }

        #endregion

        #region Helper Methods

        protected virtual async Task<List<Tuple<DataServiceOperation, Int32>>> AnalyizeFile(CsvFileContent source)
        {
            try
            {
                // These products cover the entire set of the possible required inputs that will use file based estimates
                // Note we use PublicProducts here even though we're building a manifest (and thus DataServiceOperation) to prevent accidental misuse
                var sampledProducts = new[]
                {
                    PublicProduct.CASS,
                    PublicProduct.EMAIL_VER_DELIVERABLE,
                    PublicProduct.PHONE,
                    PublicProduct.PHONE_PREM
                };
                var manifest = new ManifestBuilder();
                manifest.Operations.AddRange(sampledProducts.Select(p => OperationDefinition.LoadFromDefinition(p.Convert())));

                var fileSource = new ColumnMapper.FileSource(source);
                await fileSource.BuildMapAsync(manifest, CancellationToken.None).ConfigureAwait(false);

                var votes = new List<Tuple<DataServiceOperation, Int32>>();

                var analyzer = new AnalyzerSource();

#if DEBUG
                // Diagnostics observer
                {
                    var index = 0;
                    var o = new DynamicObserver<String[]>(r =>
                    {
                        index = index + 1;
                        Console.WriteLine($"Row {index}");
                        Console.WriteLine(String.Join(",", r));
                    });

                    analyzer.Subscribe(o);
                }
#endif
                var analyzers = new List<OperationSuitabilityObserver>();

                // Party Search Suitability - any operation that searches via party is fine. Since we expand this one, let's give it a variable so we can come back to this later
                var partyAnalysis = new PartyBasedOperationSuitabilityObserver(PublicProduct.PHONE_PREM, fileSource.ColumnMap);
                if (PartyBasedOperationSuitabilityObserver.CanAnalyze(fileSource.ColumnMap))
                {
                    analyzers.Add(partyAnalysis);
                }

                // CASS Suitability - any operation that searches via address is fine
                var addressAnalysis = new AddressSuitabilityObserver(PublicProduct.CASS, fileSource.ColumnMap);
                if (AddressSuitabilityObserver.CanAnalyze(fileSource.ColumnMap))
                {
                    analyzers.Add(addressAnalysis);
                }

                // Phone Verification Suitability
                var operation = manifest.Operations.First(op => op.Name == PublicProduct.PHONE.Convert());
                var phoneAnalysis = new MustContainOperationSuitabilityObserver(operation, fileSource.ColumnMap);
                if (MustContainOperationSuitabilityObserver.CanAnalyze(fileSource.ColumnMap, operation))
                {
                    analyzers.Add(phoneAnalysis);
                }

                // Email Verification Suitability
                operation = manifest.Operations.First(op => op.Name == PublicProduct.EMAIL_VER_DELIVERABLE.Convert());
                var emailAnalysis = new UnaryBasedOperationSuitabilityObserver(PublicProduct.EMAIL_VER_DELIVERABLE, FieldName.EmailAddress, fileSource.ColumnMap);
                if (UnaryBasedOperationSuitabilityObserver.CanAnalyze(fileSource.ColumnMap, operation))
                {
                    analyzers.Add(emailAnalysis);
                }

                analyzers.ForEach(a => analyzer.Subscribe(a));

                await analyzer.Execute(source);

                // Be a mensch and expand these out for the caller so we cover all the products supported for CSV ordering
                votes.Add(new Tuple<DataServiceOperation, Int32>(PublicProduct.DEMOGRAHICS.Convert(), partyAnalysis.Matches()));
                votes.Add(new Tuple<DataServiceOperation, Int32>(PublicProduct.EMAIL_BASIC_NO_SUPRESSION_W_VERIFICATION.Convert(), partyAnalysis.Matches()));
                votes.Add(new Tuple<DataServiceOperation, Int32>(PublicProduct.NCOA48.Convert(), partyAnalysis.Matches()));
                votes.Add(new Tuple<DataServiceOperation, Int32>(PublicProduct.PHONE_PREM.Convert(), partyAnalysis.Matches()));
                votes.Add(new Tuple<DataServiceOperation, Int32>(PublicProduct.PHONE_MOB.Convert(), partyAnalysis.Matches()));
                votes.Add(new Tuple<DataServiceOperation, Int32>(PublicProduct.PHONE.Convert(), phoneAnalysis.Matches()));
                votes.Add(new Tuple<DataServiceOperation, Int32>(PublicProduct.EMAIL_VER_DELIVERABLE.Convert(), emailAnalysis.Matches()));
                votes.Add(new Tuple<DataServiceOperation, Int32>(PublicProduct.PHONE_REV_PREM.Convert(), phoneAnalysis.Matches()));
                votes.Add(new Tuple<DataServiceOperation, Int32>(PublicProduct.EMAIL_BASIC_REV.Convert(), emailAnalysis.Matches()));

#if DEBUG
                Debug.Assert(votes.All(v => Enum.IsDefined(typeof(PublicProduct), v.Item1.Convert())), "Non PublicProduct DSO found in analysis");
#endif
                return votes;
            }
            catch (Exception e)
            {
                // Since this is rather new we'll log a fatal error here and use it to help clean out the cockroaches
                // Any errors will simply mean we use default rates and estimations
                if (Debugger.IsAttached) Debugger.Break();
                Logger.LogEvent(e, Severity.Fatal, source.ToString());
                return new List<Tuple<DataServiceOperation, Int32>>();
            }
        }

        #endregion
    }
}