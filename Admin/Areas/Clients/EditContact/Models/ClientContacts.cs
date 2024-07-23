using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using AccurateAppend.Core.Collections.Generic;

namespace AccurateAppend.Websites.Admin.Areas.Clients.EditContact.Models
{
    /// <summary>
    /// A view model for a client contacts.
    /// </summary>
    [DebuggerDisplay("{" + nameof(Id) + ("}:{" + nameof(Name) + "}, Count={Contacts.Count}"))]
    public class ClientContacts
    {
        #region Fields

        private readonly IList<ContactModel> contacts;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientContacts"/> class.
        /// </summary>
        public ClientContacts()
        {
            this.contacts = new List<ContactModel>();
            this.Name = String.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientContacts"/> class.
        /// </summary>
        public ClientContacts(AccurateAppend.Accounting.Client client) : this()
        {
            if (client == null) return;

            this.Id = client.Id ?? 0;
            this.Name = client.CompositeName;
            this.UserId = client.Logon.Id;
            client.Contacts.ForEach(c => this.contacts.Add(new ContactModel(c)));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier of the current model.
        /// </summary>
        [Display(Name = "Client ID")]
        [Required()]
        public Int32 Id { get; set; }

        /// <summary>
        /// Gets the name of the current model.
        /// </summary>
        [Display(Name = "Client Name")]
        [Required()]
        public String Name { get; set; }

        /// <summary>
        /// Gets the contacts of the current model.
        /// </summary>
        public IList<ContactModel> Contacts => this.contacts;

        /// <summary>
        /// Gets the identifier of the client the contacts are for.
        /// </summary>
        public Guid UserId { get; set; }

        #endregion
    }
}