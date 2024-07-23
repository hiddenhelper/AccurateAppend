declare let listCriteriaViewModel: AccurateAppend.ListBuilder.ListCriteriaViewModel;

module AccurateAppend.ListBuilder {
   
    export class ListCriteriaViewModel {

        requestId: KnockoutObservable<string>;
        // ESTIMATED LIST RECORD COUNT
        count: KnockoutObservable<number>;
      
        // GEOGRAPHY
        states: KnockoutObservableArray<State>;
        counties: KnockoutObservableArray<County>;
        zips: KnockoutObservableArray<Zip>;
        cities: KnockoutObservableArray<City>;
        timeZones: KnockoutObservableArray<ListBuilder.ValueLabel>;
        
        // DEMOGRAPHICS
        ageRanges: KnockoutObservableArray<ListBuilder.ValueLabel>;
        dobRanges: KnockoutObservableArray<DobRange>; 
        exactAges: KnockoutObservableArray<ListBuilder.ValueLabel>;
        gender: KnockoutObservableArray<ListBuilder.ValueLabel>;
        maritalStatus: KnockoutObservableArray<ListBuilder.ValueLabel>;
        hoh: KnockoutObservableArray<ListBuilder.ValueLabel>;
        languages: KnockoutObservableArray<ListBuilder.ValueLabel>;

        // HOUSING & FINANCE
        netWorth: KnockoutObservableArray<ListBuilder.ValueLabel>;
        estIncome: KnockoutObservableArray<ListBuilder.ValueLabel>;
        homeValue: KnockoutObservableArray<ListBuilder.ValueLabel>;
        ownRent: KnockoutObservableArray<ListBuilder.ValueLabel>;
        investmentEstimatedResidentialPropertiesOwned: KnockoutObservableArray<ListBuilder.ValueLabel>;
        lengthOfResidence: KnockoutObservableArray<ListBuilder.ValueLabel>;
        householdSize: KnockoutObservableArray<ListBuilder.ValueLabel>;
        numberOfAdults: KnockoutObservableArray<ListBuilder.ValueLabel>;
        ageRangesMale: KnockoutObservableArray<ListBuilder.ValueLabel>;
        ageRangesFemale: KnockoutObservableArray<ListBuilder.ValueLabel>;
        ageRangesUnknown: KnockoutObservableArray<ListBuilder.ValueLabel>;
        investments: KnockoutObservableArray<ListBuilder.ValueLabel>;
        businessOwner: KnockoutObservableArray<ListBuilder.ValueLabel>;
        education: KnockoutObservableArray<ListBuilder.ValueLabel>;
        occupationGeneral: KnockoutObservableArray<ListBuilder.ValueLabel>;
        occupationDetailed: KnockoutObservableArray<ListBuilder.ValueLabel>;
        donates: KnockoutObservableArray<ListBuilder.ValueLabel>;
        
        // INTERESTS
        interestsPurchases: KnockoutObservableArray<ListBuilder.ValueLabel>;
        interestsReadingGeneral: KnockoutObservableArray<ListBuilder.ValueLabel>;
        interestsReadingMagazinesAndSubscriptions: KnockoutObservableArray<ListBuilder.ValueLabel>;
        interestsSports: KnockoutObservableArray<ListBuilder.ValueLabel>;
        interestsFitness: KnockoutObservableArray<ListBuilder.ValueLabel>;
        interestsOutdoors: KnockoutObservableArray<ListBuilder.ValueLabel>;

