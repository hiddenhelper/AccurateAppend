using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AccurateAppend.Core;

namespace AccurateAppend.Websites.Admin.Areas.Clients.ContractWizard.Models
{
    /// <summary>
    /// View model for creating a new subscription.
    /// </summary>
    public class CreateSubscriptionModel : IValidatableObject
    {
        #region Fields

        private DateTime effectiveDate;
        private DateTime? endDate;
        private DateTime firstAvailableDate;
        private decimal? maxBalance;

        #endregion

        #region Properties

        public Guid UserId { get; set; }

        public DateTime EffectiveDate
        {
            get { return this.effectiveDate; }
            set { this.effectiveDate = value.ToSafeLocal().Date; }
        }

        public DateTime? EndDate
        {
            get { return this.endDate; }
            set { this.endDate = value?.ToSafeLocal().Date; }
        }

        public Decimal Prepayment { get; set; }

        public DateGrain Cycle { get; set; }

        public DateTime FirstAvailableDate
        {
            get { return this.firstAvailableDate; }
            set { this.firstAvailableDate = value.ToSafeLocal().Date; }
        }

        public Decimal? MaxBalance
        {
            get { return this.maxBalance; }
            set
            {
                if (value == 0) value = null;
                this.maxBalance = value;
            }
        }

        public Boolean HasCustomBilling { get; set; }

        #endregion

        #region IValidatableObject Members

        /// <summary>Determines whether the specified object is valid.</summary>
        /// <returns>A collection that holds failed-validation information.</returns>
        /// <param name="validationContext">The validation context.</param>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.Prepayment < 50) yield return new ValidationResult($"{nameof(this.Prepayment)} amount must be at least $50.", new[] {nameof(this.Prepayment)});
            if (this.EndDate.HasValue && this.EndDate.Value < this.EffectiveDate) yield return new ValidationResult($"{nameof(this.EndDate)} must be after the effective date.", new[] {nameof(this.EndDate)});
            if (this.Cycle != DateGrain.Month && this.Cycle != DateGrain.Year) yield return new ValidationResult("Subscription billing cycles must be monthly or yearly", new[] {nameof(this.Cycle)});
            if (this.MaxBalance.HasValue && this.MaxBalance.Value != 0 && this.MaxBalance.Value < 50) yield return new ValidationResult($"{nameof(this.MaxBalance)} must be at least $50.", new[] { nameof(this.MaxBalance) });
        }

        #endregion
    }
}