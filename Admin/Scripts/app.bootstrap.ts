/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="AccurateAppend.Ui.ts" />
$(() => {

    AccurateAppend.Ui.ApplicationId.load();

    AccurateAppend.Bootstrap.updateUserStatus();
    AccurateAppend.Bootstrap.updateDealSidebar();
    window.setInterval(AccurateAppend.Bootstrap.updateDealSidebar, 60000);
    window.setInterval(AccurateAppend.Bootstrap.updateUserStatus, 60000);

});

module AccurateAppend.Bootstrap {

    export function updateUserStatus() {
        $.ajax(
            {
                type: "GET",
                async: true,
                url: "/Operations/UserStatus/ActivitySummary",
                success: result => {
                    var html = "";
                    $.each(result, (i, v) => {

                        html += "<li><i class=\"fa fa-user\" ";
                        switch (v.Status) {
                        case "Online":
                            html += "style=\"color:green;\"";
                        case "Away":
                            html += "style=\"color:gold;\"";
                        case "Offline":
                            html += "style=\"color:black;\"";
                        }
                        html += "></i> ";
                        html += "<span class=\"fa fa-user;\"></span>" + v.UserName;
                        if (v.LastActivityDescription != '') html += "<span style=\"margin-left: 13px; font-size: .8em; display:block;color:#d0d0d0;\">" + v.LastActivityDescription + "</span>";
                        html += "</li>";
                    });

                    $(".userStatus").html(html);
                }
            });
    }

    export function updateDealSidebar() {

        // hide if page does not contain Application drop down
        if ($('#ApplicationId').val() == undefined) {
            $("#dealActivity").hide();
        } else {
            $.ajax(
                {
                    type: "GET",
                    async: true,
                    url: "/Sales/DealActivity/Query",
                    success: data => {
                        if (data.length === 0) {
                            $("#dealActivity .alert").remove();
                            $("#dealActivity").append('<div class="alert alert-info">No deals found</div>');
                        } else {
                            $("#dealActivity .alert").remove();
                            $("#dealActivity ul").remove();
                            $("#dealActivity").append('<ul style="padding: 0 0 0 10px;" class="activity"></ul>');
                            $.each(data, (i, e) => {
                                $("#dealActivity ul").append(`<li><a href="${e.Links.DetailView}">${e.StatusDescription} (${e.Count})</a></li>`);
                            });
                        }
                    }
                });
        }
    }


    
}