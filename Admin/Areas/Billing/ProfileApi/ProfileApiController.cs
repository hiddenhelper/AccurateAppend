using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.ChargeProcessing;
using AccurateAppend.ChargeProcessing.DataAccess;
using AccurateAppend.Data;

namespace AccurateAppend.Websites.Admin.Areas.Billing.ProfileApi
{
    [Authorize()]
    public class ProfileApiController : Controller
    {
        #region Fields

        private readonly DefaultContext context;

        #endregion

        #region Constructor

        public ProfileApiController(DefaultContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region Action Methods

        public virtual async Task<ActionResult> QueryByUser(Guid userId, CancellationToken cancellation)
        {
            var cards = await this.context
                .Set<ChargePayment>()
                .Where(c => c.Client.UserId == userId)
                .OrderByDescending(c => c.IsPrimary)
                .ThenByDescending(c => c.CreatedDate)
                .ToArrayAsync(cancellation);

            var data = cards
                .Where(c => c.Card.IsValid())
                .Select(c =>
                    new
                    {
                        c.Id,
                        IsPrimary = cards.Length == 1 || c.IsPrimary,
                        c.BillTo.BusinessName,
                        Name = c.BillTo.ToString(),
                        PhoneNumber = c.BillTo.PhoneNumber.ToString(),
                        c.BillTo.Address,
                        c.BillTo.City,
                        c.BillTo.State,
                        PostalCode = c.BillTo.Zip,
                        DisplayValue = c.Card.ToString(),
                        Expiration = c.Card.Expiration.ToString("MM/yyyy"),
                        c.Profile.PublicKey
                    })
                .ToArray();
            
            return this.Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}