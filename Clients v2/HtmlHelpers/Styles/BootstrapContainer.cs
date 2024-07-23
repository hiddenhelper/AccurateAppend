using System;
using System.ComponentModel;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.HtmlHelpers.Styles
{
    public static partial class PageLayoutHtmlHelper
    {
        /// <summary>
        /// Html Helper used to create and manage Bootstrap layout container.
        /// </summary>
        public sealed class BootstrapContainer : MvcElement
        {
            private readonly TagBuilder tagBuilder;

            internal BootstrapContainer(HtmlHelper htmlHelper) : base(htmlHelper)
            {
                /*<div class="container">
                 </div> */

                this.tagBuilder = new TagBuilder("div");
                this.tagBuilder.AddCssClass("container");
            }

            /// <inheritdoc />
            protected override TagBuilder Tag => this.tagBuilder;

            /// <summary>
            /// Writes an opening  &lt;div&gt; tag required to render a Bootstrap row tags that will act as a holder for html elements
            /// &lt;div class="row"&gt;
            /// </summary>
            /// <remarks>
            /// When the result of this method is disposed, the closing &lt;div&gt; tag will be written the response stream.
            ///  <example>
            /// <code>
            /// <![CDATA[
            /// @using (var container = this.Html.Layout().Container())
            /// {
            ///   @using (container.Row())
            ///   {
            ///     <p>Lorum ipsum.</p>
            ///     <h1>Title</h1>
            ///   }
            /// }
            /// ]]>
            /// </code>
            /// </example>
            /// </remarks>
            /// <returns>an opening &lt; div class="row" &gt; html tag.</returns>
            public BootstrapRow Row(String css = null)
            {
                var element = new BootstrapRow(this.Helper, css);
                element.Open();

                return element;
            }

            /// <summary>
            /// Opens a scope for a new <see cref="BootstrapContainer"/> without requiring the fluent API.
            /// </summary>
            /// <remarks>
            /// <example>
            /// <code>
            /// <![CDATA[
            /// @using (BootstrapContainer.BeginContainer())
            /// {
            ///   <p>Lorum ipsum.</p>
            ///   <h1>Title</h1>
            /// }
            /// ]]>
            /// </code>
            /// </example>
            /// </remarks>
            public static IDisposable BeginContainer(HtmlHelper htmlHelper)
            {
                var element = new BootstrapContainer(htmlHelper);
                element.Open();

                var component = new Component();
                component.Disposed += (s, e) => { element.Close(); };

                return component;
            }
        }
    }
}