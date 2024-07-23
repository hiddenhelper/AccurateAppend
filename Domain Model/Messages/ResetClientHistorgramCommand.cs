using System;
using NServiceBus;

namespace DomainModel.Messages
{
    /// <summary>
    /// Bus command to build a <see cref="AccurateAppend.Accounting.Client"/> state histogram.
    /// </summary>
    [Serializable()]
    public class ResetClientHistorgramCommand : ICommand
    {
        /// <summary>
        /// The identifier of the <see cref="AccurateAppend.Security.User"/> to reprocess a histogram for.
        /// </summary>
        public Guid UserId { get; set; }
    }
}