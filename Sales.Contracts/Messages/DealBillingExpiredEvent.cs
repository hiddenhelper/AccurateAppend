using System;
using NServiceBus;

namespace AccurateAppend.Sales.Contracts.Messages
{
    /// <summary>
    /// Contains the event data for when a deal has been moved back into editable status.
    /// </summary>
    [Serializable()]
    public class DealBillingExpiredEvent : IEvent
    {
        /// <summary>
        /// Gets the identifier of the deal that had billing expired.
        /// </summary>
        public Int32 DealId { get; set; }

        /// <summary>
        /// Gets the public key for the deal billing.
        /// </summary>
        public Guid PublicKey { get; set; }
        
        /// <summary>The user the deal is for.</summary>
        public Guid Client { get; set; }
    }
}