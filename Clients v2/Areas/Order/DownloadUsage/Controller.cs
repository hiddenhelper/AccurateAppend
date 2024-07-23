using AccurateAppend.Core;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Sales.Formatters;
using System;
using System.Diagnostics.Contracts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.Areas.Order.DownloadUsage
{
    /// <summary>
    /// Controller to generate reports of usage for a client.
    /// </summary>
    [Authorize()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private readonly IUsageReportBuilder report;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        /// <param name="report">The <see cref="IUsageReportBuilder"/> to use for this controller instance.</param>
        public Controller(IUsageReportBuilder report)
        {
            Contract.EndContractBlock();

            this.report = report;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Action to generate a report of usage (Job and API) for a given period for the interactive caller.
        /// </summary>
        /// <param name="start">The period start (inclusive).</param>
        /// <param name="end">The period end (inclusive).</param>
        /// <param name="cancellation">Used to signal asynchronous cancellation.</param>
        public virtual async Task<ActionResult> Index(DateTime start, DateTime end, CancellationToken cancellation)
        {
            var userid = this.User.Identity.GetIdentifier();
            var userName = this.User.Identity.Name;

            var range = new DateSpan(start, end);
            var generatedReport = await this.report.GenerateUsageReport(userid, range, cancellation);

            return this.File(Encoding.UTF8.GetBytes(generatedReport), "text/csv", $"Usage: {userName} - {start.ToShortDateString()} thru {end.ToShortDateString()}.csv");
        }

        #endregion
    }
}