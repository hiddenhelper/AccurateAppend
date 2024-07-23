using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.Controllers
{
    /// <summary>
    /// Controller for displaying the errors screens during unhandled or uncompensatable errors.
    /// </summary>
    [AllowAnonymous()]
    public class ErrorController : Controller
    {
        /// <summary>
        /// Displays the error screen.
        /// </summary>
        public ActionResult Error()
        {
            return this.View("Error");
        }
    }
}