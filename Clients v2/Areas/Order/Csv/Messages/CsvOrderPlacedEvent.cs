using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Xml.Linq;
using AccurateAppend.Core.Definitions;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Csv.Messages
{
    /// <summary>
    /// Contains the event data describing the results of a placed order <see cref="SubmitCsvOrderCommand"/>.
    /// </summary>
    [DebuggerDisplay("{Cart:" + nameof(CartId) + "}")]
    [Serializable()]
    public class CsvOrderPlacedEvent : IEvent
    {
        #region Fields

        private ICollection<PublicProduct> products;

        #endregion

        #region Properties

        /// <summary>
        /// The identifier of the shopping cart.
        /// </summary>
        public Guid CartId { get; set; }

        /// <summary>
        /// The set of products to ordered.
        /// </summary>
        public ICollection<PublicProduct> Products
        {
            get
            {
                if (this.products == null) this.products = new Collection<PublicProduct>();
                return this.products;
            }
        }

        /// <summary>
        /// The identifier of the user that submitted the order.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Denotes if first row in file is a header row.
        /// </summary>
        public Boolean HasHeaderRow { get; set; }

        /// <summary>
        /// Contains the column map for the file that was ordered.
        /// </summary>
        public XElement ColumnMap { get; set; }

        /// <summary>
        /// Contains the total estimated in the quote.
        /// </summary>
        public Decimal QuotedTotal { get; set; }

        /// <summary>
        /// Contains the delimiter of the system file that is stored as part of the order.
        /// </summary>
        public virtual Char Delimiter { get; set; }

        #endregion
    }
}