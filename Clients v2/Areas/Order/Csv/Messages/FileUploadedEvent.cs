using System;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Csv.Messages
{
    /// <summary>
    /// Event when a CSV file for a <see cref="Sales.Cart"/> has been uploaded.
    /// </summary>
    [Serializable()]
    public class FileUploadedEvent : IEvent
    {
        /// <summary>
        /// The identifier of the cart the file was uploaded for.
        /// </summary>
        public Guid CartId { get; set; }

        /// <summary>
        /// The identity of the account that the cart is for.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The customer provided name of the uploaded file.
        /// </summary>
        public String CustomerFileName { get; set; }

        /// <summary>
        /// Gets the number of records in the list.
        /// </summary>
        public Int64 RecordCount { get; set; }
    }
}