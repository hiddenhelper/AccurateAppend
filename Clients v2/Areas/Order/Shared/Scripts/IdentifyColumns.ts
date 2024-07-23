var requiredFields: any;
declare var orderModel: any;
var FieldName_Unknown: string;
declare let jcf: any;

$(document).ready(
  () => {

    $("select.ct").bind("change",
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
    
    $("a[name=next]").bind("click",
      () => {
        submit();
      });

    initialize();

    updateRequiredFieldsDisplay();
  }
);

// populate with options from when page is first initialized
var globalOptions = new Array();

function submit() {
  if (validate()) {
    var columnMap = "";
    $.each($("#mappings select"),
      (i, v) => {
        if (v.value === "") {
          columnMap += FieldName_Unknown + ";";
        } else {
          columnMap += v.value + ";";
        }
      });
    
    orderModel.order.columnMap = columnMap;

    console.log(orderModel.order.columnMap);

    $("<input>").attr({
      type: "hidden",
      name: "orderModel",
      value: ko.toJSON(orderModel.order)
      }).appendTo("form").first();



    $("form").first().submit();
    return true;
  }
  return false;
}

function updateRequiredFieldsDisplay() {
  // identify required fields that are still in the global options, these fields still need to be mapped
  const remainingRequiredFields = $.grep(requiredFields, f => $.inArray(f, globalOptions) > -1);
  if (remainingRequiredFields.length === 0) {
    //$("#alert").removeClass().addClass("alert alert-success").text("All required fields have been mapped.").show();
    //$("button[name=next]").prop("disabled", false);
    //submit();
  } else {
      $("#alert").removeClass().addClass("alert alert-danger").text(
      `Please identify the following columns in your file in the form below: ${requiredFieldsToReadable(requiredFields)}.`).show();
    $("button[name=next]").prop("disabled", true);
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
}

function arrayIntersect(a, b) {
  return $.grep(a, i => $.inArray(i, b) > -1);
};

// checks remaining fields against required fields
function validate() {
  if (arrayIntersect(globalOptions, requiredFields).length > 0) {
      $("#alert").removeClass().addClass("alert alert-danger").text(
          `Please identify the following columns in your file in the form below: ${requiredFieldsToReadable(requiredFields)}.`).show();
    return false;
  }
  return true;
}


//////////////////////////////////////////////////////////////////////////////////////////////
// control helpers
//////////////////////////////////////////////////////////////////////////////////////////////

// sets select to chosen option
function set(c) {
  console.log("setSelect()");
  // 1. remove option from global
  removeGlobalOption($(c).val());
  // 2. remove all options except this one from this select
  const selectedOption = $(c).find(":selected");

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
  $(c).addClass("selected");

}

// restores select when "Reset" option is choosen
function reset(c) {
  // 1. add option back into global
  const selectedOption = $(c).find("[value!='Reset']").val();
  addGlobalOption(selectedOption);
  // reset options for this select
  resetOptionsForSelect(c);
  $(c).removeClass("selected");

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
  $(c).find("option").remove();
  // 3. add global options back into select
    $(c).append(buildGlobalOptions());
    jcf.replaceAll();
}

// resets all unSelected selects to global options
function resetOptionsForUnSelected() {
  $(getUnselected()).each(function() {
      resetOptionsForSelect(this);
      jcf.replaceAll();
  });
}

//////////////////////////////////////////////////////////////////////////////////////////////
// global options helpers
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
  console.log(`Building option for '${value}'`);
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
  globalOptions = jQuery.grep(globalOptions, toRemove => toRemove !== value);
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
  requiredFields = jQuery.grep(requiredFields, toRemove => toRemove !== value);
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

function requiredFieldsToReadable(requiredFields: Array<string>) {
    var readable = "";
    $.each(requiredFields, (i, v) => {
        readable += getDisplayTextForField(v) + (i < requiredFields.length - 1 ? ", " : "");
    });
  return readable.trim();
}

function getDisplayTextForField(field: string) {
  switch (field) {
      case "FirstName":
          return "First Name";
      case "LastName":
          return "Last Name";
      case "StreetAddress":
          return "Street Address";
      case "PostalCode":
          return "Postal Code";
    }
  return field;
}