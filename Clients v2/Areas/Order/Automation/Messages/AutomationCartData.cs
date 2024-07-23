using System;
using System.Diagnostics;
using AccurateAppend.Websites.Clients.Areas.Order.Csv.Messages;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Automation.Messages
{
    /// <summary>
    /// Contains the data used by the <see cref="CsvSalesSaga"/> to operate.
    /// </summary>
    [DebuggerDisplay("Cart:{" + nameof(CartId) + "}")]
    [Serializable()]
    public class AutomationCartData : ContainSagaData
    {
        /// <summary>
        /// The identifier of the user the order saga is for.
        /// </summary>
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// The identifier of the cart the order saga is for.
        /// </summary>
        public virtual Guid CartId { get; set; }
    }
}