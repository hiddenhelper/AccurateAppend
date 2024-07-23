using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Accounting;
using AccurateAppend.Core;
using DomainModel.ActionResults;
using DomainModel.Queries;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json.Linq;

namespace AccurateAppend.Websites.Admin.Areas.Reporting.Controllers
{
    /// <summary>
    /// Controller for supplying <see cref="Deal"/> monthly recurring revenue metric reports.
    /// </summary>
    [Authorize()]
    public class LeadChannelMetricsController : Controller
    {
        #region Fields

        private readonly ILeadChannelMetricQuery dal;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <seealso cref="MrrMetricsController"/> class.
        /// </summary>
        /// <param name="dal">The <seealso cref="IMrrMetricQuery"/> to use for data access.</param>
        public LeadChannelMetricsController(ILeadChannelMetricQuery dal)
        {
            if (dal == null) throw new ArgumentNullException(nameof(dal));
            Contract.EndContractBlock();

            this.dal = dal;
        }

        #endregion

        #region Actions

        // http://localhost:60920/Reporting/LeadChannelMetrics/Query?applicationid=02b8794b-0449-4051-ac2a-1b85220f90c9
        /// <summary>
        /// Returns monthly recurring revenue metrics in Json format
        /// </summary>
        //[OutputCache(Duration = 5*60, VaryByParam = "applicationId")]
        public async Task<ActionResult> Query([DataSourceRequest] DataSourceRequest request, Guid applicationId, int leadSource, CancellationToken cancellation)
        {
            var data = (await this.dal.Query(applicationId, leadSource, cancellation)).GroupBy(d => d.MetricName).Select(g =>
            {
                var json = JToken.FromObject(new {Description = g.Key.GetDescription(), MetricName = g.Key});
                foreach (var record in g)
                {
                    json[record.Key.ToString("MMM-yyyy")] = record.Value;
                }
                //json["MetricName"] = g.Key.ToString(); // Gets around type conversion issue
                return json;
            }).ToArray();

            var jsonNetResult = new JsonNetResult
            {
                Data = data
            };
            
            return jsonNetResult;
        }

        #endregion
    }
}
