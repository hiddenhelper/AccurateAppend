using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AccurateAppend.Accounting;
using AccurateAppend.Security;

namespace AccurateAppend.Websites.Admin.Areas.Clients.LeadWizard.Models
{
    /// <summary>
    /// Model for Lead Wizard
    /// </summary>
    public class LeadWizardViewModel : IValidatableObject
    {

        #region IValidatableObject members

        /// <summary>Determines whether the specified object is valid.</summary>
        /// <returns>A collection that holds failed-validation information.</returns>
        /// <param name="validationContext">The validation context.</param>
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // todo: implement validation logic
            return new List<ValidationResult>();
        }

        #endregion
    }
}