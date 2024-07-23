using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccurateAppend.Core;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Messaging;
using AccurateAppend.Sales.Contracts.Messages;
using AccurateAppend.Sales.Contracts.Services;
using AccurateAppend.Sales.Contracts.ViewModels;
using NServiceBus;

namespace AccurateAppend.Sales.Handlers
{
    /// <summary>
    /// Base type that contains common logic and helpers for bill generating handler implementations.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to be handled.</typeparam>
    /// <typeparam name="TAccount">The type of <see cref="RecurringBillingAccount"/> type this handler is designed to interact with.</typeparam>
    public abstract class BillCreationHandler<TMessage, TAccount> : IHandleMessages<TMessage> where TMessage : CreateBillCommandBase where TAccount : RecurringBillingAccount
    {
        #region IHandleMessages<T> Members

        /// <inheritdoc />
        /// <remarks>
        /// The actual NServiceBus Handler method call is always performed by this type instead
        /// of being overriden. The  logic is that the caller identity is aliased (see <see cref="MessageHandlerContextExtensions.Alias"/>)
        /// and a default logging correlation is entered for the duration of the handler (see <see cref="MessageHandlerContextExtensions.DefaultCorrelation"/>).
        ///
        /// The subclass is expected to implement all logic in the following methods:
        /// -<see cref="AcquireAccount"/> to find the correct account to bill
        /// 
        /// -<see cref="CreateDeal"/> to generate and configure the <see cref="LedgerEntry"/> and <see cref="DealBinder"/> that has been generated, if any. If
        /// no deal is generated the subclass is REQUIRED to commit any logic as this handler will complete.
        ///
        /// Any common audit entries will be added after this method. Subclasses are not required to add generic entries.
        ///
        /// -<see cref="OnDealCreated"/> is called to publish bus events
        ///
        /// If the <see cref="RecurringBillingAccount.SpecialProcessing"/> is set or the client is a 2020Connect account, this handler will complete.
        ///
        /// -<see cref="CreateBill"/> is used to generate the appropriate bill content for the order.
        /// The <see cref="BillableOrder.PerformAutoBilling"/> is set to true by default. A subclass may change this behavior in the method.
        ///
        /// Once the <see cref="BillContent"/> is generated, the deal will be automatically reviewed and approved.
        ///
        /// -<see cref="OnDealApproved"/> is called to publish bus events
        /// </remarks>
        public async Task Handle(TMessage message, IMessageHandlerContext context)
        {
            using (context.Alias())
            {
                var account = await this.AcquireAccount(message.Id).ConfigureAwait(false);
                var entry = await this.CreateDeal(account, message.Period);

                // Empty leger means we should bail. Ledger with no deal is handled by subclasses so we can still bail
                if (entry?.WithDeal == null) return;

                var deal = entry.WithDeal;
                using (SecurityHelper.Alias(WellKnownIdentifiers.SystemIdentity))
                {
                    // Even though we're aliasing the caller, this audit is always performed by the system identity
                    deal.Notes.Add("Created by automated billing");
                }

                await this.OnDealCreated(context, deal);

                if (account.SpecialProcessing || account.ForClient.ApplicationId == WellKnownIdentifiers.TwentyTwentyId) return;

                var order = deal.OriginatingOrder();
                order.Bill.ContractType = ContractType.Receipt;
                order.PerformAutoBilling = true;

                order.Content = await this.CreateBill(order, message.Period);

                deal.SubmitForReview(new Audit("Account is set for automatic bill creation", WellKnownIdentifiers.SystemUserId));
                deal.Approve(new Audit("Automatically approved by system", WellKnownIdentifiers.SystemUserId));

                await this.OnDealApproved(context, deal);
            }
        }

