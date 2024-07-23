using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AccurateAppend.Core;
using NServiceBus;

namespace AccurateAppend.Sales.Contracts.Messages
{
    /// <summary>
    /// Command to create a new accrual based service account.
    /// </summary>
    [Serializable()]
    public class CreateAccrualCommand : ICommand
    {
        #region Fields

        private DateTime effectiveDate;
        private DateTime? endDate;
        private Decimal? maxBalance;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the identifier of the client to create the subscription for.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the alternate identifier of the the subscription to create.
        /// </summary>
        public Guid PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the date the subscription becomes effective.
        /// </summary>
        /// <remarks>
        /// Only the <see cref="DateTime.Date"/> portion is used. Any unspecified value is assumed to be in billing zone.
        /// </remarks>
        public DateTime EffectiveDate
        {
            get { return this.effectiveDate; }
            set { this.effectiveDate = value.ToSafeLocal().Date; }
        }

        /// <summary>
        /// Gets or sets the date the subscription ends, if any.
        /// </summary>
        /// <remarks>
        /// A null value means the subscription continues for an indefinite time-frame.
        /// If present, only the <see cref="DateTime.Date"/> portion is used. Any unspecified value is assumed to be in billing zone.
        /// </remarks>
        public DateTime? EndDate
        {
            get { return this.endDate; }
            set { this.endDate = value?.ToSafeLocal().Date; }
        }

        /// <summary>
        /// Indicates whether the subscription is allowed to carry a maximum outstanding balance.
        /// </summary>
        public Decimal? MaxBalance
        {
            get { return this.maxBalance; }
            set
            {
                if (value == 0) value = null;
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
            if (this.EndDate.HasValue && this.EndDate.Value < this.EffectiveDate) yield return new ValidationResult($"{nameof(this.EndDate)} must be after the effective date.", new[] { nameof(this.EndDate) });
            if (this.MaxBalance.HasValue && this.MaxBalance.Value != 0 && this.MaxBalance.Value < 50) yield return new ValidationResult($"{nameof(this.MaxBalance)} must be at least $50.", new[] { nameof(this.MaxBalance) });
        }

        #endregion
    }
}