using System;

namespace AccurateAppend.Websites.Clients.Areas.Order.Automation.Models
{
    /// <summary>
    /// Model for displaying a user's automation.
    /// </summary>
    [Serializable()]
    public class DisplayAutomationsModel
    {
        /// <summary>
        /// Gets or sets the identifier of the current cart.
        /// </summary>
        public Guid CartId { get; set; }

        #region Urls

        /// <summary>
        /// Gets or sets the url that should be used to query the user's automation definitions.
        /// </summary>
        public String QueryUrl { get; set; }

        /// <summary>
        /// The url that should be used to send the selected manifest to.
        /// </summary>
        public String SelectUrl { get; set; }

        /// <summary>
        /// The url that should be used to navigate when the cart is ready.
        /// </summary>
        public String NextUrl { get; set; }

        #endregion
    }
}