<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<AccurateAppend.Websites.Admin.Entities.BatchJobRequest>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Identify Columns
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

  <script type="text/javascript">

    var requiredFields;

    $(document).ready(
      function() {

        $('select.ct').bind('change',
          function() { // reset option was chosen
            $("#message").hide();
            // reset option selected
            if ($(this).val() === "Reset") {
              console.log("Reseting control");
              reset(this);
              updateRequiredFieldsDisplay();
              return;
            }
            // field option selected
            set(this);
            updateRequiredFieldsDisplay();
          });

        $("button[name=next]").bind('click',
          function() {
            if (validate()) {
              $('<input>').attr({
                type: 'hidden',
                name: 'HasHeaderRow'
              }).val($("#HasHeaderRow").val()).appendTo('#pf');
              $("#pf").submit();
            }
          });

        initialize();

        updateRequiredFieldsDisplay();
      }
    );

    // populate with options from when page is first initialized
    var globalOptions = new Array();

    function updateRequiredFieldsDisplay() {
      // identify required fields that are still in the global options, these fields still need to be mapped
      var remainingRequiredFields = $.grep(requiredFields,
        function(f) {
          return $.inArray(f, globalOptions) > -1;
        });
      if (remainingRequiredFields.length === 0) {
        $('#requiredFieldsSection').hide();
        $('#requiredFieldsMessage').text('All required fields have been mapped.').show();
        $("button[name=next]").show();
      } else {
        $('#requiredFieldsSection').show();
        $('#requiredFieldsMessage').hide();
        $("#requiredFieldsDisplayed").text(remainingRequiredFields.toString());
        $("button[name=next]").hide();
      }
    }

    function initialize() {

      // initialize global from existing selects
      $("select.ct:first").find("option").each(function() {
        addGlobalOption($(this).val());
      });

      // if manifest calls for mapping first and last name then give the option to map FullName
      if ($.inArray("FirstName", globalOptions) >= 0 && $.inArray("LastName", globalOptions) >= 0) {
        addGlobalOption("FullName");
        resetOptionsForUnSelected();
      }

      requiredFields = <%= ViewData["RequiredFields"] %>;

    }

    // checks remaining fields against required fields
    function validate() {
      if ($.arrayIntersect(globalOptions, requiredFields).length > 0) {
        $("#message").text('Required fields have not been mapped. ' +
          requiredFields.toString() +
          ' ' +
          (requiredFields.length === 1 ? 'is' : 'are') +
          ' required. Please continue map to proceed.').show();
        return false;
      } else
        return true;
    }

    $.arrayIntersect = function(a, b) {
      return $.grep(a,
        function(i) {
          return $.inArray(i, b) > -1;
        });
    };

    //////////////////////////////////////////////////////////////////////////////////////////////
    // control helpers
    //////////////////////////////////////////////////////////////////////////////////////////////

    // sets select to choosen option
    function set(c) {
      console.log("setSelect()");
      // 1. remove option from global
      removeGlobalOption($(c).val());
      // 2. remove all options except this one from this select
      var selectedOption = $(c).find(":selected");

      // name
      if (selectedOption.val() === "FirstName" || selectedOption.val() === "LastName") {
        removeGlobalOption("FullName");
      } else if (selectedOption.val() === "FullName") {
        removeGlobalOption("FirstName");
        removeGlobalOption("LastName");
      }
      // PostalCode selected then make City & State optional
      else if (selectedOption.val() === "PostalCode") {
        removeRequiredOption("City");
        removeRequiredOption("State");
      }
      // if City or State selected then make PostalCode optional if both have been selected
      else if (selectedOption.val() === "City") {
        // if State has been mapped then remove PostalCode
        if (!containsGlobalOption("State")) removeRequiredOption("PostalCode");
      } else if (selectedOption.val() === "State") {
        // if City has been mapped then remove PostalCode
        if (!containsGlobalOption("City")) removeRequiredOption("PostalCode");
      }

      $(c).find("option").remove();
      $(c).append(selectedOption);
      $(c).append(buildOptionTag("Reset"));
      // 3. repopulate options in Unselected
      $.each(getUnselected(),
        function() {
          console.log("Looping through UnSelected controls");
          $(this).find("option").remove();
          $(this).append(buildGlobalOptions());
        });
      $(c).addClass('selected');

    }

    // restores select when "Reset" option is choosen
    function reset(c) {
      // 1. add option back into global
      var selectedOption = $(c).find("[value!='Reset']").val();
      addGlobalOption(selectedOption);
      // reset options for this select
      resetOptionsForSelect(c);
      $(c).removeClass('selected');

      // if manifest calls for StreetAddress, City, State, PostalCode
      // if City and State are mapped then make PostalCode optional
      // if PostalCode is mapped then make City and State optional

      switch (selectedOption) {
      case "FullName":
        addGlobalOption("FirstName");
        addGlobalOption("LastName");
        break;
      case "FirstName":
        if (containsGlobalOption("LastName"))
          addGlobalOption("FullName");
        break;
      case "LastName":
        if (containsGlobalOption("FirstName"))
          addGlobalOption("FullName");
        break;
      case "PostalCode":
        if (!containsRequiredOption("State"))
          addRequiredOption("State");
        if (!containsRequiredOption("City"))
          addRequiredOption("City");
        break;
      case "State":
      case "City":
        if (containsGlobalOption("PostalCode"))
          addRequiredOption("PostalCode");
        break;
      }

      // reset options for other selects
      resetOptionsForUnSelected();
    }

    // returns collection of selects where selected value = ''
    function getUnselected() {
      var arr = [];
      $("select.ct").each(function() {
        if ($(this).find("option:selected[value='']").length > 0) {
          arr.push(this);
        }
      });
      return arr;
    }

    // returns collection of selects where selected value != ''
    function getSelectedValues() {
      var arr = [];
      $("select.ct").each(function() {
        if ($(this).find("option:selected[value!=='']").length > 0) {
          arr.push($(this).val());
        }
      });
      return arr;
    }

    // resets options using global
    function resetOptionsForSelect(c) {
      $(c).find('option').remove();
      // 3. add global options back into select
      $(c).append(buildGlobalOptions());
    }

    // resets all unSelected selects to global options
    function resetOptionsForUnSelected() {
      $(getUnselected()).each(function() {
        resetOptionsForSelect(this);
      });
    }

    //////////////////////////////////////////////////////////////////////////////////////////////
    // gloabl options helpers
    //////////////////////////////////////////////////////////////////////////////////////////////

    // builds option element collection for use with select
    function buildGlobalOptions() {
      console.log("Building global options");
      var arr = new Array();
      $.each(globalOptions,
        function() {
          arr.push(buildOptionTag(this));
        });
      return arr;
    }

    // builds option element
    function buildOptionTag(value) {
      console.log("Building option for '" + value + "'");
      if (value.toString() === "") {
        console.log("Building empty option");
        return $("<option></option>").attr("value", "").text("-- Select Column -");
      } else {
        return $("<option></option>").attr("value", value).text(value);
      }
    }

    // adds option to Global and ensures the collection stays unique
    function addGlobalOption(value) {
      var isPresent = false;
      $.each(globalOptions,
        function() {
          if (this.toString() === value)
            isPresent = true;
        });
      if (isPresent) return;
      globalOptions.push(value);
    }

    // removes option from Global options
    function removeGlobalOption(value) {
      globalOptions = jQuery.grep(globalOptions,
        function(toRemove) {
          return toRemove !== value;
        });
    }

    // adds option to Required
    function addRequiredOption(value) {
      var isPresent = false;
      $.each(requiredFields,
        function() {
          if (this.toString() === value)
            isPresent = true;
        });
      if (isPresent) return;
      requiredFields.push(value);
    }

    // removes option from Required options
    function removeRequiredOption(value) {
      requiredFields = jQuery.grep(requiredFields,
        function(toRemove) {
          return toRemove !== value;
        });
    }

    // determines if contained in Global
    function containsGlobalOption(value) {
      var present = false;
      $.each(globalOptions,
        function() {
          if (this.toString() === value)
            present = true;
        });
      return present;
    }

    // determines if contained in Global
    function containsRequiredOption(value) {
      var present = false;
      $.each(requiredFields,
        function() {
          if (this.toString() === value)
            present = true;
        });
      return present;
    }

  </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <div class="row" style="padding-bottom: 20px;">
    <div class="col-md-3">
      <h3 style="margin-top: 0">Identify Columns</h3>
    </div>
    <div class="col-md-9" style="text-align: right;">
      <% Html.RenderPartial("FileDisplay", Model.InputFile); %>
    </div>
  </div>
  <div class="col-md-12" style="margin-bottom: 20px;">
    <div class="alert alert-danger" style="display: none;" id="message"></div>
    <button class="btn btn-primary" name="next" style="margin-bottom: 10px;">Next</button>
    <div class="checkbox" style="margin-bottom: 15px;">
      <label><%: Html.CheckBoxFor(a => a.HasHeaderRow, null) %>Header row is present</label>
    </div>
    <div class="alert alert-info" style="display: none;" id="requiredFieldsMessage"></div>
    <div id="requiredFieldsSection">
      <span style="margin-right: 5px;">Required fields that still need to be mapped:</span><span style="font-weight: bold;" id="requiredFieldsDisplayed"></span>
    </div>

  </div>
  <div class="col-md-12">
    <form id="pf" method="post" action="">

      <%
        var columncount = int.Parse(ViewData["ColCount"].ToString());
        var tablerows = Convert.ToInt32(Math.Floor(columncount / 5 + 1.0));

        Response.Write("<table class=\"table\">");
        for (var s = 1; s <= tablerows; s++)
        {
          //  1   2   3   4   5
          //  6   7   8   9   10
          //  11  12  13  14  15
          //  16  17  18  19  20

          for (var i = 1; i <= 5; i++)
          {
            Response.Write("<td style='width: 20%; background-color: #f0f0f0;'>");
            var viewdataponiter = s == 1 ? i : s == 2 ? i + 5 : 5 * (s - 1) + i;
            if (viewdataponiter <= columncount)
            {
              Response.Write("Column " + viewdataponiter + "<select name='" + viewdataponiter + "' class='form-control ct'>");
              foreach (var option in (string[]) ViewData["InputColumns"])
                Response.Write(option);
              Response.Write("</select>");
            }
            Response.Write("</td>");
          }
          Response.Write("</tr>");

          Response.Write("<tr>");
          for (var i = 1; i <= 5; i++)
          {
            Response.Write("<td>");
            var viewdataponiter = s == 1 ? i : s == 2 ? i + 5 : 5 * (s - 1) + i;
            if (viewdataponiter <= columncount)
              foreach (var sample in (List<string>) ViewData[viewdataponiter.ToString()])
                Response.Write("<p style='margin-bottom:0'>" + sample + "</p>");
            Response.Write("</td>");
          }
          Response.Write("</tr>");
        }
        Response.Write("</table>");
      %>
      <%= Html.Hidden("colcount", columncount) %>
    </form>
  </div>
  <div class="col-md-12">
    <button class="btn btn-primary" name="next">Next</button>
  </div>
</asp:Content>