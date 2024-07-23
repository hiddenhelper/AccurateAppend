using System;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing;
using AccurateAppend.JobProcessing.Manifest;
using AccurateAppend.JobProcessing.Manifest.Xml;
using AccurateAppend.Websites.Admin.Controllers;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.Manifest
{
    [Authorize()]
    public class ManifestController : ContextBoundController
    {
        #region Contructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        public ManifestController(ISessionContext context) : base(context)
        {
        }

        #endregion

        #region Action Methods
        
        /// <summary>
        /// Displays read-only view of manifest from cache
        /// </summary>
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public virtual async Task<ActionResult> Read(Guid id)
        {
            using (this.Context.CreateScope(ScopeOptions.ReadOnly))
            {
                var entity = await this.Context.SetOf<ManifestCache>().Include(m => m.Owner).FirstOrDefaultAsync(m => m.Id == id);

                var result = this.CreateDownload(entity?.Manifest, $"Manifest:{id}", entity?.Owner.Id, null, id);
                return result;
            }
        }

        /// <summary>
        /// Displays read-only view of manifest for a job
        /// </summary>
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public virtual async Task<ActionResult> View(Int32 jobId)
        {
            using (this.Context.CreateScope(ScopeOptions.ReadOnly))
            {
                var entity = await this.Context.SetOf<Job>().Include(j => j.Owner).FirstOrDefaultAsync(j => j.Id == jobId);

                var result = this.CreateDownload(entity?.Manifest, $"Job:{jobId}", entity?.Owner.Id, jobId, null);
                return result;
            }
        }

        /// <summary>
        /// Download manifest in form of .xml file
        /// </summary>
        public virtual async Task<ActionResult> Download(Guid? manifestId, Int32? jobId, CancellationToken cancellation)
        {
            if (manifestId == null && jobId == null)
            {
                this.TempData["message"] = "No manifest or job identifier supplied";
                return this.View("~/Views/Shared/Error.aspx");
            }

            var manifestFileName = "manifest-{0}.xml";

            using (this.Context.CreateScope(ScopeOptions.ReadOnly))
            {
                String manifestContent;

                if (jobId != null)
                {
                    var job = await this.Context.SetOf<Job>().FirstOrDefaultAsync(j => j.Id == jobId, cancellation);
                    if (job == null) return new EmptyResult();

                    var manifest = job.Manifest;
                    manifest.SetAttributeValue("JobId", jobId);

                    manifestContent = job.Manifest.ToString();
                    manifestFileName = String.Format(manifestFileName, jobId);
                }
                else
                {
                    var entity = await this.Context.SetOf<ManifestCache>().FirstOrDefaultAsync(m => m.Id == manifestId, cancellation);
                    if (entity == null) return new EmptyResult();

                    var manifest = entity.Manifest;
                    manifest.SetAttributeValue("ManifestId", manifestId);
                    manifest.SetAttributeValue("JobId", null);

                    manifestContent = manifest.ToString();
                    manifestFileName = String.Format(manifestFileName, manifestId);
                }

                return this.File(Encoding.UTF8.GetBytes(manifestContent), MediaTypeNames.Text.Xml, manifestFileName);
            }
        }

        #endregion

        #region Helpers

        protected virtual ActionResult CreateDownload(XElement manifest, String title, Guid? userId, Int32? jobId, Guid? manifestId)
        {
            if (manifest == null) return new LiteralResult() { Data = "Manifest does not exist" };

            var data = new ManifestBuilder(manifest);
            var xml = data.ToXml();
            xml.UserId(userId);
            xml.JobId(jobId);
            xml.ManifestId(manifestId);

            var result = new LiteralResult(true) { Data = HttpUtility.HtmlEncode(xml), Title = title };
            return result;
        }

        #endregion
    }
}