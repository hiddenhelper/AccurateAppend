<%@ Page Inherits="System.Web.Mvc.ViewPage<AccurateAppend.Websites.Admin.Entities.BatchJobRequest>" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Title="" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Build Manifest
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
<div class="row" style="padding-bottom: 20px;">
    <div class="col-md-3">
        <h3 style="margin-top: 0">Build Manifest</h3>
    </div>
    <div class="col-md-9" style="text-align: right;">
        <% if(Model.InputFile != null) Html.RenderPartial("FileDisplay", Model.InputFile); %>
    </div>
</div> 
<div class="row" style="margin-bottom: 20px;">
<div class="col-md-3">
    <div class="panel-group" id="phoneAppendAccordion" role="tablist" aria-multiselectable="true">
      <div class="panel panel-default">
            <div class="panel-heading" role="tab" id="headingPopularProducts">
                <h4 class="panel-title">
                    <a role="button" data-toggle="collapse" data-parent="#phoneAppendAccordion" href="#collapsePopularProducts" aria-expanded="true" aria-controls="collapsePopularProducts">
                        Popular Products
                    </a>
                </h4>
            </div>
            <div id="collapsePopularProducts" class="panel-collapse collapse in" role="tabpanel" aria-labelledby="headingPopularProducts">
                <div class="panel-body">
                  <label style="display: block;">The Works</label>
                  <p>Multi-Match Phone Append, Email Append & Demographic Append.</p>
                  <button id="TheWorks-DynamicAppend" data-bind="click: function (data, event) { submitUsingPredefinedManifest(event); return true; }, text: getButtonText()" title="" class="btn btn-primary btn-xs">Next ></button>
                  <label style="display: block; margin-top: 10px;">Multi-Match Phone Append</label>
                  <p>Appends the best number from each source: DA, PREM & MOBILE, returns 3 columns, and up to 3 numbers for each input record.</p>
                  <button id="MultiMatchPhoneAppend-DynamicAppend" data-bind="click: function (data, event) { submitUsingPredefinedManifest(event); return true; }, text: getButtonText()" title="" class="btn btn-primary btn-xs">Next ></button>
                  <label style="display: block; margin-top: 10px;">Single Best Match Phone Append</label>
                  <p>Appends the best single number found in 1 of 3 sources. Searches DA, PREM & then MOBILE. Returns 1 column containing the best match.</p>
                  <button id="SingleBestMatch-DynamicAppend" data-bind="click: function (data, event) { submitUsingPredefinedManifest(event); return true; }, text: getButtonText()" title="" class="btn btn-primary btn-xs">Next ></button>
                  <label style="display: block; margin-top: 10px;">Unified Reverse + Multi-Match Phone Append</label>
                  <p>Appends name & postal address to phone and/or email, and then appends Multi-Match Phone Append.</p>
                  <button id="UnifiedReverseWithMultiMatchPhoneAppend-DynamicAppend" data-bind="click: function (data, event) { submitUsingPredefinedManifest(event); return true; }, text: getButtonText()" title="" class="btn btn-primary btn-xs">Next ></button>
                </div>
            </div>
        </div>
        <div class="panel panel-default">
            <div class="panel-heading" role="tab" id="headingPhoneAppend">
                <h4 class="panel-title">
                    <a role="button" data-toggle="collapse" data-parent="#phoneAppendAccordion" href="#collapsePhoneAppend" aria-expanded="true" aria-controls="collapsePhoneAppend">
                        Phone Append
                    </a>
                </h4>
            </div>
            <div id="collapsePhoneAppend" class="panel-collapse collapse" role="tabpanel" aria-labelledby="headingPhoneAppend">
                <div class="panel-body">
                    <h5>
                        <strong>Consumer Land Line</strong>
                    </h5>
                    <label class="checkbox-inline">
                        <input type="checkbox" id="PHONE_DA" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends da residential phone line to name & postal address"/>
                        DA
                    </label>
                    <label class="checkbox-inline">
                        <input type="checkbox" id="PHONE_PREM" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends premium residential phone line to name & postal address"/>
                        Premium
                    </label>
                   <%-- <label class="checkbox-inline">
                        <input type="checkbox" id="PHONE_STD" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends standard residential phone line to name & postal address"/>
                        Standard
                    </label>--%>
                     <h5>
                        <strong>Consumer Mobile</strong>
                    </h5>
                    <label class="checkbox-inline">
                        <input type="checkbox" id="PHONE_MOB" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends mobile phone line to name & postal address"/>
                        Standard
                    </label>
                    <h5>
                        <strong>Enhanced</strong>
                    </h5>
                    <label class="checkbox-inline">
                        <input type="checkbox" id="PHONE_CCO" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends enhanced residential phone line to name & postal address"/>
                        Land Line Only
                    </label>
                     <label class="checkbox-inline">
                        <input type="checkbox" id="PHONE_CCO_MOBILE" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends enhanced mobile phone line to name & postal address"/>
                        Mobile Only
                    </label>
                    <label class="checkbox" style="font-weight: 400;">
                        <input type="checkbox" id="PHONE_CCO_MIXED" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends enhanced mobile phone line to name & postal address"/>
                        Mixed - Best Match
                    </label>
                    <h5>
                        <strong>Business</strong>
                    </h5>
                    <label class="checkbox-inline">
                        <input type="checkbox" id="PHONE_BUS_DA" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends standard business phone line to name & postal address"/>
                        DA
                    </label>
                    <label class="checkbox-inline">
                        <input type="checkbox" id="PHONE_BUS_PREM" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends premium business phone line to name & postal address"/>
                        Premium
                    </label>
                    <label class="checkbox-inline">
                        <input type="checkbox" id="PHONE_BUS_STD" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends premium business phone line to name & postal address"/>
                        Standard
                    </label>
                    <h5>
                        <strong>Utilities</strong>
                    </h5>
                    <label class="checkbox" style="font-weight: 400;">
                        <input type="checkbox" name="phoneutil" id="DEDUPE_PHONE" data-bind="click: function (data, event) { toggle(event); return true; }" title="Compares the output of all preceding phone append operationNameNames to remove duplicate phone numbers. Returns multiple columns and may output more than one phone number per record."/>
                        Remove Duplicates - Multi column
                    </label>
                    <label class="checkbox" style="font-weight: 400;">
                        <input type="checkbox" name="phoneutil" id="SET_PREF_PHONE" data-bind="click: function (data, event) { toggle(event); return true; }" title="Compares the output of all preceding phone append operationNameNames to remove duplicate phone numbers. Returns multiple columns and may output more than one phone number per record."/>
                        Preference - Multi column
                    </label>
                    <label class="checkbox" style="font-weight: 400;">
                        <input type="checkbox" name="phoneutil" id="SET_PREF_PHONE_SINGLE_COLUMN" data-bind="click: function (data, event) { toggle(event); return true; }" title="Removes duplicates and also returns single column with phone, operation name, match level and quality level."/>
                        Preference - Single column
                    </label>
                    <%--<label class="checkbox" style="font-weight: 400;">
                        <input type="checkbox" name="phoneutilcompare" id="SET_PREF_PHONE_COMPARE_INPUT" data-bind="click: function (data, event) { toggle(event); return true; }" title="Compares output of preceding phone operationNameNames against input phone number and removes any matching output phones."/>
                        Compare Input Phone
                    </label>--%>
                    <%--<label class="checkbox" style="font-weight: 400;">
                        <input type="checkbox" name="phoneutilcompare" id="SET_PREF_BASED_ON_VERIFICATION" data-bind="click: function (data, event) { toggle(event); return true; }" title="Used in conjunction with Verify GetCompleteing Status. Will suppress output of preceding phone operationNameNames based on response from Verify GetCompleteing Status."/>
                        Preference using 'Verify Connection Status (C1-C7)
                    </label>--%>
                </div>
            </div>
        </div>
        <div class="panel panel-default">
            <div class="panel-heading" role="tab" id="headingReversePhoneAppend">
                <h4 class="panel-title">
                    <a role="button" data-toggle="collapse" data-parent="#phoneAppendAccordion" href="#collapseReversePhoneAppend" aria-expanded="true" aria-controls="collapseReversePhoneAppend">
                        Reverse Phone Append
                    </a>
                </h4>
            </div>
            <div id="collapseReversePhoneAppend" class="panel-collapse collapse" role="tabpanel" aria-labelledby="headingReversePhoneAppend">
                <div class="panel-body">
                    <h5>
                        <strong>Consumer</strong>
                    </h5>
                  <label class="checkbox-inline">
                    <input type="checkbox" id="PHONE_REV_CCO" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends first name, last name and postal address to phone number from enhanced phone database."/>
                    Enhanced
                  </label>  
                  <label class="checkbox-inline">
                        <input type="checkbox" id="PHONE_REV_DA" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends first name, last name and postal address to phone number from standard phone database."/>
                        DA
                    </label>
                    <label class="checkbox-inline">
                        <input type="checkbox" id="PHONE_REV_PREM" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends business name or first name, last name and postal address to phone number from premium phone database."/>
                        Premium
                    </label>
                   <%-- <label class="checkbox-inline">
                        <input type="checkbox" id="PHONE_REV_STD" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends first name, last name and postal address to phone number from standard phone database."/>
                        Standard
                    </label>--%>
                  <label class="checkbox-inline">
                    <input type="checkbox" id="PHONE_REV_MOB" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends first name, last name and postal address to phone number from mobile phone database."/>
                    Mobile
                  </label>  
                    <h5>
                    <h5>
                        <strong>Business</strong>
                    </h5>
                    <label class="checkbox-inline">
                        <input type="checkbox" id="PHONE_REV_BUS_DA" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends business name and postal address to phone number from DA phone database."/>
                        DA
                    </label>
                    <label class="checkbox-inline">
                        <input type="checkbox" id="PHONE_REV_BUS_PREM" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends business name and postal address to phone number from premium business phone database."/>
                        Premium
                    </label>
                    <label class="checkbox-inline">
                        <input type="checkbox" id="PHONE_REV_BUS_STD" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends business name and postal address to phone number from standard business phone database."/>
                        Standard
                    </label>
                    <h5>
                        <strong>Utilities</strong>
                    </h5>
                    <label class="checkbox" style="font-weight: 400;">
                        <input type="checkbox" name="phoneReverseUtil" id="SET_PREF_ADDRESS_SINGLE_COLUMN" data-bind="click: function (data, event) { toggle(event); return true; }" title="Removes duplicates and also returns single column with phone, operation name, match level and quality level."/>
                        Preference - Single column
                    </label>
                </div>
            </div>
        </div>
        <div class="panel panel-default">
            <div class="panel-heading" role="tab" id="headingReverseAddressPhoneAppend">
                <h4 class="panel-title">
                    <a role="button" data-toggle="collapse" data-parent="#phoneAppendAccordion" href="#collapseReverseAddressPhoneAppend" aria-expanded="true" aria-controls="collapseReverseAddressPhoneAppend">
                        Reverse Address Phone Append
                    </a>
                </h4>
            </div>
            <div id="collapseReverseAddressPhoneAppend" class="panel-collapse collapse" role="tabpanel" aria-labelledby="headingReverseAddressPhoneAppend">
                <div class="panel-body">
                    <h5>
                        <strong>Consumer</strong>
                    </h5>
                    <label class="checkbox-inline">
                        <input type="checkbox" id="PHONE_ADD_PREM" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends name and phone number to US postal address from DA and premium phone database."/>
                        DA & Premium
                    </label>
                    <label class="checkbox-inline">
                        <input type="checkbox" id="PHONE_ADD_STD" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends name and phone number to US postal address from standard phone database."/>
                        Standard
                    </label>
                    <label class="checkbox-inline">
                        <input type="checkbox" id="PHONE_ADD_MOB" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends name and phone number to US postal address from mobile phone database."/>
                        Mobile
                    </label>
                    <h5>
                        <strong>Business</strong>
                    </h5>
                    <label class="checkbox-inline">
                        <input type="checkbox" id="PHONE_ADD_BUS_DA" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends business name and phone number to US postal address from DA phone database."/>
                        DA
                    </label>
                    <label class="checkbox-inline">
                        <input type="checkbox" id="PHONE_ADD_BUS_PREM" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends business name and phone number to US postal address from premium business phone database."/>
                        Premium
                    </label>
                    <label class="checkbox-inline">
                        <input type="checkbox" id="PHONE_ADD_BUS_STD" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends business name and phone number to US postal address from standard business phone database."/>
                        Standard
                    </label>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="col-md-3">
    <div class="panel panel-default">
        <div class="panel-heading">
            <h3 class="panel-title">Email Append</h3>
        </div>
        <div class="panel-body">
            <div class="checkbox">
                <label>
                    <input type="checkbox" id="EMAIL_BASIC_NO_SUPRESSION_W_VERIFICATION" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends verified email address to name & postal address"/>
                    AccuSend Email<button class="btn btn-default btn-xs" style="margin-left: 5px;" data-bind="click: openSupressionModal">Add Suppression Id</button>
                </label>
            </div>
            <div class="checkbox">
                <label>
                    <input type="checkbox" id="ACCUSEND_W_INPUT_EMAIL_VERIFICATION" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends verified email address to name & postal address after verifying client input email."/>
                    AccuSend Email w/Client Email Verification
                </label>
            </div>
            <div class="checkbox">
                <label>
                    <input type="checkbox" id="EMAIL_BASIC" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends unverified email address to name & postal address"/>
                    Basic Unverified Email
                </label>
            </div>
            <div class="checkbox">
                <label>
                    <input type="checkbox" id="EMAIL_BASIC_REV" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends first name, last name and postal address to phone number from email database."/>
                    Reverse Email Append
                </label>
            </div>
        </div>
    </div>
    <div class="panel panel-default">
        <div class="panel-heading">
            <h3 class="panel-title">Demographic Append</h3>
        </div>
        <div class="panel-body">
            <h5>
                <strong>Date of Birth</strong>
            </h5>
            <label class="checkbox-inline">
                    <input type="checkbox" id="DOB" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends full date of birth to name & postal address"/>
                    Standard
            </label>
            <label class="checkbox-inline">
                    <input type="checkbox" id="DOB_CCO" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends full date of birth to name & postal address"/>
                    Enhanced
            </label>
            <h5>
                <strong>Demographics</strong>
            </h5>
            <label class="checkbox-inline">
                <input type="checkbox" id="DEMOGRAHICS" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends core demographic to name & postal address"/>
                Core
            </label>
            <label class="checkbox-inline">
                <input type="checkbox" id="DEMOGRAPHIC_DONOR" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends donor demographic to name & postal address"/>
                Donor
            </label>
            <label class="checkbox-inline">
                <input type="checkbox" id="DEMOGRAPHIC_INVESTING" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends investing demographic to name & postal address"/>
                Investing
            </label>
            <label class="checkbox-inline">
                <input type="checkbox" id="DEMOGRAPHIC_INTERESTS" data-bind="click: function (data, event) { toggle(event); return true; }" title="Appends interests demographic to name & postal address"/>
                Interests
            </label>
            <h5>
                <strong>Versium Predictive Scores</strong>
            </h5>
            <label class="checkbox-inline">
                <input type="checkbox" id="SCORE_DONOR" data-bind="click: function (data, event) { toggle(event); return true; }" title="Append donor score."/>
                Donor
            </label>
            <label class="checkbox-inline">
                <input type="checkbox" id="SCORE_WEALTH" data-bind="click: function (data, event) { toggle(event); return true; }" title="Append wealth score."/>
                Wealth
            </label>
            <label class="checkbox-inline">
                <input type="checkbox" id="SCORE_GREEN" data-bind="click: function (data, event) { toggle(event); return true; }" title="Append green score."/>
                Green
            </label>

        </div>
    </div>
