using System;
using System.Web;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.HtmlHelpers.Styles
{
    /// <summary>
    /// Html helper to produce Bootstrap 4 alert blocks.
    /// </summary>
    public static class AlertsHtmlHelper
    {
        #region Fields

        private static readonly AlertHelper Instance = new AlertHelper();

        #endregion

        #region Entry Point

        /// <summary>
        /// Provides access to the styled alert helpers/
        /// </summary>
        /// <remarks>
        /// Works like a logical namespace for Bootstrap 4 HTML helpers to reduce the API surface exposed by all
        /// <see cref="HtmlHelper"/> references. By reducing the intellisense complexity, we reduce the cognitive
        /// complexity budget for the developers.
        ///
        /// How To access the alert extension methods
        /// <example>
        /// <code>
        /// <![CDATA[
        /// @this.Html.Alerts()
        /// ]]>
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <returns>A <see cref="AlertHelper"/> for use by the current view.</returns>
        public static AlertHelper Alerts(this HtmlHelper htmlHelper)
        {
            return Instance;
        }

        #endregion

        #region Html Helper Logic

        /// <summary>
        /// HTML Helper class that collates all alert element logic.
        /// </summary>
        public sealed class AlertHelper
        {
            internal AlertHelper()
            {
            }

            /// <summary>
            /// Writes a complete &lt;div&gt; tag required to render a Bootstrap alert element that will contain the supplied message.
            /// </summary>
            /// <remarks>
            /// <example>
            /// <code>
            /// <![CDATA[
            /// @Html.Alerts().Primary("Your alert message")
            /// ]]>
            /// </code>
            /// </example>
            /// </remarks>
            /// <returns>A &lt; div &gt; html tag.</returns>
            public IHtmlString Primary(String message)
            {
                return this.Primary(MvcHtmlString.Create(message));
            }

            /// <summary>
            /// Writes a complete &lt;div&gt; tag required to render a Bootstrap alert element that will contain the supplied message.
            /// </summary>
            /// <remarks>
            /// <example>
            /// <code>
            /// <![CDATA[
            /// // display a primary alert with embedded html button
            /// @Html.Alerts().Primary(this.Html.ActionLink("Your alert message", "YourAction"))
            /// ]]>
            /// </code>
            /// </example>
            /// </remarks>
            /// <returns>A &lt; div &gt; html tag.</returns>
            public IHtmlString Primary(IHtmlString message)
            {
                var div = new TagBuilder("div");
                div.AddCssClass("alert alert-primary");
                div.MergeAttribute("role", "alert");
                div.InnerHtml = message.ToHtmlString();

                return MvcHtmlString.Create(div.ToString());
            }
            
            /// <summary>
            /// Writes a complete &lt;div&gt; tag required to render a Bootstrap alert element that will contain the supplied message.
            /// </summary>
            /// <remarks>
            /// <example>
            /// <code>
            /// <![CDATA[
            /// @Html.Alerts().Success("Your alert message")
            /// ]]>
            /// </code>
            /// </example>
            /// </remarks>
            /// <returns>A &lt; div &gt; html tag.</returns>
            public IHtmlString Success(String message)
            {
                return this.Success(MvcHtmlString.Create(message));
            }

            /// <summary>
            /// Writes a complete &lt;div&gt; tag required to render a Bootstrap alert element that will contain the supplied message.
            /// </summary>
            /// <remarks>
            /// <example>
            /// <code>
            /// <![CDATA[
            /// // display a primary alert with embedded html button
            /// @Html.Alerts().Success(this.Html.ActionLink("Your alert message", "YourAction"))
            /// ]]>
            /// </code>
            /// </example>
            /// </remarks>
            /// <returns>A &lt; div &gt; html tag.</returns>
            public IHtmlString Success(IHtmlString message)
            {
                var div = new TagBuilder("div");
                div.AddCssClass("alert alert-success");
                div.MergeAttribute("role", "alert");
                div.InnerHtml = message.ToHtmlString();

                return MvcHtmlString.Create(div.ToString());
            }

            /// <summary>
            /// Writes a complete &lt;div&gt; tag required to render a Bootstrap alert element that will contain the supplied message.
            /// </summary>
            /// <remarks>
            /// <example>
            /// <code>
            /// <![CDATA[
            /// @Html.Alerts().Danger("Your alert message")
            /// ]]>
            /// </code>
            /// </example>
            /// </remarks>
            /// <returns>A &lt; div &gt; html tag.</returns>
            public IHtmlString Danger(String message)
            {
                return this.Danger(MvcHtmlString.Create(message));
            }

            /// <summary>
            /// Writes a complete &lt;div&gt; tag required to render a Bootstrap alert element that will contain the supplied message.
            /// </summary>
            /// <remarks>
            /// <example>
            /// <code>
            /// <![CDATA[
            /// // display a primary alert with embedded html button
            /// @Html.Alerts().Danger(this.Html.ActionLink("Your alert message", "YourAction"))
            /// ]]>
            /// </code>
            /// </example>
            /// </remarks>
            /// <returns>A &lt; div &gt; html tag.</returns>
            public IHtmlString Danger(IHtmlString message)
            {
                var div = new TagBuilder("div");
                div.AddCssClass("alert alert-danger");
                div.MergeAttribute("role", "alert");
                div.InnerHtml = message.ToHtmlString();

                return MvcHtmlString.Create(div.ToString());
            }

            /// <summary>
            /// Writes a complete &lt;div&gt; tag required to render a Bootstrap alert element that will contain the supplied message.
            /// </summary>
            /// <remarks>
            /// <example>
            /// <code>
            /// <![CDATA[
            /// @Html.Alerts().Warning("Your alert message")
            /// ]]>
            /// </code>
            /// </example>
            /// </remarks>
            /// <returns>A &lt; div &gt; html tag.</returns>
            public IHtmlString Warning(String message)
            {
                return this.Warning(MvcHtmlString.Create(message));
            }

            /// <summary>
            /// Writes a complete &lt;div&gt; tag required to render a Bootstrap alert element that will contain the supplied message.
            /// </summary>
            /// <remarks>
            /// <example>
            /// <code>
            /// <![CDATA[
            /// // display a primary alert with embedded html button
            /// @Html.Alerts().Warning(this.Html.ActionLink("Your alert message", "YourAction"))
            /// ]]>
            /// </code>
            /// </example>
            /// </remarks>
            /// <returns>A &lt; div &gt; html tag.</returns>
            public IHtmlString Warning(IHtmlString message)
            {
                var div = new TagBuilder("div");
                div.AddCssClass("alert alert-warning");
                div.MergeAttribute("role", "alert");
                div.InnerHtml = message.ToHtmlString();

                return MvcHtmlString.Create(div.ToString());
            }
        }

        #endregion
    }
}