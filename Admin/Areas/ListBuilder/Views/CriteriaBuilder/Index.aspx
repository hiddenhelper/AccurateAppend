<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<AccurateAppend.ListBuilder.Models.ListCriteria>" %>
<%@ Import Namespace="Newtonsoft.Json" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  List Builder2
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<style>
        
  .tab-pane { padding: 10px 0 0 0; }

  .panel-body { padding: 20px; }

</style>

<div class="row" style="padding: 20px;">
<div class="col-md-9" style="margin-bottom: 5px;">
<ul class="nav nav-tabs" role="tablist">
  <li role="presentation" class="active">
    <a href="#geographyTab" aria-controls="geographyTab" role="tab" data-toggle="tab">Geography</a>
  </li>
  <li role="presentation">
    <a href="#demographicsTab" aria-controls="demographicsTab" role="tab" data-toggle="tab">Demographics</a>
  </li>
  <li role="presentation">
    <a href="#financeTab" aria-controls="financeTab" role="tab" data-toggle="tab">Housing & Finance</a>
  </li>
  <li role="presentation">
    <a href="#interestsTab" aria-controls="interestsTab" role="tab" data-toggle="tab">Interests</a>
  </li>
</ul>

<div class="tab-content">
<!-- GEOGRAPHY -->
<div role="tabpanel" class="tab-pane active" id="geographyTab">
  <div class="panel-group" id="geographyAcordion" role="tablist" aria-multiselectable="true">
    <!-- STATE -->
    <div class="panel panel-default">
      <div class="panel-heading" role="tab" id="headingState">
        <h4 class="panel-title">
          <a role="button" data-toggle="collapse" data-parent="#geographyAcordion" href="#collapseState" aria-expanded="true" aria-controls="collapseState">
            State
          </a>
        </h4>
      </div>
      <div id="collapseState" class="panel-collapse collapse in" role="tabpanel" aria-labelledby="collapseState">
        <div class="panel-body">
          <div class="col-md-12">
            <div class="form-group">
              <div>
                <div class="row" id="stateOptions">
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <!-- COUNTY -->
    <div class="panel panel-default">
      <div class="panel-heading" role="tab" id="headingCounty">
        <h4 class="panel-title">
          <a role="button" data-toggle="collapse" data-parent="#geographyAcordion" href="#collapseCounty" aria-expanded="false" aria-controls="collapseCounty">
            County
          </a>
        </h4>
      </div>
      <div id="collapseCounty" class="panel-collapse collapse" role="tabpanel" aria-labelledby="collapseCounty">
        <div class="panel-body">
          <div class="col-md-12">
            <div class="form-group">
              <label>State</label>
              <select name="states" class="form-control" style="width: 250px;"
                      data-bind="
                                                        event: { change: function (data, event) { geographyTabViewModel.countiesTabOnChangeState(event, $data); } },
                                                        options: geographyTabViewModel.getStates(),
                                                        optionsValue: function (item) { return item.fips; },
                                                        optionsText: function (item) { return item.stateFullName + ' (' + item.abbreviation + ')'; },
                                                        optionsCaption: 'Choose state...'">
              </select>
            </div>
            <div class="form-group" id="countyControlGroup" style="display: none;">
              <label class="control-label" id="countyCount"></label>
              <div class="row" id="countyCountyOptions"></div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <!-- CITY -->
    <div class="panel panel-default">
      <div class="panel-heading" role="tab" id="headingCities">
        <h4 class="panel-title">
          <a class="collapsed" role="button" data-toggle="collapse" data-parent="#geographyAcordion" href="#collapseCities" aria-expanded="false" aria-controls="collapseCities">
            City
          </a>
        </h4>
      </div>
      <div id="collapseCities" class="panel-collapse collapse" role="tabpanel" aria-labelledby="headingCities">
        <div class="panel-body">
          <div class="col-md-12">
            <div class="form-group">
              <label>State</label>
              <select name="states" class="form-control" style="width: 250px;"
                      data-bind="
                                                        event: { change: function (data, event) { geographyTabViewModel.citiesTabOnChangeState(event, $data); } },
                                                        options: geographyTabViewModel.getStates(),
                                                        optionsValue: function (item) { return item.fips; },
                                                        optionsText: function (item) { return item.stateFullName + ' (' + item.abbreviation + ')'; },
                                                        optionsCaption: 'Choose state...'">
              </select>
            </div>
            <div class="form-group" id="cityCityControlGroup" style="display: none;">
              <label class="control-label" id="cityCount"></label>
              <div class="row" id="cityCityOptions"></div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <!-- ZIP -->
    <div class="panel panel-default">
      <div class="panel-heading" role="tab" id="headingZips">
        <h4 class="panel-title">
          <a class="collapsed" role="button" data-toggle="collapse" data-parent="#geographyAcordion" href="#collapseZips" aria-expanded="false" aria-controls="collapseZips">
            Postal Code
          </a>
        </h4>
      </div>
      <div id="collapseZips" class="panel-collapse collapse" role="tabpanel" aria-labelledby="headingZips">
        <div class="panel-body">
          <div class="col-md-12">
            <div class="form-group">
              <label>Zips</label>
              <p>Paste 5 digit postal codes, one per line</p>
              <textarea id="zips" class="form-control" cols="10" rows="10" data-bind="event: { change: function (data, event) { listCriteriaViewModel.addZips(event, $data); }, keyup: function (data, event) { listCriteriaViewModel.addZips(event, $data); }}"></textarea>
            </div>
          </div>
        </div>
      </div>
    </div>
    <!-- TIMEZONE -->
    <div class="panel panel-default">
      <div class="panel-heading" role="tab" id="headingTimeZone">
        <h4 class="panel-title">
          <a class="collapsed" role="button" data-toggle="collapse" data-parent="#geographyAcordion" href="#collapseTimezone" aria-expanded="false" aria-controls="collapseTimeZone">
            Time Zone
          </a>
        </h4>
      </div>
      <div id="collapseTimezone" class="panel-collapse collapse" role="tabpanel" aria-labelledby="headingTimeZone">
        <div class="panel-body">
          <label>Time Zone</label>
          <div id="timeZoneBody">
            <!-- dynamically generated content -->
          </div>
        </div>
      </div>
    </div>
  </div>
