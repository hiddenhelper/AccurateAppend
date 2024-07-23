using System;
using System.ComponentModel.DataAnnotations;

namespace AccurateAppend.Websites.Clients.Areas.Authentication.NewPassword.Models
{
    /// <summary>
    /// ViewModel for a user required to update their password after logon.
    /// </summary>
    public class ForcedPasswordChangeModel
    {
        public ForcedPasswordChangeModel()
        {
        }

        public ForcedPasswordChangeModel(String redirectTo)
        {
            this.RedirectTo = redirectTo;
        }

        public String RedirectTo { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public String NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public String ConfirmPassword { get; set; }
    }
}