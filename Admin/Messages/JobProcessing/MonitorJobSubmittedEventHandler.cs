using System;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AccurateAppend.Accounting.DataAccess;
using AccurateAppend.JobManagement.Contracts;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.Summary;
using AccurateAppend.Websites.Admin.Navigator;
using Microsoft.AspNet.SignalR;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Messages.JobProcessing
{
    /// <summary>
    /// Handler for the <see cref="JobCreatedEvent"/> bus message.
    /// </summary>
    /// <remarks>
    /// Responds to a message by reacting to Client uploaded files via toast.
    /// Then non-admin submitted jobs that have at least 250k records in them
    /// is checked. If found, an internal support email is sent and a push toast
    /// notification is broadcast; Otherwise ignore everything else.
    ///
    /// The toast vs emailing scenarios need much more work so we punted on this one
    /// and combined it all here.
    /// </remarks>
    public class MonitorJobSubmittedEventHandler : IHandleMessages<JobCreatedEvent>
    {
        #region Fields

        private readonly DbMailQueue queue;
        private readonly AccurateAppend.JobProcessing.DataAccess.DefaultContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorJobSubmittedEventHandler"/> class.
        /// </summary>
        /// <param name="queue">The <see cref="DbMailQueue"/> to queue email with.</param>
        public MonitorJobSubmittedEventHandler(DbMailQueue queue, AccurateAppend.JobProcessing.DataAccess.DefaultContext dataContext)
        {
            if (queue == null) throw new ArgumentNullException(nameof(queue));
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));

            this.queue = queue;
            this.dataContext = dataContext;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Since MVC url routing components are not set up by default in a POCO,
        /// this method performs a basic hack to create a <see cref="UrlHelper"/>
        /// that can be used to generate the appropriate paths.
        /// </summary>
        /// <param name="jobId">The identifier of the job to generate a detail view for.</param>
        /// <returns>The path to the detail view of the job.</returns>
        protected virtual String GenerateHelpLink(Int32 jobId)
        {
            var request = new HttpRequest("/", "http://admin.accurateappend.com", String.Empty);
            var response = new HttpResponse(new StringWriter());
            var httpContext = new HttpContext(request, response);
            var httpContextBase = new HttpContextWrapper(httpContext);

            var routeData = new RouteData();
            var myHostContext = new RequestContext(httpContextBase, routeData);

            var url = new UrlHelper(myHostContext);
            var link = new UrlBuilder<SummaryController>(url).ToIndex(jobId, Uri.UriSchemeHttps);

            return link;
        }

        #endregion

        #region IHandleMessages<JobCreatedEvent> Members

        /// <inheritdoc />
        public virtual Task Handle(JobCreatedEvent message, IMessageHandlerContext context)
        {
            if (message.SourceChannel == Source.PublicWebsite) return this.HandlePublic(message);
            if (message.SourceChannel == Source.NationBuilder) return this.HandleNationBuilder(message);
            if (message.RecordCount >= 250000 && message.SourceChannel != Source.Admin) return this.HandleLargeJob(message);

            return Task.CompletedTask;
        }

        private Task HandlePublic(JobCreatedEvent message)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<CallbackHub>();
            hubContext.Clients.All.addNewMessageToPage($"Public client job submitted: {message.JobId}");

            return Task.CompletedTask;
        }

        private Task HandleNationBuilder(JobCreatedEvent message)
        {
            return this.dataContext
                .Database
                .ExecuteSqlCommandAsync(
                    "UPDATE [integration].[NationBuilderPush] SET [JobId]=@p1 WHERE [SupressionId]=@p0",
                    message.JobKey,
                    message.JobId);
        }

        private async Task HandleLargeJob(JobCreatedEvent message)
        {
            var url = this.GenerateHelpLink(message.JobId);
            var email = new MailMessage
            {
                From = new MailAddress("support@accurateappend.com"),
                Subject = $"LARGE FILE NOTIFICATION-{message.ClientFileName}",
                Body = $@"A large file was submitted. Please check the admin job queue.

File Name: {message.ClientFileName}
System File: {message.JobKey}
Total Records: {message.RecordCount}
Details: {url}",
                Priority = MailPriority.High
            };
            email.To.Add(new MailAddress("operations@accurateappend.com"));

            await this.queue.EnqueueMessageAsync(email).ConfigureAwait(false);
        }



        #endregion
    }
}