using System;
using System.ComponentModel.DataAnnotations;
using AccurateAppend.Accounting;
using AccurateAppend.Security;

namespace AccurateAppend.Websites.Clients.Areas.LeadPhilanthropy.Signup.Models
{
    /// <summary>
    /// Model used during Lead Philanthropy self-registration process. Responsible for
    /// extending the <see cref="CreateAccountModelBase"/> and containing the unique
    /// public key (used to uniquely identify the individual request instance). Also
    /// contains a custom lead generation implementation for Lead Philanthropy.
    /// </summary>
    public class LeadPhilanthropyCreateAccountModel : CreateAccountModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LeadPhilanthropyCreateAccountModel"/> class.
        /// </summary>
        public LeadPhilanthropyCreateAccountModel()
        {
            this.Source = LeadSource.LeadPhilanthropy;
            this.FirstName = PartyExtensions.UnknownFirstName;
            this.LastName = PartyExtensions.UnknownLastName;
        }

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
                Source = LeadSource.LeadPhilanthropy
            };

            return lead;
        }

        #endregion
    }
}