using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AccurateAppend.Accounting;
using AccurateAppend.Security;

namespace AccurateAppend.Websites.Admin.Areas.Clients.LeadDetail.Models
{
    public class LeadViewModel : IValidatableObject
    {
        public Int32 LeadId { get; set; }

        public Guid ApplicationId { get; set; }

        public LeadScore Score { get; set; }

        public LeadStatus Status { get; set; }

        public LeadQualification Qualified { get; set; }

        public LeadDisqualificationReason? DisqualificationReason { get; set; }

        public String BusinessName { get; set; }

        [Required()]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Please specify a first name.")]
        public String FirstName { get; set; }

        [Required()]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Please specify a last name.")]
        public String LastName { get; set; }

        public String Address { get; set; }

        public String City { get; set; }

        public String State { get; set; }

        public String Zip { get; set; }

        public String Phone { get; set; }

        [EmailAddress(ErrorMessage = "The email address is not formatted correctly")]
        public String Email { get; set; }

        public String Website { get; set; }

        public String Comments { get; set; }

        [Required()]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Please specify a product interest.")]
        public String ProductInterest { get; set; }

        public LeadContactMethod ContactMethod { get; set; }

        public LeadSource LeadSource { get; set; }

        public String IP { get; set; }
        
        public String LandingPageUrl { get; set; }

        public String LandingPageDomain { get; set; }

        public DateTime? FollowUpDate { get; set; }

        public DateTime DateAdded { get; set; }

        public Guid PublicKey { get; set; }

        public Boolean DoNotMarketTo { get; set; }

        public String ApiReportAction { get; set; }

        public int TrialId { get; set; }

        public String SectorIndustry { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the user that will own the deal.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Text, ErrorMessage = "*")]
        public Guid? OwnerId { get; set; }

        /// <summary>
        /// Gets the ZenSales link, if any, for the lead.
        /// </summary>
        public String CrmLink { get; set; }

        #region IValidatableObject members

        /// <summary>Determines whether the specified object is valid.</summary>
        /// <returns>A collection that holds failed-validation information.</returns>
        /// <param name="validationContext">The validation context.</param>
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.ApplicationId != Security.ApplicationExtensions.AccurateAppendId &&
                this.ApplicationId != Security.ApplicationExtensions.TwentyTwentyId)
            {
                yield return new ValidationResult("Please select a site.", new[] {nameof(this.ApplicationId)});
            }

            if (this.Qualified == LeadQualification.NotQualified && this.DisqualificationReason == null)
            {
                yield return new ValidationResult("Please indicate a disqualification reason.",new[] {nameof(this.DisqualificationReason) });
            }

            if (this.Status == LeadStatus.ConvertedToCustomer && this.DoNotMarketTo)
            {
                yield return new ValidationResult("A Do Not Market To flag cannot be used on a customer lead.", new[] { nameof(this.DoNotMarketTo) });
            }
        }

        #endregion
    }
}