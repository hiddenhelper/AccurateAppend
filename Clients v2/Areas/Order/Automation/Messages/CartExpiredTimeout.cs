using System;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Automation.Messages
{
    /// <summary>
    /// Message used to indicate a <see cref="AutomationSalesSaga"/> has expired.
    /// </summary>
    [Serializable()]
    public class CartExpiredTimeout : IMessage
    {
    }
}