</div>

<!-- DEMOGRAPHICS -->
<div role="tabpanel" class="tab-pane" id="demographicsTab">
  <div class="panel-group" id="demogrpahicsAccordion" role="tablist" aria-multiselectable="true">
    <!-- AGE -->
    <div class="panel panel-default">
      <div class="panel-heading" role="tab" id="headingAge">
        <h4 class="panel-title">
          <a role="button" data-toggle="collapse" data-parent="#demogrpahicsAccordion" href="#collapseAge" aria-expanded="true" aria-controls="collapseAge">
            Income & Age
          </a>
        </h4>
      </div>
      <div id="collapseAge" class="panel-collapse collapse in" role="tabpanel" aria-labelledby="headingAge">
        <div class="panel-body">
          <div class="col-md-12">
            <label>Age Range</label>
            <div class="form-group" id="ageRangesBody">
              <!-- dynamically generated content -->
            </div>
            <label>Exact age</label>
            <div class="form-group form-inline">
              <div class="form-group">
                <input type="text" class="form-control" id="exactAge" placeholder="Age" style="width: 50px;">
              </div>
              <button type="button" class="btn btn-default" name="exactAge" data-bind="click: listCriteriaViewModel.addExactAge">Add</button>
            </div>
            <label>Dob Range</label>
            <p>To query a single month, enter the date in Start and End</p>
            <div class="form-group form-inline">
              <div class="form-group">
                <input name="dob_start" id="dob_start" class="form-control" style="display: inline; width: 125px;" placeholder="Start: MM-YYYY"/>
              </div>
              <div class="form-group">
                <input name="dob_end" id="dob_end" class="form-control" style="display: inline; width: 125px;" placeholder="End: MM-YYYY"/>
              </div>
              <button type="button" class="btn btn-default" data-bind="click: listCriteriaViewModel.addDobRange">Add</button>
            </div>

            <label>Estimated Household Income</label>
            <div class="row form-group">
              <div id="estimatedIncomeBody">
                <!-- dynamically generated content -->
              </div>
            </div>
            <label>Estimated Net Worth</label>
            <div class="row form-group">
              <div id="netWorthBody">
                <!-- dynamically generated content -->
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <!-- GENDER, MARITAL, HOH -->
    <div class="panel panel-default">
      <div class="panel-heading" role="tab" id="headingGender">
        <h4 class="panel-title">
          <a role="button" data-toggle="collapse" data-parent="#demogrpahicsAccordion" href="#collapseGender" aria-expanded="false" aria-controls="collapseGender">
            Gender, Marital & Head of Household
          </a>
        </h4>
      </div>
      <div id="collapseGender" class="panel-collapse collapse" role="tabpanel" aria-labelledby="headingGender">
        <div class="panel-body">
          <div class="col-md-12">
            <label>Gender</label>
            <div class="form-group" id="genderBody">
            </div>
            <label>Marital Status</label>
            <div class="form-group" id="maritalStatusBody">
            </div>
            <label>Inferred Household Rank</label>
            <div class="form-group" id="headOfHouseholdBody">
            </div>
          </div>
        </div>
      </div>
    </div>
    <!-- LANGUAGE -->
    <div class="panel panel-default">
      <div class="panel-heading" role="tab" id="headingLanguage">
        <h4 class="panel-title">
          <a role="button" data-toggle="collapse" data-parent="#demogrpahicsAccordion" href="#collapseLanguage" aria-expanded="false" aria-controls="collapseLanguage">
            Language
          </a>
        </h4>
      </div>
      <div id="collapseLanguage" class="panel-collapse collapse" role="tabpanel" aria-labelledby="headingLanguage">
        <div class="panel-body" id="languageBody">
          <!-- dynamically generated content -->
        </div>
      </div>
    </div>
  </div>
