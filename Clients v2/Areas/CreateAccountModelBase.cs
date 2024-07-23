using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using AccurateAppend.Accounting;
using AccurateAppend.Security;

namespace AccurateAppend.Websites.Clients.Areas
{
    /// <summary>
    /// Base account signup model used for self service registrations.
    /// </summary>
    public abstract class CreateAccountModelBase : IValidatableObject
    {
        #region Properties

        [Required(AllowEmptyStrings = false, ErrorMessage = "A first name is required")]
        [Display(Name = "First Name")]
        [MinLength(1)]
        [MaxLength(100)]
        public String FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "A last name is required")]
        [Display(Name = "Last Name")]
        [MinLength(1)]
        [MaxLength(100)]
        public String LastName { get; set; }

        [Required(ErrorMessage = "Valid email address is required.")]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Valid email address is required.")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Your email address is not correctly formatted.")]
        [MaxLength(250)]
        public String Email { get; set; }

        public LeadSource Source { get; set; }

        public String UrlReferrer { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Your password must be at least 6 characters long.")]
        public String Password { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Shared utility to query for the requester IP.
        /// </summary>
        /// <returns>If present, the IP address.</returns>
        public virtual String DetermineIpForCaller()
        {
            return HttpContext.Current?.Request.UserHostAddress;
        }

        /// <summary>
        /// Allows implementors to create the appropriate <see cref="Lead"/> entity for the sign-up type.
        /// </summary>
        /// <param name="application">The <see cref="Application"/> that the lead should be created to.</param>
        /// <returns>The configured <see cref="Lead"/> based on the model information.</returns>
        public abstract Lead ToLead(Application application);

        #endregion

        #region IValidatableObject Members

        /// <inheritdoc />
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((this.Email ?? String.Empty).EndsWith("@accurateappend.com", StringComparison.OrdinalIgnoreCase))
            {
                yield return new ValidationResult("The email address is invalid", new[] {nameof(this.Email)});
            }
        }

        #endregion
    }
}