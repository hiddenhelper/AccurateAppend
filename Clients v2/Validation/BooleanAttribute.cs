using System;
using System.ComponentModel.DataAnnotations;

namespace AccurateAppend.Websites.Clients.Validation
{
    /// <summary>
    /// Custom <see cref="ValidationAttribute">validator</see> that enforces a required boolean value.
    /// </summary>
    public sealed class BooleanAttribute : ValidationAttribute
    {
        /// <summary>
        /// Gets or sets the required value.
        /// </summary>
        public Boolean RequiredValue { get; set; }

        /// <summary>
        /// Determines whether the specified value of the object is valid. 
        /// </summary>
        /// <returns>
        /// true if the specified value is valid; otherwise, false.
        /// </returns>
        /// <param name="value">The value of the object to validate. </param>
        public override Boolean IsValid(Object value)
        {
            return value is Boolean && (Boolean)value == this.RequiredValue;
        }
    }
}