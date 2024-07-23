using System;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Public.LeadsApi.Messages
{
    /// <summary>
    /// Command to sync an AA lead to ZenSales.
    /// </summary>
    [Serializable()]
    public class SyncToZenSalesCommand : ICommand
    {
        /// <summary>
        /// The identifier of the lead to sync.
        /// </summary>
        public Int32 LeadId { get; set; }
    }
}