using System;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Handlers;

namespace AccurateAppend.Websites.Admin.Messages.Accounts
{
    /// <summary>
    /// Command to instruct a <see cref="RecurringBillingAccount"/> entity to create a bill for the indicated <see cref="BillingPeriod"/>.
    /// </summary>
    /// <remarks>
    /// This is an internal implementation feature of the <see cref="UsageBillingSaga"/> therefore this command is implemented here.
    /// The saga is aware of the scheduling logic that should be performed for billing but delegates the actual generating of bills to
    /// this command.
    /// </remarks>
    [Serializable()]
    public class CreateUsageBillCommand : CreateBillCommandBase
    {
    }
}