using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.JobProcessing;
using AccurateAppend.JobProcessing.DataAccess;

namespace DomainModel.Commands
{
    public class ChangePriorityCommand : IChangePriorityCommand
    {
        #region Fields

        private readonly DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangePriorityCommand"/> class.
        /// </summary>
        /// <param name="context">Provides data access to the command instance.</param>
        public ChangePriorityCommand(DefaultContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region IChangePriorityCommand Members

        /// <summary>
        /// Changes the specified <paramref name="jobId"/> to the indicated <paramref name="priority"/>.
        /// </summary>
        /// <param name="jobId">The identifier of the job to be changed.</param>
        /// <param name="priority">The <see cref="Priority"/> to change to.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> used to signal that an operation should be canceled.</param>
        public virtual async Task Change(Int32 jobId, Priority priority, CancellationToken cancellation)
        {
            // Even though we're keeping domain logic in the entity,
            // there's a bunch of additional work that is easier handled in
            // this sproc, even though it's redundant somewhat.
            const String Sql = @"[jobs].[ChangeJobPriority] @JobId=@P0, @Priority=@P1";
            await this.context.Database.ExecuteSqlCommandAsync(Sql, cancellation, jobId, (Int32) priority);
        }

        #endregion
    }
}
