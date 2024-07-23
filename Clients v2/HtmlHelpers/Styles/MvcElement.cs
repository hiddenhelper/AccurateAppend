using System;
using System.Diagnostics;
using System.Web.Mvc;
using AccurateAppend.Core.ComponentModel;

namespace AccurateAppend.Websites.Clients.HtmlHelpers.Styles
{
    /// <summary>
    /// Base type used by the layout HTML helper components to reduce the duplicate behavior.
    /// </summary>
    public abstract class MvcElement : DisposeableObject
    {
        #region Fields

        private readonly HtmlHelper htmlHelper;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MvcElement"/> class.
        /// </summary>
        /// <param name="htmlHelper">The <see cref="HtmlHelper"/> used to interact with the view output.</param>
        internal MvcElement(HtmlHelper htmlHelper)
        {
            Debug.Assert(htmlHelper != null, nameof(htmlHelper));

            this.htmlHelper = htmlHelper;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the configured <see cref="TagBuilder"/> used by this component for rendering HTML content.
        /// </summary>
        protected abstract TagBuilder Tag { get; }

        /// <summary>
        /// Gets the configured <see cref="HtmlHelper"/> used by this component for rendering HTML content.
        /// </summary>
        protected HtmlHelper Helper => htmlHelper;

        #endregion

        #region Methods

        /// <summary>
        /// Opens the HTML content that this component produces.
        /// </summary>
        internal virtual void Open()
        {
            var writer = this.htmlHelper.ViewContext.Writer;

            writer.WriteLine(this.Tag.ToString(TagRenderMode.StartTag));
        }

        /// <summary>
        /// Closes the HTML content that this component produces.
        /// </summary>
        internal virtual void Close()
        {
            var writer = this.htmlHelper.ViewContext.Writer;

            writer.WriteLine(this.Tag.ToString(TagRenderMode.EndTag));
        }

        /// <inheritdoc />
        /// <remarks>
        /// When this component is disposed the <see cref="Close"/> method is automatically closed.
        /// </remarks>
        protected override void Dispose(Boolean isDisposing)
        {
            if (isDisposing) this.Close();

            base.Dispose(isDisposing);
        }

        #endregion
    }
}