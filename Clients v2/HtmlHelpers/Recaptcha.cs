using System;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.HtmlHelpers
{
    /// <summary>
    /// HTML Helpers for interacting with Google Recaptcha.
    /// </summary>
    public static class RecaptchaIntegration
    {
        #region Entry Point

        /// <summary>
        /// Provides access to the reCAPTCHA integration helpers.
        /// </summary>
        /// <remarks>
        /// Works like a logical namespace for reCAPTCHA verification  helpers to reduce the API surface exposed by all
        /// <see cref="HtmlHelper"/> references. By reducing the intellisense complexity, we reduce the cognitive
        /// complexity budget for the developers.
        ///
        /// How To access the reCAPTCHA extension methods
        /// <example>
        /// <code>
        /// <![CDATA[
        ///
        /// @this.Html.Recaptcha()
        /// ]]>
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <returns>A <see cref="RecaptchaHelper"/> for use by the current view.</returns>
        public static RecaptchaHelper Recaptcha(this HtmlHelper htmlHelper)
        {
            return new RecaptchaHelper();
        }

        #endregion

        #region Html Helper Logic

        /// <summary>
        /// HTML Helper class that collates all reCAPTCHA integration logic.
        /// </summary>
        public sealed class RecaptchaHelper
        {
            internal RecaptchaHelper()
            {
            }

            /// <summary>
            /// Creates a google analytics script element for the application.
            /// </summary>
            public MvcHtmlString GenerateScript()
            {
                return MvcHtmlString.Create("<script src=\"https://www.google.com/recaptcha/api.js\"></script>");
            }

            /// <summary>
            /// Creates a div to display a Recaptcha control.
            /// </summary>
            /// <remarks>
            /// Default callbacks are "imNotARobot" and "tooSlow".
            /// </remarks>
            /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
            /// <param name="dataCallback">The optional name of a global JS function to callback on the validation of the proof.</param>
            /// <param name="dataExpiredCallback">The optional name of a global JS function to callback on the expiration of the validation.</param>
            /// <returns>A HTML element for the Recaptcha control.</returns>
            public MvcHtmlString RenderControl(Object htmlAttributes = null, String dataCallback = "imNotARobot", String dataExpiredCallback = "tooSlow")
            {
                var builder = new TagBuilder("div");
                builder.AddCssClass("g-recaptcha");
                builder.Attributes.Add("data-sitekey", "6LeW4EkUAAAAAPX3-Zd61mquDOX_P26WRlgCvtAf");

                var dictionary = HtmlHelper.ObjectToDictionary(htmlAttributes);
                foreach (var item in dictionary)
                {
                    builder.Attributes.Add(item.Key, item.Value?.ToString());
                }

                if (!String.IsNullOrWhiteSpace(dataCallback)) builder.Attributes.Add("data-callback", dataCallback);
                if (!String.IsNullOrWhiteSpace(dataExpiredCallback)) builder.Attributes.Add("data-expired-callback", dataExpiredCallback);

                return new MvcHtmlString(builder.ToString());
            }
        }

        #endregion
    }
}