        constructor(public urls: Array<Url>, requestid: string) {
            this.requestId = ko.observable(requestid);
            this.count = ko.observable(0);

            // GEOGRAPHY
            this.states = ko.observableArray([]);
            this.counties = ko.observableArray([]);
            this.zips = ko.observableArray([]);
            this.cities = ko.observableArray([]);
            this.timeZones = ko.observableArray([]);

            // DEMOGRAHICS
            this.ageRanges = ko.observableArray([]);
            this.exactAges = ko.observableArray([]);
            this.dobRanges = ko.observableArray([]);
            this.gender = ko.observableArray([]);
            this.maritalStatus = ko.observableArray([]);
            this.languages = ko.observableArray([]);
            this.hoh = ko.observableArray([]);
                    
            // HOUSING & FINANCE
            this.estIncome = ko.observableArray([]);
            this.netWorth = ko.observableArray([]);
            this.homeValue = ko.observableArray([]);
            this.ownRent = ko.observableArray([]);
            this.investmentEstimatedResidentialPropertiesOwned = ko.observableArray([]);
            this.lengthOfResidence = ko.observableArray([]);
            //this.householdSize = ko.observableArray([]);
            this.numberOfAdults = ko.observableArray([]);
            this.ageRangesMale = ko.observableArray([]);
            this.ageRangesFemale = ko.observableArray([]);
            this.ageRangesUnknown = ko.observableArray([]);
            this.investments = ko.observableArray([]);
            this.businessOwner = ko.observableArray([]);
            this.education = ko.observableArray([]);
            this.occupationGeneral = ko.observableArray([]);
            this.occupationDetailed = ko.observableArray([]);
            this.donates = ko.observableArray([]);

            // INTERESTS
            this.interestsPurchases = ko.observableArray([]);
            this.interestsReadingGeneral = ko.observableArray([]);
            this.interestsReadingMagazinesAndSubscriptions = ko.observableArray([]);
            this.interestsSports = ko.observableArray([]);
            this.interestsFitness = ko.observableArray([]);
            this.interestsOutdoors = ko.observableArray([]);
        }

        //TODO: Need method that accepts Json generated by data() and populates UI
        // maps json to ListCrietria model
        // this method does not quite work
        //  issue is soem elements of the UI are dynamically generated and can't be checked becuase they aren't present
        //  example: to select a city, state must be selected first, item needs to be checked in ui
        //fromJson(json: any) {
        //    const o = JSON.parse(json);
        //    // GEOGRAPHY
        //    // each of these need to be run through toggle() to add them to the ui
        //    // problem is we don't know the id listed in the ui
        //    // items like City, County, Zip require the onclick events on the State dropw down to be run

        //    if (o.States && o.States.length > 0) $.each(o.States, (k, v) => {

