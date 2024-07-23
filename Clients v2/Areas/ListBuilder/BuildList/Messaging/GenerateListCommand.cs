using AccurateAppend.ListBuilder.Models;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.ListBuilder.BuildList.Messaging
{
    /// <summary>
    /// Command portion of a dialog pair to generate a list from the provided criteria.
    /// </summary>
    /// <remarks>
    /// This contract is designed as a NServiceBus wrapper for the <see cref="ListCriteria"/> type. This prevents the need to
    /// have bus specific interfaces corrupting the List Builder assembly types.
    /// </remarks>
    public class GenerateListCommand : ICommand
    {
        /// <summary>
        /// Gets or sets the <see cref="ListCriteria"/> that indicate the desired list to be generated.
        /// </summary>
        public ListCriteria Criteria { get; set; }
    }
}