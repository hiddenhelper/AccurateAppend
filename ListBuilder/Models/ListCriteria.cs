using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.ListBuilder.Models
{
    [Serializable()]
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "()}")]
    public class ListCriteria
    {
        #region Constructor

        public ListCriteria()
        {
            this.OnDeserializingMethod(default(StreamingContext));
        }

        #endregion

        #region Properties

        [OnDeserializing()]
        private void OnDeserializingMethod(StreamingContext context)
        {
            this.States = new List<State>();
            this.Zips = new List<Zip>();
            this.Counties = new List<County>();
            this.Cities = new List<City>();
            this.TimeZones = new List<ValueLabel>();

            this.AgeRanges = new List<ValueLabel>();
            this.ExactAges = new List<ValueLabel>();
            this.DobRanges = new List<DobRange>();
            this.Gender = new List<ValueLabel>();
            this.MaritalStatus = new List<ValueLabel>();
            this.Languages = new List<ValueLabel>();
            this.Hoh = new List<ValueLabel>();
            this.LengthOfResidence = new List<ValueLabel>();

            this.EstIncome = new List<ValueLabel>();
            this.NetWorth = new List<ValueLabel>();
            this.HomeValue = new List<ValueLabel>();
            this.OwnRent = new List<ValueLabel>();
            this.InvestmentEstimatedResidentialPropertiesOwned = new List<ValueLabel>();
            this.NumberOfAdults = new List<ValueLabel>();
            this.AgeRangesMale = new List<ValueLabel>();
            this.AgeRangesFemale = new List<ValueLabel>();
            this.AgeRangesUnknown = new List<ValueLabel>();
            this.Investments = new List<ValueLabel>();
            this.BusinessOwner = new List<ValueLabel>();
            this.Education = new List<ValueLabel>();
            this.OccupationGeneral = new List<ValueLabel>();
            this.OccupationDetailed = new List<ValueLabel>();
            this.DonatesTo = new List<ValueLabel>();

            this.InterestsPurchases = new List<ValueLabel>();
            this.InterestsReadingGeneral = new List<ValueLabel>();
            this.InterestsReadingMagazinesAndSubscriptions = new List<ValueLabel>();
            this.InterestsSports = new List<ValueLabel>();
            this.InterestsFitness = new List<ValueLabel>();
            this.InterestsOutdoors = new List<ValueLabel>();
        }

        // ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
        // Deserializers actually need this to be a non-readonly backing field so they can set the value. Using get only auto-properties will create
        // readonly fields preventing values from being set. Believe it or not, non-constructor based deserialization can occur using raw memory layouts
        // (JSON.Net is a great example of this) so constructors actually are not guarenteed to always be called. CrayCray but true and knowing is half the battle.

        public Guid RequestId { get; set; }

        // GEOGRAPHY

        public IList<State> States { get; private set; }

        public IList<Zip> Zips { get; private set; }
        public IList<County> Counties { get; private set; }
        public IList<City> Cities { get; private set; }
        public IList<ValueLabel> TimeZones { get; private set; }

        // DEMOGRAPHICS
        public IList<ValueLabel> AgeRanges { get; private set; }

        public IList<ValueLabel> ExactAges { get; private set; }

        public IList<DobRange> DobRanges { get; private set; }

        public IList<ValueLabel> Gender { get; private set; }

        public IList<ValueLabel> MaritalStatus { get; private set; }

        public IList<ValueLabel> Languages { get; private set; }

        public IList<ValueLabel> Hoh { get; private set; }

        public IList<ValueLabel> LengthOfResidence { get; private set; }
        
        // HOUSING & FINANCE
        public IList<ValueLabel> EstIncome { get; private set; }

        public IList<ValueLabel> NetWorth { get; private set; }

        public IList<ValueLabel> HomeValue { get; private set; }

        public IList<ValueLabel> OwnRent { get; private set; }

        public IList<ValueLabel> InvestmentEstimatedResidentialPropertiesOwned { get; private set; }

        public IList<ValueLabel> NumberOfAdults { get; private set; }

        public IList<ValueLabel> AgeRangesMale { get; private set; }

        public IList<ValueLabel> AgeRangesFemale { get; private set; }

        public IList<ValueLabel> AgeRangesUnknown { get; private set; }

        public IList<ValueLabel> Investments { get; private set; }

        public IList<ValueLabel> BusinessOwner { get; private set; }

        public IList<ValueLabel> Education { get; private set; }

        public IList<ValueLabel> OccupationGeneral { get; private set; }

        public IList<ValueLabel> OccupationDetailed { get; private set; }

        public IList<ValueLabel> DonatesTo { get; private set; }

        // INTERESTS
        public IList<ValueLabel> InterestsPurchases { get; private set; }

        public IList<ValueLabel> InterestsReadingGeneral { get; private set; }

        public IList<ValueLabel> InterestsReadingMagazinesAndSubscriptions { get; private set; }

        public IList<ValueLabel> InterestsSports { get; private set; }

        public IList<ValueLabel> InterestsFitness { get; private set; }

        public IList<ValueLabel> InterestsOutdoors { get; private set; }

        // ReSharper restore AutoPropertyCanBeMadeGetOnly.Local

        #endregion

        #region Methods

        // needs to be consolidated with IsEmpty()
        public virtual IEnumerable<ValidationResult> Validate()
        {
            // at least one geographical element needs to be chosen
            if (!this.States.Any() && !this.Zips.Any() && !this.Counties.Any() && ! this.Cities.Any())
            {
                // Business requirement
                yield return new ValidationResult("At least one geographical element must be chosen to generate a count.");
            }
        }

        private String DebuggerDisplay()
        {
            var restrictions = new List<string>();

            if (this.States.Any()) restrictions.Add(nameof(States) + ": " + String.Join(",", this.States.Select(s => s.Abbreviation)));
            if (this.Zips.Any()) restrictions.Add(nameof(Zips) + ": " + String.Join(",", this.Zips.Select(s => s.ToString())));
            if (this.Counties.Any()) restrictions.Add(nameof(Counties) + ": " + String.Join(",", this.Counties.Select(s => s.ToString())));
            if (this.Cities.Any()) restrictions.Add(nameof(Cities) + ": " + String.Join(",", this.Cities.Select(s => s.ToString())));
            if (this.TimeZones.Any()) restrictions.Add(nameof(TimeZones) + ": " + String.Join(",", this.TimeZones.Select(s => s.ToString())));
            if (this.DobRanges.Any()) restrictions.Add(nameof(DobRanges) + ": " + String.Join(",", this.DobRanges.Select(s => s.ToString())));
            if (this.Gender.Any()) restrictions.Add(nameof(Gender) + ": " + String.Join(",", this.Gender.Select(s => s.ToString())));
            if (this.MaritalStatus.Any()) restrictions.Add(nameof(MaritalStatus) + ": " + String.Join(",", this.MaritalStatus.Select(s => s.ToString())));
            if (this.Languages.Any()) restrictions.Add(nameof(Languages) + ": " + String.Join(",", this.Languages.Select(s => s.ToString())));
            if (this.Hoh.Any()) restrictions.Add(nameof(Hoh) + ": " + String.Join(",", this.Hoh.Select(s => s.ToString())));
            if (this.EstIncome.Any()) restrictions.Add(nameof(EstIncome) + ": " + String.Join(",", this.EstIncome.Select(s => s.ToString())));
            if (this.NetWorth.Any()) restrictions.Add(nameof(NetWorth) + ": " + String.Join(",", this.NetWorth.Select(s => s.ToString())));
            if (this.HomeValue.Any()) restrictions.Add(nameof(HomeValue) + ": " + String.Join(",", this.HomeValue.Select(s => s.ToString())));
            if (this.OwnRent.Any()) restrictions.Add(nameof(OwnRent) + ": " + String.Join(",", this.OwnRent.Select(s => s.ToString())));
            if (this.InvestmentEstimatedResidentialPropertiesOwned.Any()) restrictions.Add(nameof(InvestmentEstimatedResidentialPropertiesOwned) + ": " + String.Join(",", this.InvestmentEstimatedResidentialPropertiesOwned.Select(s => s.ToString())));
            if (this.BusinessOwner.Any()) restrictions.Add(nameof(BusinessOwner) + ": " + String.Join(",", this.BusinessOwner.Select(s => s.ToString())));
            if (this.LengthOfResidence.Any()) restrictions.Add(nameof(LengthOfResidence) + ": " + String.Join(",", this.LengthOfResidence.Select(s => s.ToString())));
            if (this.NumberOfAdults.Any()) restrictions.Add(nameof(NumberOfAdults) + ": " + String.Join(",", this.NumberOfAdults.Select(s => s.ToString())));
            if (this.Education.Any()) restrictions.Add(nameof(Education) + ": " + String.Join(",", this.Education.Select(s => s.ToString())));
            if (this.OccupationGeneral.Any()) restrictions.Add(nameof(OccupationGeneral) + ": " + String.Join(",", this.OccupationGeneral.Select(s => s.ToString())));
            if (this.OccupationDetailed.Any()) restrictions.Add(nameof(OccupationDetailed) + ": " + String.Join(",", this.OccupationDetailed.Select(s => s.ToString())));
            if (this.ExactAges.Any()) restrictions.Add(nameof(ExactAges) + ": " + String.Join(",", this.ExactAges.Select(s => s.ToString())));
            if (this.AgeRanges.Any()) restrictions.Add(nameof(AgeRanges) + ": " + String.Join(",", this.AgeRanges.Select(s => s.ToString())));
            if (this.AgeRangesMale.Any()) restrictions.Add(nameof(AgeRangesMale) + ": " + String.Join(",", this.AgeRangesMale.Select(s => s.ToString())));
            if (this.AgeRangesFemale.Any()) restrictions.Add(nameof(AgeRangesFemale) + ": " + String.Join(",", this.AgeRangesFemale.Select(s => s.ToString())));
            if (this.AgeRangesUnknown.Any()) restrictions.Add(nameof(AgeRangesUnknown) + ": " + String.Join(",", this.AgeRangesUnknown.Select(s => s.ToString())));
            if (this.Investments.Any()) restrictions.Add(nameof(Investments) + ": " + String.Join(",", this.Investments.Select(s => s.ToString())));
            if (this.DonatesTo.Any()) restrictions.Add(nameof(DonatesTo) + ": " + String.Join(",", this.DonatesTo.Select(s => s.ToString())));
            if (this.InterestsPurchases.Any()) restrictions.Add(nameof(InterestsPurchases) + ": " + String.Join(",", this.InterestsPurchases.Select(s => s.ToString())));
            if (this.InterestsReadingGeneral.Any()) restrictions.Add(nameof(InterestsReadingGeneral) + ": " + String.Join(",", this.InterestsReadingGeneral.Select(s => s.ToString())));
            if (this.InterestsReadingMagazinesAndSubscriptions.Any()) restrictions.Add(nameof(InterestsReadingMagazinesAndSubscriptions) + ": " + String.Join(",", this.InterestsReadingMagazinesAndSubscriptions.Select(s => s.ToString())));
            if (this.InterestsSports.Any()) restrictions.Add(nameof(InterestsSports) + ": " + String.Join(",", this.InterestsSports.Select(s => s.ToString())));
            if (this.InterestsFitness.Any()) restrictions.Add(nameof(InterestsFitness) + ": " + String.Join(",", this.InterestsFitness.Select(s => s.ToString())));
            if (this.InterestsOutdoors.Any()) restrictions.Add(nameof(InterestsOutdoors) + ": " + String.Join(",", this.InterestsOutdoors.Select(s => s.ToString())));

            var description = String.Join(". ", restrictions);

            return description;
        }

        /// <summary>
        /// Converts ListBuilder.Models.ListCriteria to DAL.DataAccess.ListBuilder.ListCriteria
        /// </summary>
        /// <remarks>
        /// This also handles business logic such as date ranges.
        /// </remarks>
        /// <param name="frontEndListCriteria">ListBuilder.Models.ListCriteria in front end format to be converted</param>
        /// <returns>ListBuilder in format accepted by DAL</returns>
        public static DAL.DataAccess.ListBuilder.ListCriteria ConvertListCriteria(ListCriteria frontEndListCriteria)
        {            
            var output = new DAL.DataAccess.ListBuilder.ListCriteria()
            {                
                { FieldName.State, frontEndListCriteria.States.Select(state => state.Abbreviation).ToArray() }, // the .ToArray()s are necessary because .net params array only accepts arrays, not IEnumerbales.
                { FieldName.PostalCode, frontEndListCriteria.Zips.Select(zip => zip.Name).ToArray() },
                { FieldName.County, frontEndListCriteria.Counties.Select(county => new DAL.DataAccess.ListBuilder.ListCriteria.County(county.Name, county.State.Abbreviation).ToString()).ToArray() },
                { FieldName.City, frontEndListCriteria.Cities.Select(city => new DAL.DataAccess.ListBuilder.ListCriteria.City(city.Name, city.State.Abbreviation).ToString()).ToArray() },
                { FieldName.TimeZoneCode, frontEndListCriteria.TimeZones.Select(timeZone => timeZone.Name.ToString()).ToArray() },

                // ageRanges => added below
                // exactAges => added below
                { FieldName.DateOfBirth, frontEndListCriteria.DobRanges.SelectMany(dobRange => dobRange.ToRanges()).ToArray() },
                { FieldName.Gender, frontEndListCriteria.Gender.Select(gender => gender.Name.ToString()).ToArray() },
                { FieldName.MaritalStatus, frontEndListCriteria.MaritalStatus.Select(marital => marital.Name.ToString()).ToArray() },
                { FieldName.PersonLanguage, frontEndListCriteria.Languages.Select(language => language.Name.ToString()).ToArray() },
                { FieldName.HeadOfHousehold, frontEndListCriteria.Hoh.Select(hoh => hoh.Name.ToString()).ToArray() },
                
                { FieldName.EstIncome, frontEndListCriteria.EstIncome.Select(estIncome => estIncome.Name.ToString()).ToArray() },
                { FieldName.EstWealth, frontEndListCriteria.NetWorth.Select(netWorth => netWorth.Name.ToString()).ToArray() },
                { FieldName.MedHomeValue, frontEndListCriteria.HomeValue.Select(homeValue => homeValue.Name.ToString()).ToArray() },
                { FieldName.HomeOwner, frontEndListCriteria.OwnRent.Select(homeowner => homeowner.Name.ToString()).ToArray() },
                { FieldName.InvestmentEstimatedResidentialPropertiesOwned, frontEndListCriteria.InvestmentEstimatedResidentialPropertiesOwned.Select(n => n.Name.ToString()).ToArray() },
                { FieldName.BusinessOwner, frontEndListCriteria.BusinessOwner.Select(n => n.Name.ToString()).ToArray() },
                { FieldName.LengthOfResidence, frontEndListCriteria.LengthOfResidence.Select(lor => lor.ToString()).ToArray() },
                // householdSize -- this field does not exist in the database, 
                { FieldName.HouseHoldNumberOfAdults, frontEndListCriteria.NumberOfAdults.Select(n => n.Name.ToString()).ToArray() },
                // investments => below
                { FieldName.MedYearsInSchool, frontEndListCriteria.Education.Select(education => education.Name.ToString()).ToArray() },  
                { FieldName.PersonOccupation, frontEndListCriteria.OccupationGeneral.Select(occupation => occupation.Name.ToString()).ToArray() },
                { FieldName.OccupationDetailed, frontEndListCriteria.OccupationDetailed.Select(occupationDetailed => occupationDetailed.Name.ToString()).ToArray() }
                // donates => added below

                // interests => added below
            };
                  
            // Exact ages
            if (frontEndListCriteria.ExactAges.Any())
            {
                foreach (var exactAge in frontEndListCriteria.ExactAges)
                {
                    var dobRange = new DobRange
                    {
                        Start = CalculateDobFromAge(int.Parse(exactAge.Name)),
                        End = CalculateDobFromAge(int.Parse(exactAge.Name) - 1)
                    };
                    output.Add(FieldName.DateOfBirth, $"{YYYYMM(dobRange.Start)}-{YYYYMM(dobRange.End)}");
                }
            }

            // Age ranges
            if (frontEndListCriteria.AgeRanges.Any())
            {
                foreach (var ageRange in frontEndListCriteria.AgeRanges)
                {
                    var start = new Dob();
                    var end = new Dob();
                    switch (ageRange.Name)
                    {
                        case "A":
                            start = CalculateDobFromAge(24);
                            end = CalculateDobFromAge(18);
                            break;
                        case "B":
                            start = CalculateDobFromAge(34);
                            end = CalculateDobFromAge(25);
                            break;
                        case "C":
                            start = CalculateDobFromAge(44);
                            end = CalculateDobFromAge(35);
                            break;
                        case "D":
                            start = CalculateDobFromAge(54);
                            end = CalculateDobFromAge(45);
                            break;
                        case "E":
                            start = CalculateDobFromAge(64);
                            end = CalculateDobFromAge(55);
                            break;
                        case "F":
                            start = CalculateDobFromAge(74);
                            end = CalculateDobFromAge(65);
                            break;
                        case "G":
                            start = CalculateDobFromAge(120);
                            end = CalculateDobFromAge(18);
                            break;
                    }
                    output.Add( FieldName.DateOfBirth, $"{YYYYMM(start)}-{YYYYMM(end)}" );
                }
            }

            // Adult Age Ranges in Household
            frontEndListCriteria.AgeRangesMale.ForEach(i => output.Add((FieldName)Enum.Parse(typeof(FieldName), char.ToUpper(i.Name[0]) + i.Name.Substring(1)), "T")); 
            frontEndListCriteria.AgeRangesFemale.ForEach(i => output.Add((FieldName)Enum.Parse(typeof(FieldName), char.ToUpper(i.Name[0]) + i.Name.Substring(1)), "T")); 
            frontEndListCriteria.AgeRangesUnknown.ForEach(i => output.Add((FieldName)Enum.Parse(typeof(FieldName), char.ToUpper(i.Name[0]) + i.Name.Substring(1)), "T")); 
            // Investments
            frontEndListCriteria.Investments.ForEach(i => output.Add((FieldName)Enum.Parse(typeof(FieldName), char.ToUpper(i.Name[0]) + i.Name.Substring(1)), "T")); // javascript const start with lower case so need to Upper case field name
            // Donates
            frontEndListCriteria.DonatesTo.ForEach(i => output.Add((FieldName)Enum.Parse(typeof(FieldName), char.ToUpper(i.Name[0]) + i.Name.Substring(1)), "T")); // javascript const start with lower case so need to Upper case field name
            // Interests
            frontEndListCriteria.InterestsPurchases.ForEach(i => output.Add((FieldName)Enum.Parse(typeof(FieldName), char.ToUpper(i.Name[0]) + i.Name.Substring(1)), "T")); // javascript const start with lower case so need to Upper case field name
            frontEndListCriteria.InterestsReadingGeneral.ForEach(i => output.Add((FieldName)Enum.Parse(typeof(FieldName), char.ToUpper(i.Name[0]) + i.Name.Substring(1)), "T")); // javascript const start with lower case so need to Upper case field name
            frontEndListCriteria.InterestsReadingMagazinesAndSubscriptions.ForEach(i => output.Add((FieldName)Enum.Parse(typeof(FieldName), char.ToUpper(i.Name[0]) + i.Name.Substring(1)), "T")); // javascript const start with lower case so need to Upper case field name
            frontEndListCriteria.InterestsSports.ForEach(i => output.Add((FieldName)Enum.Parse(typeof(FieldName), char.ToUpper(i.Name[0]) + i.Name.Substring(1)), "T")); // javascript const start with lower case so need to Upper case field name
            frontEndListCriteria.InterestsFitness.ForEach(i => output.Add((FieldName)Enum.Parse(typeof(FieldName), char.ToUpper(i.Name[0]) + i.Name.Substring(1)), "T")); // javascript const start with lower case so need to Upper case field name
            frontEndListCriteria.InterestsOutdoors.ForEach(i => output.Add((FieldName)Enum.Parse(typeof(FieldName), char.ToUpper(i.Name[0]) + i.Name.Substring(1)), "T")); // javascript const start with lower case so need to Upper case field name
          
            return output;
        }

        // ReSharper disable InconsistentNaming
        private static string YYYYMM(Dob date)
        {
            return $"{date.Year}{int.Parse(date.Month):D2}00";
        }

        // ReSharper restore InconsistentNaming

        private static Dob CalculateDobFromAge(int age)
        {
            var dob = DateTime.UtcNow.AddYears(-age);
            // if someone was 18 then they were born 18 years before today utc
            return Dob.Create(dob.Year.ToString(), dob.Month.ToString(), "00");
        }

        #endregion
    }
}