</div>

<!-- FINANCE & HOUSING-->
<div role="tabpanel" class="tab-pane" id="financeTab">
  <div class="panel-group" id="financeAccordion" role="tablist" aria-multiselectable="true">
    <!-- FINANCIAL -->
    <div class="panel panel-default">
      <div class="panel-heading" role="tab" id="headingFinancial">
        <h4 class="panel-title">
          <a role="button" data-toggle="collapse" data-parent="#financialAccordion" href="#collapseFinancial" aria-expanded="true" aria-controls="collapseFinancial">
            Income & Housing
          </a>
        </h4>
      </div>
      <div id="collapseFinancial" class="panel-collapse collapse in" role="tabpanel" aria-labelledby="headingFinancial">
        <div class="panel-body">
          <div class="col-md-12">
            <label>Estimated Home Value</label>
            <div class="row form-group">
              <div id="homeValueBody">
                <!-- dynamically generated content -->
              </div>
            </div>
            <label>Own/Rent</label>
            <div id="homeOwnerBody" class="form-group">
              <!-- dynamically generated content -->
            </div>
            <label>Length of Residence</label>
            <div id="lengthOfResidenceBody" class="row form-group">
              <%--<select name="lengthOfResidence" class="form-control" style="width: 250px;"
                      data-bind="
                                                        options: financesTabViewModel.getLengthOfResidence(),
                                                        optionsValue: function (item) { return ko.toJSON(item); },
                                                        optionsText: function (item) { return item.label; },
                                                        event: { change: function (data, event) { listCriteriaViewModel.toggle(event); return false; } },
                                                        optionsCaption: 'Choose value...'">
              </select>--%>
            </div>
            <%--<div class="form-group">
                                            <label>Household Size</label>
                                            <select name="householdSize" class="form-control" style="width: 250px;" 
                                                    data-bind="
                                                        options: financesTabViewModel.getHouseholdSize(),
                                                        optionsValue: function (item) { return ko.toJSON(item); },
                                                        optionsText: function (item) { return item.label; },
                                                        event: { change: function (data, event) { listCriteriaViewModel.toggle(event); return true; } },
                                                        optionsCaption: 'Choose value...'">
                                            </select>    
                                        </div>--%>
            <div class="form-group">
              <label>Number of Adults in Household</label>
              <select name="numberOfAdults" class="form-control" style="width: 250px;"
                      data-bind="
                                                        options: financesTabViewModel.getNumberOfAdults(),
                                                        optionsValue: function (item) { return ko.toJSON(item); },
                                                        optionsText: function (item) { return item.label; },
                                                        event: { change: function (data, event) { listCriteriaViewModel.toggle(event); return true; } },
                                                        optionsCaption: 'Choose value...'">
              </select>
            </div>

            <label>Adult Age Ranges in Household (Male)</label>
            <div id="ageRangeMaleBody" class="form-group">
            </div>
            <label>Adult Age Ranges in Household (Female)</label>
            <div id="ageRangeFemaleBody" class="form-group">
            </div>
            <%--<label>Age Range (Gender Unknown)</label>
                                        <div id="ageRangeUnknownBody" class="form-group">
                                        </div>--%>
          </div>
        </div>
      </div>
    </div>
    <!-- INVESTING -->
    <div class="panel panel-default">
      <div class="panel-heading" role="tab" id="headingInvesting">
        <h4 class="panel-title">
          <a role="button" data-toggle="collapse" data-parent="#financeAccordion" href="#collapseInvesting" aria-expanded="false" aria-controls="collapseInvesting">
            Investing
          </a>
        </h4>
      </div>
      <div id="collapseInvesting" class="panel-collapse collapse" role="tabpanel" aria-labelledby="headingInvesting">
        <div class="panel-body">
          <div class="row form-group">
            <div id="investingBody">
              <!-- dynamically generated content -->
            </div>
          </div>
          <div class="form-group">
            <label>Estimated Residential Properties Owned</label>
            <select name="investmentEstimatedResidentialPropertiesOwned" class="form-control" style="width: 250px;"
                    data-bind="
                                                    options: financesTabViewModel.getEstimatedResidentialPropertiesOwned(),
                                                    optionsValue: function (item) { return ko.toJSON(item); },
                                                    optionsText: function (item) { return item.label; },
                                                    event: { change: function (data, event) { listCriteriaViewModel.toggle(event); return true; } },
                                                    optionsCaption: 'Choose value...'">
            </select>
          </div>
        </div>
      </div>
    </div>
    <!-- EDUCATION & OCCUPATION -->
    <div class="panel panel-default">
      <div class="panel-heading" role="tab" id="headingEducation">
        <h4 class="panel-title">
          <a role="button" data-toggle="collapse" data-parent="#financeAccordion" href="#collapseEducation" aria-expanded="false" aria-controls="collapseEducation">
            Education & Occupation
          </a>
        </h4>
      </div>
      <div id="collapseEducation" class="panel-collapse collapse" role="tabpanel" aria-labelledby="headingEducation">
        <div class="panel-body">
          <div class="col-md-12">
            <label>Business Owner</label>
            <div class="row form-group">
              <div id="businessOwner">
                <!-- dynamically generated content -->
              </div>
            </div>
            <label>Education</label>
            <div class="row form-group">
              <div id="educationBody" class="col-md-4">
                <!-- dynamically generated content -->
              </div>
            </div>
            <label>Occupation</label>
            <div class="row form-group">
              <div id="occupationBody">
                <!-- dynamically generated content -->
              </div>
            </div>
            <label>Occupation - Detailed</label>
            <div class="row form-group">
              <div id="occupationDetailedBody">
                <!-- dynamically generated content -->
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <!-- DONATIONS -->
    <div class="panel panel-default">
      <div class="panel-heading" role="tab" id="headingDonatesTo">
        <h4 class="panel-title">
          <a role="button" data-toggle="collapse" data-parent="#financeAccordion" href="#collapseDonatesTo" aria-expanded="false" aria-controls="collapseDonatesTo">
            Donations
          </a>
        </h4>
      </div>
      <div id="collapseDonatesTo" class="panel-collapse collapse" role="tabpanel" aria-labelledby="headingDonatesTo">
        <div class="panel-body" id="donatesToBody">
          <!-- dynamically generated content -->
        </div>
      </div>
    </div>
  </div>
