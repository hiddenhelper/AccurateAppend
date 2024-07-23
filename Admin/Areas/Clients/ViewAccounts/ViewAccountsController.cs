using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Websites.Admin.Areas.Clients.ViewAccounts.Models;

namespace AccurateAppend.Websites.Admin.Areas.Clients.ViewAccounts
{
    [Authorize()]
    public class ViewAccountsController : Controller
    {
        private readonly DefaultContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAccountsController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> to use for this controller instance.</param>
        public ViewAccountsController(DefaultContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            this.context = context;
        }

        public virtual async Task<ActionResult> Index(Guid userid, CancellationToken cancellation)
        {
            var client = await this.context
                .SetOf<ClientRef>()
                .Where(c => c.UserId == userid)
                .FirstOrDefaultAsync(cancellation);

            if (client == null)
            {
                this.TempData["message"] = $"An error has occured while viewing the service accounts. The user {userid} does not exist.";
                return this.View("~/Views/Shared/Error.aspx");
            }

            var model = new ViewAccountsModel();
            model.UserId = client.UserId;
            model.UserName = client.UserName;

            var billing = await this.context
                .SetOf<RecurringBillingAccount>()
                .Where(b => b.ForClient.UserId == userid)
                .OrderByDescending(b => b.EffectiveDate)
                .ToArrayAsync(cancellation);

            foreach (var billingAccount in billing)
            {
                model.LoadAccount(billingAccount);
            }

            return this.View(model);
        }
    }
}