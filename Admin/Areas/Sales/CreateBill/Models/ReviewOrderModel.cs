using System;
using System.ComponentModel.DataAnnotations;

namespace AccurateAppend.Websites.Admin.Areas.Sales.CreateBill.Models
{
    /// <summary>
    /// Step 1 in the bill creation process to review the order information and select a billing type.
    /// </summary>
    [Serializable()]
    public class ReviewOrderModel
    {
        /// <summary>
        /// The identifier of the order in the deal the bill is for.
        /// </summary>
        [Required()]
        [RequiredGreaterThanZero()]
        public Int32 OrderId { get; set; }

        /// <summary>
        /// The identifier of the user the deal is for.
        /// </summary>
        [Required()]
        [RequiredNotEmptyGuid()]
        public Guid UserId { get; set; }

        /// <summary>
        /// The identifier of the deal for the bill.
        /// </summary>
        [Required()]
        [RequiredGreaterThanZero()]
        public Int32 DealId { get; set; }

        /// <summary>
        /// Gets or sets the billing type for the bill to create.
        /// </summary>
        [Display(Name = "Payment Type")]
        [Required()]
        public BillType BillType { get; set; }
    }
}