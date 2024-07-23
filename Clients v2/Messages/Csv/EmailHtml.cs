using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using AccurateAppend.Core;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Sales;
using AccurateAppend.Security;
using Atp;
using Castle.Core.Resource;

namespace AccurateAppend.Websites.Clients.Messages.Csv
{
    /// <summary>
    /// Factory component used to craft email message content for a <see cref="Cart"/> component during a CSV order process.
    /// </summary>
    /// <remarks>
    /// At first this component being separate from the only caller in the application, an event handler, would seem to be odd.
    /// After all, if there's a single caller can't we just combine the types, right? However this is a bad idea from a design
    /// standpoint when you look at it from the SOLID principles. The 'S' stands for Single-Responsibility which here means we
    /// have a component that has a responsibility to create an email based on a <seealso cref="Cart"/> so its concerns are just
    /// how to acquire the template and what tokens to supply to the template system. On the other side we have an event handler
    /// that should only only be concerns with the messaging infrastructure and ontology, not about anything to do with generating
    /// email content.
    /// </remarks>
    public static class EmailHtml
    {
        #region Helpers

        private static FileResource GetResourceByName(String fileName)
        {
            var resource = new FileResource($@"EmailTemplates\{fileName}");
            return resource;
        }

        #endregion

        #region Factories

        /// <summary>
        /// Generates an email message that can be sent to a client based on a <see cref="Cart"/>.
        /// </summary>
        /// <param name="order">The <see cref="Cart"/> to create a <see cref="MailMessage"/> for.</param>
        /// <returns>A <see cref="MailMessage"/> describing the <paramref name="order"/>.</returns>
        public static async Task<MailMessage> OrderSubmitted(CsvCart order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            Contract.EndContractBlock();
            
            var site = SiteCache.Cache.FirstOrDefault(s => s.ApplicationId == order.Client.ApplicationId) ??
                       SiteCache.Cache.First(s => s.ApplicationId == WellKnownIdentifiers.AccurateAppendId);

            var email = order.Client.UserName;

            using (var body = new TemplateEngine())
            {
                var resource = GetResourceByName("OrderSubmittedCsv.html.config");
                using (var reader = resource.GetStreamReader())
                {
                    body.LoadFromString(await reader.ReadToEndAsync().ConfigureAwait(false));
                }

                body.SetValue("telephone", site.PrimaryPhone);
                body.SetValue("date", DateTime.Now.ToUserLocal().ToString(CultureInfo.InvariantCulture));
                body.SetValue("fileName", order.Name);
                body.SetValue("recordCount", $"{order.RecordCount:0,0}");
                body.SetValue("orderTotal", $"{order.QuotedTotal():C}");
                body.SetValue("orderMinimum", $"{order.OrderMinimum():C}");
                body.SetValue("lineItems", order.QuotedProducts().Select(p => new
                {
                    Title = p.Item1.GetDescription(),
                    Cost = $"{p.Item3:C}",
                    Total = $"{(p.Item2*p.Item3):C}",
                    EstMatches = $"{p.Item2:0,0}"
                }));

                var message = new MailMessage(site.MailboxSupport, email)
                {
                    Subject = $"Order Submitted - {order.Name}",
                    Body = body.Run(),
                    IsBodyHtml = true
                };

                return message;
            }
        }
        
        #endregion
    }
}