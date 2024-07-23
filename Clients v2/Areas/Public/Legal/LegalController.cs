using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.Areas.Public.Legal
{
    /// <summary>
    /// Controller used to present legal content.
    /// </summary>
    [AllowAnonymous()]
    public class LegalController : Controller
    {
        #region Action Methods

        /// <summary>
        /// Display the terms and conditions view.
        /// </summary>
        public ActionResult ToC()
        {
            return this.View();
        }

        #endregion
    }
}