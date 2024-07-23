using System;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Csv.Messages
{
    /// <summary>
    /// Contains the event data describing the results of a <see cref="CreateCsvCartCommand"/>.
    /// </summary>
    [Serializable()]
    public class CsvCartCreatedEvent : IEvent
    {
        /// <summary>
        /// The identity of the account that requested the cart.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// The identity of the account that the cart is for.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The identifier of the shopping cart.
        /// </summary>
        public Guid CartId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the assigned sales representative.
        /// </summary>
        public Guid SalesRep { get; set; }
    }
}