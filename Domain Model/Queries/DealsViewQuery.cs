using System;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.Core;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.ReadModel;
using AccurateAppend.Sales.ReadModel.Queries;

namespace DomainModel.Queries
{
    /// <summary>
    /// Implementation of the <see cref="IDealsViewActiveDuringQuery"/> and <see cref="IDealsViewByIdQuery"/> components.
    /// </summary>
    [Obsolete("Don't directly use", true)]
    public class DealsViewQuery : IDealsViewActiveDuringQuery, IDealsViewByIdQuery, IDealNotesQuery
    {
        #region Fields

        private readonly ISessionContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DealsViewQuery"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> component providing data access.</param>
        public DealsViewQuery(ISessionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region IDealsViewActiveDuringQuery Members

        /// <inheritdoc />
        public virtual IQueryable<DealView> ActiveDuring(DateTime startOn, DateTime endBy)
        {
            startOn = startOn.Coerce();
            endBy = endBy.Coerce();

            var query = this.context.SetOf<DealView>()
                .Where(d => d.Status != DealStatus.Canceled)
                .Where(d => d.CreatedDate >= startOn && d.CreatedDate <= endBy);

            return query;
        }

        #endregion

        #region IDealsViewByIdQuery Members

        /// <inheritdoc />
        public virtual IQueryable<DealView> ForId(Int32 id)
        {
            var query = this.context.SetOf<DealView>().Where(d => d.DealId == id).AsNoTracking();

            return query;
        }

        /// <inheritdoc />
        public virtual IQueryable<DealView> ForId(Guid publicKey)
        {
            var query = this.context.SetOf<DealView>().Where(d => d.PublicKey == publicKey).AsNoTracking();

            return query;
        }

        #endregion

        #region IDealNotesQuery Members

        /// <inheritdoc />
        public virtual IQueryable<DealNoteView> ForDeal(Int32 dealId)
        {
            var query = this.context
                .SetOf<DealNoteView>()
                .Where(d => d.DealId == dealId)
                .AsNoTracking();

            return query;
        }

        #endregion
    }
}