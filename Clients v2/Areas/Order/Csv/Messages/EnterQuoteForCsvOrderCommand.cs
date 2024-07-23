using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Csv.Messages
{
    /// <summary>
    /// Command used to enter the select product quote for a product order.
    /// </summary>
    [Serializable()]
    public class EnterQuoteForCsvOrderCommand : ICommand
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

        /// <summary>
        /// The Manifest XML content that has been selected for the order. This manifest is expected
        /// to be fully populated with metadata attributes (@UserId, @ManifestId, etc)
        /// </summary>
        public XElement Manifest { get; set; }
    }
}