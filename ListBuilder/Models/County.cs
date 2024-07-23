using System;

namespace AccurateAppend.ListBuilder.Models
{
    [Serializable()]
    public class County
    {
        public string Name { get; set; }
        public int Fips { get; set; }
        public State State { get; set; }

        public static County Create(string name, int fips, State state)
        {
            return new County()
            {
                Name = name,
                Fips = fips,
                State = state,
            };
        }
    }
}