using System;
using System.Threading.Tasks;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Core.Utilities;
using AccurateAppend.CustomerManagement.Contracts;
using AccurateAppend.Security;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Messages.Accounts
{
    /// <summary>
    /// Handler for the <see cref="AccountCreatedEvent"/> bus event to reset instructions email.
    /// </summary>
    /// <remarks>
    /// Responds to a message by examining the source channel.
    /// If NationBuilder, the message is created and provides download links to the NationBuilder processing options.
    /// If Self service, the same message is create without attachments.
    /// In sales handled signup, a legacy text email is created.
    /// 
    /// The mail that is created will be enqueued for subsequent delivery but is not send as part of this handler.
    /// </remarks>
    public class SendWelcomeEmailForAccountCreatedEventHandler : IHandleMessages<AccountCreatedEvent>
    {
        #region IHandleMessages<AccountCreatedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(AccountCreatedEvent message, IMessageHandlerContext context)
        {
            var applicationId = message.ApplicationId;
            var userName = message.UserName;
            var firstName = message.FirstName;
            var generatedPassword = message.GeneratedPassword;
            var sourceChannel = message.Channel;

            using (SecurityHelper.Alias(WellKnownIdentifiers.SystemIdentity))
            {
                Task<Message> task;
                switch (sourceChannel)
                {
                    case AccountSourceChannel.Sales:
                        task = CreateSalesDirectedWelcome(firstName, userName, applicationId);
                        break;
                    case AccountSourceChannel.NationBuilder:
                        task = CreateNationBuilderWelcome(userName, generatedPassword, applicationId);
                        break;
                    case AccountSourceChannel.SelfService:
                    case AccountSourceChannel.LeadPhilanthropy:
                        task = CreatePublicWelcome(userName, generatedPassword, applicationId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Source: {sourceChannel} is not supported");
                }

                var email = await task.ConfigureAwait(false);

                var command = new Operations.Contracts.SendEmailCommand(email.ToMail(NullCompression.Instance, NullReverseFileProxyFactory.Instance)) { Track = true };

                await context.Send(command).ConfigureAwait(false);
            }
        }

        #endregion

        #region Helpers

        private static Task<Message> CreatePublicWelcome(String email, String password, Guid applicationId)
        {
            return Areas.Public.Signup.EmailFactory.SendNewAccountNotifcationToClient(email, password, applicationId);
        }

        private static Task<Message> CreateNationBuilderWelcome(String email, String password, Guid applicationId)
        {
            return Areas.NationBuilder.Signup.EmailFactory.SendNewAccountNotifcationToClient(email, password, applicationId);
        }

        private static Task<Message> CreateSalesDirectedWelcome(String firstName, String email, Guid applicationId)
        {
            return Areas.Public.NewClientRegistration.EmailFactory.SendNewAccountNotifcationToClient(firstName, email, applicationId);
        }

        #endregion
    }
}