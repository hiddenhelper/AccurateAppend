using System.Linq;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.HtmlHelpers
{
    /// <summary>
    /// HTML Helpers related SEO content.
    /// </summary>
    public static class SeoHelper
    {
        /// <summary>
        /// Gets the descriptive page title element for the current path.
        /// </summary>
        public static MvcHtmlString GetTitle(this HtmlHelper html)
        {
            var tag = new TagBuilder("title");

            var controller = html.ViewContext.RequestContext.RouteData.Values.First(a => a.Key == "controller").Value.ToString().ToLower();
            var action = html.ViewContext.RequestContext.RouteData.Values.First(a => a.Key == "action").Value.ToString().ToLower();
            var area = html.ViewContext.RouteData.DataTokens["area"]?.ToString().ToLower() ?? "";

            switch (controller)
            {
                case "order":
                    switch (action)
                    {
                        case "upload":
                            tag.SetInnerText("Upload File");
                            break;
                        case "processfile":
                            tag.SetInnerText("Preparing File");
                            break;
                        case "selectproducts":
                            tag.SetInnerText("Select Products");
                            break;
                        case "ordereeceived":
                            tag.SetInnerText("Order Placed");
                            break;
                    }
                    break;
                case "signup":
                    switch (area)
                    {
                        case "NationBuilder":
                            tag.SetInnerText("NationBuilder Data Append App");
                            break;
                        default:
                            tag.SetInnerText("Accurate Append Data App");
                            break;
                    }
                    break;
                case "direct":
                    switch (action)
                    {
                        case "login":
                            tag.SetInnerText("Log In: Accurate Append");
                            break;
                    }
                    break;
                case "current":
                    switch (action)
                    {
                        case "index":
                            tag.SetInnerText("Orders");
                            break;
                    }
                    break;
                case "displaylists":
                    switch (action)
                    {
                        case "index":
                            tag.SetInnerText("Lists");
                            break;
                    }
                    break;
                case "contact":
                    switch (action)
                    {
                        case "index":
                            tag.SetInnerText("Contact Info");
                            break;
                    }
                    break;
                case "card":
                    switch (action)
                    {
                        case "index":
                            tag.SetInnerText("Payment Info");
                            break;
                    }
                    break;
                case "password":
                    switch (action)
                    {
                        case "index":
                            tag.SetInnerText("Password Info");
                            break;
                    }
                    break;
                case "nation":
                    switch (action)
                    {
                        case "index":
                            tag.SetInnerText("Nations");
                            break;
                    }
                    break;
                case "dataqualityassesment":
                    tag.SetInnerText("Data Quality Assessment");
                    break;
                
                case "newpassword":
                    tag.SetInnerText("Update Password");
                    break;
                case "resetpassword":
                    switch (action)
                    {
                        case "request":
                        case "requestsent":
                            tag.SetInnerText("Request New Password");
                            break;
                        default:
                            tag.SetInnerText("Update Password");
                            break;
                    }
                    break;
                case "facebook":
                case "google":
                    tag.SetInnerText("Link Accounts");
                    break;
                case "file":
                    tag.SetInnerText("Upload File");
                    break;
                case "payment":
                    tag.SetInnerText("Update Credit Card");
                    break;
                case "newclientregistration":
                    tag.SetInnerText("Registration");
                    break;
            }


            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Gets the meta description element for the current path.
        /// </summary>
        public static MvcHtmlString GetMetaDescription(this HtmlHelper html)
        {
            var tag = new TagBuilder("meta");

            tag.Attributes.Add("name", "description");

            var controller = html.ViewContext.RequestContext.RouteData.Values.First(a => a.Key == "controller").Value.ToString().ToLower();
            var action = html.ViewContext.RequestContext.RouteData.Values.First(a => a.Key == "action").Value.ToString().ToLower();
            var area = html.ViewContext.RouteData.DataTokens["area"]?.ToString().ToLower() ?? "";

            switch (controller)
            {
                // target keywords: Accurate Append, NationBuilder, Phone Append, Email Append, Data Append

                case "signup":
                    switch (area)
                    {
                        case "nationbuilder":
                            switch (action)
                            {
                                case "index":
                                    tag.Attributes.Add("content", "Phone append, email append, and political data append directly in your NationBuilder account with Accurate Append.");
                                    break;
                            }
                            break;
                        default:
                            switch (action)
                            {
                                case "index":
                                    tag.Attributes.Add("content", "Phone append, email append, and data append with Accurate Append.");
                                    break;
                            }
                            break;
                    }
                    break;
                case "direct":
                    switch (action)
                    {
                        case "login":
                            tag.Attributes.Add("content", "Log In to Accurate Append and cost effectively improve the quality of your lists.");
                            break;
                    }
                    break;
            }

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }
    }
}