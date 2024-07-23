using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.Operations.Contracts;
using AccurateAppend.Security;
using Atp;
using Castle.Core.Resource;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Authentication.ResetPassword.Messages
{
    /// <summary>
    /// Handler for the <see cref="SendPasswordUpdateRequestCommand"/> bus event to reset instructions email.
    /// </summary>
    /// <remarks>
    /// Responds to a message by matching the message to a client and then enqueuing an email message about the request
    /// containing reset instructions.
    /// </remarks>
    public class SendPasswordUpdateRequestCommandHandler : IHandleMessages<SendPasswordUpdateRequestCommand>
    {
        #region Fields

        private readonly ISessionContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SendPasswordUpdateRequestCommandHandler"/> class.
        /// </summary>
        /// <param name="dataContext">The required <see cref="ISessionContext"/> component.</param>
        public SendPasswordUpdateRequestCommandHandler(ISessionContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
        }

        #endregion

        #region IHandleMessages<SendPasswordUpdateRequestCommand> Members

        /// <inheritdoc />
        public virtual async Task Handle(SendPasswordUpdateRequestCommand message, IMessageHandlerContext context)
        {
            var user = await this.dataContext.SetOf<User>()
                .Include(u => u.Application)
                .Where(r => r.UserName == message.UserName && !r.IsLockedOut)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            if (user == null) return;

            // Don't send emails for AA users
            if (user.Application.Id == WellKnownIdentifiers.AdminId) return;

            using (var uow = this.dataContext.CreateScope(ScopeOptions.AutoCommit))
            {
                var passwordResetRequest = user.CreateResetRequest();

                this.dataContext.SetOf<PasswordResetRequest>().Add(passwordResetRequest);

                // email user link
                var email = await HtmlFormatStrategy.Create(passwordResetRequest);

                var command = new SendEmailCommand(email);
                command.MessageKey = user.Id;
                command.Track = true;

                await context.Send(command).ConfigureAwait(false);
                await uow.CommitAsync().ConfigureAwait(false);
            }
        }

        #endregion

        #region Nested Types
        
        protected static class HtmlFormatStrategy
        {
            public static async Task<MailMessage> Create(PasswordResetRequest request)
            {
                var user = request.Logon;
                var site = SiteCache.Cache.First(s => s.ApplicationId == WellKnownIdentifiers.AccurateAppendId);

                using (var body = new TemplateEngine())
                {
                    var resource = GetBodyResourceForApplication();
                    using (var reader = resource.GetStreamReader())
                    {
                        body.LoadFromString(await reader.ReadToEndAsync().ConfigureAwait(false));
                    }

                    body.SetValue("email", request.Logon.EmailAddress);
                    body.SetValue("publickey", request.PublicKey.ToString());
                    body.SetValue("signature", await GetSignature(request.Logon.Application.Id).ConfigureAwait(false));
                    body.SetValue("telephone", site.PrimaryPhone);

                    var message = new MailMessage(site.MailboxSupport, user.UserName);
                    message.Subject = "Password Reset Instructions";
                    message.Body = body.Run();
                    message.IsBodyHtml = true;

                    return message;
                }
            }

            private static FileResource GetBodyResourceForApplication()
            {
                var resource = new FileResource(@"EmailTemplates\PasswordReset.html.config");
                return resource;
            }

            private static FileResource GetSignatureResourceForApplication()
            {
                var resource = new FileResource(@"EmailTemplates\Signature.html.config");
                return resource;
            }

            private static async Task<String> GetSignature(Guid applicationId)
            {
                var siteinfo = SiteCache.Cache.FirstOrDefault(s => s.ApplicationId == applicationId) ??
                               SiteCache.Cache.First(s => s.ApplicationId == WellKnownIdentifiers.AccurateAppendId);

                using (var dt = new TemplateEngine())
                {
                    var resource = GetSignatureResourceForApplication();
                    using (var reader = resource.GetStreamReader())
                    {
                        dt.LoadFromString(await reader.ReadToEndAsync().ConfigureAwait(false));
                    }

                    dt.SetValue("SiteAddress", siteinfo.Address);
                    dt.SetValue("SiteCity", siteinfo.City);
                    dt.SetValue("SiteState", siteinfo.State);
                    dt.SetValue("SiteZip", siteinfo.Zip);
                    dt.SetValue("SitePhone1", siteinfo.PrimaryPhone.ToString());
                    dt.SetValue("SitePhone2", siteinfo.SecondaryPhone.ToString());
                    dt.SetValue("Domain", siteinfo.Website);
                    dt.SetValue("SiteName", siteinfo.Title);
                    dt.SetValue("Mailbox_Support", siteinfo.MailboxSupport);

                    return dt.Run();
                }
            }
        }

        #endregion
    }
}