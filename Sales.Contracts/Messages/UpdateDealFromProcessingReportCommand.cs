using System;
using System.Xml.Linq;
using NServiceBus;

namespace AccurateAppend.Sales.Contracts.Messages
{
    /// <summary>
    /// Command to update a deal from a processing report.
    /// </summary>
    [Serializable()]
    public class UpdateDealFromProcessingReportCommand : ICommand
    {
        /// <summary>
        /// Gets or sets the XML processing report to update the deal from.
        /// </summary>
        public XElement ProcessingReport { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the deal to update.
        /// </summary>
        public Int32 DealId { get; set; }

        /// <summary>
        /// Gets or sets the public key identifier that the deal should now be associated with.
        /// </summary>
        public Guid PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the XML manifest definition to update the deal from. This MUST contain
        /// the JobId attribute in order to build notes.
        /// </summary>
        public XElement Manifest { get; set; }
    }
}