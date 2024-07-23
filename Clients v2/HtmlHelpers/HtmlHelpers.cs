using System;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.HtmlHelpers
{
    public static class HtmlHelpers
    {
        public static String SubNavigationLink(this HtmlHelper htmlHelper, String destinationUrl, String buttonText, String requesturl)
        {
            var active = false;
            var url = requesturl.ToLower();

            switch (buttonText.ToLower())
            {
                case "my nationbuilder lists":
                    if (url.ToLower().Contains("displaylists"))
                        active = true;
                    break;
                case "orders":
                    if (url.ToLower().Contains("order"))
                        active = true;
                    break;
                case "my profile":
                    if (url.ToLower().Contains("profile"))
                        active = true;
                    break;
                case "signup":
                    if (url.ToLower() == "/" || url.ToLower() == "/nationbuilder")
                        active = true;
                    break;
                case "faq":
                    if (url.ToLower().Contains("faq"))
                        active = true;
                    break;
                case "login":
                    if (url.ToLower().Contains("authentication"))
                        active = true;
                    break;
                case "products & pricing":
                    if (url.ToLower().Contains("pricing"))
                        active = true;
                    break;
            }

            var anchor = new TagBuilder("a");
            anchor.SetInnerText(buttonText);
            anchor.MergeAttribute("href", destinationUrl);

            var li = new TagBuilder("li");
            if (active) li.AddCssClass("active");
            li.InnerHtml = anchor.ToString();

            return li.ToString();
        }
    }
}