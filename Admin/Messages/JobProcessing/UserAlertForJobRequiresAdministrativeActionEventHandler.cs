using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.JobManagement.Contracts;
using AccurateAppend.JobProcessing;
using AccurateAppend.Messaging;
using EventLogger;
using Microsoft.AspNet.SignalR;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Messages.JobProcessing
{
    /// <summary>
    /// Handler for the <see cref="JobRequiresAdministrativeActionEvent"/> bus message.
    /// </summary>
    /// <remarks>
    /// Responds to a message by creating a SignalR push notification to any connected clients.
    /// </remarks>
    public class UserAlertForJobRequiresAdministrativeActionEventHandler : IHandleMessages<JobRequiresAdministrativeActionEvent>
    {
        #region Fields

        private readonly ISessionContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAlertForJobRequiresAdministrativeActionEventHandler"/> class.
        /// </summary>
        /// <param name="dataContext">The required <see cref="ISessionContext"/> component.</param>
        public UserAlertForJobRequiresAdministrativeActionEventHandler(ISessionContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
        }

        #endregion

        #region IHandleMessages<JobRequiresAdministrativeActionEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(JobRequiresAdministrativeActionEvent message, IMessageHandlerContext context)
        {
            var jobId = message.JobId;

            using (new Correlation(context.DefaultCorrelation()))
            {
                var status = await this.dataContext.SetOf<Job>()
                    .Where(j => j.Id == jobId)
                    .Select(j => (JobStatus?) j.Status)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                // Job was deleted so bail
                if (status == null) return;

                String notification;

                switch (status)
                {
                    case JobStatus.EmailVerifyError:
                        notification = $"Job {jobId} has an EVS batch failure.";
                        break;
                    case JobStatus.Failed:
                        notification = $"Job {jobId} has failed appending.";
                        break;
                    case JobStatus.NeedsReview:
                        notification = $"Job {jobId} requires review before completing.";
                        break;
                    default:
                        // Already handled for ignore
                        return;
                }

                var hubContext = GlobalHost.ConnectionManager.GetHubContext<CallbackHub>();
                hubContext.Clients.All.addNewMessageToPage(notification);
            }
        }

        #endregion
    }
}