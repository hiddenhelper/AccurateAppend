using AccurateAppend.Accounting;
using AccurateAppend.Security;

namespace AccurateAppend.Websites.Clients.Areas.Public.Signup.Models
{
    /// <summary>
    /// Model used for self-service public signup.
    /// </summary>
    public class PublicCreateAccountModel : CreateAccountModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublicCreateAccountModel"/> class.
        /// </summary>
        /// <remarks>
        /// Defaults to a client party of "No Name".
        /// </remarks>
        public PublicCreateAccountModel()
        {
            this.FirstName = PartyExtensions.UnknownFirstName;
            this.LastName = PartyExtensions.UnknownLastName;
            this.Source = LeadSource.Direct;
        }

        #region Overrides

        /// <inheritdoc />
        public override Lead ToLead(Application application)
        {
            var lead = new Lead(application, this.FirstName, this.LastName)
            {
                DefaultEmail = this.Email
            };

            return lead;
        }

        #endregion
    }
}