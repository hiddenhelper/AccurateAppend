using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Data;
using AccurateAppend.Operations.Contracts;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Sales.Formatters;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Areas.Sales.DownloadUsage
{
    [Authorize()]
    public class DownloadUsageController : Controller
    {
        #region Fields

        private readonly IUsageReportBuilder report;
        private readonly FileContext fileContext;
        private readonly DefaultContext dataContext;
        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadUsageController"/> class.
        /// </summary>
        /// <param name="report">The <see cref="IUsageReportBuilder"/> to use for this controller instance.</param>
        /// <param name="fileContext"></param>
        /// /// <param name="dataContext">The required <see cref="DefaultContext"/> component.</param>
        public DownloadUsageController(IUsageReportBuilder report, FileContext fileContext, DefaultContext dataContext, IMessageSession bus)
        {
            if (fileContext == null) throw new ArgumentNullException(nameof(fileContext));
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));

            this.report = report;
            this.fileContext = fileContext;
            this.dataContext = dataContext;
            this.bus = bus;
        }

        #endregion

        #region Actions

        public virtual async Task<ActionResult> Index(Guid userId, DateTime start, DateTime end, CancellationToken cancellation)
        {
            var userName = await this.dataContext
                .SetOf<ClientRef>()
                .Where(u => u.UserId == userId)
                .Select(u => u.UserName)
                .SingleOrDefaultAsync(cancellation);

            var range = new DateSpan(start, end);
            var generatedReport = await this.report.GenerateUsageReport(userId, range, cancellation);

            return this.File(Encoding.UTF8.GetBytes(generatedReport), "text/csv", $"Usage: {userName} - {start.ToShortDateString()} thru {end.ToShortDateString()}.csv");
        }

        public virtual async Task<ActionResult> SaveUsageToUserFiles(Guid userId, DateTime start, DateTime end, CancellationToken cancellation)
        {
            var userName = await this.dataContext
                .SetOf<ClientRef>()
                .Where(u => u.UserId == userId)
                .Select(u => u.UserName)
                .SingleAsync(cancellation);

            var filename = $"Usage: {userName} - {start.ToShortDateString()} thru {end.ToShortDateString()}.csv";

            var range = new DateSpan(start, end);
            var generatedReport = await this.report.GenerateUsageReport(userId, range, cancellation);
            var reportBytes = Encoding.ASCII.GetBytes(generatedReport);

            var assistedFiles = this.fileContext.Temp;
            var assistedFile = assistedFiles.CreateInstance(Guid.NewGuid().ToString());
            using (var stream = assistedFile.OpenStream(FileAccess.Write, true))
            {
                await stream.WriteAsync(reportBytes, 0, reportBytes.Length, cancellation);
            }

            var command = new StoreFileCommand
            {
                UserId = userId,
                CustomerFileName = filename,
                RequestId = Guid.NewGuid(),
                SystemFileName = assistedFile.Name
            };

            await this.bus.Send(command);

            return this.Json(new { success = true, message = $"{filename} saved to client files." }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
