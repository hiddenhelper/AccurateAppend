using System;
using AccurateAppend.Sales;
using DomainModel.Enum;

namespace AccurateAppend.Websites.Admin.Areas.Sales.DealSummary.Models
{
    /// <summary>
    /// View model containing the UI filters, if any, for the deal summary view.
    /// </summary>
    [Serializable()]
    public class DealSummaryViewModel
    {
        /// <summary>
        /// The user identifier of the user to filter deals to.
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// The status to filter deals to.
        /// </summary>
        public DealStatus? Status { get; set; }

        /// <summary>
        /// The email address to filter deals to.
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// The date range, inclusive, to filter deals to.
        /// </summary>
        public DateRange? DateRange { get; set; }

        /// <summary>
        /// The identifier of the application to filter deals to.
        /// </summary>
        public Guid? ApplicationId { get; set; }
    }
}