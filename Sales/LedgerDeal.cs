using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using AccurateAppend.Core;

namespace AccurateAppend.Sales
{
    public class LedgerDeal : DealBinder
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LedgerDeal"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected LedgerDeal()
        {
        }

        protected LedgerDeal(ClientRef owner, Guid creator) : base(owner, creator)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LedgerDeal"/> class specifically for a <see cref="RecurringBillingAccount"/> ledger entry.
        /// </summary>`
        /// <exception cref="InvalidOperationException">The <see cref="ClientRef"/> does not have a current active <see cref="RecurringBillingAccount"/>.</exception>
        public LedgerDeal(RecurringBillingAccount account, BillingPeriod period, Guid creator) : base(account?.ForClient, creator)
        {
            if (period == null) throw new ArgumentNullException(nameof(period));
            Contract.EndContractBlock();

            Debug.Assert(account != null);

            var outstanding = period.ToOutstandingRange();
            outstanding = outstanding.ToBillingZone();
            if (!account.IsValidForPeriod(outstanding)) throw new InvalidOperationException($"The client {account.ForClient.UserName} does not have a subscription covering dates '{outstanding}'");

            // Constructor adds to collection
            var order = new BillableOrder(this);
            Debug.Assert(base.Orders.Contains(order));
            Trace.WriteLine($"Added order: {order.PublicKey}");
        }

        #endregion
    }
}
