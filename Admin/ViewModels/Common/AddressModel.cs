using System;
using System.ComponentModel.DataAnnotations;

namespace AccurateAppend.Websites.Admin.ViewModels.Common
{
    /// <summary>
    /// Common view model element for representing a party with a postal address.
    /// </summary>
    public class AddressModel
    {
        #region Properties

        /// <summary>
        /// Gets the street component for the current address.
        /// </summary>
        [DataType(DataType.Text)]
        [Required()]
        [MaxLength(100)]
        public String Street { get; set; }

        /// <summary>
        /// Gets the city component for the current address.
        /// </summary>
        [DataType(DataType.Text)]
        [Required()]
        [MaxLength(100)]
        public String City { get; set; }

        /// <summary>
        /// Gets the state component for the current address.
        /// </summary>
        [DataType(DataType.Text)]
        [Required()]
        [MaxLength(2)]
        public String State { get; set; }

        /// <summary>
        /// Gets the postal code component for the current address.
        /// </summary>
        [DataType(DataType.Text)]
        [Required()]
        [MaxLength(10)]
        public String PostalCode { get; set; }

        /// <summary>
        /// Gets the optional name of the country component for the current address.
        /// </summary>
        [DataType(DataType.Text)]
        [MaxLength(50)]
        public String Country { get; set; }

        #endregion
    }
}