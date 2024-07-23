using System;
using System.ComponentModel.DataAnnotations;

namespace AccurateAppend.Sales.Contracts.ViewModels
{
    /// <summary>
    /// A viewmodel representation of a new <see cref="FileAttachment"/> review operation.
    /// </summary>
    public class File
    {
        /// <summary>
        /// Gets or sets the name of the file attachment.
        /// </summary>
        /// <value>The name of the file attachment.</value>
        [DataType(DataType.Text, ErrorMessage = "*")]
        [Required()]
        public String FileName { get; set; }

        /// <summary>
        /// Gets or sets the name the file attachment should be sent as.
        /// </summary>
        /// <value>The name the file attachment should be sent as.</value>
        [DataType(DataType.Text, ErrorMessage = "*")]
        public String SendFileName { get; set; }
    }
}