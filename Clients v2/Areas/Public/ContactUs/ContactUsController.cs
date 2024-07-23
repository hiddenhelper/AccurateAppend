using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.Areas.Public.ContactUs
{
    /// <summary>
    /// Provides the generic privacy and contact forms.
    /// </summary>
    [AllowAnonymous()]
    public class ContactUsController : Controller
    {
        #region Action Methods
        public virtual ActionResult Index()
        {
            return View();
        }
        #endregion
    }
}
