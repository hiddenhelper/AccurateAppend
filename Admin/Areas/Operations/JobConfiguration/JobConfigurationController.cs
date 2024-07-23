using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;
using AccurateAppend.JobProcessing.Manifest;
using AccurateAppend.Security;

namespace AccurateAppend.Websites.Admin.Areas.Operations.JobConfiguration
{
    [Authorize()]
    public class JobConfigurationController : Controller
    {
        #region Fields

        private readonly DbContext context;

        #endregion

        #region Constructor

        public JobConfigurationController(DbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        public enum Mode
        {
            Fairness = 0,
            Priority = 1
        }

        #region Action Methods

        [HttpGet()]
        public virtual async Task<ActionResult> Scheduling()
        {
            if (!this.User.Identity.IsSuperUser())
            {
                this.TempData["message"] = "Your account does not have access to this feature.";
                return this.View("~/Views/Shared/Error.aspx");
            }

            var value = await this.context.Database.SqlQuery<Mode>(
                        @"SELECT Convert(int, coalesce([Value],0)) FROM [operations].[GlobalConfiguration] WHERE [Setting]='BatchProcessor/SliceRankMode'")
                        .FirstOrDefaultAsync();

            return this.View(value);
        }

        [HttpPost()]
        public virtual async Task<ActionResult> Scheduling(Mode mode)
        {
            if (!this.User.Identity.IsSuperUser())
            {
                this.TempData["message"] = "Your account does not have access to this feature.";
                return this.View("~/Views/Shared/Error.aspx");
            }

            var ra = await this.context.Database.ExecuteSqlCommandAsync(
                @"UPDATE [operations].[GlobalConfiguration] SET [Value]=@p0 WHERE [Setting]='BatchProcessor/SliceRankMode';
UPDATE [operations].[GlobalConfiguration] SET [Value]=@p0 WHERE [Setting]='BatchProcessor_SliceRankMode'",
                (Int32) mode);

            return this.View(mode);
        }

        [HttpGet()]
        public ActionResult Validate()
        {
            return this.View(Enumerable.Empty<ValidationResult>());
        }

        [HttpPost()]
        [ValidateInput(false)]
        public ActionResult Validate(String manifest)
        {
            var builder = new ManifestBuilder(XElement.Parse(manifest));
            return this.View(builder.IsCogent());
        }

        #endregion
    }
}