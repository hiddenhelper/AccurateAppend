using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace AccurateAppend.Websites.Clients.HtmlHelpers.Styles
{
    /// <summary>
    ///     Html helper to build AA styles navbar
    /// </summary>
    public static class NavigationHtmlHelper
    {
        #region Entry Point

        /// <summary>
        /// Provides access to the navigation helper class instances.
        /// </summary>
        /// <remarks>
        /// Works like a logical namespace for AA HTML form helpers to reduce the API surface exposed by all
        /// <see cref="HtmlHelper"/> references. By reducing the intellisense complexity, we reduce the cognitive
        /// complexity budget for the developers.
        ///
        /// How To access the navigation extension methods
        /// <example>
        /// <code>
        /// <![CDATA[
        /// @Html.Nav()
        /// ]]>
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <returns>A <see cref="NavigationHelper"/> for use by the current view.</returns>
        public static NavigationHelper Nav(this HtmlHelper htmlHelper)
        {
            return new NavigationHelper(htmlHelper);
        }

        #endregion

        #region Html Helper Logic

        /// <summary>
        ///     holds all the helper methods for nav container and items generation
        /// </summary>
        public sealed class NavigationHelper
        {
            private readonly HtmlHelper htmlHelper;

            internal NavigationHelper(HtmlHelper htmlHelper)
            {
                Debug.Assert(htmlHelper != null, nameof(htmlHelper));

                this.htmlHelper = htmlHelper;
            }

            /// <summary>
            ///     writes the html required to render the base html for navigation with the content of the menu items left empty
            /// </summary>
            /// <param name="includeBrand">determines if the brand html is included with the html or not</param>
            /// <returns></returns>
            public IDisposable BeginNavBar(Boolean includeBrand = true)
            {
                var writer = htmlHelper.ViewContext.Writer;

                var tagBuilder = new TagBuilder("nav");
                tagBuilder.AddCssClass("navbar");
                tagBuilder.AddCssClass("navbar-expand-lg");
                tagBuilder.AddCssClass("navbar-light");
                writer.Write(tagBuilder.ToString(TagRenderMode.StartTag));

                if (includeBrand)
                {
                    var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
                    writer.Write(NavBarBrand(
                        urlHelper.Content("~/Content/AccurateAppend_v7/images/logo.png"),
                        "/"));
                }

                var navItemsContainerId = $"nabBarContent{DateTime.Now.Ticks}";
                writer.Write(NavBarToggle(navItemsContainerId).ToHtmlString());

                var tagNavCollapse = new TagBuilder("div");
                tagNavCollapse.AddCssClass("collapse");
                tagNavCollapse.AddCssClass("navbar-collapse");
                tagNavCollapse.Attributes.Add("id", navItemsContainerId);
                writer.Write(tagNavCollapse.ToString(TagRenderMode.StartTag));

                var tagNavHolder = new TagBuilder("div");
                tagNavHolder.AddCssClass("nav-holder");
                writer.Write(tagNavHolder.ToString(TagRenderMode.StartTag));

                var disposible = new Component();
                disposible.Disposed += (s, e) =>
                {
                    writer.WriteLine(tagNavCollapse.ToString(TagRenderMode.EndTag));
                    writer.WriteLine(tagNavHolder.ToString(TagRenderMode.EndTag));
                    writer.WriteLine(tagBuilder.ToString(TagRenderMode.EndTag));
                };

                return disposible;
            }

            /// <summary>
            ///     Writes the opening tag for a list of list items, this translates into unordered list html text &lt;ul&gt;....&lt;/ul
            ///     &lt;
            ///     <example>
            ///         @using (Html.Nav().BeginNavBarList())
            ///         {
            ///         }
            ///     </example>
            /// </summary>
            /// <returns></returns>
            public IDisposable BeginNavBarList()
            {
                var writer = htmlHelper.ViewContext.Writer;

                var tagBuilder = new TagBuilder("ul");
                tagBuilder.AddCssClass("navbar-nav");
                tagBuilder.AddCssClass("ml-auto");
                tagBuilder.AddStyle("align-items: center");
                writer.Write(tagBuilder.ToString(TagRenderMode.StartTag));

                var disposable = new Component();
                disposable.Disposed += (s, e) => { writer.WriteLine(tagBuilder.ToString(TagRenderMode.EndTag)); };

                return disposable;
            }

            /// <summary>
            ///     The method is use to generate the list items containing hyper links, it generates  &lt;li&gt;  &lt;a&gt;....&lt;/a
            ///     &lt; &lt;/li&lt;
            /// </summary>
            /// <param name="linkText">text that is displayed on the hyperlink</param>
            /// <param name="actionName">name of the action that is invoked</param>
            /// <param name="controllerName">name of the controller that is invoked</param>
            /// <param name="routeValues">mvc route values</param>
            /// <returns></returns>
            public IHtmlString NavBarLink(String linkText, String actionName, String controllerName = null, Object routeValues = null)
            {
                //var tagBuilder = new TagBuilder("li");
                //tagBuilder.AddCssClass("nav-item");
                var link = htmlHelper.ActionLink(
                    linkText,
                    actionName,
                    controllerName,
                    routeValues,
                    new { @class = "nav-link" }
                ).ToHtmlString();
                //tagBuilder.InnerHtml = link;

                return this.NavBarLink(new MvcHtmlString(link));

                //return new MvcHtmlString(tagBuilder.ToString());
            }

            /// <summary>
            ///     The method is use to generate the list items containing hyper links, it generates  &lt;li&gt;  &lt;a&gt;....&lt;/a
            ///     &lt; &lt;/li&lt;
            /// </summary>
            public IHtmlString NavBarLink(IHtmlString rawContent)
            {
                var tagBuilder = new TagBuilder("li");
                tagBuilder.AddCssClass("nav-item");
                tagBuilder.InnerHtml = rawContent.ToHtmlString();

                return new MvcHtmlString(tagBuilder.ToString());
            }

            /// <summary>
            /// this method is used to generate list items that contain anchors that point to an external web address
            /// </summary>
            /// <param name="linkText">caption of the hyper link</param>
            /// <param name="externalUrl">full address of the destination site including http/https</param>
            /// <returns></returns>
            public IHtmlString NavBarDirectLink(String linkText, String externalUrl)
            {
                var tagBuilder = new TagBuilder("li");
                tagBuilder.AddCssClass("nav-item");

                var anchorTagBuilder = new TagBuilder("a");
                anchorTagBuilder.Attributes.Add("href", externalUrl);
                anchorTagBuilder.AddCssClass("nav-link");
                anchorTagBuilder.SetInnerText(linkText);

                tagBuilder.InnerHtml = anchorTagBuilder.ToString();

                return new MvcHtmlString(tagBuilder.ToString());
            }

            /// <summary>
            ///     internal method that used to generate the brand html element, this can be exposed to public if needed
            /// </summary>
            /// <param name="brandImageUrl">the url for the image source</param>
            /// <param name="brandUrl">the home page url</param>
            /// <returns></returns>
            private IHtmlString NavBarBrand(String brandImageUrl, String brandUrl)
            {
                var tagBuilder = new TagBuilder("a");
                tagBuilder.AddCssClass("navbar-brand");
                tagBuilder.MergeAttribute("href", brandUrl);

                var imgTagBuilder = new TagBuilder("img");
                imgTagBuilder.MergeAttribute("src", brandImageUrl);
                imgTagBuilder.MergeAttribute("width", "356");
                imgTagBuilder.MergeAttribute("height", "60");
                tagBuilder.InnerHtml = imgTagBuilder.ToString();

                return new MvcHtmlString(tagBuilder.ToString());
            }

            /// <summary>
            ///     by default all the nav bars would have this element in order to show the toggle button when the menu is hidden on
            ///     mobile devices
            ///     this can be exposed if needed to be configurable
            /// </summary>
            /// <param name="toggleTargetId"></param>
            /// <returns></returns>
            private IHtmlString NavBarToggle(String toggleTargetId)
            {
                var tagBuilder = new TagBuilder("button");
                tagBuilder.AddCssClass("navbar-toggler");
                tagBuilder.Attributes.Add("type", "button");
                tagBuilder.Attributes.Add("data-toggle", "collapse");
                tagBuilder.Attributes.Add("data-target", "#" + toggleTargetId);
                tagBuilder.Attributes.Add("aria-controls", toggleTargetId);
                tagBuilder.Attributes.Add("aria-expanded", "false");
                tagBuilder.Attributes.Add("aria-label", "Toggle navigation");

                var spanTagBuilder = new TagBuilder("span");
                spanTagBuilder.AddCssClass("navbar-toggler-icon");

                tagBuilder.InnerHtml = spanTagBuilder.ToString();

                return new MvcHtmlString(tagBuilder.ToString());
            }
        }

        #endregion
    }
}