using System;

namespace AccurateAppend.ListBuilder.Models
{
    [Serializable()]
    public class Homeowner
    {
        // O OR R
        public String IndicatorValue { get; set; }
        public override string ToString()
        {
            return $"{this.IndicatorValue}";
        }
    }
}
