using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Messages;
using AccurateAppend.Websites.Clients.Areas.NationBuilder.Order.Messages;
using Integration.NationBuilder.Data;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Messages.NationBuilder
{
    /// <summary>
    /// Saga designed to process the NationBuilder list order fulfillment process.
    /// </summary>
    /// <remarks>
    /// Responds to the request by creating a new PushRequest.
    /// Waits for the matching Job Created Event prior to completing.
    /// </remarks>
    public class NationBuilderOrderProcessingSaga : Saga<NationBuilderOrderData>,
        IAmStartedByMessages<NationBuilderOrderPlacedEvent>,
        IHandleMessages<JobManagement.Contracts.JobCreatedEvent>,
        IHandleMessages<JobManagement.Contracts.JobCompletedEvent>,
        IHandleMessages<DealCompletedEvent>
    {
        #region Fields

        private readonly ISessionContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NationBuilderOrderProcessingSaga"/> class.
        /// </summary>
        /// <param name="dataContext">The <see cref="ISessionContext"/> providing data access to the handler.</param>
        public NationBuilderOrderProcessingSaga(ISessionContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<NationBuilderOrderData> mapper)
        {
            mapper.ConfigureMapping<NationBuilderOrderPlacedEvent>(message => message.CartId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<JobManagement.Contracts.JobCreatedEvent>(message => message.JobKey).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<JobManagement.Contracts.JobCompletedEvent>(message => message.JobKey).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<DealCompletedEvent>(message => message.PublicKey).ToSaga(saga => saga.OrderId);
        }

        #endregion

        #region IHandleMessages<NationBuilderOrderPlacedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(NationBuilderOrderPlacedEvent message, IMessageHandlerContext context)
        {
            var cartId = message.CartId;
            var orderedProducts = message.Products;
            var userId = message.UserId;
            var registrationId = message.IntegrationId;
            var listId = message.ListId;
            var listName = message.ListName;
            var totalRecords = message.TotalRecords;

            this.Data.OrderMinimum = message.OrderMinimum;

            var registration = await this.dataContext
                .SetOf<Registration>()
                .Where(r => r.Owner.UserId == userId)
                .Where(r => r.Id == registrationId)
                .SingleAsync()
                .ConfigureAwait(false);

            var listDetail = new NationBuilderList();
            listDetail.Id = listId;
            listDetail.Name = listName;
            listDetail.TotalPages = (Int32)Math.Ceiling(totalRecords / 100m);
            listDetail.TotalRecords = totalRecords;

            var processingInstructions = orderedProducts.Select(i =>
                            {
                                var instruction = new ProcessingInstruction(i.Product);
                                instruction.PerformOverwrites = i.Product == PublicProduct.PHONE_PREM;
                                return instruction;
                            });

            var uow = this.dataContext.CreateScope(ScopeOptions.AutoCommit);
            
            var request = new PushRequest(cartId, registration, listDetail, processingInstructions.ToArray());
            this.dataContext.SetOf<PushRequest>().Add(request);

            await uow.CommitAsync();
        }

        #endregion
        
        #region IHandleMessages<JobCreatedEvent> Members

        /// <inheritdoc />
        public virtual Task Handle(JobManagement.Contracts.JobCreatedEvent message, IMessageHandlerContext context)
        {
            this.Data.JobId = message.JobId;
            var publicKey = message.JobKey;

            var db = (ExtendedDbContext)this.dataContext;

            // Ugly hack for now as the NB system is still legacy
            return db.Database
                .ExecuteSqlCommandAsync("UPDATE [sales].[ProductOrder] SET [Status]=@p1 WHERE [OrderId]=@p0", publicKey, (Int32)ProcessingStatus.Processing);
        }

        #endregion

        #region IHandleMessages<JobCompletedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(JobManagement.Contracts.JobCompletedEvent message, IMessageHandlerContext context)
        {
            this.Data.JobId = message.JobId;

            var publicKey = message.JobKey;

            var db = (ExtendedDbContext) this.dataContext;

            // Ugly hack for now as the NB system is still legacy
            await db.Database
                .ExecuteSqlCommandAsync("UPDATE [sales].[ProductOrder] SET [Status]=@p1 WHERE [OrderId]=@p0", publicKey, (Int32) ProcessingStatus.Billing)
                .ConfigureAwait(false);
        }

        #endregion

        #region IHandleMessages<DealCompletedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(DealCompletedEvent message, IMessageHandlerContext context)
        {
            var publicKey = message.PublicKey;

            var db = (ExtendedDbContext)this.dataContext;

            // Ugly hack for now as the NB system is still legacy
            await db.Database
                .ExecuteSqlCommandAsync("UPDATE [sales].[ProductOrder] SET [Status]=@p1 WHERE [OrderId]=@p0", publicKey, (Int32)ProcessingStatus.Available)
                .ConfigureAwait(false);

            this.MarkAsComplete();
        }

        #endregion
    }
}