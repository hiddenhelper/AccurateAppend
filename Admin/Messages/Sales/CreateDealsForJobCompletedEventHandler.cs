using AccurateAppend.Accounting;
using AccurateAppend.Data;
using NServiceBus;
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.JobManagement.Contracts;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Sales.Handlers;

namespace AccurateAppend.Websites.Admin.Messages.Sales
{
    /// <summary>
    /// Handler for the <see cref="JobCompletedEvent"/> bus event that will create a <see cref="Deal"/> for completed client job.
    /// </summary>
    /// <remarks>
    /// Responds to a message by checking if the source channel is public CSV or NB.
    /// When public CSV, if the client does not have an active subscription, a <see cref="CreateCsvJobDealCommand"/> will be issued.
    /// If the client has a current subscription, no action is taken. (this complexity is here until the new Sales context takes over subscriptions)
    ///
    /// When NB, a new <see cref="CreateNationBuilderDealCommand"/> will be issued.
    /// </remarks>
    public class CreateDealsForJobCompletedEventHandler : IHandleMessages<JobCompletedEvent>
    {
        #region Fields

        private readonly DefaultContext dataContext;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateDealsForJobCompletedEventHandler"/> class.
        /// </summary>
        /// <param name="dataContext">The required <see cref="DefaultContext"/> component.</param>
        public CreateDealsForJobCompletedEventHandler(DefaultContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
        }

        #endregion

        #region IHandleMessages<JobCompletedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(JobCompletedEvent message, IMessageHandlerContext context)
        {
            var customerFileName = message.ClientFileName;
            var userId = message.UserId;

            using (SecurityHelper.Alias(WellKnownIdentifiers.SystemIdentity))
            {
                switch (message.SourceChannel)
                {
                    case Source.PublicWebsite:
                        {
                            // Check to see if client has a current contract. If so, roll into that.
                            var accounts = await this.dataContext
                                .SetOf<RecurringBillingAccount>()
                                .Where(a => a.ForClient.UserId == userId)
                                .ToArrayAsync()
                                .ConfigureAwait(false);

                            // has an account, no action to take
                            if (accounts.Any(a => a.IsValidForDate(message.CompletedDate)))
                            {
                                Trace.TraceInformation($"Client {userId} has an active service account. No action to take for job {message.JobId}.");
                                return;
                            }

                            var command = new CreateCsvJobDealCommand
                            {
                                UserId = message.UserId,
                                CustomerFileName = customerFileName,
                                Manifest = message.Manifest,
                                ProcessingReport = message.ProcessingReport,
                                PublicKey = message.JobKey
                            };

                            await context.SendLocal(command);
                            break;
                        }
                    case Source.NationBuilder:
                        {
                            var command = new CreateNationBuilderDealCommand
                            {
                                UserId = message.UserId,
                                PublicKey = message.JobKey,
                                ProcessingReport = message.ProcessingReport
                            };

                            await context.SendLocal(command);
                            break;
                        }
                    default:
                        Trace.TraceInformation($"Job: {message.JobId} was not a Client/NB submitted job. Exiting...");
                        break;
                }
            }
        }

        #endregion
    }
}