using System;
using System.ComponentModel.DataAnnotations;
using AccurateAppend.Accounting;
using AccurateAppend.Security;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.Signup.Models
{
    /// <summary>
    /// Model used during NationBuilder self-registration process. Responsible for
    /// extending the <see cref="CreateAccountModelBase"/> and containing the unique
    /// public key (used to uniquely identify the individual request instance) and the
    /// SLUG of the nation itself. Also contains a custom lead generation implementation
    /// for NationBuilder.
    /// </summary>
    public class NationBuilderCreateAccountModel : CreateAccountModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NationBuilderCreateAccountModel"/> class.
        /// </summary>
        public NationBuilderCreateAccountModel()
        {
            this.Source = LeadSource.NationBuilder;
            this.FirstName = PartyExtensions.UnknownFirstName;
            this.LastName = PartyExtensions.UnknownLastName;
        }

        /// <summary>
        /// Gets or sets the SLUG (or name) of the Nation being registered in this request. The SLUG is
        /// generally the DNS host name portion of the FQDN of the Nation.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"[\d\w_]+", ErrorMessage = "Please only supply the unique name portion of your Nation.")]
        public String Slug { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Guid"/> that is used to uniquely identify the registration instance.
        /// </summary>
        [Required()]
        public Guid PublicKey { get; set; }

        #region Overrides

        /// <summary>
        /// Creates a new <see cref="Lead"/> entity based on the current model instance.
        /// </summary>
        /// <param name="application">The <see cref="Application"/> the created lead should be created rom.</param>
        /// <returns>The new <see cref="Lead"/> instance based on the current model data.</returns>
        public override Lead ToLead(Application application)
        {
            var lead = new Lead(application, this.FirstName, this.LastName)
            {
                DefaultEmail = this.Email,
                Website = $"http://{this.Slug}.nationbuilder.com"
            };

            return lead;
        }

        #endregion
    }
}