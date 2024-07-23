using System;
using System.Diagnostics;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Messages.Accounts
{
    /// <summary>
    /// Contains the data used by the <see cref="OverageBillingSaga"/> to operate.
    /// </summary>
    [DebuggerDisplay("Usage: {" + nameof(SubscriptionId) + "}")]
    [Serializable()]
    public class OverageBillingData : ContainSagaData
    {
        /// <summary>
        /// The identifier of the current subscription the saga is for.
        /// </summary>
        public virtual Int32 SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the alternate identifier of the the subscription the saga is for.
        /// </summary>
        /// <remarks>
        /// Required as the saga &lt;=&gt; correlation needs to work with a non-database
        /// generated key value. This allows the initial request to be built from the caller
        /// and then is stored thereafter.
        /// </remarks>
        public virtual Guid PublicKey { get; set; }
    }
}