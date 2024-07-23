using System;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.ListBuilder.Messaging
{
    [Serializable()]
    public class CartCreatedEvent : IEvent
    {
        public Guid CartId { get; set; }

        public Guid ForUser { get; set; }
    }
}