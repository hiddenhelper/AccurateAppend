using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AccurateAppend.Websites.Clients.Areas.Profile.Password.Models
{
    /// <summary>
    /// View model for updating a password to a new one.
    /// </summary>
    [Serializable()]
    public class PasswordDetailsModel : IValidatableObject
    {
        #region Properties

        /// <summary>
        /// Gets or sets the current password. This is used to add security by requiring the change
        /// to only users with the current password.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public String OldPassword { get; set; }

        /// <summary>
        /// Gets or sets the new password.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public String NewPassword { get; set; }

        /// <summary>
        /// Gets or sets a copy of the new password value to prevent typos during update.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare(nameof(NewPassword), ErrorMessage = "The new password and confirmation password do not match.")]
        public String ConfirmPassword { get; set; }

        #endregion

        #region  IValidatableObject Members

        /// <inheritdoc />
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.NewPassword != this.ConfirmPassword)
            {
                yield return new ValidationResult("New password and confirmation password does not match", new[] {nameof(NewPassword), nameof(ConfirmPassword)});
            }
        }

        #endregion
    }
}