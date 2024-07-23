using System;
using System.Diagnostics;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.Order.Messages
{
    /// <summary>
    /// Contains the data used by the <see cref="NationBuilderSalesSaga"/> to operate.
    /// </summary>
    [DebuggerDisplay("Cart:{" + nameof(CartId) + "}")]
    [Serializable()]
    public class NationBuilderCartData : ContainSagaData
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