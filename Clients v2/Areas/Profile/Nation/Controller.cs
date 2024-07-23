using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.Areas.Profile.Nation
{
    /// <summary>
    /// Controller to render view for NationBuilder integrations.
    /// </summary>
    [Authorize()]
    public class NationController : Controller
    {
        #region Action Methods

        /// <summary>
        /// Display the list of nations for the interactive user.
        /// </summary>
        public ActionResult Index()
        {
            return this.View();
        }

        #endregion
    }
}