        //        this.states.push(new State(v.Abbreviation, v.Fips, v.StateFullName));
        //    });
        //    if (o.Counties && o.Counties.length > 0) $.each(o.Counties, (k, v) => {
        //        this.counties.push(new County(v.Name, v.Fips, new State(v.State.StateFullName, v.State.Fips)));
        //    });
        //    if (o.Zips && o.Zips.length > 0) $.each(o.Zips, (k, v) => {
        //        this.zips.push(new Zip(v.Name, new State(v.State.StateFullName, v.State.Fips)));
        //    });
        //    if (o.Cities && o.Cities.length > 0) $.each(o.Cities, (k, v) => {
        //        // if last then trigger 
        //        this.zips.push(new City(v.Name, new State(v.State.StateFullName, v.State.Fips)));
        //    });
        //    if (o.TimeZones && o.TimeZones.length > 0) $.each(o.TimeZones, (k, v) => {
        //        this.timeZones.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    // DEMOGRAPHICS
        //    if (o.AgeRanges && o.AgeRanges.length > 0) $.each(o.AgeRanges, (k, v) => {
        //        this.ageRanges.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.DobRanges && o.DobRanges.length > 0) $.each(o.DobRanges, (k, v) => {
        //        this.dobRanges.push(new DobRange(new Dob(v.start), new Dob(v.end)));
        //    });
        //    if (o.ExactAges && o.ExactAges.length > 0) $.each(o.ExactAges, (k, v) => {
        //        this.exactAges.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.Gender && o.Gender.length > 0) $.each(o.Gender, (k, v) => {
        //        this.gender.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.MaritalStatus && o.MaritalStatus.length > 0) $.each(o.MaritalStatus, (k, v) => {
        //        this.maritalStatus.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.Hoh && o.Hoh.length > 0) $.each(o.Hoh, (k, v) => {
        //        this.hoh.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.Languages && o.Languages.length > 0) $.each(o.Languages, (k, v) => {
        //        this.languages.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    // HOUSING & FINANCE
        //    if (o.NetWorth && o.NetWorth.length > 0) $.each(o.NetWorth, (k, v) => {
        //        this.netWorth.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.EstIncome && o.EstIncome.length > 0) $.each(o.EstIncome, (k, v) => {
        //        this.estIncome.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.HomeValue && o.HomeValue.length > 0) $.each(o.HomeValue, (k, v) => {
        //        this.homeValue.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.OwnRent && o.OwnRent.length > 0) $.each(o.OwnRent, (k, v) => {
        //        this.ownRent.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.InvestmentEstimatedResidentialPropertiesOwned && o.InvestmentEstimatedResidentialPropertiesOwned.length > 0) $.each(o.InvestmentEstimatedResidentialPropertiesOwned, (k, v) => {
        //        this.investmentEstimatedResidentialPropertiesOwned.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.LengthOfResidence && o.LengthOfResidence.length > 0) $.each(o.LengthOfResidence, (k, v) => {
        //        this.lengthOfResidence.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.HouseholdSize && o.HouseholdSize.length > 0) $.each(o.HouseholdSize, (k, v) => {
        //        this.householdSize.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.NumberOfAdults && o.NumberOfAdults.length > 0) $.each(o.NumberOfAdults, (k, v) => {
        //        this.numberOfAdults.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.AgeRangesMale && o.AgeRangesMale.length > 0) $.each(o.AgeRangesMale, (k, v) => {
        //        this.ageRangesMale.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.AgeRangesFemale && o.AgeRangesFemale.length > 0) $.each(o.AgeRangesFemale, (k, v) => {
        //        this.ageRangesFemale.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.AgeRangesUnknown && o.AgeRangesUnknown.length > 0) $.each(o.AgeRangesUnknown, (k, v) => {
        //        this.ageRangesUnknown.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.Investments && o.Investments.length > 0) $.each(o.Investments, (k, v) => {
        //        this.investments.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.BusinessOwner && o.BusinessOwner.length > 0) $.each(o.BusinessOwner, (k, v) => {
        //        this.businessOwner.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.Education && o.Education.length > 0) $.each(o.Education, (k, v) => {
        //        this.education.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.OccupationGeneral && o.OccupationGeneral.length > 0) $.each(o.OccupationGeneral, (k, v) => {
        //        this.occupationGeneral.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.OccupationDetailed && o.OccupationDetailed.length > 0) $.each(o.OccupationDetailed, (k, v) => {
        //        this.occupationDetailed.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.Donates && o.Donates.length > 0) $.each(o.Donates, (k, v) => {
        //        this.donates.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    // INTERESTS
        //    if (o.InterestsPurchases && o.InterestsPurchases.length > 0) $.each(o.InterestsPurchases, (k, v) => {
        //        this.interestsPurchases.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.InterestsReadingGeneral && o.InterestsReadingGeneral.length > 0) $.each(o.InterestsReadingGeneral, (k, v) => {
        //        this.interestsReadingGeneral.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.InterestsReadingMagazinesAndSubscriptions && o.InterestsReadingMagazinesAndSubscriptions.length > 0) $.each(o.InterestsReadingMagazinesAndSubscriptions, (k, v) => {
        //        this.interestsReadingMagazinesAndSubscriptions.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.InterestsSports && o.InterestsSports.length > 0) $.each(o.InterestsSports, (k, v) => {
        //        this.interestsSports.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.InterestsFitness && o.InterestsFitness.length > 0) $.each(o.InterestsFitness, (k, v) => {
        //        this.interestsFitness.push(new ValueLabel(v.Name, v.Label));
        //    });
        //    if (o.InterestsOutdoors && o.InterestsOutdoors.length > 0) $.each(o.InterestsOutdoors, (k, v) => {
        //        this.interestsOutdoors.push(new ValueLabel(v.Name, v.Label));
        //    });
        //}

