using System;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Csv.Messages
{
    /// <summary>
    /// Command used to submit an order.
    /// </summary>
    [Serializable()]
    public class SubmitCsvOrderCommand : ICommand
    {
        /// <summary>
        /// The identifier of the cart the to submit the product order for.
        /// </summary>
        public Guid CartId { get; set; }
    }
}