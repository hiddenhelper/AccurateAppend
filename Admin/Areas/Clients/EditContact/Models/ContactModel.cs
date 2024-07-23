using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

namespace AccurateAppend.Websites.Admin.Areas.Clients.EditContact.Models
{
    /// <summary>
    /// A view model for a single contact entry.
    /// </summary>
    /// <remarks>Maps to <see cref="AccurateAppend.Accounting.Contact"/> object.</remarks>
    [DebuggerDisplay("{" + nameof(EmailAddress) + ("}, Jobs={" + nameof(ShouldNotify) + ("}, Bill={" + nameof(BillTo) + "}")))]
    public class ContactModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactModel"/> class.
        /// </summary>
        public ContactModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactModel"/> class.
        /// </summary>
        /// <param name="contact">The <see cref="AccurateAppend.Accounting.Contact"/> to use as a prototype for this view model.</param>
        public ContactModel(AccurateAppend.Accounting.Contact contact) : this()
        {
            if (contact == null) return;

            this.Id = contact.Id ?? 0;
            this.EmailAddress = contact.EmailAddress;
            this.ShouldNotify = contact.NotifyJobs;
            this.SubmitJobs = contact.SubmitJobs;
            this.BillTo = contact.Billing;
            this.IsAdmin = contact.Admin;
            this.Name = contact.Name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier of the current model.
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// Gets the email address of the contact.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Valid email address is required.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Valid email address is required.")]
        [Display(Name = "Email Address")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Email address is not correctly formatted.")]
        [MaxLength(50)]
        public String EmailAddress { get; set; }

        /// <summary>
        /// Gets the name, if any, of the contact.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Indicates whether job notifications are sent to the contact.
        /// </summary>
        public Boolean ShouldNotify { get; set; }

        /// <summary>
        /// Indicates whether the contact can submit SMTP jobs for the client.
        /// </summary>
        public Boolean SubmitJobs { get; set; }

        /// <summary>
        /// Indicates whether bills are sent to the contact.
        /// </summary>
        public Boolean BillTo { get; set; }

        /// <summary>
        /// Indicates whether this contact can be used for account queries and management.
        /// </summary>
        public Boolean IsAdmin { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Consolidates all the provided contacts into a unified instance grouped by email address.
        /// </summary>
        /// <remarks>
        /// Any contact identifiers are expunged from the output.
        /// </remarks>
        /// <param name="contacts">The contacts to consolidate.</param>
        /// <returns>The consolidated list.</returns>
        public static IEnumerable<ContactModel> Consolodate(IEnumerable<ContactModel> contacts)
        {
            if (contacts == null) yield break;

            contacts = contacts.Where(c => c != null);

            foreach (var group in contacts.GroupBy(c => c.EmailAddress, StringComparer.OrdinalIgnoreCase))
            {
                var billTo = group.Any(i => i.BillTo);
                var shouldNotify = group.Any(i => i.ShouldNotify);
                var submitJobs = group.Any(i => i.SubmitJobs);
                var isAdmin = group.Any(i => i.IsAdmin);

                var contact = new ContactModel
                {
                    BillTo = billTo,
                    ShouldNotify = shouldNotify,
                    SubmitJobs = submitJobs,
                    IsAdmin = isAdmin,
                    EmailAddress = group.Key,
                    Name = group.OrderByDescending(c => c.Name?.Length).First().Name
                };
                yield return contact;
            }
        }

        #endregion
    }
}