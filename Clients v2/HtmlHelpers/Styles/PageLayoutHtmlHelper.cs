using System;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.HtmlHelpers.Styles
{
    /// <summary>
    /// Html helper to build AA page layout styles.
    /// </summary>
    public static partial class PageLayoutHtmlHelper
    {
        #region Entry Point

        /// <summary>
        /// Provides access to the styled page layout helpers.
        /// </summary>
        /// <remarks>
        /// Works like a logical namespace for AA HTML form helpers to reduce the API surface exposed by all
        /// <see cref="HtmlHelper"/> references. By reducing the intellisense complexity, we reduce the cognitive
        /// complexity budget for the developers.
        ///
        /// How To access the layout extension methods
        /// <example>
        /// <code>
        /// <![CDATA[
        /// @this.Html.Layout()
        /// ]]>
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <returns>A <see cref="LayoutHelper"/> for use by the current view.</returns>
        public static LayoutHelper Layout(this HtmlHelper htmlHelper)
        {
            return new LayoutHelper(htmlHelper);
        }

        #endregion

        #region Html Helper Logic

        /// <summary>
        /// HTML Helper class that collates all styled page layout logic.
        /// </summary>
        public sealed class LayoutHelper
        {
            private readonly HtmlHelper htmlHelper;

            internal LayoutHelper(HtmlHelper htmlHelper)
            {
                Debug.Assert(htmlHelper != null, nameof(htmlHelper));

                this.htmlHelper = htmlHelper;
            }

            /// <summary>
            /// Writes an opening  &lt;div&gt; tag required to render a Bootstrap row tags that will act as a holder for html elements
            /// &lt;div class="container"&gt;
            /// </summary>
            /// <remarks>
            /// When the result of this method is disposed, the closing &lt;div&gt; tag will be written the response stream.
            ///  <example>
            /// <code>
            /// <![CDATA[
            /// @using (var container = this.Html.Layout().Container())
            /// {
            ///  <p>Lorum ipsum.</p>
            ///  <h1>Title</h1>
            /// }
            /// ]]>
            /// </code>
            /// </example>
            /// </remarks>
            /// <returns>an opening &lt; div class="container" &gt; html tag.</returns>
            public BootstrapContainer Container()
            {
                var element = new BootstrapContainer(this.htmlHelper);
                element.Open();

                return element;
            }

            /// <summary>
            /// Returns a styled and populated promo section (section element) with the specified title.
            /// </summary>
            /// <remarks>
            /// <code>
            /// <![CDATA[
            /// @this.Html.Layout().Promo("Your Section Title") 
            /// ]]>
            /// </code>
            /// </remarks>
            /// <param name="linkText">The page promo title value.</param>
            /// <returns>A section (section element).</returns>
            public IHtmlString Promo(String linkText)
            {
                var sectionTag = new TagBuilder("section");
                sectionTag.AddCssClass("promo-section");

                var image = this.Image();

                var title = this.Title(linkText);

                sectionTag.InnerHtml = image.ToHtmlString() + title.ToHtmlString();

                return new MvcHtmlString(sectionTag.ToString());
            }

            private IHtmlString Image()
            {
                /* <div class="bg-stretch">
                 *  <span data-srcset="images/promo-img1.jpg, images/promo-img1.jpg 2x"></span>
                 * </div>*/

                var urlHelper = new UrlHelper(this.htmlHelper.ViewContext.RequestContext);
                var image = urlHelper.Content("~/Content/AccurateAppend_v7/images/promo-img1.jpg");

                var spanTag = new TagBuilder("span");
                spanTag.MergeAttribute("data-srcset", $"{image}, {image} 2x");

                var divTag = new TagBuilder("div");
                divTag.AddCssClass("bg-stretch");
                divTag.InnerHtml = spanTag.ToString(TagRenderMode.SelfClosing);

                return new MvcHtmlString(divTag.ToString());
            }

            private IHtmlString Title(String titleValue)
            {
                /*<div class="container">
                   <div class="row">
                    <div class="col-12">
                     <h1>PAGE TITLE</h1>
                    </div>
                   </div>
                  </div>*/

                var header = new TagBuilder("h1");
                header.SetInnerText(titleValue);

                var colTag = new TagBuilder("div");
                colTag.AddCssClass("col-12");
                colTag.InnerHtml = header.ToString();

                var rowTag = new TagBuilder("div");
                rowTag.AddCssClass("row");
                rowTag.InnerHtml = colTag.ToString();

                var containerTag = new TagBuilder("div");
                containerTag.AddCssClass("container");
                containerTag.InnerHtml = rowTag.ToString();

                return new MvcHtmlString(containerTag.ToString());
            }

            /// <summary>
            /// Returns a styled horizontal rule (hr element).
            /// </summary>
            /// <remarks>
            /// <code>
            /// <![CDATA[
            /// @this.Html.Layout().Hr() 
            /// ]]>
            /// </code>
            /// </remarks>
            /// <returns>A horizontal rule (hr element).</returns>
            public IHtmlString Hr()
            {
                var sectionTag = new TagBuilder("section");
                sectionTag.AddCssClass("hr-section");

                sectionTag.AddCssClass("row");
                sectionTag.AddCssClass("hr");

                return new MvcHtmlString(sectionTag.ToString());
            }

            /// <summary>
            /// Returns a styled bootstrap 4 step block.
            /// </summary>
            /// <remarks>
            /// <code>
            /// <![CDATA[
            /// @this.Html.Layout().Steps(new Step("Step 1") , new Step("Step 2", true));
            /// ]]>
            /// </code>
            /// </remarks>
            /// <returns>A step block (div element).</returns>
            public IHtmlString Steps(params Step[] steps)
            {
                var stepTagContainer = new TagBuilder("div");
                stepTagContainer.AddStyle("background-color: #f4f4f4");

                var stepTag = new TagBuilder("div");
                stepTag.AddCssClass("steps second container");

                var olTag = new TagBuilder("ol");

                var sb = new StringBuilder(steps.Length);
                foreach (var step in steps)
                {
                    sb.AppendLine(step.ToHtmlString());
                }

                olTag.InnerHtml = sb.ToString();

                var helpTag = new TagBuilder("a");
                helpTag.SetInnerText("Help");
                helpTag.AddCssClass("help");
                helpTag.MergeAttribute("href", "https://www.accurateappend.com/contact");

                stepTag.InnerHtml = olTag + helpTag.ToString();
                stepTagContainer.InnerHtml = stepTag.ToString();

                return new MvcHtmlString(stepTagContainer.ToString());
            }

        }

        

        #endregion
    }
}