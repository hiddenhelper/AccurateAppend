using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using AccurateAppend.Core;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.ReadModel;
using AccurateAppend.Sales.ReadModel.Queries;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Admin.Areas.Sales.DealActivity
{
    /// <summary>
    /// Controller for displaying deal status counts in the sidebar.
    /// </summary>
    [Authorize()]
    public class DealActivityController : Controller
    {
        #region Fields
        
        private readonly IDealsViewActiveDuringQuery dal;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DealActivityController"/> class.
        /// </summary>
        /// <param name="dal">The <see cref="IDealsViewActiveDuringQuery"/> DAL component.</param>
        public DealActivityController(IDealsViewActiveDuringQuery dal)
        {
            if (dal == null) throw new ArgumentNullException(nameof(dal));
            Contract.EndContractBlock();

            this.dal = dal;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Returns a JSON payload containing the number of deal counts by status optionally filtered by application.
        /// </summary>
        /// <param name="applicationId">The optional identifier of the application identifier to filter the counts by.</param>
        /// <param name="cancellation">The token to monitor for cancellation requests.</param>
        [OutputCache(Duration = 1 * 60, VaryByParam = "*", Location = OutputCacheLocation.Server)]
        public virtual async Task<ActionResult> Query(Guid? applicationId, CancellationToken cancellation)
        {
            // Deals table is UTC so we need to use appropriate start/end date ranges
            var startDate = DateTime.UtcNow.AddDays(-30);
            var endDate = DateTime.UtcNow;
            
            var query = this.dal
                .ActiveDuring(startDate, endDate)
                .InStatus(
                    DealStatus.InProcess,
                    DealStatus.Billing,
                    DealStatus.Approval
                );
            if (applicationId != null)
            {
                query = query.ForApplication(applicationId.Value);
            }

            var final = (await query.GroupBy(d => d.Status).ToArrayAsync(cancellation))
                .Select(
                    g => new
                    {
                        Count = g.Count(),
                        Status = g.Key,
                        StatusDescription = g.Key.GetDescription(),
                        Links = new
                        {
                            DetailView = this.Url.Action("Index", "DealSummary", new { Area = "Sales", DateRange = DomainModel.Enum.DateRange.Last30Days, Status = g.Key })
                        }
                    }
                )
                .ToArray();

            var jsonNetResult = new JsonNetResult()
            {
                Data = final
            };

            return jsonNetResult;
        }

        #endregion
    }
}