using System;
using System.Linq;
using System.Threading.Tasks;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Security;
using Atp;
using Castle.Core.Resource;

namespace AccurateAppend.Websites.Clients.Areas.Public.NewClientRegistration
{
    public static class EmailFactory
    {
        public static async Task<Message> SendNewAccountNotifcationToClient(String firstName, String email, Guid applicationId)
        {
            var siteinfo = SiteCache.Cache.FirstOrDefault(s => s.ApplicationId == applicationId) ??
                           SiteCache.Cache.First(s => s.ApplicationId == WellKnownIdentifiers.AccurateAppendId);

            using (var body = new TemplateEngine())
            {
                var resource = GetBodyResourceForApplication();
                using (var reader = resource.GetStreamReader())
                {
                    body.LoadFromString(await reader.ReadToEndAsync().ConfigureAwait(false));
                }

                body.SetValue("firstname", firstName);
                body.SetValue("signature", await GetSignature(applicationId).ConfigureAwait(false));

                var message = new Message(siteinfo.MailboxSupport)
                {
                    Subject = "Registration request received",
                    Body = body.Run(),
                    IsHtml = false
                };
                message.SendTo.Add(email);
                message.BccTo.Add("support@accurateappend.com");

                message.ReadyToPublish();

                return message;
            }
        }

        private static FileResource GetBodyResourceForApplication()
        {
            var resource = new FileResource(@"EmailTemplates\Welcome_Classic.Text.config");
            return resource;
        }

        private static FileResource GetSignatureResourceForApplication()
        {
            var resource = new FileResource(@"EmailTemplates\Signature.config");
            return resource;
        }

        private static async Task<String> GetSignature(Guid applicationId)
        {
            var siteinfo = SiteCache.Cache.First(s => s.ApplicationId == applicationId);

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
}