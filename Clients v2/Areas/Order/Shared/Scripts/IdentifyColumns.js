var requiredFields;
var FieldName_Unknown;
$(document).ready(function () {
    $("select.ct").bind("change", function () {
        $("#message").hide();
        if ($(this).val() === "Reset") {
            console.log("Reseting control");
            reset(this);
            updateRequiredFieldsDisplay();
            return;
        }
        set(this);
        updateRequiredFieldsDisplay();
    });
    $("a[name=next]").bind("click", function () {
        submit();
    });
    initialize();
    updateRequiredFieldsDisplay();
});
var globalOptions = new Array();
function submit() {
    if (validate()) {
        var columnMap = "";
        $.each($("#mappings select"), function (i, v) {
            if (v.value === "") {
                columnMap += FieldName_Unknown + ";";
            }
            else {
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
    var remainingRequiredFields = $.grep(requiredFields, function (f) { return $.inArray(f, globalOptions) > -1; });
    if (remainingRequiredFields.length === 0) {
    }
    else {
        $("#alert").removeClass().addClass("alert alert-danger").text("Please identify the following columns in your file in the form below: " + requiredFieldsToReadable(requiredFields) + ".").show();
        $("button[name=next]").prop("disabled", true);
    }
}
function initialize() {
    $("select.ct:first").find("option").each(function () {
        addGlobalOption($(this).val());
    });
    if ($.inArray("FirstName", globalOptions) >= 0 && $.inArray("LastName", globalOptions) >= 0) {
        addGlobalOption("FullName");
        resetOptionsForUnSelected();
    }
}
function arrayIntersect(a, b) {
    return $.grep(a, function (i) { return $.inArray(i, b) > -1; });
}
;
function validate() {
    if (arrayIntersect(globalOptions, requiredFields).length > 0) {
        $("#alert").removeClass().addClass("alert alert-danger").text("Please identify the following columns in your file in the form below: " + requiredFieldsToReadable(requiredFields) + ".").show();
        return false;
    }
    return true;
}
function set(c) {
    console.log("setSelect()");
    removeGlobalOption($(c).val());
    var selectedOption = $(c).find(":selected");
    if (selectedOption.val() === "FirstName" || selectedOption.val() === "LastName") {
        removeGlobalOption("FullName");
    }
    else if (selectedOption.val() === "FullName") {
        removeGlobalOption("FirstName");
        removeGlobalOption("LastName");
    }
    else if (selectedOption.val() === "PostalCode") {
        removeRequiredOption("City");
        removeRequiredOption("State");
    }
    else if (selectedOption.val() === "City") {
        if (!containsGlobalOption("State"))
            removeRequiredOption("PostalCode");
    }
    else if (selectedOption.val() === "State") {
        if (!containsGlobalOption("City"))
            removeRequiredOption("PostalCode");
    }
    $(c).find("option").remove();
    $(c).append(selectedOption);
    $(c).append(buildOptionTag("Reset"));
    $.each(getUnselected(), function () {
        console.log("Looping through UnSelected controls");
        $(this).find("option").remove();
        $(this).append(buildGlobalOptions());
    });
    $(c).addClass("selected");
}
function reset(c) {
    var selectedOption = $(c).find("[value!='Reset']").val();
    addGlobalOption(selectedOption);
    resetOptionsForSelect(c);
    $(c).removeClass("selected");
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
    resetOptionsForUnSelected();
}
function getUnselected() {
    var arr = [];
    $("select.ct").each(function () {
        if ($(this).find("option:selected[value='']").length > 0) {
            arr.push(this);
        }
    });
    return arr;
}
function getSelectedValues() {
    var arr = [];
    $("select.ct").each(function () {
        if ($(this).find("option:selected[value!=='']").length > 0) {
            arr.push($(this).val());
        }
    });
    return arr;
}
function resetOptionsForSelect(c) {
    $(c).find("option").remove();
    $(c).append(buildGlobalOptions());
    jcf.replaceAll();
}
function resetOptionsForUnSelected() {
    $(getUnselected()).each(function () {
        resetOptionsForSelect(this);
        jcf.replaceAll();
    });
}
function buildGlobalOptions() {
    console.log("Building global options");
    var arr = new Array();
    $.each(globalOptions, function () {
        arr.push(buildOptionTag(this));
    });
    return arr;
}
function buildOptionTag(value) {
    console.log("Building option for '" + value + "'");
    if (value.toString() === "") {
        console.log("Building empty option");
        return $("<option></option>").attr("value", "").text("-- Select Column -");
    }
    else {
        return $("<option></option>").attr("value", value).text(value);
    }
}
function addGlobalOption(value) {
    var isPresent = false;
    $.each(globalOptions, function () {
        if (this.toString() === value)
            isPresent = true;
    });
    if (isPresent)
        return;
    globalOptions.push(value);
}
function removeGlobalOption(value) {
    globalOptions = jQuery.grep(globalOptions, function (toRemove) { return toRemove !== value; });
}
function addRequiredOption(value) {
    var isPresent = false;
    $.each(requiredFields, function () {
        if (this.toString() === value)
            isPresent = true;
    });
    if (isPresent)
        return;
    requiredFields.push(value);
}
function removeRequiredOption(value) {
    requiredFields = jQuery.grep(requiredFields, function (toRemove) { return toRemove !== value; });
}
function containsGlobalOption(value) {
    var present = false;
    $.each(globalOptions, function () {
        if (this.toString() === value)
            present = true;
    });
    return present;
}
function containsRequiredOption(value) {
    var present = false;
    $.each(requiredFields, function () {
        if (this.toString() === value)
            present = true;
    });
    return present;
}
function requiredFieldsToReadable(requiredFields) {
    var readable = "";
    $.each(requiredFields, function (i, v) {
        readable += getDisplayTextForField(v) + (i < requiredFields.length - 1 ? ", " : "");
    });
    return readable.trim();
}
function getDisplayTextForField(field) {
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
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiSWRlbnRpZnlDb2x1bW5zLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiSWRlbnRpZnlDb2x1bW5zLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLElBQUksY0FBbUIsQ0FBQztBQUV4QixJQUFJLGlCQUF5QixDQUFDO0FBRzlCLENBQUMsQ0FBQyxRQUFRLENBQUMsQ0FBQyxLQUFLLENBQ2Y7SUFFRSxDQUFDLENBQUMsV0FBVyxDQUFDLENBQUMsSUFBSSxDQUFDLFFBQVEsRUFDMUI7UUFDRSxDQUFDLENBQUMsVUFBVSxDQUFDLENBQUMsSUFBSSxFQUFFLENBQUM7UUFFckIsSUFBSSxDQUFDLENBQUMsSUFBSSxDQUFDLENBQUMsR0FBRyxFQUFFLEtBQUssT0FBTyxFQUFFO1lBQzdCLE9BQU8sQ0FBQyxHQUFHLENBQUMsa0JBQWtCLENBQUMsQ0FBQztZQUNoQyxLQUFLLENBQUMsSUFBSSxDQUFDLENBQUM7WUFDWiwyQkFBMkIsRUFBRSxDQUFDO1lBQzlCLE9BQU87U0FDUjtRQUVELEdBQUcsQ0FBQyxJQUFJLENBQUMsQ0FBQztRQUNWLDJCQUEyQixFQUFFLENBQUM7SUFDaEMsQ0FBQyxDQUFDLENBQUM7SUFFTCxDQUFDLENBQUMsY0FBYyxDQUFDLENBQUMsSUFBSSxDQUFDLE9BQU8sRUFDNUI7UUFDRSxNQUFNLEVBQUUsQ0FBQztJQUNYLENBQUMsQ0FBQyxDQUFDO0lBRUwsVUFBVSxFQUFFLENBQUM7SUFFYiwyQkFBMkIsRUFBRSxDQUFDO0FBQ2hDLENBQUMsQ0FDRixDQUFDO0FBR0YsSUFBSSxhQUFhLEdBQUcsSUFBSSxLQUFLLEVBQUUsQ0FBQztBQUVoQyxTQUFTLE1BQU07SUFDYixJQUFJLFFBQVEsRUFBRSxFQUFFO1FBQ2QsSUFBSSxTQUFTLEdBQUcsRUFBRSxDQUFDO1FBQ25CLENBQUMsQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLGtCQUFrQixDQUFDLEVBQzFCLFVBQUMsQ0FBQyxFQUFFLENBQUM7WUFDSCxJQUFJLENBQUMsQ0FBQyxL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ZUFBZTtZQUNoQixPQUFPLGdCQUFnQixDQUFDO1FBQzVCLEtBQUssWUFBWTtZQUNiLE9BQU8sYUFBYSxDQUFDO0tBQzFCO0lBQ0gsT0FBTyxLQUFLLENBQUM7QUFDZixDQUFDIiwic291cmNlc0NvbnRlbnQiOlsidmFyIHJlcXVpcmVkRmllbGRzOiBhbnk7XHJcbmRlY2xhcmUgdmFyIG9yZGVyTW9kZWw6IGFueTtcclxudmFyIEZpZWxkTmFtZV9Vbmtub3duOiBzdHJpbmc7XHJcbmRlY2xhcmUgbGV0IGpjZjogYW55O1xyXG5cclxuJChkb2N1bWVudCkucmVhZHkoXHJcbiAgKCkgPT4ge1xyXG5cclxuICAgICQoXCJzZWxlY3QuY3RcIikuYmluZChcImNoYW5nZVwiLFxyXG4gICAgICBmdW5jdGlvbigpIHsgLy8gcmVzZXQgb3B0aW9uIHdhcyBjaG9zZW5cclxuICAgICAgICAkKFwiI21lc3NhZ2VcIikuaGlkZSgpO1xyXG4gICAgICAgIC8vIHJlc2V0IG9wdGlvbiBzZWxlY3RlZFxyXG4gICAgICAgIGlmICgkKHRoaXMpLnZhbCgpID09PSBcIlJlc2V0XCIpIHtcclxuICAgICAgICAgIGNvbnNvbGUubG9nKFwiUmVzZXRpbmcgY29udHJvbFwiKTtcclxuICAgICAgICAgIHJlc2V0KHRoaXMpO1xyXG4gICAgICAgICAgdXBkYXRlUmVxdWlyZWRGaWVsZHNEaXNwbGF5KCk7XHJcbiAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG4gICAgICAgIC8vIGZpZWxkIG9wdGlvbiBzZWxlY3RlZFxyXG4gICAgICAgIHNldCh0aGlzKTtcclxuICAgICAgICB1cGRhdGVSZXF1aXJlZEZpZWxkc0Rpc3BsYXkoKTtcclxuICAgICAgfSk7XHJcbiAgICBcclxuICAgICQoXCJhW25hbWU9bmV4dF1cIikuYmluZChcImNsaWNrXCIsXHJcbiAgICAgICgpID0+IHtcclxuICAgICAgICBzdWJtaXQoKTtcclxuICAgICAgfSk7XHJcblxyXG4gICAgaW5pdGlhbGl6ZSgpO1xyXG5cclxuICAgIHVwZGF0ZVJlcXVpcmVkRmllbGRzRGlzcGxheSgpO1xyXG4gIH1cclxuKTtcclxuXHJcbi8vIHBvcHVsYXRlIHdpdGggb3B0aW9ucyBmcm9tIHdoZW4gcGFnZSBpcyBmaXJzdCBpbml0aWFsaXplZFxyXG52YXIgZ2xvYmFsT3B0aW9ucyA9IG5ldyBBcnJheSgpO1xyXG5cclxuZnVuY3Rpb24gc3VibWl0KCkge1xyXG4gIGlmICh2YWxpZGF0ZSgpKSB7XHJcbiAgICB2YXIgY29sdW1uTWFwID0gXCJcIjtcclxuICAgICQuZWFjaCgkKFwiI21hcHBpbmdzIHNlbGVjdFwiKSxcclxuICAgICAgKGksIHYpID0+IHtcclxuICAgICAgICBpZiAodi52YWx1ZSA9PT0gXCJcIikge1xyXG4gICAgICAgICAgY29sdW1uTWFwICs9IEZpZWxkTmFtZV9Vbmtub3duICsgXCI7XCI7XHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgIGNvbHVtbk1hcCArPSB2LnZhbHVlICsgXCI7XCI7XHJcbiAgICAgICAgfVxyXG4gICAgICB9KTtcclxuICAgIFxyXG4gICAgb3JkZXJNb2RlbC5vcmRlci5jb2x1bW5NYXAgPSBjb2x1bW5NYXA7XHJcblxyXG4gICAgY29uc29sZS5sb2cob3JkZXJNb2RlbC5vcmRlci5jb2x1bW5NYXApO1xyXG5cclxuICAgICQoXCI8aW5wdXQ+XCIpLmF0dHIoe1xyXG4gICAgICB0eXBlOiBcImhpZGRlblwiLFxyXG4gICAgICBuYW1lOiBcIm9yZGVyTW9kZWxcIixcclxuICAgICAgdmFsdWU6IGtvLnRvSlNPTihvcmRlck1vZGVsLm9yZGVyKVxyXG4gICAgICB9KS5hcHBlbmRUbyhcImZvcm1cIikuZmlyc3QoKTtcclxuXHJcblxyXG5cclxuICAgICQoXCJmb3JtXCIpLmZpcnN0KCkuc3VibWl0KCk7XHJcbiAgICByZXR1cm4gdHJ1ZTtcclxuICB9XHJcbiAgcmV0dXJuIGZhbHNlO1xyXG59XHJcblxyXG5mdW5jdGlvbiB1cGRhdGVSZXF1aXJlZEZpZWxkc0Rpc3BsYXkoKSB7XHJcbiAgLy8gaWRlbnRpZnkgcmVxdWlyZWQgZmllbGRzIHRoYXQgYXJlIHN0aWxsIGluIHRoZSBnbG9iYWwgb3B0aW9ucywgdGhlc2UgZmllbGRzIHN0aWxsIG5lZWQgdG8gYmUgbWFwcGVkXHJcbiAgY29uc3QgcmVtYWluaW5nUmVxdWlyZWRGaWVsZHMgPSAkLmdyZXAocmVxdWlyZWRGaWVsZHMsIGYgPT4gJC5pbkFycmF5KGYsIGdsb2JhbE9wdGlvbnMpID4gLTEpO1xyXG4gIGlmIChyZW1haW5pbmdSZXF1aXJlZEZpZWxkcy5sZW5ndGggPT09IDApIHtcclxuICAgIC8vJChcIiNhbGVydFwiKS5yZW1vdmVDbGFzcygpLmFkZENsYXNzKFwiYWxlcnQgYWxlcnQtc3VjY2Vzc1wiKS50ZXh0KFwiQWxsIHJlcXVpcmVkIGZpZWxkcyBoYXZlIGJlZW4gbWFwcGVkLlwiKS5zaG93KCk7XHJcbiAgICAvLyQoXCJidXR0b25bbmFtZT1uZXh0XVwiKS5wcm9wKFwiZGlzYWJsZWRcIiwgZmFsc2UpO1xyXG4gICAgLy9zdWJtaXQoKTtcclxuICB9IGVsc2Uge1xyXG4gICAgICAkKFwiI2FsZXJ0XCIpLnJlbW92ZUNsYXNzKCkuYWRkQ2xhc3MoXCJhbGVydCBhbGVydC1kYW5nZXJcIikudGV4dChcclxuICAgICAgYFBsZWFzZSBpZGVudGlmeSB0aGUgZm9sbG93aW5nIGNvbHVtbnMgaW4geW91ciBmaWxlIGluIHRoZSBmb3JtIGJlbG93OiAke3JlcXVpcmVkRmllbGRzVG9SZWFkYWJsZShyZXF1aXJlZEZpZWxkcyl9LmApLnNob3coKTtcclxuICAgICQoXCJidXR0b25bbmFtZT1uZXh0XVwiKS5wcm9wKFwiZGlzYWJsZWRcIiwgdHJ1ZSk7XHJcbiAgfVxyXG59XHJcblxyXG5mdW5jdGlvbiBpbml0aWFsaXplKCkge1xyXG4gIC8vIGluaXRpYWxpemUgZ2xvYmFsIGZyb20gZXhpc3Rpbmcgc2VsZWN0c1xyXG4gICQoXCJzZWxlY3QuY3Q6Zmlyc3RcIikuZmluZChcIm9wdGlvblwiKS5lYWNoKGZ1bmN0aW9uKCkge1xyXG4gICAgYWRkR2xvYmFsT3B0aW9uKCQodGhpcykudmFsKCkpO1xyXG4gIH0pO1xyXG5cclxuICAvLyBpZiBtYW5pZmVzdCBjYWxscyBmb3IgbWFwcGluZyBmaXJzdCBhbmQgbGFzdCBuYW1lIHRoZW4gZ2l2ZSB0aGUgb3B0aW9uIHRvIG1hcCBGdWxsTmFtZVxyXG4gIGlmICgkLmluQXJyYXkoXCJGaXJzdE5hbWVcIiwgZ2xvYmFsT3B0aW9ucykgPj0gMCAmJiAkLmluQXJyYXkoXCJMYXN0TmFtZVwiLCBnbG9iYWxPcHRpb25zKSA+PSAwKSB7XHJcbiAgICBhZGRHbG9iYWxPcHRpb24oXCJGdWxsTmFtZVwiKTtcclxuICAgIHJlc2V0T3B0aW9uc0ZvclVuU2VsZWN0ZWQoKTtcclxuICB9XHJcbn1cclxuXHJcbmZ1bmN0aW9uIGFycmF5SW50ZXJzZWN0KGEsIGIpIHtcclxuICByZXR1cm4gJC5ncmVwKGEsIGkgPT4gJC5pbkFycmF5KGksIGIpID4gLTEpO1xyXG59O1xyXG5cclxuLy8gY2hlY2tzIHJlbWFpbmluZyBmaWVsZHMgYWdhaW5zdCByZXF1aXJlZCBmaWVsZHNcclxuZnVuY3Rpb24gdmFsaWRhdGUoKSB7XHJcbiAgaWYgKGFycmF5SW50ZXJzZWN0KGdsb2JhbE9wdGlvbnMsIHJlcXVpcmVkRmllbGRzKS5sZW5ndGggPiAwKSB7XHJcbiAgICAgICQoXCIjYWxlcnRcIikucmVtb3ZlQ2xhc3MoKS5hZGRDbGFzcyhcImFsZXJ0IGFsZXJ0LWRhbmdlclwiKS50ZXh0KFxyXG4gICAgICAgICAgYFBsZWFzZSBpZGVudGlmeSB0aGUgZm9sbG93aW5nIGNvbHVtbnMgaW4geW91ciBmaWxlIGluIHRoZSBmb3JtIGJlbG93OiAke3JlcXVpcmVkRmllbGRzVG9SZWFkYWJsZShyZXF1aXJlZEZpZWxkcyl9LmApLnNob3coKTtcclxuICAgIHJldHVybiBmYWxzZTtcclxuICB9XHJcbiAgcmV0dXJuIHRydWU7XHJcbn1cclxuXHJcblxyXG4vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vXHJcbi8vIGNvbnRyb2wgaGVscGVyc1xyXG4vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vXHJcblxyXG4vLyBzZXRzIHNlbGVjdCB0byBjaG9zZW4gb3B0aW9uXHJcbmZ1bmN0aW9uIHNldChjKSB7XHJcbiAgY29uc29sZS5sb2coXCJzZXRTZWxlY3QoKVwiKTtcclxuICAvLyAxLiByZW1vdmUgb3B0aW9uIGZyb20gZ2xvYmFsXHJcbiAgcmVtb3ZlR2xvYmFsT3B0aW9uKCQoYykudmFsKCkpO1xyXG4gIC8vIDIuIHJlbW92ZSBhbGwgb3B0aW9ucyBleGNlcHQgdGhpcyBvbmUgZnJvbSB0aGlzIHNlbGVjdFxyXG4gIGNvbnN0IHNlbGVjdGVkT3B0aW9uID0gJChjKS5maW5kKFwiOnNlbGVjdGVkXCIpO1xyXG5cclxuICAvLyBuYW1lXHJcbiAgaWYgKHNlbGVjdGVkT3B0aW9uLnZhbCgpID09PSBcIkZpcnN0TmFtZVwiIHx8IHNlbGVjdGVkT3B0aW9uLnZhbCgpID09PSBcIkxhc3ROYW1lXCIpIHtcclxuICAgIHJlbW92ZUdsb2JhbE9wdGlvbihcIkZ1bGxOYW1lXCIpO1xyXG4gIH0gZWxzZSBpZiAoc2VsZWN0ZWRPcHRpb24udmFsKCkgPT09IFwiRnVsbE5hbWVcIikge1xyXG4gICAgcmVtb3ZlR2xvYmFsT3B0aW9uKFwiRmlyc3ROYW1lXCIpO1xyXG4gICAgcmVtb3ZlR2xvYmFsT3B0aW9uKFwiTGFzdE5hbWVcIik7XHJcbiAgfVxyXG4gIC8vIFBvc3RhbENvZGUgc2VsZWN0ZWQgdGhlbiBtYWtlIENpdHkgJiBTdGF0ZSBvcHRpb25hbFxyXG4gIGVsc2UgaWYgKHNlbGVjdGVkT3B0aW9uLnZhbCgpID09PSBcIlBvc3RhbENvZGVcIikge1xyXG4gICAgcmVtb3ZlUmVxdWlyZWRPcHRpb24oXCJDaXR5XCIpO1xyXG4gICAgcmVtb3ZlUmVxdWlyZWRPcHRpb24oXCJTdGF0ZVwiKTtcclxuICB9XHJcbiAgLy8gaWYgQ2l0eSBvciBTdGF0ZSBzZWxlY3RlZCB0aGVuIG1ha2UgUG9zdGFsQ29kZSBvcHRpb25hbCBpZiBib3RoIGhhdmUgYmVlbiBzZWxlY3RlZFxyXG4gIGVsc2UgaWYgKHNlbGVjdGVkT3B0aW9uLnZhbCgpID09PSBcIkNpdHlcIikge1xyXG4gICAgLy8gaWYgU3RhdGUgaGFzIGJlZW4gbWFwcGVkIHRoZW4gcmVtb3ZlIFBvc3RhbENvZGVcclxuICAgIGlmICghY29udGFpbnNHbG9iYWxPcHRpb24oXCJTdGF0ZVwiKSkgcmVtb3ZlUmVxdWlyZWRPcHRpb24oXCJQb3N0YWxDb2RlXCIpO1xyXG4gIH0gZWxzZSBpZiAoc2VsZWN0ZWRPcHRpb24udmFsKCkgPT09IFwiU3RhdGVcIikge1xyXG4gICAgLy8gaWYgQ2l0eSBoYXMgYmVlbiBtYXBwZWQgdGhlbiByZW1vdmUgUG9zdGFsQ29kZVxyXG4gICAgaWYgKCFjb250YWluc0dsb2JhbE9wdGlvbihcIkNpdHlcIikpIHJlbW92ZVJlcXVpcmVkT3B0aW9uKFwiUG9zdGFsQ29kZVwiKTtcclxuICB9XHJcblxyXG4gICQoYykuZmluZChcIm9wdGlvblwiKS5yZW1vdmUoKTtcclxuICAkKGMpLmFwcGVuZChzZWxlY3RlZE9wdGlvbik7XHJcbiAgJChjKS5hcHBlbmQoYnVpbGRPcHRpb25UYWcoXCJSZXNldFwiKSk7XHJcbiAgLy8gMy4gcmVwb3B1bGF0ZSBvcHRpb25zIGluIFVuc2VsZWN0ZWRcclxuICAkLmVhY2goZ2V0VW5zZWxlY3RlZCgpLFxyXG4gICAgZnVuY3Rpb24oKSB7XHJcbiAgICAgIGNvbnNvbGUubG9nKFwiTG9vcGluZyB0aHJvdWdoIFVuU2VsZWN0ZWQgY29udHJvbHNcIik7XHJcbiAgICAgICQodGhpcykuZmluZChcIm9wdGlvblwiKS5yZW1vdmUoKTtcclxuICAgICAgJCh0aGlzKS5hcHBlbmQoYnVpbGRHbG9iYWxPcHRpb25zKCkpO1xyXG4gICAgfSk7XHJcbiAgJChjKS5hZGRDbGFzcyhcInNlbGVjdGVkXCIpO1xyXG5cclxufVxyXG5cclxuLy8gcmVzdG9yZXMgc2VsZWN0IHdoZW4gXCJSZXNldFwiIG9wdGlvbiBpcyBjaG9vc2VuXHJcbmZ1bmN0aW9uIHJlc2V0KGMpIHtcclxuICAvLyAxLiBhZGQgb3B0aW9uIGJhY2sgaW50byBnbG9iYWxcclxuICBjb25zdCBzZWxlY3RlZE9wdGlvbiA9ICQoYykuZmluZChcIlt2YWx1ZSE9J1Jlc2V0J11cIikudmFsKCk7XHJcbiAgYWRkR2xvYmFsT3B0aW9uKHNlbGVjdGVkT3B0aW9uKTtcclxuICAvLyByZXNldCBvcHRpb25zIGZvciB0aGlzIHNlbGVjdFxyXG4gIHJlc2V0T3B0aW9uc0ZvclNlbGVjdChjKTtcclxuICAkKGMpLnJlbW92ZUNsYXNzKFwic2VsZWN0ZWRcIik7XHJcblxyXG4gIC8vIGlmIG1hbmlmZXN0IGNhbGxzIGZvciBTdHJlZXRBZGRyZXNzLCBDaXR5LCBTdGF0ZSwgUG9zdGFsQ29kZVxyXG4gIC8vIGlmIENpdHkgYW5kIFN0YXRlIGFyZSBtYXBwZWQgdGhlbiBtYWtlIFBvc3RhbENvZGUgb3B0aW9uYWxcclxuICAvLyBpZiBQb3N0YWxDb2RlIGlzIG1hcHBlZCB0aGVuIG1ha2UgQ2l0eSBhbmQgU3RhdGUgb3B0aW9uYWxcclxuXHJcbiAgc3dpdGNoIChzZWxlY3RlZE9wdGlvbikge1xyXG4gIGNhc2UgXCJGdWxsTmFtZVwiOlxyXG4gICAgYWRkR2xvYmFsT3B0aW9uKFwiRmlyc3ROYW1lXCIpO1xyXG4gICAgYWRkR2xvYmFsT3B0aW9uKFwiTGFzdE5hbWVcIik7XHJcbiAgICBicmVhaztcclxuICBjYXNlIFwiRmlyc3ROYW1lXCI6XHJcbiAgICBpZiAoY29udGFpbnNHbG9iYWxPcHRpb24oXCJMYXN0TmFtZVwiKSlcclxuICAgICAgYWRkR2xvYmFsT3B0aW9uKFwiRnVsbE5hbWVcIik7XHJcbiAgICBicmVhaztcclxuICBjYXNlIFwiTGFzdE5hbWVcIjpcclxuICAgIGlmIChjb250YWluc0dsb2JhbE9wdGlvbihcIkZpcnN0TmFtZVwiKSlcclxuICAgICAgYWRkR2xvYmFsT3B0aW9uKFwiRnVsbE5hbWVcIik7XHJcbiAgICBicmVhaztcclxuICBjYXNlIFwiUG9zdGFsQ29kZVwiOlxyXG4gICAgaWYgKCFjb250YWluc1JlcXVpcmVkT3B0aW9uKFwiU3RhdGVcIikpXHJcbiAgICAgIGFkZFJlcXVpcmVkT3B0aW9uKFwiU3RhdGVcIik7XHJcbiAgICBpZiAoIWNvbnRhaW5zUmVxdWlyZWRPcHRpb24oXCJDaXR5XCIpKVxyXG4gICAgICBhZGRSZXF1aXJlZE9wdGlvbihcIkNpdHlcIik7XHJcbiAgICBicmVhaztcclxuICBjYXNlIFwiU3RhdGVcIjpcclxuICBjYXNlIFwiQ2l0eVwiOlxyXG4gICAgaWYgKGNvbnRhaW5zR2xvYmFsT3B0aW9uKFwiUG9zdGFsQ29kZVwiKSlcclxuICAgICAgYWRkUmVxdWlyZWRPcHRpb24oXCJQb3N0YWxDb2RlXCIpO1xyXG4gICAgYnJlYWs7XHJcbiAgfVxyXG5cclxuICAvLyByZXNldCBvcHRpb25zIGZvciBvdGhlciBzZWxlY3RzXHJcbiAgcmVzZXRPcHRpb25zRm9yVW5TZWxlY3RlZCgpO1xyXG59XHJcblxyXG4vLyByZXR1cm5zIGNvbGxlY3Rpb24gb2Ygc2VsZWN0cyB3aGVyZSBzZWxlY3RlZCB2YWx1ZSA9ICcnXHJcbmZ1bmN0aW9uIGdldFVuc2VsZWN0ZWQoKSB7XHJcbiAgdmFyIGFyciA9IFtdO1xyXG4gICQoXCJzZWxlY3QuY3RcIikuZWFjaChmdW5jdGlvbigpIHtcclxuICAgIGlmICgkKHRoaXMpLmZpbmQoXCJvcHRpb246c2VsZWN0ZWRbdmFsdWU9JyddXCIpLmxlbmd0aCA+IDApIHtcclxuICAgICAgYXJyLnB1c2godGhpcyk7XHJcbiAgICB9XHJcbiAgfSk7XHJcbiAgcmV0dXJuIGFycjtcclxufVxyXG5cclxuLy8gcmV0dXJucyBjb2xsZWN0aW9uIG9mIHNlbGVjdHMgd2hlcmUgc2VsZWN0ZWQgdmFsdWUgIT0gJydcclxuZnVuY3Rpb24gZ2V0U2VsZWN0ZWRWYWx1ZXMoKSB7XHJcbiAgdmFyIGFyciA9IFtdO1xyXG4gICQoXCJzZWxlY3QuY3RcIikuZWFjaChmdW5jdGlvbigpIHtcclxuICAgIGlmICgkKHRoaXMpLmZpbmQoXCJvcHRpb246c2VsZWN0ZWRbdmFsdWUhPT0nJ11cIikubGVuZ3RoID4gMCkge1xyXG4gICAgICBhcnIucHVzaCgkKHRoaXMpLnZhbCgpKTtcclxuICAgIH1cclxuICB9KTtcclxuICByZXR1cm4gYXJyO1xyXG59XHJcblxyXG4vLyByZXNldHMgb3B0aW9ucyB1c2luZyBnbG9iYWxcclxuZnVuY3Rpb24gcmVzZXRPcHRpb25zRm9yU2VsZWN0KGMpIHtcclxuICAkKGMpLmZpbmQoXCJvcHRpb25cIikucmVtb3ZlKCk7XHJcbiAgLy8gMy4gYWRkIGdsb2JhbCBvcHRpb25zIGJhY2sgaW50byBzZWxlY3RcclxuICAgICQoYykuYXBwZW5kKGJ1aWxkR2xvYmFsT3B0aW9ucygpKTtcclxuICAgIGpjZi5yZXBsYWNlQWxsKCk7XHJcbn1cclxuXHJcbi8vIHJlc2V0cyBhbGwgdW5TZWxlY3RlZCBzZWxlY3RzIHRvIGdsb2JhbCBvcHRpb25zXHJcbmZ1bmN0aW9uIHJlc2V0T3B0aW9uc0ZvclVuU2VsZWN0ZWQoKSB7XHJcbiAgJChnZXRVbnNlbGVjdGVkKCkpLmVhY2goZnVuY3Rpb24oKSB7XHJcbiAgICAgIHJlc2V0T3B0aW9uc0ZvclNlbGVjdCh0aGlzKTtcclxuICAgICAgamNmLnJlcGxhY2VBbGwoKTtcclxuICB9KTtcclxufVxyXG5cclxuLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vL1xyXG4vLyBnbG9iYWwgb3B0aW9ucyBoZWxwZXJzXHJcbi8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy8vLy9cclxuXHJcbi8vIGJ1aWxkcyBvcHRpb24gZWxlbWVudCBjb2xsZWN0aW9uIGZvciB1c2Ugd2l0aCBzZWxlY3RcclxuZnVuY3Rpb24gYnVpbGRHbG9iYWxPcHRpb25zKCkge1xyXG4gIGNvbnNvbGUubG9nKFwiQnVpbGRpbmcgZ2xvYmFsIG9wdGlvbnNcIik7XHJcbiAgdmFyIGFyciA9IG5ldyBBcnJheSgpO1xyXG4gICQuZWFjaChnbG9iYWxPcHRpb25zLFxyXG4gICAgZnVuY3Rpb24oKSB7XHJcbiAgICAgIGFyci5wdXNoKGJ1aWxkT3B0aW9uVGFnKHRoaXMpKTtcclxuICAgIH0pO1xyXG4gIHJldHVybiBhcnI7XHJcbn1cclxuXHJcbi8vIGJ1aWxkcyBvcHRpb24gZWxlbWVudFxyXG5mdW5jdGlvbiBidWlsZE9wdGlvblRhZyh2YWx1ZSkge1xyXG4gIGNvbnNvbGUubG9nKGBCdWlsZGluZyBvcHRpb24gZm9yICcke3ZhbHVlfSdgKTtcclxuICBpZiAodmFsdWUudG9TdHJpbmcoKSA9PT0gXCJcIikge1xyXG4gICAgY29uc29sZS5sb2coXCJCdWlsZGluZyBlbXB0eSBvcHRpb25cIik7XHJcbiAgICByZXR1cm4gJChcIjxvcHRpb24+PC9vcHRpb24+XCIpLmF0dHIoXCJ2YWx1ZVwiLCBcIlwiKS50ZXh0KFwiLS0gU2VsZWN0IENvbHVtbiAtXCIpO1xyXG4gIH0gZWxzZSB7XHJcbiAgICByZXR1cm4gJChcIjxvcHRpb24+PC9vcHRpb24+XCIpLmF0dHIoXCJ2YWx1ZVwiLCB2YWx1ZSkudGV4dCh2YWx1ZSk7XHJcbiAgfVxyXG59XHJcblxyXG4vLyBhZGRzIG9wdGlvbiB0byBHbG9iYWwgYW5kIGVuc3VyZXMgdGhlIGNvbGxlY3Rpb24gc3RheXMgdW5pcXVlXHJcbmZ1bmN0aW9uIGFkZEdsb2JhbE9wdGlvbih2YWx1ZSkge1xyXG4gIHZhciBpc1ByZXNlbnQgPSBmYWxzZTtcclxuICAkLmVhY2goZ2xvYmFsT3B0aW9ucyxcclxuICAgIGZ1bmN0aW9uKCkge1xyXG4gICAgICBpZiAodGhpcy50b1N0cmluZygpID09PSB2YWx1ZSlcclxuICAgICAgICBpc1ByZXNlbnQgPSB0cnVlO1xyXG4gICAgfSk7XHJcbiAgaWYgKGlzUHJlc2VudCkgcmV0dXJuO1xyXG4gIGdsb2JhbE9wdGlvbnMucHVzaCh2YWx1ZSk7XHJcbn1cclxuXHJcbi8vIHJlbW92ZXMgb3B0aW9uIGZyb20gR2xvYmFsIG9wdGlvbnNcclxuZnVuY3Rpb24gcmVtb3ZlR2xvYmFsT3B0aW9uKHZhbHVlKSB7XHJcbiAgZ2xvYmFsT3B0aW9ucyA9IGpRdWVyeS5ncmVwKGdsb2JhbE9wdGlvbnMsIHRvUmVtb3ZlID0+IHRvUmVtb3ZlICE9PSB2YWx1ZSk7XHJcbn1cclxuXHJcbi8vIGFkZHMgb3B0aW9uIHRvIFJlcXVpcmVkXHJcbmZ1bmN0aW9uIGFkZFJlcXVpcmVkT3B0aW9uKHZhbHVlKSB7XHJcbiAgdmFyIGlzUHJlc2VudCA9IGZhbHNlO1xyXG4gICQuZWFjaChyZXF1aXJlZEZpZWxkcyxcclxuICAgIGZ1bmN0aW9uKCkge1xyXG4gICAgICBpZiAodGhpcy50b1N0cmluZygpID09PSB2YWx1ZSlcclxuICAgICAgICBpc1ByZXNlbnQgPSB0cnVlO1xyXG4gICAgfSk7XHJcbiAgaWYgKGlzUHJlc2VudCkgcmV0dXJuO1xyXG4gIHJlcXVpcmVkRmllbGRzLnB1c2godmFsdWUpO1xyXG59XHJcblxyXG4vLyByZW1vdmVzIG9wdGlvbiBmcm9tIFJlcXVpcmVkIG9wdGlvbnNcclxuZnVuY3Rpb24gcmVtb3ZlUmVxdWlyZWRPcHRpb24odmFsdWUpIHtcclxuICByZXF1aXJlZEZpZWxkcyA9IGpRdWVyeS5ncmVwKHJlcXVpcmVkRmllbGRzLCB0b1JlbW92ZSA9PiB0b1JlbW92ZSAhPT0gdmFsdWUpO1xyXG59XHJcblxyXG4vLyBkZXRlcm1pbmVzIGlmIGNvbnRhaW5lZCBpbiBHbG9iYWxcclxuZnVuY3Rpb24gY29udGFpbnNHbG9iYWxPcHRpb24odmFsdWUpIHtcclxuICB2YXIgcHJlc2VudCA9IGZhbHNlO1xyXG4gICQuZWFjaChnbG9iYWxPcHRpb25zLFxyXG4gICAgZnVuY3Rpb24oKSB7XHJcbiAgICAgIGlmICh0aGlzLnRvU3RyaW5nKCkgPT09IHZhbHVlKVxyXG4gICAgICAgIHByZXNlbnQgPSB0cnVlO1xyXG4gICAgfSk7XHJcbiAgcmV0dXJuIHByZXNlbnQ7XHJcbn1cclxuXHJcbi8vIGRldGVybWluZXMgaWYgY29udGFpbmVkIGluIEdsb2JhbFxyXG5mdW5jdGlvbiBjb250YWluc1JlcXVpcmVkT3B0aW9uKHZhbHVlKSB7XHJcbiAgdmFyIHByZXNlbnQgPSBmYWxzZTtcclxuICAkLmVhY2gocmVxdWlyZWRGaWVsZHMsXHJcbiAgICBmdW5jdGlvbigpIHtcclxuICAgICAgaWYgKHRoaXMudG9TdHJpbmcoKSA9PT0gdmFsdWUpXHJcbiAgICAgICAgcHJlc2VudCA9IHRydWU7XHJcbiAgICB9KTtcclxuICByZXR1cm4gcHJlc2VudDtcclxufVxyXG5cclxuZnVuY3Rpb24gcmVxdWlyZWRGaWVsZHNUb1JlYWRhYmxlKHJlcXVpcmVkRmllbGRzOiBBcnJheTxzdHJpbmc+KSB7XHJcbiAgICB2YXIgcmVhZGFibGUgPSBcIlwiO1xyXG4gICAgJC5lYWNoKHJlcXVpcmVkRmllbGRzLCAoaSwgdikgPT4ge1xyXG4gICAgICAgIHJlYWRhYmxlICs9IGdldERpc3BsYXlUZXh0Rm9yRmllbGQodikgKyAoaSA8IHJlcXVpcmVkRmllbGRzLmxlbmd0aCAtIDEgPyBcIiwgXCIgOiBcIlwiKTtcclxuICAgIH0pO1xyXG4gIHJldHVybiByZWFkYWJsZS50cmltKCk7XHJcbn1cclxuXHJcbmZ1bmN0aW9uIGdldERpc3BsYXlUZXh0Rm9yRmllbGQoZmllbGQ6IHN0cmluZykge1xyXG4gIHN3aXRjaCAoZmllbGQpIHtcclxuICAgICAgY2FzZSBcIkZpcnN0TmFtZVwiOlxyXG4gICAgICAgICAgcmV0dXJuIFwiRmlyc3QgTmFtZVwiO1xyXG4gICAgICBjYXNlIFwiTGFzdE5hbWVcIjpcclxuICAgICAgICAgIHJldHVybiBcIkxhc3QgTmFtZVwiO1xyXG4gICAgICBjYXNlIFwiU3RyZWV0QWRkcmVzc1wiOlxyXG4gICAgICAgICAgcmV0dXJuIFwiU3RyZWV0IEFkZHJlc3NcIjtcclxuICAgICAgY2FzZSBcIlBvc3RhbENvZGVcIjpcclxuICAgICAgICAgIHJldHVybiBcIlBvc3RhbCBDb2RlXCI7XHJcbiAgICB9XHJcbiAgcmV0dXJuIGZpZWxkO1xyXG59Il19