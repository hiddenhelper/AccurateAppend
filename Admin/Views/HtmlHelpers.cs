using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.Websites.Admin.Views
{
    /// <summary>
    /// Extension methods for the <see cref="HtmlHelper"/> class and other related UI rendering.
    /// </summary>
    public static class HtmlHelpers
    {
        #region FormatHtml

        /// <summary>
        /// Returns the supplied <paramref name="value"/> formatted for HTML display.
        /// </summary>
        /// <param name="value">The string value to format.</param>
        /// <returns>The HTML formatted string.</returns>
        public static String FormatHtml(this String value)
        {
            return (value ?? String.Empty).Replace("<", "&lt;").Replace(">", "&gt;").Replace("\r\n", "<br/>");
        }

        #endregion
        
        #region StandardDateDisplay

        /// <summary>
        /// Creates the standardized HTML used for displayng dates in the UI.
        /// </summary>
        /// <param name="helper">The current <see cref="HtmlHelper"/> instance.</param>
        /// <param name="date">The <see cref="DateTime"/> to display.</param>
        /// <returns></returns>
        public static string StandardDateDisplay(this HtmlHelper helper, DateTime date)
        {
            var builder = new TagBuilder("label");
            builder.Attributes.Add("title", String.Format(date.ToStandardTooltip()));
            builder.SetInnerText(date.ToStandardDisplay());
            var html = builder.ToString(TagRenderMode.Normal);

            return html;
        }

        #endregion
               
        #region Product GetComplete

        public static string ProductDropDown(this HtmlHelper html, String value, String formName, String formId)
        {
            var products = ProductCache.Cache;

            var sb = new StringBuilder();
            sb.AppendLine("<select id='" + formId + "' name='" + formName + "' class='form-control'>");
            sb.AppendLine("<option value=\"\">- select product --------------</option>");

            sb.AppendLine("<option value=\"\">");
            sb.AppendLine("<option value=\"\">" + "-- OPERATIONS ".PadRight(25, '-') + "</option>");
            foreach (var product in products.Where(a => a.Category == "Operations").OrderBy(p => p.Key))
            {
                sb.FormatProduct(value, product);
            }

            sb.AppendLine("<option value=\"\">");
            sb.AppendLine("<option value=\"\">" + "-- PHONE APPEND".PadRight(25, '-') + "</option>");
            foreach (var product in products.Where(a => a.Category == "PhoneAppend").OrderBy(p => p.Key))
            {
                sb.FormatProduct(value, product);
            }

            sb.AppendLine("<option value=\"\">");
            sb.AppendLine("<option value=\"\">" + "-- EMAIL APPEND".PadRight(25, '-') + "</option>");
            foreach (var product in products.Where(a => a.Category == "EmailAppend").OrderBy(p => p.Key))
            {
                sb.FormatProduct(value, product);
            }

            sb.AppendLine("<option value=\"\">");
            sb.AppendLine("<option value=\"\">" + "-- COMPOSITE APPEND".PadRight(25, '-') + "</option>");
            foreach (var product in products.Where(a => a.Category == "Composite").OrderBy(p => p.Key))
            {
                sb.FormatProduct(value, product);
            }

            sb.AppendLine("<option value=\"\">");
            sb.AppendLine("<option value=\"\">" + "-- MISC".PadRight(25, '-') + "</option>");
            foreach (var product in products.Where(a => a.Category == "Misc").OrderBy(p => p.Key))
            {
                sb.FormatProduct(value, product);
            }

            sb.AppendLine("<option value=\"\">");
            sb.AppendLine("<option value=\"\">" + "-- EVERYTHING ELSE".PadRight(25, '-') + "</option>");
            foreach (var product in products.Where(a => !(a.Category == "PhoneAppend" || a.Category == "EmailAppend" || a.Category == "Composite" || a.Category == "Operations")).OrderBy(p => p.Key))
            {
                sb.FormatProduct(value, product);
            }
            sb.AppendLine("</select>");
            return sb.ToString();
        }

        private static void FormatProduct(this StringBuilder builder, String value, ProductCache.ProductInfo product)
        {
            var option = new TagBuilder("option");
            option.MergeAttribute("value", product.Key);
            option.SetInnerText(product.Title);

            if (value == product.Key) option.MergeAttribute("selected", "selected");

            builder.AppendLine(option.ToString());
        }

        #endregion

        #region Operation

        public static string OperationDropDown(this HtmlHelper html, DataServiceOperation value, String formName, String formId)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<select id='" + formId + "' name='" + formName + "' class='form-control'>");

            foreach (var name in System.Enum.GetNames(typeof(DataServiceOperation)).Select(EnumExtensions.Parse<DataServiceOperation>))
            {
                sb.FormatOperation(value, name);
            }

            sb.AppendLine("</select>");
            return sb.ToString();
        }

        private static void FormatOperation(this StringBuilder builder, DataServiceOperation value, DataServiceOperation operation)
        {
            if (value == operation)
            {
                builder.AppendLine($"<option value=\"{operation}\" selected=\"selected\">{operation} ({operation.GetDescription()})</option>");
            }
            else
            {
                builder.AppendLine($"<option value=\"{operation}\">{operation} ({operation.GetDescription()})</option>");
            }
        }

        #endregion

        #region Match Levels

        public static String MatchLevelDropDown(this HtmlHelper html, MatchLevel value, String formName, String formId)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<select id='" + formId + "' name='" + formName + "' class='form-control'>");
            
            foreach (var name in System.Enum.GetNames(typeof(MatchLevel)).Select(EnumExtensions.Parse<MatchLevel>))
            {
                sb.FormatMatchLevel(value, name);
            }

            sb.AppendLine("</select>");
            return sb.ToString();
        }

        private static void FormatMatchLevel(this StringBuilder builder, MatchLevel value, MatchLevel matchLevel)
        {
            if (value == matchLevel)
            {
                builder.AppendLine($"<option value=\"{matchLevel}\" selected=\"selected\">{matchLevel} ({matchLevel.GetCategoryDescription()})</option>");
            }
            else
            {
                builder.AppendLine($"<option value=\"{matchLevel}\">{matchLevel} ({matchLevel.GetCategoryDescription()})</option>");
            }
        }

        #endregion

        #region Quality Levels

        public static String QualityLevelDropDown(this HtmlHelper html, QualityLevel value, String formName, String formId)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<select id='" + formId + "' name='" + formName + "' class='form-control'>");

            foreach (var name in System.Enum.GetNames(typeof(QualityLevel)).Select(EnumExtensions.Parse<QualityLevel>))
            {
                sb.FormatQualityLevel(value, name);
            }

            sb.AppendLine("</select>");
            return sb.ToString();
        }

        private static void FormatQualityLevel(this StringBuilder builder, QualityLevel value, QualityLevel qualityLevel)
        {
            if (value == qualityLevel)
            {
                builder.AppendLine($"<option value=\"{qualityLevel}\" selected=\"selected\">{qualityLevel}</option>");
            }
            else
            {
                builder.AppendLine($"<option value=\"{qualityLevel}\">{qualityLevel}</option>");
            }
        }

        #endregion

        #region SiteDropDown

        public static MvcHtmlString SiteDropDown<T>(this HtmlHelper<T> helper, string value)
        {
            var user = HttpContext.Current.User.Identity as ClaimsIdentity ?? new ClaimsIdentity();
            
            if (value == null)
            {
                var username = user.Name;
                var components = username.Split(new[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
                if (components.Length < 2) throw new ArgumentException($"Username:\'{username}\' does not appear in the \'user@realm.domain\' format", nameof(value));

                switch (components[1].ToLowerInvariant())
                {
                    case "accurateappend.com":
                        value = Security.ApplicationExtensions.AccurateAppendId.ToString();
                        break;
                    case "2020connect.net":
                        value = Security.ApplicationExtensions.TwentyTwentyId.ToString();
                        break;
                    default:
                        value = Security.ApplicationExtensions.AccurateAppendId.ToString();
                        break;
                }
            }

            var items = new List<SelectListItem>();
            items.Add(new SelectListItem {Text = "- select site --------------"});
            items.Add(new SelectListItem {Value = Security.ApplicationExtensions.AccurateAppendId.ToString(), Text = "Accurate Append"});
            items.Add(new SelectListItem {Value = Security.ApplicationExtensions.TwentyTwentyId.ToString(), Text = "2020Connect.net"});

            items.ForEach(i => i.Selected = String.Equals(i.Value, value, StringComparison.OrdinalIgnoreCase));

            var result = helper.DropDownList("ApplicationId", items, new { @class = "form-control", id = "ApplicationId", style = "width: 175px;display: inline;" });
            return result;
        }

        #endregion

        #region AdminUsers

        public static MvcHtmlString AdminUsersDropDown(this HtmlHelper helper, Guid? userId = null, String id = null, String style = null)
        {
            userId = Guid.Empty;
            id = id ?? "AdminUserId";
            style = style ?? String.Empty;

            var items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "- select user - "});

            foreach (var user in AdminUserCache.Cache)
            {
                items.Add(new SelectListItem { Value = user.UserId.ToString(), Text = user.UserName });
            }
            
            items.ForEach(i => i.Selected = String.Equals(i.Value, userId.ToString(), StringComparison.OrdinalIgnoreCase));

            var result = helper.DropDownList(id, items, new { @class = "form-control", id, style });
            return result;
        }

        #endregion

        /// <summary>
        /// Displays anchor button with Font Awesome icon
        /// </summary>
        /// <param name="html"></param>
        /// <param name="linkText"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="fontAwesomCssClass"></param>
        /// <returns></returns>
        public static MvcHtmlString ActionLinkWithFontAwesomIcon(this HtmlHelper html, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes, string fontAwesomCssClass)
        {
            var attributes = new RouteValueDictionary(htmlAttributes);
            var linkTag = new TagBuilder("a");
            var spanTag = new TagBuilder("span");
            spanTag.AddCssClass(fontAwesomCssClass);
            linkTag.InnerHtml = spanTag.ToString(TagRenderMode.Normal) + linkText;
            linkTag.MergeAttributes(attributes);
            var url = new UrlHelper(html.ViewContext.RequestContext);
            linkTag.Attributes.Add("href", url.Action(actionName, controllerName, routeValues));
            return MvcHtmlString.Create(linkTag.ToString(TagRenderMode.Normal));
        }
    }
}