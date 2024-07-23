using System;
using System.Diagnostics.Contracts;
using AccurateAppend.Core;
using NServiceBus;

namespace AccurateAppend.Sales.Contracts.Messages
{
    /// <summary>
    /// Contains the event data for when an existing Subscription has been canceled.
    /// </summary>
    [Serializable()]
    public class SubscriptionCanceledEvent : IEvent
    {
        private DateTime dateCanceled = DateTime.UtcNow;
        private DateTime startingDate;
        private DateTime endingDate;

        /// <summary>
        /// Gets the identifier of the subscription that was canceled.
        /// </summary>
        public Int32 SubscriptionId { get; set; }

        /// <summary>
        /// Gets the public key for the Subscription.
        /// </summary>
        public Guid PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the date (UTC) when the Subscription was canceled.
        /// </summary>
        public DateTime DateCanceled
        {
            get
            {
                Contract.Ensures(Contract.Result<DateTime>().Kind == DateTimeKind.Utc);
                return this.dateCanceled;
            }
            set
            {
                Contract.Ensures(this.dateCanceled.Kind == DateTimeKind.Utc);
                this.dateCanceled = value.Coerce();
            }
        }

        /// <summary>
        /// Gets the starting date of the Subscription.
        /// </summary>
        public DateTime StartingDate
        {
            get { return this.startingDate; }
            set { this.startingDate = value.ToSafeLocal().ToBillingZone().Date; }
        }

        /// <summary>
        /// Gets the ending date of the Subscription.
        /// </summary>
        /// <remarks>
        /// Unlike the <see cref="SubscriptionCreatedEvent"/>, this value is never null.
        /// </remarks>
        public DateTime EndingDate
        {
            get { return this.endingDate; }
            set { this.endingDate = value.ToSafeLocal().ToBillingZone().Date; }
        }
    }
}
