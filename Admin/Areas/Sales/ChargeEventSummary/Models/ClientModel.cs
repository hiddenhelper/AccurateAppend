using System;

namespace AccurateAppend.Websites.Admin.Areas.Sales.ChargeEventSummary.Models
{
    /// <summary>
    /// View model containing the UI filters, if any, for the charge event summary view.
    /// </summary>
    [Serializable()]
    public class ClientModel
    {
        /// <summary>
        /// The email address to filter charge events to.
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// The user identifier of the user to filter charge events to.
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// The deal identifier to filter charge events to.
        /// </summary>
        /// <remarks>
        /// While charge events are tied to a single order, it is generally needed to be able to view all
        /// the entries by the entire deal. This allows charges to be shown alongside refunds.
        /// </remarks>
        public Int32? DealId { get; set; }
    }
}