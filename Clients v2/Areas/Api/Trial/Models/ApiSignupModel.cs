using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AccurateAppend.Websites.Clients.Areas.Api.Trial.Models
{
    [Serializable()]
    public class ApiSignupModel : IValidatableObject
    {
        #region Properties

        public String Name { get; set; }

        public String FirstName { get; set; }

        public String LastName { get; set; }

        public String Company { get; set; }

        public String Email { get; set; }

        public String Phone { get; set; }

        public String Ip { get; set; }

        public String Referrer { get; set; }

        #endregion

        #region Overrides

        public override String ToString()
        {
            return $"FirstName:{this.FirstName},LastName:{this.LastName},Company:{this.Company},Email:{this.Email},Phone:{this.Phone},Ip:{this.Ip},Referrer:{this.Referrer}";
        }

        #endregion

        #region IValidatableObject Members

        /// <summary>
        /// Determines whether the specified object is valid.
        /// </summary>
        /// <returns>
        /// A collection that holds failed-validation information.I
        /// </returns>
        /// <param name="validationContext">The validation context.</param>
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (String.IsNullOrWhiteSpace(this.Email) && String.IsNullOrWhiteSpace(this.Phone)) yield return new ValidationResult("Email OR Phone must be supplied.", new[] {nameof(this.Email), nameof(this.Phone)});
            if (!String.IsNullOrWhiteSpace(this.Email))
            {
                var re = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
                if (!re.IsMatch(this.Email)) yield return new ValidationResult("Your email address is not correctly formatted.", new[]{nameof(this.Email)});
            }
        }

        #endregion
    }
}