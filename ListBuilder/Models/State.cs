using System;

namespace AccurateAppend.ListBuilder.Models
{
    [Serializable()]
    public class State
    {
        public string Abbreviation { get; set; }
        public string StateFullName { get; set; }
        public int Fips { get; set; }

        public static State Create(string abbreviation, int fips)
        {
            return new State()
            {
                Abbreviation = abbreviation,
                Fips = fips,
            };
        }
    }
}