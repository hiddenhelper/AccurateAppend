/// <reference path="typings/knockout/knockout.d.ts" />
/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="typings/underscore/underscore.d.ts" />
/// <reference path="typings/bootstrap/bootstrap.d.ts" />

module AccurateAppend.DyanmicAppend.BuildManifest {

    import Manifest = DynamicAppend.Manifest;
    import OperationName = DynamicAppend.OperationName;

    export class ViewModel {

        manifest: Manifest;
        category: string;

        constructor() {
            this.manifest = new Manifest();
        }

        // Toggles inclusion of Operation based on state of checkbox in UI
        toggle(event: JQueryEventObject) {
            var self = this;
            this.displayMessage(null);
            var operationName = <OperationName>OperationName[<string>$(event.target).prop("id")];
            
            // add Operation
            if ($(event.target).prop("checked"))
                // special cases where other Operations are affected by the operation being added. May need to un-check output fields
                switch (operationName) {
                    case OperationName.DEDUPE_PHONE:
                    case OperationName.SET_PREF_PHONE:
                    case OperationName.SET_PREF_PHONE_SINGLE_COLUMN:
                        self.manifest.add($(event.target).prop("id"),
                            // success
                            () => {
                                console.log("success");
                                // uncheck phone util checkboxes except the one that triggered the event
                                _.each(_.difference(_.pluck($("[name=phoneutil]"), "id"), [$(event.target).prop("id")]), (id) => {
                                    self.manifest.remove(id);
                                    $("#" + id).removeAttr("checked");
                                });
                            },
                            // error
                            e => {
                                console.log("failure " + <string>$(event.target).prop("id"));
                                self.displayMessage(e.message);
                                $("#" + $(event.target).prop("id")).removeAttr("checked");
                            });
                        break;
                    case OperationName.SET_PREF_ADDRESS_SINGLE_COLUMN:
                        self.manifest.add($(event.target).prop("id"),
                            // success
                            () => {
                                console.log("success");
                            },
                            // error
                            e => {
                                console.log("failure " + <string>$(event.target).prop("id"));
                                self.displayMessage(e.message);
                                $("#" + $(event.target).prop("id")).removeAttr("checked");
                            });
                        break;
                    case OperationName.SET_PREF_PHONE_COMPARE_INPUT:
                        self.manifest.add($(event.target).prop("id"),
                            // success
                            () => {
                                console.log("success");
                            },
                            // error
                            e => {
                                console.log("failure " + <string>$(event.target).prop("id"));
                                self.displayMessage(e.message);
                                $("#" + $(event.target).prop("id")).removeAttr("checked");
                            });
                        break;
                    case OperationName.SET_PREF_BASED_ON_VERIFICATION:
                        self.manifest.add($(event.target).prop("id"),
                            // success
                            () => {
                                console.log("success");
                            },
                            // error
                            e => {
                                console.log("failure " + <string>$(event.target).prop("id"));
                                self.displayMessage(e.message);
                                $("#" + $(event.target).prop("id")).removeAttr("checked");
                            });
                        break;
                    case OperationName.SET_PREF_EMAIL_VER:
                        self.manifest.add($(event.target).prop("id"),
                            // success
                            () => {
                                console.log("success");
                            },
                            // error
                            e => {
                                console.log("failure " + <string>$(event.target).prop("id"));
                                self.displayMessage(e.message);
                                $("#" + $(event.target).prop("id")).removeAttr("checked");
                            });
                        break;
                    default:
                        self.manifest.add($(event.target).prop("id"),
                        // success callback
                            null,
                            // error callback
                            e => {
                                console.log("error adding operation " + $(event.target).prop("id"));
                                self.displayMessage(e.message);
                            });
                        break;
                }
            // remove Operation
            else {
                self.manifest.remove($(event.target).prop("id"));
            }
        }

        // submits using pre-defined manifest
        submitUsingPredefinedManifest(event: JQueryEventObject) {
            var url;
            if (<string>$(event.target).text() === "Download Manifest") {
                url = "/Batch/DownloadManifest";
            } else {
                url = "/Batch/DynamicAppend";
            }
            $.getJSON("/Batch/GetPredefinedManifest", { manifestName: <string>$(event.target).prop("id") }).done(data => {
                // submit form
                $("#form1").remove();
                $("body").append($("<form action='" + url + "' method='POST' id='form1'>" +
                    "<input type='hidden' name='manifest' value='" + JSON.stringify(data) + "' />" +
                    "<input type='hidden' name='suppressionid' value='" + this.manifest.supressionid() + "' />" +
                    "</form>"));
                $("#form1").submit();
            });
        }

        // displays error message
        displayMessage(message: string) {
            if (message)
                $("#alert").removeAttr("class").addClass("alert alert-danger").text(message).show();
            else
                $("#alert").hide();
        }

        // submit to Controller
        submit() {
            var url = "/Batch/DynamicAppend";
            $("#form1").remove();
            $("body").append($("<form action='" + url + "' method='POST' id='form1'>" +
                "<input type='hidden' name='manifest' value='" + this.manifest.toJson() + "' />" +
                "<input type='hidden' name='suppressionid' value='" + this.manifest.supressionid() + "' />" +
                "</form>"));
            $("#form1").submit();
        }

        download() {
            var url = "/Batch/DownloadManifest";
            $("#form1").remove();
            $("body").append($("<form action='" + url + "' method='POST' id='form1'>" +
                "<input type='hidden' name='manifest' value='" + this.manifest.toJson() + "' />" +
                "</form>"));
            $("#form1").submit();
        }

        getButtonText() {
            return $("#Category").val() === "DownloadManifest" ? "Download Manifest" : "Next";
        }

        nextClick() {
            if ($("#Category").val() === "DownloadManifest")
                buildManifestViewModel.download();
            else
                buildManifestViewModel.submit();
        }

        setSupressionId() {
            // http://stackoverflow.com/questions/7905929/how-to-test-valid-uuid-guid
            $("#addSuppressionIdModelAlert").hide();
            var suppressionid = $("#txtSupressionId").val().toLowerCase();
            var regex = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
            if (regex.exec(suppressionid) != null) {
                console.log("suppression appears to be valid");
                this.manifest.supressionid(suppressionid);
                $("#addSuppressionIdModel").modal("hide");
                return true;
            } else {
                console.log("suppression appears to be invalid");
                $("#addSuppressionIdModelAlert").text("The value you entered does not appear to a GUID.").show();
            }
        }

        openSupressionModal() {
            $("#addSuppressionIdModel").modal("show");
        }
    }

}

//var manifest: AccurateAppend.DynamicAppend.Manifest;
var buildManifestViewModel: AccurateAppend.DyanmicAppend.BuildManifest.ViewModel;

$(() => {

    buildManifestViewModel = new AccurateAppend.DyanmicAppend.BuildManifest.ViewModel();
    ko.applyBindings(buildManifestViewModel);

    $("#phoneAppendAccordion").collapse();

});

