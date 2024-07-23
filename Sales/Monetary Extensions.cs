#pragma warning disable SA1649 // File name must match first type name

using System;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Extension methods for currency manipulations.
    /// </summary>
    public static class MonetaryExtensions
    {
        /// <summary>
        /// Treating the supplied <paramref name="amount"/> as a monetary unit, convert fractional pennies to
        /// a whole value.
        /// </summary>
        /// <param name="amount">The monetary value to convert.</param>
        /// <returns>The converted value.</returns>
        public static Decimal RoundFractionalPennies(this Decimal amount)
        {
            // convert partial pennies to whole
            amount = amount * 100;
            amount = Math.Ceiling(amount);
            amount = amount / 100M;

            return amount;
        }
    }
}