</div>

<!-- INTERESTS -->
<div role="tabpanel" class="tab-pane" id="interestsTab">
  <div class="panel-group" id="interestsAccordion" role="tablist" aria-multiselectable="true">
    <!-- PURCHASES -->
    <div class="panel panel-default">
      <div class="panel-heading" role="tab" id="headingPurchases">
        <h4 class="panel-title">
          <a role="button" data-toggle="collapse" data-parent="#interestsAccordion" href="#collapsePurchases" aria-expanded="true" aria-controls="collapsePurchases">
            Purchases
          </a>
        </h4>
      </div>
      <div id="collapsePurchases" class="panel-collapse collapse in" role="tabpanel" aria-labelledby="headingPurchases">
        <div class="panel-body" id="purchasesBody">
          <!-- dynamically generated content -->
        </div>
      </div>
    </div>
    <!-- SPORTS, FITNESS & OUTDOORS -->
    <div class="panel panel-default">
      <div class="panel-heading" role="tab" id="headingSportsFitnessOutdoor">
        <h4 class="panel-title">
          <a role="button" data-toggle="collapse" data-parent="#interestsAccordion" href="#collapseSportsFitnessOutdoor" aria-expanded="false" aria-controls="collapseSportsFitnessOutdoor">
            Sports, Fitness & Outdoors
          </a>
        </h4>
      </div>
      <div id="collapseSportsFitnessOutdoor" class="panel-collapse collapse" role="tabpanel" aria-labelledby="headingSportsFitnessOutdoor">
        <div class="panel-body">
          <div class="col-md-12">
            <div class="row form-group">
              <label>Sports</label>
              <div id="sportsBody" class="form-group">
                <!-- dynamically generated content -->
              </div>
            </div>
            <div class="row form-group">
              <label>Fitness</label>
              <div id="fitnessBody" class="form-group">
                <!-- dynamically generated content -->
              </div>
            </div>
            <div class="row form-group">
              <label>Outdoors</label>
              <div id="outdoorsGeneralBody" class="form-group">
                <!-- dynamically generated content -->
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <!-- MAGAZINES -->
    <div class="panel panel-default">
      <div class="panel-heading" role="tab" id="headingReading">
        <h4 class="panel-title">
          <a role="button" data-toggle="collapse" data-parent="#interestsAccordion" href="#collapseReading" aria-expanded="false" aria-controls="collapseReading">
            Reading & Magazines
          </a>
        </h4>
      </div>
      <div id="collapseReading" class="panel-collapse collapse" role="tabpanel" aria-labelledby="headingReading">
        <div class="panel-body">
          <div class="col-md-12">
            <label>General Reading</label>
            <div id="readingGeneralBody" class="form-group">
              <!-- dynamically generated content -->
            </div>
            <label>Magazines & Publications</label>
            <div id="readingMagazinesBody" class="form-group">
              <!-- dynamically generated content -->
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</div>
</div>

