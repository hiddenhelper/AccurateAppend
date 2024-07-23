using System;
using System.ComponentModel.DataAnnotations;

namespace AccurateAppend.Websites.Clients.Areas.Authentication.ResetPassword.Models
{
    public class LocalPasswordModel
    {
        [Required()]
        public Guid PublicKey { get; set; }

        public Boolean IsValid { get; set; }

        [Required()]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public String NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare(nameof(NewPassword), ErrorMessage = "The new password and confirmation password do not match.")]
        public String ConfirmPassword { get; set; }
    }
}