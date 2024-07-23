using System;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Automation.Messages
{
    /// <summary>
    /// Command to instruct a new <see cref="Sales.Cart"/> to be created for the specific user.
    /// </summary>
    [Serializable()]
    public class CreateAutomationCartCommand : ICommand
    {
        /// <summary>
        /// The identifier of the user to create the cart for.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The identifier of the new cart to create.
        /// </summary>
        public Guid CartId { get; set; }
    }
}