</div>
<div class="col-md-3">
    <div class="panel panel-default">
        <div class="panel-heading">
            <h3 class="panel-title">Data Hygiene</h3>
        </div>
        <div class="panel-body">
            <h5>
                <strong>Name</strong>
            </h5>
            <label class="checkbox-inline">
                <input type="checkbox" id="NAME" data-bind="click: function (data, event) { toggle(event); return true; }" title="Parses full name into first, middle, last, gender."/>
                Parse Full Name
            </label>
            <label class="checkbox-inline">
                <input type="checkbox" id="NAME_FIRSTLAST" data-bind="click: function (data, event) { toggle(event); return true; }" title="Combines first + last and then parses back into first, middle, last, gender."/>
                Parse First + Last
            </label>
            <h5>
                <strong>Postal Address</strong>
            </h5>
            <label class="checkbox-inline">
                <input type="checkbox" id="CASS" data-bind="click: function (data, event) { toggle(event); return true; }" title="Parses, standardizes and verifies delivery of postal addresses."/>
                Parse
            </label>
            <label class="checkbox-inline">
                <input type="checkbox" id="NCOA48" data-bind="click: function (data, event) { toggle(event); return true; }" title="Parses, standardizes and updates postal addresses using 48 month NCOA."/>
                NCOA
            </label>
            <label class="checkbox-inline">
                <input type="checkbox" id="ADDRESS_UPDATE" data-bind="click: function (data, event) { toggle(event); return true; }" title="Append updated postal addresses using enhanced data."/>
                Address Update
            </label>
            <h5>
                <strong>Phone</strong>
            </h5>
            <label class="checkbox" style="font-weight: normal;">
                <input type="checkbox" id="PHONE" data-bind="click: function (data, event) { toggle(event); return true; }" title="Parses 10 digit US phone number and compares verifies if connected using premium phone database."/>
                Phone Parse & Connection Status (AccuValidate)
            </label>
            <%--<label class="checkbox" style="font-weight: normal;">
                <input type="checkbox" id="PHONE_VER" data-bind="click: function (data, event) { toggle(event); return true; }" title="Compares business name, first, last, postal address and phone number against premium database to determine validity of a listing."/>
                Verify Connection Status (C1-C7)
            </label>--%>
            <h5>
                <strong>Email</strong>
            </h5>
            <label class="checkbox" style="font-weight: normal;">
                <input type="checkbox" id="EMAIL" data-bind="click: function (data, event) { toggle(event); return true; }" title="Parses email address and verifies syntax is valid."/>
                Parse/Verify Syntax (AccuValidate)
            </label>
            <%--<label class="checkbox" style="font-weight: normal;">
                <input type="checkbox" id="EMAIL_VER_SUPRESSION" data-bind="click: function (data, event) { toggle(event); return true; }" title="Compares input email address against internal suppression file to determine if it has previously bounced or been marked as a SPAM complainer."/>
                Internal Suppression
            </label>--%>
            <label class="checkbox-inline" style="font-weight: normal;">
                <input type="checkbox" id="EMAIL_VER_DELIVERABLE" data-bind="click: function (data, event) { toggle(event); return true; }" title="Verifies email address with ISP to determine delivery status."/>
                xVerify / ImpWise
            </label>
            <label class="checkbox-inline" style="font-weight: normal;">
                <input type="checkbox" id="SET_PREF_EMAIL_VER" data-bind="click: function (data, event) { toggle(event); return true; }" title="Identifies SPAM complainers and mole accounts."/>
                Single Column Output
            </label>
        </div>
    </div>
