using System;
using System.Linq;

namespace AccurateAppend.Sales.ReadModel.Queries
{
    /// <summary>
    /// A query component that can provides access to <see cref="DealView"/> queries.
    /// </summary>
    public interface IDealsViewByIdQuery
    {
        /// <summary>
        /// Crafts a queryable for a single <see cref="DealView"/> entity by identifier.
        /// </summary>
        /// <remarks>
        /// The returned queryable can be further customized but will always return 0 or 1 instances.
        /// </remarks>
        IQueryable<DealView> ForId(Int32 id);

        /// <summary>
        /// Crafts a queryable for a single <see cref="DealView"/> entity by public key.
        /// </summary>
        /// <remarks>
        /// The returned queryable can be further customized but will always return 0 or 1 instances.
        /// </remarks>
        IQueryable<DealView> ForId(Guid publicKey);
    }
}