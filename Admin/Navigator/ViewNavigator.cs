using System;
using System.Diagnostics.Contracts;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Stub object used for extension methods performing RedirectToAction calls with the navigator system from views.
    /// </summary>
    /// <typeparam name="T">The <see cref="IController"/> type to perform RedirectToAction calls against.</typeparam>
    public sealed class ViewNavigator<T> : IAdapter<HtmlHelper> where T : IController
    {
        #region Fields
        
        private readonly HtmlHelper html;

        #endregion

        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <seealso cref="ViewNavigator{T}"/> class.
        /// </summary>
        /// <param name="html">The <see cref="HtmlHelper"/> instance to proxy.</param>
        public ViewNavigator(HtmlHelper html)
        {
            if (html == null) throw new ArgumentNullException("html");
            Contract.EndContractBlock();

            this.html = html;
        }

        #endregion

        #region IAdapter Members

        HtmlHelper IAdapter<HtmlHelper>.Item
        {
            get { return this.html; }
        }

        #endregion
    }
}