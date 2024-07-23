using System;
using AccurateAppend.Sales;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Messages.Accounts
{
    /// <summary>
    /// Message used to indicate a Billing Saga has a period that should be billed.
    /// </summary>
    /// <remarks>
    /// This timeout message state is an internal implementation detail for the sagas therefore is not part of the general published contracts.
    /// </remarks>
    [Serializable()]
    public class BillingPeriodClosedTimeout : IMessage
    {
        /// <summary>
        /// Gets or sets the <see cref="BillingPeriod"/> that should be billed.
        /// </summary>
        public BillingPeriod Period
        {
            get;
            set;
        }
    }
}