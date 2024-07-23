using System;
using System.Collections.Generic;
using System.Diagnostics;
using AccurateAppend.Core.Definitions;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Automation.Messages
{
    /// <summary>
    /// Contains the event data describing the results of a placed order <see cref="SubmitAutomationOrderCommand"/>.
    /// </summary>
    [DebuggerDisplay("{Cart:" + nameof(CartId) + "}")]
    [Serializable()]
    public class AutomationOrderPlacedEvent : IEvent
    {
        #region Fields

        private ICollection<PublicProduct> products;

        #endregion

        #region Properties

        /// <summary>
        /// The identifier of the shopping cart.
        /// </summary>
        public Guid CartId { get; set; }
        
        /// <summary>
        /// The identifier of the user that submitted the order.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Contains the total estimated in the quote.
        /// </summary>
        public Decimal QuotedTotal { get; set; }

        #endregion
    }
}