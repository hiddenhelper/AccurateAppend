using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Data;
using AccurateAppend.Security;

namespace DomainModel.Commands
{
    /// <summary>
    /// Default implementation of the <see cref="IAcknowledgeAlertCommand"/> command.
    /// </summary>
    public class AcknowledgeAlertCommand : IAcknowledgeAlertCommand
    {
        #region Fields

        private readonly ISessionContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AcknowledgeAlertCommand"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> used to provide data access.</param>
        public AcknowledgeAlertCommand(ISessionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        /// <summary>
        /// Marks the indicated <see cref="UserAlert"/> as <see cref="AlertStatus">acknowledged</see> by a user.
        /// </summary>
        /// <param name="alertId">The identifier of the <see cref="UserAlert"/> to acknowledge.</param>
        /// <param name="acknowledgedBy">The identifier of the <see cref="User"/> that performed the acknowledgement (this may not the <see cref="UserAlert.Subject"/> of the alert).</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> used to signal that an operation should be canceled.</param>
        public virtual async Task Acknowledge(Int32 alertId, Guid acknowledgedBy, CancellationToken cancellation)
        {
            using (var uow = this.context.CreateScope(ScopeOptions.AutoCommit))
            {
                var user = await this.context.SetOf<User>().Where(u => u.Id == acknowledgedBy).FirstOrDefaultAsync(cancellation).ConfigureAwait(false);
                if (user == null) throw new InvalidOperationException($"User {acknowledgedBy} does not exist");

                var alert = await this.context.SetOf<UserAlert>()
                    .Where(a => a.Subject.Id == user.Id)
                    .Where(a => a.Id == alertId)
                    .Where(a => a.Status == AlertStatus.Acknowledged)
                    .FirstOrDefaultAsync(cancellation);

                if (alert == null) return;

                alert.Acknowledge(user);

                await uow.CommitAsync();
            }
        }
    }
}