</div>
<div class="col-md-3">
  <div class="panel panel-default" data-bind="template: { name: 'tplListCriteria' }">
  </div>
</div>
</div>

<%= Html.Hidden("listCriteria", JsonConvert.SerializeObject(Model)) %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

<script type="text/javascript">

  // setup to insert navigators
  var lookupUrls = [
    { name: "GetCounties", url: "<%: Url.Action("GetCounties", "Api", new {area = "ListBuilder"}) %>" },
    { name: "GetZips", url: "<%: Url.Action("GetZips", "Api", new {area = "ListBuilder"}) %>" },
    { name: "GetCities", url: "<%: Url.Action("GetCities", "Api", new {area = "ListBuilder"}) %>" },
    { name: "GetStates", url: "<%: Url.Action("GetStates", "Api", new {area = "ListBuilder"}) %>" },
    {
      name: "GenerateListCount",
      url: "<%: Url.Action("Query", "GenerateListCount", new {area = "ListBuilder"}) %>"
    }
  ];

  var requestId = "<%: Model.RequestId %>";

  $(document).ready(function() {
    $("#defintionFile").kendoUpload();
  });

</script>

<script src="<%= Url.Content("~/scripts/knockout-3.4.2.js") %>" type="text/javascript"></script>
<%--<script src="<%= Url.Content("~/scripts/underscore.min.js") %>" type="text/javascript"></script>--%>
<script src="<%= Url.Content("~/scripts/json2.js") %>" type="text/javascript"></script>
<script src="<%= Url.Content("~/Areas/ListBuilder/Scripts/Objects.js") %>" type="text/javascript"></script>
<script src="<%= Url.Content("~/Areas/ListBuilder/Scripts/ListCriteriaViewModel.js") %>" type="text/javascript"></script>
<script src="<%= Url.Content("~/Areas/ListBuilder/Scripts/GeographyTabViewModel.js") %>" type="text/javascript"></script>
<script src="<%= Url.Content("~/Areas/ListBuilder/Scripts/FinancesTabViewModel.js") %>" type="text/javascript"></script>
<script src="<%= Url.Content("~/Areas/ListBuilder/Scripts/DemographicsTabViewModel.js") %>" type="text/javascript"></script>
<script src="<%= Url.Content("~/Areas/ListBuilder/Scripts/InterestsTabViewModel.js") %>" type="text/javascript"></script>
<script src="<%= Url.Content("~/Areas/ListBuilder/Scripts/Index.js") %>" type="text/javascript"></script>


