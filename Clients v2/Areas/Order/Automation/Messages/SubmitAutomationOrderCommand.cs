using System;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Automation.Messages
{
    /// <summary>
    /// Command used to submit an order.
    /// </summary>
    [Serializable()]
    public class SubmitAutomationOrderCommand : ICommand
    {
        /// <summary>
        /// The identifier of the cart the to submit the product order for.
        /// </summary>
        public Guid CartId { get; set; }
    }
}