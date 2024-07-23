using System.Collections.Generic;

namespace AccurateAppend.ListBuilder
{
    public static class StatesHelper
    {
        /// <summary>
        /// Returns all US states, including Fips" , Abbreviation and Description
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<State> GetStates()
        {
            return new List<State>
            {
                new State{Fips = 1 , Abbreviation = "AL", StateFullName = "Alabama" },
                new State{Fips = 2 , Abbreviation = "AK", StateFullName = "Alaska" },
                new State{Fips = 4 , Abbreviation = "AZ", StateFullName = "Arizona" },
                new State{Fips = 5 , Abbreviation = "AR", StateFullName = "Arkansas" },
                new State{Fips = 6 , Abbreviation = "CA", StateFullName = "California" },
                new State{Fips = 8 , Abbreviation = "CO", StateFullName = "Colorado" },
                new State{Fips = 9 , Abbreviation = "CT", StateFullName = "Connecticut" },
                new State{Fips = 10 , Abbreviation = "DE", StateFullName = "Deleware" },
                new State{Fips = 11 , Abbreviation = "DC", StateFullName = "Washington DC" },
                new State{Fips = 12 , Abbreviation = "FL", StateFullName = "Florida" },
                new State{Fips = 13 , Abbreviation = "GA", StateFullName = "Georgia" },
                new State{Fips = 15 , Abbreviation = "HI", StateFullName = "Hawaii" },
                new State{Fips = 16 , Abbreviation = "ID", StateFullName = "Idaho" },
                new State{Fips = 17 , Abbreviation = "IL", StateFullName = "Illinois" },
                new State{Fips = 18 , Abbreviation = "IN", StateFullName = "Indiana" },
                new State{Fips = 19 , Abbreviation = "IA", StateFullName = "Iowa" },
                new State{Fips = 20 , Abbreviation = "KS", StateFullName = "Kansas" },
                new State{Fips = 21 , Abbreviation = "KY", StateFullName = "Kentucky" },
                new State{Fips = 22 , Abbreviation = "LA", StateFullName = "Louisiana" },
                new State{Fips = 23 , Abbreviation = "ME", StateFullName = "Maine" },
                new State{Fips = 24 , Abbreviation = "MD", StateFullName = "Maryland" },
                new State{Fips = 25 , Abbreviation = "MA", StateFullName = "Massachusetts" },
                new State{Fips = 26 , Abbreviation = "MI", StateFullName = "Michigan" },
                new State{Fips = 27 , Abbreviation = "MN", StateFullName = "Minnesota" },
                new State{Fips = 28 , Abbreviation = "MS", StateFullName = "Mississippi" },
                new State{Fips = 29 , Abbreviation = "MO", StateFullName = "Missouri" },
                new State{Fips = 30 , Abbreviation = "MT", StateFullName = "Montana" },
                new State{Fips = 31 , Abbreviation = "NE", StateFullName = "Nebraska" },
                new State{Fips = 32 , Abbreviation = "NV", StateFullName = "Nevada" },
                new State{Fips = 33 , Abbreviation = "NH", StateFullName = "New Hampshire" },
                new State{Fips = 34 , Abbreviation = "NJ", StateFullName = "New Jersey" },
                new State{Fips = 35 , Abbreviation = "NM", StateFullName = "New Mexico" },
                new State{Fips = 36 , Abbreviation = "NY", StateFullName = "New York" },
                new State{Fips = 37 , Abbreviation = "NC", StateFullName = "North Carolina" },
                new State{Fips = 38 , Abbreviation = "ND", StateFullName = "North Dakota" },
                new State{Fips = 39 , Abbreviation = "OH", StateFullName = "Ohio" },
                new State{Fips = 40 , Abbreviation = "OK", StateFullName = "Oaklahoma" },
                new State{Fips = 41 , Abbreviation = "OR", StateFullName = "Oregon" },
                new State{Fips = 42 , Abbreviation = "PA", StateFullName = "Pennsylvania" },
                new State{Fips = 44 , Abbreviation = "RI", StateFullName = "Rhode Island" },
                new State{Fips = 45 , Abbreviation = "SC", StateFullName = "South Carolina" },
                new State{Fips = 46 , Abbreviation = "SD", StateFullName = "South Dakota" },
                new State{Fips = 47 , Abbreviation = "TN", StateFullName = "Tennessee" },
                new State{Fips = 48 , Abbreviation = "TX", StateFullName = "Texas" },
                new State{Fips = 49 , Abbreviation = "UT", StateFullName = "Utah" },
                new State{Fips = 50 , Abbreviation = "VT", StateFullName = "Vermont" },
                new State{Fips = 51 , Abbreviation = "VA", StateFullName = "Virginia" },
                new State{Fips = 53 , Abbreviation = "WA", StateFullName = "Washington" },
                new State{Fips = 54 , Abbreviation = "WV", StateFullName = "West Virginia" },
                new State{Fips = 55 , Abbreviation = "WI", StateFullName = "Wisconsin" },
                new State{Fips = 56 , Abbreviation = "WY", StateFullName = "Wyoming" },
                new State{Fips = 60 , Abbreviation = "AS", StateFullName = "American Somoa" },
                new State{Fips = 64 , Abbreviation = "FM", StateFullName = "Micronesia" },
                new State{Fips = 66 , Abbreviation = "GU", StateFullName = "Guam" },
                new State{Fips = 68 , Abbreviation = "MH", StateFullName = "Marshall Islands" },
                new State{Fips = 69 , Abbreviation = "MP", StateFullName = "Northern Mariana Islands" },
                new State{Fips = 70 , Abbreviation = "PW", StateFullName = "Palau" },
                new State{Fips = 72 , Abbreviation = "PR", StateFullName = "Puerto Rico" },
                new State{Fips = 78 , Abbreviation = "VI", StateFullName = "U.S. Virgin Islands" },
            };
        }
    }

    public class State {
        // TODO: String is used because other methods that use zip code data search on 2 digit string that contains int
        public int Fips { get; set; }
        public string Abbreviation { get; set; }
        public string StateFullName { get; set; }
    }
}