using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using AccurateAppend.ZenDesk.Support;

namespace AccurateAppend.Websites.Admin.Areas.Tickets.CreateTicket.Models
{
    /// <summary>
    /// View model for creating a new <see cref="ZenDesk.Contracts.Support.CreateTicketCommand"/> .
    /// </summary>
    [Serializable()]
    public class CreateTicketViewModel : IValidatableObject
    {
        /// <summary>
        /// Constructor for <see cref="CreateTicketViewModel"/>
        /// </summary>
        public CreateTicketViewModel()
        {
            this.Recipients = new List<CheckBoxes>();
        }

        /// <summary>
        /// Userid of client
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Recipients to who the ticket will go.
        /// </summary>
        public IList<CheckBoxes> Recipients { get; }
        
        /// <summary>
        /// Other recipients who are not included in the accounts contacts to include. Uses a comma
        /// delimited list of email addresses.
        /// </summary>
        public String OtherRecipients { get; set; }

        /// <summary>
        /// Ticket type. 
        /// </summary>
        [Required()]
        public TicketType Type { get; set; }

        /// <summary>
        /// Ticket priority
        /// </summary>
        [Required()]
        public TicketPriority Priority { get; set; }

        /// <summary>
        /// The details of the ticket
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public String Comments { get; set; }

        /// <summary>
        /// The subject of the ticket
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public String Subject { get; set; }
        
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
            // validate subject & comments
            if (String.IsNullOrWhiteSpace(this.Subject) && String.IsNullOrWhiteSpace(this.Comments)) yield return new ValidationResult("Subject and Comments must be supplied.", new[] { nameof(this.Subject), nameof(this.Subject), nameof(this.Comments) });

            // validate other recipients
            var otherRecipients = this.OtherRecipients ?? String.Empty;
            foreach (var otherRecipient in otherRecipients.Split(','))
            {
                if (String.IsNullOrEmpty(otherRecipient)) continue;
                var re = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
                if (!re.IsMatch(otherRecipient)) yield return new ValidationResult("Other Recipient email address is not correctly formatted.", new[] { nameof(this.OtherRecipients) });
            }

            // validate recipients
            if (String.IsNullOrEmpty(otherRecipients) && !this.Recipients.Any(a => a.Checked)) yield return new ValidationResult("Please select at least one recipient", new[] { nameof(this.Recipients) });
        }

        #endregion
    }
}