</div>
<div class="col-md-3">
    <% 
        if (Model.Instructions != null) { %>
    <div class="panel panel-default" id="dealInstructionsPanel">
        <div class="panel-heading">
            <h3 class="panel-title">Deal Instructions</h3>
        </div>
        <div class="panel-body">
            <pre id="dealInstructions"><%: Model.Instructions %></pre>
        </div>
    </div>
    <% } %>
    <div class="panel panel-default">
        <div class="panel-heading">
            <h3 class="panel-title">Pre-built Products</h3>
        </div>
        <div class="panel-body">
            <label style="display: block;">Email Change of Address</label>
            <p>Verifies input email and appends new email where the input email is undeliverable. Returns input email delivery status code and new email address.</p>
            <button id="eCOA-DynamicAppend" data-bind="click: function (data, event) { submitUsingPredefinedManifest(event); return true; }, text: getButtonText()" title="" class="btn btn-primary btn-xs">Next ></button>
            <label style="display: block; margin-top: 10px;">Unified Reverse</label>
            <p>Appends postal address using phone and then email address. Returns single postal address column with OperationName and QualityLevel</p>
            <button id="UnifiedReversePhoneEmail-DynamicAppend" data-bind="click: function (data, event) { submitUsingPredefinedManifest(event); return true; }, text: getButtonText()" title="" class="btn btn-primary btn-xs">Next ></button>
        </div>
    </div>
