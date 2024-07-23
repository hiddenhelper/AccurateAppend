using System;
using System.Collections.Generic;

namespace AccurateAppend.ListBuilder.Models
{
    [Serializable()]
    public class DobRange
    {
        public Dob Start { get; set; }
        public Dob End { get; set; }
        public override string ToString()
        {
            return $"{this.Start.Year}{int.Parse(this.Start.Month):D2}00-{this.End.Year}{int.Parse(this.End.Month):D2}{DateTime.DaysInMonth(int.Parse(this.End.Year), int.Parse(this.End.Month)):D2}";
        }

        public IEnumerable<string> ToRanges()
        {
            // Dates in the DB are stored with trailing zeros to indicate a wildcard. For example a DOB of sometime in 1954 is stored as "19540000".
            // Dates in the front end are coming in like this: "1953-12" to "1954-01". This is causing the 19540000 dates to show up in ranges, because "19540000" is between "19531130" and "19540131".

            // Here is fix that would always be correct:
            //
            // Dates can exist at three levels of precision: 
            // 1954-1955
            // 195402-195403
            // 19540214-19540219
            //
            // in the case of precision to a year, like "1954-1956" then no special logic, let the dates ending in 0000 are allowed.
            // in the case of precision to a month or tighter, "19560000" is not okay. Represent the range as multiple months, none of which allow "00".
            // in the case of precision to exact day, every day must be output.
            //
            // but that's too much work.

            // so, the 80/20 fix (which will work as long as the front just deals in yyyy-mm) is to output individual ranges for each month:
            try
            {
                // this turns "1953-11" to "1954-01" into three ranges: "1953-11-01 to 1953-11-30" + "1953-12-01 to 1953-12-31" + "1954-01-01 to 1954-1-31" 

                int begin_yyyy = Int32.Parse(this.Start.Year);
                int begin_mm = Int32.Parse(this.Start.Month)-1; // the -1 adjusts the month range from 1-12 to 0-11, which is necessary for the math to work.

                int end_yyyy = Int32.Parse(this.End.Year);
                int end_mm = Int32.Parse(this.End.Month)-1;

                int begin = begin_yyyy * 12 + begin_mm; // months since jan 0000
                int end = end_yyyy * 12 + end_mm;

                var output = new List<string>();
                for (int pos=begin;pos<=end; pos++)
                {
                    // using this as a model:
                    //output.Add($"{this.Start.Year}{int.Parse(this.Start.Month):D2}00-{this.End.Year}{int.Parse(this.End.Month):D2}{DateTime.DaysInMonth(int.Parse(this.End.Year), int.Parse(this.End.Month)):D2}";

                    int year = pos / 12;
                    int month = (pos % 12)+1; // adjust back from 0-11 to 1-12

                    output.Add($"{year:D2}{month:D2}00-{year:D2}{month:D2}{DateTime.DaysInMonth(year, month):D2}"); // range starts at yyyymm00 rathre than yyyymm01 because steve m. is okay with ambiguous days.
                }

                return output;
            }
            catch (ArgumentException)
            {
                return new string[0];
            }            
        }
    }
}
