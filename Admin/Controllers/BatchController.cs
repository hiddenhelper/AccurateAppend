using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using AccurateAppend.Accounting;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.JobManagement.Contracts;
using AccurateAppend.JobProcessing;
using AccurateAppend.JobProcessing.Manifest;
using AccurateAppend.JobProcessing.Manifest.Xml;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.Reset;
using AccurateAppend.Websites.Admin.Entities;
using AccurateAppend.Websites.Admin.Navigator;
using DataStreams.Csv;
using DomainModel;
using Kendo.Mvc.Extensions;
using JsonNetResult = DomainModel.ActionResults.JsonNetResult;
using Newtonsoft.Json.Linq;
using NServiceBus;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.Summary;

namespace AccurateAppend.Websites.Admin.Controllers
{

    [Authorize()]
    public class BatchController : ActivityLoggingController
    {
        #region Fields

        private BatchJobRequest jobrequest;
        private readonly IFileLocation temp;
        private readonly IFileLocation rawCustomerFiles;
        private readonly IFileLocation inbox;
        private readonly IFileLocation outbox;
        private readonly IFileLocation assistedFiles;
        private readonly IFileLocation legacyManifest;
        private readonly IEncryptor encryption;
        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        public BatchController(ISessionContext contex, FileContext files, IEncryptor encryption, IMessageSession bus) : base(contex)
        {
            if (files == null) throw new ArgumentNullException(nameof(files));
            if (encryption == null) throw new ArgumentNullException(nameof(encryption));
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            Contract.EndContractBlock();

            this.temp = files.Temp;
            this.rawCustomerFiles = files.RawCustomer;
            this.inbox = files.Inbox;
            this.outbox = files.Outbox;
            this.encryption = encryption;
            this.assistedFiles = files.Assisted;
            this.legacyManifest = files.LegacyManifest;
            this.bus = bus;
        }

        #endregion

        #region Overrides

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cookievalue = Request.Cookies["jobrequest"] == null
                                     ? string.Empty
                                     : Request.Cookies["jobrequest"].Value;

            jobrequest = String.IsNullOrEmpty(cookievalue)
                ? new BatchJobRequest()
                : SerializationUtils.Deserialize<BatchJobRequest>(cookievalue);
        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            Response.Cookies["jobrequest"].Value = SerializationUtils.Serialize(jobrequest);
        }

        #endregion

        #region Action Metods

        /// <summary>
        /// Upload a file and create new BatchRequest
        /// </summary>
        /// <param name="id"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public ActionResult UploadFile(string id)
        {
            jobrequest = new BatchJobRequest {Category = id, ManifestId = Guid.NewGuid()};
            ViewData["Category"] = jobrequest.Category;
            var userid = Request["userid"];
            if (!String.IsNullOrEmpty(userid))
                jobrequest.UserId = new Guid(userid);

            var scheme = Uri.UriSchemeHttps;
#if DEBUG
            // If we're running in VS we need to use the http protocol so we override it here
            if (this.Request.Url.Host.EndsWith("localhost", StringComparison.OrdinalIgnoreCase)) scheme = Uri.UriSchemeHttp;
#endif
            var request = new UploadRequest(this.Url.Action(nameof(this.DynamicAppend), "Batch", null, scheme))
            {
                ConvertToCsv = true
            };

            var uri = request.CreateRequest(this.encryption);
            jobrequest.PostbackUri = uri;

            return View(jobrequest);
        }

