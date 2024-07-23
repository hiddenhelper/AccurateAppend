using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.JobManagement.Contracts;
using AccurateAppend.JobProcessing;
using AccurateAppend.JobProcessing.Manifest;
using AccurateAppend.JobProcessing.Manifest.Strategies;
using AccurateAppend.JobProcessing.Manifest.Xml;
using AccurateAppend.Messaging;
using AccurateAppend.Plugin.Storage;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Messages;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Websites.Clients.Areas.Order.Automation.Messages;
using AccurateAppend.Websites.Clients.Areas.Order.Csv.Messages;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Messages.Csv
{
    /// <summary>
    /// Saga designed to process the CSV file order fulfillment process.
    /// </summary>
    /// <remarks>
    /// Responds to the request by submitting a New CSV job command.
    /// Waits for the matching Job Created Event prior to completing.
    /// </remarks>
    public class CsvOrderProcessingSaga : Saga<CsvOrderData>,
        IAmStartedByMessages<CsvOrderPlacedEvent>,
        IAmStartedByMessages<AutomationOrderPlacedEvent>,
        IHandleMessages<JobCreatedEvent>,
        IHandleMessages<JobDeletedEvent>,
        IHandleMessages<JobCompletedEvent>,
        IHandleMessages<DealCompletedEvent>
    {
        #region Fields

        private readonly DefaultContext dataContext;
        private readonly IFileLocation tempFolder;
        private readonly IFileLocation inbox;
        private readonly IFileLocation rawCustomerFiles;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvOrderProcessingSaga"/> class.
        /// </summary>
        /// <param name="dataContext">The <see cref="DefaultContext"/> providing data access to the handler.</param>
        /// <param name="files">The <see cref="StandardFileLocations"/> that provides access to the files location.</param>
        public CsvOrderProcessingSaga(DefaultContext dataContext, StandardFileLocations files)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            if (files == null) throw new ArgumentNullException(nameof(files));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
            this.tempFolder = files.Temp;
            this.inbox = files.Inbox;
            this.rawCustomerFiles = files.RawCustomerFiles;
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<CsvOrderData> mapper)
        {
            mapper.ConfigureMapping<CsvOrderPlacedEvent>(message => message.CartId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<AutomationOrderPlacedEvent>(message => message.CartId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<JobCreatedEvent>(message => message.JobKey).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<JobCompletedEvent>(message => message.JobKey).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<JobDeletedEvent>(message => message.JobKey).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<DealCompletedEvent>(message => message.PublicKey).ToSaga(saga => saga.OrderId);
        }

        #endregion

        #region IHandleMessages<CsvOrderPlacedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(CsvOrderPlacedEvent message, IMessageHandlerContext context)
        {
            var cartId = message.CartId;
            var orderedProducts = message.Products;

            using (context.Alias())
            {
                var cart = await this.dataContext
                    .SetOf<Cart>()
                    .ForCsv(cartId)
                    .Include(c => c.Client)
                    .Where(c => !c.IsActive)
                    .SingleAsync()
                    .ConfigureAwait(false);
                
                var hasHeader = message.HasHeaderRow;
                var columnMap = message.ColumnMap;
                var delimiter = message.Delimiter;
                var manifest = this.CreateManifest(cart, columnMap, delimiter, hasHeader, orderedProducts);

                await this.CopyToInbox(cart).ConfigureAwait(false);

                var command = new CreateCsvJobCommand
                {
                    UserId = cart.Client.UserId,
                    CustomerFileName = cart.Name,
                    Delimiter = delimiter,
                    Manifest = manifest.ToXml(),
                    JobKey = cart.Id
                };

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
                var cart = await this.dataContext
                    .SetOf<Cart>()
                    .ForCsv(cartId)
                    .Include(c => c.Client)
                    .Where(c => !c.IsActive)
                    .SingleAsync()
                    .ConfigureAwait(false);

                var manifest = cart.Manifest;

                await this.CopyToInbox(cart).ConfigureAwait(false);

                var command = new CreateCsvJobCommand
                {
                    UserId = cart.Client.UserId,
                    CustomerFileName = cart.Name,
                    Delimiter = manifest.InputDelimiter(),
                    Manifest = manifest,
                    JobKey = cart.Id
                };

                await context.Send(command).ConfigureAwait(false);
            }
        }

        #endregion

        #region Helpers

        protected virtual async Task CopyToInbox(CsvCart cart)
        {
            if (cart == null) throw new ArgumentNullException(nameof(cart));
            Contract.EndContractBlock();

            var clientFile = this.tempFolder.CreateInstance(cart.SystemFileName);

            await JobPipeline.BackupClientInputFileAsync(clientFile, this.rawCustomerFiles, cart.Id).ConfigureAwait(false);

            var inboxFile = this.inbox.CreateInstance(cart.Id.ToString());
            await clientFile.CopyToAsync(inboxFile).ConfigureAwait(false);
        }

        protected virtual ManifestBuilder CreateManifest(Cart cart, XElement columnMap, Char delimiter, Boolean hasHeaderRow, IEnumerable<PublicProduct> forProducts)
        {
            if (cart == null) throw new ArgumentNullException(nameof(cart));
            if (columnMap == null) throw new ArgumentNullException(nameof(columnMap));
            Contract.EndContractBlock();

            var phone = new[] {PublicProduct.PHONE_PREM, PublicProduct.PHONE_MOB, PublicProduct.PHONE_DA, PublicProduct.PHONE_BUS_DA};
            var products = forProducts.Normalize().OrderBy(p => p, PublicProductOrdering.Instance).ToArray();

            var manifest = new ManifestBuilder();
            foreach (var product in products)
            {
                var builder = OperationStrategyBuilder.ForProduct(product);
                builder.AppendToManifest(manifest);

                if (!phone.Contains(product)) continue;

                // Make sure that phone operations output MatchType
                var operation = manifest.Operations.Last();
                manifest.EnableMatchType(operation);
            }

            manifest.DeduplicateAllPhones();
            manifest.ColumnMap = new ColumnMap(columnMap);
            manifest.HasHeaderRow = hasHeaderRow;
            manifest.InputFieldDelimiter = delimiter.ToString();
            manifest.OutputFieldDelimiter = delimiter.ToString();

            return manifest;
        }

        #endregion

        #region IHandleMessages<JobCreatedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(JobCreatedEvent message, IMessageHandlerContext context)
        {
            this.Data.JobId = message.JobId;
            var publicKey = message.JobKey;

            var po = await this.dataContext
                .SetOf<ProductOrder>()
                .Where(o => o.Id == publicKey)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            po.Status = ProcessingStatus.Processing;
            
            await this.dataContext.SaveChangesAsync().ConfigureAwait(false);
        }

        #endregion

        #region IHandleMessages<JobCompletedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(JobCompletedEvent message, IMessageHandlerContext context)
        {
            var publicKey = message.JobKey;
            var userId = message.UserId;
            var completedDate = message.CompletedDate;

            var po = await this.dataContext
                .SetOf<ProductOrder>()
                .Where(o => o.Id == publicKey)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            var accounts = await this.dataContext
                .SetOf<RecurringBillingAccount>()
                .Where(a => a.ForClient.UserId == userId)
                .ToArrayAsync()
                .ConfigureAwait(false);

            var isSubscriber = accounts.Any(a => a.IsValidForDate(completedDate));

            if (isSubscriber)
            {
                po.Status = ProcessingStatus.Available;
                this.MarkAsComplete();
            }
            else
            {
                po.Status = ProcessingStatus.Billing;
            }

            await this.dataContext.SaveChangesAsync().ConfigureAwait(false);
        }

        #endregion

        #region IHandleMessages<DealCompletedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(DealCompletedEvent message, IMessageHandlerContext context)
        {
            var publicKey = message.PublicKey;
            
            var po = await this.dataContext
                .SetOf<ProductOrder>()
                .Where(o => o.Id == publicKey)
                .FirstAsync()
                .ConfigureAwait(false);

            po.Status = ProcessingStatus.Available;

            await this.dataContext.SaveChangesAsync().ConfigureAwait(false);

            this.MarkAsComplete();
        }

        #endregion

        #region IHandleMessages<JobDeletedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(JobDeletedEvent message, IMessageHandlerContext context)
        {
            var publicKey = message.JobKey;

            var po = await this.dataContext
                .SetOf<ProductOrder>()
                .Where(o => o.Id == publicKey)
                .FirstAsync()
                .ConfigureAwait(false);

            po.Status = ProcessingStatus.Canceled;

            await this.dataContext.SaveChangesAsync().ConfigureAwait(false);

            this.MarkAsComplete();
        }

        #endregion
    }
}