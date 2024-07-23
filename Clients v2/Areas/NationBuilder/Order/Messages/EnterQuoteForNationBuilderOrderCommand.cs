using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.Order.Messages
{
    /// <summary>
    /// Command used to enter the select product quote for a product order.
    /// </summary>
    [Serializable()]
    public class EnterQuoteForNationBuilderOrderCommand : ICommand
    {
        private ICollection<ProductQuote> products;

        /// <summary>
        /// The identifier of the cart the to submit the product order for.
        /// </summary>
        public Guid CartId { get; set; }

        /// <summary>
        /// The set of products to submit the order for.
        /// </summary>
        public ICollection<ProductQuote> Products
        {
            get
            {
                if (this.products == null) this.products = new Collection<ProductQuote>();
                return this.products;
            }
        }

        /// <summary>
        /// The order minimum value, if any.
        /// </summary>
        public Decimal? OrderMinimum { get; set; }
    }
}