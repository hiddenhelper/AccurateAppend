using System;
using System.ComponentModel;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Indicates the possible payment processes accepted in Accurate Append applications. 
    /// </summary>
    /// <remarks>
    /// This type is really a discriminator column value (TPH style mapping) that is explicitly exposed
    /// on the <see cref="BillingContract"/> type. This is done as Entity Splitting w/ TPH is not a
    /// mapping system supported by EF. When the DB is refactored to support bill content extraction,
    /// we can roll this into the mapping configuration.
    /// </remarks>
    [Serializable()]
    public enum ContractType
    {
        /// <summary>
        /// The bill should be invoiced and any associated <see cref="DealBinder"/> and <see cref="Order"/> should be considered completed once Approved.
        /// </summary>
        [Description("Invoice")]
        Invoice = 0,

        /// <summary>
        /// The bill should be charged to a credit card and any associated <see cref="DealBinder"/> and <see cref="Order"/> should be considered completed once full payment is processed.
        /// </summary>
        [Description("Receipt")]
        Receipt = 1
    }
}