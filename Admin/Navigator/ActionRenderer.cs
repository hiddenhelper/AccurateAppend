using System;
using System.Diagnostics.Contracts;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Stub object used for extension methods performing RenderAction calls with the navigator system from controllers.
    /// </summary>
    /// <typeparam name="T">The <see cref="IController"/> type to perform RenderAction calls against.</typeparam>
    public sealed class ActionRenderer<T> : IAdapter<HtmlHelper> where T : IController
    {
        #region Fields
        
        private readonly HtmlHelper html;

        #endregion

        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionRenderer{T}"/> class.
        /// </summary>
        /// <param name="html">The <see cref="HtmlHelper"/> instance to proxy.</param>
        public ActionRenderer(HtmlHelper html)
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