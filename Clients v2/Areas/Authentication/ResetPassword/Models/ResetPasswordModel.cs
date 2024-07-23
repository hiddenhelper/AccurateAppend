using System;
using System.ComponentModel.DataAnnotations;

namespace AccurateAppend.Websites.Clients.Areas.Authentication.ResetPassword.Models
{
    /// <summary>
    /// ViewModel to interact with password reset
    /// </summary>
    public class ResetPasswordModel
    {
        #region Properties 

        /// <summary>
        /// Gets or sets the user name value.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Valid email address is required.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Valid email address is required.")]
        [Display(Name = "Email Address")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Your email address is not correctly formatted.")]
        [MaxLength(250)]
        public String UserName { get; set; }

        #endregion
    }
}