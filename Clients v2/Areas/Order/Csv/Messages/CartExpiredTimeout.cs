using System;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Csv.Messages
{
    /// <summary>
    /// Message used to indicate a <see cref="CsvSalesSaga"/> has expired.
    /// </summary>
    [Serializable()]
    public class CartExpiredTimeout : IMessage
    {
    }
}