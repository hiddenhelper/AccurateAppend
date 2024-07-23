using System;

namespace AccurateAppend.Websites.Clients.Areas.Order.Shared.Models
{
    public class ProcessFileModel
    {
        /// <summary>
        /// The identifier of the cart the file is for.
        /// </summary>
        public Guid CartId { get; set; }
        
        /// <summary>
        /// Contains the uri that when file processing is complete (<see cref="SubmitUrl"/>,
        /// the client should be redirected to.
        /// </summary>
        public String NextUrl { get; set; }

        /// <summary>
        /// Contains the uri that should be run when the process file routine starts. 
        /// </summary>
        public String SubmitUrl { get; set; }

        /// <summary>
        /// The amount of time the file processing should be allowed prior to
        /// continuation of the workflow in <see cref="NextUrl"/>.
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
    }
}