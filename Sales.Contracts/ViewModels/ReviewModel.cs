
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace AccurateAppend.Sales.Contracts.ViewModels
{
    /// <summary>
    /// A viewmodel representation of a new <see cref="DealBinder"/> review operation.
    /// </summary>
    public class ReviewViewModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReviewViewModel"/> class.
        /// </summary>
        public ReviewViewModel()
        {
            this.SendTo = new Collection<String>();
            this.BccTo = new Collection<String>();
            this.Files = new Collection<File>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier of the current model.
        /// </summary>
        [Display(Name = "Deal Id")]
        [Required()]
        public Int32 Id { get; set; }

        /// <summary>
        /// Gets or sets the email subject the bill will be sent with.
        /// </summary>
        /// <value>The email subject that the bill will be sent with.</value>
        [DataType(DataType.Text, ErrorMessage = "*")]
        [Display(Name = "Subject")]
        [Required()]
        public String Subject { get; set; }

        /// <summary>
        /// Gets or sets the email that the bill will be sent from.
        /// </summary>
        /// <value>The email that the bill will be sent from.</value>
        [DataType(DataType.Text, ErrorMessage = "*")]
        [Display(Name = "From")]
        [Required()]
        public String SendFrom { get; set; }

        /// <summary>
        /// Gets the emails that the bill will be sent to.
        /// </summary>
        /// <value>The emails that the bill will be sent to.</value>
        [Display(Name = "To")]
        public ICollection<String> SendTo { get; private set; }


        /// <summary>
        /// Gets the emails that the bill will be bcc'd to.
        /// </summary>
        /// <value>The emails that the bill will be bcc'd to.</value>
        [Display(Name = "Bcc")]
        public ICollection<String> BccTo { get; private set; }

        /// <summary>
        /// Gets or sets the description value for the current model.
        /// </summary>
        [DataType(DataType.Text)]
        [Display(Name = "Description")]
        [Required()]
        [MaxLength(500)]
        public String Description { get; set; }

        /// <summary>
        /// Gets the list of files that are included in this bill.
        /// </summary>
        /// <value>The list of files that are included in this bill.</value>
        public ICollection<File> Files { get; private set; }

        /// <summary>
        /// Gets or sets the bill message content.
        /// </summary>
        /// <value>The bill message content.</value>
        [DataType(DataType.MultilineText, ErrorMessage = "*")]
        [Display(Name = "Body")]
        [Required()]
        public String Body { get; set; }

        /// <summary>
        /// Indicates whether the body is html content or not.
        /// </summary>
        /// <value>True if the content is html; otherwise false.</value>
        public Boolean IsHtml { get; set; }

        /// <summary>
        /// Gets the identifier of the user that owns the deal.
        /// </summary>
        public Guid UserId { get; set; }

        #endregion
    }
}