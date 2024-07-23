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

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.Order.Messages
{
    /// <summary>
    /// Handler for the <see cref="NationBuilderOrderPlacedEvent"/> bus event to send order email to client.
    /// </summary>
    /// <remarks>
    /// Responds to a message by enqueuing an email message about the event.
    /// </remarks>
    public class SendEmailForNationBuilderOrderPlacedHandler : IHandleMessages<NationBuilderOrderPlacedEvent>
    {
        #region Fields

        private readonly Sales.DataAccess.DefaultContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SendEmailForNationBuilderOrderPlacedHandler"/> class.
        /// </summary>
        /// <param name="dataContext">The required <see cref="Sales.DataAccess.DefaultContext"/> component.</param>
        public SendEmailForNationBuilderOrderPlacedHandler(Sales.DataAccess.DefaultContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
        }

        #endregion

        #region IHandleMessages<NationBuilderOrderSubmittedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(NationBuilderOrderPlacedEvent message, IMessageHandlerContext context)
        {
            // Only AA clients can integrate with NB
            var site = SiteCache.Cache.First(s => s.ApplicationId == WellKnownIdentifiers.AccurateAppendId);

            // Message isn't timely anymore so douche
            if ((DateTime.UtcNow - message.DateSubmitted).TotalDays > 1) return;

            // Order has no products so again, douche
            if (message.Products.Count == 0) return;

            var cart = await this.dataContext
                .SetOf<Sales.Cart>()
                .Include(c => c.Client)
                .SingleAsync(c => c.Id == message.CartId)
                .ConfigureAwait(false);

            var data = new
            {
                DefaultEmail = cart.Client.UserName,
                NationName = message.Slug ?? String.Empty,
                ListName = cart.Name,
                TotalRecords = cart.RecordCount
            };
            
            using (var body = new TemplateEngine())
            {
                var resource = GetBodyResourceForApplication();
                using (var reader = resource.GetStreamReader())
                {
                    body.LoadFromString(await reader.ReadToEndAsync());
                }

                var orderMinimum = message.OrderMinimum ?? 0.00m;
                var orderTotal = message.Total();

                body.SetValue("signature", await GetSignature());
                body.SetValue("nationName", data.NationName);
                body.SetValue("listName", data.ListName);
                body.SetValue("recordCount", $"{data.TotalRecords:0,0}");
                body.SetValue("telephone", site.PrimaryPhone);

                // Determine if we enter order minimum note
                if (orderMinimum == 0 || orderTotal > orderMinimum)
                {
                    body.SetValue("orderTotal", $"{message.Total():C}");
                }
                else
                {
                    body.SetValue("orderTotal", $"{message.Total():C} (there is a {orderMinimum:C} minimum charge)");
                }

                body.SetValue("lineItems", message.Products.Select(p => new
                {
                    p.Title,
                    Cost = $"{p.PPU:C}",
                    Total = $"{p.Total():C}",
                    EstMatches = $"{p.EstimatedMatches:0,0}"
                }));

                var email = new MailMessage(site.MailboxSupport, data.DefaultEmail)
                {
                    Subject = $"Order Submitted - {data.ListName}",
                    Body = body.Run(),
                    IsBodyHtml = true
                };

                var command = new SendEmailCommand(email);
                command.Track = true;

                await context.Send(command);
            }
        }

        #endregion

        #region Methods

        private static FileResource GetBodyResourceForApplication()
        {
            var resource = new FileResource(@"EmailTemplates\OrderSubmittedNationBuilder.html.config");
            return resource;
        }

        private static FileResource GetSignatureResourceForApplication()
        {
            var resource = new FileResource(@"EmailTemplates\Signature.html.config");
            return resource;
        }

        private static async Task<String> GetSignature()
        {
            var siteinfo = SiteCache.Cache.First(s => s.ApplicationId == ApplicationExtensions.AccurateAppendId);

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

        #endregion
    }
}