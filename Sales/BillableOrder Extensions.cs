#pragma warning disable SA1649 // File name must match first type name

using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using AccurateAppend.Core.IdentityModel;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Extension methods for the <see cref="BillableOrder"/> class.
    /// </summary>
    public static class BillableOrderExtensions
    {
        /// <summary>
        /// Convenience method to quickly set a refund notice content to an order.
        /// </summary>
        /// <param name="order">The <see cref="RefundOrder"/> to draft a refund notice for.</param>
        /// <param name="communication">The refund content that will be sent to the client once the order is refunded.</param>
        public static void DraftRefund(this RefundOrder order, BillContent communication)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            order.Content = communication;
        }

        /// <summary>
        /// Convenience method to quickly set an invoice content and move to review with the interactive user.
        /// </summary>
        /// <param name="order">The <see cref="BillableOrder"/> to draft an invoice for.</param>
        /// <param name="communication">The invoice content that will be sent to the client once the order completes.</param>
        public static void DraftInvoice(this BillableOrder order, BillContent communication)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            var adminUserId = Thread.CurrentPrincipal.Identity.GetIdentifier();

            order.DraftBill(communication, ContractType.Invoice, adminUserId);
        }

        /// <summary>
        /// Convenience method to quickly set a receipt content and move to review with the interactive user.
        /// </summary>
        /// <param name="order">The <see cref="BillableOrder"/> to draft a receipt for.</param>
        /// <param name="communication">The receipt content that will be sent to the client once the order completes.</param>
        public static void DraftReceipt(this BillableOrder order, BillContent communication)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            var adminUserId = Thread.CurrentPrincipal.Identity.GetIdentifier();

            order.DraftBill(communication, ContractType.Receipt, adminUserId);
        }

        /// <summary>
        /// Convenience method to set billing content and move to review with the specified user.
        /// </summary>
        /// <remarks>
        /// <note type="implementnotes">
        /// This is public for testing purposes only. Hidden from IDE otherwise.
        /// </note>
        /// </remarks>
        /// <param name="order">The <see cref="BillableOrder"/> to draft a bill for.</param>
        /// <param name="communication">The bill content that will be sent to the client once the order completes.</param>
        /// <param name="billingProcess">The <see cref="ContractType"/> that describes the desired billing process to enact.</param>
        /// <param name="userId">The identifier of the user that is performing the action.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void DraftBill(this BillableOrder order, BillContent communication, ContractType billingProcess, Guid userId)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            order.Content = communication;
            order.Bill.ContractType = billingProcess;
            order.Deal.SubmitForReview(new Audit("Send to review", userId));
        }

        /// <summary>
        /// Gets the calculated sub-total for all the provided <paramref name="order "/> items.
        /// </summary>
        /// <remarks>
        /// Unlike the core <see cref="Order.Total"/> logic, this value is unaffected by the presence
        /// of a non zero value in the <see cref="Order.OrderMinimum"/> property. This will always calculate
        /// the total of the <see cref="ProductLine"/> in the order.
        /// </remarks>
        /// <returns>The calculated total for all the provided <paramref name="order"/> items.</returns>
        public static Decimal SubTotal(this BillableOrder order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            return !order.Lines.Any() ? 0m : order.Lines.Sum(l => l.Total());
        }

        /// <summary>
        /// Gets the calculated outstanding total that remains to be processed on the <paramref name="order"/> to complete.
        /// </summary>
        /// <remarks>
        /// Unlike the core <see cref="Order.Total"/> logic, this value is the difference of the sum of the posted
        /// <see cref="Order.Transactions"/> and requested <see cref="Order.PendingTransactions"/>. This value takes
        /// the presence of a non zero value in the <see cref="Order.OrderMinimum"/> property into account. If the
        /// <see cref="Order.Status"/> is not Open, this value is always 0.
        /// </remarks>
        /// <returns>The calculated outstanding total remaining to be handled for the current <see cref="BillableOrder"/> to complete.</returns>
        public static Decimal OutstandingTotal(this BillableOrder order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            Contract.Ensures(Contract.Result<Decimal>() >= 0m);
            Contract.EndContractBlock();

            if (!order.Status.CanBeEdited()) return 0m;

            var remaining = order.Total() - order.Transactions
                                .Where(e => e.Status.IsCaptured())
                                .Select(e => e.AmountProcessed)
                                .Where(v => v != null)
                                .Cast<Decimal>()
                                .DefaultIfEmpty()
                                .Sum();
            remaining = remaining - order.PendingTransactions.Select(t => t.AmountRequested).DefaultIfEmpty().Sum();

            // Handle fractional overages
            return Math.Max(remaining, 0m);
        }
    }
}
