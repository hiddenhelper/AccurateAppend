using System;

namespace AccurateAppend.ListBuilder.Models
{
    [Serializable()]
    public class Zip
    {
        public string Name { get; set; }
        public State State { get; set; }

        public override string ToString()
        {
            return this.Name;
        }

        public static Zip Create(string name, State state)
        {
            return new Zip()
            {
                Name = name,                
                State = state,
            };
        }
    }
}
