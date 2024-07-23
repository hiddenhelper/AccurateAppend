using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.ChargeProcessing;
using AccurateAppend.Core;
using AccurateAppend.Standardization;
using DomainModel.Html;

namespace AccurateAppend.Websites.Clients.Areas.Profile.Card.Models
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

        public PaymentDetailsModel(ChargePayment account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            Contract.EndContractBlock();

            this.Id = account.Id;
            this.CardHolderBusinessName = account.BillTo.BusinessName;
            this.CardHolderFirstName = account.BillTo.FirstName;
            this.CardHolderLastName = account.BillTo.LastName;
            this.CardAddress = account.BillTo.Address;
            this.CardCity = account.BillTo.City;
            this.CardState = account.BillTo.State;
            this.CardCountry = account.BillTo.Country;
            this.CardPostalCode = account.BillTo.Zip;
            this.CardExpirationMonth = account.Card.Expiration.Month.ToString("D2");
            this.CardExpirationYear = account.Card.Expiration.Year.ToString();
            this.CardHolderPhone = account.BillTo.PhoneNumber;
            this.Display = account.Card.ToString();
            this.UserId = account.Client.UserId;
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

        [MaxLength(100)]
        [Display(Name = "Card Address")]
        public String CardAddress { get; set; }

        [MaxLength(50)]
        [Display(Name = "Card City")]
        public String CardCity { get; set; }

        [MaxLength(50)]
        [Display(Name = "Card State")]
        public String CardState
        {
            get => (this.CardCountry == Countries.Canada || this.CardCountry == Countries.UnitedStates)
                ? this.CardStateAbbreviation
                : this.CardStatePlainText;
            set
            {
                this.CardStateAbbreviation = value;
                this.CardStatePlainText = value;
            }
        }

        /// <summary>
        /// this value is used when the country selected is US or Canada
        /// </summary>
        public String CardStateAbbreviation { get; set; }

        /// <summary>
        /// this value is used when the country selected is non-US or Canada
        /// </summary>
        public String CardStatePlainText { get; set; }

        [MaxLength(100)]
        [Display(Name = "Card Country")]
        public String CardCountry { get; set; }

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

        public Guid UserId { get; set; }

        public Int32? Id { get; set; }

        public Guid RequestId { get; set; }

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

        /// <summary>
        /// Attempts to use the supplied <paramref name="parser"/> to cleanup and standardize address information.
        /// </summary>
        /// <remarks>
        /// Only supports this with US and CA addresses
        /// </remarks>
        /// <param name="parser">The <see cref="IAddressStandardizer"/> component that can provide US and CA address standardization.</param>
        public void Cleanup(IAddressStandardizer parser)
        {
            if (this.CardCountry != Countries.UnitedStates &&
                this.CardCountry != Countries.Canada) return;

            var result = parser.Parse(this.CardAddress + "", this.CardCity + "", this.CardState + "", this.CardPostalCode + "");
            if (result.StatusCode != "9" && result.StatusCode != "C" && result.StatusCode != "S") return;

            this.CardAddress = result.Address;
            this.CardCity = result.City;
            this.CardState = result.State;
            this.CardPostalCode = result.Zip;
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