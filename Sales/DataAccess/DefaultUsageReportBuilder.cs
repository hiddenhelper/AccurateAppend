using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AccurateAppend.Core;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing.Reporting;
using AccurateAppend.Sales.Formatters;

namespace AccurateAppend.Sales.DataAccess
{
    /// <summary>
    /// The default implementation of <see cref="IUsageReportBuilder"/> used by the Sales system.
    /// </summary>
    public class DefaultUsageReportBuilder : IUsageReportBuilder
    {
        #region Fields

        private readonly DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultUsageReportBuilder"/> class.
        /// </summary>
        public DefaultUsageReportBuilder(DefaultContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        /// <inheritdoc />
        public async Task<String> GenerateUsageReport(Guid userId, DateSpan range, CancellationToken cancellation = default(CancellationToken))
        {
            var start = range.StartingOn;
            var end = range.EndingOn;

            var userName = await this.context
                .SetOf<ClientRef>()
                .Where(c => c.UserId == userId)
                .Select(c => c.UserName)
                .SingleAsync(cancellation)
                .ConfigureAwait(false);

            // inputs are from user local time
            start = start.Date;
            end = end.Date;

            var results = new DtoUsageReport
            {
                ClientName = userName,
                StartDate = start,
                EndDate = end,
            };

            var jobsQuery = this.context.Database
                .SqlQuery<JobUsageReport>(@"
SELECT j.JobId, j.UserName, j.UserId, CustomerFileName, DateComplete, TotalRecords, Report
FROM [sales].[JobReference] j
WHERE j.[UserId] = @p0 AND j.[DateComplete] >= @p1 AND j.[DateComplete] <= @p2",
                    userId,
                    start,
                    end.AddDays(1));

            var jobReports = new List<DtoBatchUsageJob>();
            await jobsQuery.ForEachAsync(job =>
            {
                var jobReport = new DtoBatchUsageJob()
                {
                    JobId = job.JobId,
                    DateCompleted = job.DateComplete,
                    FileName = job.CustomerFileName.Replace(CsvFileContent.DefaultDelimiter, ' '),
                    RecordCount = job.TotalRecords
                };

                var groupedOperations = job.CoalleseOperationReports()
                    .Select(o =>
                        new DtoBatchUsageOperation
                        {
                            OperationName = o.Name,
                            MatchCount = o.Count
                        });
                jobReport.Operations.AddRange(groupedOperations);

                jobReports.Add(jobReport);
            }, cancellation);

            results.Jobs = jobReports.OrderBy(j => j.DateCompleted).ToList();

            // add usage by operation to report

            var operationsQuery = this.context
                .SetOf<DailyUsageRollup>()
                .Where(u => u.Date >= start && u.Date <= end)
                .Where(u => u.UserId == userId)
                .AsNoTracking();

            var grouping = operationsQuery.GroupBy(u => u.UserId);
            foreach (var group in grouping)
            {
                var batches = group.OfType<BatchUsageRollup>();
                var xml = group.OfType<ApiUsageRollup>();
                var services = group.OfType<ApiCallsUsageRollup>();
                
                foreach (var batch in batches.GroupBy(b => b.Key).OrderBy(b => b.Key))
                {
                    results.OperationUsages.Add(new DtoOperationUsage { OperationName = batch.Key, OperationDescription = EnumExtensions.Parse<DataServiceOperation>(batch.Key).GetDescription(), MatchCount = batch.Sum(b => b.Matches), Source = "Batch", Count = batch.Sum(b => b.Count) });
                }

                foreach (var batch in xml.GroupBy(b => b.Key).OrderBy(b => b.Key))
                {
                    results.OperationUsages.Add(new DtoOperationUsage { OperationName = batch.Key, OperationDescription = EnumExtensions.Parse<DataServiceOperation>(batch.Key).GetDescription(), MatchCount = batch.Sum(b => b.Matches), Source = "API", Count = batch.Sum(b => b.Count) });
                }

                foreach (var batch in services.GroupBy(b => b.Key).OrderBy(b => b.Key))
                {
                    results.OperationUsages.Add(new DtoOperationUsage { OperationName = batch.Key, OperationDescription = String.Empty, MatchCount = batch.Sum(b => b.Matches), Source = "Services", Count = batch.Sum(b => b.Count) });
                }
            }
            return results.ToString();
        }

        internal sealed class DtoUsageReport
        {
            internal DtoUsageReport()
            {
                this.OperationUsages = new List<DtoOperationUsage>();
            }

            public String ClientName { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int RecordCount
            {
                get { return Jobs.Sum(a => a.RecordCount); }
            }
            public IEnumerable<DtoBatchUsageJob> Jobs { get; set; }
            public List<DtoOperationUsage> OperationUsages { get; }
            
            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendLine("Client," + ClientName.Replace(",", " "));
                sb.AppendLine("Date," + StartDate.ToShortDateString() + " - " + EndDate.ToShortDateString());

                sb.AppendLine();
                sb.AppendLine("USAGE BY OPERATION");
                sb.AppendLine(DtoOperationUsage.HeaderRow());
                this.OperationUsages.ForEach(a => sb.AppendLine(a.ToString()));
                sb.AppendLine();
                sb.AppendLine("JOBS BY OPERATION");
                //sb.AppendLine("Files Submitted," + Jobs.Count());
                //sb.AppendLine("Records Submitted," + RecordCount);
                if (Jobs.Any())
                {
                    sb.AppendLine(DtoBatchUsageJob.HeaderRow() + "," + DtoBatchUsageOperation.HeaderRow());
                    Jobs.ForEach(a => sb.Append(a.ToString()));
                }
                else
                {
                    sb.AppendLine("*** No jobs for this period ***");
                }

                return sb.ToString();
            }
        }

        internal sealed class DtoBatchUsageJob
        {
            internal DtoBatchUsageJob()
            {
                this.Operations = new List<DtoBatchUsageOperation>();
            }

            public Int32 JobId { get; set; }
            public string FileName { get; set; }
            public DateTime DateCompleted { get; set; }
            public int RecordCount { get; set; }
            public IList<DtoBatchUsageOperation> Operations { get; }

            public override string ToString()
            {
                var sb = new StringBuilder();
                if (this.Operations.Any())
                {
                    this.Operations.ForEach(a => sb.AppendLine($"{this.JobId},{this.FileName.Replace(",", " ")},{this.DateCompleted.Coerce().ToLocalTime()},{this.RecordCount},{a.ToString()}"));
                }
                else
                    sb.AppendLine($"{this.JobId},{this.FileName.Replace(",", " ")},{this.DateCompleted.Coerce().ToLocalTime()},{this.RecordCount},0,");

                return sb.ToString();
            }
            public static String HeaderRow()
            {
                return "JobId,Filename,Date,Records";
            }
        }

        internal class DtoBatchUsageOperation
        {
            public string Source { get; set; }
            public DataServiceOperation OperationName { get; set; }
            public int MatchCount { get; set; }

            public override string ToString()
            {
                return $"{this.MatchCount},{this.OperationName}";
            }

            public static String HeaderRow()
            {
                return "Match Count,Operation";
            }
        }

        internal class DtoOperationUsage
        {
            public string Source { get; set; }
            public string OperationName { get; set; }
            public string OperationDescription { get; set; }
            public int Count { get; set; }
            public int MatchCount { get; set; }

            public override string ToString()
            {
                switch (this.Source)
                {
                    case "API":
                        return $"{this.Source},{this.OperationName.ToUpperInvariant()},{this.OperationDescription.Replace(",", " ").Replace("  ", " ")},,{this.MatchCount}";
                    case "Services":
                        return $"{this.Source},{this.OperationName.ToUpperInvariant()},{this.OperationDescription.Replace(",", " ").Replace("  ", " ")},{this.Count},";
                    default:
                        return $"{this.Source},{this.OperationName.ToUpperInvariant()},{this.OperationDescription.Replace(",", " ").Replace("  ", " ")},{this.Count},{this.MatchCount}";
                }
            }

            public static String HeaderRow()
            {
                return "Source,Operation,Description,Count,Match Count";
            }
        }

        internal class JobUsageReport
        {
            #region Fields

            private DateTime dateComplete;

            #endregion

            #region Constructor

            /// <summary>
            /// This is a readonly type.
            /// </summary>
            protected JobUsageReport()
            {
            }

            #endregion

            #region Properties

            public String UserName { get; protected set; }

            public Guid UserId { get; protected set; }

            public Int32 JobId { get; protected set; }

            public String Report { get; protected set; }

            public String CustomerFileName { get; protected set; }

            public DateTime DateComplete
            {
                get { return this.dateComplete; }
                protected set
                {
                    if (value.Kind == DateTimeKind.Unspecified) value = new DateTime(value.Ticks, DateTimeKind.Local);
                    if (value.Kind == DateTimeKind.Utc) value = value.ToLocalTime();
                    this.dateComplete = value;
                }
            }

            public Int32 TotalRecords { get; protected set; }

            #endregion

            #region Methods

            public virtual IEnumerable<OperationReport> CoalleseOperationReports()
            {
                var xml = XElement.Parse(this.Report);
                var report = new ProcessingReport(xml);


                return report.Operations
                    .Where(o => !o.Name.IsPreference())
                    .GroupBy(o => o.Name).Select(g => new OperationReport(g.Key) { Count = g.Sum(gg => gg.Count) });
            }

            #endregion
        }
    }
}
