using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace DomainModel.Html
{
    /// <summary>
    /// Html Helper for credit card expiration dates.
    /// </summary>
    public static class CreditCardExpiration
    {
        /// <summary>
        /// Holds the standard month value and display text used in the HTML UI.
        /// </summary>
        private static readonly IDictionary<String, String> MonthDictionary = new Dictionary<String, String> {
                {"-Month----", ""},
                {$"{CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(1)} (01)", "01"},
                {$"{CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(2)} (02)", "02"},
                {$"{CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(3)} (03)", "03"},
                {$"{CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(4)} (04)", "04"},
                {$"{CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(5)} (05)", "05"},
                {$"{CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(6)} (06)", "06"},
                {$"{CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(7)} (07)", "07"},
                {$"{CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(8)} (08)", "08"},
                {$"{CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(9)} (09)", "09"},
                {$"{CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(10)} (10)", "10"},
                {$"{CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(11)} (11)", "11"},
                {$"{CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(12)} (12)", "12"},
        };

        /// <summary>
        /// Gets the <see cref="SelectList"/> used in the UI for expiration months.
        /// </summary>
        public static SelectList MonthSelectList => new SelectList(MonthDictionary, "Value", "Key");

        /// <summary>
        /// Gets the <see cref="SelectList"/> used in the UI for expiration years.
        /// </summary>
        public static SelectList YearSelectList
        {
            get
            {
                
                var yearDictionary = new Dictionary<string, string>();
                yearDictionary.Add("-Year----", "");

                foreach (var year in Enumerable.Range(DateTime.Now.Year, 10).Select(i => i.ToString()))
                {
                    yearDictionary.Add(year, year);
                }
                return new SelectList(yearDictionary, "Value", "Key");
            }
        }
    }
}