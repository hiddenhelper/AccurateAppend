using System;
using System.Threading;
using System.Threading.Tasks;

namespace DomainModel.Commands
{
    /// <summary>
    /// Represents a command capable of marking alerts as <see cref="AccurateAppend.Security.AlertStatus">acknowledged</see> by a user.
    /// </summary>
    public interface IAcknowledgeAlertCommand
    {
        /// <summary>
        /// Marks the indicated <see cref="AccurateAppend.Security.UserAlert"/> as <see cref="AccurateAppend.Security.AlertStatus">acknowledged</see> by a user.
        /// </summary>
        /// <param name="alertId">The identifier of the <see cref="AccurateAppend.Security.UserAlert"/> to acknowledge.</param>
        /// <param name="acknowledgedBy">The identifier of the <see cref="AccurateAppend.Security.User"/> that performed the acknowledgement (this may not the <see cref="AccurateAppend.Security.UserAlert.Subject"/> of the alert).</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> used to signal that an operation should be canceled.</param>
        Task Acknowledge(Int32 alertId, Guid acknowledgedBy, CancellationToken cancellation);
    }
}