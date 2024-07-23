using DomainModel.Html;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AccurateAppend.Standardization;

namespace AccurateAppend.Websites.Clients.Areas.Profile.Contact.Models
{
    [Serializable()]
    public class ContactDetailsModel : IValidatableObject
    {
        #region Properties

        /// <summary>
        /// Gets or sets the business name value of the model.
        /// </summary>
        [MaxLength(100)]
        [Display(Name = "Business Name")]
        public String BusinessName { get; set; }

        /// <summary>
        /// Gets or sets the first name value of the model.
        /// </summary>
        [Display(Name = "First Name")]
        [MaxLength(100)]
        public String FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name value of the model.
        /// </summary>
        [Display(Name = "Last Name")]
        [MaxLength(100)]
        public String LastName { get; set; }

        /// <summary>
        /// Gets or sets the postal address value of the model.
        /// </summary>
        [Display(Name = "Address")]
        [MaxLength(50)]
        public String Address { get; set; }

        /// <summary>
        /// Gets or sets the city name value of the model.
        /// </summary>
        [Display(Name = "City")]
        [MaxLength(50)]
        public String City { get; set; }

        /// <summary>
        /// Gets or sets the state or province name value of the model.
        /// </summary>
        /// <remarks>
        /// The reason behind two separate variables for state is to accomodate for dropdown and textbox entries
        /// </remarks>
        [Display(Name = "State")]
        public String State
        {
            get
            {
                return (this.Country == Countries.Canada || this.Country == Countries.Canada)
                    ? this.StateAbbreviation
                    : this.StatePlainText;
            }
            set
            {
                this.StateAbbreviation = value;
                this.StatePlainText = value;
            }
        }

        /// <summary>
        /// This value is used when the country selected is US or Canada
        /// </summary>
        public String StateAbbreviation { get; set; }

        /// <summary>
        /// This value is used when the country selected is non-US or Canada
        /// </summary>
        public String StatePlainText { get; set; }

        /// <summary>
        /// Gets or sets the postal code value of the model.
        /// </summary>
        [Display(Name = "Postal code")]
        [DataType(DataType.PostalCode)]
        public String PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country name value of the model.
        /// </summary>
        [Display(Name = "Country")]
        public String Country { get; set; }

        /// <summary>
        /// Gets or sets the phone number value of the model.
        /// </summary>
        [Display(Name = "Phone")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Valid phone number is required")]
        public String Phone { get; set; }

        /// <summary>
        /// Gets or sets the email address value of the model.
        /// </summary>
        [Required()]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Valid email address is required")]
        public String Email { get; set; }

        #endregion

        #region IValidatableObject Members

        /// <inheritdoc />
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!String.IsNullOrEmpty(FirstName) && String.IsNullOrEmpty(LastName))
                yield return new ValidationResult("Last Name is required", new[] {nameof(LastName)});
            if (!String.IsNullOrEmpty(LastName) && String.IsNullOrEmpty(FirstName))
                yield return new ValidationResult("First Name is required", new[] {nameof(FirstName)});

            if (!String.IsNullOrEmpty(Address)
                || !String.IsNullOrEmpty(City)
                || !String.IsNullOrEmpty(PostalCode))
            {
                if (String.IsNullOrEmpty(Address))
                    yield return new ValidationResult("Address is required", new[] {nameof(Address)});
                if (String.IsNullOrEmpty(City))
                    yield return new ValidationResult("City is required", new[] {nameof(City)});
                if (String.IsNullOrEmpty(PostalCode))
                    yield return new ValidationResult("Postal Code is required", new[] {nameof(PostalCode)});
                if (String.IsNullOrEmpty(Country))
                    yield return new ValidationResult("Country is required", new[] {nameof(Country)});
                if (String.IsNullOrEmpty(State))
                    yield return new ValidationResult("State is required", new[] {nameof(State)});
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Perform standardization cleanup on the model.
        /// </summary>
        /// <param name="addressParser">The <see cref="IAddressStandardizer"/> component.</param>
        /// <param name="nameParser">The <see cref="INameStandardizer"/> component.</param>
        public virtual void CleanUp(IAddressStandardizer addressParser, INameStandardizer nameParser)
        {
            if (addressParser == null) throw new ArgumentNullException(nameof(addressParser));
            if (nameParser == null) throw new ArgumentNullException(nameof(nameParser));

            #region Name Parse

            {
                var result = nameParser.Parse($"{this.FirstName} {this.LastName}");
                this.FirstName = result.FirstName;
                this.LastName = result.LastName;
            }

            #endregion

            if (this.Country != Countries.UnitedStates && this.Country != Countries.Canada) return;

            #region Address Parse

            {
                var result = addressParser.Parse(this.Address + "", this.City + "", this.State + "", this.PostalCode + "");
                if (result.StatusCode != "9" && result.StatusCode != "C" && result.StatusCode != "S") return;

                this.Address = result.Address;
                this.City = result.City;
                this.State = result.State;
                this.PostalCode = result.Zip;
            }

            #endregion
        }
        
        #endregion
    }
}