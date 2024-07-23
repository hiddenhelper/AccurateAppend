using System;
using NServiceBus;

namespace AccurateAppend.Sales.Contracts.Messages
{
    /// <summary>
    /// Command to resume a usage based service account that lacks saga data.
    /// Nominally just used while we sort out the kinks in the new automation
    /// perhaps we may keep it around to reopen sagas when a usage bill
    /// is canceled/expired.
    /// </summary>
    [Serializable()]
    public class ResumeUsageCommand : ICommand
    {
        /// <summary>
        /// Gets or sets the alternate identifier of the the subscription to resume.
        /// </summary>
        public Guid PublicKey { get; set; }
    }
}