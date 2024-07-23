using System;
using System.ComponentModel.DataAnnotations;

namespace AccurateAppend.Websites.Clients.Areas.Profile.SocialMediaLogin.Models
{
    [Serializable()]
    public class InviteUserModel
    {
        /// <summary>
        /// Gets or sets the email address to invite.
        /// </summary>
        [Required(ErrorMessage = "Valid email address is required.", AllowEmptyStrings = false)]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Valid email address is required.")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Your email address is not correctly formatted.")]
        [MaxLength(250)]
        public String EmailAddress { get; set; }
    }
}