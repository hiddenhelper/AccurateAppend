using System;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Box.Messages
{
    /// <remarks>
    /// <example>
    ///  Using a specific command type to create an event
    /// <code>
    /// <![CDATA[
    /// var @event = new BoxTransferCompletedEvent(originatingCommand)
    /// {
    ///    SystemFileName = systemFileName,
    ///    CustomerFileName = customerFileName
    /// };
    ///
    /// // These properties are automatically set
    /// Debug.Assert(@event.PublicKey = originatingCommand.PublicKey);
    /// Debug.Assert(@event.NodeId = originatingCommand.NodeId);
    /// ]]>
    /// </code>
    /// </example>
    /// </remarks>
    [Serializable()]
    public class BoxTransferCompletedEvent:IEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxTransferCompletedEvent"/> class.
        /// </summary>
        public BoxTransferCompletedEvent()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxTransferCompletedEvent"/> class.
        /// </summary>
        /// <param name="originatingCommand">The originating command to correlated this event instance with.</param>
        public BoxTransferCompletedEvent(TransferBoxFileCommand originatingCommand)
        {
            if (originatingCommand == null) throw new ArgumentNullException(nameof(originatingCommand));

            this.PublicKey = originatingCommand.PublicKey;
            this.NodeId = originatingCommand.NodeId;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the public key of the box registration the file was transferred for.
        /// </summary>
        public Guid PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the Box.com identifier of the downloaded file.
        /// </summary>
        public Int64 NodeId { get; set; }

        /// <summary>
        /// The temp downloaded file name.
        /// </summary>
        public String SystemFileName { get; set; }

        /// <summary>
        /// The original uploaded file name.
        /// </summary>
        public String CustomerFileName { get; set; }

        #endregion 

    }
}