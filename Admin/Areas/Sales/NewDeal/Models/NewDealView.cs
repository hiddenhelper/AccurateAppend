using System;

namespace AccurateAppend.Websites.Admin.Areas.Sales.NewDeal.Models
{
    /// <summary>
    /// View model for displaying the details for a new deal
    /// </summary>
    [Serializable()]
    public class NewDealView
    {
        /// <summary>
        /// The user identifier of the user that owns the order.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Post-back after file upload
        /// </summary>
        public Uri PostbackUri { get; set; }
    }
}