        /// <summary>
        /// Start job from previously uploaded file and create new BatchRequest
        /// </summary>
        [AcceptVerbs(HttpVerbs.Get)]
        public async Task<ActionResult> FromAdminFile(Guid fileId, int? dealid, CancellationToken cancellation)
        {
            // need to pass instructions to this view
            jobrequest = new BatchJobRequest { ManifestId = Guid.NewGuid() };
            jobrequest.InputFile = new List<BatchJobRequestFile>();

            if (dealid != null)
            {
                var deals = (from d in this.Context.SetOf<Deal>()
                    where d.Id == dealid
                    select new { d.Client.Logon.UserName, d.Instructions }).AsNoTracking();
                jobrequest.Instructions = (await deals.FirstOrDefaultAsync(cancellation))?.Instructions;
            }

            var userFile = await this.Context.SetOf<UserFile>()
                    .Include(f => f.Owner)
                    .FirstOrDefaultAsync(f => f.Id == fileId && !f.Owner.IsLockedOut, cancellation);
            if (userFile == null)
            {
                this.TempData["message"] = $"No file with id {fileId} exists or the account is locked out";
                return this.View("~/Views/Shared/Error.aspx");
            }

            jobrequest.UserId = userFile.Owner.Id;

            var identifier = Guid.NewGuid();
            var filename = JobPipeline.CleanFileName(Path.GetFileName(userFile.CustomerFileName));
            var fileExtension = (Path.GetExtension(filename) ?? String.Empty).ToLowerInvariant();
            var filePath = this.temp.CreateInstance($"{identifier}{fileExtension}");

            if (!JobPipeline.IsSupported(filePath))
            {
                this.TempData["message"] = $"File with extension {fileExtension} are not supported";
                return this.View("~/Views/Shared/Error.aspx");
            }

            var file = this.assistedFiles.CreateInstance(userFile.FileName);
            using (var readFrom = file.OpenStream(FileAccess.Read, true))
            {
                using (var writeTo = filePath.OpenStream(FileAccess.Write, true))
                {
                    await readFrom.CopyToAsync(writeTo, cancellation);
                }
            }

            if (JobPipeline.IsExcel(filePath))
            {
                var excelFile = new Plugin.Storage.ExcelFile(Plugin.Storage.ExcelFile.DetermineExcelFormat(filePath), filePath);
                filePath = await JobPipeline.HandleExcel(excelFile, identifier, this.temp, this.rawCustomerFiles, cancellation);
                filename = Path.ChangeExtension(filename, "csv");
            }

            // backup to Raw Customer Files
            await JobPipeline.BackupClientInputFileAsync(filePath, this.rawCustomerFiles, identifier, cancellation);

            jobrequest.Delimiter = await CsvFileContent.DiscoverDelimiterAsync(filePath, cancellation);
            jobrequest.InputFile.Add(await BatchJobRequestFile.Create(filePath, filename, identifier, this.inbox, cancellation));

            await this.LogEventAsync("Batch, upload file");

            return RedirectToAction("DynamicAppend", new {category = "userfile"});
        }

        /// <summary>
        /// Start job from previously generated listbuilder file and create new BatchRequest
        /// </summary>
        [AcceptVerbs(HttpVerbs.Get)]
        public async Task<ActionResult> FromListBuilder(Guid id, CancellationToken cancellation)
        {
            // need to pass instructions to this view
            jobrequest = new BatchJobRequest { ManifestId = Guid.NewGuid() };
            jobrequest.InputFile = new List<BatchJobRequestFile>();

            var listFile = this.rawCustomerFiles.CreateInstance(id.ToString());

            if (!listFile.Exists())
            {
                this.TempData["message"] = $"No list builder file with id {listFile} exists";
                return this.View("~/Views/Shared/Error.aspx");
            }

            jobrequest.UserId = this.User.Identity.GetIdentifier(); // We submit under the current identity

            var filename = JobPipeline.CleanFileName($"ListBuilder: {id}");

            jobrequest.Delimiter = (await CsvFileContent.DiscoverDelimiterAsync(listFile, cancellation)) ?? CsvFileContent.DefaultDelimiter;
            jobrequest.InputFile.Add(await BatchJobRequestFile.Create(listFile, filename, id, this.inbox, cancellation));

            await this.LogEventAsync("Batch, from listbuilder file");

            return this.RedirectToAction("DynamicAppend", new { category = "userfile" });
        }

