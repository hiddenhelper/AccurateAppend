using System;

namespace AccurateAppend.Websites.Clients.Areas.Order.Box.Models
{
    /// <summary>
    /// Model used for file processing during Box.com based file downloads.
    /// </summary>
    [Serializable()]
    public class ProcessFileModel
    {
        /// <summary>
        /// The identifier of the cart the file is for.
        /// </summary>
        public Guid CartId { get; set; }

        /// <summary>
        /// Contains the uri that when file processing is complete (<see cref="CheckUrl"/>,
        /// the client should be redirected to.
        /// </summary>
        public String NextUrl { get; set; }

        /// <summary>
        /// Contains the uri that should be run when the process file routine starts. 
        /// </summary>
        public String CheckUrl { get; set; }
    }
}