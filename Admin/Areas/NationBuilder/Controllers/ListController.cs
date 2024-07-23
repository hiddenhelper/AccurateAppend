using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Websites.Admin.Areas.Clients.UserDetail;
using AccurateAppend.Websites.Admin.Filters;
using AccurateAppend.Websites.Admin.Navigator;
using DomainModel.ActionResults;
using DomainModel.Queries;
using DomainModel.ReadModel;
using Integration.NationBuilder.Data;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace AccurateAppend.Websites.Admin.Areas.NationBuilder.Controllers
{
    /// <summary>
    /// Controller used to query nation builder pushes.
    /// </summary>
    [Authorize()]
    public class ListController : Controller
    {
        #region Fields

        private readonly INationBuilderPushViewQuery query;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ListController"/> class.
        /// </summary>
        /// <param name="query">The <see cref="INationBuilderPushViewQuery"/> to use for this controller instance.</param>
        public ListController(INationBuilderPushViewQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            Contract.EndContractBlock();

            this.query = query;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Queries NationBulider <see cref="PushRequest"/>.
        /// </summary>
        [HandleErrorWithAjaxFilter()]
        [OutputCache(Duration = 10 * 1, VaryByParam = "*")]
        public virtual async Task<ActionResult> Index([DataSourceRequest] DataSourceRequest request, DateTime startdate, DateTime enddate, List<PushStatus> pushStatuses)
        {
            // NB Push table is UTC so we ned to convert start/end dates
            startdate = startdate.ToStartOfDay().FromUserLocal().Coerce();
            enddate = enddate.ToEndOfDay().FromUserLocal().Coerce();

            var pushRequests = await this.query.SubmittedDuring(startdate, enddate, pushStatuses.ToArray()).OrderByDescending(p => p.RequestDate).ToArrayAsync();

            var data = pushRequests.ToDataSourceResult(request, r =>
                    new
                    {
                        r.CorrelationId,
                        r.JobId,
                        NationName = r.Slug,
                        r.Product,
                        r.UserId,
                        r.ErrorsEncountered,
                        r.TotalRecords,
                        Progress = $"{r.CurrentPage}/{r.TotalPages}",
                        r.Status,
                        r.StatusDescription,
                        Name = r.ListName,
                        r.UserName,
                        r.RequestDate,
                        r.Id,
                        r.CanResume,
                        r.CanCancel,
                        Links = new
                        {
                            UserDetail = this.Url.BuildFor<UserDetailController>().ToDetail(r.UserId),
                            JobDetail = this.Url.Action("Index", "Summary", new { Area = "JobProcessing", jobid = r.JobId }),
                            Events = this.Url.Action("Index", "EventLog", new { Area = "Operations", correlationId = r.CorrelationId })

                        }
                    });

            data.Total = data.Data.OfType<NationBuilderPushView>().Count();

            var jsonNetResult = new JsonNetResult(DateTimeKind.Utc)
            {
                Data = data
            };

            return jsonNetResult;
        }

        #endregion
    }
}