var AccurateAppend;
(function (AccurateAppend) {
    var Websites;
    (function (Websites) {
        var Clients;
        (function (Clients) {
            var NationBuilder;
            (function (NationBuilder) {
                var DisplayListsView = (function () {
                    function DisplayListsView() {
                    }
                    DisplayListsView.prototype.initialize = function (viewModel) {
                        var _this = this;
                        console.log("initialize DisplayListsView");
                        this.ViewModel = viewModel;
                        this.populateNationsDropDown();
                        var self = this;
                        $("#nations").change(function () {
                            console.log("nations change firing. value = " + this.value);
                            $("#error").hide();
                            if (this.value === "AddNew") {
                                history.pushState(null, "DisplayLists", this.ViewModel.Index_DisplayLists_NationBuilder_Url);
                                window.location.replace(this.ViewModel.Index_Renew_NationBuilder_Url);
                            }
                            else {
                                console.log("nations change break 1");
                                $("#selectedListName").val("");
                                var autocomplete = $("#listNames").data("kendoAutoComplete");
                                autocomplete.value("");
                                autocomplete.dataSource.read();
                                self.renderGrid();
                            }
                        });
                        $("#clearFilter").click(function () {
                            history.pushState(null, "DisplayLists", "/NationBuilder/DisplayLists");
                            window.location.replace(_this.ViewModel.Index_DisplayLists_NationBuilder_Url);
                        });
                        this.initKendoUi();
                    };
                    DisplayListsView.prototype.initKendoUi = function () {
                        var self = this;
                        $("#listNames").kendoAutoComplete({
                            dataSource: {
                                type: "json",
                                schema: {
                                    type: "json",
                                    data: "Data",
                                    total: function (response) {
                                        return response.length;
                                    }
                                },
                                transport: {
                                    read: function (options) {
                                        $.ajax({
                                            url: self.ViewModel.GetListNameJson_DisplayLists_NationBuilder_Url,
                                            dataType: "json",
                                            type: "GET",
                                            data: { id: $("#nations").val() },
                                            success: function (result) {
                                                options.success(result);
                                            }
                                        });
                                    }
                                }
                            },
                            filter: "startswith",
                            placeholder: "Search by list name...",
                            dataTextField: "name",
                            select: function (e) {
                                var dataItem = this.dataItem(e.item.index());
                                $("#selectedListName").val(dataItem.name);
                                $("#selectedListId").val(dataItem.id);
                                self.renderGrid();
                            },
                            template: "<span >#: data.name #</span>",
                            change: function () {
                                var value = this.value();
                                if (value === "") {
                                    $("#selectedListName").val("");
                                    $("#selectedListId").val("");
                                    self.renderGrid();
                                }
                            }
                        });
                    };
                    DisplayListsView.prototype.populateNationsDropDown = function () {
                        var _this = this;
                        var self = this;
                        console.log("populateNationsDropDown firing");
                        $.getJSON(this.ViewModel.GetNationsForUserJson_DisplayLists_NationBuilder_Url, function (data) {
                            $("#nationSelectHolder")
                                .append("<select id=\"nations\" class=\"form-control\" style=\"width: 300px;\">")
                                .change(function (e) {
                                $("#error").hide();
                                if ($(e.target).val() === "AddNew") {
                                    history.pushState(null, "DisplayLists", _this.ViewModel.Index_DisplayLists_NationBuilder_Url);
                                    window.location.replace(_this.ViewModel.Index_Renew_NationBuilder_Url);
                                }
                                else {
                                    console.log("nations change break 1");
                                    $("#selectedListName").val("");
                                    var autocomplete = $("#listNames").data("kendoAutoComplete");
                                    autocomplete.value("");
                                    autocomplete.dataSource.read();
                                    self.renderGrid();
                                }
                            });
                            $("#nations").append("<option value='AddNew'>Add new nation</option>");
                            $.each(data, function (index, value) {
                                if (data.length === 1)
                                    $("#nations").append("<option selected='selected' value=" + value.Id + ">" + value.NationName + "</option>");
                                else if (value.IsActive)
                                    $("#nations").append("<option selected='selected' value=" + value.Id + ">" + value.NationName + "</option>");
                                else
                                    $("#nations").append("<option value=" + value.Id + ">" + value.NationName + "</option>");
                            });
                            self.renderGrid();
                        });
                    };
                    DisplayListsView.prototype.cmdClick = function (e) {
                        e.preventDefault();
                        var listId = $(e.target).closest("tr").find("td:eq(0)").text();
                        var recordCount = parseFloat($(e.target).closest("tr").find("td:eq(2)").text().replace(/\,/g, ""));
                        var listName = encodeURIComponent($(e.target).closest("tr").find("td:eq(1)").text());
                        if (recordCount === 0) {
                            $("#error span")
                                .html("The list you have chosen does not appear to contain any records and can not be processed.");
                            $("#error").show();
                            return false;
                        }
                        else if (Math.round(recordCount) > maxListSize) {
                            $("#error span")
                                .html("The list you have chosen exceeds the " + maxListSize.toLocaleString() + " record processing limit for self-service. Please contact customer support and we will submit the list for you.");
                            $("#error").show();
                            return false;
                        }
                        history.pushState(null, "NationBuilder lists", this.ViewModel.Index_DisplayLists_NationBuilder_Url);
                        window.location.replace("/NationBuilder/Order/SelectList?cartId=" + this.ViewModel.CartId + "&listId=" + listId + "&listName=" + listName + "&recordCount=" + recordCount + "&regId=" + $("#nations").val());
                        return false;
                    };
                    DisplayListsView.prototype.renderGrid = function () {
                        var self = this;
                        console.log("calling renderGrid()");
                        $("#error").hide();
                        var nationName = $("#nations option:selected").text();
                        $.ajax({
                            url: self.ViewModel.CheckRegistrationValidToken_DisplayLists_NationBuilder_Url,
                            dataType: "json",
                            type: "GET",
                            async: false,
                            data: { nationName: $("#nations option:selected").text() },
                            success: function (token) {
                                if (token.HttpStatusCodeResult === AccurateAppend.Web.HttpStatusCode.FOUND) {
                                    console.log("CheckRegistrationValidToken returned 302");
                                    $("#error span").html("We contacted NationBuilder and our access to your nation, '" + nationName + "' appears to have been removed. <a href=\"/NationBuilder/Renew?slug=" + nationName + "\" class=\"alert-link\">Click here to renew your access.</a>");
                                    $("#error").show();
                                    $("#listDisplay").hide();
                                    $("#grid").hide();
                                }
                                else if (token.HttpStatusCodeResult === AccurateAppend.Web.HttpStatusCode.GONE ||
                                    token.HttpStatusCodeResult == AccurateAppend.Web.HttpStatusCode.BAD_REQUEST) {
                                    console.log("CheckRegistrationValidToken returned 400");
                                    $("#error span")
                                        .html("We contacted NationBuilder and the Nation '" + nationName + "' appears to have been closed. Please contact support.");
                                    $("#error").show();
                                    $("#listDisplay").hide();
                                    $("#grid").hide();
                                }
                                else {
                                    $("#listDisplay").show();
                                    console.log("CheckRegistrationValidToken returned " + token.HttpStatusCodeResult);
                                    var grid = $("#grid").data("kendoGrid");
                                    if (grid !== undefined && grid !== null) {
                                        grid.dataSource.read();
                                    }
                                    else {
                                        var dsOptions = {
                                            type: "json",
                                            schema: {
                                                type: "json",
                                                data: "Data",
                                                total: function (response) {
                                                    return response.Data.length;
                                                }
                                            },
                                            pageSize: 10,
                                            change: function () {
                                                var autocomplete = $("#listNames").data("kendoAutoComplete");
                                                if (this.total() <= 0) {
                                                    $("#warning span").html("No lists found for " + nationName + ". This usually occurs when your nation has not been populated with data. <a href=\"http://nationbuilder.com/how_to_create_a_list\" class=\"alert-link\" target=\"_blank\">Click here for NationBuilder's how-to on creating lists.</a>");
                                                    $("#warning").show();
                                                    $("#listDisplay").hide();
                                                    autocomplete.enable(false);
                                                    autocomplete.value("");
                                                }
                                                else {
                                                    $("#warning").hide();
                                                    $("#listDisplay").show();
                                                    autocomplete.enable(true);
                                                }
                                            }
                                        };
                                        $("#grid").kendoGrid({
                                            autoBind: true,
                                            dataSource: {
                                                type: "json",
                                                schema: {
                                                    type: "json",
                                                    data: "Data",
                                                    total: function (response) {
                                                        return response.Data.length;
                                                    }
                                                },
                                                pageSize: 10,
                                                transport: {
                                                    read: function (options) {
                                                        $("#warning").hide();
                                                        $.ajax({
                                                            url: self.ViewModel.GetListsJson_DisplayLists_NationBuilder_Url,
                                                            dataType: "json",
                                                            type: "GET",
                                                            data: {
                                                                listname: encodeURIComponent($("#selectedListName").val()),
                                                                listid: $("#selectedListId").val(),
                                                                Id: $("#nations").val()
                                                            },
                                                            success: function (result) {
                                                                if (result.HttpStatusCodeResult === 500) {
                                                                    console.log("GetListsJson returned 500 status.");
                                                                    $("#grid").hide();
                                                                    $("#error span")
                                                                        .html("We were unable to retrieve the lists for " + nationName + ". Please contact customer support.");
                                                                    $("#error").show();
                                                                }
                                                                else {
                                                                    $("#grid").show();
                                                                    $("#error").hide();
                                                                    options.success(result);
                                                                }
                                                                $("#grid").find("[data-role=pager]").css({ "margin-bottom": 0 });
                                                            }
                                                        });
                                                    }
                                                }
                                            },
                                            scrollable: false,
                                            sortable: true,
                                            pageable: {
                                                input: true,
                                                numeric: false
                                            },
                                            filterable: {
                                                extra: false,
                                                operators: {
                                                    string: {
                                                        eq: "Equals",
                                                        contains: "Contains"
                                                    }
                                                }
                                            },
                                            columns: [
                                                {
                                                    field: "id",
                                                    title: "Id",
                                                    filterable: true,
                                                    sortable: true,
                                                    headerAttributes: { style: "text-align: center;" },
                                                    attributes: { style: "text-align: center;" },
                                                    media: "(min-width: 450px)"
                                                },
                                                {
                                                    field: "name",
                                                    title: "Name",
                                                    filterable: true,
                                                    sortable: true,
                                                    media: "(min-width: 450px)"
                                                },
                                                {
                                                    field: "count",
                                                    title: "Count",
                                                    filterable: false,
                                                    sortable: true,
                                                    format: "{0:n0}",
                                                    headerAttributes: { style: "text-align: center;" },
                                                    attributes: { style: "text-align: center;" },
                                                    media: "(min-width: 450px)"
                                                },
                                                {
                                                    command: [
                                                        {
                                                            template: '<a href="##" onClick="displayListView.cmdClick(event)">Get Estimate<i class="icon-arrow-right"></i></a>'
                                                        }
                                                    ],
                                                    title: " ",
                                                    width: "180px",
                                                    media: "(min-width: 450px)"
                                                },
                                                {
                                                    title: "Summary",
                                                    template: kendo.template($("#responsive-column-template").html()),
                                                    media: "(max-width: 450px)"
                                                }
                                            ]
                                        });
                                    }
                                }
                            }
                        });
                    };
                    return DisplayListsView;
                }());
                NationBuilder.DisplayListsView = DisplayListsView;
            })(NationBuilder = Clients.NationBuilder || (Clients.NationBuilder = {}));
        })(Clients = Websites.Clients || (Websites.Clients = {}));
    })(Websites = AccurateAppend.Websites || (AccurateAppend.Websites = {}));
})(AccurateAppend || (AccurateAppend = {}));
var displayListView;
function initializeView(viewModel) {
    $(document).ready(function () {
        displayListView = new AccurateAppend.Websites.Clients.NationBuilder.DisplayListsView();
        displayListView.initialize(viewModel);
    });
}
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiSW5kZXguanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJJbmRleC50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFHQSxJQUFPLGNBQWMsQ0FnVnBCO0FBaFZELFdBQU8sY0FBYztJQUFDLElBQUEsUUFBUSxDQWdWN0I7SUFoVnFCLFdBQUEsUUFBUTtRQUFDLElBQUEsT0FBTyxDQWdWckM7UUFoVjhCLFdBQUEsT0FBTztZQUFDLElBQUEsYUFBYSxDQWdWbkQ7WUFoVnNDLFdBQUEsYUFBYTtnQkFnQmxEO29CQUFBO29CQStUQSxDQUFDO29CQTVUQyxxQ0FBVSxHQUFWLFVBQVcsU0FBaUM7d0JBQTVDLGlCQTJCQzt3QkExQkMsT0FBTyxDQUFDLEdBQUcsQ0FBQyw2QkFBNkIsQ0FBQyxDQUFDO3dCQUMzQyxJQUFJLENBQUMsU0FBUyxHQUFHLFNBQVMsQ0FBQzt3QkFDM0IsSUFBSSxDQUFDLHVCQUF1QixFQUFFLENBQUM7d0JBQy9CLElBQUksSUFBSSxHQUFHLElBQUksQ0FBQzt3QkFDaEIsQ0FBQyxDQUFDLFVBQVUsQ0FBQyxDQUFDLE1BQU0sQ0FBQzs0QkFDbkIsT0FBTyxDQUFDLEdBQUcsQ0FBQyxvQ0FBa0MsSUFBSSxDQUFDLEtBQU8sQ0FBQyxDQUFDOzRCQUM1RCxDQUFDLENBQUMsUUFBUSxDQUFDLENBQUMsSUFBSSxFQUFFLENBQUM7NEJBQ25CLElBQUksSUFBSSxDQUFDLEtBQUssS0FBSyxRQUFRLEVBQUU7Z0NBQzNCLE9BQU8sQ0FBQyxTQUFTLENBQUMsSUFBSSxFQUFFLGNBQWMsRUFBRSxJQUFJLENBQUMsU0FBUyxDQUFDLG9DQUFvQyxDQUFDLENBQUM7Z0NBQzdGLE1BQU0sQ0FBQyxRQUFRLENBQUMsT0FBTyxDQUFDLElBQUksQ0FBQyxTQUFTLENBQUMsNkJBQTZCLENBQUMsQ0FBQzs2QkFDdkU7aUNBQU07Z0NBQ0wsT0FBTyxDQUFDLEdBQUcsQ0FBQyx3QkFBd0IsQ0FBQyxDQUFDO2dDQUN0QyxDQUFDLENBQUMsbUJBQW1CLENBQUMsQ0FB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K1Q1QixDQUFBO1lBQ0gsQ0FBQyxFQWhWc0MsYUFBYSxHQUFiLHFCQUFhLEtBQWIscUJBQWEsUUFnVm5EO1FBQUQsQ0FBQyxFQWhWOEIsT0FBTyxHQUFQLGdCQUFPLEtBQVAsZ0JBQU8sUUFnVnJDO0lBQUQsQ0FBQyxFQWhWcUIsUUFBUSxHQUFSLHVCQUFRLEtBQVIsdUJBQVEsUUFnVjdCO0FBQUQsQ0FBQyxFQWhWTSxjQUFjLEtBQWQsY0FBYyxRQWdWcEI7QUFFRCxJQUFJLGVBQStFLENBQUM7QUFFcEYsU0FBUyxjQUFjLENBQUMsU0FBK0U7SUFDckcsQ0FBQyxDQUFDLFFBQVEsQ0FBQyxDQUFDLEtBQUssQ0FBQztRQUNoQixlQUFlLEdBQUcsSUFBSSxjQUFjLENBQUMsUUFBUSxDQUFDLE9BQU8sQ0FBQyxhQUFhLENBQUMsZ0JBQWdCLEVBQUUsQ0FBQztRQUN2RixlQUFlLENBQUMsVUFBVSxDQUFDLFNBQVMsQ0FBQyxDQUFDO0lBRXhDLENBQUMsQ0FBQyxDQUFDO0FBQ0wsQ0FBQyIsInNvdXJjZXNDb250ZW50IjpbIi8vLyA8cmVmZXJlbmNlIHBhdGg9XCIuLi8uLi8uLi8uLi9TY3JpcHRzL3R5cGluZ3MvaHR0cHN0YXR1c2NvZGUudHNcIiAvPlxyXG4vLy8gPHJlZmVyZW5jZSBwYXRoPVwiLi4vLi4vLi4vLi4vU2NyaXB0cy90eXBpbmdzL2tlbmRvLXVpL2tlbmRvLXVpLmQudHNcIiAvPlxyXG5cclxubW9kdWxlIEFjY3VyYXRlQXBwZW5kLldlYnNpdGVzLkNsaWVudHMuTmF0aW9uQnVpbGRlciB7XHJcblxyXG4gIC8vIFVzZWQgdG8gaG9sZCB0aGUgbWF4aW11bSBsaXN0IHNpemVcclxuICBkZWNsYXJlIHZhciBtYXhMaXN0U2l6ZTogbnVtYmVyO1xyXG5cclxuICBleHBvcnQgaW50ZXJmYWNlIElEaXNwbGF5TGlzdHNWaWV3TW9kZWwge1xyXG4gICAgR2V0TGlzdE5hbWVKc29uX0Rpc3BsYXlMaXN0c19OYXRpb25CdWlsZGVyX1VybDogc3RyaW5nLFxyXG4gICAgSW5kZXhfRGlzcGxheUxpc3RzX05hdGlvbkJ1aWxkZXJfVXJsOiBzdHJpbmcsXHJcbiAgICBJbmRleF9SZW5ld19OYXRpb25CdWlsZGVyX1VybDogc3RyaW5nLFxyXG4gICAgR2V0TGlzdHNKc29uX0Rpc3BsYXlMaXN0c19OYXRpb25CdWlsZGVyX1VybDogc3RyaW5nLFxyXG4gICAgR2V0TmF0aW9uc0ZvclVzZXJKc29uX0Rpc3BsYXlMaXN0c19OYXRpb25CdWlsZGVyX1VybDogc3RyaW5nLFxyXG4gICAgQ2hlY2tSZWdpc3RyYXRpb25WYWxpZFRva2VuX0Rpc3BsYXlMaXN0c19OYXRpb25CdWlsZGVyX1VybDogc3RyaW5nLFxyXG4gICAgUHJvZmlsZV9OYXRpb25zX1VybDogc3RyaW5nLFxyXG4gICAgQ2FydElkOiBzdHJpbmc7XHJcbiAgfVxyXG5cclxuICBleHBvcnQgY2xhc3MgRGlzcGxheUxpc3RzVmlldyB7XHJcbiAgICBWaWV3TW9kZWw6IElEaXNwbGF5TGlzdHNWaWV3TW9kZWw7XHJcblxyXG4gICAgaW5pdGlhbGl6ZSh2aWV3TW9kZWw6IElEaXNwbGF5TGlzdHNWaWV3TW9kZWwpIHtcclxuICAgICAgY29uc29sZS5sb2coXCJpbml0aWFsaXplIERpc3BsYXlMaXN0c1ZpZXdcIik7XHJcbiAgICAgIHRoaXMuVmlld01vZGVsID0gdmlld01vZGVsO1xyXG4gICAgICB0aGlzLnBvcHVsYXRlTmF0aW9uc0Ryb3BEb3duKCk7XHJcbiAgICAgIHZhciBzZWxmID0gdGhpcztcclxuICAgICAgJChcIiNuYXRpb25zXCIpLmNoYW5nZShmdW5jdGlvbigpIHtcclxuICAgICAgICBjb25zb2xlLmxvZyhgbmF0aW9ucyBjaGFuZ2UgZmlyaW5nLiB2YWx1ZSA9ICR7dGhpcy52YWx1ZX1gKTtcclxuICAgICAgICAkKFwiI2Vycm9yXCIpLmhpZGUoKTtcclxuICAgICAgICBpZiAodGhpcy52YWx1ZSA9PT0gXCJBZGROZXdcIikge1xyXG4gICAgICAgICAgaGlzdG9yeS5wdXNoU3RhdGUobnVsbCwgXCJEaXNwbGF5TGlzdHNcIiwgdGhpcy5WaWV3TW9kZWwuSW5kZXhfRGlzcGxheUxpc3RzX05hdGlvbkJ1aWxkZXJfVXJsKTtcclxuICAgICAgICAgIHdpbmRvdy5sb2NhdGlvbi5yZXBsYWNlKHRoaXMuVmlld01vZGVsLkluZGV4X1JlbmV3X05hdGlvbkJ1aWxkZXJfVXJsKTtcclxuICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgY29uc29sZS5sb2coXCJuYXRpb25zIGNoYW5nZSBicmVhayAxXCIpO1xyXG4gICAgICAgICAgJChcIiNzZWxlY3RlZExpc3ROYW1lXCIpLnZhbChcIlwiKTtcclxuICAgICAgICAgIGNvbnN0IGF1dG9jb21wbGV0ZSA9ICQoXCIjbGlzdE5hbWVzXCIpLmRhdGEoXCJrZW5kb0F1dG9Db21wbGV0ZVwiKTtcclxuICAgICAgICAgIGF1dG9jb21wbGV0ZS52YWx1ZShcIlwiKTtcclxuICAgICAgICAgIGF1dG9jb21wbGV0ZS5kYXRhU291cmNlLnJlYWQoKTtcclxuICAgICAgICAgIHNlbGYucmVuZGVyR3JpZCgpO1xyXG4gICAgICAgIH1cclxuICAgICAgfSk7XHJcblxyXG4gICAgICAkKFwiI2NsZWFyRmlsdGVyXCIpLmNsaWNrKCgpID0+IHtcclxuICAgICAgICBoaXN0b3J5LnB1c2hTdGF0ZShudWxsLCBcIkRpc3BsYXlMaXN0c1wiLCBcIi9OYXRpb25CdWlsZGVyL0Rpc3BsYXlMaXN0c1wiKTtcclxuICAgICAgICB3aW5kb3cubG9jYXRpb24ucmVwbGFjZSh0aGlzLlZpZXdNb2RlbC5JbmRleF9EaXNwbGF5TGlzdHNfTmF0aW9uQnVpbGRlcl9VcmwpO1xyXG4gICAgICB9KTtcclxuXHJcbiAgICAgIHRoaXMuaW5pdEtlbmRvVWkoKTtcclxuICAgIH1cclxuXHJcbiAgICBpbml0S2VuZG9VaSgpIHtcclxuICAgICAgdmFyIHNlbGYgPSB0aGlzO1xyXG5cclxuICAgICAgJChcIiNsaXN0TmFtZXNcIikua2VuZG9BdXRvQ29tcGxldGUoe1xyXG4gICAgICAgIGRhdGFTb3VyY2U6IHtcclxuICAgICAgICAgIHR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgc2NoZW1hOiB7XHJcbiAgICAgICAgICAgIHR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICBkYXRhOiBcIkRhdGFcIixcclxuICAgICAgICAgICAgdG90YWwocmVzcG9uc2UpIHtcclxuICAgICAgICAgICAgICByZXR1cm4gcmVzcG9uc2UubGVuZ3RoO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAgdHJhbnNwb3J0OiB7XHJcbiAgICAgICAgICAgIHJlYWQob3B0aW9ucykge1xyXG4gICAgICAgICAgICAgICQuYWpheCh7XHJcbiAgICAgICAgICAgICAgICB1cmw6IHNlbGYuVmlld01vZGVsLkdldExpc3ROYW1lSnNvbl9EaXNwbGF5TGlzdHNfTmF0aW9uQnVpbGRlcl9VcmwsXHJcbiAgICAgICAgICAgICAgICBkYXRhVHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgICAgICAgICB0eXBlOiBcIkdFVFwiLFxyXG4gICAgICAgICAgICAgICAgZGF0YTogeyBpZDogJChcIiNuYXRpb25zXCIpLnZhbCgpIH0sXHJcbiAgICAgICAgICAgICAgICBzdWNjZXNzOiByZXN1bHQgPT4ge1xyXG4gICAgICAgICAgICAgICAgICBvcHRpb25zLnN1Y2Nlc3MocmVzdWx0KTtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgfVxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgZmlsdGVyOiBcInN0YXJ0c3dpdGhcIixcclxuICAgICAgICBwbGFjZWhvbGRlcjogXCJTZWFyY2ggYnkgbGlzdCBuYW1lLi4uXCIsXHJcbiAgICAgICAgZGF0YVRleHRGaWVsZDogXCJuYW1lXCIsXHJcbiAgICAgICAgc2VsZWN0KGUpIHtcclxuICAgICAgICAgIGNvbnN0IGRhdGFJdGVtID0gdGhpcy5kYXRhSXRlbShlLml0ZW0uaW5kZXgoKSk7XHJcbiAgICAgICAgICAkKFwiI3NlbGVjdGVkTGlzdE5hbWVcIikudmFsKGRhdGFJdGVtLm5hbWUpO1xyXG4gICAgICAgICAgJChcIiNzZWxlY3RlZExpc3RJZFwiKS52YWwoZGF0YUl0ZW0uaWQpO1xyXG4gICAgICAgICAgc2VsZi5yZW5kZXJHcmlkKCk7XHJcbiAgICAgICAgfSxcclxuICAgICAgICB0ZW1wbGF0ZTogXCI8c3BhbiA+IzogZGF0YS5uYW1lICM8L3NwYW4+XCIsXHJcbiAgICAgICAgY2hhbmdlKCkge1xyXG4gICAgICAgICAgY29uc3QgdmFsdWUgPSB0aGlzLnZhbHVlKCk7XHJcbiAgICAgICAgICBpZiAodmFsdWUgPT09IFwiXCIpIHtcclxuICAgICAgICAgICAgJChcIiNzZWxlY3RlZExpc3ROYW1lXCIpLnZhbChcIlwiKTtcclxuICAgICAgICAgICAgJChcIiNzZWxlY3RlZExpc3RJZFwiKS52YWwoXCJcIik7XHJcbiAgICAgICAgICAgIHNlbGYucmVuZGVyR3JpZCgpO1xyXG4gICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuICAgICAgfSk7XHJcbiAgICB9XHJcblxyXG4gICAgcG9wdWxhdGVOYXRpb25zRHJvcERvd24oKSB7XHJcbiAgICAgIHZhciBzZWxmID0gdGhpcztcclxuICAgICAgY29uc29sZS5sb2coXCJwb3B1bGF0ZU5hdGlvbnNEcm9wRG93biBmaXJpbmdcIik7XHJcbiAgICAgICQuZ2V0SlNPTih0aGlzLlZpZXdNb2RlbC5HZXROYXRpb25zRm9yVXNlckpzb25fRGlzcGxheUxpc3RzX05hdGlvbkJ1aWxkZXJfVXJsLFxyXG4gICAgICAgIGRhdGEgPT4ge1xyXG4gICAgICAgICAgJChcIiNuYXRpb25TZWxlY3RIb2xkZXJcIilcclxuICAgICAgICAgICAgLmFwcGVuZChgPHNlbGVjdCBpZD1cIm5hdGlvbnNcIiBjbGFzcz1cImZvcm0tY29udHJvbFwiIHN0eWxlPVwid2lkdGg6IDMwMHB4O1wiPmApXHJcbiAgICAgICAgICAgIC5jaGFuZ2UoZSA9PiB7XHJcbiAgICAgICAgICAgICAgJChcIiNlcnJvclwiKS5oaWRlKCk7XHJcbiAgICAgICAgICAgICAgaWYgKCQoZS50YXJnZXQpLnZhbCgpID09PSBcIkFkZE5ld1wiKSB7XHJcbiAgICAgICAgICAgICAgICBoaXN0b3J5LnB1c2hTdGF0ZShudWxsLCBcIkRpc3BsYXlMaXN0c1wiLCB0aGlzLlZpZXdNb2RlbC5JbmRleF9EaXNwbGF5TGlzdHNfTmF0aW9uQnVpbGRlcl9VcmwpO1xyXG4gICAgICAgICAgICAgICAgd2luZG93LmxvY2F0aW9uLnJlcGxhY2UodGhpcy5WaWV3TW9kZWwuSW5kZXhfUmVuZXdfTmF0aW9uQnVpbGRlcl9VcmwpO1xyXG4gICAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICBjb25zb2xlLmxvZyhcIm5hdGlvbnMgY2hhbmdlIGJyZWFrIDFcIik7XHJcbiAgICAgICAgICAgICAgICAkKFwiI3NlbGVjdGVkTGlzdE5hbWVcIikudmFsKFwiXCIpO1xyXG4gICAgICAgICAgICAgICAgY29uc3QgYXV0b2NvbXBsZXRlID0gJChcIiNsaXN0TmFtZXNcIikuZGF0YShcImtlbmRvQXV0b0NvbXBsZXRlXCIpO1xyXG4gICAgICAgICAgICAgICAgYXV0b2NvbXBsZXRlLnZhbHVlKFwiXCIpO1xyXG4gICAgICAgICAgICAgICAgYXV0b2NvbXBsZXRlLmRhdGFTb3VyY2UucmVhZCgpO1xyXG4gICAgICAgICAgICAgICAgc2VsZi5yZW5kZXJHcmlkKCk7XHJcbiAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICAgICQoXCIjbmF0aW9uc1wiKS5hcHBlbmQoXCI8b3B0aW9uIHZhbHVlPSdBZGROZXcnPkFkZCBuZXcgbmF0aW9uPC9vcHRpb24+XCIpO1xyXG4gICAgICAgICAgJC5lYWNoKGRhdGEsXHJcbiAgICAgICAgICAgIChpbmRleCwgdmFsdWUpID0+IHtcclxuICAgICAgICAgICAgICBpZiAoZGF0YS5sZW5ndGggPT09IDEpXHJcbiAgICAgICAgICAgICAgICAkKFwiI25hdGlvbnNcIikuYXBwZW5kKGA8b3B0aW9uIHNlbGVjdGVkPSdzZWxlY3RlZCcgdmFsdWU9JHt2YWx1ZS5JZH0+JHt2YWx1ZS5OYXRpb25OYW1lfTwvb3B0aW9uPmApO1xyXG4gICAgICAgICAgICAgIGVsc2UgaWYgKHZhbHVlLklzQWN0aXZlKSAvLyBzZWxlY3QgZmlyc3QgYWN0aXZlIG5hdGlvblxyXG4gICAgICAgICAgICAgICAgJChcIiNuYXRpb25zXCIpLmFwcGVuZChgPG9wdGlvbiBzZWxlY3RlZD0nc2VsZWN0ZWQnIHZhbHVlPSR7dmFsdWUuSWR9PiR7dmFsdWUuTmF0aW9uTmFtZX08L29wdGlvbj5gKTtcclxuICAgICAgICAgICAgICBlbHNlXHJcbiAgICAgICAgICAgICAgICAkKFwiI25hdGlvbnNcIikuYXBwZW5kKGA8b3B0aW9uIHZhbHVlPSR7dmFsdWUuSWR9PiR7dmFsdWUuTmF0aW9uTmFtZX08L29wdGlvbj5gKTtcclxuXHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgLy8kKFwiI25hdGlvbnNcIikucmVtb3ZlQ2xhc3MoKTtcclxuICAgICAgICAgIHNlbGYucmVuZGVyR3JpZCgpO1xyXG4gICAgICAgIH0pO1xyXG4gICAgfVxyXG5cclxuICAgIGNtZENsaWNrKGUpIHtcclxuICAgICAgZS5wcmV2ZW50RGVmYXVsdCgpO1xyXG4gICAgICBjb25zdCBsaXN0SWQgPSAkKGUudGFyZ2V0KS5jbG9zZXN0KFwidHJcIikuZmluZChcInRkOmVxKDApXCIpLnRleHQoKTtcclxuICAgICAgY29uc3QgcmVjb3JkQ291bnQgPSBwYXJzZUZsb2F0KCQoZS50YXJnZXQpLmNsb3Nlc3QoXCJ0clwiKS5maW5kKFwidGQ6ZXEoMilcIikudGV4dCgpLnJlcGxhY2UoL1xcLC9nLCBcIlwiKSk7XHJcbiAgICAgIGNvbnN0IGxpc3ROYW1lID0gZW5jb2RlVVJJQ29tcG9uZW50KCQoZS50YXJnZXQpLmNsb3Nlc3QoXCJ0clwiKS5maW5kKFwidGQ6ZXEoMSlcIikudGV4dCgpKTtcclxuICAgICAgLy8gdmFsaWRhdGUgbGlzdCBwcmlvciB0byBuZXh0IHNjcmVlblxyXG4gICAgICBpZiAocmVjb3JkQ291bnQgPT09IDApIHtcclxuICAgICAgICAkKFwiI2Vycm9yIHNwYW5cIilcclxuICAgICAgICAgIC5odG1sKFwiVGhlIGxpc3QgeW91IGhhdmUgY2hvc2VuIGRvZXMgbm90IGFwcGVhciB0byBjb250YWluIGFueSByZWNvcmRzIGFuZCBjYW4gbm90IGJlIHByb2Nlc3NlZC5cIik7XHJcbiAgICAgICAgJChcIiNlcnJvclwiKS5zaG93KCk7XHJcbiAgICAgICAgcmV0dXJuIGZhbHNlO1xyXG4gICAgICB9IGVsc2UgaWYgKE1hdGgucm91bmQocmVjb3JkQ291bnQpID4gbWF4TGlzdFNpemUpIHtcclxuICAgICAgICAkKFwiI2Vycm9yIHNwYW5cIilcclxuICAgICAgICAgIC5odG1sKFxyXG4gICAgICAgICAgICBgVGhlIGxpc3QgeW91IGhhdmUgY2hvc2VuIGV4Y2VlZHMgdGhlICR7bWF4TGlzdFNpemUudG9Mb2NhbGVTdHJpbmcoKVxyXG4gICAgICAgICAgICB9IHJlY29yZCBwcm9jZXNzaW5nIGxpbWl0IGZvciBzZWxmLXNlcnZpY2UuIFBsZWFzZSBjb250YWN0IGN1c3RvbWVyIHN1cHBvcnQgYW5kIHdlIHdpbGwgc3VibWl0IHRoZSBsaXN0IGZvciB5b3UuYCk7XHJcbiAgICAgICAgJChcIiNlcnJvclwiKS5zaG93KCk7XHJcbiAgICAgICAgcmV0dXJuIGZhbHNlO1xyXG4gICAgICB9XHJcbiAgICAgIC8vIHJlZGlyZWN0IHRvIG9yZGVyIHNjcmVlblxyXG4gICAgICBoaXN0b3J5LnB1c2hTdGF0ZShudWxsLCBcIk5hdGlvbkJ1aWxkZXIgbGlzdHNcIiwgdGhpcy5WaWV3TW9kZWwuSW5kZXhfRGlzcGxheUxpc3RzX05hdGlvbkJ1aWxkZXJfVXJsKTtcclxuICAgICAgd2luZG93LmxvY2F0aW9uLnJlcGxhY2UoXHJcbiAgICAgICAgYC9OYXRpb25CdWlsZGVyL09yZGVyL1NlbGVjdExpc3Q/Y2FydElkPSR7dGhpcy5WaWV3TW9kZWwuQ2FydElkfSZsaXN0SWQ9JHtsaXN0SWR9Jmxpc3ROYW1lPSR7bGlzdE5hbWVcclxuICAgICAgICB9JnJlY29yZENvdW50PSR7cmVjb3JkQ291bnRcclxuICAgICAgICB9JnJlZ0lkPSR7JChcIiNuYXRpb25zXCIpLnZhbCgpfWApO1xyXG4gICAgICByZXR1cm4gZmFsc2U7XHJcbiAgICB9XHJcblxyXG4gICAgcmVuZGVyR3JpZCgpIHtcclxuICAgICAgdmFyIHNlbGYgPSB0aGlzO1xyXG4gICAgICBjb25zb2xlLmxvZyhcImNhbGxpbmcgcmVuZGVyR3JpZCgpXCIpO1xyXG5cclxuICAgICAgJChcIiNlcnJvclwiKS5oaWRlKCk7XHJcblxyXG4gICAgICB2YXIgbmF0aW9uTmFtZSA9ICQoXCIjbmF0aW9ucyBvcHRpb246c2VsZWN0ZWRcIikudGV4dCgpO1xyXG4gICAgICAkLmFqYXgoe1xyXG4gICAgICAgIHVybDogc2VsZi5WaWV3TW9kZWwuQ2hlY2tSZWdpc3RyYXRpb25WYWxpZFRva2VuX0Rpc3BsYXlMaXN0c19OYXRpb25CdWlsZGVyX1VybCxcclxuICAgICAgICBkYXRhVHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgdHlwZTogXCJHRVRcIixcclxuICAgICAgICBhc3luYzogZmFsc2UsXHJcbiAgICAgICAgZGF0YTogeyBuYXRpb25OYW1lOiAkKFwiI25hdGlvbnMgb3B0aW9uOnNlbGVjdGVkXCIpLnRleHQoKSB9LFxyXG4gICAgICAgIHN1Y2Nlc3ModG9rZW4pIHtcclxuXHJcbiAgICAgICAgICBpZiAodG9rZW4uSHR0cFN0YXR1c0NvZGVSZXN1bHQgPT09IFdlYi5IdHRwU3RhdHVzQ29kZS5GT1VORCkge1xyXG4gICAgICAgICAgICBjb25zb2xlLmxvZyhcIkNoZWNrUmVnaXN0cmF0aW9uVmFsaWRUb2tlbiByZXR1cm5lZCAzMDJcIik7XHJcbiAgICAgICAgICAgICQoXCIjZXJyb3Igc3BhblwiKS5odG1sKGBXZSBjb250YWN0ZWQgTmF0aW9uQnVpbGRlciBhbmQgb3VyIGFjY2VzcyB0byB5b3VyIG5hdGlvbiwgJyR7bmF0aW9uTmFtZVxyXG4gICAgICAgICAgICAgIH0nIGFwcGVhcnMgdG8gaGF2ZSBiZWVuIHJlbW92ZWQuIDxhIGhyZWY9XCIvTmF0aW9uQnVpbGRlci9SZW5ldz9zbHVnPSR7bmF0aW9uTmFtZVxyXG4gICAgICAgICAgICAgIH1cIiBjbGFzcz1cImFsZXJ0LWxpbmtcIj5DbGljayBoZXJlIHRvIHJlbmV3IHlvdXIgYWNjZXNzLjwvYT5gKTtcclxuICAgICAgICAgICAgJChcIiNlcnJvclwiKS5zaG93KCk7XHJcbiAgICAgICAgICAgICQoXCIjbGlzdERpc3BsYXlcIikuaGlkZSgpO1xyXG4gICAgICAgICAgICAkKFwiI2dyaWRcIikuaGlkZSgpO1xyXG4gICAgICAgICAgfSBlbHNlIGlmICh0b2tlbi5IdHRwU3RhdHVzQ29kZVJlc3VsdCA9PT0gV2ViLkh0dHBTdGF0dXNDb2RlLkdPTkUgfHxcclxuICAgICAgICAgICAgdG9rZW4uSHR0cFN0YXR1c0NvZGVSZXN1bHQgPT0gV2ViLkh0dHBTdGF0dXNDb2RlLkJBRF9SRVFVRVNUKSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKFwiQ2hlY2tSZWdpc3RyYXRpb25WYWxpZFRva2VuIHJldHVybmVkIDQwMFwiKTtcclxuICAgICAgICAgICAgJChcIiNlcnJvciBzcGFuXCIpXHJcbiAgICAgICAgICAgICAgLmh0bWwoYFdlIGNvbnRhY3RlZCBOYXRpb25CdWlsZGVyIGFuZCB0aGUgTmF0aW9uICcke25hdGlvbk5hbWVcclxuICAgICAgICAgICAgICAgIH0nIGFwcGVhcnMgdG8gaGF2ZSBiZWVuIGNsb3NlZC4gUGxlYXNlIGNvbnRhY3Qgc3VwcG9ydC5gKTtcclxuICAgICAgICAgICAgJChcIiNlcnJvclwiKS5zaG93KCk7XHJcbiAgICAgICAgICAgICQoXCIjbGlzdERpc3BsYXlcIikuaGlkZSgpO1xyXG4gICAgICAgICAgICAkKFwiI2dyaWRcIikuaGlkZSgpO1xyXG4gICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgJChcIiNsaXN0RGlzcGxheVwiKS5zaG93KCk7XHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKGBDaGVja1JlZ2lzdHJhdGlvblZhbGlkVG9rZW4gcmV0dXJuZWQgJHt0b2tlbi5IdHRwU3RhdHVzQ29kZVJlc3VsdH1gKTtcclxuICAgICAgICAgICAgY29uc3QgZ3JpZCA9ICQoXCIjZ3JpZFwiKS5kYXRhKFwia2VuZG9HcmlkXCIpO1xyXG4gICAgICAgICAgICBpZiAoZ3JpZCAhPT0gdW5kZWZpbmVkICYmIGdyaWQgIT09IG51bGwpIHtcclxuICAgICAgICAgICAgICBncmlkLmRhdGFTb3VyY2UucmVhZCgpO1xyXG4gICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgIGNvbnN0IGRzT3B0aW9uczoga2VuZG8uZGF0YS5EYXRhU291cmNlT3B0aW9ucyA9IHtcclxuICAgICAgICAgICAgICAgIHR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgICAgc2NoZW1hOiB7XHJcbiAgICAgICAgICAgICAgICAgIHR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgICAgICBkYXRhOiBcIkRhdGFcIixcclxuICAgICAgICAgICAgICAgICAgdG90YWwocmVzcG9uc2UpIHtcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gcmVzcG9uc2UuRGF0YS5sZW5ndGg7XHJcbiAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICBwYWdlU2l6ZTogMTAsXHJcbiAgICAgICAgICAgICAgICBjaGFuZ2UoKSB7XHJcbiAgICAgICAgICAgICAgICAgIGNvbnN0IGF1dG9jb21wbGV0ZSA9ICQoXCIjbGlzdE5hbWVzXCIpLmRhdGEoXCJrZW5kb0F1dG9Db21wbGV0ZVwiKTtcclxuICAgICAgICAgICAgICAgICAgaWYgKHRoaXMudG90YWwoKSA8PSAwKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgJChcIiN3YXJuaW5nIHNwYW5cIikuaHRtbChgTm8gbGlzdHMgZm91bmQgZm9yICR7bmF0aW9uTmFtZVxyXG4gICAgICAgICAgICAgICAgICAgICAgfS4gVGhpcyB1c3VhbGx5IG9jY3VycyB3aGVuIHlvdXIgbmF0aW9uIGhhcyBub3QgYmVlbiBwb3B1bGF0ZWQgd2l0aCBkYXRhLiA8YSBocmVmPVwiaHR0cDovL25hdGlvbmJ1aWxkZXIuY29tL2hvd190b19jcmVhdGVfYV9saXN0XCIgY2xhc3M9XCJhbGVydC1saW5rXCIgdGFyZ2V0PVwiX2JsYW5rXCI+Q2xpY2sgaGVyZSBmb3IgTmF0aW9uQnVpbGRlcidzIGhvdy10byBvbiBjcmVhdGluZyBsaXN0cy48L2E+YCk7XHJcbiAgICAgICAgICAgICAgICAgICAgJChcIiN3YXJuaW5nXCIpLnNob3coKTtcclxuICAgICAgICAgICAgICAgICAgICAkKFwiI2xpc3REaXNwbGF5XCIpLmhpZGUoKTtcclxuICAgICAgICAgICAgICAgICAgICBhdXRvY29tcGxldGUuZW5hYmxlKGZhbHNlKTtcclxuICAgICAgICAgICAgICAgICAgICBhdXRvY29tcGxldGUudmFsdWUoXCJcIik7XHJcbiAgICAgICAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICAgICAgJChcIiN3YXJuaW5nXCIpLmhpZGUoKTtcclxuICAgICAgICAgICAgICAgICAgICAkKFwiI2xpc3REaXNwbGF5XCIpLnNob3coKTtcclxuICAgICAgICAgICAgICAgICAgICBhdXRvY29tcGxldGUuZW5hYmxlKHRydWUpO1xyXG4gICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgfTtcclxuXHJcbiAgICAgICAgICAgICAgJChcIiNncmlkXCIpLmtlbmRvR3JpZCh7XHJcbiAgICAgICAgICAgICAgICBhdXRvQmluZDogdHJ1ZSxcclxuICAgICAgICAgICAgICAgIGRhdGFTb3VyY2U6IHtcclxuICAgICAgICAgICAgICAgICAgdHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgICAgICAgICAgIHNjaGVtYToge1xyXG4gICAgICAgICAgICAgICAgICAgIHR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgICAgICAgIGRhdGE6IFwiRGF0YVwiLFxyXG4gICAgICAgICAgICAgICAgICAgIHRvdGFsKHJlc3BvbnNlKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICByZXR1cm4gcmVzcG9uc2UuRGF0YS5sZW5ndGg7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICBwYWdlU2l6ZTogMTAsXHJcbiAgICAgICAgICAgICAgICAgIHRyYW5zcG9ydDoge1xyXG4gICAgICAgICAgICAgICAgICAgIHJlYWQob3B0aW9ucykge1xyXG4gICAgICAgICAgICAgICAgICAgICAgJChcIiN3YXJuaW5nXCIpLmhpZGUoKTtcclxuICAgICAgICAgICAgICAgICAgICAgICQuYWpheCh7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHVybDogc2VsZi5WaWV3TW9kZWwuR2V0TGlzdHNKc29uX0Rpc3BsYXlMaXN0c19OYXRpb25CdWlsZGVyX1VybCxcclxuICAgICAgICAgICAgICAgICAgICAgICAgZGF0YVR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB0eXBlOiBcIkdFVFwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBkYXRhOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgbGlzdG5hbWU6IGVuY29kZVVSSUNvbXBvbmVudCgkKFwiI3NlbGVjdGVkTGlzdE5hbWVcIikudmFsKCkpLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgIGxpc3RpZDogJChcIiNzZWxlY3RlZExpc3RJZFwiKS52YWwoKSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICBJZDogJChcIiNuYXRpb25zXCIpLnZhbCgpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHN1Y2Nlc3MocmVzdWx0KSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgaWYgKHJlc3VsdC5IdHRwU3RhdHVzQ29kZVJlc3VsdCA9PT0gNTAwKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjb25zb2xlLmxvZyhcIkdldExpc3RzSnNvbiByZXR1cm5lZCA1MDAgc3RhdHVzLlwiKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICQoXCIjZ3JpZFwiKS5oaWRlKCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKFwiI2Vycm9yIHNwYW5cIilcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgLmh0bWwoYFdlIHdlcmUgdW5hYmxlIHRvIHJldHJpZXZlIHRoZSBsaXN0cyBmb3IgJHtuYXRpb25OYW1lXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfS4gUGxlYXNlIGNvbnRhY3QgY3VzdG9tZXIgc3VwcG9ydC5gKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICQoXCIjZXJyb3JcIikuc2hvdygpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKFwiI2dyaWRcIikuc2hvdygpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgJChcIiNlcnJvclwiKS5oaWRlKCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBvcHRpb25zLnN1Y2Nlc3MocmVzdWx0KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgJChcIiNncmlkXCIpLmZpbmQoXCJbZGF0YS1yb2xlPXBhZ2VyXVwiKS5jc3MoeyBcIm1hcmdpbi1ib3R0b21cIjogMCB9KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgc2Nyb2xsYWJsZTogZmFsc2UsXHJcbiAgICAgICAgICAgICAgICBzb3J0YWJsZTogdHJ1ZSxcclxuICAgICAgICAgICAgICAgIHBhZ2VhYmxlOiB7XHJcbiAgICAgICAgICAgICAgICAgIGlucHV0OiB0cnVlLFxyXG4gICAgICAgICAgICAgICAgICBudW1lcmljOiBmYWxzZVxyXG4gICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgIGZpbHRlcmFibGU6IHtcclxuICAgICAgICAgICAgICAgICAgZXh0cmE6IGZhbHNlLFxyXG4gICAgICAgICAgICAgICAgICBvcGVyYXRvcnM6IHtcclxuICAgICAgICAgICAgICAgICAgICBzdHJpbmc6IHtcclxuICAgICAgICAgICAgICAgICAgICAgIGVxOiBcIkVxdWFsc1wiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgY29udGFpbnM6IFwiQ29udGFpbnNcIlxyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgIGNvbHVtbnM6IFtcclxuICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgIGZpZWxkOiBcImlkXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgdGl0bGU6IFwiSWRcIixcclxuICAgICAgICAgICAgICAgICAgICBmaWx0ZXJhYmxlOiB0cnVlLFxyXG4gICAgICAgICAgICAgICAgICAgIHNvcnRhYmxlOiB0cnVlLFxyXG4gICAgICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwibmFtZVwiLFxyXG4gICAgICAgICAgICAgICAgICAgIHRpdGxlOiBcIk5hbWVcIixcclxuICAgICAgICAgICAgICAgICAgICBmaWx0ZXJhYmxlOiB0cnVlLFxyXG4gICAgICAgICAgICAgICAgICAgIHNvcnRhYmxlOiB0cnVlLFxyXG4gICAgICAgICAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICBmaWVsZDogXCJjb3VudFwiLFxyXG4gICAgICAgICAgICAgICAgICAgIHRpdGxlOiBcIkNvdW50XCIsXHJcbiAgICAgICAgICAgICAgICAgICAgZmlsdGVyYWJsZTogZmFsc2UsXHJcbiAgICAgICAgICAgICAgICAgICAgc29ydGFibGU6IHRydWUsXHJcbiAgICAgICAgICAgICAgICAgICAgZm9ybWF0OiBcInswOm4wfVwiLFxyXG4gICAgICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgY29tbWFuZDogW1xyXG4gICAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB0ZW1wbGF0ZTpcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAnPGEgaHJlZj1cIiMjXCIgb25DbGljaz1cImRpc3BsYXlMaXN0Vmlldy5jbWRDbGljayhldmVudClcIj5HZXQgRXN0aW1hdGU8aSBjbGFzcz1cImljb24tYXJyb3ctcmlnaHRcIj48L2k+PC9hPidcclxuICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICBdLFxyXG4gICAgICAgICAgICAgICAgICAgIHRpdGxlOiBcIiBcIixcclxuICAgICAgICAgICAgICAgICAgICB3aWR0aDogXCIxODBweFwiLFxyXG4gICAgICAgICAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICB0aXRsZTogXCJTdW1tYXJ5XCIsXHJcbiAgICAgICAgICAgICAgICAgICAgdGVtcGxhdGU6IGtlbmRvLnRlbXBsYXRlKCQoXCIjcmVzcG9uc2l2ZS1jb2x1bW4tdGVtcGxhdGVcIikuaHRtbCgpKSxcclxuICAgICAgICAgICAgICAgICAgICBtZWRpYTogXCIobWF4LXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICBdXHJcbiAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIH1cclxuICAgICAgICB9XHJcbiAgICAgIH0pO1xyXG4gICAgfVxyXG4gIH1cclxufVxyXG5cclxubGV0IGRpc3BsYXlMaXN0VmlldzogQWNjdXJhdGVBcHBlbmQuV2Vic2l0ZXMuQ2xpZW50cy5OYXRpb25CdWlsZGVyLkRpc3BsYXlMaXN0c1ZpZXc7XHJcblxyXG5mdW5jdGlvbiBpbml0aWFsaXplVmlldyh2aWV3TW9kZWw6IEFjY3VyYXRlQXBwZW5kLldlYnNpdGVzLkNsaWVudHMuTmF0aW9uQnVpbGRlci5JRGlzcGxheUxpc3RzVmlld01vZGVsKSB7XHJcbiAgJChkb2N1bWVudCkucmVhZHkoKCkgPT4ge1xyXG4gICAgZGlzcGxheUxpc3RWaWV3ID0gbmV3IEFjY3VyYXRlQXBwZW5kLldlYnNpdGVzLkNsaWVudHMuTmF0aW9uQnVpbGRlci5EaXNwbGF5TGlzdHNWaWV3KCk7XHJcbiAgICBkaXNwbGF5TGlzdFZpZXcuaW5pdGlhbGl6ZSh2aWV3TW9kZWwpO1xyXG5cclxuICB9KTtcclxufSJdfQ==