</div>
</div>

<div class="row">
    <div class="col-md-12">
        <button class="btn btn-primary" data-bind="visible: manifest.operations().length > 0, text: getButtonText(), click: nextClick" style="margin-bottom: 20px; display: none;">Next ></button>
    </div>
</div>

<div class="row">
    <div class="col-md-12">
        <div id="alert" style="display: none;"></div>
    </div>
</div>

<div class="row" data-bind="visible: manifest.inputfieldnames.length > 0">
    <div class="col-md-12" data-bind="template: { name: 'tplManifestInputFieldNames' }">
    </div>
</div>

<div class="row" data-bind="visible: manifest.outputfields.length > 0">
    <div class="col-md-12" data-bind="template: { name: 'tplManifestOutputFields' }">
    </div>
</div>

<div class="row">
    <div class="col-md-12">
        <span data-bind="template: { name: 'tplOperation', foreach: manifest.operations }"></span>    
    </div>
</div>

<%: Html.Hidden("Category", ViewData["Category"]) %>

<div class="row">
    <div class="col-md-12">
        <button class="btn btn-primary" data-bind="visible: manifest.operations().length > 0, text: getButtonText(), click: nextClick" style="margin-bottom: 20px; display: none;">Next ></button>
    </div>
</div>

<script id="tplOperation" type="text/html">
    <div class="panel panel-default">
        <div class="panel-heading">
            <span data-bind="text: ($index()+1) + ':' + title + ' (' + description + ')'"></span>
            <%--<a href="#" class="pull-right" data-bind="click: $root.manifest.remove(name)">Remove</a>--%>
        </div>
        <div class="panel-body">
            <div class="col-md-6">
                <span data-bind="template: { name: 'tplMatchLevels' }, visible: matchlevels.length > 0"></span>
                <span data-bind="template: { name: 'tplQualityLevels' }, visible: qualitylevels.length > 0"></span>
                <span data-bind="template: { name: 'tplSources' }, visible: sources.length > 0"></span>
            </div>
            <div class="col-md-3">
                <span data-bind="template: { name: 'tplInputFields' }"></span>
            </div>
            <div class="col-md-3">
                <span data-bind="template: { name: 'tplOutputFields' }"></span>
            </div>
        </div>
    </div>
