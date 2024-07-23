using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.ReadModel.Queries;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Admin.Areas.Sales.DealNotes
{
    /// <summary>
    /// Controller managing <see cref="Audit">Notes</see> for a <see cref="DealBinder"/>.
    /// </summary>
    [Authorize()]
    public class DealNotesController : Controller
    {
        #region Fields

        private readonly IDealNotesQuery dal;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DealNotesController"/> class.
        /// </summary>
        /// <param name="dal">The <see cref="IDealNotesQuery"/> DAL component.</param>
        public DealNotesController(IDealNotesQuery dal)
        {
            if (dal == null) throw new ArgumentNullException(nameof(dal));
            Contract.EndContractBlock();

            this.dal = dal;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Retrieves notes for a given deal by identifier.
        /// </summary>
        /// <param name="dealId">The identifier of the deal to return audit notes for.</param>
        /// <param name="cancellation">The token to monitor for cancellation requests.</param>
        public virtual async Task<ActionResult> Index(Int32 dealId, CancellationToken cancellation)
        {
            var query = this.dal.ForDeal(dealId);

            var notes = await query
                .Select(n =>
                    new
                    {
                        n.Content,
                        n.CreatedBy,
                        n.CreatedDate,
                        Id = n.NoteId
                    })
                .OrderByDescending(n => n.CreatedDate)
                .ToArrayAsync(cancellation);

            var data = notes.Select(n =>
                            new
                            {
                                n.Content,
                                n.CreatedBy,
                                CreatedDate = n.CreatedDate.ToUserLocal(),
                                n.Id
                            });

            var jsonNetResult = new JsonNetResult(DateTimeKind.Local)
            {
                Data = data
            };
            return jsonNetResult;
        }

        #endregion
    }
}