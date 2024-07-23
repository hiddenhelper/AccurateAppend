using System;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.Data;

namespace DomainModel.Queries
{
    /// <summary>
    /// Standard default implementation of <see cref="ILeadsViewQuery"/>.
    /// </summary>
    public class LeadsViewQuery : ILeadsViewQuery
    {
        #region Fields

        private readonly ISessionContext context;

        #endregion

        #region Constructor

        public LeadsViewQuery(ISessionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion
        
        #region ILeadsViewQuery Members

        /// <summary>
        /// Crafts a queryable for <see cref="ReadModel.LeadView"/> entities that have been active during the indicated time frame.
        /// </summary>
        public virtual IQueryable<ReadModel.LeadView> ActiveDuring(Guid applicationId, DateTime startOn, DateTime endBy)
        {
            var query = this.context
                .SetOf<ReadModel.LeadView>()
                .Where(l => l.LastUpdate >= startOn && l.DateAdded <= endBy)
                .Where(l => l.ApplicationId == applicationId);

            return query.AsNoTracking();
        }

        /// <summary>
        /// Performs a fuzzy leads search using the indicated term.
        /// </summary>
        /// <param name="term">The value to search lead information with.</param>
        public virtual IQueryable<ReadModel.LeadView> Search(String term)
        {
            var query = this.context.SetOf<ReadModel.LeadView>()
                .Where(l => l.FirstName.Contains(term)
                            || l.LastName.Contains(term)
                            || l.Email.Contains(term)
                            || l.Phone.Contains(term)
                            || l.BusinessName.Contains(term));

            return query.AsNoTracking();
        }

        #endregion
    }
}
