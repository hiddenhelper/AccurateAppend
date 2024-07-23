using System;
using System.Diagnostics;
using System.Xml.Linq;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Csv.Messages
{
    /// <summary>
    /// Contains the data used by the <see cref="CsvSalesSaga"/> to operate.
    /// </summary>
    [DebuggerDisplay("Cart:{" + nameof(CartId) + "}")]
    [Serializable()]
    public class CsvCartData : ContainSagaData
    {
        /// <summary>
        /// The identifier of the user the order saga is for.
        /// </summary>
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// The identifier of the cart the order saga is for.
        /// </summary>
        public virtual Guid CartId { get; set; }

        /// <summary>
        /// Denotes if first row in file is a header row.
        /// </summary>
        public virtual Boolean HasHeaderRow { get; set; }

        /// <summary>
        /// Contains the column map for the file that was ordered.
        /// </summary>
        public virtual XDocument ColumnMap { get; set; }

        /// <summary>
        /// Contains the delimiter of the system file that is stored as part of the order.
        /// </summary>
        public virtual Char? Delimiter { get; set; }
    }
}