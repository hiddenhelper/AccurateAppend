using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing;
using AccurateAppend.JobProcessing.Manifest.Xml;
using AccurateAppend.Websites.Admin.Controllers;
using DomainModel.ActionResults;
using DomainModel.ReadModel.Prospector;
using JobStatus = AccurateAppend.Core.Definitions.JobStatus;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.PreviewJob
{
    /// <summary>
    /// Returns preview of input and output files
    /// </summary>
    [Authorize()]
    public class PreviewJobController : ActivityLoggingController
    {
        #region Fields

        private readonly IFileLocation rawFiles;
        private readonly IFileLocation outbox;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewJobController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        /// <param name="files">The <see cref="FileContext"/> used to provide access to the all standard <see cref="IFileLocation"/> locations.</param>
        public PreviewJobController(ISessionContext context, FileContext files) : base(context)
        {
            if (files == null) throw new ArgumentNullException(nameof(files));
            Contract.EndContractBlock();

            this.rawFiles = files.RawCustomer;
            this.outbox = files.Outbox;
        }

        #endregion

        #region Actions
        
        /// <summary>
        /// Returns view containing head of Input file
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public virtual async Task<ActionResult> Input(Int32 jobId, CancellationToken cancellation)
        {
            using (this.Context.CreateScope(ScopeOptions.ReadOnly))
            {
                var job = await this.Context.SetOf<Job>().SingleOrDefaultAsync(j => j.Id == jobId, cancellation);
                if (job == null) return DownloadResult.Empty;

                // We use the output delimimiter as we're looking at the raw input file. This value contains the delimiter of the file we got
                var rawCustomerFile = new Plugin.Storage.CsvFile(this.rawFiles.CreateInstance(job.InputFileName), job.Manifest.OutputDelimiter());

                if (!rawCustomerFile.Exists()) return new LiteralResult(false) { Data = $"Inbox file {rawCustomerFile.Name} for job {jobId} does not exist" };

                rawCustomerFile.HasHeaderRow = false; // Just assume we want the header values

                var csvRowsTask = this.SampleFile(rawCustomerFile);
                var factsTask = DAL.DataAccess.ProspectStore.GetFactsAsync(jobId);

                await Task.WhenAll(csvRowsTask, factsTask);

                var csvRows = csvRowsTask.Result;
                var facts = factsTask.Result.ToArray();

                var model = new JobPreview(job, csvRows) { FileType = "Input", ProspectorGraph = facts.Any() ? new ProspectorViewModel(facts) : null };

                return this.View("Index", model);
            }
        }

        /// <summary>
        /// Returns view containing head of Output file
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public virtual async Task<ActionResult> Output(Int32 jobId)
        {
            using (this.Context.CreateScope(ScopeOptions.ReadOnly))
            {
                var job = await this.Context.SetOf<Job>().FirstOrDefaultAsync(j => j.Id == jobId && j.Status == JobStatus.Complete);
                if (job == null) return DownloadResult.Empty;

                // We use the output delimimiter as we're looking at the apepded file. This value contains the delimiter of the file we got
                var outboxFile = new Plugin.Storage.CsvFile(job.JobFiles().CsvOutputFile(this.outbox), job.Manifest.OutputDelimiter());

                if (!outboxFile.Exists()) return new LiteralResult(false) {Data = $"Outbox file {outboxFile.Name} for job {jobId} does not exist"};

                outboxFile.HasHeaderRow = false; // Just assume we want the header values

                var csvRowsTask = this.SampleFile(outboxFile);
                var factsTask = DAL.DataAccess.ProspectStore.GetFactsAsync(jobId);

                await Task.WhenAll(csvRowsTask, factsTask);

                var csvRows = csvRowsTask.Result;
                var facts = factsTask.Result.ToArray();

                var model = new JobPreview(job, csvRows) { FileType = "Output", ProspectorGraph = facts.Any() ? new ProspectorViewModel(facts) : null };

                return this.View("Index", model);
            }
        }

        #endregion

        #region Helper Methods

        protected virtual async Task<IList<String[]>> SampleFile(CsvFileContent csvFile)
        {
            var csvRows = new List<String[]>();

            if (csvFile.Exists())
            {
                csvFile.HasHeaderRow = false; // Just assume we want the header values

                foreach (var row in csvFile.ReadRecords().Take(100))
                {
                    var values = await row.ConfigureAwait(false);
                    csvRows.Add(values);
                }

                return csvRows;
            }

            return csvRows;
        }

        #endregion
    }

    #region dto objects


    /// <summary>
    /// Model displayed in view
    /// </summary>
    public sealed class JobPreview
    {
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="job"></param>
        /// <param name="csvRows"></param>
        public JobPreview(Job job, IList<String[]> csvRows)
        {
            this.JobId = job.Id.Value;
            this.CustomerFileName = job.CustomerFileName;
            this.TotalRecords = job.TotalRecords;
            this.CsvRows = csvRows;
            this.DateSubmitted = job.DateSubmitted.ToSafeLocal().ToUserLocal();
            this.HasHeader = job.Manifest.HasHeaderRow();
            this.ColumnMap = job.Manifest.ColumnMap().Split(';');
        }

        /// <summary>
        /// Gets the date the job was created.
        /// </summary>
        public DateTime DateSubmitted { get; private set; }

        /// <summary>
        /// Gets the number of rows in the job.
        /// </summary>
        public Int32 TotalRecords { get; private set; }

        /// <summary>
        /// Gets the name of the job.
        /// </summary>
        public String CustomerFileName { get; private set; }

        /// <summary>
        /// Gets the identifier of the job.
        /// </summary>
        public Int32 JobId { get; private set; }

        /// <summary>
        /// Record sample in csv form
        /// </summary>
        public IList<String[]> CsvRows { get; private set; }
        
        /// <summary>
        /// Indicates if first row is header
        /// </summary>
        public Boolean HasHeader { get; private set; }

        /// <summary>
        /// Input or Output
        /// </summary>
        public String FileType { get; set; }

        /// <summary>
        /// Column map
        /// </summary>
        public String[] ColumnMap { get; set; }

        /// <summary>
        /// File and column Facts pulled by DataProspector
        /// </summary>
        public ProspectorViewModel ProspectorGraph { get; set; }
    }

    #endregion
}