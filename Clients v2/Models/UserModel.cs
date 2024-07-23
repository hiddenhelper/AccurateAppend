using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using AccurateAppend.Accounting;
using AccurateAppend.Security;
using AccurateAppend.Websites.Clients.Areas;
using AccurateAppend.Websites.Clients.Validation;
using DomainModel.Html;

namespace AccurateAppend.Websites.Clients.Models
{
    /// <summary>
    /// Model used for sales directed signup.
    /// </summary>
    [Serializable()]
    public class UserModel : CreateAccountModelBase
    {
        #region Properties

        #region Contact Info

        [Required()]
        [Display(Name = "Business Name")]
        public String BusinessName { get; set; }
        
        [Required]
        [Display(Name = "Address")]
        public String Address { get; set; }

        [Required]
        [Display(Name = "City")]
        public String City { get; set; }

        [Required]
        [Display(Name = "State")]
        public String State
        {
            get
            {
                return (this.Country == Countries.Canada
                    || this.Country == Countries.UnitedStates)
                        ? this.StateAbbreviation : this.StatePlainText;
            }
            set
            {
                this.StateAbbreviation = value;
                this.StatePlainText = value;
            }
        }

        /// <summary>
        /// this value is used when the country selected is US or Canada
        /// </summary>
        public string StateAbbreviation { get; set; }

        /// <summary>
        /// this value is used when the country selected is non-US or Canada
        /// </summary>
        public string StatePlainText { get; set; }

        [Required]
        [Display(Name = "Country")]
        public String Country { get; set; }

        [Required]
        [Display(Name = "Postal code")]
        [DataType(DataType.PostalCode)]
        public String PostalCode { get; set; }

