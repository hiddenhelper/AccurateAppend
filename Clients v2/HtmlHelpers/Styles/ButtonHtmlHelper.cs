using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace AccurateAppend.Websites.Clients.HtmlHelpers.Styles
{
    /// <summary>
    /// Html Helper to generate AA styled html buttons.
    /// </summary>
    public static class ButtonHtmlHelper
    {
        #region Entry Point

        /// <summary>
        /// Provides access to the styled button helpers.
        /// </summary>
        /// <remarks>
        /// Works like a logical namespace for AA HTML button helpers to reduce the API surface exposed by all
        /// <see cref="HtmlHelper"/> references. By reducing the intellisense complexity, we reduce the cognitive
        /// complexity budget for the developers.
        ///
        /// How To access the button extension methods
        /// <example>
        /// <code>
        /// <![CDATA[
        ///
        /// @this.Html.Buttons()
        /// ]]>
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <returns>A <see cref="ButtonHelper"/> for use by the current view.</returns>
        public static ButtonHelper Buttons(this HtmlHelper htmlHelper)
        {
            return new ButtonHelper(htmlHelper);
        }

        #endregion

        #region Html Helper Logic

        /// <summary>
        /// HTML Helper class that collates all style button logic.
        /// </summary>
        public sealed class ButtonHelper
        {
            private readonly HtmlHelper htmlHelper;

            internal ButtonHelper(HtmlHelper htmlHelper)
            {
                Debug.Assert(htmlHelper != null, nameof(htmlHelper));

                this.htmlHelper = htmlHelper;
            }

            /// <summary>Returns a top navigation styled anchor element (a element) for the specified link text, action, controller, route values, and HTML attributes.</summary>
            /// <returns>An anchor element (a element).</returns>
            /// <param name="linkText">The inner text of the anchor element.</param>
            /// <param name="actionName">The name of the action.</param>
            /// <param name="controllerName">The name of the controller.</param>
            /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. The object is typically created by using object initializer syntax.</param>
            /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
            /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
            public IHtmlString TopNavButton(String linkText, String actionName, String controllerName = null, Object routeValues = null, Object htmlAttributes = null)
            {
                return StyledButton(this.htmlHelper, linkText, actionName, controllerName, routeValues, ButtonType.TopNav, htmlAttributes);
            }

            /// <summary>Returns a Profile styled anchor element (a element) for the specified link text, action, controller, route values, and HTML attributes.</summary>
            /// <returns>An anchor element (a element).</returns>
            /// <param name="linkText">The inner text of the anchor element.</param>
            /// <param name="actionName">The name of the action.</param>
            /// <param name="controllerName">The name of the controller.</param>
            /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. The object is typically created by using object initializer syntax.</param>
            /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
            /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
            public IHtmlString ProfileButton(String linkText, String actionName, String controllerName = null, Object routeValues = null, Object htmlAttributes = null)
            {
                return StyledButton(this.htmlHelper, linkText, actionName, controllerName, routeValues, ButtonType.Profile, htmlAttributes);
            }

            /// <summary>Returns a styled anchor element (a element) for the specified link text, action, controller, route values, and HTML attributes.</summary>
            /// <returns>An anchor element (a element).</returns>
            /// <param name="linkText">The inner text of the anchor element.</param>
            /// <param name="actionName">The name of the action.</param>
            /// <param name="controllerName">The name of the controller.</param>
            /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. The object is typically created by using object initializer syntax.</param>
            /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
            /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
            public IHtmlString PrimaryButton(String linkText, String actionName, String controllerName = null, Object routeValues = null, Object htmlAttributes = null)
            {
                return StyledButton(this.htmlHelper, linkText, actionName, controllerName, routeValues, ButtonType.Primary, null);
            }

            /// <summary>Returns a styled anchor element (a element) for the specified link text, action, controller, route values, and HTML attributes.</summary>
            /// <returns>An anchor element (a element).</returns>
            /// <param name="linkText">The inner text of the anchor element.</param>
            /// <param name="actionName">The name of the action.</param>
            /// <param name="controllerName">The name of the controller.</param>
            /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. The object is typically created by using object initializer syntax.</param>
            /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
            /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
            public IHtmlString PrimaryButtonLarge(String linkText, String actionName, String controllerName = null, Object routeValues = null, Object htmlAttributes = null)
            {
                return StyledButton(this.htmlHelper, linkText, actionName, controllerName, routeValues, ButtonType.PrimaryLarge, htmlAttributes);
            }

            /// <summary>Returns a styled anchor element (a element) for the specified link text, action, controller, route values, and HTML attributes.</summary>
            /// <returns>An anchor element (a element).</returns>
            /// <param name="linkText">The inner text of the anchor element.</param>
            /// <param name="actionName">The name of the action.</param>
            /// <param name="controllerName">The name of the controller.</param>
            /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. The object is typically created by using object initializer syntax.</param>
            /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
            /// <exception cref="T:System.ArgumentException">The <paramref name="linkText" /> parameter is null or empty.</exception>
            public IHtmlString SecondaryButton(String linkText, String actionName, String controllerName = null, Object routeValues = null, Object htmlAttributes = null)
            {
                return StyledButton(this.htmlHelper, linkText, actionName, controllerName, routeValues, ButtonType.Secondary, htmlAttributes);
            }

            /// <summary>
            /// Writes an opening &lt;div&gt; tag to the response. The container div that will contain the styled buttons, in order for the buttons to be styled correctly the buttons should be added in these.
            /// </summary>
            /// <remarks>
            /// The BeginButtonHolder method renders a div block. You can use this method in a using block. In that case, the method renders the closing &lt;/div&gt; tag at the end of the using block.
            /// </remarks>
            /// <param name="buttonHolderType">Indicates the type of contained buttons.</param>
            /// <returns>An opening &lt;div&gt; tag.</returns>
            public IDisposable BeginButtonHolder(ButtonHolderType buttonHolderType)
            {
                var writer = this.htmlHelper.ViewContext.Writer;
                var css = String.Empty;
                switch (buttonHolderType)
                {
                    case ButtonHolderType.Button:
                        css = "btn-holder";
                        break;
                    case ButtonHolderType.Link:
                        css = "link-holder";
                        break;
                }

                var div = new TagBuilder("div");
                div.AddCssClass(css);

                writer.Write(div.ToString(TagRenderMode.StartTag));

                var dispoable = new Component();
                dispoable.Disposed += (s, e) => writer.WriteLine(div.ToString(TagRenderMode.EndTag));

                return dispoable;
            }

            /// <summary>
            /// Performs the work to generate the html that is styled correctly with the right css class for the theme that is being used.
            /// </summary>
            /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
            /// <param name="linkText">The inner text of the anchor element.</param>
            /// <param name="actionName">The name of the action.</param>
            /// <param name="controllerName">The name of the controller.</param>
            /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. The object is typically created by using object initializer syntax.</param>
            /// <param name="buttonType">Indicates what style of button to create. </param>
            /// <param name="htmlAttributes">An object that contains the HTML attributes to set for the element.</param>
            /// <returns>An anchor element (a element).</returns>
            private static MvcHtmlString StyledButton(HtmlHelper htmlHelper, String linkText, String actionName, String controllerName, Object routeValues, ButtonType buttonType, Object htmlAttributes)
            {
                dynamic expando = new ExpandoObject();
                var dictionary = (IDictionary<String, Object>)expando;

                if (htmlAttributes != null)
                {
                    //HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                    foreach (var property in htmlAttributes.GetType().GetProperties())
                    {
                        dictionary.Add(property.Name, property.GetValue(htmlAttributes));
                    }
                }

                String css;
                
                switch (buttonType)
                {
                    case ButtonType.Primary:
                        css = "btn btn-primary";
                        break;
                    case ButtonType.Secondary:
                        css = "btn btn-secondary";
                        break;
                    case ButtonType.PrimaryLarge:
                        css = "btn btn-primary large";
                        break;
                    case ButtonType.Link:
                        css = String.Empty;
                        break;
                    case ButtonType.Profile:
                        css = "btn btn-primary profile";
                        break;
                    case ButtonType.TopNav:
                        css = "log-in";
                        break;
                    default:
                        css = "btn";
                        break;
                }

                expando.@class = css;

                var button = htmlHelper.ActionLink(linkText,
                    actionName,
                    controllerName,
                    new RouteValueDictionary(routeValues),
                    dictionary);

                return button;
            }
        }

        #endregion
    }
}