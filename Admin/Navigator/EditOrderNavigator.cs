using System;
using System.Web.Mvc;
using AccurateAppend.Websites.Admin.Areas.Sales.EditOrder;
using AccurateAppend.Websites.Admin.Areas.Sales.UpdateOrderFromJob;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the <see cref="EditOrderController"/>.
    /// </summary>
    public static class EditOrderNavigator
    {
        #region OrderItemRow

        /// <summary>
        /// Builds a Url to the <see cref="EditOrderController.OrderItemRow"/> action.
        /// </summary>
        public static String AddRow(this UrlBuilder<EditOrderController> navigator, Guid userId)
        {
            var url = ((IAdapter<UrlHelper>) navigator).Item;
            return url.Action("OrderItemRow", "EditOrder", new {Area = "Sales", UserId = userId});
        }

        #endregion

        #region RefreshFromJob

        /// <summary>
        /// Builds a Url to the <see cref="UpdateOrderFromJobController.Index"/> action.
        /// </summary>
        public static String RefreshFromJob(this UrlBuilder<UpdateOrderFromJobController> navigator, Int32 orderId)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Index", "UpdateOrderFromJob", new { Area = "Sales", orderId });
        }

        #endregion
    }
}