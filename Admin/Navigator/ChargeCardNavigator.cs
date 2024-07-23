using System;
using System.Web.Mvc;
using AccurateAppend.Websites.Admin.Areas.Sales.SubmitCharge;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the <see cref="SubmitChargeController"/>.
    /// </summary>
    public static class ChargeCardNavigator
    {
        /// <summary>
        /// Build the appropriate URL to the <see cref="SubmitChargeController"/> action.
        /// </summary>
        public static String ViewOrder(this UrlBuilder<SubmitChargeController> builder, Int32 orderId)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "SubmitCharge", new { Area = "Sales", orderId });
        }
    }
}