using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AccurateAppend.Sales.Contracts.ViewModels
{
    /// <summary>
    /// A viewmodel representation of a new <see cref="DealBinder"/> operation.
    /// </summary>
    /// <remarks>
    /// Simplifies the integration of the <see cref="DealBinder"/> edit and display
    /// content with higher layers.
    /// </remarks>
    public class DealModel : IValidatableObject
    {
        #region Properties

        /// <summary>
        /// Gets or sets the identifier of the current model.
        /// </summary>
        public Int32? Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the user to create a deal for with the current model.
        /// </summary>
        [Required()]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the user that will own the deal.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Text, ErrorMessage = "*")]
        public Guid? OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the description value for the current model.
        /// </summary>
        [DataType(DataType.Text, ErrorMessage = "*")]
        [Display(Name = "Description")]
        [Required(AllowEmptyStrings = false)]
        [MaxLength(500)]
        public String Description { get; set; }

        /// <summary>
        /// Gets or sets the title value for the current model.
        /// </summary>
        [DataType(DataType.Text, ErrorMessage = "*")]
        [Display(Name = "Title")]
        [Required(AllowEmptyStrings = false)]
        [MaxLength(150)]
        public String Title { get; set; }

        /// <summary>
        /// Gets or sets the processing instructions value, if any, for the current model.
        /// </summary>
        [DataType(DataType.MultilineText, ErrorMessage = "*")]
        [Display(Name = "Processing Instructions")]
        public String Instructions { get; set; }

        /// <summary>
        /// Gets or sets the amount the current model.
        /// </summary>
        [DataType(DataType.Currency, ErrorMessage = "*")]
        [Display(Name = "Suggested Amount")]
        [Range(0, Int32.MaxValue)]
        public Decimal Amount { get; set; }

        /// <summary>
        /// Indicates whether the deal is allowed for auto billing.
        /// </summary>
        [Display(Name = "Perform Auto Billing?")]
        public Boolean AutoBill { get; set; }

        /// <summary>
        /// Indicates whether the deal should suppress change notifications.
        /// </summary>
        [Display(Name = "Suppress Notifications?")]
        public Boolean SuppressNotifications { get; set; }

        #endregion

        #region IValidatableObject Members

        /// <inheritdoc />
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.OwnerId == null || this.OwnerId == Guid.Empty) yield return new ValidationResult($"{nameof(OwnerId)} is required", new[] {nameof(OwnerId) });
        }

        #endregion
    }
}
