using System;
using System.Diagnostics;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Messages.Csv
{
    /// <summary>
    /// Contains the data used by the <see cref="CsvOrderProcessingSaga"/> to operate.
    /// </summary>
    [DebuggerDisplay("Order:{" + nameof(OrderId) + "}")]
    [Serializable()]
    public class CsvOrderData : ContainSagaData
    {
        /// <summary>
        /// The identifier of the order saga is for.
        /// </summary>
        public virtual Guid OrderId { get; set; }

        /// <summary>
        /// The identifier of the job the saga is waiting for.
        /// </summary>
        public virtual Int32? JobId { get; set; }

        /// <summary>
        /// The order minimum, if any.
        /// </summary>
        public virtual Decimal? OrderMinimum { get; set; }
    }
}