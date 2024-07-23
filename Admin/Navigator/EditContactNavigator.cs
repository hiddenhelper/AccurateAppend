using System;
using System.Web.Mvc;
using AccurateAppend.Websites.Admin.Areas.Clients.EditContact;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the <see cref="EditContactController"/>.
    /// </summary>
    public static class EditContactNavigator
    {
        #region ContactRow

        /// <summary>
        /// Builds a Url to the <see cref="EditContactController.ContactRow"/> action.
        /// </summary>
        public static String AddRow(this UrlBuilder<EditContactController> navigator)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("ContactRow", "EditContact", new { Area = "Clients" });
        }

        #endregion
    }
}