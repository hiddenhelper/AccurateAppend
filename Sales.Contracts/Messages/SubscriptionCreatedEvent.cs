using System;
using System.Diagnostics.Contracts;
using AccurateAppend.Core;
using NServiceBus;

namespace AccurateAppend.Sales.Contracts.Messages
{
    /// <summary>
    /// Contains the event data for when a new Subscription has been created.
    /// </summary>
    [Serializable()]
    public class SubscriptionCreatedEvent : IEvent
    {
        private DateTime dateCreated = DateTime.UtcNow;
        private DateTime startingDate;
        private DateTime? endingDate;

        /// <summary>
        /// Gets the identifier of the subscription that was created.
        /// </summary>
        public Int32 SubscriptionId { get; set; }

        /// <summary>
        /// Gets the public key for new Subscription.
        /// </summary>
        public Guid PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the date (UTC) when the Subscription was created.
        /// </summary>
        public DateTime DateCreated
        {
            get
            {
                Contract.Ensures(Contract.Result<DateTime>().Kind == DateTimeKind.Utc);
                return this.dateCreated;
            }
            set
            {
                Contract.Ensures(this.dateCreated.Kind == DateTimeKind.Utc);
                this.dateCreated = value.Coerce();
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
        /// Gets the ending date of the Subscription. A null means no end.
        /// </summary>
        public DateTime? EndingDate
        {
            get { return this.endingDate; }
            set { this.endingDate = value?.ToSafeLocal().ToBillingZone().Date; }
        }
    }
}