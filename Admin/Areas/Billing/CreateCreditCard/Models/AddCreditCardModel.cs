using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AccurateAppend.Websites.Admin.Areas.Billing.Shared.Models;

namespace AccurateAppend.Websites.Admin.Areas.Billing.CreateCreditCard.Models
{
    [Serializable()]
    public class AddCreditCardModel : IValidatableObject
    {
        public String UserName { get; set; }

        [Required()]
        public Guid UserId { get; set; }

        [Required()]
        public CreditCardModel Card { get; set; }

        [Required()]
        public BillingAddressModel Address { get; set; }

        /// <inheritdoc />
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return this.Card?.Validate(validationContext) ?? Enumerable.Empty<ValidationResult>();
        }
    }
}