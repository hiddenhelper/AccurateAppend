using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.Areas.Public.HowItWorks
{
    /// <summary>
    /// Responsible for managing public self-service signups.
    /// </summary>
    [AllowAnonymous()]
    [RestrictUrl("clients.2020connect.net", "http://www.2020connect.net")]
    [RestrictUrl("devclients.2020connect.net", "http://www.2020connect.net")]
    [Restricted()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Action Methods

        /// <summary>
        /// Action method to display the how it works view.
        /// </summary>
        public ActionResult Index()
        {
            return this.View();
        }

        #endregion
    }
}