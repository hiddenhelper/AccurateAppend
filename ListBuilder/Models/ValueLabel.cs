using System;

namespace AccurateAppend.ListBuilder.Models
{
    [Serializable()]
    public class ValueLabel
    {
        public string Name { get; set; }
        public string Label { get; set; }

        public override string ToString()
        {
            return this.Name;
        }

        public static ValueLabel Create(string name, string label)
        {
            return new ValueLabel()
            {
                Name = name,                
                Label = label
            };
        }
    }
}
