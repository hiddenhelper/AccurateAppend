using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Messages;
using AccurateAppend.Sales.Contracts.ViewModels;
using AccurateAppend.Sales.DataAccess;
using NServiceBus;

namespace DomainModel.Services
{
    /// <summary>
    /// High level API for sales order process management. That is it abstracts the deal/order lifetime
    /// management process (approve/decline/cancel/expire) that the application leverages.
    /// </summary>
    /// <remarks>
    /// Designed to reduce the surface area and cognitive overhead of interacting with deal lifetime
    /// workflow and events. By hiding this complexity and rules behind this component, we can instead
    /// present a chunky command driven interface for this work.
    /// </remarks>
    public class DealProcessService
    {
        #region Fields

        private readonly DefaultContext context;
        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DealProcessService"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> used to provide data access.</param>
        /// <param name="bus">The <see cref="IMessageSession"/> used to send messages.</param>
        public DealProcessService(DefaultContext context, IMessageSession bus)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            Contract.EndContractBlock();

            this.context = context;
            this.bus = bus;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs the deal approval step.
        /// </summary>
        /// <remarks>
        /// Publishes the <see cref="DealApprovedEvent"/> event.
        /// </remarks>
        /// <param name="dealId">The identifier of the deal to approve.</param>
        /// <param name="reason">The description of the approval step.</param>
        /// <param name="cancellation">The <see cref="CancellationToken"/> used to signal the desire for an asynchronous process to cancel.</param>
        public virtual async Task Approve(Int32 dealId, String reason, CancellationToken cancellation = default(CancellationToken))
        {
            if (String.IsNullOrWhiteSpace(reason)) throw new ArgumentNullException(nameof(reason));

            var deal = await this.context
                .SetOf<DealBinder>()
                .AreInApproval()
                .Include(d => d.Orders)
                .Include(d => d.Client)
                .FirstOrDefaultAsync(d => d.Id == dealId, cancellation)
                .ConfigureAwait(false);

            if (deal == null) throw new InvalidOperationException($"The deal does not exist or is not in the {nameof(DealStatus.Approval)} status.");

            var adminUserId = Thread.CurrentPrincipal.Identity.GetIdentifier();

            var history = new Audit(reason, adminUserId);

            deal.Approve(history);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                await this.context.SaveChangesAsync(cancellation);

                await this.Publish(new DealApprovedEvent
                {
                    Client = deal.Client.UserId,
                    Amount = deal.Amount,
                    DealId = dealId,
                    PublicKey = new Guid(deal.Orders.First().PublicKey)
                });

                if (deal.Status == DealStatus.Complete)
                {
                    await this.Publish(new DealCompletedEvent
                    {
                        Client = deal.Client.UserId,
                        Amount = deal.Amount,
                        DealId = dealId,
                        PublicKey = new Guid(deal.Orders.First().PublicKey)
                    });
                }

                transaction.Complete();
            }
        }

