using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using AccurateAppend.Accounting;
using AccurateAppend.Data;
using AccurateAppend.ZenDesk.Sales.Contacts;
using NServiceBus;
using Contact = AccurateAppend.ZenDesk.Sales.Contacts.Contact;

namespace AccurateAppend.Websites.Clients.Security
{
    /// <summary>
    /// Handler for the Public Application authentication events.
    ///
    /// <see cref="PublicLogonEvent"/> : Upsert a new ZenSell Contact with Name, Email, and PublicKey.
    /// </summary>
    public class ZenSellHandler : IHandleMessages<PublicLogonEvent>
    {
        #region Fields

        private readonly ISessionContext dataContext;
        private readonly IContactsService contactsService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ZenSellHandler"/> class.
        /// </summary>
        /// <param name="dataContext">The data access component that provides access to the security context.</param>
        /// <param name="contactsService">The <see cref="IContactsService"/> providing access to ZenSell data.</param>
        public ZenSellHandler(ISessionContext dataContext, IContactsService contactsService)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            if (contactsService == null) throw new ArgumentNullException(nameof(contactsService));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
            this.contactsService = contactsService;
        }

        #endregion

        #region IHandleMessages<PublicLogonEvent> Members

        /// <inheritdoc />
        public async Task Handle(PublicLogonEvent message, IMessageHandlerContext context)
        {
            var userName = message.UserName;

            var user = await this.dataContext
                .SetOf<Client>()
                .Where(c => c.Logon.UserName == userName)
                .Select(c => new {c.Logon.UserName, UserId = c.Logon.Id, c.FirstName, c.LastName})
                .FirstAsync()
                .ConfigureAwait(false);

            var contact = new Contact();
            contact.FirstName = user.FirstName;
            contact.LastName = user.LastName;
            contact.Email = user.UserName;
            contact.CustomFields.PublicKey = user.UserId.ToString();

            await this.contactsService.Upsert(contact);
        }

        #endregion
    }
}