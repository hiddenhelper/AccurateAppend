using System;
using System.ComponentModel.DataAnnotations;
using AccurateAppend.Core;

namespace AccurateAppend.Websites.Admin.ViewModels.Common
{
    /// <summary>
    /// Common view model element for representing a single addressable party.
    /// </summary>
    public class Party<T>
    {
        #region Properties

        /// <summary>
        /// Gets the identifier for the current party.
        /// </summary>
        [DataType(DataType.Text)]
        [Required()]
        public T Id { get; set; }

        /// <summary>
        /// Gets or sets the company or business name of the party.
        /// </summary>
        [DataType(DataType.Text)]
        [MaxLength(100)]
        public String BusinessName { get; set; }

        /// <summary>
        /// Gets or sets given name for the party.
        /// </summary>
        [DataType(DataType.Text)]
        [Required()]
        [MaxLength(100)]
        public String FirstName { get; set; }

        /// <summary>
        /// Gets or sets the family name for the party.
        /// </summary>
        [DataType(DataType.Text)]
        [Required()]
        [MaxLength(100)]
        public String LastName { get; set; }

        /// <summary>
        /// Gets or sets the email address for the current party.
        /// </summary>
        [DataType(DataType.EmailAddress)]
        [EmailAddress()]
        public String Email { get; set; }

        /// <summary>
        /// Gets or sets a phone number for the current party.
        /// </summary>
        [DataType(DataType.PhoneNumber)]
        [PhoneAttribute()]
        public String PhoneNumber { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Converts the current model into a formatted display name.
        /// </summary>
        /// <returns>The formatted display name.</returns>
        public virtual String ToDisplayName()
        {
            return Accounting.PartyExtensions.BuildCompositeName(this.FirstName, this.LastName, this.BusinessName).ToTitleCase();
        }

        #endregion
    }
}