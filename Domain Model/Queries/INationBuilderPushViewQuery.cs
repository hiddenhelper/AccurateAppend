using System;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.Data;
using DomainModel.ReadModel;
using Integration.NationBuilder.Data;

namespace DomainModel.Queries
{
    /// <summary>
    /// A query component that can provides access to <see cref="NationBuilderPushView"/> queries.
    /// </summary>
    public interface INationBuilderPushViewQuery
    {
        /// <summary>
        /// Crafts a queryable for <see cref="NationBuilderPushView"/> entities that have been submitted during the indicated time frame.
        /// </summary>
        IQueryable<NationBuilderPushView> SubmittedDuring(DateTime startOn, DateTime endBy, params PushStatus[] statuses);
    }


    /// <summary>
    /// Standard default implementation of <see cref="INationBuilderPushViewQuery"/>.
    /// </summary>
    public class NationBuilderPushViewQuery : INationBuilderPushViewQuery
    {
        #region Fields

        private readonly ISessionContext context;

        #endregion

        #region Constructor

        public NationBuilderPushViewQuery(ISessionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region INationBuilderPushViewQuery Members

        /// <summary>
        /// Crafts a queryable for <see cref="NationBuilderPushView"/> entities that have been submitted during the indicated time frame.
        /// </summary>
        public IQueryable<NationBuilderPushView> SubmittedDuring(DateTime startOn, DateTime endBy, params PushStatus[] statuses)
        {
            var query = this.context.SetOf<NationBuilderPushView>().Where(p => p.RequestDate >= startOn && p.RequestDate < endBy);
            if (statuses != null && statuses.Length > 0)
            {
                query = query.Where(p => statuses.Contains(p.Status));
            }

            return query;
        }

        #endregion
    }
}
