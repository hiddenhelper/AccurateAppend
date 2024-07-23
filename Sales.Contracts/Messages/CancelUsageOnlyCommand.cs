using System;
using AccurateAppend.Core;
using NServiceBus;

namespace AccurateAppend.Sales.Contracts.Messages
{
    /// <summary>
    /// Command to instruct a usage only account to close on the indicated date.
    /// </summary>
    [Serializable()]
    public class CancelUsageOnlyCommand : ICommand
    {
        #region Fields

        private DateTime endDate;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the public key of the usage only account to cancel.
        /// </summary>
        public Guid PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the date the usage only account ends.
        /// </summary>
        public DateTime EndDate
        {
            get { return this.endDate; }
            set { this.endDate = value.ToSafeLocal().Date; }
        }

        #endregion
    }
}