        /// <summary>
        /// Start job from previously completed job and create new BatchRequest
        /// </summary>
        [AcceptVerbs(HttpVerbs.Get)]
        public async Task<ActionResult> FromExistingJob(Int32 jobId, Guid userId, CancellationToken cancellation)
        {
            jobrequest = new BatchJobRequest {ManifestId = Guid.NewGuid(), UserId = userId};
            jobrequest.InputFile = new List<BatchJobRequestFile>();

            var existingJob = await this.Context
                .SetOf<Job>()
                .Where(j => j.Id == jobId && j.Status == JobStatus.Complete && j.RunningOn == null)
                .Include(j => j.Lookups)
                .FirstOrDefaultAsync(cancellation);
            if (existingJob == null)
            {
                this.TempData["message"] = $"No job with id {jobId} exists, is not complete, or has an exclusive lock by a system at this time. Please try again later.";
                return this.View("~/Views/Shared/Error.aspx");
            }

            if (existingJob.AccessLookups().SensitiveData)
            {
                this.TempData["message"] = $"Job with id {jobId} was processed with immediate delete. New jobs cannot be created from this output.";
                return this.View("~/Views/Shared/Error.aspx");
            }

            var identifier = Guid.NewGuid();
            var filename = existingJob.CustomerFileName;

            var existingFile = existingJob.JobFiles().CsvOutputFile(this.outbox);
            if (!existingFile.Exists())
            {
                this.TempData["message"] = $"Job with id {jobId} output file no longer exists on our system.";
                return this.View("~/Views/Shared/Error.aspx");
            }

            var filePath = this.temp.CreateInstance($"{identifier}");

            await existingFile.CopyToAsync(filePath, cancellation);

            // backup to Raw Customer Files
            await JobPipeline.BackupClientInputFileAsync(filePath, this.rawCustomerFiles, identifier, cancellation);

            jobrequest.Delimiter = (await CsvFileContent.DiscoverDelimiterAsync(filePath, cancellation)) ?? CsvFileContent.DefaultDelimiter;
            jobrequest.InputFile.Add(await BatchJobRequestFile.Create(filePath, filename, identifier, this.inbox, cancellation));

            await this.LogEventAsync("Batch, upload file");

            return RedirectToAction("DynamicAppend", new { category = "userfile" });
        }

        /// <summary>
        /// Create a manifest
        /// </summary>
        /// <param name="category"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public virtual async Task<ActionResult> DynamicAppend(String category, String userid, CancellationToken cancellation)
        {
            jobrequest.Category = string.IsNullOrEmpty(category) ? jobrequest.Category : category;

            if (jobrequest.Category != "DownloadManifest" && jobrequest.Category != "userfile" && jobrequest.Category != "AddAutoRule_AutoMap")
            {
                var result = UploadResult.HandleFromPostback(this.Request.QueryString, this.encryption);
                var identifier = result.Identifier;
                var customerFileName = JobPipeline.CleanFileName(Path.GetFileName(result.ClientFileName));
                var file = this.temp.CreateInstance(result.SystemFileName);

                // Kendo upload control validates the file extension so we shouldn't hit this but defensive programming
                if (!JobPipeline.IsSupported(file)) return this.View("Error");

                // determine if file contents are ',' or '|' or '\t' delimited
                var fileDelimiter = await CsvFileContent.DiscoverDelimiterAsync(file, cancellation);
                fileDelimiter = fileDelimiter ?? CsvFileContent.DefaultDelimiter;

                jobrequest.Delimiter = fileDelimiter;
                jobrequest.InputFile.Add(await BatchJobRequestFile.Create(file, customerFileName, identifier, this.inbox, cancellation));

                // backup to Raw Customer Files
                await JobPipeline.BackupClientInputFileAsync(file, this.rawCustomerFiles, identifier, cancellation);
            }

            if (!String.IsNullOrEmpty(userid))
            {
                jobrequest.UserId = new Guid(userid);
            }
            // auto-mapped rules start at this step so we need to name the manifest here
            if (jobrequest.Category == "AddAutoRule_AutoMap") jobrequest.ManifestId = Guid.NewGuid();

            ViewData["Category"] = jobrequest.Category;
            ViewData["InputFile"] = jobrequest.InputFile;

            return View(jobrequest);
        }

