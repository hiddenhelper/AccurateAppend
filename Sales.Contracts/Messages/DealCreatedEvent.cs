using System;
using System.Diagnostics.Contracts;
using AccurateAppend.Core;
using NServiceBus;

namespace AccurateAppend.Sales.Contracts.Messages
{
    /// <summary>
    /// Contains the event data for when a new deal and order have been created.
    /// </summary>
    [Serializable()]
    public class DealCreatedEvent : IEvent
    {
        private DateTime dateCreated = DateTime.UtcNow;

        /// <summary>
        /// Gets the identifier of the deal that was created.
        /// </summary>
        public Int32 DealId { get; set; }

        /// <summary>
        /// Gets the public key for the deal billing.
        /// </summary>
        public Guid PublicKey { get; set; }

        /// <summary>The total amount of the deal.</summary>
        public Decimal Amount { get; set; }

        /// <summary>The user the deal is for.</summary>
        public Guid Client { get; set; }

        /// <summary>
        /// Gets or sets the date (UTC) when the deal was created.
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
    }
}
