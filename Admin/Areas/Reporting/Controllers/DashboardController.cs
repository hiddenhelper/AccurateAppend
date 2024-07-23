using System.Web.Mvc;

namespace AccurateAppend.Websites.Admin.Areas.Reporting.Controllers
{
    [Authorize()]
    public class DashboardController : Controller
    {
        /// <summary>
        /// Action to display the reporting dashboard.
        /// </summary>
        public ActionResult Index()
        {
            if (this.User.Identity.IsVendor() || this.User.Identity.IsLimitedAccess()) return this.DisplayErrorResult("You do not have permission to view this page.");

            return this.View();
        }
    }
}