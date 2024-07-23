using System;
using System.Web.Mvc;
using AccurateAppend.Websites.Admin.Areas.Sales.CreateBill;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the <see cref="CreateBillController"/>.
    /// </summary>
    public static class CreateBillNavigator
    {
        /// <summary>
        /// Build the appropriate URL to the <see cref="CreateBillController.CreateBillFromDeal"/> action.
        /// </summary>
        public static String ForDeal(this UrlBuilder<CreateBillController> builder, Int32 dealId)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("CreateBillFromDeal", "CreateBill", new { Area = "Sales", DealId = dealId });
        }
    }
}