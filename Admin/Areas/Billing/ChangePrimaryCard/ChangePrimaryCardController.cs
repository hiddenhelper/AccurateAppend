using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.ChargeProcessing;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Websites.Admin.Areas.Billing.ViewCreditCards;
using AccurateAppend.Websites.Admin.Navigator;
using EventLogger;
using DefaultContext = AccurateAppend.ChargeProcessing.DataAccess.DefaultContext;

namespace AccurateAppend.Websites.Admin.Areas.Billing.ChangePrimaryCard
{
    [Authorize()]
    public class ChangePrimaryCardController : Controller
    {
        #region Fields

        private readonly DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangePrimaryCardController"/> class.
        /// </summary>
        public ChangePrimaryCardController(DefaultContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region Action Methods
        
        public virtual async Task<ActionResult> Index(Int32 cardId, CancellationToken cancellation)
        {
            try
            {
                var account = await this.context
                    .Set<ChargePayment>()
                    .Where(a => a.Id == cardId)
                    .Include(a => a.Client)
                    .FirstOrDefaultAsync(cancellation);
                if (account == null) return this.DisplayErrorResult($"An error has occured while changing the card. The card {cardId} does not exist.");

                account.Client.ChargePayments.ChangePrimaryCard(account);

                await this.context.SaveChangesAsync(cancellation);

                return this.NavigationFor<ViewCreditCardsController>().Detail(account.Client.UserId);
            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, Severity.High, Application.AccurateAppend_Admin, this.Request.UserHostAddress, this.User.Identity.Name);
                return this.DisplayErrorResult($"An error has occurred while updating the card. Message: {ex.Message}");
            }
        }

        #endregion
    }
}