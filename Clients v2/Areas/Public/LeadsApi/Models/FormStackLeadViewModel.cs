using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace AccurateAppend.Websites.Clients.Areas.Public.LeadsApi.Models
{
    /// <summary>
    /// Lead structure used by FormStack form post
    /// </summary>
    // TODO: merge this with the bus command
    [Serializable()]
    public class FormStackLeadViewModel : IValidatableObject
    {
        #region Properties

        public String Company { get; set; }

        public String Name { get; set; }

        public String Email { get; set; }

        public String Phone { get; set; }

        public String Comments { get; set; }

        public String Ip { get; set; }

        public String Referrer { get; set; }

        public String HandshakeKey { get; set; }

        [BindingName(Name = "Product Interest")]
        public String ProductInterest { get; set; }

        [BindingName(Name = "Estimated Record Count")]
        public String RecordCount { get; set; }

        public String LandingPageUrl { get; set; }

        [BindingName(Name = "Request Type")]
        public String RequestType { get; set; }

        [BindingName(Name = "Nature of Request")]
        public String TicketType { get; set; }
        
        #endregion

        #region IValidatableObject Members

        /// <summary>
        /// Determines whether the specified object is valid.
        /// </summary>
        /// <returns>
        /// A collection that holds failed-validation information.I
        /// </returns>
        /// <param name="validationContext">The validation context.</param>
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!String.Equals(this.HandshakeKey, "CF1311A0-6332-4CF1-A2E2-3B02AA64771D"))
                yield return new ValidationResult("Your request was unable to be processed. Bad request.");

            if (String.IsNullOrWhiteSpace(this.Email) && String.IsNullOrWhiteSpace(this.Phone))
                yield return new ValidationResult("Email OR Phone must be supplied.");
        }

        #endregion
    }
}