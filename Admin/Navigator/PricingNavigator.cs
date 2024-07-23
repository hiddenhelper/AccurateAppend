using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AccurateAppend.Websites.Admin.Areas.Sales.Pricing;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the <see cref="PricingController"/>.
    /// </summary>
    public static class PricingNavigator
    {
        #region Index

        /// <summary>
        /// Build the appropriate URL to the <see cref="PricingController.Index"/> action.
        /// </summary>
        public static String ToIndex(this UrlBuilder<PricingController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "Pricing", new { Area = "Sales" });
        }

        /// <summary>
        /// Navigates to the <see cref="PricingController.Index"/> action.
        /// </summary>
        public static ActionResult ToIndex(this ActionNavigator<PricingController> navigator)
        {
            var action = navigator.RedirectToAction("Index", "Pricing", new {Area = "Sales" });
            return action;
        }

        /// <summary>
        /// Navigates to the <see cref="PricingController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<PricingController> navigator, String linkText)
        {
            return navigator.ToIndex(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="PricingController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<PricingController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "Pricing", new { Area = "Sales" }, htmlAttributes);
        }
        #endregion

        #region Edit Cost

        /// <summary>
        /// Build the appropriate URL to the <see cref="PricingController"/> action.
        /// </summary>
        public static String ToEdit(this UrlBuilder<PricingController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("EditCost", "Pricing", new { Area = "Sales" });
        }

        /// <summary>
        /// Build the appropriate URL to the <see cref="PricingController"/> action.
        /// </summary>
        public static String ToCopyExisting(this UrlBuilder<PricingController> builder, String product, Guid userId)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("EditCost", "Pricing", new { Area = "Sales", product, category = userId, CopyExisting = true });
        }

        /// <summary>
        /// Navigates to the <see cref="PricingController.Index"/> action.
        /// </summary>
        public static ActionResult ToEdit(this ActionNavigator<PricingController> navigator)
        {
            var action = navigator.RedirectToAction("EditCost", "Pricing", new { Area = "Sales" });
            return action;
        }

        /// <summary>
        /// Navigates to the <see cref="PricingController"/> action.
        /// </summary>
        public static MvcHtmlString ToEdit(this ViewNavigator<PricingController> navigator, String linkText)
        {
            return navigator.ToIndex(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="PricingController"/> action.
        /// </summary>
        public static MvcHtmlString ToEdit(this ViewNavigator<PricingController> navigator, String linkText, string category, string product, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "EditCost", "Pricing", new { Area = "Sales", category, product }, htmlAttributes);
        }

        #endregion
        
    }
}