using System;
using NServiceBus;

namespace AccurateAppend.Sales.Contracts.Messages
{
    /// <summary>
    /// Contains the event data for when a deal public key has changed.
    /// </summary>
    [Serializable()]
    public class DealPublicKeyChangedEvent : IEvent
    {
        /// <summary>
        /// Gets the identifier of the deal that was changed.
        /// </summary>
        public Int32 DealId { get; set; }

        /// <summary>
        /// Gets the previous public key for the deal billing.
        /// </summary>
        public Guid OriginalPublicKey { get; set; }

        /// <summary>
        /// Gets the current public key for the deal billing.
        /// </summary>
        public Guid NewPublicKey { get; set; }

        /// <summary>The user the deal is for.</summary>
        public Guid Client { get; set; }
    }
}