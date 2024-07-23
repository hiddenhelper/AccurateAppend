using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AccurateAppend.Data;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Areas.Clients.LeadWizard
{
    /// <summary>
    /// Controller performing lead wizard functions.
    /// </summary>
    /// <remarks>
    /// Walks the agent, and the prospect, through the sales process while gathering information about the prospect and opportunity.
    /// </remarks>
    [Authorize]
    public class LeadWizardController : Controller
    {
        #region Fields

        private readonly AccurateAppend.Accounting.DataAccess.DefaultContext context;
        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LeadWizardController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        /// <param name="bus">The <see cref="IMessageSession"/> used to publish messages.</param>
        public LeadWizardController(AccurateAppend.Accounting.DataAccess.DefaultContext context, IMessageSession bus)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        #endregion
    }
}