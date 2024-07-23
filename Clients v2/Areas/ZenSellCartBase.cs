using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Sales;
using AccurateAppend.ZenDesk.Configuration;
using AccurateAppend.ZenDesk.Sales;
using AccurateAppend.ZenDesk.Sales.Deals;
using AccurateAppend.ZenDesk.Sales.LineItems;
using AccurateAppend.ZenDesk.Sales.Orders;
using AccurateAppend.ZenDesk.Sales.Products;

namespace AccurateAppend.Websites.Clients.Areas
{
    /// <summary>
    /// Contains implementation of the common Deal management routines for handlers related to public ordering.
    /// </summary>
    public abstract class ZenSellCartBase
    {
        #region Fields

        private readonly IDealsService dealsService;
        private readonly IOrdersService ordersService;
        private readonly ILineItemService lineService;
        private readonly IProductService productService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ZenSellCartBase"/> class.
        /// </summary>
        /// <param name="sales">The <see cref="SalesContext"/> providing access to ZenSell data.</param>
        protected ZenSellCartBase(SalesContext sales)
        {
            if (sales == null) throw new ArgumentNullException(nameof(sales));

            this.dealsService = sales.Deals;
            this.ordersService = sales.Orders;
            this.lineService = sales.Lines;
            this.productService = sales.Products;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Provides a lookup from AA user to Zen Id value.
        /// </summary>
        /// <remarks>
        /// If the user is Andy, return his ZenSell ID; Otherwise just return our operations account.
        /// </remarks>
        /// <param name="assignedTo">The identifier of the AA user to get a Zen Id value for.</param>
        protected virtual Int64? MapToZenUser(Guid assignedTo)
        {
            return assignedTo == WellKnownIdentifiers.Andy
                ? WellKnownUsers.Andy.Id
                : WellKnownUsers.Operations.Id;
        }

        protected async Task ListSelected(Guid cartId, Int64 contactId, String listName, Int64? salesRep)
        {
            var deal = new Deal
            {
                CustomFields = { PublicKey = cartId.ToString() },
                Currency = "USD", // Always
                ContactId = contactId,
                Name = listName,
                OwnerId = salesRep,
                StageId = Pipelines.SelfService.Stages.ListSelected // Start here
            };

            await this.dealsService.UpsertAsync(deal);
            Console.WriteLine($"Deal: {deal.Id} updated for cart: {cartId}");
        }
        
        protected async Task UpdateWithQuote(Guid cartId, XElement quote)
        {
            var deal = await this.dealsService.DetailAsync(cartId, CancellationToken.None).ConfigureAwait(false);
            if (deal == null) throw new InvalidOperationException($"Cannot find Zen Deal for cart: {cartId}");

            deal.Hot = true; // We're hot
            deal.StageId = Pipelines.SelfService.Stages.QuoteCreated;
            deal.Value = CartQuote.QuotedTotal(quote);

            deal = await this.dealsService.UpdateAsync(deal);

            Console.WriteLine($"Deal: {deal.Id} marked hot for cart: {cartId}");

            var order = await this.ordersService.DetailAsync(deal.Id, cartId);
            if (order != null)
            {
                // Nuke and rebuild since we died part way through building
                await this.ordersService.DeleteAsync(order.Id.Value);
            }

            order = new ZenDesk.Sales.Orders.Order();
            order.DealId = deal.Id;
            order = await this.ordersService.CreateAsync(order);

            foreach (var quotedProduct in CartQuote.QuotedProducts(quote))
            {
                var sku = quotedProduct.Item1.ToString();
                var rate = quotedProduct.Item3.RoundFractionalPennies();
                var quantity = quotedProduct.Item2;

                var product = await this.productService.DetailAsync(sku);
                var productId = product.Id.Value;

                var line = new NewLineItem();
                line.Sku = sku;
                line.Description = product.Name;
                line.ProductId = productId;
                line.Currency = "USD";
                line.Value = rate;
                line.Quantity = quantity;

                await this.lineService.CreateAsync(order.Id.Value, line);
            }
        }

        protected async Task Won(Guid cartId, Decimal quotedTotal)
        {
            var deal = await this.dealsService.DetailAsync(cartId, CancellationToken.None).ConfigureAwait(false);
            if (deal == null) throw new InvalidOperationException($"Cannot find Zen Deal for cart: {cartId}");

            deal.Value = quotedTotal;
            deal.StageId = Pipelines.SelfService.Stages.OrderSubmitted;

            await this.dealsService.UpdateAsync(deal);
            Console.WriteLine($"Deal: {deal.Id} Won for cart: {cartId}");
        }

        protected async Task Lost(Guid cartId)
        {
            var deal = await this.dealsService.DetailAsync(cartId, CancellationToken.None).ConfigureAwait(false);
            if (deal == null) return; // Never selected a list so we can ignore

            if (deal.StageId == Pipelines.SelfService.Stages.OrderSubmitted) return; // Once we're won, prevent mistakes

            deal.Hot = false;
            deal.StageId = Pipelines.SelfService.Stages.Lost;
            deal.LossReasonId = LossReasons.Abandoned;

            await this.dealsService.UpdateAsync(deal);
            Console.WriteLine($"Deal: {deal.Id} Lost for cart: {cartId}");
        }

        #endregion
    }
}