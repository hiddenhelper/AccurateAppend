using System;
using AccurateAppend.Core;
using NServiceBus;

namespace AccurateAppend.Sales.Contracts.Messages
{
    /// <summary>
    /// Command to instruct an accrual account to close on the indicated date.
    /// </summary>
    [Serializable()]
    public class CancelAccrualCommand : ICommand
    {
        #region Fields

        private DateTime endDate;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the public key of the subscription to cancel.
        /// </summary>
        public Guid PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the date the subscription ends.
        /// </summary>
        public DateTime EndDate
        {
            get { return this.endDate; }
            set { this.endDate = value.ToSafeLocal().ToBillingZone().Date; }
        }

        #endregion
    }
}