        /// <summary>
        /// The subclass defined logic for locating the correct <see cref="RecurringBillingAccount"/> for the identifier.
        /// </summary>
        /// <remarks>
        /// Subclasses may chose to enact additional filtering or selection logic for their implementation.
        /// </remarks>
        /// <param name="id">The identifier of the <see cref="RecurringBillingAccount"/> to use with the billing.</param>
        /// <returns>The <see cref="RecurringBillingAccount"/> for the billing.</returns>
        protected abstract Task<TAccount> AcquireAccount(Int32 id);

        /// <summary>
        /// The specific logic for generating a billing <see cref="LedgerEntry"/>.
        /// </summary>
        /// <remarks>
        /// Subclasses will need to implement the correct logic for generation of the <seealso cref="LedgerEntry"/> for the provided
        /// <paramref name="billingPeriod"/>. The result should never be null though logic may dictate that individual entries may
        /// not have an <see cref="DealBinder"/> generated.
        /// </remarks>
        /// <param name="account">The <see cref="RecurringBillingAccount"/> to generate billing for the <paramref name="billingPeriod"/>.</param>
        /// <param name="billingPeriod">The <see cref="BillingPeriod"/> that we should generate a <see cref="LedgerEntry"/> for.</param>
        /// <returns>The <see cref="LedgerEntry"/> for the billing period. The value will never be null but the <see cref="LedgerEntry"/> may not generate a deal.</returns>
        protected abstract Task<LedgerEntry> CreateDeal(TAccount account, BillingPeriod billingPeriod);

        /// <summary>
        /// Publishes the <see cref="DealCreatedEvent"/> message to the bus. Subclasses may override this
        /// but should always call the base version.
        /// </summary>
        /// <remarks>
        /// To customize the <see cref="DealCreatedEvent"/> created, override the <see cref="CreatedEventFromDeal"/> method.
        /// </remarks>
        /// <param name="context">The context of the currently handled message.</param>
        /// <param name="deal">The <see cref="DealBinder"/> to create the event for. This must be in the InProcess <see cref="DealStatus"/>.</param>
        protected virtual Task OnDealCreated(IMessageHandlerContext context, DealBinder deal)
        {
            return context.Publish(this.CreatedEventFromDeal(deal));
        }

