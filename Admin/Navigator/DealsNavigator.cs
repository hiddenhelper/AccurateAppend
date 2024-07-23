using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.LinkJobToDeal;
using AccurateAppend.Websites.Admin.Areas.Sales.CancelDeal;
using AccurateAppend.Websites.Admin.Areas.Sales.CreateRefund;
using AccurateAppend.Websites.Admin.Areas.Sales.DealDetail;
using AccurateAppend.Websites.Admin.Areas.Sales.DealSummary;
using AccurateAppend.Websites.Admin.Areas.Sales.DownloadDeals;
using AccurateAppend.Websites.Admin.Areas.Sales.EditDeal;
using AccurateAppend.Websites.Admin.Areas.Sales.ExpireDeal;
using AccurateAppend.Websites.Admin.Areas.Sales.NewDeal;
using AccurateAppend.Websites.Admin.Areas.Sales.NewDealFromJob;
using AccurateAppend.Websites.Admin.Areas.Sales.RefundDeal;
using AccurateAppend.Websites.Admin.Areas.Sales.ReviewDeal;
using NHibernate.Mapping;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the various deal related controllers (<see cref="EditDealController"/>, 
    /// <see cref="DealDetailController"/>, and <see cref="LinkJobToDealController"/>).
    /// </summary>
    public static class DealsNavigator
    {
        #region NewDeal.Create

        /// <summary>
        /// Build the appropriate URL to the <see cref="NewDealController"/> action.
        /// </summary>
        public static String ToCreate(this UrlBuilder<NewDealController> builder, Guid userId)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Create", "NewDeal", new { Area = "Sales", UserId = userId });
        }

        /// <summary>
        /// Navigates to the <see cref="NewDealController"/> action.
        /// </summary>
        public static MvcHtmlString Create(this ViewNavigator<NewDealController> navigator, String linkText, Guid userId)
        {
            return navigator.Create(linkText, userId, null);
        }

        /// <summary>
        /// Navigates to the <see cref="NewDealController"/> action.
        /// </summary>
        public static MvcHtmlString Create(this ViewNavigator<NewDealController> navigator, String linkText, Guid userId, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Create", "NewDeal", new { Area = "Sales", UserId = userId }, htmlAttributes);
        }

        #endregion

        #region EditDeal.AssociateWithJob

        /// <summary>
        /// Navigates to the <see cref="LinkJobToDealController.Display"/> action.
        /// </summary>
        public static MvcHtmlString AssociateWithJob(this ViewNavigator<LinkJobToDealController> navigator, String linkText, Int32 jobId)
        {
            return navigator.AssociateWithJob(linkText, jobId, null);
        }

        /// <summary>
        /// Navigates to the <see cref="LinkJobToDealController.Display"/> action.
        /// </summary>
        public static MvcHtmlString AssociateWithJob(this ViewNavigator<LinkJobToDealController> navigator, String linkText, Int32 jobId, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Display", "LinkJobToDeal", new { Area = "JobProcessing", jobId }, htmlAttributes);
        }

        /// <summary>
        /// Navigates to the <see cref="LinkJobToDealController.Display"/> action.
        /// </summary>
        public static String AssociateWithJob(this UrlBuilder<LinkJobToDealController> builder, Int32 jobId)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Display", "LinkJobToDeal", new { Area = "JobProcessing", jobId });
        }

        #endregion

        #region NewDeal.FromJob

        /// <summary>
        /// Navigates to the <see cref="NewDealFromJobController.Index"/> action.
        /// </summary>
        public static MvcHtmlString FromJob(this ViewNavigator<NewDealFromJobController> navigator, String linkText, Int32 jobId)
        {
            return navigator.FromJob(linkText, jobId, null);
        }

        /// <summary>
        /// Navigates to the <see cref="NewDealFromJobController.Index"/> action.
        /// </summary>
        public static MvcHtmlString FromJob(this ViewNavigator<NewDealFromJobController> navigator, String linkText, Int32 jobId, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "NewDealFromJob", new { Area = "Sales", JobId = jobId }, htmlAttributes);
        }

        /// <summary>
        /// Navigates to the <see cref="NewDealFromJobController.Index"/> action.
        /// </summary>
        public static String FromJob(this UrlBuilder<NewDealFromJobController> builder, Int32 jobId)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "NewDealFromJob", new { Area = "Sales", jobId });
        }

        #endregion

        #region DealDetail.Index

        /// <summary>
        /// Builds a Url to the <see cref="DealDetailController.Index"/> action for the indicated deal.
        /// </summary>
        public static String Detail(this UrlBuilder<DealDetailController> navigator, Int32 dealId, String scheme = null)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Index", "DealDetail", new { Area = "Sales", DealId = dealId }, scheme);
        }

        /// <summary>
        /// Navigates to the <see cref="DealDetailController.Index"/> action.
        /// </summary>
        public static ActionResult Detail(this ActionNavigator<DealDetailController> navigator, Int32 dealid)
        {
            return navigator.RedirectToAction("Index", "DealDetail", new { Area = "Sales", DealId = dealid});
        }

        /// <summary>
        /// Navigates to the <see cref="DealDetailController.Index"/> action.
        /// </summary>
        public static MvcHtmlString Detail(this ViewNavigator<DealDetailController> navigator, String linkText, Int32 dealid)
        {
            return navigator.Detail(linkText, dealid, null);
        }

        /// <summary>
        /// Navigates to the <see cref="DealDetailController.Index"/> action.
        /// </summary>
        public static MvcHtmlString Detail(this ViewNavigator<DealDetailController> navigator, String linkText, Int32 dealId, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "DealDetail", new { Area = "Sales", DealId = dealId }, htmlAttributes);
        }

        #endregion

        #region EditDeal.Delete

        /// <summary>
        /// Navigates to the <see cref="CancelDealController.Index"/> action.
        /// </summary>
        public static ActionResult Delete(this ActionNavigator<CancelDealController> navigator, Int32 dealid)
        {
            return navigator.RedirectToAction("Index", "CancelDeal", new { Area = "Sales", DealId = dealid });
        }

        #endregion

        #region EditDeal.Index

        /// <summary>
        /// Builds a Url to the <see cref="EditDealController"/> action for the indicated deal.
        /// </summary>
        public static String Edit(this UrlBuilder<EditDealController> navigator, Int32 dealId, String scheme = null)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Index", "EditDeal", new { Area = "Sales", DealId = dealId }, scheme);
        }

        /// <summary>
        /// Navigates to the <see cref="Index"/> action.
        /// </summary>
        public static ActionResult Edit(this ActionNavigator<EditDealController> navigator, Int32 dealId)
        {
            return navigator.RedirectToAction("Index", "EditDeal", new { Area = "Sales", dealId });
        }

        /// <summary>
        /// Navigates to the <see cref="Index"/> action.
        /// </summary>
        public static MvcHtmlString Edit(this ViewNavigator<EditDealController> navigator, String linkText, Int32 dealid)
        {
            return navigator.Edit(linkText, dealid, null);
        }

        /// <summary>
        /// Navigates to the <see cref="Index"/> action.
        /// </summary>
        public static MvcHtmlString Edit(this ViewNavigator<EditDealController> navigator, String linkText, Int32 dealid, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "EditDeal", new { Area = "Sales", dealid }, htmlAttributes);
        }

        #endregion

        #region EditDeal.Expire
        
        /// <summary>
        /// Build the appropriate URL to the <see cref="ExpireDealController.Index"/> action.
        /// </summary>
        public static String Expire(this UrlBuilder<ExpireDealController> builder, Int32 dealId)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "ExpireDeal", new { Area = "Sales", DealId = dealId });
        }

        #endregion

        #region Deals.Index

        /// <summary>
        /// Navigates to the <see cref="DealSummaryController.Index"/> action.
        /// </summary>
        public static ActionResult ToIndex(this ActionNavigator<DealSummaryController> navigator)
        {
            var action = navigator.RedirectToAction("Index", "DealSummary", new {Area = "Sales"});
            return action;
        }

        /// <summary>
        /// Build the appropriate URL to the <see cref="DealSummaryController.Index"/> action.
        /// </summary>
        public static String ToIndex(this UrlBuilder<DealSummaryController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "DealSummary", new { Area = "Sales" });
        }

        /// <summary>
        /// Navigates to the <see cref="DealSummaryController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<DealSummaryController> navigator, String linkText, Guid userId)
        {
            return navigator.ToIndex(linkText, userId, null);
        }

        /// <summary>
        /// Navigates to the <see cref="DealSummaryController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<DealSummaryController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "DealSummary", new { Area = "Sales" }, htmlAttributes);
        }

        /// <summary>
        /// Navigates to the <see cref="DealSummaryController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<DealSummaryController> navigator, String linkText, Guid userId, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "DealSummary", new { Area = "Sales", UserId = userId }, htmlAttributes);
        }

        /// <summary>
        /// Navigates to the <see cref="DealSummaryController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<DealSummaryController> navigator, String linkText, String email, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "DealSummary", new { Area = "Sales", email }, htmlAttributes);
        }

        #endregion

        #region Deal.Review

        /// <summary>
        /// Build the appropriate URL to the <see cref="ReviewDealController"/> action.
        /// </summary>
        public static String Review(this UrlBuilder<ReviewDealController> builder, Int32 dealId)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "ReviewDeal", new { Area = "Sales", DealId = dealId });
        }

        #endregion

        #region CreateRefundController.Index

        /// <summary>
        /// Navigates to the <see cref="CreateRefundController.Index"/> action.
        /// </summary>
        public static ActionResult Refund(this ActionNavigator<CreateRefundController> navigator, Int32 dealid)
        {
            return navigator.RedirectToAction("Index", "CreateRefund", new { Area = "Sales", dealid });
        }

        /// <summary>
        /// Navigates to the <see cref="CreateRefundController.Index"/> action.
        /// </summary>
        public static MvcHtmlString Refund(this ViewNavigator<CreateRefundController> navigator, String linkText, Int32 dealId)
        {
            return navigator.Refund(linkText, dealId, null);
        }

        /// <summary>
        /// Navigates to the <see cref="CreateRefundController.Index"/> action.
        /// </summary>
        public static MvcHtmlString Refund(this ViewNavigator<CreateRefundController> navigator, String linkText, Int32 dealId, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "CreateRefund", new { Area = "Sales", dealId }, htmlAttributes);
        }

        /// <summary>
        /// Build the appropriate URL to the <see cref="CreateRefundController.Index"/> action.
        /// </summary>
        public static String Refund(this UrlBuilder<CreateRefundController> builder, Int32 dealId)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "CreateRefund", new { Area = "Sales",  dealId });
        }

        #endregion

        #region RefundDeal.Draft

        /// <summary>
        /// Navigates to the <see cref="RefundDealController.Draft"/> action.
        /// </summary>
        public static ActionResult Edit(this ActionNavigator<RefundDealController> navigator, Int32 orderId)
        {
            return navigator.RedirectToAction("Index", "RefundDeal", new { Area = "Sales", dealid = orderId });
        }

        /// <summary>
        /// Navigates to the <see cref="RefundDealController.Draft"/> action.
        /// </summary>
        public static MvcHtmlString Edit(this ViewNavigator<RefundDealController> navigator, String linkText, Int32 orderId)
        {
            return navigator.Edit(linkText, orderId, null);
        }

        /// <summary>
        /// Navigates to the <see cref="RefundDealController.Draft"/> action.
        /// </summary>
        public static MvcHtmlString Edit(this ViewNavigator<RefundDealController> navigator, String linkText, Int32 orderId, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "RefundDeal", new { Area = "Sales", orderId }, htmlAttributes);
        }

        /// <summary>
        /// Build the appropriate URL to the <see cref="RefundDealController.Draft"/> action.
        /// </summary>
        public static String Edit(this UrlBuilder<RefundDealController> builder, Int32 orderId)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "RefundDeal", new { Area = "Sales", orderId });
        }

        #endregion

        #region DownloadDeals.Index

        /// <summary>
        /// Builds a Url to the <see cref="DownloadDealsController.Index"/> action without input parameters.
        /// </summary>
        public static String IndexRoot(this UrlBuilder<DownloadDealsController> navigator)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Index", "DownloadDeals", new { Area = "Sales" });
        }

        #endregion
    }
}