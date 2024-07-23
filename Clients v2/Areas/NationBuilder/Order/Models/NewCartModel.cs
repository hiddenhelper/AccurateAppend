using System;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.Order.Models
{
    [Serializable()]
    public class NewCartModel
    {
        /// <summary>
        /// A unique value that represents the order, start to finish.
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// The url that should be used to check for cart ready.
        /// </summary>
        public String CheckUrl { get; set; }

        /// <summary>
        /// The url that should be used to navigate when the cart is ready.
        /// </summary>
        public String NextUrl { get; set; }
    }
}