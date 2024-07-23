using System;
using System.Threading.Tasks;
using AccurateAppend.Accounting;
using AccurateAppend.CustomerManagement.Contracts;
using AccurateAppend.ZenDesk.Sales.Contacts;
using NServiceBus;
using Contact = AccurateAppend.ZenDesk.Sales.Contacts.Contact;

namespace AccurateAppend.Websites.Clients.Messages.Accounts
{
    /// <summary>
    /// Handler for the <see cref="AccountCreatedEvent"/> bus event to issue a <see cref="SyncToZenSell"/> message when a new account is created.
    /// </summary>
    public class CreateZenSellContactForAccountCreatedEvent : IHandleMessages<AccountCreatedEvent>, IHandleMessages<CreateZenSellContactForAccountCreatedEvent.SyncToZenSell>
    {
        #region Fields

        private readonly IContactsService service;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateZenSellContactForAccountCreatedEvent"/> class.
        /// </summary>
        /// <param name="service">The <see cref="IContactsService"/> providing access to ZenSell data.</param>
        public CreateZenSellContactForAccountCreatedEvent(IContactsService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            this.service = service;
        }

        #endregion

        #region IHandleMessages<AccountCreatedEvent> Members

        /// <inheritdoc />
        public Task Handle(AccountCreatedEvent message, IMessageHandlerContext context)
        {
            var contact = new SyncToZenSell
            {
                FirstName = message.FirstName,
                LastName = message.LastName,
                Email = message.UserName,
                CustomFields = {PublicKey = message.UserId.ToString()},
                Phone = message.PrimaryPhone
            };

            return context.SendLocal(contact);
        }

        #endregion

        #region Nested Type

        /// <summary>
        /// Not to be directly used.
        /// </summary>
        public class SyncToZenSell : Contact, ICommand
        {

        }

        #endregion

        #region IHandleMessages<SyncToZenSell> Members

        /// <inheritdoc />
        public virtual async Task Handle(SyncToZenSell message, IMessageHandlerContext context)
        {
            var contact = new Contact()
            {
                FirstName = message.FirstName ?? PartyExtensions.UnknownFirstName,
                LastName = message.LastName ?? PartyExtensions.UnknownLastName,
                Email = message.Email,
                CustomFields = {PublicKey = message.CustomFields.PublicKey}
            };

            contact = await service.Upsert(contact).ConfigureAwait(false);

            Console.WriteLine($"Contact Handled: {contact.Id}/{message.Email}");
        }

        #endregion
    }
}