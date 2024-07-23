using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Accounting;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Messages;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Websites.Admin.Areas.Clients.CancelAccount.Models;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Areas.Clients.CancelAccount
{
    /// <summary>
    /// Controller to immediately cancel a <see cref="ServiceAccount"/>.
    /// </summary>
    [Authorize()]
    public class CancelAccountController : Controller
    {
        #region Fields

        private readonly IMessageSession bus;
        private readonly DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelAccountController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> to use for this controller instance.</param>
        /// <param name="bus">The <see cref="IMessageSession"/> used to publish messages.</param>
        public CancelAccountController(DefaultContext context, IMessageSession bus)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            Contract.EndContractBlock();

            this.context = context;
            this.bus = bus;
        }

        #endregion

        #region Action Methods

        public virtual async Task<ActionResult> Index(Guid userid, String redirectTo, CancellationToken cancellation)
        {
            redirectTo = redirectTo ?? this.Url.Action("Index", "ViewAccounts", new { area = "Clients", userid });

            var account = await this.context
                .SetOf<RecurringBillingAccount>()
                .AsNoTracking()
                .Where(a => a.ForClient.UserId == userid)
                .OrderByDescending(a => a.EffectiveDate)
                .FirstOrDefaultAsync(cancellation);
            if (account == null)
            {
                this.TempData["message"] = "There are no service accounts for this user ";
                return this.View("~/Views/Shared/Error.aspx");
            }

            var model = new CancelAccountModel();
            model.AccountId = account.Id.Value;
            model.FirstAvailableDate = account.EndDate ?? DateTime.Today.AddDays(1);
            model.RedirectTo = redirectTo;

            return this.View(model);
        }

        public virtual async Task<ActionResult> Index2(Int32 accountId, DateTime? endDate, String redirectTo, CancellationToken cancellation)
        {
            redirectTo = redirectTo ?? this.Url.Action("Index", "UserSummary", new { area = "Clients" });

            var account = await this.context
                .SetOf<RecurringBillingAccount>()
                .AsNoTracking()
                .Where(a => a.Id == accountId)
                .Include(a => a.ForClient)
                .FirstAsync(cancellation);

            if (account is SubscriptionBilling)
            {
                var publicKey = account.PublicKey;

                var command = new CancelSubscriptionCommand();
                command.EndDate = endDate.Value;
                command.PublicKey = publicKey;

                await this.bus.SendLocal(command);
            }
            else if (account is UsageBilling)
            {
                var publicKey = account.PublicKey;

                var command = new CancelUsageOnlyCommand();
                command.EndDate = endDate.Value;
                command.PublicKey = publicKey;

                await this.bus.SendLocal(command);
            }
            else
            {
                var publicKey = account.PublicKey;

                var command = new CancelAccrualCommand();
                command.EndDate = endDate.Value;
                command.PublicKey = publicKey;

                await this.bus.SendLocal(command);
            }

            // Fake wait processing time because we're not using callbacks
            await Task.Delay(TimeSpan.FromSeconds(4), cancellation);

            return this.Redirect(redirectTo);
        }

        #endregion
    }
}