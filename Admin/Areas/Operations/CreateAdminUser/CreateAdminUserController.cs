using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Security;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Admin.Areas.Operations.CreateAdminUser
{
    [Authorize()]
    public class CreateAdminUserController : Controller
    {
        #region Fields

        private readonly IMembershipService membership;

        #endregion

        #region Controller

        public CreateAdminUserController(IMembershipService membership)
        {
            if (membership == null) throw new ArgumentNullException(nameof(membership));
            Contract.EndContractBlock();

            this.membership = membership;
        }

        #endregion

        #region Action Methods

        [AcceptVerbs(HttpVerbs.Get)]
        public virtual ActionResult Index()
        {
            return this.View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual async Task<ActionResult> Index(String userName, String password, CancellationToken cancellation)
        {
            if (!this.User.Identity.IsSuperUser())
            {
                this.TempData["message"] = "Your account does not have access to this feature.";
                return this.View("~/Views/Shared/Error.aspx");
            }

            var status = await this.membership.CreateUserAsync(userName, password, ApplicationExtensions.AccurateAppendId, cancellation);
            return new LiteralResult(true) {Data = status};
        }

        #endregion
    }
}