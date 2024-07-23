using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Sales;
using DomainModel.ActionResults;
using DomainModel.Queries;
using Kendo.Mvc.UI;
using Newtonsoft.Json.Linq;

namespace AccurateAppend.Websites.Admin.Areas.Reporting.Controllers
{
    /// <summary>
    /// Controller for supplying <see cref="DealBinder"/> monthly recurring revenue metric reports by sales rep.
    /// </summary>
    [Authorize()]
    public class AgentMetricsController : Controller
    {
        #region Fields

        private readonly IAgentMetricQuery dal;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <seealso cref="AgentMetricsController"/> class.
        /// </summary>
        /// <param name="dal">The <seealso cref="IAgentMetricQuery"/> to use for data access.</param>
        public AgentMetricsController(IAgentMetricQuery dal)
        {
            if (dal == null) throw new ArgumentNullException(nameof(dal));
            Contract.EndContractBlock();

            this.dal = dal;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Returns monthly recurring revenue metrics by sales rep in Json format
        /// </summary>
        [OutputCache(Duration = 5*60, VaryByParam = "applicationId")]
        public async Task<ActionResult> Query([DataSourceRequest] DataSourceRequest request, Guid applicationId, CancellationToken cancellation)
        {
            var data = (await this.dal.Query(applicationId, cancellation)).GroupBy(d => d.MetricName).Select(g =>
            {
                var json = JToken.FromObject(new {Description = g.Key.GetDescription()});
                foreach (var record in g)
                {
                    json[record.Key.ToString("MMM-yyyy")] = $"{record.Amount:C0}";
                }
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
