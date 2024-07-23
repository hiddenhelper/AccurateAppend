using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AccurateAppend.Core;

namespace AccurateAppend.Websites.Admin.Areas.Billing.CreateCreditCard.Models
{
    [Serializable()]
    public class CreditCardModel : IValidatableObject
    {
        public static class DataSize
        {
            public const Int32 CardNumber = 25;
            public const Int32 CscValue = 8;
        }
        
        public CreditCardModel()
        { 
            this.ExpMonth = DateTime.Now.Month.ToString("00");
            this.ExpYear = DateTime.Now.Year.ToString("0000");
        }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Card is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Card Number")]
        [MinLength(15)]
        public String CardNumber { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Expiration is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Exp Date")]
        [MinLength(2)]
        [MaxLength(2)]
        public String ExpMonth { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Expiration is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Exp Date")]
        [MinLength(4)]
        [MaxLength(4)]
        public String ExpYear { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "CSC Number")]
        [MinLength(3)]
        [MaxLength(DataSize.CscValue)]
        public String CscValue { get; set; }

        public virtual DateTime CreateExpirationDate()
        {
            var result = DateTime.Parse($@"{this.ExpMonth}/{1}/{this.ExpYear}");
            return result.ToLastOfMonth();
        }

        /// <inheritdoc />
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Luhn algorithm
            var checksum = this.CardNumber
                .Select((c, i) => (c - '0') << ((this.CardNumber.Length - i - 1) & 1))
                .Sum(n => n > 9 ? n - 9 : n);

            var isValid = (checksum % 10) == 0 && checksum > 0;
            if (!isValid)
            {
                yield return new ValidationResult("Please enter a valid credit card number.", new[] { nameof(this.CardNumber) });
            }
        }
    }
}