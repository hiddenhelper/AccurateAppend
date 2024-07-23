using System;
using System.Linq;
using System.Threading.Tasks;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing.Manifest.Xml;
using AccurateAppend.JobProcessing.Reporting;
using AccurateAppend.Messaging;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Messages;
using AccurateAppend.Sales.DataAccess;
using EntityFramework.Extensions;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Areas.Sales.UpdateOrderFromJob.Messaging
{
    /// <summary>
    /// Command handler for the <see cref="UpdateDealFromProcessingReportCommand"/> message. Operates by taking the provided
    /// report and building an updated billable order for the indicated deal.
    /// </summary>
    public class UpdateDealFromProcessingReportCommandHandler : IHandleMessages<UpdateDealFromProcessingReportCommand>
    {
        #region Fields

        private readonly DefaultContext dataContext;
        private readonly IOrderCalculationService calculator;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateDealFromProcessingReportCommandHandler"/> class.
        /// </summary>
        /// <param name="dataContext">The required <see cref="DefaultContext"/> component.</param>
        /// <param name="calculator">The required <see cref="IOrderCalculationService"/> component that performs calculation services..</param>
        public UpdateDealFromProcessingReportCommandHandler(DefaultContext dataContext, IOrderCalculationService calculator)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            if (calculator == null) throw new ArgumentNullException(nameof(calculator));

            this.dataContext = dataContext;
            this.calculator = calculator;
        }

        #endregion

        #region IHandleMessages<UpdateDealFromProcessingReportCommand> Members

        /// <inheritdoc />
        public virtual async Task Handle(UpdateDealFromProcessingReportCommand message, IMessageHandlerContext context)
        { 
            var dealId = message.DealId;
            var publicKey = message.PublicKey;
            var report = new ProcessingReport(message.ProcessingReport);
            var jobId = message.Manifest?.JobId() ?? 0;
            var customerFileName = message.Manifest?.CustomerFileName();

            using (context.Alias())
            {
                var dealQuery = this.dataContext
                    .SetOf<MutableDeal>()
                    .Where(d => d.Id == dealId && d.Status == DealStatus.InProcess);

                dealQuery.Select(d => d.Client).FutureFirstOrDefault();
                dealQuery.Select(d => d.Orders).Future();
                dealQuery.SelectMany(d => d.Orders.Select(o => o.Lines)).Future();
                
                var deal = await dealQuery
                    .Include(d => d.Client)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                if (deal == null) return;

                deal.Title = customerFileName ?? deal.Title;

                var order = deal.OriginatingOrder();

                var costService = new CustomerCostService(order.Deal.Client, this.dataContext);

                // Reset the key if this is new association and add note
                if (order.PublicKey != publicKey.ToString())
                {
                    // Let subscribers know the original and new key
                    var @event = new DealPublicKeyChangedEvent();
                    @event.Client = deal.Client.UserId;
                    @event.DealId = dealId;
                    @event.OriginalPublicKey = new Guid(order.PublicKey);
                    @event.NewPublicKey = publicKey;

                    await context.Publish(@event);

                    order.ResetKey(publicKey);

                    if (jobId > 0) deal.Notes.Add($"Deal associated to job: {jobId}");
                }
                
                // Tag the processing report to the order
                order.Processing.AssociateReport(message.ProcessingReport);
                
                await this.calculator.FillFromRateCard(order, costService, report);

                deal.Amount = deal.Total(); // HACK: we don't have the events in place to keep this synched

                await this.dataContext.SaveChangesAsync();
            }
        }

        #endregion
    }
}