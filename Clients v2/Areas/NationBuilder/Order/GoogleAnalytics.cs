using System;
using System.Text;
using System.Web.Mvc;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Websites.Clients.Areas.NationBuilder.Order.Models;
using AccurateAppend.Websites.Clients.Areas.Shared.Models;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.Order
{
    public static class GoogleAnalytics
    {
        public static MvcHtmlString GetAnalyticsTransaction(this NationBuilderOrderModel model, Boolean isTestAccount)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<script>");
            sb.AppendLine("ga('require', 'ecommerce');");
            sb.AppendLine("ga('ecommerce:addTransaction', {");
            sb.AppendLine("'id': '" + model.OrderId + "',");
            sb.AppendLine("'affiliation': 'NationBuilder',");
            sb.AppendLine("'revenue': '" + model.Total + "',");
            sb.AppendLine("'shipping': '0',");
            sb.AppendLine("'tax': '0'");
            sb.AppendLine("});");
            model.Products.ForEach(p => sb.Append(GetAnalyticsItem(p, model.OrderId)));
            if (!isTestAccount) sb.AppendLine("ga('ecommerce:send');");
            sb.AppendLine("ga('ecommerce:send');");
            sb.AppendLine("</script>");
            return MvcHtmlString.Create(sb.ToString());
        }

        private static MvcHtmlString GetAnalyticsItem(ProductModel model, Guid orderid)
        {
            var sb = new StringBuilder();
            sb.AppendLine("ga('ecommerce:addItem', {");
            sb.AppendLine("'id': '" + orderid + "',");
            sb.AppendLine("'name': '" + model.Title + "',");
            sb.AppendLine("'sku': '" + model.ProductKey + "',");
            sb.AppendLine("'category': 'Data Processing',");
            sb.AppendLine("'price': '" + model.Cost + "',");
            sb.AppendLine("'quantity': '" + model.EstMatches + "'");
            sb.AppendLine("});");
            return MvcHtmlString.Create(sb.ToString());
        }
    }
}