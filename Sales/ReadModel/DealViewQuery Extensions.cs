#pragma warning disable SA1649 // File name must match first type name
using System;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.Core.IdentityModel;

namespace AccurateAppend.Sales.ReadModel
{
    /// <summary>
    /// Contains common intention based queryable extensions for the <see cref="DealView"/> readmodel type.
    /// </summary>
    public static class DealViewQueryExtensions
    {
        /// <summary>
        /// Extends the provided query to filter by an application identifier.
        /// </summary>
        /// <remarks>
        /// The Admin application is always included in this filter.
        /// </remarks>
        /// <param name="query">The source query to filter.</param>
        /// <param name="applicationId">The identifier of the application.</param>
        /// <returns>A new queryable that can be realized or further specified.</returns>
        public static IQueryable<DealView> ForApplication(this IQueryable<DealView> query, Guid applicationId)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            Contract.EndContractBlock();

            return query.Where(d => d.ApplicationId == applicationId || d.ApplicationId == WellKnownIdentifiers.AdminId);
        }

        /// <summary>
        /// Extends the provided query to filter by a set of statuses.
        /// </summary>
        /// <param name="query">The source query to filter.</param>
        /// <param name="userId">The statuses to filter in.</param>
        /// <returns>A new queryable that can be realized or further specified.</returns>
        public static IQueryable<DealView> InStatus(this IQueryable<DealView> query, params DealStatus[] userId)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            Contract.EndContractBlock();

            userId = userId ?? new DealStatus[0];

            return query.Where(d => userId.Contains(d.Status));
        }

        /// <summary>
        /// Extends the provided query to filter to a particular user account.
        /// </summary>
        /// <param name="query">The source query to filter.</param>
        /// <param name="userId">The identifier of the user to filter by.</param>
        /// <returns>A new queryable that can be realized or further specified.</returns>
        public static IQueryable<DealView> ForUser(this IQueryable<DealView> query, Guid userId)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            Contract.EndContractBlock();

            return query.Where(d => d.UserId == userId);
        }

        /// <summary>
        /// Extends the provided query to filter to a particular user account.
        /// </summary>
        /// <param name="query">The source query to filter.</param>
        /// <param name="userName">The username of the user to filter by.</param>
        /// <returns>A new queryable that can be realized or further specified.</returns>
        public static IQueryable<DealView> ForUser(this IQueryable<DealView> query, String userName)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            Contract.EndContractBlock();

            return query.Where(d => d.UserName == userName);
        }
    }
}
