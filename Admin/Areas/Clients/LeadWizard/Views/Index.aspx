<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="ViewPage<LeadWizardViewModel>" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Clients.LeadWizard.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Lead Wizard
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

  <script src="<%= Url.Content("~/Areas/Clients/LeadWizard/Scripts/LeadWizard.js") %>" type="text/javascript"></script>

  <script type="text/javascript">

    $(function() {


    });

  </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div class="row" style="padding: 0 20px 0 20px;">

<form>

<section id="salesWizard">
  


<div class="row">

<div class="col-md-4">
  
  <div class="panel panel-default" id="panelContactDetails">
    <div class="panel-heading">Contact Details </div>
    <div class="panel-body">
      <div class="form-group">
        <label for="name">Name</label>
        <input type="email" class="form-control" id="name">
      </div>
      <div class="form-group">
        <label for="businessName">Business Name</label>
        <input type="email" class="form-control" id="businessName">
      </div>
      <div class="form-group">
        <label for="emailAddress">Email address</label>
        <input type="email" class="form-control" id="emailAddress">
      </div>
      <div class="form-group">
        <label for="phone">Phone</label>
        <input type="email" class="form-control" id="phone">
      </div>
      <%--<button type="submit" class="btn btn-default">Submit</button>--%>
    </div>
  </div>

  <!-- step 2: natureOfRequest -->
  <div class="panel panel-default" id="panelNatureOfRequest">
    <div class="panel-heading">Is this a one-time job or do you have an ongoing need? </div>
    <div class="panel-body">
      <div class="radio">
        <label>
          <input type="radio" name="natureOfRequest" value="option1">
          Doing Research
        </label>
      </div>
      <div class="radio">
        <label>
          <input type="radio" name="natureOfRequest" value="option2">
          One-time
        </label>
      </div>
      <div class="radio">
        <label>
          <input type="radio" name="natureOfRequest" value="option3">
          Ongoing Need
        </label>
      </div>
      <div class="radio">
        <label>
          <input type="radio" name="natureOfRequest" value="option4">
          Other
        </label>
      </div>
    </div>
  </div>

  <div class="panel panel-default" id="panelHasFile">
    <div class="panel-heading">Do you have a file?</div>
    <div class="panel-body">
      <div class="radio">
        <label>
          <input type="radio" name="hasFile" value="yes">
          Yes
        </label>
      </div>
      <div class="radio">
        <label>
          <input type="radio" name="hasFile" value="no">
          No
        </label>
      </div>
      <div class="form-group" id="panelRecordCount">
        <label>Estimated Record count</label>
        <input type="email" class="form-control" name="recordCount">
      </div>
    </div>
  </div>

  <div class="panel panel-default" id="panelTimeFrame">
    <div class="panel-heading">What is your time frame?</div>
    <div class="panel-body">
      <div class="radio">
        <label>
          <input type="radio" name="timeFrame">
          Today
        </label>
      </div>
      <div class="radio">
        <label>
          <input type="radio" name="timeFrame">
          Week
        </label>
      </div>
      <div class="radio">
        <label>
          <input type="radio" name="timeFrame">
          Month
        </label>
      </div>
      <div class="radio">
        <label>
          <input type="radio" name="timeFrame">
          Not sure
        </label>
      </div>
    </div>
  </div>

  <div class="panel panel-default" id="panelProductInterest">
    <div class="panel-heading">What products are you interested in?</div>
    <div class="panel-body">
      <div class="checkbox">
        <label>
          <input type="checkbox" name="productInterest" value="phoneAppend">
          Phone Append
        </label>
      </div>
      <div class="checkbox">
        <label>
          <input type="checkbox" name="productInterest" value="emailAppend">
          Email Append
        </label>
      </div>
      <div class="checkbox">
        <label>
          <input type="checkbox" name="productInterest" value="demographicAppend">
          Demographic Append
        </label>
      </div>
      <div class="checkbox">
        <label>
          <input type="checkbox" name="productInterest" value="other">
          Other
        </label>
      </div>
    </div>
  </div>

</div>