        // HELPERS
        // TODO: Json should not send empty properties

        data() {
            return {
                requestId: this.requestId,
                // GEOGRAPHY
                states: this.states,
                counties: this.counties,
                zips: this.zips,
                cities: this.cities,
                timeZones: this.timeZones,

                // DEMOGRAHICS
                ageRanges: this.ageRanges,
                exactAges: this.exactAges,
                dobRanges: this.dobRanges,
                gender: this.gender,
                maritalStatus: this.maritalStatus,
                languages: this.languages,
                hoh: this.hoh,

                // HOUSING & FINANCE
                estIncome: this.estIncome,
                netWorth: this.netWorth,
                homeValue: this.homeValue,
                ownRent: this.ownRent,
                investmentEstimatedResidentialPropertiesOwned: this.investmentEstimatedResidentialPropertiesOwned,
                lengthOfResidence: this.lengthOfResidence,
                //householdSize: this.householdSize, -- not in database
                numberOfAdults: this.numberOfAdults,
                ageRangesMale: this.ageRangesMale,
                ageRangesFemale: this.ageRangesFemale,
                ageRangesUnknown: this.ageRangesUnknown,
                investments: this.investments,
                businessOwner: this.businessOwner,
                education: this.education,
                occupationGeneral: this.occupationGeneral,
                occupationDetailed: this.occupationDetailed,
                donates: this.donates,

                // INTERESTS
                interestsPurchases: this.interestsPurchases,
                interestsReadingGeneral: this.interestsReadingGeneral,
                interestsReadingMagazinesAndSubscriptions: this.interestsReadingMagazinesAndSubscriptions,
                interestsSports: this.interestsSports,
                interestsFitness: this.interestsFitness,
                interestsOutdoors: this.interestsOutdoors
            };
        }

        // adds/removes item from collection
        toggle(e) {
            
            // attribute collection name
            const name = e.currentTarget.name;
            // Collection is which value will be added or removed
            const coll = this[name];

            // parse Json value into obj
            let obj: any;
            switch (e.currentTarget.type) {
                case "select-one":
                    // spacial case needed because target is Select but json is stored in subbordinate Option
                    const value = $(e.currentTarget).find("option:selected").val();
                    coll.removeAll();
                    if (value) { // executed when option value is popualted
                        obj = JSON.parse(value);
                        // remove any existing values
                        coll.removeAll();
                        coll.push(obj);
                    }
                    break;
                default:
                    // parses checkboxes and the buttons used in listCriteriaViewModelDisplay
                    obj = JSON.parse(e.currentTarget.value);
                    // checkboxes only affected by this block
                    if (e.currentTarget.checked) {  // add to collection
                        // add item to appropriate collection
                        coll.push(obj);
                    } else { // buttons will be affected by this block
                        // remove existing item
                        coll.remove(item => item.id === obj.id);
                        // only affects checkbox elements in the UI that are checked
                        $(e.view.document).find(`#${obj.id}`).prop("checked", false);
                        // set value of select back to nothing if one is present
                        $(e.view.document).find(`[name=${name}] option[value=""]`).prop("selected", true);
                    }
                    break;
            }
            
            this.updateCount();
        }

        // TODO: clear collections
        removeAllItems() {
           
            // TODO: uncheck criteria that uses checkboxes  
            // set count = 0
            this.count(0);
        }

