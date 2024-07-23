using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AccurateAppend.Websites.Clients.Areas.Public.OptOut.Models
{
    [Serializable()]
    public class OptOutModel : IValidatableObject
    {
        [Required(AllowEmptyStrings = false)]
        public String FirstName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public String LastName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public String Address { get; set; }

        [Required(AllowEmptyStrings = false)]
        public String City { get; set; }

        public String State { get; set; }
        
        public String StatePlainText { get; set; }

        [Required]
        public String StateAbbreviation { get; set; }

        public String PostalCode { get; set; }

        public String Phone { get; set; }

        public String Email { get; set; }

        public String Comments { get; set; }

        #region IValidatableObject Members

        /// <inheritdoc />
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!String.IsNullOrEmpty(FirstName) && String.IsNullOrEmpty(LastName))
                yield return new ValidationResult("Last Name is required", new[] { nameof(LastName) });
            if (!String.IsNullOrEmpty(LastName) && String.IsNullOrEmpty(FirstName))
                yield return new ValidationResult("First Name is required", new[] { nameof(FirstName) });

            if (!String.IsNullOrEmpty(Address)
                || !String.IsNullOrEmpty(City)
                || !String.IsNullOrEmpty(PostalCode))
            {
                if (String.IsNullOrEmpty(Address))
                    yield return new ValidationResult("Address is required", new[] { nameof(Address) });
                if (String.IsNullOrEmpty(City))
                    yield return new ValidationResult("City is required", new[] { nameof(City) });
                if (String.IsNullOrEmpty(PostalCode))
                    yield return new ValidationResult("Postal Code is required", new[] { nameof(PostalCode) });
                if (String.IsNullOrEmpty(StateAbbreviation))
                    yield return new ValidationResult("State is required", new[] { nameof(State) });
            }
        }

        #endregion
    }
}