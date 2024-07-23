using System;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Csv.Messages
{
    /// <summary>
    /// Command used to submit an order.
    /// </summary>
    [Serializable()]
    public class ColumnMapCsvOrderCommand : ICommand
    {
        /// <summary>
        /// The identifier of the cart the to submit the product order for.
        /// </summary>
        public Guid CartId { get; set; }

        /// <summary>
        /// Denotes if first row in file is a header row.
        /// </summary>
        public Boolean HasHeaderRow { get; set; }

        /// <summary>
        /// Contains the column map for the file that was ordered.
        /// </summary>
        public String ColumnMap { get; set; }
    }
}