using System.Threading.Tasks;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.CustomerManagement.Contracts;
using DomainModel.Messages;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Messages.Accounts
{
    /// <summary>
    /// Handler for the <see cref="AccountCreatedEvent"/> bus event to issue a <see cref="ResetClientHistorgramCommand"/> message when a new account is created.
    /// </summary>
    public class CreateHistogramForAccountCreatedEvent : IHandleMessages<AccountCreatedEvent>
    {
        #region IHandleMessages<AccountCreatedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(AccountCreatedEvent message, IMessageHandlerContext context)
        {
            var userId = message.UserId;

            using (SecurityHelper.Alias(WellKnownIdentifiers.SystemIdentity))
            {
                var command = new ResetClientHistorgramCommand() {UserId = userId};
                await context.Send(command).ConfigureAwait(false);
            }
        }

        #endregion
    }
}