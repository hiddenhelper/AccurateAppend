using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Accounting;
using AccurateAppend.Core;
using AccurateAppend.Data;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.Areas.Clients.LeadDetail;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Navigator;
using DomainModel.ActionResults;
using Kendo.Mvc;
using Kendo.Mvc.UI;

namespace AccurateAppend.Websites.Admin.Areas.Clients.ApiTrialSummary
{
    /// <summary>
    /// Controller for displaying API Trial Request information.
    /// </summary>
    public class ApiTrialSummaryController : ContextBoundController
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiTrialSummaryController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> used for data access.</param>
        public ApiTrialSummaryController(ISessionContext context) : base(context)
        {
        }

        #endregion

        #region Action Methods

        public virtual async Task<ActionResult> Query(
            [DataSourceRequest] DataSourceRequest request,
            Guid applicationid,
            DateTime startdate,
            DateTime enddate,
            CancellationToken cancellation)
        {
            if (request.Sorts == null || !request.Sorts.Any())
            {
                request.Sorts = new List<SortDescriptor>
                {
                    new SortDescriptor(nameof(TrialIdentity.DateCreated), ListSortDirection.Descending)
                };
            }

            // Leads table is UTC so we ned to convert start/end dates
            startdate = startdate.ToStartOfDay().FromUserLocal().Coerce();
            enddate = enddate.ToEndOfDay().FromUserLocal().Coerce();

            using (this.Context.CreateScope(ScopeOptions.NoTracking))
            {
                var query = this.Context.SetOf<Lead>()
                    .Where(l => l.Trial != null)
                    .Where(l => l.Application.Id == applicationid)
                    .Where(l => l.Trial.DateCreated >= startdate && l.Trial.DateCreated <= enddate);
                var records = await query.Select(l => new
                {
                    ApplicationId = l.Application.Id,
                    ApplicationName = l.Application.Details.Title,
                    l.FirstName,
                    l.LastName,
                    Email = l.DefaultEmail,
                    l.Score,
                    l.Trial.DateCreated,
                    l.Trial.IsEnabled,
                    LeadId = l.Id.Value,
                    TrialId = l.Trial.Id
                }).ToArrayAsync(cancellation);

                var data = Kendo.Mvc.Extensions.QueryableExtensions.ToDataSourceResult(records, request, o =>
                    new
                    {
                        o.ApplicationId,
                        o.ApplicationName,
                        o.FirstName,
                        o.LastName,
                        o.Email,
                        o.Score,
                        DateCreated = o.DateCreated.ToUserLocal(),
                        o.IsEnabled,
                        o.LeadId,
                        DetailUrl = this.Url.Action("Index", "ApiTrialDetail", new {Area = "Clients", Id = o.TrialId }),
                    });

                data.Total = records.Length;

                var jsonNetResult = new JsonNetResult(DateTimeKind.Local) { Data = data };

                return jsonNetResult;
            }
        }
       
        #endregion
    }
}