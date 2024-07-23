using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.ChargeProcessing.DataAccess;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Clients.Areas.Profile.WalletApi
{
    /// <summary>
    /// Controller to provide payment profile access.
    /// </summary>
    [Authorize()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private readonly DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> providing data access.</param>
        public Controller(DefaultContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            this.context = context;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Endpoint to query the current payment information for the user.
        /// </summary>
        public async Task<ActionResult> PrimaryCard(CancellationToken cancellation)
        {
            var card = await this.context
                .Set<ChargeProcessing.ChargePayment>()
                .CardsForInteractiveUser()
                .OrderByDescending(c => c.IsPrimary)
                .ThenByDescending(c => c.CreatedDate)
                .FirstOrDefaultAsync(cancellation);

            if (card?.Card.IsValid() != true) return new JsonNetResult() {Data = new {Primary = (Object) null}};

            var data = new
            {
                UserId = this.User.Identity.GetIdentifier(),
                card.Id,
                card.Card.DisplayValue,
                Expiration = $"{card.Card.Expiration.Month:D2}-{card.Card.Expiration.Year}",
                BillTo = new
                {
                    card.BillTo.FirstName,
                    card.BillTo.LastName,
                    card.BillTo.BusinessName,
                    PostalCode = card.BillTo.Zip,
                    card.BillTo.PhoneNumber
                }
            };

            return new JsonNetResult() {Data = new {Primary = data}};
        }

        #endregion
    }
}