        /// <summary>
        /// Create a manifest
        /// </summary>
        /// <param name="coll"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> DynamicAppend(FormCollection coll, CancellationToken cancellation)
        {
            var manifestBuilder = BuildManifestFromJson(coll["manifest"]);
            manifestBuilder.ManifestId = this.jobrequest.ManifestId;
            manifestBuilder.UserId = this.jobrequest.UserId;

            var suppressionid = coll["suppressionid"];
            if (!String.IsNullOrEmpty(suppressionid)) manifestBuilder.SupressionId = new Guid(suppressionid);

            manifestBuilder.InputFieldDelimiter = this.jobrequest.Delimiter.ToString();
            manifestBuilder.OutputFieldDelimiter = this.jobrequest.Delimiter.ToString();
            manifestBuilder.UserId = this.jobrequest.UserId;

            using (var uow = this.Context.CreateScope(ScopeOptions.AutoCommit))
            {
                var manifests = Context.SetOf<ManifestCache>();
                var user = (await this.Context.SetOf<User>().FirstOrDefaultAsync(u => u.Id == this.jobrequest.UserId, cancellation)) ??
                           await this.Context.CurrentUserAsync(cancellation);
                var manifestEntity = new ManifestCache(user, manifestBuilder.ToXml());
                manifests.Add(manifestEntity);

                await uow.CommitAsync(cancellation);

                this.jobrequest.ManifestId = manifestEntity.Id;
            }

            this.ViewData["InputFile"] = this.jobrequest.InputFile;

            // used by AutoProcessorRules
            if (this.jobrequest.Category == "AddAutoRule_AutoMap")
            {
                return this.RedirectToAction("Index", "Summary",
                                             new
                                             {
                                                 userid = this.jobrequest.UserId,
                                                 manifestId = this.jobrequest.ManifestId,
                                                 area = "SmtpRules"
                                             });
            }

            this.jobrequest.Product = new BatchJobRequestProduct { Key = "DynamicAppend" };

            await this.LogEventAsync("Batch, build product using dynamic append.");

            return this.RedirectToAction("IdentifyColumns");
        }

        /// <summary>
        /// Used as a starting point to update the column map on an existing job
        /// </summary>
        /// <param name="jobid"></param>
        /// <returns></returns>
        public virtual async Task<ActionResult> UpdateColumnMap(int jobid, CancellationToken cancellation)
        {
            using (this.Context.CreateScope(ScopeOptions.ReadOnly))
            {
                var job = await this.Context.SetOf<Job>()
                    .Where(j => j.Id == jobid)
                    .Select(j => new {j.InputFileName, j.CustomerFileName, j.FileSize, j.TotalRecords})
                    .FirstAsync(cancellation);

                this.jobrequest.Category = "Remap";
                var batchrequestfile = new BatchJobRequestFile
                {
                    InputFileName = job.InputFileName,
                    ClientFileName = job.CustomerFileName,
                    FileLength = job.FileSize,
                    RecordCount = job.TotalRecords
                };
                this.jobrequest.JobId = jobid;
                this.jobrequest.InputFile = new List<BatchJobRequestFile> {batchrequestfile};

                await this.LogEventAsync($"Batch, update column map for existing job {jobid}.");

                return this.RedirectToAction("IdentifyColumns");
            }
        }

