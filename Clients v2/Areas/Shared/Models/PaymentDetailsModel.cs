using AccurateAppend.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AccurateAppend.Websites.Clients.Areas.Shared.Models
{
    [Serializable()]
    public class PaymentDetailsModel : IValidatableObject
    {
        #region Constructors

        public PaymentDetailsModel()
        {
            this.CardExpirationMonth = DateTime.Now.Month.ToString();
            this.CardExpirationYear = DateTime.Now.Year.ToString();
        }

        #endregion

        #region Properties

        [Display(Name = "Card Holder Business Name")]
        public String CardHolderBusinessName { get; set; }

        [Required()]
        [MaxLength(100)]
        [Display(Name = "Card Holder First Name")]
        public String CardHolderFirstName { get; set; }

        [Required()]
        [MaxLength(100)]
        [Display(Name = "Card Holder Last Name")]
        public String CardHolderLastName { get; set; }

        [MaxLength(50)]
        [Display(Name = "Card Holder Phone")]
        public String CardHolderPhone { get; set; }
        
        [Required]
        [Display(Name = "Card Postal code")]
        [DataType(DataType.PostalCode)]
        public String CardPostalCode { get; set; }

        [Required()]
        [Display(Name = "Card Number")]
        [DataType(DataType.CreditCard, ErrorMessage = "Valid credit card number is required")]
        [RegularExpression(@"^((4\d{3})|(5[1-5]\d{2})|(6011))-?\d{4}-?\d{4}-?\d{4}|3[4,7]\d{13}$", ErrorMessage = "Please enter a valid credit card number.")]
        public String CardNumber { get; set; }

        public String Display { get; set; }

        [Display(Name = "Card Expiration Month")]
        public String CardExpirationMonth { get; set; }

        [Display(Name = "Card Expiration Year")]
        public String CardExpirationYear { get; set; }

        [Required()]
        [MaxLength(4)]
        [MinLength(3)]
        [RegularExpression("^[0-9]*$",ErrorMessage="Only numeric values are allowed")]
        [Display(Name = "Card CVV")]
        public String CardCvv { get; set; }

        public String RedirectTo { get; set; }

        public Guid ApplicationId { get; set; }

        public Guid UserId { get; set; }

        #endregion

        #region Methods

        public DateTime GetExpirationDate()
        {
            try
            {
                var value = $"{this.CardExpirationMonth}-1-{this.CardExpirationYear}";
                return DateTime.Parse(value).ToLastOfMonth().Date;
            }
            catch
            {
                return new DateTime(1900, 1, 1);
            }
        }

        #endregion

        #region IValidatableObject Members

        /// <inheritdoc />
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            if (this.GetExpirationDate() < DateTime.UtcNow)
            {
                errors.Add(new ValidationResult("Please enter a valid expiration date.", new[] {nameof(this.CardExpirationYear)}));
            }

            // Luhn algorithm
            var checksum = this.CardNumber
                .Select((c, i) => (c - '0') << ((this.CardNumber.Length - i - 1) & 1))
                .Sum(n => n > 9 ? n - 9 : n);

            var isValid = (checksum % 10) == 0 && checksum > 0;
            if (!isValid)
            {
                errors.Add(new ValidationResult("Please enter a valid credit card number.", new[] {nameof(this.CardNumber)}));
            }

            return errors;
        }

        #endregion
    }
}