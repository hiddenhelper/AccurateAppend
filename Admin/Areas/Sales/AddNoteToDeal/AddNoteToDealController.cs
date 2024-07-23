using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Admin.Areas.Sales.AddNoteToDeal
{
    /// <summary>
    /// Controller responsible for appending a new immutable note to a <see cref="DealBinder"/> model.
    /// </summary>
    [Authorize()]
    public class AddNoteToDealController : Controller
    {
        #region Fields

        private readonly DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AddNoteToDealController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> DAL component.</param>
        public AddNoteToDealController(DefaultContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Saves note for a given Deal.
        /// </summary>
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual async Task<ActionResult> Index(String body, Int32 dealId, CancellationToken cancellation)
        {
            body = (body ?? String.Empty).Trim().Left(4000);
            if (body.Length == 0) return new JsonResult();

            var deal = await this.context
                .SetOf<DealBinder>()
                .SingleOrDefaultAsync(d => d.Id == dealId, cancellation);
            if (deal == null) return new JsonNetResult() {Data = new {Sucess = false, Message = "Deal does not exist"}};

            deal.Notes.Add(body);

            await this.context.SaveChangesAsync(cancellation);

            return new JsonNetResult() {Data = new {Sucess = true}};
        }

        #endregion
    }
}