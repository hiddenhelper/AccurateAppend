using System;
using System.Linq;
using System.Net.Mail;
using System.Text;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Security;

namespace AccurateAppend.Websites.Clients.Areas.Public.File
{
    public static class EmailFactory
    {
        public static MailMessage FileUploadNotifcation(String emailAddress, Guid userid, Guid applicationId, String filename)
        {
            var siteinfo = SiteCache.Cache.FirstOrDefault(s => s.ApplicationId == applicationId) ??
                           SiteCache.Cache.First(s => s.ApplicationId == WellKnownIdentifiers.AccurateAppendId);

            var body = new StringBuilder();
            body.AppendLine($"New file uploaded by user {emailAddress}");
            body.AppendLine();
            body.AppendLine($"File name: {filename}");
            body.AppendLine();
            body.AppendLine($"https://admin.accurateappend.com/Users/Detail?userid={userid}");
            var message = new MailMessage(siteinfo.MailboxSupport, siteinfo.MailboxSupport);
            message.Subject = $"New file uploaded by user - {emailAddress}";
            message.Body = body.ToString();

            return message;
        }
    }
}