/// <reference path="../../../../Scripts/typings/httpstatuscode.ts" />
/// <reference path="../../../../views/shared/scripts/notificationhelper.ts" />
/// <reference path="../../../../Scripts/typings/kendo-ui/kendo-ui.d.ts" />

module AccurateAppend.Websites.Clients.Profile.SocialMediaLogin {

    export class IndexView {

        notification: Clients.Shared.NotificationHelper;

        constructor() {
            this.notification = new Clients.Shared.NotificationHelper('message');
        }

        //an event handler for remove button
        removeLink(url,e)
        {
            let self = this;
            this.notification.clearMessage();
            e.preventDefault();

            this.showAjaxInProgress(e.target);
            
            $.ajax({
                url: url,
                dataType: 'json',
                type: 'DELETE',
                success(result) {
                    if (result.HttpStatusCodeResult === 500) {
                        self.notification.showError('Error occured removing social medial login');
                    }
                    else {
                        if (result.Success === true) {
                            self.notification.showSuccess(result.Message);
                            var grid = $("#grid").data("kendoGrid");
                            grid.dataSource.read();
                        }
                        else {
                            self.notification.showWarning(result.Message);
                        }
                    }

                },
                complete() {
                    self.clearAjaxInProgress(e.target);
                }
            });

            return false;
        }

        //TODO : move this method to a share area for other scripts to use
        //disables and adds a loading gif while an ajax call is inprogress
        showAjaxInProgress(el) {
            $(el).addClass('ajax-in-progress');
            $(el).attr('disabled', 'true');
        }

        //TODO : move this method to a share area for other scripts to use
        //clears the loading gif and re-enables the button after an ajax call is completed 
        clearAjaxInProgress(el) {
            $(el).removeClass('ajax-in-progress');
            $(el).removeAttr("disabled");
        }

        //initialize the kendo grid
        initialize(url) {

            $("#grid").kendoGrid({
                autoBind: true,
                dataSource: {
                    type: "json",
                    schema: {
                        type: "json",
                        data: "Data",
                        total(response) {
                            return response.length;
                        }
                    },
                    change: function () {
                        if (this.data().length <= 0) {
                            $("#no-logins-message").show();
                            $("#grid").hide();
                        } else {
                            $("#no-logins-message").hide();
                            $("#grid").show();
                        }
                    },
                    transport: {
                        read(options) {
                            $.ajax({
                                url: url,
                                dataType: 'json',
                                type: 'GET',
                                data: {},
                                success(result) {
                                    if (result.HttpStatusCodeResult === 500) {
                                        console.log("Read returned 500 status.");
                                    } else {
                                        if (result.length <= 0) {
                                            $("#no-logins-message").show();
                                            $("#grid").hide();
                                        } else {
                                            $("#no-logins-message").hide();
                                            $("#grid").show();
                                        }

                                        options.success({ Data: result });
                                    }
                                }
                            });
                        }
                    }
                },
                scrollable: false,
                
                pageable: false,
                columns: [
                    {
                        field: "Name",
                        title: "Name"
                    },
                    {
                        field: "ProviderName",
                        title: "Login From"
                    },
                    {
                        template: "<button class=\"btn btn-primary\" onClick=\"socialMediaLoginIndexView.removeLink(\'#= Actions.Remove #\',event)\">remove</button>"
                    }
                ]
            });
        }

        
    }
}

let socialMediaLoginIndexView: AccurateAppend.Websites.Clients.Profile.SocialMediaLogin.IndexView;

function initializeSocialMediaLoginView(url) {
    $(document).ready(() => {
        socialMediaLoginIndexView = new AccurateAppend.Websites.Clients.Profile.SocialMediaLogin.IndexView();
        socialMediaLoginIndexView.initialize(url);
    });
}