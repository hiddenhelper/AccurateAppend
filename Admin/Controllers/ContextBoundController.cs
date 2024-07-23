using System;
using System.Diagnostics.Contracts;
using System.Web.Mvc;
using AccurateAppend.Data;

namespace AccurateAppend.Websites.Admin.Controllers
{
    /// <summary>
    /// Base type for all controllers that leverage an <see cref="ISessionContext"/> system.
    /// </summary>
    public abstract class ContextBoundController : Controller
    {
        #region Fields
        
        private readonly ISessionContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextBoundController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        protected ContextBoundController(ISessionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current <see cref="ISessionContext"/> to use for the controller.
        /// </summary>
        /// <value>The current <see cref="ISessionContext"/> to use for the controller.</value>
        protected ISessionContext Context
        {
            get
            {
                Contract.Ensures(Contract.Result<ISessionContext>() != null);

                return this.context;
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(Boolean disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;
            this.context.Dispose();
        }

        #endregion
    }
}
