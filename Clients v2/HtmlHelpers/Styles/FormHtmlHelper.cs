using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace AccurateAppend.Websites.Clients.HtmlHelpers.Styles
{
    /// <summary>
    /// HTML helper to build AA style form html
    /// </summary>
    public static class FormHtmlHelper
    {
        #region Entry Point

        /// <summary>
        /// Provides access to the styled form helpers.
        /// </summary>
        /// <remarks>
        /// Works like a logical namespace for AA HTML form helpers to reduce the API surface exposed by all
        /// <see cref="HtmlHelper"/> references. By reducing the intellisense complexity, we reduce the cognitive
        /// complexity budget for the developers.
        ///
        /// How To access the form extension methods
        /// <example>
        /// <code>
        /// <![CDATA[
        /// @this.Html.Forms()
        /// ]]>
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <returns>A <see cref="FormHelper"/> for use by the current view.</returns>
        public static FormHelper Forms(this HtmlHelper htmlHelper)
        {
            return new FormHelper(htmlHelper);
        }

        #endregion

        /// <summary>
        /// HTML Helper class that collates all style form logic.
        /// </summary>
        public sealed class FormHelper
        {
            private readonly HtmlHelper htmlHelper;

            internal FormHelper(HtmlHelper htmlHelper)
            {
                Debug.Assert(htmlHelper != null, nameof(htmlHelper));

                this.htmlHelper = htmlHelper;
            }

            /// <summary>
            /// Writes an opening &lt;form&gt; tag with AA styling to the response and sets the action tag to the specified controller, action, and route values.
            /// The form uses the specified HTTP method and automatically includes the styled HTML attributes.
            /// </summary>
            /// <remarks>
            /// When the result of this method is disposed, the closing &lt;form&gt; tag will be written the response stream.
            /// <example>
            /// <code>
            /// <![CDATA[
            /// @using (this.Html.Forms().BeginForm(ActionName, ControllerName))
            /// {
            ///   // Do form manipulation
            /// }
            /// ]]>
            /// </code>
            /// </example>
            /// </remarks>
            /// <param name="actionName">The name of the action method.</param>
            /// <param name="controllerName">The name of the controller.</param>
            /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.</param>
            /// <param name="method">The HTTP method for processing the form, either GET or POST.</param>
            /// <returns>An opening &lt;form&gt; tag.</returns>
            public MvcForm BeginForm(String actionName = null, String controllerName = null, Object routeValues = null, FormMethod method = FormMethod.Post)
            {
                return this.htmlHelper.BeginForm(actionName, controllerName, routeValues, method, new {@class = "style-guide"});
            }

            /// <summary>
            /// Writes an opening  &lt;div&gt; tag required to render a sequence of html tags that will act as a holder for html input elements
            /// &lt; div class="input-holder" &gt;
            /// </summary>
            /// <remarks>
            /// When the result of this method is disposed, the closing &lt;div&gt; tag will be written the response stream.
            ///  <example>
            /// <code>
            /// <![CDATA[
            /// @using (var form = this.Html.Forms().BeginForm(ActionName, ControllerName))
            /// {
            ///  using (form.BeginFormRow())
            ///  {
            ///   // Add a form element
            ///   @Html.InputFor(m => m.MyModelProperty)
            ///  }
            /// }
            /// ]]>
            /// </code>
            /// </example>
            /// </remarks>
            /// <returns>an opening &lt; div class="input-holder" &gt; html tag.</returns>
            public IDisposable BeginFormRow(String css=null)
            {
                var writer = this.htmlHelper.ViewContext.Writer;

                var tagBuilder = new TagBuilder("div");
                tagBuilder.AddCssClass("input-row");
                if (!String.IsNullOrEmpty(css)) tagBuilder.AddCssClass(css);

                writer.WriteLine(tagBuilder.ToString(TagRenderMode.StartTag));

                var component = new Component();
                component.Disposed += (s, e) => { writer.WriteLine(tagBuilder.ToString(TagRenderMode.EndTag)); };

                return component;
            }

            /// <summary>
            /// Writes  &lt;div&gt; that can be nested in form row block
            /// </summary>
            /// <param name="css"></param>
            ///  <example>
            /// <code>
            /// <![CDATA[
            /// @using (var form = this.Html.Forms().BeginForm(ActionName, ControllerName))
            /// {
            ///  using (form.BeginFormRow())
            ///  {
            ///   // Add a form element
            ///   using(form.BeginInputHolder())
            ///   {
            ///   }
            ///  }
            /// }
            /// ]]>
            /// </code>
            /// </example>
            /// <returns></returns>
            public IDisposable BeginInputHolder(String css = null)
            {
                var writer = this.htmlHelper.ViewContext.Writer;

                var tagBuilder = new TagBuilder("div");
                tagBuilder.AddCssClass("input-holder");
                if (!String.IsNullOrEmpty(css)) tagBuilder.AddCssClass(css);
                writer.WriteLine(tagBuilder.ToString(TagRenderMode.StartTag));

                var component = new Component();
                component.Disposed += (s, e) => { writer.WriteLine(tagBuilder.ToString(TagRenderMode.EndTag)); };

                return component;
            }

            /// <summary>
            /// Writes an opening  &lt;div&gt; tag required to render a container with in a block of an input group,
            /// normally this will be used to hold multiple input controls for a single label like month year combo
            /// &lt; div class="input-box" &gt;
            /// </summary>
            ///  <example>
            /// <code>
            /// <![CDATA[
            /// @using (var form = this.Html.Forms().BeginForm(ActionName, ControllerName))
            /// {
            ///  using (form.BeginFormRow())
            ///  {
            ///   // Add a form element
            ///   using(form.BeginInputHolder())
            ///   {
            ///     &lt;label&gt; test lable &lt;label&tg;
            ///     using(form.BeginInputBox())
            ///     {
            /// 
            ///     }
            ///   }
            ///  }
            /// }
            /// ]]>
            /// </code>
            /// </example>
            /// <param name="css"></param>
            /// <returns></returns>
            public IDisposable BeginInputBox(String css = null)
            {
                var writer = this.htmlHelper.ViewContext.Writer;

                var tagBuilder = new TagBuilder("div");
                tagBuilder.AddCssClass("input-box");
                if (!String.IsNullOrEmpty(css)) tagBuilder.AddCssClass(css);
                writer.WriteLine(tagBuilder.ToString(TagRenderMode.StartTag));

                var component = new Component();
                component.Disposed += (s, e) => { writer.WriteLine(tagBuilder.ToString(TagRenderMode.EndTag)); };

                return component;
            }
        }
    }
}