        // update list count
        updateCount() {
            console.log("updateCount called");
            // TODO: update list count using ListCriteria
            const self = this;
            console.log(ko.toJSON(this.data()));
            $("#updateCountLoading").show();
            $.ajax(
                {
                    type: "POST",
                    url: this.getUrl("GenerateListCount"),
                    data: { listCriteria: ko.toJSON(this.data()) },
                    success(response) {
                        console.log(ko.toJSON(response));
                        if (response.HttpStatusCodeResult === 200) {
                            self.count(response.Count);
                        } else if (response.HttpStatusCodeResult === 400) {
                            self.count(response.Count);
                            //if (data.Message !== "")
                                //$("#listCriteriaDisplay").prepend("<div style=\"padding: 8px;\" class=\"alert alert-warning\" id=\"message\">" + data.Message + "</div>");
                        } else {
                            self.count(response.Count);
                            //if (data.Message != '')
                                //$("#listCriteriaDisplay").prepend("<div style=\"padding: 8px;\" class=\"alert alert-danger\" id=\"message\">" + data.Message + "</div>");
                        }
                        ////$('#listCountWaiting').hide();
                        $("#updateCountLoading").hide();
                    }
                });
            return true;
        }   

        // add exact age
        addExactAge = (data: any, event: any) => { // use of arrow function ensures this = this class instead of the caller which is the $parent
            console.log("addExactAge called");
            const target = event.target || event.srcElement;
            const taregtValue = $(target.parentNode).find("#exactAge").val();
            $(target.parentNode).find("#alertMessage").remove();
            // verify age is 18 or older
            if (taregtValue < 18) {
                $(target.parentNode)
                    .append(
                        `<div style='color: red; margin-top: 15px;' id="alertMessage">Age must be 18 or older.</div>`);
                return false;
            }
            // verify age is not already in colleciton
            if (ko.utils.arrayFirst(viewModel1.listCriteriaViewModel.exactAges(), item => item.name === taregtValue)) {
                $(target.parentNode)
                    .append(
                    `<div style='color: red; margin-top: 15px;' id="alertMessage">The exact age ${taregtValue} has already been added.</div>`);
                return false;
            }
            this.exactAges.push(new ListBuilder.ValueLabel(taregtValue, taregtValue + " years old"));
            $(target.parentNode).find("#exactAge").val("");
            this.updateCount();
        };

      // add dob range
        addDobRange = (data: any, event: any) => {
            const target = event.target || event.srcElement;
            // remove previous validation message if present
            $(target.parentNode).find("#alertMessage").remove();
            const dobStart = new Dob($(target.parentNode).find("#dob_start").val());
            const dobEnd = new Dob($(target.parentNode).find("#dob_end").val());
            // validate
            const dobRange = new DobRange(dobStart, dobEnd);
            const validation = dobRange.isValid();
            if (!validation.result) {
                $(target.parentNode).append(`<div style='color: red; margin-top: 15px;' id="alertMessage">${validation.message}</div>`);
                return false;
            }
            if (ko.utils.arrayFirst(viewModel1.listCriteriaViewModel.dobRanges(), item => item.toString() === dobRange.toString())) {
                $(target.parentNode)
                    .append(
                    `<div style='color: red; margin-top: 15px;' id="alertMessage">The dob range ${dobRange.toString()} has already been added.</div>`);
                return false;
            }
            this.dobRanges.push(new DobRange(dobStart, dobEnd));
            $(target.parentNode).find("#dob_start").val("");
            $(target.parentNode).find("#dob_end").val("");
            this.updateCount();
        };

      addZips = (event: any, vm: any) => {
        const target = event.target || event.srcElement;
        var lines = $(event.target).val().split("\n");
        for (let i = 0; i < lines.length; i++) {
          const inputZip = lines[i].trim();
          // is this a valid 5 digit zip
          if (/^\d{5}$|^\d{5}$/.test(inputZip)) {
            // if zip does not exist in the array then add it
            if (jQuery.grep(this.zips(), a => a.name === inputZip).length === 0)
              this.zips.push(new Zip(inputZip, new State("", 0)));
            continue;
          }
        }
        if (this.zips().length === 0) {
          $(target.parentNode)
            .append(
              `<div style='color: red; margin-top: 15px;' id="alertMessage">Zips seems to be empty.</div>`);
          return false;
        }
        this.updateCount();
      };

      // END HELPERS

        getUrl(urlName: string) {
            const match = jQuery.grep(this.urls, (n) => n.name === urlName);
            return match[0].url;
        }
    }
}

