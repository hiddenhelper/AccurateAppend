using System;
using System.Diagnostics;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Csv.Messages
{
    /// <summary>
    /// Contains the event data describing the results of an order that has been lapsed and abandoned.
    /// </summary>
    [DebuggerDisplay("{Cart:" + nameof(CartId) + "}")]
    [Serializable()]
    public class CsvOrderExpiredEvent : IEvent
    {
        #region Properties

        /// <summary>
        /// The identifier of the shopping cart.
        /// </summary>
        public Guid CartId { get; set; }

        /// <summary>
        /// The identifier of the user that submitted the order.
        /// </summary>
        public Guid UserId { get; set; }
        
        #endregion
    }
}