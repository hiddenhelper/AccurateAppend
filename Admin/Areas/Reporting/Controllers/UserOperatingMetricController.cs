using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using DomainModel.ActionResults;
using DomainModel.Queries;

namespace AccurateAppend.Websites.Admin.Areas.Reporting.Controllers
{
    /// <summary>
    /// Controller for supplying <see cref="IUserOperatingMetricQuery"/> reports.
    /// </summary>
    [Authorize()]
    public class UserOperatingMetricController : Controller
    {
        #region Fields

        private readonly IUserOperatingMetricQuery dal;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <seealso cref="UserOperatingMetricController"/> class.
        /// </summary>
        /// <param name="dal">The <seealso cref="IUserOperatingMetricQuery"/> to use for data access.</param>
        public UserOperatingMetricController(IUserOperatingMetricQuery dal)
        {
            if (dal == null) throw new ArgumentNullException(nameof(dal));
            Contract.EndContractBlock();

            this.dal = dal;
        }

        #endregion

        #region Actions
        
        /// <summary>
        /// Returns PPC data aggregated by performance metrics in Json format
        /// </summary>
        [OutputCache(Duration = 5*60, VaryByParam = "userid")]
        public async Task<ActionResult> Read(Guid userid, CancellationToken cancellation)
        {
            var data = (await this.dal.Query(userid, cancellation)).ToArray();
            return new JsonNetResult
            {
                Data = new {Data = data}
            };
        }

        #endregion
    }
}
