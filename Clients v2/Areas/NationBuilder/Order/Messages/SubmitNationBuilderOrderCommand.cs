using System;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.Order.Messages
{
    /// <summary>
    /// Command used to submit an order.
    /// </summary>
    [Serializable()]
    public class SubmitNationBuilderOrderCommand : ICommand
    {
        /// <summary>
        /// The identifier of the cart the to submit the product order for.
        /// </summary>
        public Guid CartId { get; set; }
    }
}