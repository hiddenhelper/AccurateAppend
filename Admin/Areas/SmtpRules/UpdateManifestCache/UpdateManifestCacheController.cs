using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AccurateAppend.Accounting;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing;
using AccurateAppend.Websites.Admin.Controllers;

namespace AccurateAppend.Websites.Admin.Areas.SmtpRules.UpdateManifestCache
{
    /// <summary>
    /// Handles functionality related to updating the <see cref="ManifestCache"/>
    /// </summary>
    [Authorize()]
    public class UpdateManifestCacheController : ContextBoundController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateManifestCacheController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        public UpdateManifestCacheController(ISessionContext context) : base(context)
        {
        }

        /// <summary>
        /// Updates a manifest in <see cref="ManifestCache"/> for a given <see cref="SmtpAutoprocessorRule"/>
        /// </summary>
        /// <param name="files"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual async Task<ActionResult> Index(IEnumerable<HttpPostedFileBase> files,
            CancellationToken cancellation)
        {

            //using (var uow = this.Context.CreateScope(ScopeOptions.AutoCommit))
            //{
            //    var manifests = Context.SetOf<ManifestCache>();
            //    var user = (await this.Context.SetOf<User>().FirstOrDefaultAsync(u => u.Id == this.jobrequest.UserId, cancellation)) ??
            //               await this.Context.CurrentUserAsync(cancellation);
            //    var manifestEntity = new ManifestCache(user, manifestBuilder.ToXml());
            //    manifests.Add(manifestEntity);

            //    await uow.CommitAsync(cancellation);

            //    this.jobrequest.ManifestId = manifestEntity.Id;
            //}

            return View();
        }
    }
}