using System;
using System.ComponentModel;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.HtmlHelpers.Styles
{
    public static partial class PageLayoutHtmlHelper
    {
        /// <summary>
        /// Html Helper used to create and manage Bootstrap grid row.
        /// </summary>
        public sealed class BootstrapRow : MvcElement
        {
            private readonly TagBuilder tagBuilder;

            internal BootstrapRow(HtmlHelper htmlHelper, String css = null) : base(htmlHelper)
            {
                /*<div class="row">
                 </div> */

                this.tagBuilder = new TagBuilder("div");
                this.tagBuilder.AddCssClass("row");
                if(css != null) tagBuilder.AddCssClass(css);
            }

            /// <inheritdoc />
            protected override TagBuilder Tag => this.tagBuilder;

            /// <summary>
            /// Writes an opening  &lt;div&gt; tag required to render a Bootstrap row tags that will act as a holder for html elements
            /// &lt;div class="row"&gt;
            /// </summary>
            /// <remarks>
            /// When the result of this method is disposed, the closing &lt;div&gt; tag will be written the response stream.
            /// <example>
            /// <code>
            /// <![CDATA[
            /// @using (var container = this.Html.Layout().Container())
            /// {
            ///   @using (var row = container.Row())
            ///   {
            ///     @using (row.Column())
            ///     {
            ///       <p>Lorum ipsum.</p>
            ///       <h1>Title</h1>
            ///     }
            ///   }
            /// }
            /// ]]>
            /// </code>
            /// </example>
            /// </remarks>
            /// <returns>an opening &lt; div class="col-12 col-md-{width}" &gt; html tag.</returns>
            public IDisposable Column(Int32 width = 6, Target target = Target.Md, String style = null)
            {
                var element = new BootstrapColumn(this.Helper, width, target, style);
                element.Open();

                return element;
            }
            
            /// <summary>
            /// Opens a scope for a new <see cref="BootstrapRow"/> without requiring the fluent API.
            /// </summary>
            /// <remarks>
            /// <example>
            /// <code>
            /// <![CDATA[
            /// @using (BootstrapRow.BeginRow())
            /// {
            ///   <p>Lorum ipsum.</p>
            ///   <h1>Title</h1>
            /// }
            /// ]]>
            /// </code>
            /// </example>
            /// </remarks>
            public static IDisposable BeginRow(HtmlHelper htmlHelper)
            {
                var element = new BootstrapRow(htmlHelper);
                element.Open();

                var component = new Component();
                component.Disposed += (s, e) => { element.Close(); };

                return component;
            }
        }
    }
}