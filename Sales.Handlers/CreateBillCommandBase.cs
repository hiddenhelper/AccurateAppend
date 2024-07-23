using System;
using AccurateAppend.Sales;
using NServiceBus;

namespace AccurateAppend.Sales.Handlers
{
    /// <summary>
    /// Command to instruct a <see cref="RecurringBillingAccount"/> entity to create a bill for the indicated <see cref="BillingPeriod"/>.
    /// </summary>
    /// <remarks>
    /// This is an internal implementation feature of the various billing sagas therefore this command is implemented here.
    /// The saga is aware of the scheduling logic that should be performed for billing but delegates the actual generating of bills to
    /// handlers of this command.
    /// </remarks>
    public class CreateBillCommandBase : ICommand
    {
        /// <summary>
        /// The identifier of the <see cref="RecurringBillingAccount"/> instance to create billing for.
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// The <see cref="BillingPeriod"/> that billing should be generated for.
        /// </summary>
        public BillingPeriod Period { get; set; }
    }
}