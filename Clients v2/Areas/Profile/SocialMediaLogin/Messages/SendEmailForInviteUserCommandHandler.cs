using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AccurateAppend.Core;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.Messaging;
using AccurateAppend.Operations.Contracts;
using AccurateAppend.Security;
using AccurateAppend.Websites.Clients.Messages.Accounts;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Profile.SocialMediaLogin.Messages
{
    public class SendEmailForInviteUserCommandHandler:IHandleMessages<InviteUserCommand>
    {
        #region Fields

        private readonly ISessionContext dataContext;

        #endregion

        #region Constructor

        public SendEmailForInviteUserCommandHandler(ISessionContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));

            this.dataContext = dataContext;
        }

        #endregion

        #region IHandleMessages<InviteUserCommand> Members

        /// <inheritdoc />
        public virtual async Task Handle(InviteUserCommand message, IMessageHandlerContext context)
        {
            Validator.ValidateObject(message, new ValidationContext(message));

            var userId = message.UserId;
            var emailAddress = message.EmailAddress;

            using (context.Alias())
            {
                using (var uow = this.dataContext.CreateScope(ScopeOptions.AutoCommit))
                {
                    var user = await this.dataContext
                        .SetOf<User>()
                        .FirstAsync(u => u.Id == userId)
                        .ConfigureAwait(false);

                    var invite = new InvitedLogon(user, emailAddress);

                    this.dataContext.SetOf<InvitedLogon>().Add(invite);

                    await uow.CommitAsync().ConfigureAwait(false);

                    var email = await EmailFactory.SendInvite(emailAddress, user.UserName, invite.ExpirationDate, $"https://clients.accurateappend.com/Public/Invite/Index/{invite.Id}").ConfigureAwait(false);

                    var command = new SendEmailCommand(email.ToMail(NullCompression.Instance, NullReverseFileProxyFactory.Instance)) {Track = true};
                    await context.Send(command).ConfigureAwait(false);
                }
            }
        }

        #endregion
    }
}