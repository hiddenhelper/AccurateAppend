using System;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Indicates the possible states a <see cref="ProductOrder"/> can be in.
    /// </summary>
    [Serializable()]
    public enum ProcessingStatus
    {
        /// <summary>
        /// The PO was canceled.
        /// </summary>
        Canceled = -1,

        /// <summary>
        /// The order has been submitted.
        /// </summary>
        Accepted = 0,

        /// <summary>
        /// The order is being actively appended.
        /// </summary>
        Processing = 1,

        /// <summary>
        /// The order is waiting on billing.
        /// </summary>
        Billing = 2,

        /// <summary>
        /// The order is fulfilled and available to the customer.
        /// </summary>
        Available = 3
    }
}