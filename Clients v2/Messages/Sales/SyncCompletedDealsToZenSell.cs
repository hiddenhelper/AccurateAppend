using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Messages;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.ZenDesk.Configuration;
using AccurateAppend.ZenDesk.Sales.Deals;
using AccurateAppend.ZenDesk.Sales.LineItems;
using AccurateAppend.ZenDesk.Sales.Orders;
using AccurateAppend.ZenDesk.Sales.Products;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Messages.Sales
{
    public class SyncCompletedDealsToZenSell : IHandleMessages<DealCompletedEvent>, IHandleMessages<SyncCompletedDealsToZenSell.SyncToZen>
    {
        #region Fields

        private readonly DefaultContext dataContext;
        private readonly IDealsService deals;
        private readonly IOrdersService orders;
        private readonly IProductService products;
        private readonly ILineItemService lines;

        #endregion

        #region Constructor

        public SyncCompletedDealsToZenSell(DefaultContext dataContext, SalesContext sales)
        {
            this.dataContext = dataContext;
            this.deals = sales.Deals;
            this.orders = sales.Orders;
            this.products = sales.Products;
            this.lines = sales.Lines;
        }

        #endregion

        #region IHandleMessages<DealCompletedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(DealCompletedEvent message, IMessageHandlerContext context)
        {
            var dealId = message.DealId;

            var source = await this.dataContext
                .SetOf<BillableOrder>()
                .Where(o => o.Deal.Id == dealId)
                .SelectMany(o => o.Lines)
                .Select(l =>
                    new OrderLines
                    {
                        ProductKey = l.Product.Key,
                        Description = l.Description,
                        Quantity = l.Quantity,
                        Price = l.Price
                    }
                )
                .Where(l => !ProductExtensions.NonBillableProductKeys.Contains(l.ProductKey))
                .ToArrayAsync()
                .ConfigureAwait(false);
            
            var command = new SyncToZen();
            command.Lines.AddRange(source);
            command.PublicKey = message.PublicKey;
            command.Total = message.Amount;

            await context.SendLocal(command);
        }

        #endregion

        #region IHandleMessages<SyncToZen> Members

        /// <inheritdoc />
        public virtual async Task Handle(SyncToZen message, IMessageHandlerContext context)
        {
            var publicKey = message.PublicKey;
            
            var deal = await this.deals.DetailAsync(publicKey).ConfigureAwait(false);
            if (deal == null) return; // No Zen Deal then bail

            deal.Value = message.Total;

            var order = await this.orders.DetailAsync(deal.Id, publicKey, CancellationToken.None);
            if (order != null)
            {
                // Nuke the whole damn thing and rebuild
                await this.orders.DeleteAsync(order.Id.Value);
            }

            order = new ZenDesk.Sales.Orders.Order();
            order.DealId = deal.Id;

            order = await this.orders.CreateAsync(order);
            
            foreach (var lineItem in message.Lines.Where(l => !ProductExtensions.NonBillableProductKeys.Contains(l.ProductKey)))
            {
                var productKey = lineItem.ProductKey;
                var productDescription = lineItem.Description;
                var ppu = lineItem.Price.RoundFractionalPennies();
                var quanity = lineItem.Quantity;

                var product = await this.products.DetailAsync(productKey);

                await this.lines.CreateAsync(order.Id.Value,
                    new NewLineItem()
                    {
                        Currency = "USD",
                        Name = productDescription,
                        ProductId = product.Id.Value,
                        Sku = productKey,
                        Value = ppu,
                        Quantity = quanity
                    });
            }
        }

        #endregion

        #region Command

        [Serializable()]
        public class SyncToZen : ICommand
        {
            public SyncToZen()
            {
                this.Lines = new List<OrderLines>();
            }

            /// <summary>
            /// Gets the public key for the deal.
            /// </summary>
            public Guid PublicKey { get; set; }

            public IList<OrderLines> Lines { get; }

            /// <summary>
            /// The total of the deal, in USD.
            /// </summary>
            /// <remarks>
            /// This may be different than the sum of <see cref="Lines"/> due to order rebates or minimums.
            /// </remarks>
            public Decimal Total { get; set; }
        }

        [Serializable()]
        public class OrderLines
        {
            public String ProductKey { get; set; }

            public String Description { get; set; }

            public Decimal Price { get; set; }

            public Int32 Quantity { get; set; }
        }

        #endregion
    }
}