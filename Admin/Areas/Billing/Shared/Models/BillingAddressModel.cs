using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AccurateAppend.Websites.Admin.Areas.Billing.Shared.Models
{
    [Serializable()]
    public class BillingAddressModel : IValidatableObject
    {
        public static class DataSize
        {
            public const Int32 BusinessName = 100;
            public const Int32 FirstName = 100;
            public const Int32 LastName = 100;
            public const Int32 Address = 100;
            public const Int32 City = 50;
            public const Int32 State = 50;
            public const Int32 Zip = 10;
            public const Int32 PhoneNumber = 50;
        }

        public BillingAddressModel()
        {
        }

        public BillingAddressModel(Int32 cardId) : this()
        {
            this.CarId = cardId;
        }
        

        [Required()]
        public Int32 CarId { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Business Name")]
        [MaxLength(DataSize.BusinessName)]
        public String BusinessName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required")]
        [DataType(DataType.Text)]
        [Display(Name = "First Name")]
        [MaxLength(DataSize.FirstName)]
        public String FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Last Name")]
        [MaxLength(DataSize.LastName)]
        public String LastName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Phone is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$")]
        [MaxLength(DataSize.PhoneNumber)]
        public String PhoneNumber { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Address is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Address")]
        [MaxLength(DataSize.Address)]
        public String Address { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "City is required")]
        [DataType(DataType.Text)]
        [Display(Name = "City")]
        [MaxLength(DataSize.City)]
        public String City { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "State is required")]
        [DataType(DataType.Text)]
        [Display(Name = "State")]
        public String State { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Zip is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Zip")]
        [MaxLength(DataSize.Zip)]
        public String Zip { get; set; }

        [Required()]
        [Display(Name = "Card Country")]
        public String Country { get; set; }

        #region IValidatableObject Members

        /// <inheritdoc />
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.Country != DomainModel.Html.Countries.UnitedStates &&
                this.Country != DomainModel.Html.Countries.Canada) yield break;

            var state = (this.State ?? String.Empty).Trim();
            if (state.Length != 2) yield return new ValidationResult("US/CA states must be a valid 2 character abbreviation code", new[] { nameof(this.State) });
        }

        #endregion
    }
}