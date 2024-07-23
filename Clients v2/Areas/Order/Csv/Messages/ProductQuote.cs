using System;
using System.Diagnostics;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.Websites.Clients.Areas.Order.Csv.Messages
{
    /// <summary>
    /// Contains the selected products that were quoted.
    /// </summary>
    [Serializable()]
    [DebuggerDisplay("{" + nameof(Product) + "}")]
    public class ProductQuote
    {
        /// <summary>
        /// The <see cref="PublicProduct"/> that was selected in the order.
        /// </summary>
        public PublicProduct Product { get; set; }

        /// <summary>
        /// The estimated number of matches that were quoted.
        /// </summary>
        public Int32 EstimatedMatches { get; set; }

        /// <summary>
        /// The estimated price per match that was estimated based on probable matches.
        /// </summary>
        public Decimal QuotedRate { get; set; }
    }
}