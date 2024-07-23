using System;

namespace AccurateAppend.Websites.Clients.Areas.Order.Box.Models
{
    /// <summary>
    /// Model for displaying a list selection display from Box.com integration.
    /// </summary>
    [Serializable()]
    public class SelectListModel
    {
        /// <summary>
        /// Gets or sets the uri to acquire the Box.com api data.
        /// </summary>
        public Uri DataUrl { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the current cart.
        /// </summary>
        public Guid CartId { get; set; }
    }
}