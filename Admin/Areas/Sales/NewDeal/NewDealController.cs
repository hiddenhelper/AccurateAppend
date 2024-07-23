using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Services;
using AccurateAppend.Sales.Contracts.ViewModels;
using AccurateAppend.Websites.Admin.Areas.Sales.DealDetail;
using AccurateAppend.Websites.Admin.Areas.Sales.NewDeal.Models;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Navigator;
using DomainModel;
using EventLogger;

namespace AccurateAppend.Websites.Admin.Areas.Sales.NewDeal
{
    /// <summary>
    /// Controller creating a new <see cref="DealBinder"/>.
    /// </summary>
    [Authorize()]
    public class NewDealController : ActivityLoggingController2
    {
        #region Fields

        private readonly IDealManagementService service;
        private readonly IEncryptor encryption;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NewDealController"/> class.
        /// </summary>
        /// <param name="service">The <see cref="IDealManagementService"/> that provides deal and order content management logic.</param>
        /// <param name="encryption"></param>
        public NewDealController(IDealManagementService service, IEncryptor encryption)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (encryption == null) throw new ArgumentNullException(nameof(encryption));
            Contract.EndContractBlock();

            this.service = service;
            this.encryption = encryption;
        }

        #endregion
        
        #region Actions

        /// <summary>
        /// Upload a file and create new BatchRequest
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public ActionResult UploadFile(Guid userid)
        {
            var model = new NewDealView { UserId = userid};
            var scheme = Uri.UriSchemeHttps;
#if DEBUG
            // If we're running in VS we need to use the http protocol so we override it here
            if (this.Request.Url.Host.EndsWith("localhost", StringComparison.OrdinalIgnoreCase)) scheme = Uri.UriSchemeHttp;
#endif
            var request = new UploadRequest(this.Url.Action("CreateFromUploadFile", "NewDeal", new { userid, Area = "Sales" }, scheme))
            {
                ConvertToCsv = true
            };

            var uri = request.CreateRequest(this.encryption);
            model.PostbackUri = uri;

            return View(model);
        }

        /// <summary>
        /// Performs the action to display the form to create a new <see cref="DealBinder"/> from file upload.
        /// </summary>
        [HttpGet()]
        public virtual async Task<ActionResult> CreateFromUploadFile(Guid userId,  CancellationToken cancellation)
        {
            try
            {
                var result = UploadResult.HandleFromPostback(this.Request.QueryString, this.encryption);
                
                var model = await this.service.Default(userId, cancellation);
                return RedirectToAction("Create", new { userId, result.Identifier });
            }
            catch (Exception ex)
            {
                return this.DisplayErrorResult(ex.Message);
            }
        }

        /// <summary>
        /// Performs the action to display the form to create a new <see cref="DealBinder"/>.
        /// </summary>
        [HttpGet()]
        public virtual async Task<ActionResult> Create(Guid userId, Guid? identifier,  CancellationToken cancellation)
        {
            try
            {
                var model = await this.service.Default(userId, cancellation);
                ViewData["identifier"] = identifier;
                return this.View(model);
            }
            catch (Exception ex)
            {
                return this.DisplayErrorResult(ex.Message);
            }
        }

        /// <summary>
        /// Performs the action to create a new <see cref="DealBinder"/>.
        /// </summary>
        [HttpPost()]
        public virtual async Task<ActionResult> Create(FormCollection formCollection, DealModel model, CancellationToken cancellation)
        {
            model = model ?? new DealModel();
            var identifier = formCollection["Identifier"] ?? string.Empty;

            if (!this.ModelState.IsValid) return this.View(model);

            try
            {
                var dealId = await this.service.Create(model, cancellation);

                this.OnEvent($"Created new deal for user {model.UserId}");

                return this.NavigationFor<DealDetailController>().Detail(dealId);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached) Debugger.Break();

                Logger.LogEvent(ex, Severity.Medium, Application.AccurateAppend_Admin, "Fatal request exception encountered while creating deal");

                return this.DisplayErrorResult(ex.Message);
            }
        }

        #endregion
    }
}
