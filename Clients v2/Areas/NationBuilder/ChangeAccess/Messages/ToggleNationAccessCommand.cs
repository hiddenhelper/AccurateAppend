using System;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.ChangeAccess.Messages
{
    /// <summary>
    /// Instructs the NationBuilder integration to toggle integration access on or off.
    /// </summary>
    /// <remarks>
    /// This command is an internal operation to the Clients application only.
    /// </remarks>
    [Serializable()]
    public class ToggleNationAccessCommand : ICommand
    {
        /// <summary>
        /// The identifier of the Nation to toggle integration access for.
        /// </summary>
        public Int32 NationId { get; set; }

        /// <summary>
        /// Indicates whether the Nation should be enabled or disabled.
        /// </summary>
        public Boolean Enable { get; set; }

        /// <summary>
        /// Gets the unique request identifier for the command.
        /// </summary>
        public Guid RequestId { get; set; }
    }
}