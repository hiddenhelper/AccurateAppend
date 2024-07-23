using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Data;
using AccurateAppend.Security;

namespace AccurateAppend.Websites.Clients.Areas.Public.Invite
{
    [AllowAnonymous()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private readonly ISessionContext context;

        #endregion

        #region Constructor

        public Controller(ISessionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            this.context = context;
        }

        #endregion

        #region Action Methods

        public virtual async Task<ActionResult> Index(Guid id, CancellationToken cancellation)
        {
            if (this.User.Identity.IsAuthenticated) return this.DisplayErrorResult("You cannot be logged in when using this invite.");

            var invite = await this.context
                .SetOf<InvitedLogon>()
                .FirstAsync(i => i.Id == id && i.IsActive, cancellation);
            if (invite == null || invite.ExpirationDate >= DateTime.UtcNow) return this.DisplayErrorResult("This invite does not exist or is has expired and is no longer available.");

            return this.View();
        }

        #endregion
    }
}