</script>

<script id="tplOutputFields" type="text/html">
    <p data-bind="visible: outputfields.length > 0">
        <strong>Output Fields</strong>
    </p>
    <div data-bind="foreach: outputfields">
        <div class="input-group" style="margin-bottom: 7px;">
            <span class="input-group-addon">
                    <input type="checkbox" data-bind="checked: include">
                </span>
            <input type="text" class="form-control" data-bind="value: columntitle">
            <input type="hidden" data-bind="value: operationparamname"/>
            <input type="hidden" data-bind="value: required"/>
        </div>
    </div>
</script>

<script id="tplInputFields" type="text/html">
    <p>
        <strong>Input Fields</strong>
    </p>
    <div data-bind="foreach: inputfields">
        <div data-bind="text: operationparamname"></div>
    </div>
</script>

<script id="tplMatchLevels" type="text/html">
    <p class="category-label">
        <strong>Match Levels </strong><span style="font-size: .8em;">(IND = E1,E2,N2,B2,B4 HH = N1,B1)</span>
    </p>
    <%--<p><input data-bind="attr: { value: name }, checked: include" type="checkbox" style="margin-right: 5px;"/>Simple match levels</p>--%>
    <ul class="list-inline" data-bind="foreach: matchlevels">
        <li>
            <input data-bind="attr: { value: name }, checked: include" type="checkbox" style="margin-right: 5px;"/><span data-bind="text: name"></span></li>
    </ul>