<script id="tplListCriteria" type="text/html">

  <div class="panel-heading">
    <h3 class="panel-title">Summary</h3>
  </div>
  <div class="panel-body" id="listCriteriaViewModelDisplay">
    <h3 id="listCount" style="margin: 10px 0 20px 0;">
      Count: <span data-bind="text: numberWithCommas(listCriteriaViewModel.count())"></span>
      <i class="fa fa-refresh fa-spin" style="display: none; font-size: 20px;" id="updateCountLoading"></i>
    </h3>

    <!-- GEOGRAPHY -->
    <section data-bind="template: { name: 'tplStatesCriteria' }, visible: listCriteriaViewModel.states().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplCitiesCriteria' }, visible: listCriteriaViewModel.cities().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplZipsCriteria' }, visible: listCriteriaViewModel.zips().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplCountiesCriteria' }, visible: listCriteriaViewModel.counties().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplTimeZonesCriteria' }, visible: listCriteriaViewModel.timeZones().length" style="margin-bottom: 15px;"></section>

    <!-- DEMOGRPHICS -->
    <section data-bind="template: { name: 'tplAgeRangeCriteria' }, visible: listCriteriaViewModel.ageRanges().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplExactAgeCriteria' }, visible: listCriteriaViewModel.exactAges().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplDobRangeCriteria' }, visible: listCriteriaViewModel.dobRanges().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplGenderCriteria' }, visible: listCriteriaViewModel.gender().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplMaritalStatusCriteria' }, visible: listCriteriaViewModel.maritalStatus().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplHohCriteria' }, visible: listCriteriaViewModel.hoh().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplLanguageCriteria' }, visible: listCriteriaViewModel.languages().length" style="margin-bottom: 15px;"></section>

    <!-- FINANCES -->
    <section data-bind="template: { name: 'tplEstIncomeCriteria' }, visible: listCriteriaViewModel.estIncome().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplNetWorthCriteria' }, visible: listCriteriaViewModel.netWorth().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplHomeValueCriteria' }, visible: listCriteriaViewModel.homeValue().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplOwnRentCriteria' }, visible: listCriteriaViewModel.ownRent().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplLengthOfResidenceCriteria' }, visible: listCriteriaViewModel.lengthOfResidence().length" style="margin-bottom: 15px;"></section>
    <%--<section data-bind="template: { name: 'tplHouseholdSizeCriteria' }, visible: listCriteriaViewModel.householdSize().length" style="margin-bottom: 15px;"></section>--%>
    <section data-bind="template: { name: 'tplNumberOfAdultsCriteria' }, visible: listCriteriaViewModel.numberOfAdults().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplAgeRangeMaleCriteria' }, visible: listCriteriaViewModel.ageRangesMale().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplAgeRangeFemaleCriteria' }, visible: listCriteriaViewModel.ageRangesFemale().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplInvestingCriteria' }, visible: listCriteriaViewModel.investments().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplRentalPropertiesOwnedCriteria' }, visible: listCriteriaViewModel.investmentEstimatedResidentialPropertiesOwned().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplEducationCriteria' }, visible: listCriteriaViewModel.education().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplOccupationGeneralCriteria' }, visible: listCriteriaViewModel.occupationGeneral().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplOccupationDetailedCriteria' }, visible: listCriteriaViewModel.occupationDetailed().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplDonatesToCriteria' }, visible: listCriteriaViewModel.donates().length" style="margin-bottom: 15px;"></section>

    <!-- INTERESTS -->
    <section data-bind="template: { name: 'tplInterestsPurchasesCriteria' }, visible: listCriteriaViewModel.interestsPurchases().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplInterestsSportsCriteria' }, visible: listCriteriaViewModel.interestsSports().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplInterestsFitnessCriteria' }, visible: listCriteriaViewModel.interestsFitness().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplInterestsOutdoorsCriteria' }, visible: listCriteriaViewModel.interestsOutdoors().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplInterestsReadingGeneralCriteria' }, visible: listCriteriaViewModel.interestsReadingGeneral().length" style="margin-bottom: 15px;"></section>
    <section data-bind="template: { name: 'tplInterestsMagazinesCriteria' }, visible: listCriteriaViewModel.interestsReadingMagazinesAndSubscriptions().length" style="margin-bottom: 15px;"></section>

    <div class="form-group" id="listCriteriaDisplay">
      <%--<button type="button" class="btn btn-default" data-toggle="modal" data-target="#loadFromDefintion">
                    Load From File
                </button>--%>
      <%--<button class="btn btn-default" onclick="viewModel.reset()">Reset</button>--%>
      <div class="form-group">
        <button class="btn btn-default" data-bind="click: downloadList, visible: listCriteriaViewModel.count() > 0" id="downloadList">
          <i class="fa fa-refresh fa-spin" style="display: none; font-size: 16px;"></i>&nbsp;Download List
        </button>
        <button class="btn btn-default" data-bind="click: enhanceList, visible: listCriteriaViewModel.count() > 0" id="enhanceList">
          <i class="fa fa-refresh fa-spin" style="display: none; font-size: 16px;"></i>&nbsp;Next >
        </button>
        <%--<button class="btn btn-default" data-bind="click: downloadDefintion, visible: listDefintionDownloadUri() != ''"  id="downloadListDefintion" >
                        &nbsp;Download Definition
                    </button>--%>
      </div>
    </div>
  </div>

