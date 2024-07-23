using System;
using System.Web.Mvc;
using AccurateAppend.Websites.Admin.Areas.Sales.AddNoteToDeal;
using AccurateAppend.Websites.Admin.Areas.Sales.DealNotes;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the <see cref="DealNotesController"/>.
    /// </summary>
    public static class NotesNavigator
    {
        #region GetNotesForDeal
        
        /// <summary>
        /// Builds a Url to the <see cref="DealNotesController.Index"/> action.
        /// </summary>
        public static String GetNotesForDeal(this UrlBuilder<DealNotesController> navigator, Int32 dealId)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Index", "DealNotes", new { Area = "Sales", DealId = dealId });
        }

        #endregion

        #region AddNoteToDeal

        /// <summary>
        /// Builds a Url to the <see cref="AddNoteToDealController.Index"/> action.
        /// </summary>
        public static String AddNoteToDeal(this UrlBuilder<AddNoteToDealController> navigator)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Index", "AddNoteToDeal", new { Area = "Sales" });
        }

        #endregion
    }
}