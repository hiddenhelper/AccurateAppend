using System;
using NServiceBus;

namespace AccurateAppend.Sales.Contracts.Messages
{
    /// <summary>
    /// Contains the event data for when a deal has been partially or completely refunded.
    /// </summary>
    [Serializable()]
    public class DealRefundedEvent : IEvent
    {
        /// <summary>
        /// Gets the identifier of the deal that was refunded.
        /// </summary>
        public Int32 DealId { get; set; }

        /// <summary>
        /// Gets the public key for the deal billing.
        /// </summary>
        public Guid PublicKey { get; set; }

        /// <summary>The remaining total amount of the deal.</summary>
        public Decimal Amount { get; set; }
        
        /// <summary>The user the deal is for.</summary>
        public Guid Client { get; set; }
    }
}