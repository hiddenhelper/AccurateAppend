using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing.Manifest;
using AccurateAppend.JobProcessing.Reporting;
using AccurateAppend.Sales.Contracts.Messages;
using AccurateAppend.Sales.DataAccess;
using NServiceBus;

namespace AccurateAppend.Sales.Handlers
{
    /// <summary>
    /// Used to normalized automation based deal creation for public ordering logic to a single type.
    /// </summary>
    public abstract class PublicDealCreationHandlerBase
    {
        #region Fields

        private readonly IOrderCalculationService calculator;
        private readonly DefaultContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicDealCreationHandlerBase"/> class.
        /// </summary>
        /// <param name="dataContext">The required <see cref="DefaultContext"/> component.</param>
        /// <param name="calculator">The required <see cref="IOrderCalculationService"/> component that performs calculation services.</param>
        protected PublicDealCreationHandlerBase(DefaultContext dataContext, IOrderCalculationService calculator)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            if (calculator == null) throw new ArgumentNullException(nameof(calculator));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
            this.calculator = calculator;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Crafts a queryable that can be used to locate the specific <see cref="Cart"/> that is
        /// appropriate for the type of cart matching the user order.
        /// </summary>
        /// <remarks>
        /// Implementors must override this method to return a cart for the order that is owned by
        /// the indicated user AND filtered to the required order type.
        /// </remarks>
        /// <param name="userId">The owner of the cart.</param>
        /// <param name="cartId">The identifier of the order.</param>
        /// <returns>A queryable that can produce the required search for the needed cart.</returns>
        protected abstract IQueryable<Cart> BuildQueryForCart(Guid userId, Guid cartId);

        /// <summary>
        /// Creates the appropriate <see cref="ICostService"/> that should be used for order item
        /// rate card based generation that is appropriate for the type of order represented by
        /// the supplied <paramref name="cart"/>.
        /// </summary>
        /// <remarks>
        /// Implementors must override this method to create the concrete cost service based on the
        /// logic of the component that is to be used to craft and populate order rates for the
        /// <paramref name="cart"/>.
        /// </remarks>
        /// <param name="cart">The <see cref="Cart"/> to create the cost service instance for. This parameter should be the result from <see cref="BuildQueryForCart"/> method.</param>
        /// <returns>The <see cref="ICostService"/> instance that should be used by this component for order rates.</returns>
        protected abstract ICostService CreateCostService(Cart cart);
        
        /// <summary>
        /// Using the supplied <paramref name="costService"/> and <paramref name="message"/>, populate the
        /// <paramref name="order"/> content.
        /// </summary>
        /// <remarks>
        /// Implementors are not required to override this logic unless
        /// </remarks>
        /// <param name="order">The <see cref="BillableOrder"/> that should be populated.</param>
        /// <param name="costService">The <see cref="ICostService"/> that is used to build rates used in the <paramref name="order"/>.</param>
        /// <param name="message">The message that contains the processing report and optional manifest directives to base the order from.</param>
        protected virtual Task FillFromRateCard(BillableOrder order, ICostService costService, CreateJobDealCommand message)
        {
            var manifest = message.Manifest == null ? null : new ManifestBuilder(message.Manifest);
            var report = new ProcessingReport(message.ProcessingReport);

            order.Processing.AssociateReport(message.ProcessingReport);

            return manifest == null
                ? this.calculator.FillFromRateCard(order, costService, report)
                : this.calculator.FillFromRateCard(order, costService, manifest, report);
        }

        /// <summary>
        /// Contains the core logic to locate or create the <see cref="MutableDeal"/> needed for the job order.
        /// </summary>
        /// <remarks>
        /// Deals created here will always be system owned.
        /// </remarks>
        /// <param name="cart">The <see cref="Cart"/> locate the deal for. This parameter should be the result from <see cref="BuildQueryForCart"/> method.</param>
        /// <returns>The existing or the created <see cref="MutableDeal"/> for the job order.</returns>
        protected virtual async Task<MutableDeal> LocateDeal(Cart cart)
        {
            var publicKey = cart.Id;

            // All automation deals start off belonging to system
            using (SecurityHelper.Alias(WellKnownIdentifiers.SystemIdentity))
            {
                // do we have a deal already?
                var deal = await this.dataContext
                    .SetOf<BillableOrder>()
                    .Where(o => o.PublicKey == publicKey.ToString())
                    .Select(o => o.Deal)
                    .OfType<MutableDeal>()
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                if (deal != null) return deal;

                var client = cart.Client;

                deal = new MutableDeal(client);
                deal.Notes.Add("Deal created by automated billing");
                deal.Title = $"Deal for {cart.Name}";
                deal.Description = $"Deal for {cart.Client.UserName}";
                
                var order = deal.CreateOrder(publicKey);
                order.OrderMinimum = cart.OrderMinimum();
                order.PerformAutoBilling = true;

                this.dataContext
                    .SetOf<MutableDeal>()
                    .Add(deal);

                return deal;
            }
        }

