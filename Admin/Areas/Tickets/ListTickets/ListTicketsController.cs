using System.Web.Mvc;
using AccurateAppend.Websites.Admin.Areas.Tickets.ListTickets.Models;

namespace AccurateAppend.Websites.Admin.Areas.Tickets.ListTickets
{
    /// <summary>
    /// Controller for listing Zendesk Tickets.
    /// </summary>
    [Authorize()]
    public class ListTicketsController : Controller
    {
        public virtual ActionResult Index()
        {
            var model = new PageSettings()
            {
                Data = Url.Action("ListAllTickets", "TicketsApi", new {Area = "Tickets"}),
                ZenDesk = "https://accurateappend.zendesk.com"
            };

            return this.View(model);
        }
    }
}