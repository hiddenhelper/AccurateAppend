using System;

namespace AccurateAppend.Websites.Admin.Entities
{
    [Serializable()]
    public class BatchJobRequestProduct
    {
        public decimal CostPerRecord { get; set; }
        public decimal CostPerMatch { get; set; }
        public int Quantity { get; set; }
        public Guid UserId { get; set; }
        public BatchJobRequestField[] Fields { get; set; }    // could not use an array of Field because Field is a Linq Entity class
        public string Title { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
    }
}
