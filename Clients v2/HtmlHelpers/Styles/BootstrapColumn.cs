using System;
using System.ComponentModel;
using System.Web.Mvc;
using AccurateAppend.Core;

namespace AccurateAppend.Websites.Clients.HtmlHelpers.Styles
{
    public static partial class PageLayoutHtmlHelper
    {
        /// <summary>
        /// Html Helper used to create and manage Bootstrap grid column.
        /// </summary>
        public sealed class BootstrapColumn : MvcElement
        {
            private readonly TagBuilder tagBuilder;

            internal BootstrapColumn(HtmlHelper htmlHelper, Int32 width, Target target, String style = null) : base(htmlHelper)
            {
                /* <div class="col-12 col-md-{width}">
                 </div> */

                this.tagBuilder = new TagBuilder("div");
                tagBuilder.AddCssClass($"col-{target.GetDescription()}-{width}");
                tagBuilder.AddCssClass("col-12");

                if (style != null) tagBuilder.AddStyle(style);
            }

            /// <inheritdoc />
            protected override TagBuilder Tag => this.tagBuilder;

            /// <summary>
            /// Opens a scope for a new <see cref="BootstrapColumn"/> without requiring the fluent API.
            /// </summary>
            /// <remarks>
            /// <example>
            /// <code>
            /// <![CDATA[
            /// @using (BootstrapColumn.BeginColumn())
            /// {
            ///   <p>Lorum ipsum.</p>
            ///   <h1>Title</h1>
            /// }
            /// ]]>
            /// </code>
            /// </example>
            /// </remarks>
            public static IDisposable BeginColumn(HtmlHelper htmlHelper, Int32 width = 6, Target target = Target.Md)
            {
                var element = new BootstrapColumn(htmlHelper, width, target);
                element.Open();

                var component = new Component();
                component.Disposed += (s, e) => { element.Close(); };

                return component;
            }
        }
    }
}