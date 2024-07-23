using System;

namespace AccurateAppend.ListBuilder.Models
{
    [Serializable()]
    public class Dob
    {
        public string Year { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }

        public override string ToString()
        {
            return $"{this.Year},{this.Month},{this.Day}";
        }

        public static Dob Create(string date)
        {
            String yyyy = "";
            String mm = "";
            String dd = "";

            if (date.Trim().Length == 8)
            {
                yyyy = date.Substring(0, 4); 
                mm = date.Substring(4, 2);
                dd = date.Substring(6, 2);
            }

            return Create(yyyy, mm, dd);
        }

        public static Dob Create(string Year, string Month, string Day)
        {
            return new Dob()
            {
                Year = Year,
                Month = Month,
                Day = Day,
            };
        }
    }
}
