using System;

namespace AccurateAppend.ListBuilder.Models
{
    [Serializable()]
    public class Address
    {
        public string StreetAddress { get; set; }
        public string HouseNumber { get; set; }
        public string StreetName { get; set; }
        public City City { get; set; }
        public State State { get; set; }
        public County County { get; set; }
        public Zip Zip { get; set; }

        public static Address Create(string streetAddress, string HouseNumber, string streetName, string city, string state, string county, string zip, int fips)
        {
            var _State = State.Create(state, fips);

            return new Address()
            {
                StreetAddress = streetAddress,
                HouseNumber = HouseNumber,
                StreetName = streetName,
                City = City.Create(city, _State),
                State = _State,
                County = County.Create(county, fips, _State),
                Zip = Zip.Create(zip, _State),
            };
        }
    }
}
