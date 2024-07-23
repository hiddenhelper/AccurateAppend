using System;

namespace AccurateAppend.ListBuilder.Models
{
    [Serializable()]
    public class City
    {
        public string Name { get; set; }
        public State State { get; set; }

        public static City Create(string name, State state)
        {
            return new City()
            {
                Name = name,
                State = state,
            };
        }
    }
}
