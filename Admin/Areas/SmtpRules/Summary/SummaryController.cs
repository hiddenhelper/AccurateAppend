using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Accounting;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing;
using AccurateAppend.Websites.Admin.Controllers;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Admin.Areas.SmtpRules.Summary
{
    /// <summary>
    /// Controller for operations related to displaying <see cref="SmtpAutoprocessorRule"/>
    /// </summary>
    [Authorize()]
    public class SummaryController : ContextBoundController
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SummaryController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        public SummaryController(ISessionContext context)
            : base(context)
        {
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Displays summary of rules for client
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="manifestId">ManifestId created by DynamicAppend</param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public virtual async Task<ActionResult> Index(Guid userid, Guid? manifestId, CancellationToken cancellation)
        {
            var client = await this.Context
                .SetOf<Client>()
                .FirstAsync(c => c.Logon.Id == userid, cancellation);

            dynamic model = new ExpandoObject();
            model.UserId = userid;
            model.ManifestId = manifestId;
            model.Username = client.DefaultEmail;
            model.Links = new
            {
                SaveManifest = Url.Action("Index", "UpdateManifestCache", new { Area = "SmtpRules" }),
                NewMappedRule = $"/Batch/UploadFile?id=AddAutoRule&userid={userid}", 
                NewAutoMappedRule = $"/Batch/DynamicAppend?category=AddAutoRule_AutoMap&userid={userid}",
                Update = Url.Action("Index", "Update", new { Area = "SmtpRules" }),
                Delete = Url.Action("Index", "Delete", new { Area = "SmtpRules" }),
                ForCurrentUser = Url.Action("ForCurrentUser", "Summary", new { Area = "SmtpRules", id = userid }),
                Download = Url.Action("Index", "DownloadManifest", new { Area = "SmtpRules" })
            };
            return this.View(model);
        }

        /// <summary>
        /// Returns <see cref="SmtpAutoprocessorRule"/> for a 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public virtual ActionResult ForCurrentUser(Guid id)
        {
            using (this.Context.CreateScope(ScopeOptions.NoTracking))
            {
                var data = this.Context.SetOf<SmtpAutoprocessorRule>().AsNoTracking().Where(r => r.Logon.Id == id).Select(r => new
                {
                    rid = r.Id,
                    UserId = r.Logon.Id,
                    DateAdded = r.CreatedDate,
                    r.Terms,
                    r.Description,
                    Subject = r.ForSubject,
                    ManifestId = r.ManifestToRun.Id,
                    Default = r.IsDefault,
                    FileName = r.ForFilename,
                    Body = r.ForBodyContent,
                    RunOrder = r.Order
                });

                var jsonNetResult = new JsonNetResult(DateTimeKind.Utc) {Data = data};

                return jsonNetResult;
            }
        }

        #endregion
    }
}