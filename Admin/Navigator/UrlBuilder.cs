using System;
using System.Diagnostics.Contracts;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Stub object used for extension methods performing Action calls with the navigator system from views or controllers useing the <see cref="UrlHelper"/> component.
    /// </summary>
    /// <typeparam name="T">The <see cref="IController"/> type to perform Action calls against.</typeparam>
    public class UrlBuilder<T> : IAdapter<UrlHelper> where T : IController
    {
        #region Fields

        private readonly UrlHelper url;

        #endregion

        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <seealso cref="UrlBuilder{T}"/> class.
        /// </summary>
        /// <param name="url">The <see cref="UrlHelper"/> instance to proxy.</param>
        public UrlBuilder(UrlHelper url)
        {
            if (url == null) throw new ArgumentNullException("url");
            Contract.EndContractBlock();

            this.url = url;
        }

        #endregion

        #region IAdapter Members

        UrlHelper IAdapter<UrlHelper>.Item
        {
            get { return this.url; }
        }

        #endregion
    }
}