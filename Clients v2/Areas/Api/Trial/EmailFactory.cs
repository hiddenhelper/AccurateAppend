using System;
using System.Linq;
using System.Threading.Tasks;
using AccurateAppend.Accounting;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Security;
using Atp;
using Castle.Core.Resource;

namespace AccurateAppend.Websites.Clients.Areas.Api.Trial
{
    public static class EmailFactory
    {
        private static FileResource GetBodyResourceForApplication()
        {
            var resource = new FileResource(@"EmailTemplates\ApiTrialWelcomeMessage.config");
            return resource;
        }

        private static FileResource GetSignatureResourceForApplication()
        {
            var resource = new FileResource(@"EmailTemplates\Signature.html.config");
            return resource;
        }

        private static async Task<String> GetSignature()
        {
            var siteinfo = SiteCache.Cache.First(s => s.ApplicationId == WellKnownIdentifiers.AccurateAppendId);

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

        public static async Task<Message> ApiTrialWelcomMessage(String email, Lead lead)
        {
            var site = SiteCache.Cache.FirstOrDefault(s => s.ApplicationId == lead.Application.Id) ??
                       SiteCache.Cache.First(s => s.ApplicationId == WellKnownIdentifiers.AccurateAppendId);

            using (var body = new TemplateEngine())
            {
                var resource = GetBodyResourceForApplication();
                using (var reader = resource.GetStreamReader())
                {
                    body.LoadFromString(await reader.ReadToEndAsync().ConfigureAwait(false));
                }

                body.SetValue("accessid", lead.Trial.AccessId);
                body.SetValue("signature", await GetSignature().ConfigureAwait(false));
                body.SetValue("telephone", site.PrimaryPhone);

                var message = new Message(site.MailboxSupport)
                {
                    Subject = "Welcome to Accurate Append",
                    Body = body.Run(),
                    IsHtml = true
                };
                message.SendTo.Add(email);

                message.ReadyToPublish();

                return message;
            }
        }
    }
}