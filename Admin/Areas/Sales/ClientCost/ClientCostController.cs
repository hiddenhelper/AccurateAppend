using System;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;

namespace AccurateAppend.Websites.Admin.Areas.Sales.ClientCost
{
    /// <summary>
    /// Controller to perform rate card lookup for a customer for a product.
    /// </summary>
    /// <remarks>
    /// Uses a specific <see cref="ClientRef"/> to locate a <see cref="Cost"/> structure custom for the client for 
    /// each requested <see cref="Product"/>. If the client has a custom schedule the <see cref="CostPricingModel"/>
    /// is consulted to determine the proper <see cref="PricingModel"/> that should be applied. Any time a <seealso cref="Product"/>
    /// lacks a customer specific schedule, the component will default to the default rate card.
    /// </remarks>
    [Authorize()]
    public class ClientCostController : Controller
    {
        #region Fields

        private readonly DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCostController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> to use.</param>
        public ClientCostController(DefaultContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Performs a rate lookup for a specific product, by <see cref="Product.Key"/>, for a specified quantity and user identifier.
        /// </summary>
        [OutputCache(Duration = 10 * 1000, VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public virtual async Task<ActionResult> Index(String name, Int32 qty, Guid userId, Decimal cost, CancellationToken cancellation)
        {
            var unitcost = 0M;

            try
            {
                var client = await this.context.SetOf<ClientRef>().SingleOrDefaultAsync(c => c.UserId == userId, cancellation);

                var costService = client == null
                    ? new ContextBasedCostService(this.context)
                    : new CustomerCostService(client, this.context);

                var rateCard = await costService.CreateRateCard(name, cancellation);
                var costStructure = rateCard.FindCost(qty);
                unitcost = Math.Max(costStructure.PerRecord, costStructure.PerMatch);

                unitcost = cost < 0 ? cost : Math.Max(cost, unitcost);
            }
            catch (DbException ex)
            {
                Debugger.Break();
                EventLogger.Logger.LogEvent(ex, Severity.Low, Application.AccurateAppend_Admin, description: "Error performing cost lookup on product");
            }

            return this.Json(new {unitcost}, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}