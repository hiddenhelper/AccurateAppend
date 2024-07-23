using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DomainModel.Html
{
    /// <summary>
    /// Html Helper for North American territories.
    /// </summary>
    public static class NorthAmericanTerritories
    {
        /// <summary>
        /// Holds the standard US State/CA Territory value and display text used in the HTML UI.
        /// </summary>
        private static readonly IDictionary<String, String> StateDictionary = new Dictionary<String, String> {
                {"-Select----", ""},
                {"Alabama", "AL"},
                {"Alaska", "AK"},
                {"American Samoa", "AS"},
                {"Arizona", "AZ"},
                {"Arkansas", "AR"},
                {"California", "CA"},
                {"Colorado", "CO"},
                {"Connecticut", "CT"},
                {"Delaware", "DE"},
                {"District of Columbia", "DC"},
                {"Federated States of Micronesia", "FM"},
                {"Florida", "FL"},
                {"Georgia", "GA"},
                {"Guam", "GU"},
                {"Hawaii", "HI"},
                {"Idaho", "ID"},
                {"Illinois", "IL"},
                {"Indiana", "IN"},
                {"Iowa", "IA"},
                {"Kansas", "KS"},
                {"Kentucky", "KY"},
                {"Louisiana", "LA"},
                {"Maine", "ME"},
                {"Marshall Islands", "MH"},
                {"Maryland", "MD"},
                {"Massachusetts", "MA"},
                {"Michigan", "MI"},
                {"Minnesota", "MN"},
                {"Mississippi", "MS"},
                {"Missouri", "MO"},
                {"Montana", "MT"},
                {"Nebraska", "NE"},
                {"Nevada", "NV"},
                {"New Hampshire", "NH"},
                {"New Jersey", "NJ"},
                {"New Mexico", "NM"},
                {"New York", "NY"},
                {"North Carolina", "NC"},
                {"North Dakota", "ND"},
                {"Northern Mariana Islands", "MP"},
                {"Ohio", "OH"},
                {"Oklahoma", "OK"},
                {"Oregon", "OR"},
                {"Palau", "PW"},
                {"Pennsylvania", "PA"},
                {"Puerto Rico", "PR"},
                {"Rhode Island", "RI"},
                {"South Carolina", "SC"},
                {"South Dakota", "SD"},
                {"Tennessee", "TN"},
                {"Texas", "TX"},
                {"Utah", "UT"},
                {"Vermont", "VT"},
                {"Virgin Islands", "VI"},
                {"Virginia", "VA"},
                {"Washington", "WA"},
                {"West Virginia", "WV"},
                {"Wisconsin", "WI"},
                {"Wyoming", "WY"},
                {"Alberta","AB"}, 	
                {"British Columbia","BC"},
                {"Manitoba","MB"},
                {"New Brunswick","NB"} 	,
                {"Newfoundland","NL"},
                {"Nova Scotia","NS"},	
                {"Northwest Territories","NT"},
                {"Nunavut","NU"},
                {"Ontario","ON"},	
                {"Prince Edward Island","PE"},
                {"Quebec","QC"},
                {"Saskatchewan","SK"},
                {"Yukon","YT"}
        };

        /// <summary>
        /// Created a <see cref="SelectList"/> used in the UI for territory selections.
        /// </summary>
        public static SelectList StateSelectList()
        {
            var list = new SelectList(StateDictionary, "Value", "Key");

            return list;

        }

        /// <summary>
        /// Created a <see cref="SelectList"/> used in the UI for territory selections preselected for the indicated <paramref name="territory"/>.
        /// </summary>
        /// <param name="territory">The value of the territory that should be preselected in the UI (if possible)</param>
        public static SelectList StateSelectList(String territory)
        {
            var list = new SelectList(StateDictionary, "Value", "Key", territory);

            return list;
        }
    }
}