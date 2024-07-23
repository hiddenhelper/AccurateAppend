using System;
using System.ComponentModel.DataAnnotations;

namespace AccurateAppend.Websites.Clients.Areas.Authentication.Shared.Models
{
    /// <summary>
    /// ViewModel to interact with an AccurateAppend account logon process.
    /// </summary>
    public abstract class LoginModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginModel"/> class.
        /// </summary>
        protected LoginModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginModel"/> class with the supplied <paramref name="redirectTo"/> value.
        /// </summary>
        /// <param name="redirectTo">The URL that the user should be redirected to after logon.</param>
        protected LoginModel(String redirectTo)
        {
            this.RedirectTo = redirectTo;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the URL that the user should be redirected to after successful logon.
        /// </summary>
        /// <remarks>Null or empty values indicate that the default action for the site should be leveragted.</remarks>
        public String RedirectTo { get; set; }

        /// <summary>
        /// Gets or sets the user name value.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public String UserName { get; set; }

        /// <summary>
        /// Gets or sets the password value.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public String Password { get; set; }

        /// <summary>
        /// Indicates whether the user would like a persistent logon.
        /// </summary>
        [Display(Name = "Remember me?")]
        public Boolean RememberMe { get; set; }

        /// <summary>
        /// Indicates whether external partner login links should be displayed
        /// </summary>
        public Boolean AllowExternalLogin { get; set; }

        #endregion
    }
}