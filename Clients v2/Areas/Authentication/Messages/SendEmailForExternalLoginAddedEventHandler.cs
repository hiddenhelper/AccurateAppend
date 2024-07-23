using System.Threading.Tasks;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Operations.Contracts;
using AccurateAppend.Websites.Clients.Messages.Accounts;
using AccurateAppend.Websites.Clients.Security;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Authentication.Messages
{
    /// <summary>
    /// Handler for the <see cref="ExternalLoginAddedEvent"/> bus event to create a notification email.
    /// </summary>
    /// <remarks>
    /// Responds to a message by creating an Accurate Append branded email.
    /// 
    /// The mail that is created will be enqueued for subsequent delivery but is not send as part of this handler.
    /// </remarks>
    public class SendEmailForExternalLoginAddedEventHandler:IHandleMessages<ExternalLoginAddedEvent>
    {
        #region IHandleMessages<ExternalLoginAddedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(ExternalLoginAddedEvent message, IMessageHandlerContext context)
        {
            using (SecurityHelper.Alias(WellKnownIdentifiers.SystemIdentity))
            {
                var userName = message.UserName;
                var displayName = message.DisplayName;
                var provider = message.Source;

                var email = await EmailFactory.SendNewAccountNotifcationToClient(userName, provider, displayName, WellKnownIdentifiers.AccurateAppendId).ConfigureAwait(false);

                var command = new SendEmailCommand(email.ToMail(NullCompression.Instance, NullReverseFileProxyFactory.Instance)) {Track = true};

                await context.Send(command).ConfigureAwait(false);
            }
        }

        #endregion
    }
}