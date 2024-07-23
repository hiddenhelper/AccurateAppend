using System;

namespace AccurateAppend.Websites.Admin.Areas.Sales.OrderDetail.Models
{
    /// <summary>
    /// View model for displaying the details for a single order.
    /// </summary>
    [Serializable()]
    public class OrderDetailView
    {
        /// <summary>
        /// The identifier of the order to view.
        /// </summary>
        public Int32 OrderId { get; set; }

        /// <summary>
        /// The user identifier of the user that owns the order.
        /// </summary>
        public Guid UserId { get; set; }
    }
}