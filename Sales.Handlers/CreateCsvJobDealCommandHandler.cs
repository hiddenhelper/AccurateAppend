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
    /// Handler for the <see cref="CreateCsvJobDealCommand"/> message. This will create a <see cref="DealBinder"/> for completed CSV upload.
    /// </summary>
    /// <remarks>
    /// Will use the client specific rate cards, if defined, falling back to the default rate card if need be.
    /// </remarks>
    public class CreateCsvJobDealCommandHandler : PublicDealCreationHandlerBase, IHandleMessages<CreateCsvJobDealCommand>
    {
        #region Fields

        private readonly DefaultContext dataContext;
        private readonly IBillFormatterFactory formatterFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCsvJobDealCommandHandler"/> class.
        /// </summary>
        /// <param name="dataContext">The required <see cref="DefaultContext"/> component.</param>
        /// <param name="calculator">The required <see cref="IOrderCalculationService"/> component that performs calculation services.</param>
        /// <param name="formatterFactory">The <see cref="IBillFormatterFactory"/> used to create bill content formatters with.</param>
        public CreateCsvJobDealCommandHandler(DefaultContext dataContext, IOrderCalculationService calculator, IBillFormatterFactory formatterFactory) : base(dataContext, calculator)
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
        /// that is based on NB orders.
        /// </remarks>
        protected override IQueryable<Cart> BuildQueryForCart(Guid userId, Guid cartId)
        {
            return this.dataContext
                .SetOf<Cart>()
                .ForClient(userId)
                .ForCsv(cartId)
                .Include(c => c.Client);
        }

        /// <inheritdoc />
        /// <remarks>
        /// Always returns a <see cref="CustomerCostService"/> instance based on the <paramref name="cart"/> owner.
        /// </remarks>
        protected override ICostService CreateCostService(Cart cart)
        {
            var client = cart.Client;
            return new CustomerCostService(client, this.dataContext);
        }

        /// <inheritdoc />
        protected override async Task<MutableDeal> LocateDeal(Cart cart)
        {
            var deal = await base.LocateDeal(cart).ConfigureAwait(false);

            deal.Description = $"Deal for {cart.Name}";
            deal.Title = cart.Name;

            return deal;
        }

        /// <inheritdoc />
        protected override async Task<BillContent> CreateBill(MutableDeal deal)
        {
            var order = deal.OriginatingOrder();
            var formatter = await this.formatterFactory.ForPublic(new Guid(order.PublicKey)).ConfigureAwait(false);
            var model = new BillModel(order);

            var content = new BillContent(await formatter.SendFrom(model).ConfigureAwait(false));
            content.IsHtml = formatter.IsHtml;
            content.SendTo.AddRange(await formatter.CreateTo(model));
            content.BccTo.AddRange(await formatter.CreateBcc(model));
            content.Subject = await formatter.CreateSubject(model);
            content.Body = $"{await formatter.CreateHeader(model)}{await formatter.CreateBody(order)}{await formatter.CreateFooter(model)}";

            return content;
        }

        #endregion

        #region IHandleMessages<CreateCsvJobDealCommand> Members

        /// <inheritdoc />
        public virtual Task Handle(CreateCsvJobDealCommand message, IMessageHandlerContext context)
        {
            return this.HandleCore(message, context);
        }

        #endregion
    }
}