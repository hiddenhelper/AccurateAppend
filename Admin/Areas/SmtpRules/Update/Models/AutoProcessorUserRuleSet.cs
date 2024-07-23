using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.UpdateSmtpRule.Models
{
    [DebuggerDisplay("{" + nameof(UserId) + "}:{" + nameof(Rules) + ".Count}")]
    public class AutoProcessorUserRuleSetModel : IValidatableObject
    {
        public Guid UserId { get; set; }

        public List<AutoProcessorUserRule> Rules { get; set; }

        #region IValidatableObject Members

        /// <inheritdoc />
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            foreach (var rule in Rules)
            {
                if (String.IsNullOrEmpty(rule.Terms))
                {
                    errors.Add(new ValidationResult("Terms must be populated."));
                }
                if (String.IsNullOrEmpty(rule.Description))
                {
                    errors.Add(new ValidationResult("Description must be populated."));
                }
                if (!(rule.Subject || rule.Body || rule.FileName))
                {
                    errors.Add(new ValidationResult("Subject, Body or File name must be checked."));
                }
            }

            return errors;
        }

        #endregion


    }
}