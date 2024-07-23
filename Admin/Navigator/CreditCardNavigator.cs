using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AccurateAppend.Websites.Admin.Areas.Billing.EditCreditCard;
using AccurateAppend.Websites.Admin.Areas.Billing.ViewCreditCards;
using AccurateAppend.Websites.Admin.Areas.Billing.ChangePrimaryCard;
using AccurateAppend.Websites.Admin.Areas.Billing.CreateCreditCard;
using AccurateAppend.Websites.Admin.Areas.Billing.DeleteCreditCard;
using NHibernate.Mapping;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for credit card controllers.
    /// </summary>
    public static class CreditCardNavigator
    {
        #region Detail

        /// <summary>
        /// Navigates to the <see cref="Index"/> action.
        /// </summary>
        public static ActionResult Detail(this ActionNavigator<ViewCreditCardsController> navigator, Guid userId)
        {
            var action = navigator.RedirectToAction("Index", "ViewCreditCards", new { Area = "Billing", UserId = userId });
            return action;
        }

        /// <summary>
        /// Navigates to the <see cref="ViewCreditCardsController.Index"/> action.
        /// </summary>
        public static MvcHtmlString Detail(this ViewNavigator<ViewCreditCardsController> navigator, String linkText, Guid userId)
        {
            return navigator.Detail(linkText, userId, null);
        }

        /// <summary>
        /// Navigates to the <see cref="ViewCreditCardsController.Index"/> action.
        /// </summary>
        public static MvcHtmlString Detail(this ViewNavigator<ViewCreditCardsController> navigator, String linkText, Guid userId, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "ViewCreditCards", new { Area = "Billing", UserId = userId }, htmlAttributes);
        }

        #endregion

        #region Create

        /// <summary>
        /// Navigates to the <see cref="Index"/> action.
        /// </summary>
        public static MvcHtmlString Create(this ViewNavigator<CreateCreditCardController> navigator, String linkText, Guid userId)
        {
            return navigator.Create(linkText, userId, null);
        }

        /// <summary>
        /// Navigates to the <see cref="CreateCreditCardController.Index(Guid, System.Threading.CancellationToken)"/> action.
        /// </summary>
        public static MvcHtmlString Create(this ViewNavigator<CreateCreditCardController> navigator, String linkText, Guid userId, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "CreateCreditCard", new { Area = "Billing", UserId = userId }, htmlAttributes);
        }

        #endregion

        #region Make Primary

        /// <summary>
        /// Navigates to the <see cref="Index"/> action.
        /// </summary>
        public static MvcHtmlString MakePrimary(this ViewNavigator<ChangePrimaryCardController> navigator, String linkText, Int32 cardId)
        {
            return navigator.MakePrimary(linkText, cardId, null);
        }

        /// <summary>
        /// Navigates to the <see cref="ChangePrimaryCardController.Index"/> action.
        /// </summary>
        public static MvcHtmlString MakePrimary(this ViewNavigator<ChangePrimaryCardController> navigator, String linkText, Int32 cardId, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "ChangePrimaryCard", new { Area = "Billing", CardId = cardId }, htmlAttributes);
        }

        #endregion

        #region Update Billing Address

        /// <summary>
        /// Navigates to the <see cref="EditCreditCardController.ChangeBillingAddress"/> action.
        /// </summary>
        public static MvcHtmlString EditBillingAddress(this ViewNavigator<EditCreditCardController> navigator, String linkText, Int32 cardId)
        {
            return navigator.EditBillingAddress(linkText, cardId, null);
        }

        /// <summary>
        /// Navigates to the <see cref="EditCreditCardController.ChangeBillingAddress"/> action.
        /// </summary>
        public static MvcHtmlString EditBillingAddress(this ViewNavigator<EditCreditCardController> navigator, String linkText, Int32 cardId, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "ChangeBillingAddress", "EditCreditCard", new { Area = "Billing", CardId = cardId }, htmlAttributes);
        }

        #endregion

        #region Remove

        /// <summary>
        /// Navigates to the <see cref="Index"/> action.
        /// </summary>
        public static ActionResult Remove(this ActionNavigator<ViewCreditCardsController> navigator, Int32 cardId)
        {
            var action = navigator.RedirectToAction("Index", "DeleteCreditCardController", new { Area = "Billing", UserId = cardId });
            return action;
        }

        /// <summary>
        /// Navigates to the <see cref="Index"/> action.
        /// </summary>
        public static MvcHtmlString Remove(this ViewNavigator<DeleteCreditCardController> navigator, String linkText, Int32 cardId)
        {
            return navigator.Remove(linkText, cardId, null);
        }

        /// <summary>
        /// Navigates to the <see cref="DeleteCreditCardController.Index"/> action.
        /// </summary>
        public static MvcHtmlString Remove(this ViewNavigator<DeleteCreditCardController> navigator, String linkText, Int32 cardId, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "DeleteCreditCard", new { Area = "Billing", CardId = cardId }, htmlAttributes);
        }

        #endregion
    }
}