</script>

<script id="tplQualityLevels" type="text/html">
    <p class="category-label">
        <strong>Quality Levels</strong>
    </p>
    <ul class="list-inline" data-bind="foreach: qualitylevels">
        <li>
            <input data-bind="attr: { value: name }, checked: include" type="checkbox" style="margin-right: 5px;"/><span data-bind="text: name"></span></li>
    </ul>
</script>
    
<script id="tplSources" type="text/html">
    <p class="category-label">
        <strong>Sources</strong>
    </p>
    <ul class="list-inline" data-bind="foreach: sources">
        <li>
            <input data-bind="attr: { value: name }, checked: include" type="checkbox" style="margin-right: 5px;"/><span data-bind="text: name"></span></li>
    </ul>
</script>

<script id="tplManifestInputFieldNames" type="text/html">
    <div class="panel panel-default">
        <div class="panel-heading">Required Input Fields</div>
        <div class="panel-body">
            <ul data-bind="foreach: manifest.inputfieldnames" class="list-inline">
                <li data-bind="text: $data" style="border: solid 1px silver; padding: 6px 12px; border-radius: 5px;"></li>
            </ul>
        </div>
    </div>
</script>

<script id="tplManifestOutputFields" type="text/html">
    <div class="panel panel-default">
        <div class="panel-heading">Output Fields</div>
        <div class="panel-body">
            <ul data-bind="foreach: manifest.outputfields" class="list-inline">
                <li style="padding-left: 0;">
                    <span class="input-group" style="width: 175px;">
                        <span class="input-group-addon">
                            <input type="checkbox" data-bind="checked: include">
                        </span>
                        <input type="text" class="form-control" data-bind="value: columntitle, style: { color: color }">
                    </span>
                </li>
            </ul>
        </div>
    </div>
