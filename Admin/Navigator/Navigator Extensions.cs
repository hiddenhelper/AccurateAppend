using System.Web.Mvc;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Extension methods specifically for creating navigation use.
    /// </summary>
    public static class NavigationExtensions
    {
        #region Renderer Factory

        /// <summary>
        /// Creates a navigator for the indicated controller.
        /// </summary>
        /// <typeparam name="T">The type of controller to navigate to.</typeparam>
        /// <param name="html">The <seealso cref="HtmlHelper"/> instance.</param>
        public static ActionRenderer<T> RendererFor<T>(this HtmlHelper html) where T : IController
        {
            return new ActionRenderer<T>(html);
        }

        #endregion

        #region UrlBuilder Factory

        /// <summary>
        /// Creates a navigator for the indicated controller.
        /// </summary>
        /// <typeparam name="T">The type of controller to navigate to.</typeparam>
        /// <param name="url">The <seealso cref="UrlHelper"/> instance.</param>
        public static UrlBuilder<T> BuildFor<T>(this UrlHelper url) where T : IController
        {
            return new UrlBuilder<T>(url);
        }

        #endregion

        #region View Factory

        /// <summary>
        /// Creates a navigator for the indicated controller.
        /// </summary>
        /// <typeparam name="T">The type of controller to navigate to.</typeparam>
        /// <param name="html">The <seealso cref="HtmlHelper"/> instance.</param>
        public static ViewNavigator<T> NavigationFor<T>(this HtmlHelper html) where T : IController
        {
            return new ViewNavigator<T>(html);
        }

        #endregion

        #region Controller Factory

        /// <summary>
        /// Creates a navigator for the indicated controller.
        /// </summary>
        /// <typeparam name="T">The type of controller to navigate to.</typeparam>
        /// <param name="controller">The <seealso cref="IController"/> instance.</param>
        public static ActionNavigator<T> NavigationFor<T>(this IController controller) where T : IController
        {
            return new ActionNavigator<T>();
        }

        #endregion

        #region Utilities

        private static HtmlHelper Html<T>(this ViewNavigator<T> navigator) where T : IController
        {
            var html = ((IAdapter<HtmlHelper>)navigator).Item;
            return html;
        }

        private static HtmlHelper Html<T>(this ActionRenderer<T> renderer) where T : IController
        {
            var html = ((IAdapter<HtmlHelper>)renderer).Item;
            return html;
        }

        #endregion
    }
}