        /// <summary>
        /// Publishes the <see cref="DealApprovedEvent"/> message to the bus. Subclasses may override this
        /// but should always call the base version.
        /// </summary>
        /// <remarks>
        /// To customize the <see cref="DealApprovedEvent"/> created, override the <see cref="ApprovedEventFromDeal"/> method.
        /// </remarks>
        /// <param name="context">The context of the currently handled message.</param>
        /// <param name="deal">The <see cref="DealBinder"/> to create the event for. This must be in the Billing <see cref="DealStatus"/>.</param>
        protected virtual Task OnDealApproved(IMessageHandlerContext context, DealBinder deal)
        {
            return context.Publish(this.ApprovedEventFromDeal(deal));
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Generates a new <see cref="BillContent"/> for the provided <paramref name="order"/>. This logic
        /// should not require customization typically though does rely on several abstract methods that
        /// subclasses will need to implement the correct logic for:
        /// -<see cref="CreateFormatter"/>
        /// -<see cref="CreateAttachments"/>
        /// </summary>
        /// <param name="order">The <see cref="BillableOrder"/> to create a bill for.</param>
        /// <param name="period">The <see cref="DateSpan"/> the bill covers.</param>
        /// <returns>A complete <see cref="BillContent"/>.</returns>
        protected virtual async Task<BillContent> CreateBill(BillableOrder order, DateSpan period)
        {
            var formatter = await this.CreateFormatter(order.Deal.Client.UserId).ConfigureAwait(false);
            var model = new BillModel(order);

            var content = new BillContent(await formatter.SendFrom(model).ConfigureAwait(false));
            content.IsHtml = formatter.IsHtml;
            content.SendTo.AddRange(await formatter.CreateTo(model));
            content.BccTo.AddRange(await formatter.CreateBcc(model));
            content.Subject = await formatter.CreateSubject(model);
            content.Body = $"{await formatter.CreateHeader(model)}{await formatter.CreateBody(order)}{await formatter.CreateFooter(model)}";

            foreach (var attachmentToCreate in this.CreateAttachments(order, period))
            {
                var attachmentFile = await attachmentToCreate;
                content.Attachments.Add(attachmentFile);
            }

            return content;
        }

        /// <summary>
        /// Required to be implemented by a subclass to provide the concrete <see cref="IBillFormatter"/> instance
        /// that should be leveraged by <see cref="CreateBill"/> to generate <see cref="BillContent"/> with.
        /// </summary>
        /// <param name="userId">The identifier of the user that owns the bill being created.</param>
        /// <returns>The concrete <see cref="IBillFormatter"/> instance to be used.</returns>
        protected abstract Task<IBillFormatter> CreateFormatter(Guid userId);

        /// <summary>
        /// Provides a sequence of generated <see cref="FileAttachment"/> instances that should be included as part
        /// of the <see cref="BillContent"/> attachments, if any.
        /// </summary>
        /// <param name="order">The <see cref="BillableOrder"/> to create any attachments that should be included in the bill.</param>
        /// <param name="period">The <see cref="DateSpan"/> the bill covers.</param>
        /// <returns>A sequence of <see cref="FileAttachment"/> instances that should be attached to the bill, if any.</returns>
        protected abstract IEnumerable<Task<FileAttachment>> CreateAttachments(BillableOrder order, DateSpan period);

        #endregion

        #region Bus Events

        /// <summary>
        /// Generates a <see cref="DealCreatedEvent"/> appropriate for the supplied <paramref name="deal"/>.
        /// </summary>
        /// <param name="deal">The <see cref="DealBinder"/> instance that the event is generated for. This must have an Id value and be InProcess.</param>
        /// <returns>A new <see cref="DealCreatedEvent"/> corresponding to the <paramref name="deal"/>.</returns>
        protected virtual DealCreatedEvent CreatedEventFromDeal(DealBinder deal)
        {
            if (deal.Status != DealStatus.InProcess) throw new ArgumentOutOfRangeException(nameof(deal), deal.Status, $"Deal status must be {DealStatus.InProcess} to create a {nameof(DealCreatedEvent)}");
            if (deal.Id == null) throw new ArgumentOutOfRangeException(nameof(deal), deal.Id, $"Deal Id must be persisted and have a generated {nameof(DealBinder.Id)} to create a {nameof(DealCreatedEvent)}");

            var @event = new DealCreatedEvent
            {
                PublicKey = new Guid(deal.Orders.First().PublicKey),
                Amount = deal.Total(),
                Client = deal.OwnerId,
                DateCreated = deal.CreatedDate,
                DealId = deal.Id.Value
            };

            return @event;
        }

        /// <summary>
        /// Generates a <see cref="DealApprovedEvent"/> appropriate for the supplied <paramref name="deal"/>.
        /// </summary>
        /// <param name="deal">The <see cref="DealBinder"/> instance that the event is generated for. This must have be in either Billing or Complete status.</param>
        /// <returns>A new <see cref="DealApprovedEvent"/> corresponding to the <paramref name="deal"/>.</returns>
        protected virtual DealApprovedEvent ApprovedEventFromDeal(DealBinder deal)
        {
            if (deal.Status != DealStatus.Billing | deal.Status == DealStatus.Complete) throw new ArgumentOutOfRangeException(nameof(deal), deal.Status, $"Deal status must be {DealStatus.Billing} | {DealStatus.Complete} to create a {nameof(DealApprovedEvent)}");

            var @event = new DealApprovedEvent()
            {
                PublicKey = new Guid(deal.Orders.First().PublicKey),
                Amount = deal.Total(),
                Client = deal.Client.UserId,
                DealId = deal.Id.Value
            };

            return @event;
        }

        #endregion
    }
}