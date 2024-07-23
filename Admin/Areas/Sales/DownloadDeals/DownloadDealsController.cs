using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.Sales.ReadModel;
using AccurateAppend.Sales.ReadModel.Queries;
using DataStreams.Csv;

namespace AccurateAppend.Websites.Admin.Areas.Sales.DownloadDeals
{
    /// <summary>
    /// Controller for building a CSV of requested deal information.
    /// </summary>
    [Authorize()]
    public class DownloadDealsController : Controller
    {
        #region Fields

        private readonly IDealsViewActiveDuringQuery deals;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadDealsController"/> class.
        /// </summary>
        /// <param name="deals">The <see cref="IDealsViewActiveDuringQuery"/> to use for this controller instance.</param>
        public DownloadDealsController(IDealsViewActiveDuringQuery deals)
        {
            this.deals = deals;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Download the requested deal information.
        /// </summary>
        public virtual async Task<ActionResult> Index(DateTime startdate, DateTime enddate, Guid applicationId)
        {
            startdate = startdate.ToBillingZone().ToStartOfDay().ToUniversalTime();
            enddate = enddate.ToBillingZone().ToEndOfDay().ToUniversalTime();

            var query = this.deals
                .ActiveDuring(startdate, enddate)
                .ForApplication(applicationId)
                .Select(d => new
                {
                    d.DealId,
                    d.CreatedDate,
                    d.CompleteDate,
                    d.UserName,
                    d.Amount,
                    d.Title,
                    d.Status,
                    d.OwnerName,
                    d.ApplicationName,
                })
                .OrderBy(d => d.CreatedDate);

            var report = new StringBuilder();

            using (var writer = new StringWriter(report))
            {
                using (var csvWriter = new CsvWriter(writer, CsvFileContent.DefaultDelimiter))
                {
                    csvWriter.WriteRecord(new[] {"Id", "DateCreated (local)", "DateComplete (local)", "UserName", "Amount (USD)", "Description", "Status", "Sales Rep", "Site"});

                    foreach (var deal in await query.ToArrayAsync())
                    {
                        csvWriter.WriteRecord(new[]
                        {
                            deal.DealId.ToString(),
                            deal.CreatedDate.ToUserLocal().ToString(CultureInfo.CurrentUICulture),
                            deal.CompleteDate?.ToUserLocal().ToString(CultureInfo.CurrentUICulture),
                            deal.UserName,
                            deal.Amount.ToString("C", CultureInfo.CurrentCulture),
                            deal.Title,
                            deal.Status.GetDescription(),
                            deal.OwnerName,
                            deal.ApplicationName
                        });
                    }

                    csvWriter.WriteRecord(new[] { String.Empty });
                    csvWriter.WriteRecord(new[] { "Dates are in YOUR user local time, not necessarily company billing timezone" });
                }
            }
            return this.File(Encoding.UTF8.GetBytes(report.ToString()), "text/csv", $"Deals {enddate:yyyyMMdd}-{startdate:yyyyMMdd}.csv");
        }

        #endregion
    }
}
