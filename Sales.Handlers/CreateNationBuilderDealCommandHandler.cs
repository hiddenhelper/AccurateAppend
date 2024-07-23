using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Data;
using AccurateAppend.Sales.Contracts.Services;
using AccurateAppend.Sales.Contracts.ViewModels;
using AccurateAppend.Sales.DataAccess;
using NServiceBus;

namespace AccurateAppend.Sales.Handlers
{
    /// <summary>
    /// Handler for the <see cref="CreateNationBuilderDealCommand"/> bus message.
    /// </summary>
    /// <remarks>
    /// Responds to a message by
    /// -Creating a new deal correlated to the job, if one does not already exist
    /// -If the Deal is editable, populate the order based on the processing report of job
    /// -If a Deal already exists and is NOT editable, no action is taken
    /// </remarks>
    public class CreateNationBuilderDealCommandHandler : PublicDealCreationHandlerBase, IHandleMessages<CreateNationBuilderDealCommand>
    {
        #region Fields

        private readonly DefaultContext dataContext;
        private readonly IBillFormatterFactory formatterFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateNationBuilderDealCommandHandler"/> class.
        /// </summary>
        /// <param name="dataContext">The required <see cref="DefaultContext"/> component.</param>
        /// <param name="calculator">The required <see cref="IOrderCalculationService"/> component that performs calculation services.</param>
        /// <param name="formatterFactory">The <see cref="IBillFormatterFactory"/> used to create bill content formatters with.</param>
        public CreateNationBuilderDealCommandHandler(DefaultContext dataContext, IOrderCalculationService calculator, IBillFormatterFactory formatterFactory) : base(dataContext, calculator)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            if (formatterFactory == null) throw new ArgumentNullException(nameof(formatterFactory));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
            this.formatterFactory = formatterFactory;
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        /// <remarks>
        /// Crafts a query predicate that returns the indicated cart for the user
        /// that is based on CSV orders.
        /// </remarks>
        protected override IQueryable<Cart> BuildQueryForCart(Guid userId, Guid cartId)
        {
            return this.dataContext
                .SetOf<Cart>()
                .ForClient(userId)
                .ForNationBuilder(cartId)
                .Include(c => c.Client);
        }

        /// <inheritdoc />
        /// <remarks>
        /// Always returns a <see cref="NationBuilderCostService"/> instance.
        /// </remarks>
        protected override ICostService CreateCostService(Cart cart)
        {
            return new NationBuilderCostService(this.dataContext);
        }

        /// <inheritdoc />
        protected override async Task<MutableDeal> LocateDeal(Cart cart)
        {
            var deal = await base.LocateDeal(cart).ConfigureAwait(false);

            deal.Title = $"Deal for {cart.Name}";
            deal.Description = $"NationBuilder list append for {cart.Name}";

            return deal;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Since at this point we have the actual deal logic complete and we're simply at the point of
        /// publishing the events we can interject ourselves here to deal with the extra NB specific
        /// logic of setting the deal id and status prior to the handler completing.
        /// </remarks>
        protected override async Task DispatchEvents(MutableDeal deal, IMessageHandlerContext context)
        {
            // HACK: We'll flip the status here till we get NationBuilder working on the same deal complete events
            await this.dataContext.Database.ExecuteSqlCommandAsync(
                    "UPDATE [integration].[NationBuilderPush] SET [Status]=3, [CurrentPage]=0, [DealId]=@p0 WHERE [SupressionId]=@p1",
                    deal.Id.Value,
                    new Guid(deal.OriginatingOrder().PublicKey))
                .ConfigureAwait(false);

            await base.DispatchEvents(deal, context);
        }

        /// <inheritdoc />
        protected override async Task<BillContent> CreateBill(MutableDeal deal)
        {
            var order = deal.OriginatingOrder();
            var formatter = await this.formatterFactory.ForNationBuilder(new Guid(order.PublicKey)).ConfigureAwait(false);
            var model = new BillModel(order);

            var content = new BillContent(await formatter.SendFrom(model).ConfigureAwait(false));
            content.SendTo.AddRange(await formatter.CreateTo(model));
            content.BccTo.AddRange(await formatter.CreateBcc(model));
            content.Subject = await formatter.CreateSubject(model);
            content.Body = $"{await formatter.CreateHeader(model)}{await formatter.CreateBody(order)}{await formatter.CreateFooter(model)}";
            content.IsHtml = formatter.IsHtml;

            return content;
        }

        #endregion

        #region IHandleMessages<CreateNationBuilderDealCommand> Members

        /// <inheritdoc />
        public virtual Task Handle(CreateNationBuilderDealCommand message, IMessageHandlerContext context)
        {
            return this.HandleCore(message, context);
        }

        #endregion
    }
}