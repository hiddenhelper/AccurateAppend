using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace DomainModel.Queries
{
    /// <summary>
    /// Default implementation of the <see cref="IUserOperatingMetricQuery"/> component.
    /// </summary>
    public class UserOperatingMetricQuery : IUserOperatingMetricQuery
    {
        #region Fields

        private readonly DbContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UserOperatingMetricQuery"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> providing access to the data store.</param>
        public UserOperatingMetricQuery(DbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region UserOperatingMetricQuery Members

        async Task<IEnumerable<UserOperatingMetricRecord>> IUserOperatingMetricQuery.Query(Guid userid, CancellationToken cancellation)
        {
            const String Sql = "EXEC [sales].[CalculateUserOperatingMetrics] @UserId=@p0";

            return await this.context.Database.SqlQuery<UserOperatingMetricRecord>(Sql, userid)
                    .ToArrayAsync(cancellation)
                    .ConfigureAwait(false);
        }
      
        #endregion
    }
}