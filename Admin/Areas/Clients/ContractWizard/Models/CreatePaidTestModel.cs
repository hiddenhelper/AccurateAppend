using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AccurateAppend.Core;

namespace AccurateAppend.Websites.Admin.Areas.Clients.ContractWizard.Models
{
    /// <summary>
    /// View model for creating a new Accruing contract for paid tests.
    /// </summary>
    public class CreatePaidTestModel : IValidatableObject
    {
        #region Fields

        private DateTime effectiveDate;
        private DateTime endDate;
        private DateTime firstAvailableDate;
        private Decimal maxBalance;

        #endregion

        #region Properties

        public Guid UserId { get; set; }

        public DateTime EffectiveDate
        {
            get { return this.effectiveDate; }
            set { this.effectiveDate = value.ToSafeLocal().Date; }
        }

        public DateTime EndDate
        {
            get { return this.endDate; }
            set { this.endDate = value.ToSafeLocal().Date; }
        }
        
        public DateTime FirstAvailableDate
        {
            get { return this.firstAvailableDate; }
            set { this.firstAvailableDate = value.ToSafeLocal().Date; }
        }

        public Decimal MaxBalance
        {
            get { return this.maxBalance; }
            set
            {
                this.maxBalance = value;
            }
        }

        #endregion

        #region IValidatableObject Members

        /// <summary>Determines whether the specified object is valid.</summary>
        /// <returns>A collection that holds failed-validation information.</returns>
        /// <param name="validationContext">The validation context.</param>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.EndDate < this.EffectiveDate) yield return new ValidationResult($"{nameof(this.EndDate)} must be after the effective date.", new[] { nameof(this.EndDate) });
            if ((this.EndDate - this.EffectiveDate).TotalDays > 90) yield return new ValidationResult($"{nameof(this.EndDate)} cannot be greater than 90 days after the effective date.", new[] { nameof(this.EndDate) });
            if (this.MaxBalance < 50) yield return new ValidationResult($"{nameof(this.MaxBalance)} must be at least $50.", new[] { nameof(this.MaxBalance) });
        }

        #endregion
    }
}