        [Required]
        [Display(Name = "Phone")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Valid phone number is required")]
        public String Phone { get; set; }

        #endregion
        
        #region Payment Info

        [Display(Name = "Card Holder First Name")]
        public String CardHolderFirstName { get; set; }

        [Display(Name = "Card Holder Last Name")]
        public String CardHolderLastName { get; set; }

        [Display(Name = "Card Holder Phone")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Valid phone number is required")]
        public String CardHolderPhone { get; set; }

        [Display(Name = "Card Address")]
        public String CardAddress { get; set; }

        [Display(Name = "Card City")]
        public String CardCity { get; set; }

        [Display(Name = "Card State")]
        public String CardState {
            get
            {
                return (this.CardCountry == Countries.Canada
                    || this.CardCountry == Countries.UnitedStates)
                        ? this.CardStateAbbreviation : this.CardStatePlainText;
            }
            set
            {
                this.CardStateAbbreviation = value;
                this.CardStatePlainText = value;
            }
        }

        /// <summary>
        /// this value is used when the country selected is US or Canada
        /// </summary>
        public string CardStateAbbreviation { get; set; }

        /// <summary>
        /// this value is used when the country selected is non-US or Canada
        /// </summary>
        public string CardStatePlainText { get; set; }

        [Display(Name = "Card Country")]
        public String CardCountry { get; set; }

        [Display(Name = "Card Postal code")]
        [DataType(DataType.PostalCode)]
        public String CardPostalCode { get; set; }

        [Display(Name = "Card Number")]
        [DataType(DataType.CreditCard, ErrorMessage = "Valid credit card number is required")]
        public String CardNumber { get; set; }

        [Display(Name = "Card Expiration Month")]
        public String CardExpirationMonth { get; set; }

        [Display(Name = "Card Expiration Year")]
        public String CardExpirationYear { get; set; }

        [Display(Name = "Card CVV")]
        public String CardCvv { get; set; }

        #endregion

        #region Terms2

        [Required()]
        [Boolean(RequiredValue = true, ErrorMessage = "You must accept the Terms of Service to continue.")]
        public Boolean AgreeToTerms { get; set; }
        
        #endregion

        #region File Upload Info

        [Display(Name = "File")]
        public IEnumerable<HttpPostedFileBase> Files { get; set; }

        [Display(Name = "Comments")]
        public String Comments { get; set; }

        #endregion
        
        #region Misc properties

        public Guid PublicKey { get; set; }

        public Guid ApplicationId { get; set; }
        
        #endregion
        
        #endregion

        #region Methods

        public DateTime GetExpirationDate()
        {
            var value = $"{this.CardExpirationMonth}-1-{this.CardExpirationYear}";
            return DateTime.Parse(value);
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var error in base.Validate(validationContext))
            {
                yield return error;
            }

            if (String.IsNullOrEmpty(this.CardHolderFirstName))
            {
                yield return new ValidationResult("Please specify card holder first name.",new []{nameof(this.CardHolderFirstName)});
            }

            if (String.IsNullOrEmpty(this.CardHolderLastName))
            {
                yield return new ValidationResult("Please specify card holder last name.", new[] { nameof(this.CardHolderLastName) });
            }

            if (String.IsNullOrEmpty(this.CardHolderPhone))
            {
                yield return new ValidationResult("Please specify card holder phone.", new[] {nameof(this.CardHolderPhone)});
            }

            if (String.IsNullOrEmpty(this.CardAddress))
            {
                yield return new ValidationResult("Please specify card holder address.", new[] {nameof(this.CardAddress)});
            }

            if (String.IsNullOrEmpty(this.CardCity))
            {
                yield return new ValidationResult("Please specify card holder city.", new[] {nameof(this.CardCity)});
            }

            if (String.IsNullOrEmpty(this.CardState))
            {
                yield return new ValidationResult("Please specify card holder state.", new[] {nameof(this.CardState)});
            }

            if (String.IsNullOrEmpty(this.CardPostalCode))
            {
                yield return new ValidationResult("Please specify card holder zip code.", new[] { nameof(this.CardPostalCode)});
            }

            if (String.IsNullOrEmpty(this.CardNumber))
            {
                yield return new ValidationResult("Please specify a valid credit card number.", new[] { nameof(this.CardNumber)});
            }
            else
            {
                if (!Regex.IsMatch(this.CardNumber + "", @"^((4\d{3})|(5[1-5]\d{2})|(6011))-?\d{4}-?\d{4}-?\d{4}|3[4,7]\d{13}$"))
                {
                    yield return new ValidationResult("Please specify a valid credit card number.", new[] { nameof(this.CardNumber)});
                }
                else
                {
                    // Luhn algorithm
                    var checksum = this.CardNumber
                        .Select((c, i) => (c - '0') << ((this.CardNumber.Length - i - 1) & 1))
                        .Sum(n => n > 9 ? n - 9 : n);

                    var isValid = (checksum % 10) == 0 && checksum > 0;
                    if (!isValid)
                    {
                        yield return new ValidationResult("Please specify a valid credit card number.", new[] { nameof(this.CardNumber) });
                    }

                    if (String.IsNullOrEmpty(this.CardExpirationMonth))
                    {
                        yield return new ValidationResult("Please specify credit card expiration month.", new[] { nameof(this.CardExpirationMonth)});
                    }
                    if (String.IsNullOrEmpty(this.CardExpirationYear))
                    {
                        yield return new ValidationResult("Please specify credit card expiration year.", new[] { nameof(this.CardExpirationYear)});
                    }
                    if (String.IsNullOrEmpty(this.CardCvv))
                    {
                        yield return new ValidationResult("Please specify credit card CVV.", new[] { nameof(this.CardCvv)});
                    }
                }
            }

            if (String.IsNullOrEmpty(this.CardState))
                yield return new ValidationResult("Card State is required", new[] { nameof(this.CardState) });

            if (String.IsNullOrEmpty(this.State))
                yield return new ValidationResult("State is required", new[] { nameof(this.CardState) });
        }
        
        /// <inheritdoc />
        public sealed override Lead ToLead(Application application)
        {
            throw new NotSupportedException("This signup model is predicated on having a sales generated lead");
        }

        #endregion
    }
}