</script>

<!-- GEOGRAPHY -->

<script id="tplStatesCriteria" type="text/html">
  <h4>States</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.states">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: stateFullName + ' (' + abbreviation + ')'"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="states"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplCitiesCriteria" type="text/html">
  <h4>Cities</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.cities">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: name + ' (' + state.abbreviation + ')'"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="cities"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplZipsCriteria" type="text/html">
  <h4>Zip</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.zips">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: name"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="zips"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplCountiesCriteria" type="text/html">
  <h4>County</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.counties">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: name + ' (' + state.abbreviation + ')'"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="counties"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplTimeZonesCriteria" type="text/html">
  <h4>Time Zone</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.timeZones">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="timeZones"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<!-- DEMOGRPHICS -->

<script id="tplAgeRangeCriteria" type="text/html">
  <h4>Age Range</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.ageRanges">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="ageRanges"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplExactAgeCriteria" type="text/html">
  <h4>Exact Age</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.exactAges">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="exactAges"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplDobRangeCriteria" type="text/html">
  <h4>Date of Birth Range</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.dobRanges">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: start.month + '-' + start.year + ' to ' + end.month + '-' + end.year"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="dobRanges"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplGenderCriteria" type="text/html">
  <h4>Gender</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.gender">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="gender"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplMaritalStatusCriteria" type="text/html">
  <h4>Marital Status</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.maritalStatus">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="maritalStatus"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplLanguageCriteria" type="text/html">
  <h4>Language</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.languages">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="languages"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplHohCriteria" type="text/html">
  <h4>Inferred Household Rank</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.hoh">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="hoh"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<!-- FINANCES -->