        /// <summary>
        /// Performs the deal decline step.
        /// </summary>
        /// <remarks>
        /// Publishes the <see cref="DealDeclinedEvent"/> event.
        /// </remarks>
        /// <param name="dealId">The identifier of the deal to decline.</param>
        /// <param name="reason">The description of why the deal was declined.</param>
        /// <param name="cancellation">The <see cref="CancellationToken"/> used to signal the desire for an asynchronous process to cancel.</param>
        public virtual async Task Decline(Int32 dealId, String reason, CancellationToken cancellation = default(CancellationToken))
        {
            if (String.IsNullOrWhiteSpace(reason)) throw new ArgumentNullException(nameof(reason));

            var deal = await this.context
                .SetOf<DealBinder>()
                .AreInApproval()
                .Include(d => d.Orders)
                .FirstOrDefaultAsync(d => d.Id == dealId, cancellation)
                .ConfigureAwait(false);

            if (deal == null) throw new InvalidOperationException($"The deal does not exist or is not in the {nameof(DealStatus.Approval)} status.");

            var adminUserId = Thread.CurrentPrincipal.Identity.GetIdentifier();

            var history = new Audit(reason, adminUserId);

            deal.Decline(history);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                await this.context.SaveChangesAsync(cancellation);

                await this.Publish(new DealDeclinedEvent
                {
                    Client = deal.Client.UserId,
                    Amount = deal.Amount,
                    DealId = dealId,
                    PublicKey = new Guid(deal.Orders.First().PublicKey)
                });

                transaction.Complete();
            }
        }

        /// <summary>
        /// Requests the <see cref="ReviewViewModel"/> for the indicated deal.
        /// </summary>
        /// <param name="dealId">The identifier for the deal to perform the review process on.</param>
        /// <param name="cancellation">The <see cref="CancellationToken"/> used to signal the desire for an asynchronous process to cancel.</param>
        /// <returns>THe <see cref="ReviewViewModel"/> describing the deal to be reviewed.</returns>
        public virtual async Task<ReviewViewModel> Review(Int32 dealId, CancellationToken cancellation = default(CancellationToken))
        {
            var deal = await this.context
                .SetOf<DealBinder>()
                .AreInApproval()
                .Where(d => d.Id == dealId)
                .Include(d => d.Orders)
                .Include(d => d.Orders.Select(o => o.Content))
                .SingleOrDefaultAsync(cancellation)
                .ConfigureAwait(false);
                
            if (deal == null) throw new InvalidOperationException($"Deal does not exist or is not in the {DealStatus.Approval} state.");

            var message = deal.OriginatingOrder().Content;

            var model = new ReviewViewModel();
            message.BccTo.ForEach(e => model.BccTo.Add(e.Address));
            model.Body = message.Body;
            model.Description = deal.Description;
            model.Id = deal.Id.Value;
            model.IsHtml = message.IsHtml;
            model.SendFrom = message.SendFrom;
            message.SendTo.ForEach(e => model.SendTo.Add(e.Address));
            model.Subject = message.Subject;
            model.UserId = deal.Client.UserId;
            message.Attachments.ForEach(a => model.Files.Add(new File { FileName = a.FilePath, SendFileName = a.SendFileName }));

            return model;
        }

        /// <summary>
        /// Performs the deal cancellation action.
        /// </summary>
        /// <remarks>
        /// Publishes the <see cref="DealCanceledEvent"/> event.
        /// </remarks>
        /// <param name="dealId">The identifier of the deal to be canceled.</param>
        /// <param name="cancellation">The <see cref="CancellationToken"/> used to signal the desire for an asynchronous process to cancel.</param>
        public async Task Cancel(Int32 dealId, CancellationToken cancellation = default(CancellationToken))
        {
            var deal = await this.context
                .SetOf<DealBinder>()
                .Where(d => d.Id == dealId)
                .AreEditable()
                .FirstOrDefaultAsync(cancellation)
                .ConfigureAwait(false);
            if (deal == null) return;

            var adminUserId = Thread.CurrentPrincipal.Identity.GetIdentifier();
            var audit = new Audit("Canceled by user", adminUserId);
            var publicKey = new Guid(deal.Orders.First().PublicKey); // Must capture this prior to cancel as the value mutates

            deal.Cancel(audit);

            // Was this a ledger deal for an account?
            var accountPublicKey = await this.context
                .SetOf<LedgerEntry>()
                .Where(e => e.WithDeal.Id == dealId)
                .Select(e => (Guid?)e.ForAccount.PublicKey)
                .SingleOrDefaultAsync(cancellation);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                await this.context.SaveChangesAsync(cancellation);

                await this.Publish(new DealCanceledEvent
                {
                    Client = deal.Client.UserId,
                    DealId = dealId,
                    PublicKey = publicKey,
                    BillingAccount = accountPublicKey ?? Guid.Empty
                });

                transaction.Complete();
            }
        }

        /// <summary>
        /// Performs the deal billing expiration action.
        /// </summary>
        /// <remarks>
        /// Publishes the <see cref="DealBillingExpiredEvent"/> event.
        /// </remarks>
        /// <param name="dealId">The identifier of the deal to be expired.</param>
        /// <param name="cancellation">The <see cref="CancellationToken"/> used to signal the desire for an asynchronous process to cancel.</param>
        public async Task Expire(Int32 dealId, CancellationToken cancellation = default(CancellationToken))
        {
            var deal = await this.context
                .SetOf<DealBinder>()
                .Where(d => d.Id == dealId)
                .CanBeBilled()
                .Where(d => d.Orders.SelectMany(o => o.Transactions).All(c => c.Status != TransactionResult.Approved))
                .FirstOrDefaultAsync(cancellation)
                .ConfigureAwait(false);
            if (deal == null) return;

            var adminUserId = Thread.CurrentPrincipal.Identity.GetIdentifier();
            var audit = new Audit("Expired by user", adminUserId);

            deal.Expire(audit);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                await this.context.SaveChangesAsync(cancellation);

                await this.Publish(new DealBillingExpiredEvent
                {
                    Client = deal.Client.UserId,
                    DealId = dealId,
                    PublicKey = new Guid(deal.Orders.First().PublicKey)
                });
                
                transaction.Complete();
            }
        }

        #endregion

        #region Helpers

        protected virtual Task Publish<T>(T message) where T : IEvent
        {
            return this.bus.Publish(message);
        }

        #endregion
    }
}
