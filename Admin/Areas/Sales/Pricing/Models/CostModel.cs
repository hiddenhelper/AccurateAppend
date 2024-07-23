using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace AccurateAppend.Websites.Admin.Areas.Sales.Pricing.Models
{
    /// <summary>
    /// A view model for a rate card ranged cost.
    /// </summary>
    [DebuggerDisplay("{" + nameof(Floor) + ("}-{" + nameof(Ceiling) + "}"))]
    public class CostModel
    {
        /// <summary>
        /// The lower numeric value of the lower range.
        /// </summary>
        /// <remarks>
        /// Always should be less than or equal to <see cref="Ceiling"/>.
        /// </remarks>
        [Required()]
        [Range(0, Int32.MaxValue)]
        [Display(Name = "From")]
        public Int32 Floor { get; set; }

        /// <summary>
        /// The lower numeric value of the upper range.
        /// </summary>
        /// <remarks>
        /// Always should be greater than or equal to <see cref="Floor"/>.
        /// </remarks>
        [Required()]
        [Range(0, Int32.MaxValue)]
        [Display(Name = "To")]
        public Int32 Ceiling { get; set; }

        /// <summary>
        /// The price per-record.
        /// </summary>
        [Required()]
        [Display(Name = "Per Record")]
        public Decimal PerRecord { get; set; }

        /// <summary>
        /// The price per-match.
        /// </summary>
        [Required()]
        [Display(Name = "Per Match")]
        public Decimal PerMatch { get; set; }

        /// <summary>
        /// The minimum price, if any, a deal should have if present.
        /// </summary>
        [Display(Name = "Minimum (optional)?")]
        public Decimal? FileMinimum { get; set; }
    }
}