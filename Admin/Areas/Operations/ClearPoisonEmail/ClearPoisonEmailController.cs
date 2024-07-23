using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Data;
using AccurateAppend.Operations.DataAccess;

namespace AccurateAppend.Websites.Admin.Areas.Operations.ClearPoisonEmail
{
    /// <summary>
    /// Controller for marking a tracked email as sent.
    /// </summary>
    [Authorize()]
    public class ClearPoisonEmailController : Controller
    {
        #region Fields

        private readonly DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ClearPoisonEmailController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> providing data access.</param>
        public ClearPoisonEmailController(DefaultContext context)
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
            var message = await this.context
                .Set<AccurateAppend.Operations.Message>()
                .FirstOrDefaultAsync(m => m.Id == id, cancellation);

                message?.MarkSent();

            await this.context.SaveChangesAsync(cancellation);

            return this.RedirectToAction("Index", "Message", new {Area = "Operations", id});
        }

        #endregion
    }
}