</script>

<!-- Add suppression id -->
<div class="modal fade" tabindex="-1" role="dialog" id="addSuppressionIdModel">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header" style="background-color: #f0f0f0;">
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span></button>
                <h4 class="modal-title">Add Suppression Id</h4>
            </div>
            <div class="modal-body" style="padding: 25px;">
                <div class="alert alert-danger" style="display: none;" id="addSuppressionIdModelAlert"></div>
                <input id="txtSupressionId" type="text" class="form-control"/>
            </div>
            <div class="modal-footer" style="background-color: #f0f0f0;">
                <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                <button type="button" class="btn btn-primary" onclick="buildManifestViewModel.setSupressionId()">Save Changes</button>
            </div>
        </div>
    </div>
</div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

    <script src="//cdnjs.cloudflare.com/ajax/libs/underscore.js/1.8.3/underscore-min.js" type="text/javascript"></script>
    <script src="<%= Url.Content("~/scripts/knockout-3.4.2.js") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/scripts/AccurateAppend.Enums.js") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/scripts/AccurateAppend.DynamicAppend.Objects.js") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/scripts/AccurateAppend.DynamicAppend.Strategies.js") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/scripts/AccurateAppend.DynamicAppend.BuildManifest.js") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jquery.getParams.js") %>" type="text/javascript"></script>

</asp:Content>