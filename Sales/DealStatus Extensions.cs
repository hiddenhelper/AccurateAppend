#pragma warning disable SA1649 // File name must match first type name

using System;
using System.Linq;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Extension methods for the <see cref="DealStatus"/> type.
    /// </summary>
    public static class DealStatusExtensions
    {
        internal static readonly DealStatus[] EditableStatus = {DealStatus.InProcess};

        /// <summary>
        /// Indicates whether the current status is in an editable state.
        /// </summary>
        /// <remarks>
        /// Current logic dictates that InProcess status is editable.
        /// </remarks>
        /// <returns>True if the the deal is editable, otherwise false.</returns>
        public static Boolean CanBeEdited(this DealStatus status)
        {
            return EditableStatus.Contains(status);
        }

        internal static readonly DealStatus[] AprrovalStatus = { DealStatus.Approval };

        /// <summary>
        /// Indicates whether the current status is in an approval review state.
        /// </summary>
        /// <remarks>
        /// Current logic dictates that Approval status is editable.
        /// </remarks>
        /// <returns>True if the the deal is editable, otherwise false.</returns>
        public static Boolean CanBeReviewed(this DealStatus status)
        {
            return AprrovalStatus.Contains(status);
        }
    }
}
