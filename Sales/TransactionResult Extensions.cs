#pragma warning disable SA1649 // File name must match first type name

using System;
using System.Linq;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Query extensions for the <see cref="TransactionResult"/> type.
    /// </summary>
    public static class TransactionResultExtensions
    {
        internal static readonly TransactionResult[] CapturedStatus =
        {
            TransactionResult.Approved,
            TransactionResult.Voided
        };

        /// <summary>
        /// Indicates whether the indicated <paramref name="status"/> is one that posted a captured amount.
        /// </summary>
        /// <param name="status">The <see cref="TransactionResult"/> to determine.</param>
        /// <returns>True if the status is a captured amount; Otherwise false.</returns>
        public static Boolean IsCaptured(this TransactionResult status)
        {
            return CapturedStatus.Contains(status);
        }
    }
}