        /// <summary>
        /// Maps
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ActionResult> IdentifyColumns(CancellationToken cancellation)
        {
            var uploadedFile = jobrequest.InputFile.First().InputFilePath(this.rawCustomerFiles);

            using (var readFrom = uploadedFile.OpenStream(FileAccess.Read))
            {
                using (var reader = new CsvReader(readFrom, Encoding.Default))
                {
                    reader.Settings.Delimiter = this.jobrequest.Delimiter ?? (await CsvFileContent.DiscoverDelimiterAsync(uploadedFile) ?? CsvFileContent.DefaultDelimiter);
                    var dt = reader.ReadToEnd(false, 10);
                    var colcount = dt.Columns.Count;
                    // detect header row
                    jobrequest.HasHeaderRow = await ColumnMapper.Tests.Header.IsHeaderLine(dt.Rows[0].ItemArray.Select(x => x.ToString()).ToArray());
                    for (var i = 1; i <= colcount; i++)
                    {
                        var sample = (from DataRow row in dt.Rows select row[i - 1].ToString()).ToList();
                        ViewData[(i).ToString()] = sample;
                    }
                    ViewData["ColCount"] = colcount;

                    // HACK
                    using (this.Context.CreateScope(ScopeOptions.NoTracking))
                    {
                        XElement xml;
                        if (this.jobrequest.JobId > 0)
                        {
                            xml = (await this.Context.SetOf<Job>().FirstAsync(j => j.Id == this.jobrequest.JobId, cancellation)).Manifest;
                        }
                        else
                        {
                            xml = (await this.Context.SetOf<ManifestCache>().FirstAsync(m => m.Id == this.jobrequest.ManifestId, cancellation)).Manifest;
                        }

                        var manifest = new ManifestBuilder(xml);
                        // manifest.InputFields should contain PhoneNumber
                        ViewData["InputColumns"] = BuildColumnDropDown(manifest.DetermineInputFields().Where(a => !a.MetaFieldName.StartsWith("_")));  // fields with underscore should not be mapped
                        var serializer = new JavaScriptSerializer();
                        var s = serializer.Serialize(manifest.DetermineInputFields().Where(a => a.Required).Select(a => a.MetaFieldName));
                        ViewData["RequiredFields"] = s;
                    }
                }
            }
            return View(jobrequest);
        }

