using System;

namespace AccurateAppend.Websites.Admin.Entities
{
    [Serializable()]
    public class BatchJobRequestField
    {
        public string Title { get; set; }
        public string Key { get; set; }
        public string Direction { get; set; }
        public int OrderBy { get; set; }
    }
}
