using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.Operations;
using AccurateAppend.Operations.Contracts;
using AccurateAppend.Websites.Admin.Areas.Clients.UserFiles.Models;
using AccurateAppend.Websites.Admin.Controllers;
using DomainModel;
using DomainModel.ActionResults;
using EventLogger;
using NServiceBus;
using DefaultContext = AccurateAppend.Operations.DataAccess.DefaultContext;

namespace AccurateAppend.Websites.Admin.Areas.Clients.UserFiles
{
    /// <summary>
    /// Functionality to save and manage customer documents
    /// </summary>
    [Authorize()]
    public class UserFilesController : Controller
    {
        #region Fields

        private readonly DefaultContext context;
        private readonly IEncryptor encryption;
        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextBoundController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> to use for this controller instance.</param>
        /// <param name="encryption">The encryption component used to securely build and handle upload actions over the public internet.</param>
        public UserFilesController(DefaultContext context, IEncryptor encryption, IMessageSession bus)
        {
            this.context = context;
            this.encryption = encryption;
            this.bus = bus;
        }

        #endregion

        #region Action Methods
        
        public virtual async Task<ActionResult> Index(String email, Guid? userId, CancellationToken cancellation)
        {
            var model = new FilesRequest { UserId = userId, UserName = email };
            if (userId != null || email != null)
            {
                // Since this is kinda a temp solution here...
                var user = await this.context
                    .Set<ClientRef>()
                    .Where(c => c.UserId == userId || c.UserName == email)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellation);

                if (user == null) return this.View(model);

                model.UserId = user.UserId;
                model.UserName = user.UserName;

                var url = user.ApplicationId == WellKnownIdentifiers.TwentyTwentyId
                    ? Properties.Settings.Default.url_Clients2020
                    : Properties.Settings.Default.url_ClientsAccurateAppend;

                var builder = new UriBuilder(url)
                {
                    Path = $"/Public/File/Upload/{user.UserId}"
                };
                if (builder.Uri.IsDefaultPort) builder.Port = -1;

                model.PublicUploadLink = builder.ToString();

                var scheme = Uri.UriSchemeHttps;
#if DEBUG
                // If we're running in VS we need to use the http protocol so we override it here
                if (this.Request.Url.Host.EndsWith("localhost", StringComparison.OrdinalIgnoreCase)) scheme = Uri.UriSchemeHttp;
#endif

                var request = new UploadRequest(this.Url.Action(nameof(this.HandlePostback), "UserFiles", new {Area = "Clients", user.UserId}, scheme));
                var uri = request.CreateRequest(this.encryption);

#if DEBUG
                //uri = request.CreateRequest(this.encryption, UploadRequest.Local); // Uncomment to switch to a local VS instance of the STORAGE app           
#endif

                model.UploadFileLink = uri.ToString();
            }

            return this.View(model);
        }

        public async Task<ActionResult> HandlePostback(Guid userId, CancellationToken cancellation)
        {
            var user = await this.context
                .Set<ClientRef>()
                .FirstAsync(u => u.UserId == userId, cancellation);

            await this.Process(userId, cancellation);

            // Since we're not waiting for signalR callback we can simulate the window for work
            await Task.Delay(TimeSpan.FromSeconds(4), cancellation);

            return this.RedirectToAction("Index", "UserFiles", new {area = "Clients", email = user.UserName});
        }

        public async Task<ActionResult> InvokePostback(Guid userId, CancellationToken cancellation)
        {
            try
            {
                await this.Process(userId, cancellation);

                return new JsonNetResult
                {
                    Data = new
                    {
                        HttpStatusCodeResult = (Int32)HttpStatusCode.OK,
                        Message = "Request processing"
                    }
                };
             }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, Severity.Medium, Application.AccurateAppend_Admin);

                return new JsonNetResult
                {
                    Data = new
                    {
                        HttpStatusCodeResult = (Int32)HttpStatusCode.InternalServerError,
                        ex.Message
                    }
                };
            }
        }

        #endregion

        #region Helpers

        protected virtual async Task Process(Guid userId, CancellationToken cancellation)
        {
            var result = UploadResult.HandleFromPostback(this.Request.QueryString, this.encryption);
            var systemFileName = result.SystemFileName;
            var clientFileName = result.ClientFileName;
            var correlationId = result.CorrelationId;
            
            var command = new StoreFileCommand
            {
                UserId = userId,
                CustomerFileName = clientFileName,
                RequestId = Guid.NewGuid(),
                SystemFileName = systemFileName,
                CorrelationId = correlationId
            };

            await this.bus.Send(command);
        }

        #endregion
    }
}