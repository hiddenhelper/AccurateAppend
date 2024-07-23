using System;
using System.ComponentModel.DataAnnotations;

namespace AccurateAppend.Websites.Admin.ViewModels.Authentication
{
    public class LogonModel
    {
        [Required()]
        [Display(Name = "User name")]
        public String UserName { get; set; }

        [Required()]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public String Password { get; set; }

        [Display(Name = "Remember me")]
        public Boolean RememberMe { get; set; }

        public String ReturnUrl { get; set; }

        public Int32 Offset { get; set; }
    }
}