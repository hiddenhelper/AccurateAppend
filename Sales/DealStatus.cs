using System;
using System.ComponentModel;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Represents one of the possible states a <see cref="DealBinder"/> can be in.
    /// </summary>
    /// <remarks>
    /// A deal has a set life cycle:
    /// When created, they will be InProcess. A Deal will either transition to Canceled
    /// or to Approval. If Approved, the deal will move the Billing status; Otherwise
    /// when Declined it will return to the InProcess state. When the deal total is
    /// completely billed it will move to the final Completed state. 
    /// 
    /// -Only Completed deals can begin a refund process.
    /// -Canceled deals become effectively invisible to the system and read-only
    /// </remarks>
    [Serializable()]
    public enum DealStatus
    {
        /// <summary>
        /// A deal that is has been canceled.
        /// </summary>
        Canceled = -1,

        /// <summary>
        /// A deal that is new and open for modification.
        /// </summary>
        [Description("In Process")]
        InProcess = 3,

        /// <summary>
        /// A deal that is locked and currently awaiting quality control review.
        /// </summary>
        [Description("Ready For Approval")]
        Approval = 8,

        /// <summary>
        /// A deal that has been approved and is to be charged against a client credit card.
        /// </summary>
        [Description("Ready to bill")]
        Billing = 4,

        /// <summary>
        /// A deal that has been charged or invoiced.
        /// </summary>
        [Description("Complete")]
        Complete = 5
    }
}
