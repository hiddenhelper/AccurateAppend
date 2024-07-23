using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using AccurateAppend.Accounting;
using AccurateAppend.ChargeProcessing.Contracts;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.Messaging;
using AccurateAppend.Security;
using Atp;
using Castle.Core.Resource;
using EventLogger;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Profile.Card.Messaging
{
    /// <summary>
    /// Handler for the <see cref="PaymentProfileCreatedEvent"/> bus event to send client notification emails.
    /// </summary>
    /// <remarks>
    /// Responds to a message by enqueuing an email message about the event.
    /// </remarks>
    public class SendEmailForCardUpdatedEventHandler : IHandleMessages<PaymentProfileCreatedEvent>
    {
        #region Fields

        private readonly ISessionContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SendEmailForCardUpdatedEventHandler"/> class.
        /// </summary>
        /// <param name="dataContext">The required <see cref="ISessionContext"/> component.</param>
        public SendEmailForCardUpdatedEventHandler(ISessionContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
        }

        #endregion

        #region IHandleMessages<PaymentProfileCreatedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(PaymentProfileCreatedEvent message, IMessageHandlerContext context)
        {
            var id = context.DefaultCorrelation();
            var updatedBy = context.InitiatingUserId();

            using (new Correlation(id))
            { 
                try
                {
                    // Don't send internal alerts when updated by AA users
                    if (message.UserId != updatedBy) return;

                    var clients = this.dataContext.SetOf<Client>().Where(c => c.Logon.Id == message.UserId);
                    var client = await clients.Select(c =>
                            new ClientData
                            {
                                UserId = c.Logon.Id,
                                ApplicationId = c.Logon.Application.Id,
                                ApplicationName = c.Logon.Application.Details.Title,
                                FirstName = c.FirstName,
                                LastName = c.LastName,
                                DefaultEmail = c.DefaultEmail,
                                SupportAddress = c.Logon.Application.Details.Mail.SupportAddress,
                                DisplayValue = message.CardDisplay,
                            })
                        .SingleAsync()
                        .ConfigureAwait(false);

                    // Don't send emails for AA users
                    if (client.ApplicationId == WellKnownIdentifiers.AdminId) return;

                    Func<ClientData, Task<MailMessage>> factory;
                    if (client.ApplicationId == WellKnownIdentifiers.TwentyTwentyId)
                    {
                        factory = TextFormatStrategy.Create;
                    }
                    else
                    {
                        factory = HtmlFormatStrategy.Create;
                    }

                    var email = new Message(await factory(client).ConfigureAwait(false));
                    email.BccTo.Add(client.SupportAddress);
                    email.ReadyToPublish();

                    using (var uow = this.dataContext.CreateScope(ScopeOptions.AutoCommit))
                    {
                        this.dataContext.SetOf<Message>().Add(email);
                        await uow.CommitAsync().ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    if (Debugger.IsAttached) Debugger.Break();
                    Logger.LogEvent(ex, Severity.High, Core.Definitions.Application.Clients, description: $"Failure on {nameof(SendEmailForCardUpdatedEventHandler)}");

                    throw;
                }
            }
        }

        #endregion

        #region Nested Types

        protected class ClientData
        {
            public Guid UserId { get; set; }

            public Guid ApplicationId { get; set; }

            public String ApplicationName { get; set; }

            public String FirstName { get; set; }

            public String LastName { get; set; }

            public String DefaultEmail { get; set; }

            public String SupportAddress { get; set; }

            public String DisplayValue { get; set; }
        }

        protected static class HtmlFormatStrategy
        {
            public static async Task<MailMessage> Create(ClientData client)
            {
                var siteinfo = SiteCache.Cache.FirstOrDefault(s => s.ApplicationId == client.ApplicationId) ??
                               SiteCache.Cache.First(s => s.ApplicationId == WellKnownIdentifiers.AccurateAppendId);

                using (var body = new TemplateEngine())
                {
                    var resource = GetBodyResourceForApplication();
                    using (var reader = resource.GetStreamReader())
                    {
                        body.LoadFromString(await reader.ReadToEndAsync().ConfigureAwait(false));
                    }

                    body.SetValue("firstname", PartyExtensions.BuildCompositeName(client.FirstName, client.LastName, client.DefaultEmail));
                    body.SetValue("signature", await GetSignature(client.ApplicationId).ConfigureAwait(false));
                    body.SetValue("CardNumber", client.DisplayValue);
                    body.SetValue("telephone", siteinfo.PrimaryPhone);

                    var email = new MailMessage(client.SupportAddress, client.DefaultEmail)
                    {
                        Sender = new MailAddress(client.SupportAddress),
                        Subject = "Payment information updated",
                        Body = body.Run(),
                        IsBodyHtml = true
                    };

                    return email;
                }
            }

            private static FileResource GetBodyResourceForApplication()
            {
                var resource = new FileResource(@"EmailTemplates\PaymentInfoUpdated.html.config");
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

        protected static class TextFormatStrategy
        {
            public static async Task<MailMessage> Create(ClientData client)
            {
                using (var body = new TemplateEngine())
                {
                    var resource = GetBodyResourceForApplication();
                    using (var reader = resource.GetStreamReader())
                    {
                        body.LoadFromString(await reader.ReadToEndAsync().ConfigureAwait(false));
                    }

                    body.SetValue("firstname", PartyExtensions.BuildCompositeName(client.FirstName, client.LastName, client.DefaultEmail));
                    body.SetValue("signature", await GetSignature(client.ApplicationId).ConfigureAwait(false));
                    body.SetValue("CardNumber", client.DisplayValue);

                    var email = new MailMessage(client.SupportAddress, client.DefaultEmail)
                    {
                        Sender = new MailAddress(client.SupportAddress),
                        Subject = "Payment information updated",
                        Body = body.Run(),
                        IsBodyHtml = false
                    };

                    return email;
                }
            }

            private static FileResource GetBodyResourceForApplication()
            {
                var resource = new FileResource(@"EmailTemplates\PaymentInfoUpdated.config");
                return resource;
            }

            private static FileResource GetSignatureResourceForApplication()
            {
                var resource = new FileResource(@"EmailTemplates\Signature.config");
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