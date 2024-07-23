using System;
using System.Linq;

namespace AccurateAppend.Websites.Admin.Areas.Sales.EditOrder.Models
{
    /// <summary>
    /// View model for displaying the details for a single order.
    /// </summary>
    [Serializable()]
    public class EditViewModel : AccurateAppend.Sales.Contracts.ViewModels.OrderDetail
    {
        /// <summary>
        /// Indicates whether the current order has a relationship to a job and can be refreshed from it.
        /// </summary>
        public Boolean CanUpdateFromJob { get; set; }

        /// <summary>
        /// Calculates the current total of the order.
        /// </summary>
        public virtual Decimal Total()
        {
            var subTotal = this.Items.Sum(i => i.Total());
            if (this.OrderMinimum == 0m) return subTotal;

            if (this.OrderMinimum < 0m)
            {
                return Math.Max(subTotal + this.OrderMinimum, 0m);
            }

            return Math.Max(subTotal, this.OrderMinimum);
        }
    }
}