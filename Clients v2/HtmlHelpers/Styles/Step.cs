using System;
using System.Web;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.HtmlHelpers.Styles
{
    public static partial class PageLayoutHtmlHelper
    {
        /// <summary>
        /// Html Helper used to create and manage Bootstrap step bar item content.
        /// </summary>
        public class Step : IHtmlString
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Step"/> class.
            /// </summary>
            public Step() : this(MvcHtmlString.Empty)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Step"/> class.
            /// </summary>
            /// <param name="innerText">The inner text content that will be added to the step item. This value will be HTML encoded.</param>
            /// <param name="isActive">Indicates whether this step item should be rendered as an active item.</param>
            public Step(String innerText, Boolean isActive = false) : this(MvcHtmlString.Create(innerText), isActive)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Step"/> class.
            /// </summary>
            /// <param name="innerHtml">The inner HTML content that will be added to the step item.</param>
            /// <param name="isActive">Indicates whether this step item should be rendered as an active item.</param>
            public Step(MvcHtmlString innerHtml, Boolean isActive = false)
            {
                innerHtml = innerHtml ?? MvcHtmlString.Empty;

                this.InnerHtml = innerHtml.ToString();
                this.IsActive = isActive;
            }

            /// <summary>Gets or sets the inner HTML value for the element.</summary>
            /// <returns>The inner HTML value for the element.</returns>
            public String InnerHtml { get; set; }

            /// <summary>
            /// Indicates whether the current step item should be styled as being the active element.
            /// </summary>
            public Boolean IsActive { get; set; }

            #region IHtmlString Members

            /// <inheritdoc />
            public virtual String ToHtmlString()
            {
                var tagBuilder = new TagBuilder("li");
                tagBuilder.InnerHtml = this.InnerHtml;

                if (this.IsActive) tagBuilder.AddCssClass("active");

                return tagBuilder.ToString();
            }

            #endregion
        }
    }
}