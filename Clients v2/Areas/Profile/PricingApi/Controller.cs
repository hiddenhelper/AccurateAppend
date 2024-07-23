using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;

namespace AccurateAppend.Websites.Clients.Areas.Profile.PricingApi
{
    /// <summary>
    /// Controller to provide custom rate card API access.
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
        /// /// <param name="context">The <see cref="ISessionContext"/> component providing data access to this instance.</param>
        public Controller(DefaultContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region Action Methods

        public virtual async Task<ActionResult> RateCard(CancellationToken cancellation)
        {
            var userId = this.User.Identity.GetIdentifier();

            var operations = new[]
            {
                DataServiceOperation.PHONE_VER,
                DataServiceOperation.DEMOGRAHICS,
                DataServiceOperation.PHONE_REV_DA,
                DataServiceOperation.PHONE,
                DataServiceOperation.NAME,
                DataServiceOperation.CASS,
                DataServiceOperation.EMAIL_VERIFICATION,
                DataServiceOperation.EMAIL_BASIC_REV,
                DataServiceOperation.PHONE_CCO_MIXED,
                DataServiceOperation.DOB,
                DataServiceOperation.EMAIL,
                DataServiceOperation.PHONE_REV_CCO,
                DataServiceOperation.PHONE_MOB,
                DataServiceOperation.EMAIL_BASIC_NO_SUPRESSION_W_VERIFICATION,
                DataServiceOperation.PHONE_REV_PREM,
                DataServiceOperation.PHONE_DA,
                DataServiceOperation.PHONE_PREM,
                DataServiceOperation.PHONE_STD,
                DataServiceOperation.PHONE_REV_STD,
                DataServiceOperation.PHONE_BUS_STD,
                DataServiceOperation.EMAIL_VER_SUPRESSION,
                DataServiceOperation.PHONE_BUS_PREM,
                DataServiceOperation.PHONE_CONN,
                DataServiceOperation.NCOA48,
                DataServiceOperation.PHONE_BUS_DA
            }.Select(p => p.ToString());

            var q1 = this.context
                .SetOf<Cost>()
                .Where(c => operations.Contains(c.Product.Key))
                .Where(c => c.Category == Cost.DefaultCategory)
                .Select(c =>
                    new
                    {
                        Rank = 0,
                        c.Product.Key,
                        c.Product.Title,
                        Price = c.PerMatch,
                        c.Floor,
                        c.Ceiling
                    });

            var q2 = this.context
                .SetOf<Cost>()
                .Where(c => operations.Contains(c.Product.Key))
                .Where(c => c.Category == userId.ToString())
                .Select(c =>
                    new
                    {
                        Rank = 1,
                        c.Product.Key,
                        c.Product.Title,
                        Price = c.PerMatch,
                        c.Floor,
                        c.Ceiling
                    });

            var rates = await q1.Concat(q2)
                .GroupBy(c => c.Key)
                .OrderBy(c => c.Key)
                .ToArrayAsync(cancellation);

            var data = rates.SelectMany(grouping =>
            {
                var client = grouping.Where(g => g.Rank == 1).ToArray();
                var defaults = grouping.Where(g => g.Rank == 0).ToArray();

                return (client.Any() ? client : defaults).OrderBy(g => g.Floor);
            });

            return this.Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}