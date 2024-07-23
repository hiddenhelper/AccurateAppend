using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.Operations;
using AccurateAppend.Operations.Contracts;
using DomainModel;
using DomainModel.ActionResults;
using EventLogger;
using NServiceBus;
using DefaultContext = AccurateAppend.Operations.DataAccess.DefaultContext;

namespace AccurateAppend.Websites.Clients.Areas.Public.File
{
    [AllowAnonymous()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private readonly DefaultContext context;
        private readonly UploadRequestBuilder uploader;
        private readonly IMessageSession bus;
        private readonly IFileLocation assistedFiles;

        #endregion

        #region Constructor

        public Controller(DefaultContext context, UploadRequestBuilder uploader, IMessageSession bus, IFileLocation assistedFiles)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            if (assistedFiles == null) throw new ArgumentNullException(nameof(assistedFiles));
            Contract.EndContractBlock();

            this.context = context;
            this.uploader = uploader;
            this.bus = bus;
            this.assistedFiles = assistedFiles;
        }

        #endregion

        public virtual async Task<ActionResult> Upload(Guid id, CancellationToken cancellation)
        {
            var logon = await this.context.Set<ClientRef>()
                .Where(u => u.UserId == id)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellation);

            if (logon == null) return this.DisplayErrorResult("You have followed an invalid upload link or you are not logged in. Please log in and try your request again.");

            var scheme = UploadRequestBuilder.DetermineScheme(this.Request);
            var postbackTo = this.Url.Action("UploadComplete", "File", new {Area = "Public", id}, scheme);
            
            var uri = this.uploader.CreateRequest(postbackTo);

            return this.View(uri);
        }

        public virtual async Task<ActionResult> UploadComplete(Guid id, CancellationToken cancellation)
        {
            var result = this.uploader.HandleFromPostback(this.Request.QueryString);

            var customerFileName = result.ClientFileName;

            try
            {
                var user = await this.context
                    .Set<ClientRef>()
                    .Where(u => u.UserId == id)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellation);
                if (user == null) return this.DisplayErrorResult("You have followed an invalid upload link");
                
                var identity = this.User.Identity.IsAuthenticated
                    ? this.User.Identity
                    : WellKnownIdentifiers.SystemIdentity; // Revert to system identity if the current account is not logged in
                using (SecurityHelper.Alias(identity))
                {
                    using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await this.StoreFile(id, result);
                        await this.SendEmail(EmailFactory.FileUploadNotifcation(user.UserName, user.UserId, user.ApplicationId, customerFileName));

                        transaction.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached) Debugger.Break();
                Logger.LogEvent(ex, Severity.High, Application.Clients, this.Request.UserHostAddress, "Public File Upload failing.");
                throw;
            }
            TempData["message"] = $"{customerFileName} has been successfully sent to customer support.";
            return this.RedirectToAction("Upload", "File", new { id });
        }

        public virtual async Task<ActionResult> Download(Guid id, CancellationToken cancellation)
        {
            var adminDocument = await this.context.Set<UserFile>()
                .Where(d => d.UploadBy == ContentSource.Admin)
                .Where(d => d.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellation);

            if (adminDocument == null) return this.DisplayErrorResult("You have followed an invalid download link");

            var assistedFile = this.assistedFiles.CreateInstance(adminDocument.FileName);

            if (!assistedFile.Exists()) return new LiteralResult() {Data = "Content does not exist"};

            return new FileProxyResult(assistedFile) {FileDownloadName = adminDocument.CustomerFileName};
        }

        #region Helpers

        protected Task StoreFile(Guid userId, UploadResult upload)
        {
            var requestId = upload.Identifier;
            var customerFileName = upload.ClientFileName;
            var systemFileName = upload.SystemFileName;

            var command = new StoreFileCommand
            {
                UserId = userId,
                CustomerFileName = customerFileName,
                SystemFileName = systemFileName,
                RequestId = requestId
            };

            return this.bus.Send(command);
        }

        protected Task SendEmail(MailMessage email)
        {
            var command = new SendEmailCommand(email);
            return this.bus.Send(command);
        }

        #endregion
    }
}