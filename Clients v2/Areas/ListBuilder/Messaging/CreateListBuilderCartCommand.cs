using System;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.ListBuilder.Messaging
{
    [Serializable()]
    public class CreateListBuilderCartCommand : ICommand
    {
        public Guid UserId { get; set; }

        public Guid CartId { get; set; }
    }
}