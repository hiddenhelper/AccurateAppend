using System;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.Order.Messages
{
    /// <summary>
    /// Event when a NationBuilder list for a <see cref="Sales.Cart"/> has been selected.
    /// </summary>
    [Serializable()]
    public class ListSelectedEvent : IEvent
    {
        /// <summary>
        /// The identifier of the cart the file was uploaded for.
        /// </summary>
        public Guid CartId { get; set; }

        /// <summary>
        /// The identity of the account that the cart is for.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The customer provided name of the list.
        /// </summary>
        public String ListName { get; set; }

        /// <summary>
        /// Gets the number of records in the list.
        /// </summary>
        public Int64 RecordCount { get; set; }
    }
}