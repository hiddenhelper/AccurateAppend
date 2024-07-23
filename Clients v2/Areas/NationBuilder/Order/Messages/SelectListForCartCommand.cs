using System;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.Order.Messages
{
    /// <summary>
    /// Command to add NationBuilder list information to a <see cref="Sales.Cart"/>.
    /// </summary>
    [Serializable()]
    public class SelectListForCartCommand : ICommand
    {
        /// <summary>
        /// The identifier of the new cart to create.
        /// </summary>
        public Guid CartId { get; set; }

        /// <summary>
        /// The remote identifier of the list in the Nation.
        /// </summary>
        public Int32 ListId { get; set; }

        /// <summary>
        /// The list name in the Nation for the cart.
        /// </summary>
        public String ListName { get; set; }

        /// <summary>
        /// The internal identifier of the Nation integration for the cart.
        /// </summary>
        public Int32 RegistrationId { get; set; }

        /// <summary>
        /// The number of contacts in the NationBuilder list.
        /// </summary>
        public Int32 RecordCount { get; set; }
    }
}