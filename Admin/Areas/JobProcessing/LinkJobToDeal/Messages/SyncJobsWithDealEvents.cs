using System;
using System.Linq;
using System.Threading.Tasks;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing;
using AccurateAppend.Sales.Contracts.Messages;
using EventLogger;
using NServiceBus;
using DefaultContext = AccurateAppend.JobProcessing.DataAccess.DefaultContext;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.LinkJobToDeal.Messages
{
    /// <summary>
    /// Handler that is used to synchronize job association logic with deal billing.
    /// </summary>
    /// <remarks>
    /// This handler operates as an anti-corruption layer for the Job Processing context by putting a strong
    /// barrier between the Sales context events and the rest of the jobs. It will locate any job instances
    /// that match a deal public key and depending on the event, will add/update or remove the job association
    /// lookup entry. If there is no matching job, no work is performed.
    ///
    /// Jobs that are associated to a deal are filtered from further ability to create and be associated to deals unless the
    /// deal is canceled.
    ///
    /// -<see cref="DealCreatedEvent"/>, <see cref="DealPublicKeyChangedEvent"/>: Locate the matching job (if any) and associate the job to the deal id.
    /// -<see cref="DealCanceledEvent"/>: Locate the matching job (if any) and remove the association with the deal id.
    /// </remarks>
    public class SyncJobsWithDealEvents : IHandleMessages<DealCreatedEvent>, IHandleMessages<DealCanceledEvent>, IHandleMessages<DealPublicKeyChangedEvent>
    {
        #region Fields

        private readonly DefaultContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncJobsWithDealEvents"/> class.
        /// </summary>
        /// <param name="dataContext">The required <see cref="DefaultContext"/> component.</param>
        public SyncJobsWithDealEvents(DefaultContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));

            this.dataContext = dataContext;
        }

        #endregion

        #region Helpers

        protected virtual Task<Job> Find(Guid publicKey)
        {
            return this.dataContext
                .SetOf<Job>()
                .Where(j => j.PublicKey == publicKey)
                .Include(j => j.Lookups)
                .FirstOrDefaultAsync();
        }

        #endregion

        #region IHandleMessages<DealCreatedEvent> Members

        /// <inheritdoc />
        public async Task Handle(DealCanceledEvent message, IMessageHandlerContext context)
        {
            var publicKey = message.PublicKey;

            var job = await this.Find(publicKey).ConfigureAwait(false);
            if (job == null) return;

            job.AccessLookups().AssociatedWithDeal = null;

            await this.dataContext.SaveChangesAsync();
        }

        #endregion

        #region IHandleMessages<DealCanceledEvent> Members

        /// <inheritdoc />
        public async Task Handle(DealCreatedEvent message, IMessageHandlerContext context)
        {
            var dealId = message.DealId;
            var publicKey = message.PublicKey;

            Logger.LogEvent($"Deal created {dealId}, {publicKey}", Severity.None, Application.AccurateAppend_Admin);

            var job = await this.Find(publicKey).ConfigureAwait(false);
            if (job == null) return;

            job.AccessLookups().AssociatedWithDeal = dealId;

            await this.dataContext.SaveChangesAsync();
        }

        #endregion

        #region IHandleMessages<DealPublicKeyChangedEvent> Members

        /// <inheritdoc />
        public async Task Handle(DealPublicKeyChangedEvent message, IMessageHandlerContext context)
        {
            var dealId = message.DealId;
            var oldPublicKey = message.OriginalPublicKey;
            var newPublicKey = message.NewPublicKey;

            // Set the new key
            var job = await this.Find(newPublicKey).ConfigureAwait(false);
            if (job != null)
            {
                Logger.LogEvent($"Associating job {job.Id} with deal {dealId}", Severity.None, Application.AccurateAppend_Admin);
                job.AccessLookups().AssociatedWithDeal = dealId;
            }

            // Remove the old association
            job = await this.Find(oldPublicKey).ConfigureAwait(false);
            if (job != null)
            {
                Logger.LogEvent($"Removing job {job.Id} from deal {dealId}", Severity.None, Application.AccurateAppend_Admin);
                job.AccessLookups().AssociatedWithDeal = null;
            }

            await this.dataContext.SaveChangesAsync();
        }

        #endregion
    }
}