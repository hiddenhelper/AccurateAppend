using System;

namespace AccurateAppend.Websites.Clients.Areas.ListBuilder.Order.Models
{
    [Serializable()]
    public class GenerateListModel
    {
        public Guid PublicKey { get; set; }

        public String NextUrl { get; set; }

        public String CheckUrl { get; set; }
    }
}