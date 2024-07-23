module AccurateAppend.ListBuilder {

  export class FinancesTabViewModel {

    constructor() {
      this.initializeDonatesToSection("#donatesToBody");
      this.initializeOccupationGeneralSection("#occupationBody");
      this.initializeOccupationDetailedSection("#occupationDetailedBody");
      this.initializeEducationSection("#educationBody");
      this.initializeEstIncomeSection("#estimatedIncomeBody");
      this.initializeNetWorthSection("#netWorthBody");
      this.initializeHomeValueSection("#homeValueBody");
      this.initializeHomeownerSection("#homeOwnerBody");
      this.initializeLengthOfResidenceSection("#lengthOfResidenceBody");
      this.initializeInvestingSection("#investingBody");
      this.initializeBusinessOwnerSection("#businessOwner");
      this.initializeAgeRangeMaleSection("#ageRangeMaleBody");
      this.initializeAgeRangeFemaleSection("#ageRangeFemaleBody");
    }

    initializeDonatesToSection(selector) {

      const inputs = this.getDonatesTo();

      // can be encapsulated in a common funtion
      const cols = 3;
      var elementsPerColumn = Math.ceil(inputs.length / cols) + 1;
      for (var col = 0; col < cols; col++) {
        var div = $('<div class="col-md-4"></div>');
        $.each(inputs,
          (i, v) => {
            if (col === 0 && (i >= elementsPerColumn)) return true;
            if (col === 1 && (i < elementsPerColumn || i >= elementsPerColumn * 2)) return true;
            if (col === 2 && (i < (elementsPerColumn * 2) || i >= elementsPerColumn * 3)) return true;
            div.append(
              `<div class="checkbox"><label><input type='checkbox' name='donates' id='${v.id}' value='${ko.toJSON(v)
              }' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${
              v.label}</label></div>`);
          });
        $(selector).append(div);
      }

    }

    initializeOccupationGeneralSection(selector) {

      const inputs = this.getOccupation();

      // can be encapsulated in a common funtion
      const cols = 3;
      var elementsPerColumn = Math.ceil(inputs.length / cols) + 1;
      for (var col = 0; col < cols; col++) {
        var div = $('<div class="col-md-4"></div>');
        $.each(inputs,
          (i, v) => {
            if (col === 0 && (i >= elementsPerColumn)) return true;
            if (col === 1 && (i < elementsPerColumn || i >= elementsPerColumn * 2)) return true;
            if (col === 2 && (i < (elementsPerColumn * 2) || i >= elementsPerColumn * 3)) return true;
            div.append(
              `<div class="checkbox"><label><input type='checkbox' name='occupationGeneral' id='${v.id}' value='${
              ko.toJSON(v)
              }' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${
              v.label}</label></div>`);
          });
        $(selector).append(div);
      }

    }

    initializeOccupationDetailedSection(selector) {

      const inputs = this.getOccupationDetailed();

      // can be encapsulated in a common funtion
      const cols = 3;
      var elementsPerColumn = Math.ceil(inputs.length / cols) + 1;
      for (var col = 0; col < cols; col++) {
        var div = $('<div class="col-md-4"></div>');
        $.each(inputs,
          (i, v) => {
            if (col === 0 && (i >= elementsPerColumn)) return true;
            if (col === 1 && (i < elementsPerColumn || i >= elementsPerColumn * 2)) return true;
            if (col === 2 && (i < (elementsPerColumn * 2) || i >= elementsPerColumn * 3)) return true;
            div.append(
              `<div class="checkbox"><label><input type='checkbox' name='occupationDetailed' id='${v.id}' value='${
              ko.toJSON(v)
              }' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${
              v.label}</label></div>`);
          });
        $(selector).append(div);
      }

    }

    initializeEducationSection(selector) {

      const inputs = this.getEducation();

      var div = $(selector);
      $.each(inputs,
        (i, v) => {
          div.append(
            `<div class="checkbox"><label><input type='checkbox' name='education' id='${v.id}' value='${ko.toJSON(v)
            }' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${
            v.label}</label></div>`);
          $(selector).append(div);
        });

    }

    initializeEstIncomeSection(selector) {

      const inputs = this.getEstIncome();

      // can be encapsulated in a common funtion
      const cols = 2;
      var elementsPerColumn = Math.ceil(inputs.length / cols) + 1;
      for (var col = 0; col < cols; col++) {
        var div = $('<div class="col-md-4"></div>');
        $.each(inputs,
          (i, v) => {
            if (col === 0 && (i >= elementsPerColumn)) return true;
            if (col === 1 && (i < elementsPerColumn || i >= elementsPerColumn * 2)) return true;
            div.append(
              `<div class="checkbox"><label><input type='checkbox' name='estIncome' id='${v.id}' value='${ko.toJSON(v)
              }' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${
              v.label}</label></div>`);
          });
        $(selector).append(div);
      }

    }

    initializeNetWorthSection(selector) {

      const inputs = this.getNetWorth();

      // can be encapsulated in a common funtion
      const cols = 2;
      var elementsPerColumn = Math.ceil(inputs.length / cols) + 1;
      for (var col = 0; col < cols; col++) {
        var div = $('<div class="col-md-4"></div>');
        $.each(inputs,
          (i, v) => {
            if (col === 0 && (i >= elementsPerColumn)) return true;
            if (col === 1 && (i < elementsPerColumn || i >= elementsPerColumn * 2)) return true;
            div.append(
              `<div class="checkbox"><label><input type='checkbox' name='netWorth' id='${v.id}' value='${ko.toJSON(v)
              }' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${
              v.label}</label></div>`);
          });
        $(selector).append(div);
      }

    }

    initializeHomeValueSection(selector) {

      const inputs = this.getHomeValue();

      // can be encapsulated in a common funtion
      const cols = 2;
      var elementsPerColumn = Math.ceil(inputs.length / cols) + 1;
      for (var col = 0; col < cols; col++) {
        var div = $('<div class="col-md-4"></div>');
        $.each(inputs,
          (i, v) => {
            if (col === 0 && (i >= elementsPerColumn)) return true;
            if (col === 1 && (i < elementsPerColumn || i >= elementsPerColumn * 2)) return true;
            div.append(
              `<div class="checkbox"><label><input type='checkbox' name='homeValue' id='${v.id}' value='${ko.toJSON(v)
              }' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${
              v.label}</label></div>`);
          });
        $(selector).append(div);
      }

    }

    initializeHomeownerSection(selector) {

      const inputs = this.getHomeowner();

      var div = $(selector);
      $.each(inputs,
        (i, v) => {
          div.append(
            `<label class="checkbox-inline"><input type='checkbox' name='ownRent' id='${v.id}' value='${ko.toJSON(v)
            }' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${
            v.label}</label>`);
          $(selector).append(div);
        });

    }

    initializeLengthOfResidenceSection(selector) {

      const inputs = this.getLengthOfResidence();

      // can be encapsulated in a common funtion
      const cols = 2;
      var elementsPerColumn = Math.ceil(inputs.length / cols) + 1;
      for (var col = 0; col < cols; col++) {
        var div = $('<div class="col-md-2"></div>');
        $.each(inputs,
          (i, v) => {
            if (col === 0 && (i >= elementsPerColumn)) return true;
            if (col === 1 && (i < elementsPerColumn || i >= elementsPerColumn * 2)) return true;
            div.append(
              `<div class="checkbox"><label><input type='checkbox' name='lengthOfResidence' id='${v.id}' value='${
              ko.toJSON(v)
              }' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${
              v.label}</label></div>`);
          });
        $(selector).append(div);
      }

    }

    initializeInvestingSection(selector) {

      const inputs = this.getInvestments();

      // can be encapsulated in a common funtion
      const cols = 2;
      var elementsPerColumn = Math.ceil(inputs.length / cols) + 1;
      for (var col = 0; col < cols; col++) {
        var div = $('<div class="col-md-5"></div>');
        $.each(inputs,
          (i, v) => {
            if (col === 0 && (i >= elementsPerColumn)) return true;
            if (col === 1 && (i < elementsPerColumn || i >= elementsPerColumn * 2)) return true;
            div.append(
              `<div class="checkbox"><label><input type='checkbox' name='investments' id='${v.id}' value='${ko.toJSON(v)
              }' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${
              v.label}</label></div>`);
          });
        $(selector).append(div);
      }

    }

    initializeBusinessOwnerSection(selector) {

      const inputs = this.getBusinessOwner();

      // can be encapsulated in a common funtion
      const cols = 2;
      var elementsPerColumn = Math.ceil(inputs.length / cols) + 1;
      for (var col = 0; col < cols; col++) {
        var div = $('<div class="col-md-3"></div>');
        $.each(inputs,
          (i, v) => {
            if (col === 0 && (i >= elementsPerColumn)) return true;
            if (col === 1 && (i < elementsPerColumn || i >= elementsPerColumn * 2)) return true;
            div.append(
              `<div class="checkbox"><label><input type='checkbox' name='businessOwner' id='${v.id}' value='${
              ko.toJSON(v)
              }' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${
              v.label}</label></div>`);
          });
        $(selector).append(div);
      }

    }

    initializeAgeRangeMaleSection(selector) {

      const inputs = this.getAgeRangesMale();

      // can be encapsulated in a common funtion
      var div = $(selector);
      $.each(inputs,
        (i, v) => {
          div.append(
            `<label class="checkbox-inline"><input type='checkbox' name='ageRangesMale' id='${v.id}' value='${
            ko.toJSON(v)
            }' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${
            v.label}</label>`);
          $(selector).append(div);
        });
    }

    initializeAgeRangeFemaleSection(selector) {

      const inputs = this.getAgeRangesFemale();

      // can be encapsulated in a common funtion
      var div = $(selector);
      $.each(inputs,
        (i, v) => {
          div.append(
            `<label class="checkbox-inline"><input type='checkbox' name='ageRangesFemale' id='${v.id}' value='${
            ko.toJSON(v)
            }' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${
            v.label}</label>`);
          $(selector).append(div);
        });
    }

    // LOOKUPS

    getDonatesTo(): ListBuilder.ValueLabel[] {
      return [
        new ListBuilder.ValueLabel("donationsContributions", "Contributions"),
        new ListBuilder.ValueLabel("donationsMailOrder", "Mail Order"),
        new ListBuilder.ValueLabel("donatestoCharitableCauses", "Charitable Causes"),
        new ListBuilder.ValueLabel("donatestoAnimalWelfare", "Animal Welfare"),
        new ListBuilder.ValueLabel("donatestoArtsCultural", "Arts Cultural"),
        new ListBuilder.ValueLabel("donatestoChildrens", "Childrens"),
        new ListBuilder.ValueLabel("donatestoEnvironmentWildlife", "Environment Wildlife"),
        new ListBuilder.ValueLabel("donatestoHealth", "Health"),
        new ListBuilder.ValueLabel("donatestoInternationalAid", "InternationalAid"),
        new ListBuilder.ValueLabel("donatestoPolitical", "Political"),
        new ListBuilder.ValueLabel("donatestoPoliticalConservative", "Political Conservative"),
        new ListBuilder.ValueLabel("donatestoPoliticalLiberal", "Political Liberal"),
        new ListBuilder.ValueLabel("donatestoReligious", "Religious"),
        new ListBuilder.ValueLabel("donatestoVeterans", "Veterans"),
        new ListBuilder.ValueLabel("donatestoOther", "Other"),
        new ListBuilder.ValueLabel("donatestoCommunityCharities", "Community Charities"),
        new ListBuilder.ValueLabel("donatestoPolitics", "Politics"),
        new ListBuilder.ValueLabel("donatestoCommunityInvolvement", "Community Involvement"),
        new ListBuilder.ValueLabel("donatestoEnvironmentalIssues", "Environmental Issues"),
        new ListBuilder.ValueLabel("donatestoPoliticalCharity", "Political Charity"),
        new ListBuilder.ValueLabel("donatestoCommunityCharity", "Community Charity")
      ];
    }

    getOccupation(): ListBuilder.ValueLabel[] {
      return [
        new ListBuilder.ValueLabel("A", "Professional / Technical"),
        new ListBuilder.ValueLabel("B", "Administration / Managerial"),
        new ListBuilder.ValueLabel("C", "Sales / Service"),
        new ListBuilder.ValueLabel("D", "Clerical / White Collar"),
        new ListBuilder.ValueLabel("E", "Craftsman / Blue Collar"),
        new ListBuilder.ValueLabel("F", "Student"),
        new ListBuilder.ValueLabel("G", "Homemaker"),
        new ListBuilder.ValueLabel("H", "Retired"),
        new ListBuilder.ValueLabel("I", "Farmer"),
        new ListBuilder.ValueLabel("J", "Military"),
        new ListBuilder.ValueLabel("K", "Religious"),
        new ListBuilder.ValueLabel("L", "Self Employed"),
        new ListBuilder.ValueLabel("M", "Self Employed - Professional / Technical"),
        new ListBuilder.ValueLabel("N", "Self Employed - Administration / Managerial"),
        // new ListBuilder.ValueLabel("O", "Self Employed - Sales / Service"), // no matching values in database
        new ListBuilder.ValueLabel("P", "Self Employed - Clerical / White Collar"),
        new ListBuilder.ValueLabel("Q", "Self Employed - Craftsman / Blue Collar"),
        //new ListBuilder.ValueLabel("R", "Self Employed - Student"), // no matching values in database
        //new ListBuilder.ValueLabel("S", "Self Employed - Homemaker"), // no matching values in database
        //new ListBuilder.ValueLabel("T", "Self Employed - Retired"), // no matching values in database
        //new ListBuilder.ValueLabel("U", "Self Employed - Other"), // no matching values in database
        new ListBuilder.ValueLabel("V", "Educator"),
        new ListBuilder.ValueLabel("W", "Financial Professional"),
        new ListBuilder.ValueLabel("X", "Legal Professional"),
        new ListBuilder.ValueLabel("Y", "Medical Professional"),
        new ListBuilder.ValueLabel("Z", "Other"),
        new ListBuilder.ValueLabel("", "Unknown")
      ];
    }

    getOccupationDetailed(): ListBuilder.ValueLabel[] {
      return [
        new ListBuilder.ValueLabel("T999", "Professional"),
        new ListBuilder.ValueLabel("T998", "Architect"),
        new ListBuilder.ValueLabel("T997", "Chemist"),
        new ListBuilder.ValueLabel("T996", "Curator"),
        new ListBuilder.ValueLabel("T995", "Engineer"),
        new ListBuilder.ValueLabel("T994", "Engineer/Aerospace"),
        new ListBuilder.ValueLabel("T993", "Engineer/Chemical"),
        new ListBuilder.ValueLabel("T992", "Engineer/Civil"),
        new ListBuilder.ValueLabel("T991", "Engineer/Electrical/Electronic"),
        new ListBuilder.ValueLabel("T990", "Engineer/Field"),
        new ListBuilder.ValueLabel("T989", "Engineer/Industrial"),
        new ListBuilder.ValueLabel("T988", "Engineer/Mechanical"),
        new ListBuilder.ValueLabel("T987", "Geologist"),
        new ListBuilder.ValueLabel("T986", "Home Economist"),
        new ListBuilder.ValueLabel("T985", "Legal/Attorney/Lawyer"),
        new ListBuilder.ValueLabel("T984", "Librarian/Archivist"),
        new ListBuilder.ValueLabel("T983", "Medical Doctor/Physician"),
        new ListBuilder.ValueLabel("T982", "Pastor"),
        new ListBuilder.ValueLabel("T981", "Pilot"),
        new ListBuilder.ValueLabel("T980", "Scientist"),
        new ListBuilder.ValueLabel("T979", "Statistician/Actuary"),
        new ListBuilder.ValueLabel("T978", "Veterinarian"),
        new ListBuilder.ValueLabel("T899", "Computer"),
        new ListBuilder.ValueLabel("T898", "Computer Operator"),
        new ListBuilder.ValueLabel("T897", "Computer Programmer"),
        new ListBuilder.ValueLabel("T896", "Computer/Systems Analyst"),
        new ListBuilder.ValueLabel("E799", "Executive/Upper Management"),
        new ListBuilder.ValueLabel("E798", "CEO/CFO/Chairman/Corp Officer"),
        new ListBuilder.ValueLabel("E797", "Comptroller"),
        new ListBuilder.ValueLabel("E796", "Politician/Legislator/Diplomat"),
        new ListBuilder.ValueLabel("E795", "President"),
        new ListBuilder.ValueLabel("E794", "Treasurer"),
        new ListBuilder.ValueLabel("E793", "Vice President"),
        new ListBuilder.ValueLabel("M699", "Middle Management"),
        new ListBuilder.ValueLabel("M698", "Account Executive"),
        new ListBuilder.ValueLabel("M697", "Director/Art Director"),
        new ListBuilder.ValueLabel("M696", "Director/Executive Director"),
        new ListBuilder.ValueLabel("M695", "Editor"),
        new ListBuilder.ValueLabel("M694", "Manager"),
        new ListBuilder.ValueLabel("M693", "Manager/Assistant Manager"),
        new ListBuilder.ValueLabel("M692", "Manager/Branch Manager"),
        new ListBuilder.ValueLabel("M691", "Manager/Credit Manager"),
        new ListBuilder.ValueLabel("M690", "Manager/District Manager"),
        new ListBuilder.ValueLabel("M689", "Manager/Division Manager"),
        new ListBuilder.ValueLabel("M688", "Manger/General Manager"),
        new ListBuilder.ValueLabel("M687", "Manager/Marketing Manager"),
        new ListBuilder.ValueLabel("M686", "Manager/Office Manager"),
        new ListBuilder.ValueLabel("M685", "Manager/Plant Manager"),
        new ListBuilder.ValueLabel("M684", "Manager/Product Manager"),
        new ListBuilder.ValueLabel("M683", "Manager/Project Manager"),
        new ListBuilder.ValueLabel("M682", "Manager/Property Manager"),
        new ListBuilder.ValueLabel("M681", "Manager/Regional Manager"),
        new ListBuilder.ValueLabel("M680", "Manager/Sales Manager"),
        new ListBuilder.ValueLabel("M679", "Manager/Store Manager"),
        new ListBuilder.ValueLabel("M678", "Manager/Traffic Manager"),
        new ListBuilder.ValueLabel("M677", "Manager/Warehouse Manager"),
        new ListBuilder.ValueLabel("M676", "Planner"),
        new ListBuilder.ValueLabel("M675", "Principal/Dean/Educator"),
        new ListBuilder.ValueLabel("M674", "Superintendent"),
        new ListBuilder.ValueLabel("M673", "Supervisor"),
        new ListBuilder.ValueLabel("W599", "White Collar Worker"),
        new ListBuilder.ValueLabel("W598", "Accounting/Biller/Billing clerk"),
        new ListBuilder.ValueLabel("W597", "Actor/Entertainer/Announcer"),
        new ListBuilder.ValueLabel("W596", "Adjuster"),
        new ListBuilder.ValueLabel("W595", "Administration/Management"),
        new ListBuilder.ValueLabel("W594", "Advertising"),
        new ListBuilder.ValueLabel("W593", "Agent"),
        new ListBuilder.ValueLabel("W592", "Aide/Assistant"),
        new ListBuilder.ValueLabel("W591", "Aide/Assistant/Executive"),
        new ListBuilder.ValueLabel("W590", "Aide/Assistant/Office"),
        new ListBuilder.ValueLabel("W589", "Aide/Assistant/School"),
        new ListBuilder.ValueLabel("W588", "Aide/Assistant/Staff"),
        new ListBuilder.ValueLabel("W587", "Aide/Assistant/Technical"),
        new ListBuilder.ValueLabel("W586", "Analyst"),
        new ListBuilder.ValueLabel("W585", "Appraiser"),
        new ListBuilder.ValueLabel("W584", "Artist"),
        new ListBuilder.ValueLabel("W583", "Auctioneer"),
        new ListBuilder.ValueLabel("W582", "Auditor"),
        new ListBuilder.ValueLabel("W581", "Banker"),
        new ListBuilder.ValueLabel("W580", "Banker/Loan Office"),
        new ListBuilder.ValueLabel("W579", "Banker/Loan Processor"),
        new ListBuilder.ValueLabel("W578", "Bookkeeper"),
        new ListBuilder.ValueLabel("W577", "Broker"),
        new ListBuilder.ValueLabel("W576", "Broker/Stock/Trader"),
        new ListBuilder.ValueLabel("W575", "Buyer"),
        new ListBuilder.ValueLabel("W574", "Cashier"),
        new ListBuilder.ValueLabel("W573", "Caterer"),
        new ListBuilder.ValueLabel("W572", "Checker"),
        new ListBuilder.ValueLabel("W571", "Claims Examiner/Rep/Adjudicator"),
        new ListBuilder.ValueLabel("W570", "Clerk"),
        new ListBuilder.ValueLabel("W569", "Clerk/File"),
        new ListBuilder.ValueLabel("W568", "Collector"),
        new ListBuilder.ValueLabel("W567", "Communications"),
        new ListBuilder.ValueLabel("W566", "Conservation/Environment"),
        new ListBuilder.ValueLabel("W565", "Consultant/Advisor"),
        new ListBuilder.ValueLabel("W564", "Coordinator"),
        new ListBuilder.ValueLabel("W563", "Customer Service/Representative"),
        new ListBuilder.ValueLabel("W562", "Designer"),
        new ListBuilder.ValueLabel("W561", "Detective/Investigator"),
        new ListBuilder.ValueLabel("W560", "Dispatcher"),
        new ListBuilder.ValueLabel("W559", "Draftsman"),
        new ListBuilder.ValueLabel("W558", "Estimator"),
        new ListBuilder.ValueLabel("W557", "Expeditor"),
        new ListBuilder.ValueLabel("W556", "Finance"),
        new ListBuilder.ValueLabel("W555", "Flight Attendant/Steward"),
        new ListBuilder.ValueLabel("W554", "Florist"),
        new ListBuilder.ValueLabel("W553", "Graphic Designer/Commercial Artist"),
        new ListBuilder.ValueLabel("W552", "Hostess/Host/Usher"),
        new ListBuilder.ValueLabel("W551", "Insurance/Agent"),
        new ListBuilder.ValueLabel("W550", "Insurance/Underwriter"),
        new ListBuilder.ValueLabel("W549", "Interior Designer"),
        new ListBuilder.ValueLabel("W548", "Jeweler"),
        new ListBuilder.ValueLabel("W547", "Marketing"),
        new ListBuilder.ValueLabel("W546", "Merchandiser"),
        new ListBuilder.ValueLabel("W545", "Model"),
        new ListBuilder.ValueLabel("W544", "Musician/Music/Dance"),
        new ListBuilder.ValueLabel("W543", "Personnel/Recruiter/Interviewer"),
        new ListBuilder.ValueLabel("W542", "Photography"),
        new ListBuilder.ValueLabel("W541", "Public Relations"),
        new ListBuilder.ValueLabel("W540", "Publishing"),
        new ListBuilder.ValueLabel("W539", "Purchasing"),
        new ListBuilder.ValueLabel("W538", "Quality Control"),
        new ListBuilder.ValueLabel("W537", "Real Estate/Realtor"),
        new ListBuilder.ValueLabel("W536", "Receptionist"),
        new ListBuilder.ValueLabel("W535", "Reporter"),
        new ListBuilder.ValueLabel("W534", "Researcher"),
        new ListBuilder.ValueLabel("W533", "Sales"),
        new ListBuilder.ValueLabel("W532", "Sales Clerk/Counterman"),
        new ListBuilder.ValueLabel("W531", "Security"),
        new ListBuilder.ValueLabel("W530", "Surveyor"),
        new ListBuilder.ValueLabel("W529", "Technician"),
        new ListBuilder.ValueLabel("W528", "Telemarketer/Telephone/Operator"),
        new ListBuilder.ValueLabel("W527", "Teller/Bank Teller"),
        new ListBuilder.ValueLabel("W526", "Tester"),
        new ListBuilder.ValueLabel("W525", "Transcripter/Translator"),
        new ListBuilder.ValueLabel("W524", "Travel Agent"),
        new ListBuilder.ValueLabel("W523", "Union Member/Rep."),
        new ListBuilder.ValueLabel("W522", "Ward Clerk"),
        new ListBuilder.ValueLabel("W521", "Water Treatment"),
        new ListBuilder.ValueLabel("W520", "Writer"),
        new ListBuilder.ValueLabel("L499", "Blue Collar Worker"),
        new ListBuilder.ValueLabel("L498", "Animal Technician/Groomer"),
        new ListBuilder.ValueLabel("L497", "Apprentice"),
        new ListBuilder.ValueLabel("L496", "Assembler"),
        new ListBuilder.ValueLabel("L495", "Athlete/Professional"),
        new ListBuilder.ValueLabel("L494", "Attendant"),
        new ListBuilder.ValueLabel("L493", "Auto Mechanic"),
        new ListBuilder.ValueLabel("L492", "Baker"),
        new ListBuilder.ValueLabel("L491", "Barber/Hairstylist/Beautician"),
        new ListBuilder.ValueLabel("L490", "Bartender"),
        new ListBuilder.ValueLabel("L489", "Binder"),
        new ListBuilder.ValueLabel("L488", "Bodyman"),
        new ListBuilder.ValueLabel("L487", "Brakeman"),
        new ListBuilder.ValueLabel("L486", "Brewer"),
        new ListBuilder.ValueLabel("L485", "Butcher/Meat Cutter"),
        new ListBuilder.ValueLabel("L484", "Carpenter/Furniture/Woodworking"),
        new ListBuilder.ValueLabel("L483", "Chef/Butler"),
        new ListBuilder.ValueLabel("L482", "Child Care/Day Care/Babysitter"),
        new ListBuilder.ValueLabel("L481", "Cleaner/Laundry"),
        new ListBuilder.ValueLabel("L480", "Clerk/Deli"),
        new ListBuilder.ValueLabel("L479", "Clerk/Produce"),
        new ListBuilder.ValueLabel("L478", "Clerk/Stock"),
        new ListBuilder.ValueLabel("L477", "Conductor"),
        new ListBuilder.ValueLabel("L476", "Construction"),
        new ListBuilder.ValueLabel("L475", "Cook"),
        new ListBuilder.ValueLabel("L474", "Cosmetologist"),
        new ListBuilder.ValueLabel("L473", "Courier/Delivery/Messenger"),
        new ListBuilder.ValueLabel("L472", "Crewman"),
        new ListBuilder.ValueLabel("L471", "Custodian"),
        new ListBuilder.ValueLabel("L470", "Cutter"),
        new ListBuilder.ValueLabel("L469", "Dock Worker"),
        new ListBuilder.ValueLabel("L468", "Driver"),
        new ListBuilder.ValueLabel("L467", "Driver/Bus Driver"),
        new ListBuilder.ValueLabel("L466", "Driver/Truck Driver"),
        new ListBuilder.ValueLabel("L465", "Electrician"),
        new ListBuilder.ValueLabel("L464", "Fabricator"),
        new ListBuilder.ValueLabel("L463", "Factory Workman"),
        new ListBuilder.ValueLabel("L462", "Farmer/Dairyman"),
        new ListBuilder.ValueLabel("L461", "Finisher"),
        new ListBuilder.ValueLabel("L460", "Fisherman/Seaman"),
        new ListBuilder.ValueLabel("L459", "Fitter"),
        new ListBuilder.ValueLabel("L458", "Food Service"),
        new ListBuilder.ValueLabel("L457", "Foreman/Crew leader"),
        new ListBuilder.ValueLabel("L456", "Foreman/Shop Foreman"),
        new ListBuilder.ValueLabel("L455", "Forestry"),
        new ListBuilder.ValueLabel("L454", "Foundry Worker"),
        new ListBuilder.ValueLabel("L453", "Furrier"),
        new ListBuilder.ValueLabel("L452", "Gardener/Landscaper"),
        new ListBuilder.ValueLabel("L451", "Glazier"),
        new ListBuilder.ValueLabel("L450", "Grinder"),
        new ListBuilder.ValueLabel("L449", "Grocer"),
        new ListBuilder.ValueLabel("L448", "Helper"),
        new ListBuilder.ValueLabel("L447", "Housekeeper/Maid"),
        new ListBuilder.ValueLabel("L446", "Inspector"),
        new ListBuilder.ValueLabel("L445", "Installer"),
        new ListBuilder.ValueLabel("L444", "Ironworker"),
        new ListBuilder.ValueLabel("L443", "Janitor"),
        new ListBuilder.ValueLabel("L442", "Journeyman"),
        new ListBuilder.ValueLabel("L441", "Laborer"),
        new ListBuilder.ValueLabel("L440", "Lineman"),
        new ListBuilder.ValueLabel("L439", "Lithographer"),
        new ListBuilder.ValueLabel("L438", "Loader"),
        new ListBuilder.ValueLabel("L437", "Locksmith"),
        new ListBuilder.ValueLabel("L436", "Machinist"),
        new ListBuilder.ValueLabel("L435", "Maintenance"),
        new ListBuilder.ValueLabel("L434", "Maintenance/Supervisor"),
        new ListBuilder.ValueLabel("L433", "Mason/Brick/Etc."),
        new ListBuilder.ValueLabel("L432", "Material Handler"),
        new ListBuilder.ValueLabel("L431", "Mechanic"),
        new ListBuilder.ValueLabel("L430", "Meter Reader"),
        new ListBuilder.ValueLabel("L429", "Mill worker"),
        new ListBuilder.ValueLabel("L428", "Millwright"),
        new ListBuilder.ValueLabel("L427", "Miner"),
        new ListBuilder.ValueLabel("L426", "Mold Maker/Molder/Injection Mold"),
        new ListBuilder.ValueLabel("L425", "Oil Industry/Driller"),
        new ListBuilder.ValueLabel("L424", "Operator"),
        new ListBuilder.ValueLabel("L423", "Operator/Boilermaker"),
        new ListBuilder.ValueLabel("L422", "Operator/Crane Operator"),
        new ListBuilder.ValueLabel("L421", "Operator/Forklift Operator"),
        new ListBuilder.ValueLabel("L420", "Operator/Machine Operator"),
        new ListBuilder.ValueLabel("L419", "Packer"),
        new ListBuilder.ValueLabel("L418", "Painter"),
        new ListBuilder.ValueLabel("L417", "Parts (Auto Etc.)"),
        new ListBuilder.ValueLabel("L416", "Pipe fitter"),
        new ListBuilder.ValueLabel("L415", "Plumber"),
        new ListBuilder.ValueLabel("L414", "Polisher"),
        new ListBuilder.ValueLabel("L413", "Porter"),
        new ListBuilder.ValueLabel("L412", "Press Operator"),
        new ListBuilder.ValueLabel("L411", "Presser"),
        new ListBuilder.ValueLabel("L410", "Printer"),
        new ListBuilder.ValueLabel("L409", "Production"),
        new ListBuilder.ValueLabel("L408", "Repairman"),
        new ListBuilder.ValueLabel("L407", "Roofer"),
        new ListBuilder.ValueLabel("L406", "Sanitation/Exterminator"),
        new ListBuilder.ValueLabel("L405", "Seamstress/Tailor/Handicraft"),
        new ListBuilder.ValueLabel("L404", "Setup man"),
        new ListBuilder.ValueLabel("L403", "Sheet Metal Worker/Steel Worker"),
        new ListBuilder.ValueLabel("L402", "Shipping/Import/Export/Custom"),
        new ListBuilder.ValueLabel("L401", "Sorter"),
        new ListBuilder.ValueLabel("L400", "Toolmaker"),
        new ListBuilder.ValueLabel("L399", "Transportation"),
        new ListBuilder.ValueLabel("L398", "Typesetter"),
        new ListBuilder.ValueLabel("L397", "Upholstery"),
        new ListBuilder.ValueLabel("L396", "Utility"),
        new ListBuilder.ValueLabel("L395", "Waiter/Waitress"),
        new ListBuilder.ValueLabel("L394", "Welder"),
        new ListBuilder.ValueLabel("H349", "Health Services"),
        new ListBuilder.ValueLabel("H348", "Chiropractor"),
        new ListBuilder.ValueLabel("H347", "Dental Assistant"),
        new ListBuilder.ValueLabel("H346", "Dental Hygienist"),
        new ListBuilder.ValueLabel("H345", "Dentist"),
        new ListBuilder.ValueLabel("H344", "Dietician"),
        new ListBuilder.ValueLabel("H343", "Health Care"),
        new ListBuilder.ValueLabel("H342", "Medical Assistant"),
        new ListBuilder.ValueLabel("H341", "Medical Secretary"),
        new ListBuilder.ValueLabel("H340", "Medical Technician"),
        new ListBuilder.ValueLabel("H339", "Medical/Paramedic"),
        new ListBuilder.ValueLabel("H338", "Nurses Aide/Orderly"),
        new ListBuilder.ValueLabel("H337", "Optician"),
        new ListBuilder.ValueLabel("H336", "Optometrist"),
        new ListBuilder.ValueLabel("H335", "Pharmacist/Pharmacy"),
        new ListBuilder.ValueLabel("H334", "Psychologist"),
        new ListBuilder.ValueLabel("H333", "Technician/Lab"),
        new ListBuilder.ValueLabel("H332", "Technician/X-ray"),
        new ListBuilder.ValueLabel("H331", "Therapist"),
        new ListBuilder.ValueLabel("H330", "Therapists/Physical"),
        new ListBuilder.ValueLabel("H329", "Nurse"),
        new ListBuilder.ValueLabel("H328", "Nurse (Registered)"),
        new ListBuilder.ValueLabel("H327", "Nurse/LPN"),
        new ListBuilder.ValueLabel("H326", "Social Worker/Case Worker"),
        new ListBuilder.ValueLabel("S299", "Legal/Paralegal/Assistant"),
        new ListBuilder.ValueLabel("S298", "Legal Secretary"),
        new ListBuilder.ValueLabel("S297", "Secretary"),
        new ListBuilder.ValueLabel("S296", "Typist"),
        new ListBuilder.ValueLabel("S295", "Data Entry/Key Punch"),
        new ListBuilder.ValueLabel("P249", "Homemaker"),
        new ListBuilder.ValueLabel("P248", "Retired"),
        new ListBuilder.ValueLabel("P247", "Retired/Pensioner"),
        new ListBuilder.ValueLabel("P246", "Part Time"),
        new ListBuilder.ValueLabel("P245", "Student"),
        new ListBuilder.ValueLabel("P244", "Volunteer"),
        new ListBuilder.ValueLabel("A199", "Armed Forces"),
        new ListBuilder.ValueLabel("A198", "Army Credit Union Trades"),
        new ListBuilder.ValueLabel("A197", "Navy Credit Union Trades"),
        new ListBuilder.ValueLabel("A196", "Air Force"),
        new ListBuilder.ValueLabel("A195", "National Guard"),
        new ListBuilder.ValueLabel("A194", "Coast Guard"),
        new ListBuilder.ValueLabel("A193", "Marines"),
        new ListBuilder.ValueLabel("I149", "Coach"),
        new ListBuilder.ValueLabel("I148", "Counselor"),
        new ListBuilder.ValueLabel("I147", "Instructor"),
        new ListBuilder.ValueLabel("I146", "Lecturer"),
        new ListBuilder.ValueLabel("I145", "Professor"),
        new ListBuilder.ValueLabel("I144", "Teacher"),
        new ListBuilder.ValueLabel("I143", "Trainer"),
        new ListBuilder.ValueLabel("C129", "Civil Service"),
        new ListBuilder.ValueLabel("C128", "Air Traffic Control"),
        new ListBuilder.ValueLabel("C127", "Civil Service/Government"),
        new ListBuilder.ValueLabel("C126", "Corrections/Probation/Parole"),
        new ListBuilder.ValueLabel("C125", "Court Reporter"),
        new ListBuilder.ValueLabel("C124", "Firefighter"),
        new ListBuilder.ValueLabel("C123", "Judge/Referee"),
        new ListBuilder.ValueLabel("C122", "Mail Carrier/Postal"),
        new ListBuilder.ValueLabel("C121", "Mail/Postmaster"),
        new ListBuilder.ValueLabel("C120", "Police/Trooper"),
      ];
    }

    getEducation(): ListBuilder.ValueLabel[] {
      return [
        new ListBuilder.ValueLabel("A", "Completed High School"),
        new ListBuilder.ValueLabel("B", "Completed College"),
        new ListBuilder.ValueLabel("C", "Completed Graduate School"),
        new ListBuilder.ValueLabel("D", "Attended Vocational/Technical"),
        new ListBuilder.ValueLabel("", "Unknown")
      ];
    }

    getEstIncome(): ListBuilder.ValueLabel[] {
      return [
        new ListBuilder.ValueLabel("A", "Under $10,000"),
        new ListBuilder.ValueLabel("B", "$10,000 - $14,999"),
        new ListBuilder.ValueLabel("C", "$15,000 - $19,999"),
        new ListBuilder.ValueLabel("D", "$20,000 - $24,999"),
        new ListBuilder.ValueLabel("E", "$25,000 - $29,999"),
        new ListBuilder.ValueLabel("F", "$30,000 - $34,999"),
        new ListBuilder.ValueLabel("G", "$35,000 - $39,999"),
        new ListBuilder.ValueLabel("H", "$40,000 - $44,999"),
        new ListBuilder.ValueLabel("I", "$45,000 - $49,999"),
        new ListBuilder.ValueLabel("J", "$50,000 - $54,999"),
        new ListBuilder.ValueLabel("K", "$55,000 - $59,999"),
        new ListBuilder.ValueLabel("L", "$60,000 - $64,999"),
        new ListBuilder.ValueLabel("M", "$65,000 - $74,999"),
        new ListBuilder.ValueLabel("N", "$75,000 - $99,999"),
        new ListBuilder.ValueLabel("O", "$100,000 - $149,999"),
        new ListBuilder.ValueLabel("P", "$150,000 - $174,999"),
        new ListBuilder.ValueLabel("Q", "$175,000 - $199,999"),
        new ListBuilder.ValueLabel("R", "$200,000 - $249,999"),
        new ListBuilder.ValueLabel("S", "$250,000 +")
      ];
    }

    getNetWorth(): ListBuilder.ValueLabel[] {
      return [
        new ListBuilder.ValueLabel("A", "Less than $1"),
        new ListBuilder.ValueLabel("B", "$1 - $4,999"),
        new ListBuilder.ValueLabel("C", "$5,000 - $9,999"),
        new ListBuilder.ValueLabel("D", "$10,000 - $24,999"),
        new ListBuilder.ValueLabel("E", "$25,000 - $49,999"),
        new ListBuilder.ValueLabel("F", "$50,000 - $99,999"),
        new ListBuilder.ValueLabel("G", "$100,000 - $249,999"),
        new ListBuilder.ValueLabel("H", "$250,000 - $499,999"),
        new ListBuilder.ValueLabel("I", "Greater than $499,999")
      ];
    }

    getHomeValue(): ListBuilder.ValueLabel[] {
      return [
        new ListBuilder.ValueLabel("A", "$1,000 - $24,999"),
        new ListBuilder.ValueLabel("B", "$25,000 - $49,999"),
        new ListBuilder.ValueLabel("C", "$50,000 - $74,999"),
        new ListBuilder.ValueLabel("D", "$75,000 - $99,999"),
        new ListBuilder.ValueLabel("E", "$100,000 - $124,999"),
        new ListBuilder.ValueLabel("F", "$125,000 - $149,999"),
        new ListBuilder.ValueLabel("G", "$150,000 - $174,999"),
        new ListBuilder.ValueLabel("H", "$175,000 - $199,999"),
        new ListBuilder.ValueLabel("I", "$200,000 - $224,999"),
        new ListBuilder.ValueLabel("J", "$225,000 - $249,999"),
        new ListBuilder.ValueLabel("K", "$250,000 - $274,999"),
        new ListBuilder.ValueLabel("L", "$275,000 - $299,999"),
        new ListBuilder.ValueLabel("M", "$300,000 - $349,999"),
        new ListBuilder.ValueLabel("N", "$350,000 - $399,999"),
        new ListBuilder.ValueLabel("O", "$400,000 - $449,999"),
        new ListBuilder.ValueLabel("P", "$450,000 - $499,999"),
        new ListBuilder.ValueLabel("Q", "$500,000 - $749,999"),
        new ListBuilder.ValueLabel("R", "$750,000 - $999,999"),
        new ListBuilder.ValueLabel("S", "$1,000,000 Plus")
      ];
    }

    getHomeowner(): ListBuilder.ValueLabel[] {
      return [
        new ListBuilder.ValueLabel("H", "Homeowner"),
        new ListBuilder.ValueLabel("R", "Renter"),
        new ListBuilder.ValueLabel("", "Unknown")
      ];
    }

    getLengthOfResidence(): ListBuilder.ValueLabel[] {
      return [
        new ListBuilder.ValueLabel("1", "1 Year"),
        new ListBuilder.ValueLabel("2", "2 Years"),
        new ListBuilder.ValueLabel("3", "3 Years"),
        new ListBuilder.ValueLabel("4", "4 Years"),
        new ListBuilder.ValueLabel("5", "5 Years"),
        new ListBuilder.ValueLabel("6", "6 Years"),
        new ListBuilder.ValueLabel("7", "7 Years"),
        new ListBuilder.ValueLabel("8", "8 Years"),
        new ListBuilder.ValueLabel("9", "9 Years"),
        new ListBuilder.ValueLabel("10", "10 Years"),
        new ListBuilder.ValueLabel("11", "11 Years"),
        new ListBuilder.ValueLabel("12", "12 Years"),
        new ListBuilder.ValueLabel("13", "13 Years"),
        new ListBuilder.ValueLabel("14", "14 Years"),
        new ListBuilder.ValueLabel("15", "15 Years")
      ];
    }

    getHouseholdSize(): ListBuilder.ValueLabel[] {
      return [
        new ListBuilder.ValueLabel("1", "1 people"),
        new ListBuilder.ValueLabel("2", "2 people"),
        new ListBuilder.ValueLabel("3", "3 people"),
        new ListBuilder.ValueLabel("4", "4 people"),
        new ListBuilder.ValueLabel("5", "5 people"),
        new ListBuilder.ValueLabel("6", "6 people"),
        new ListBuilder.ValueLabel("7", "7 people"),
        new ListBuilder.ValueLabel("8", "8 people"),
        new ListBuilder.ValueLabel("9", "9 people")
      ];
    }

    getNumberOfAdults(): ListBuilder.ValueLabel[] {
      return [
        new ListBuilder.ValueLabel("1", "1 adult"),
        new ListBuilder.ValueLabel("2", "2 adults"),
        new ListBuilder.ValueLabel("3", "3 adults"),
        new ListBuilder.ValueLabel("4", "4 adults"),
        new ListBuilder.ValueLabel("5", "5 adults"),
        new ListBuilder.ValueLabel("6", "6 adults")
      ];
    }

    getInvestments(): ListBuilder.ValueLabel[] {
      return [
        new ListBuilder.ValueLabel("investmentsActive", "Active Investments"),
        new ListBuilder.ValueLabel("investmentsForeign", "Foreign Investments"),
        new ListBuilder.ValueLabel("investingActive", "Active Investor"),
        new ListBuilder.ValueLabel("investmentsPersonal", "Personal Investor"),
        new ListBuilder.ValueLabel("investmentsRealEstate", "Real Estate Investor"),
        new ListBuilder.ValueLabel("investmentsStocksBonds", "Stocks Bonds Investor")
      ];
    }

    getBusinessOwner(): ListBuilder.ValueLabel[] {
      return [
        new ListBuilder.ValueLabel("1", "Accountant"),
        new ListBuilder.ValueLabel("2", "Builder"),
        new ListBuilder.ValueLabel("3", "Contractor"),
        new ListBuilder.ValueLabel("4", "Dealer/Retailer/Storekeeper"),
        new ListBuilder.ValueLabel("5", "Distributor/Wholesaler"),
        new ListBuilder.ValueLabel("6", "Funeral Director"),
        new ListBuilder.ValueLabel("7", "Maker/Manufacturer"),
        new ListBuilder.ValueLabel("8", "Owner"),
        new ListBuilder.ValueLabel("9", "Partner"),
        new ListBuilder.ValueLabel("A", "Self-Employed")
      ];
    }

    getEstimatedResidentialPropertiesOwned() {
      return [
        new ListBuilder.ValueLabel("0", "0"),
        new ListBuilder.ValueLabel("1", "1"),
        new ListBuilder.ValueLabel("2", "2"),
        new ListBuilder.ValueLabel("3", "3"),
        new ListBuilder.ValueLabel("4", "4"),
        new ListBuilder.ValueLabel("5", "5"),
        new ListBuilder.ValueLabel("5", "6")
      ];
    }

    gettEstimatedResidentialPropertiesOwned(): ListBuilder.ValueLabel[] {
      return [
        new ListBuilder.ValueLabel("investmentEstimatedResidentialPropertiesOwned",
          "Estimated Residential Properties Owned")
      ];
    }

    getAgeRangesMale(): ListBuilder.ValueLabel[] {
      return [
        new ListBuilder.ValueLabel("HouseHoldHasMales18to24", "18-24"),
        new ListBuilder.ValueLabel("HouseHoldHasMales25to34", "25-34"),
        new ListBuilder.ValueLabel("HouseHoldHasMales35to44", "35-44"),
        new ListBuilder.ValueLabel("HouseHoldHasMales45to54", "45-54"),
        new ListBuilder.ValueLabel("HouseHoldHasMales55to64", "55-64"),
        new ListBuilder.ValueLabel("HouseHoldHasMales65to74", "65-74"),
        new ListBuilder.ValueLabel("HouseHoldHasMales75plus", "75+")
      ];
    }

    getAgeRangesFemale(): ListBuilder.ValueLabel[] {
      return [
        new ListBuilder.ValueLabel("HouseHoldHasFemales18to24", "18-24"),
        new ListBuilder.ValueLabel("HouseHoldHasFemales25to34", "25-34"),
        new ListBuilder.ValueLabel("HouseHoldHasFemales35to44", "35-44"),
        new ListBuilder.ValueLabel("HouseHoldHasFemales45to54", "45-54"),
        new ListBuilder.ValueLabel("HouseHoldHasFemales55to64", "55-64"),
        new ListBuilder.ValueLabel("HouseHoldHasFemales65to74", "65-74"),
        new ListBuilder.ValueLabel("HouseHoldHasFemales75plus", "75+")
      ];
    }

  }

}