using System;
using System.Diagnostics.Contracts;
using System.Web.Mvc;
using AccurateAppend.Security;

namespace AccurateAppend.Websites.Clients.Areas.Authentication.LogOff
{
    /// <summary>
    /// Controller to manage destroying a current interactive user session.
    /// </summary>
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private readonly IFormsAuthentication fa;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        /// <param name="fa">The <see cref="IFormsAuthentication"/> component that provides logon functionality.</param>
        public Controller(IFormsAuthentication fa)
        {
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            Contract.EndContractBlock();

            this.fa = fa;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Action to perform the logout of the current interactive user.
        /// </summary>
        public virtual ActionResult Index()
        {
            if (this.User.Identity.IsAuthenticated) this.fa.SignOut();
            if (this.HttpContext.Session != null) this.HttpContext.Session.Abandon();

            return this.RedirectToAction("Login", "Direct", new {area = "Authentication"});
        }

        #endregion
    }
}