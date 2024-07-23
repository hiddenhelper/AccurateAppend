using System;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Web.Mvc;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing;
using AccurateAppend.Websites.Admin.Controllers;

namespace AccurateAppend.Websites.Admin.Areas.SmtpRules.DownloadManifest
{
    /// <summary>
    /// </summary>
    [Authorize]
    public class DownloadManifestController : ContextBoundController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadManifestController" /> class.
        /// </summary>
        /// <param name="context"></param>
        public DownloadManifestController(ISessionContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Download xml manifest document for a given rule
        /// </summary>
        /// <param name="id">manifestId</param>
        /// <returns></returns>
        public ActionResult Index(Guid id)
        {
            using (Context.CreateScope(ScopeOptions.ReadOnly))
            {
                var manifests = Context.SetOf<ManifestCache>();
                var manifestEntity = manifests.FirstOrDefault(m => m.Id == id);
                if (manifestEntity == null)
                    return File(Encoding.UTF8.GetBytes("No manifest present"), MediaTypeNames.Text.Xml,
                        $"{id}.xml");

                return File(Encoding.UTF8.GetBytes(manifestEntity.Manifest.ToString()), MediaTypeNames.Text.Xml,
                    $"{id}.xml");
            }
        }
    }
}