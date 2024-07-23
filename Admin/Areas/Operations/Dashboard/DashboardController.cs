using System.Web.Mvc;

namespace AccurateAppend.Websites.Admin.Areas.Operations.Dashboard
{
    [Authorize()]
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            return this.View();
        }
    }
}