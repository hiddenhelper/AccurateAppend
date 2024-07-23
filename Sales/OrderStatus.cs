using System;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Represents one of the possible states an <see cref="Order"/> can be in.
    /// Some status values are only used by particular concrete <see cref="Order"/>
    /// implementations. See the specific concrete type for understanding the usage.
    /// </summary>
    [Serializable()]
    public enum OrderStatus
    {
        /// <summary>
        /// An order that is not yet billed or completed. e.g. The Order can be mutated.
        /// </summary>
        Open = 0,

        /// <summary>
        /// An order that has been billed (charged or invoiced).
        /// </summary>
        Billed = 1,

        /// <summary>
        /// An order that has been refunded. (eg used with <see cref="RefundOrder"/> when complete)
        /// </summary>
        Refunded = 2,

        /// <summary>
        /// An order that has resulted in being a write off, usually due to extreme low totals.
        /// </summary>
        WriteOff = 3,

        /// <summary>
        /// An order that has identified as using a fraudulent card. (not currently used)
        /// </summary>
        Fraud = 4,

        /// <summary>
        /// An order that is has been canceled.
        /// </summary>
        Canceled = -1
    }
}