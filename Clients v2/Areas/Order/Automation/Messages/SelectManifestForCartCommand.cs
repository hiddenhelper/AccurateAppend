using System;
using System.Xml.Linq;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Automation.Messages
{
    /// <summary>
    /// Command to instruct a <see cref="Sales.Cart"/> to be updated with the specified Manifest XML.
    /// </summary>
    [Serializable()]
    public class SelectManifestForCartCommand : ICommand
    {
        /// <summary>
        /// Gets or sets the identifier of the cart to select the manifest for.
        /// </summary>
        public Guid CartId { get; set; }

        /// <summary>
        /// The Manifest XML content that has been selected for the order. This manifest is expected
        /// to be fully populated with metadata attributes (@UserId, @ManifestId, etc)
        /// </summary>
        public XElement Manifest { get; set; }
    }
}