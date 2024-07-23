using System;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Box.Messages
{
    /// <summary>
    /// Command to instruct the system to download a file via Box.com integration.
    /// </summary>
    [Serializable()]
    public class TransferBoxFileCommand : ICommand
    {
        /// <summary>
        /// The identifier of the cart the file to download is for.
        /// </summary>
        public Guid PublicKey { get; set; }

        /// <summary>
        /// The Box.com identifier of the file to download.
        /// </summary>
        public Int64 NodeId { get; set; }
    }
}