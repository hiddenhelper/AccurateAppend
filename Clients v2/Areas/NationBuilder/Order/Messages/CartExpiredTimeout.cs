using System;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.Order.Messages
{
    /// <summary>
    /// Message used to indicate a <see cref="NationBuilderSalesSaga"/> has expired.
    /// </summary>
    [Serializable()]
    public class CartExpiredTimeout : IMessage
    {
    }
}