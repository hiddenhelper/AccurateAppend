using System;
using System.ComponentModel;
using AccurateAppend.Sales;

namespace AccurateAppend.Websites.Admin.Areas.Sales.CreateBill.Models
{
    /// <summary>
    /// Indicates the possible payment forms accepted in Accurate Append applications. 
    /// </summary>
    [Serializable()]
    public enum BillType
    {
        /// <summary>
        /// The bill should be invoiced and any associated <see cref="DealBinder"/> and <see cref="BillableOrder"/> should be considered completed once processed.
        /// </summary>
        [Description("Invoice")]
        Invoice = 0,

        /// <summary>
        /// The bill should be charged to a credit card and any associated <see cref="DealBinder"/> and <see cref="BillableOrder"/> should be considered completed once payment gateway is processed.
        /// </summary>
        [Description("Receipt")]
        Receipt = 1
    }
}