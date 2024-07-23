using System;
using NServiceBus;

namespace AccurateAppend.Sales.Contracts.Messages
{
    /// <summary>
    /// Contains the event data for when a deal has been permanently canceled.
    /// </summary>
    [Serializable()]
    public class DealCanceledEvent : IEvent
    {
        /// <summary>
        /// Gets the identifier of the deal that was canceled.
        /// </summary>
        public Int32 DealId { get; set; }

        /// <summary>
        /// Gets the public key for the deal billing.
        /// </summary>
        public Guid PublicKey { get; set; }

        /// <summary>
        /// Gets the public key for the automated billing account the deal is for, if any.
        /// </summary>
        public Guid BillingAccount { get; set; }

        /// <summary>The user the deal is for.</summary>
        public Guid Client { get; set; }
    }
}