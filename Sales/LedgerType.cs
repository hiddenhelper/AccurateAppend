using System;
using System.ComponentModel;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Categorizes <see cref="LedgerEntry"/> into topical groups.
    /// </summary>
    [Serializable()]
    public enum LedgerType
    {
        /// <summary>
        /// The <see cref="LedgerEntry"/> is for <see cref="SubscriptionBilling"/> billing.
        /// </summary>
        [Description("Subscription")]
        ForSubscriptionPeriod = 0,

        /// <summary>
        /// The <seealso cref="LedgerEntry"/> is for usage billing.
        /// </summary>
        [Description("Usage")]
        ForUsage = 1,
        /// <summary>
        /// The <seealso cref="LedgerEntry"/> is part of a general adjust entry.
        /// </summary>
        [Description("Adjustment")]
        GeneralAdjustment = -1
    }
}
