using System;

namespace AccurateAppend.ListBuilder.Models
{
    [Serializable()]
    public class Hoh
    {
        public int Rank { get; set; }
        public override string ToString()
        {
            return $"{this.Rank}";
        }
    }
}
