using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.ListBuilder.Models;

namespace AccurateAppend.ListBuilder.DataSources.ConsumerProfile
{
    //[Obsolete("Use the DAL listbuilder", true)]
    [ContractClass(typeof(IDataAccessContracts))]
    public interface IDataAccess
    {
        /// <summary>
        /// Returns records for a specific list criteria in common Record format.
        /// </summary>
        /// <param name="criteria">The criteria for the list to build.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> used to single a request to cancel an asynchronous operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with the records for the supplied <paramref name="criteria"/>.</returns>
        Task<IEnumerable<Record>> GetRecordsAsync(ListCriteria criteria, CancellationToken cancellation);

        /// <summary>
        /// Returns a count of records for a specific list criteria.
        /// </summary>
        /// <param name="criteria">The criteria for the list to build.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> used to single a request to cancel an asynchronous operation.</param>
        /// <returns>A count of the matching records for the supplied <paramref name="criteria"/>.</returns>
        Task<Int32> GetCountAsync(ListCriteria criteria, CancellationToken cancellation);
    }

    [ContractClassFor(typeof(IDataAccess))]
    internal abstract class IDataAccessContracts : IDataAccess
    {
        Task<IEnumerable<Record>> IDataAccess.GetRecordsAsync(ListCriteria criteria, CancellationToken cancellation)
        {
            Contract.Requires<ArgumentNullException>(criteria != null, nameof(criteria));

            return default(Task<IEnumerable<Record>>);
        }
        
        Task<Int32> IDataAccess.GetCountAsync(ListCriteria criteria, CancellationToken cancellation)
        {
            Contract.Requires<ArgumentNullException>(criteria != null, nameof(criteria));

            return default(Task<Int32>);
        }
    }
}