<div class="col-md-8">
  
  <section id="productInterestDetails" style="display: none;">
    
     <!-- Nav tabs -->
  <ul class="nav nav-tabs" role="tablist">
    <li role="presentation" class="active">
      <a href="#emailAppend" aria-controls="emailAppend" role="tab" data-toggle="tab">Email Append</a>
    </li>
    <li role="presentation">
      <a href="#phoneAppend" aria-controls="phoneAppend" role="tab" data-toggle="tab">Phone Append</a>
    </li>
    <li role="presentation">
      <a href="#demographicAppend" aria-controls="demographicAppend" role="tab" data-toggle="tab">Demographic Append</a>
    </li>
    <%--<li role="presentation"><a href="#settings" aria-controls="settings" role="tab" data-toggle="tab">Settings</a></li>--%>
  </ul>

  <!-- Tab panes -->
  <div class="tab-content">
    <div role="tabpanel" class="tab-pane active" id="emailAppend" style="padding: 20px 10px;">
      <p style="font-weight: bold;">How much does it cost?</p>
      <p>Pricing varies depending on the number of records in your job and is based on the number of records for which we find matches. Cost starts at 15 cents per verified email appended.</p>
      <p style="font-weight: bold;">How many matches will you find?</p>
      <p>Our average match rate is 45%.</p>
      <p style="font-weight: bold;">Estimated costs</p>
      <table class="table table-bordered">
        <thead>
        <tr>
          <th>Records</th>
          <th>Matches</th>
          <th>Cost per Match</th>
          <th>Estimated Cost</th>
        </tr>
        </thead>
        <tbody>
        <tr>
          <td>500</td>
          <td>225</td>
          <td>15 Cents</td>
          <td>$75</td>
        </tr>
        <tr>
          <td>2,500</td>
          <td>1125</td>
          <td>15 Cents</td>
          <td>$170</td>
        </tr>
        <tr>
          <td>10,000</td>
          <td>4,500</td>
          <td>15 Cents</td>
          <td>$675</td>
        </tr>
        <tr>
          <td>25,000</td>
          <td>11,250</td>
          <td>15 Cents</td>
          <td>$1,600</td>
        </tr>
        </tbody>
      </table>
      <p style="font-weight: bold;">How long does it take?</p>
      <p>Most files are processed during the same day they are submitted.</p>
      <p style="font-weight: bold;">Can Accurate Append verify whether my existing email addresses are good?</p>
      <p>Yes, we can determine the real-time delivery status for your existing email addresses. We can even append a new verified email address to records whose email was verified as undeliverable.</p>
      <p>
        <a href="https://www.accurateappend.com/email-append" target="_blank"></a><a href="https://www.accurateappend.com/phone-append">https://www.accurateappend.com/email-append</a>
      </p>
    </div>
    <div role="tabpanel" class="tab-pane" id="phoneAppend" style="padding: 20px 10px;">
      <p style="font-weight: bold;">How much does it cost?</P>
      <p>Pricing varies depending on the number of records in your job and is based on the number of records for which we find matches. Cost starts at 10 cents per phone number appended.</p>
      <p style="font-weight: bold;">How many matches will you find?</p>
      <p>Our average match rate is 75%.</p>
      <p style="font-weight: bold;">Estimated costs</p>
      <p>Multi-match Phone Append. Includes two landline source plus mobile.</p>

      <table class="table table-bordered">
        <thead>
        <tr>
          <th>Records</th>
          <th>Matches</th>
          <th>Cost per Match</th>
          <th>Estimated Cost</th>
        </tr>
        </thead>
        <tbody>
        <tr>
          <td>500</td>
          <td>375</td>
          <td>10 Cents</td>
          <td>$75</td>
        </tr>
        <tr>
          <td>2,500</td>
          <td>1,875</td>
          <td>10 Cents</td>
          <td>$187</td>
        </tr>
        <tr>
          <td>10,000</td>
          <td>7,500</td>
          <td>10 Cents</td>
          <td>$750</td>
        </tr>
        <tr>
          <td>25,000</td>
          <td>18,750</td>
          <td>10 Cents</td>
          <td>$1,870</td>
        </tr>
        </tbody>
      </table>

      <p style="font-weight: bold;">How long does it take?</p>
      <p>Most files are processed during the same day they are submitted.</p>
      <p>
        <a href="https://www.accurateappend.com/phone-append" target="_blank"></a><a href="https://www.accurateappend.com/phone-append">https://www.accurateappend.com/phone-append</a>
      </p>
    </div>
    <div role="tabpanel" class="tab-pane" id="demographicAppend" style="padding: 20px 10px;">
      <p style="font-weight: bold;">How much does it cost?</p>
      <p>Pricing varies depending on the number of records in your job and is based on the number of records for which we find matches. Cost starts at 10 cents per record appended.</p>
      <p style="font-weight: bold;">How many matches will you find?</p>
      <p>Our average match rate is 35%.</p>
      <p style="font-weight: bold;">Estimated costs</p>
      <p>Multi-match Phone Append. Includes two landline source plus mobile.</p>

      <table class="table table-bordered">
        <thead>
        <tr>
          <th>Records</th>
          <th>Matches</th>
          <th>Cost per Match</th>
          <th>Estimated Cost</th>
        </tr>
        </thead>
        <tbody>
        <tr>
          <td>500</td>
          <td>175</td>
          <td>10 Cents</td>
          <td>$75</td>
        </tr>
        <tr>
          <td>2,500</td>
          <td>875</td>
          <td>10 Cents</td>
          <td>$87</td>
        </tr>
        <tr>
          <td>10,000</td>
          <td>3,500</td>
          <td>10 Cents</td>
          <td>$350</td>
        </tr>
        <tr>
          <td>25,000</td>
          <td>8,750</td>
          <td>10 Cents</td>
          <td>$870</td>
        </tr>
        </tbody>
      </table>

      <p style="font-weight: bold;">How long does it take?</p>
      <p>Most files are processed during the same day they are submitted.</p>
      <p style="font-weight: bold;">What information is available?</p>
      <ul>
        <li>Age</li>
        <li>Gender</li>
        <li>Length of Residence</li>
        <li>Year in School</li>
        <li>Estimated Income</li>
        <li>Home Owner</li>
        <li>Ethnic Group</li>
        <li>Marital</li>
        <li>Med Home Value</li>
        <li>Estimated Wealth</li>
        <li>Children Present</li>
        <li>Head of Household Rank</li>
      </ul>

      <p>
        <a href="https://www.accurateappend.com/demographic-append">https://www.accurateappend.com/demographic-append</a>
      </p>
    </div>
    <%--<div role="tabpanel" class="tab-pane" id="settings">...</div>--%>
  </div>

  </section>

 

</div>

<button type="submit" class="btn btn-default">Submit</button>

</div>

</section>

</form>

</div>

</asp:Content>