        /// <summary>
        /// After all order logic has run, publishes any required events used by this and any other system.
        /// </summary>
        /// <remarks>
        /// Publishes the <see cref="DealCreatedEvent"/> events always.
        /// If the <see cref="DealStatus"/> is Billing, the <see cref="DealApprovedEvent"/> event will be published as well.
        /// </remarks>
        /// <param name="deal">The populated deal for the job.</param>
        /// <param name="context">The context of the currently handled message.</param>
        protected virtual async Task DispatchEvents(MutableDeal deal, IMessageHandlerContext context)
        {
            Debug.Assert(deal.Id.HasValue);

            var event2 = new DealCreatedEvent
            {
                Client = deal.Client.UserId,
                Amount = deal.Total(),
                DateCreated = deal.CreatedDate,
                DealId = deal.Id.Value,
                PublicKey = new Guid(deal.OriginatingOrder().PublicKey)
            };

            await context.Publish(event2);

            if (deal.Status == DealStatus.Billing)
            {
                var event3 = new DealApprovedEvent()
                {
                    Client = deal.Client.UserId,
                    Amount = deal.Total(),
                    DealId = deal.Id.Value,
                    PublicKey = new Guid(deal.OriginatingOrder().PublicKey)
                };

                await context.Publish(event3);
            }
        }

        #endregion

        #region Message Handler

        /// <summary>
        /// Performs the actual common core logic of processing a <see cref="CreateJobDealCommand"/> message.
        /// </summary>
        /// <remarks>
        /// Concrete handlers should never be required to override this method and can simply dispatch the
        /// concrete <paramref name="message"/> instance. Implemented by performing (in order):
        ///
        /// - <see cref="BuildQueryForCart"/>
        /// - <see cref="CreateCostService"/>
        /// - <see cref="LocateDeal"/>
        /// - <see cref="FillFromRateCard"/>
        /// - <see cref="CreateBill"/>
        /// - Save to DB
        /// - <see cref="DispatchEvents"/>
        /// </remarks>
        public virtual async Task HandleCore(CreateJobDealCommand message, IMessageHandlerContext context)
        {
            var publicKey = message.PublicKey;

            var cart = await this.BuildQueryForCart(message.UserId, publicKey)
                .FirstAsync()
                .ConfigureAwait(false);

            var costService = this.CreateCostService(cart);

            var deal = await this.LocateDeal(cart);
            var order = deal.OriginatingOrder();

            // Is the deal editable anymore?
            if (!deal.Status.CanBeEdited()) return;

            await this.FillFromRateCard(order, costService, message);

            deal.Amount = deal.Total(); // HACK: we don't have the events in place to keep this synched

            await this.dataContext.SaveChangesAsync(); // Ensures we've got PKs

            var content = await this.CreateBill(deal);
            if (content !=null)
            {
                order.Bill.ContractType = ContractType.Receipt;
                order.PerformAutoBilling = true;
                order.Content = content;

                deal.SubmitForReview(new Audit("Account is set for automatic bill creation", WellKnownIdentifiers.SystemUserId));
                deal.Approve(new Audit("Automatically approved by system", WellKnownIdentifiers.SystemUserId));
            }

            await this.dataContext.SaveChangesAsync(); // Ensures we've got PKs

            await this.DispatchEvents(deal, context);
        }

        /// <summary>
        /// Allows the concrete handler instance to perform any logic to construct the appropriate
        /// <see cref="BillContent"/> for the <paramref name="deal"/>.
        /// </summary>
        /// <param name="deal">The <see cref="MutableDeal"/> that should be used to create a <see cref="BillContent"/> for, if any.</param>
        /// <returns>The <see cref="BillContent"/> that should be used for the <paramref name="deal"/>, if any. A null return means the <paramref name="deal"/> should be created but not billed automatically.</returns>
        protected abstract Task<BillContent> CreateBill(MutableDeal deal);

        #endregion
    }
}