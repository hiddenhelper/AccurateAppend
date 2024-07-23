using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AccurateAppend.Data;
using AccurateAppend.Operations.DataAccess;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Admin.Areas.Operations.ViewEmail
{
    /// <summary>
    /// Controller for viewing a tracked email content.
    /// </summary>
    [Authorize()]
    public class ViewEmailController : Controller
    {
        #region Fields

        private readonly DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewEmailController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> providing data access.</param>
        public ViewEmailController(DefaultContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            this.context = context;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Clears Poison status for a message.
        /// </summary>
        public virtual async Task<ActionResult> Index(Int32 id, CancellationToken cancellation)
        {
            try
            {
                var message = await this.context
                    .Set<AccurateAppend.Operations.Message>()
                    .FirstOrDefaultAsync(m => m.Id == id, cancellation);
                if (message == null) return new LiteralResult(true) {Data = "No email found"};

                if (message.IsHtml) return this.Content(message.Body, MediaTypeNames.Text.Html);

                return new LiteralResult(true) {Data = HttpUtility.HtmlEncode(message.Body)};
            }
            catch (Exception ex)
            {
                return this.DisplayErrorResult(ex.Message);
            }
        }

        #endregion
    }
}