        /// <summary>
        /// Used to associate a column with a field
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual async Task<ActionResult> IdentifyColumns(FormCollection collection, CancellationToken cancellation)
        {
            var userid = (await this.Context.CurrentUserAsync(cancellation)).Id;

            this.jobrequest.UserId = jobrequest.UserId == Guid.Empty ? userid : this.jobrequest.UserId;
            this.jobrequest.HasHeaderRow = bool.Parse(collection["HasHeaderRow"]);

            var columnmap = "";
            for (var i = 1; i <= Int32.Parse(collection["colcount"]); i++)
            {
                columnmap += (collection[i.ToString()] == "" ? "Unknown" : collection[i.ToString()]) + ";";
            }
            foreach (var f in this.jobrequest.InputFile)
            {
                f.ColumnMap = columnmap;
            }

            // used for auto-processor rule creation
            if (jobrequest.Category == "AddAutoRule")
            {
                using (var uow = this.Context.CreateScope(ScopeOptions.AutoCommit))
                {
                    var manifests = this.Context.SetOf<ManifestCache>();
                    var manifestEntity = await manifests.FirstOrDefaultAsync(m => m.Id == this.jobrequest.ManifestId, cancellation);
                    if (manifestEntity != null)
                    {
                        var map = new ColumnMap(columnmap);
                        var manifest = manifestEntity.Manifest;
                        manifest.AddFirst(map.ToXml());
                        manifest.HasHeaderRow(this.jobrequest.HasHeaderRow);
                        manifestEntity.ReplaceManifest(manifest);

                        await uow.CommitAsync(cancellation);
                    }
                }

                return this.RedirectToAction("Index", "Summary",
                    new
                    {
                        userid = this.jobrequest.UserId,
                        manifestId = this.jobrequest.ManifestId,
                        Area = "SmtpRules"
                    });
            }

            XElement xml;

            using (var uow = this.Context.CreateScope(ScopeOptions.AutoCommit))
            {
                if (this.jobrequest.JobId > 0)
                {
                    var job = await this.Context.SetOf<Job>().FirstAsync(j => j.Id == this.jobrequest.JobId, cancellation);
                    // insert columnmap into Manifest
                    var manifest = new ManifestBuilder(job.Manifest)
                    {
                        ColumnMap = new ColumnMap(columnmap),
                        HasHeaderRow = this.jobrequest.HasHeaderRow
                    };

                    xml = manifest.ToXml();
                    job.Manifest.ColumnMap(xml.ColumnMap());
                }
                else
                {
                    var cache = await this.Context.SetOf<ManifestCache>().FirstAsync(m => m.Id == this.jobrequest.ManifestId, cancellation);
                    // insert columnmap into Manifest
                    var manifest = new ManifestBuilder(cache.Manifest)
                    {
                        ColumnMap = new ColumnMap(columnmap),
                        HasHeaderRow = this.jobrequest.HasHeaderRow
                    };

                    xml = manifest.ToXml();
                    xml.HasHeaderRow(this.jobrequest.HasHeaderRow);
                    cache.Manifest.ColumnMap(xml.ColumnMap());
                    cache.Manifest.HasHeaderRow(xml.HasHeaderRow());
                }

                await uow.CommitAsync(cancellation);
            }

            if (this.jobrequest.Category == "Remap")
            {
                // update manifest in job
                using (var uow = this.Context.CreateScope(ScopeOptions.AutoCommit))
                {
                    var job = await this.Context.SetOf<Job>().FirstAsync(j => j.Id == this.jobrequest.JobId, cancellation);
                    job.Manifest.ColumnMap(xml.ColumnMap());

                    await uow.CommitAsync(cancellation);

                    return this.NavigationFor<ResetController>().Interactive(this.jobrequest.JobId);
                }
            }

            if (this.jobrequest.Category == "AddAutoRule")
            {
                return this.RedirectToAction("Index", "Summary",
                    new
                    {
                        userid = this.jobrequest.UserId,
                        manifestId = this.jobrequest.ManifestId,
                        Area = "SmtpRules"
                    });
            }

            // insert in jobqueue
            await this.QueueJobRequest(this.jobrequest);

            return this.NavigationFor<SummaryController>().ToIndex();
        }

        /// <summary>
        /// Converts Json from manifest builder into xml and downloads as ote
        /// </summary>
        /// <param name="coll"></param>
        /// <returns></returns>
        public ActionResult DownloadManifest(FormCollection coll)
        {
            var manifestBuilder = BuildManifestFromJson(coll["manifest"]);
            return this.File(Encoding.UTF8.GetBytes(manifestBuilder.ToXml().ToString()), "application/xml", "manifest.xml");
        }

        #endregion

        #region HELPERS

        /// <summary>
        /// Builds new ManifestBuilder object from Json string
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private static ManifestBuilder BuildManifestFromJson(string json)
        {
            var manifestBuilder = new ManifestBuilder();
            manifestBuilder.Operations.AddRange(JObject.Parse(json).SelectToken("Operations")
                .Select(o =>
                {
                    var operationName = EnumExtensions.Parse<DataServiceOperation>(o["Name"].ToString());
                    var operation = new OperationDefinition();
                    operation.Name = operationName;
                    foreach (var matchLevel in o["MatchLevels"])
                    {
                        operation.MatchLevels.Add(new MatchLevelDefinition
                        {
                            Name = EnumExtensions.Parse<MatchLevel>(matchLevel["Name"].ToString()),
                            Include = (bool)matchLevel["Include"]
                        });
                    }
                    foreach (var qualityLevel in o["QualityLevels"])
                    {
                        operation.QualityLevels.Add(new QualityLevelDefinition
                        {
                            Name = EnumExtensions.Parse<QualityLevel>(qualityLevel["Name"].ToString()),
                            Include = (bool)qualityLevel["Include"]
                        });
                    }
                    foreach (var source in o["Sources"])
                    {
                        operation.Sources.Add(new SourceDefinition
                        {
                            Name = source["Name"].ToString(),
                            Include = (bool)source["Include"]
                        });
                    }
                    foreach (var field in o["InputFields"])
                    {
                        operation.InputFields.Add(new Field
                        {
                            MetaFieldName = field["MetaFieldName"].ToString(),
                            OperationParamName = field["OperationParamName"].ToString(),
                            Include = (bool)field["Include"],
                            Required = (bool)field["Required"]
                        });
                    }
                    foreach (var field in o["OutputFields"])
                    {
                        operation.OutputFields.Add(new Field
                        {
                            ColumnTitle = field["ColumnTitle"].ToString(),
                            MetaFieldName = field["MetaFieldName"].ToString(),
                            OperationParamName = field["OperationParamName"].ToString(),
                            Include = (bool)field["Include"],
                            Required = (bool)field["Required"]
                        });
                    }

                    return operation;

                }));
            return manifestBuilder;
        }

