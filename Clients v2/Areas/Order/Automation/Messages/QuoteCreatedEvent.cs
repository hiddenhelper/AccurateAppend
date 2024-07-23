using System;
using System.Xml.Linq;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Automation.Messages
{
    /// <summary>
    /// Event when quote for an Automation order has been presented.
    /// </summary>
    [Serializable()]
    public class QuoteCreatedEvent : IEvent
    {
        /// <summary>
        /// The identifier of the cart the file the quote is for.
        /// </summary>
        public Guid CartId { get; set; }

        /// <summary>
        /// Contains the total estimated in the quote.
        /// </summary>
        public Decimal QuotedTotal { get; set; }

        /// <summary>
        /// The quote for the cart.
        /// </summary>
        /// <remarks>Uses the <see cref="Sales.CartQuote"/> syntax.</remarks>
        public XElement Quote { get; set; }
    }
}