using Atp;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AccurateAppend.Accounting;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Security;
using Order = AccurateAppend.Sales.Order;

namespace AccurateAppend.Websites.Templates
{
    /// <summary>
    /// Factory component used to craft html content for a <see cref="Order"/> detail.
    /// </summary>
    /// <remarks>
    /// As this is shared code added as part of various applications, the location of the
    /// template isn't able to be known here. Therefore applications incorporating this
    /// component will need to contend with this being a partial type and require defining
    /// a method "static FileResource GetResourceByName(String fileName)".
    /// </remarks>
    public static partial class OrderDetailHtml
    {
        #region Factories

        /// <summary>
        /// Generates HTML version of order detail for PDF creation.
        /// </summary>
        /// <param name="order">The <see cref="Order"/> to create a PDF for.</param>
        /// <returns>An HTML encoded </returns>
        public static async Task<String> OrderDetail(Order order)
        {
            using (var body = new TemplateEngine())
            {
                var resource = GetResourceByName("OrderDetail.html.config");
                using (var reader = resource.GetStreamReader())
                {
                    body.LoadFromString(await reader.ReadToEndAsync().ConfigureAwait(false));
                }

                var client = order.Deal.Client;
                var site = SiteCache.Cache.FirstOrDefault(s => s.ApplicationId == client.ApplicationId) ??
                           SiteCache.Cache.First(s => s.ApplicationId == WellKnownIdentifiers.AccurateAppendId);

                body.SetValue("siteName", site.Title);
                body.SetValue("siteAddress", site.Address);
                body.SetValue("siteCity", site.City);
                body.SetValue("siteState", site.State);
                body.SetValue("siteZip", site.Zip);
                body.SetValue("siteTelephone", site.PrimaryPhone);
                
                if (client.BusinessName != String.Empty) body.SetValue("customerBusinessName", $"<div>{client.BusinessName}</div>");
                if (client.LastName != String.Empty) body.SetValue("customerName", $"<div>{PartyExtensions.BuildCompositeName(client.FirstName, client.LastName, client.UserName)}</div>");
                if (client.Address.StreetAddress != String.Empty && client.Address.City != String.Empty)
                {
                    body.SetValue("customerAddress", $"<div>{client.Address.StreetAddress}</div>");
                    body.SetValue("customerCityStateZip", $"<div>{(client.Address.City + ", " + client.Address.State + " " + client.Address.PostalCode).Trim()}</div>");
                }

                if (client.PrimaryPhone != String.Empty) body.SetValue("customerTelephone", $"<div>{client.PrimaryPhone}</div>");
                if (client.UserName != String.Empty) body.SetValue("customerEmail", $"<div>{client.UserName.ToLower()}</div>");
                if (order.Transactions.Any(a => a.Status == TransactionResult.Approved)) body.SetValue("billingStatus", "<div>BILLED TO CARD ON FILE</div>");

                body.SetValue("date", DateTime.Now.ToUserLocal().ToString("d", CultureInfo.InvariantCulture));
                body.SetValue("orderId", order.Id);
                body.SetValue("total", order.Total().ToString("C"));
                body.SetValue("lineItems", order.Lines.Select(p => new
                {
                    Title = p.Description,
                    OperationName = p.Product.Key,
                    Cost = $"{p.Price:C3}",
                    Quantity = $"{p.Quantity:N0}",
                    Total = $"{(p.Price * p.Quantity):C}"
                }));

                return body.Run();
            }
        }
        
        #endregion
    }
}