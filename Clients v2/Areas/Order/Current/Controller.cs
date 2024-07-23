using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Websites.Clients.Data;
using DomainModel.ActionResults;
using Integration.NationBuilder.Data;

namespace AccurateAppend.Websites.Clients.Areas.Order.Current
{
    /// <summary>
    /// Controller for display recent orders for the interactive client.
    /// </summary>
    [Authorize()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private readonly ISessionContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> component providing entity access.</param>
        public Controller(ISessionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// View to access the current orders.
        /// </summary>
        public virtual ActionResult Index()
        {
            return this.View();
        }

        public virtual async Task<ActionResult> DefaultRange(CancellationToken cancellation)
        {
            using (this.context.CreateScope(ScopeOptions.NoTracking))
            {
                var cutOff = DateTime.UtcNow.AddDays(-30);

                var newestOrder = await this.context.SetOf<Data.Order>()
                    .ForInteractiveUser()
                    .Where(j => j.DateSubmitted >= cutOff)
                    .OrderByDescending(j => j.DateSubmitted)
                    .Select(j => System.Data.Entity.DbFunctions.TruncateTime(j.DateSubmitted))
                    .FirstOrDefaultAsync(cancellation);

                newestOrder = newestOrder == null ? DateTime.UtcNow.Date : newestOrder.Coerce();

                var startDate = newestOrder.Value.AddDays(-1);
                var endDate = newestOrder.Value;

                return new JsonNetResult { Data = new { StartDate = startDate.ToString("d"), EndDate = endDate.ToString("d") } };
            }
        }

        /// <summary>
        /// Returns collection of Order formatted in Json
        /// </summary>
        public virtual async Task<ActionResult> Read(DateTime startDate, DateTime endDate, CancellationToken cancellation)
        {
            using (this.context.CreateScope(ScopeOptions.NoTracking))
            {
                var baseQuery = this.context.SetOf<Data.Order>()
                    .ForInteractiveUser()
                    .Where(j => j.DateSubmitted >= startDate && j.DateSubmitted <= endDate)
                    .Where(c => c.OrderStatus != ProcessingStatus.Canceled);
                var orders1 = baseQuery.OfType<NationBuilderOrder>().Where(j => j.PushStatus != PushStatus.Canceled);
                var orders2 = baseQuery.OfType<BatchOrder>();
                var orders3 = baseQuery.OfType<DirectClientOrder>();

                var orders = orders1.Cast<Data.Order>().Concat(orders2.Cast<Data.Order>()).Concat(orders3.Cast<Data.Order>());
                orders = orders.OrderByDescending(o => o.DateSubmitted).Take(50);
                var d = await orders.ToArrayAsync(cancellation);
                
                return new JsonNetResult { Data = new { Data = d, Total = d.Length } };
            }
        }

        #endregion
    }
}