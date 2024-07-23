using System;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// One of the <seealso cref="Product"/> usage scenarios.
    /// </summary>
    /// <remarks>
    /// In general the Public and Admin values should never be used. Filtering to exclude Legacy is common.
    /// </remarks>
    [Serializable()]
    public enum ProductUsage
    {
        /// <summary>
        /// Displayed on the public product sites.
        /// </summary>
        Public = 1,

        /// <summary>
        /// Displayed on the admin site.
        /// </summary>
        Admin = 2,

        /// <summary>
        /// Retired products.
        /// </summary>
        Legacy = 99
    }
}