using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AccurateAppend.Websites.Admin.Areas.Sales.CreateBill.Models
{
    /// <summary>
    /// Contains the content of the bill message to draft.
    /// </summary>
    [Serializable()]
    public class BillMessage
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BillMessage"/> class.
        /// </summary>
        public BillMessage()
        {
            this.SendTo = new HashSet<String>(StringComparer.OrdinalIgnoreCase);
            this.BccTo = new HashSet<String>(StringComparer.OrdinalIgnoreCase);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the email address to send the bill message from.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.EmailAddress, ErrorMessage = "Valid email address is required.")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Your email address is not correctly formatted.")]
        [MaxLength(254)]
        public String SendFrom { get; set; }

        /// <summary>
        /// Indicate whether the content <see cref="Body"/> contains HTML or raw text data.
        /// </summary>
        public Boolean IsHtml { get; set; }

        /// <summary>
        /// Gets the set of email addresses to send the bill content to.
        /// </summary>
        public ICollection<String> SendTo { get; }

        /// <summary>
        /// Gets the set of email addresses to blind copy the bill content to.
        /// </summary>
        public ICollection<String> BccTo { get; }

        /// <summary>
        /// Gets or sets the subject of the bill message.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [MaxLength(254)]
        public String Subject { get; set; }

        /// <summary>
        /// Gets or sets the content body of the bill message.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public String Body { get; set; }

        #endregion
    }
}