        /// <summary>
        /// Builds column mapping dropdown
        /// </summary>
        /// <param name="inputfields"></param>
        /// <returns></returns>
        private static string[] BuildColumnDropDown(IEnumerable<Field> inputfields)
        {
            var options = new List<string> {"<option value=''>-- Select Column -</option>"};
            options.AddRange(inputfields.Select(field => "<option required='" + field.Required + "' value='" + field.MetaFieldName + "'>" + field.MetaFieldName + (field.Required ? "*" : "") + "</option>"));
            return options.ToArray();
        }

        /// <summary>
        /// Submits BatchJobRequest into JobQueue for processing
        /// </summary>
        /// <param name="jobrequest"></param>
        private async Task QueueJobRequest(BatchJobRequest jobrequest)
        {
            using (this.Context.CreateScope(ScopeOptions.ReadOnly))
            {
                var cache = await this.Context.SetOf<ManifestCache>().FirstAsync(m => m.Id == jobrequest.ManifestId);

                foreach (var f in jobrequest.InputFile)
                {
                    var command = new CreateAdminJobCommand();
                    command.CustomerFileName = f.ClientFileName;
                    command.Delimiter = cache.Manifest.InputDelimiter();
                    command.JobKey = new Guid(f.InputFileName);
                    command.Manifest = cache.Manifest;
                    command.UserId = jobrequest.UserId;

                    await this.bus.Send(command);
                }
            }
        }

        /// <summary>
        /// Used to grab pre-built manifests
        /// </summary>
        /// <remarks>
        /// Checks manifest directory for a manifest xml file named the same as the product. If it finds a file then it uses that manifest for the job.
        /// This allows us to create manifest files that emulate our existing products.
        /// Supports manifests that are currently too complicated to build in the DynamicAppend
        /// </remarks>
        /// <param name="manifestName"></param>
        /// <returns></returns>
        public ActionResult GetPredefinedManifest(string manifestName)
        {
            var manifestDir = this.legacyManifest;

            var manifestFile = new XmlFile(manifestDir.CreateInstance($"{manifestName}.xml"));
            if (!manifestFile.Exists()) throw new InvalidOperationException($"{manifestFile} manifest does not exist!");

            var manifest = new ManifestBuilder(manifestFile.OpenDocument());

            var jsonNetResult = new JsonNetResult();
            jsonNetResult.Data = manifest;
            return jsonNetResult;
        }

        /// <summary>
        /// Returns Json representation of base manifest definition.
        /// </summary>
        /// <remarks>Used by javascript manifest builder in DynamicAppend</remarks>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActionResult GetOperationDefintion(DataServiceOperation name)
        {
            var manifest = OperationManager.DefaultManifest(name);
            var op = new OperationDefinition(manifest);

            var jsonNetResult = new JsonNetResult();
            jsonNetResult.Data = op;
            return jsonNetResult;
        }

        #endregion
    }
}