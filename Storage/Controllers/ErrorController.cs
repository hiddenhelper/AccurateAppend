using System.Web.Mvc;

namespace AccurateAppend.Websites.Storage.Controllers
{
    /// <summary>
    /// Controller for enabling user friendly error screens.
    /// </summary>
    public class ErrorController : Controller
    {
        /// <summary>
        /// Displays the error view.
        /// </summary>
        public ActionResult Error()
        {
            return this.View();
        }
    }
}