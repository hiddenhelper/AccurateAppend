using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AccurateAppend.Websites.Admin.Areas.Sales.EditOrder;
using AccurateAppend.Websites.Admin.Areas.Sales.OrderDetail;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the orders.
    /// </summary>
    public static class OrderNavigator
    {
        #region Detail

        /// <summary>
        /// Builds a Url to the <see cref="OrderDetailController.Index"/> action.
        /// </summary>
        public static String Detail(this UrlBuilder<OrderDetailController> navigator, Int32 orderId)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Index", "OrderDetail", new { Area = "Sales", orderId });
        }

        /// <summary>
        /// Navigates to the <see cref="OrderDetailController.Index"/> action.
        /// </summary>
        public static ActionResult Detail(this ActionNavigator<OrderDetailController> navigator, Int32 orderId)
        {
            var action = navigator.RedirectToAction("Index", "OrderDetail", new { Area = "Sales", orderId });
            return action;
        }

        #endregion

        #region Edit

        /// <summary>
        /// Builds a Url to the <see cref="EditOrderController"/> action.
        /// </summary>
        public static String Edit(this UrlBuilder<EditOrderController> navigator, Int32 orderId)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Index", "EditOrder", new { Area = "Sales", orderId });
        }

        /// <summary>
        /// Navigates to the <see cref="EditOrderController"/> action.
        /// </summary>
        public static ActionResult Edit(this ActionNavigator<EditOrderController> navigator, Int32 orderId)
        {
            var action = navigator.RedirectToAction("Index", "EditOrder", new { Area = "Sales", OrderId = orderId });
            return action;
        }

        /// <summary>
        /// Navigates to the <see cref="EditOrderController"/> action.
        /// </summary>
        public static MvcHtmlString Edit(this ViewNavigator<EditOrderController> navigator, Int32 orderId, String linkText)
        {
            return navigator.Edit(orderId, linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="EditOrderController"/> action.
        /// </summary>
        public static MvcHtmlString Edit(this ViewNavigator<EditOrderController> navigator, Int32 orderId, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "EditOrder", new { Area = "Sales", OrderId = orderId }, htmlAttributes);
        }

        #endregion
    }
}