<script id="tplNetWorthCriteria" type="text/html">
  <h4>Net Worth</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.netWorth">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="netWorth"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplEstIncomeCriteria" type="text/html">
  <h4>Estimated Income</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.estIncome">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="estIncome"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplHomeValueCriteria" type="text/html">
  <h4>Home Value</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.homeValue">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="homeValue"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplOwnRentCriteria" type="text/html">
  <h4>Own/Rent</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.ownRent">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="ownRent"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplLengthOfResidenceCriteria" type="text/html">
  <h4>Length of Residence</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.lengthOfResidence">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="lengthOfResidence"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<%--<script id="tplHouseholdSizeCriteria" type="text/html">
        <h4>Household Size</h4>
        <table>
            <tbody data-bind="foreach: listCriteriaViewModel.householdSize">
            <tr>
                <td style="width: 90%">
                    <span data-bind="text: label"></span>
                </td>
                <td>
                    <button type="button" class="btn btn-default btn-xs" name="householdSize"
                            data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
                        Remove
                    </button>
                </td>
            </tr>
            </tbody>
        </table>
    </script>--%>

<script id="tplNumberOfAdultsCriteria" type="text/html">
  <h4>Number of Adults</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.numberOfAdults">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="numberOfAdults"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplAgeRangeMaleCriteria" type="text/html">
  <h4>Adult Age Ranges in Household (Male)</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.ageRangesMale">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="ageRanges"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplAgeRangeFemaleCriteria" type="text/html">
  <h4>Adult Age Ranges in Household (Female)</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.ageRangesFemale">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="ageRanges"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplInvestingCriteria" type="text/html">
  <h4>Investing</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.investments">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="investments"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplRentalPropertiesOwnedCriteria" type="text/html">
  <h4>Estimated Residential Properties Owned</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.investmentEstimatedResidentialPropertiesOwned">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="investments"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplEducationCriteria" type="text/html">
  <h4>Education</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.education">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="education"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplOccupationGeneralCriteria" type="text/html">
  <h4>Occupation - General</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.occupationGeneral">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="occupationGeneral"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplOccupationDetailedCriteria" type="text/html">
  <h4>Occupation - Detailed</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.occupationDetailed">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="occupationDetailed"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplDonatesToCriteria" type="text/html">
  <h4>Donates To</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.donates">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="donates"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<!-- INTERESTS -->

<script id="tplInterestsPurchasesCriteria" type="text/html">
  <h4>Interests - Purchases</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.interestsPurchases">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="interestsPurchases"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplInterestsSportsCriteria" type="text/html">
  <h4>Interests - Sports</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.interestsSports">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="interestsSports"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplInterestsFitnessCriteria" type="text/html">
  <h4>Interests - Fitness</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.interestsFitness">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="interestsFitness"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplInterestsOutdoorsCriteria" type="text/html">
  <h4>Interests - Outdoors</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.interestsOutdoors">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="interestsOutdoors"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplInterestsReadingGeneralCriteria" type="text/html">
  <h4>Interests - General Reading</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.interestsReadingGeneral">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="interestsReadingGeneral"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

<script id="tplInterestsMagazinesCriteria" type="text/html">
  <h4>Interests - Magazines</h4>
  <table>
    <tbody data-bind="foreach: listCriteriaViewModel.interestsReadingMagazinesAndSubscriptions">
    <tr>
      <td style="width: 90%">
        <span data-bind="text: label"></span>
      </td>
      <td>
        <button type="button" class="btn btn-default btn-xs" name="interestsReadingMagazinesAndSubscriptions"
                data-bind="value: ko.toJSON($data), click: function (data, event) { $root.listCriteriaViewModel.toggle(event); return true; }">
          Remove
        </button>
      </td>
    </tr>
    </tbody>
  </table>
</script>

</asp:Content>