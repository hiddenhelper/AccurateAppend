/// <reference path="../../../../Scripts/typings/kendo-ui/kendo-ui.d.ts" />
/// <reference path="../../../../Scripts/typings/HttpStatusCode.ts" />
/// <reference path="../../../../scripts/app/shared/util.ts" />

declare let forCurrentUser: string;
var viewModel: any;

$(document).ready(() => {

  viewModel = new AccurateAppend.Order.Box.ViewModel(forCurrentUser);
  viewModel.init();

});

module AccurateAppend.Order.Box {
  export class ViewModel {

    registration: BoxRegistration;

    constructor(public forCurrentUser: string) {
    }

    init() {
      this.loadRegistrationDetails();
    }

    loadRegistrationDetails() {
      $.ajax({
        context: this,
        url: forCurrentUser,
        dataType: "json",
        type: "GET",
        success(result) {
          if (result.HttpStatus === 500) {
            Util.displayMessage("globalMessage", result.Message, "warning");
            return;
          }
          const data = result.data;
          $.each(data,
            () => {
              this.registration = new BoxRegistration(
                data.Name,
                data.Id,
                data.UserId,
                data.DateRegistered,
                data.IsActive,
                new BoxActions(
                  data.Actions.Enumerate,
                  data.Actions.Renew,
                  data.Actions.ChangeAccess)
              );
            });
          this.loadTreeView(this.registration);
        }
      });
    }

    loadTreeView(registration: any) {
      const ds = new kendo.data.HierarchicalDataSource({
        transport: {
          read: {
            url: registration.Actions.Enumerate,
            dataType: "json"
          }
        },
        schema: {
          model: {
            id: "NodeId",
            hasChildren: "HasChildren"
          }
        }
      });

      $("#treeview").kendoTreeView({
        dataSource: ds,
        dataTextField: "Name",
        select (e) {
          const node = e.node;
          const treeview = $("#treeview").data("kendoTreeView");
          const dataItem = treeview.dataItem(node);
          $.ajax({
            context: this,
            url: dataItem["Actions"].Details,
            dataType: "json",
            type: "GET",
            success(result) {
              $("#fileDetailPane").show();
              $("#details").html(kendo.template($("#file-detail").html())(result));
            }
          });
        }
      });
    }
  }

  class BoxRegistration {
    constructor(
      public Name: string,
      public Id: number,
      public UserId: string,
      public DateRegistered: string,
      public IsActive: boolean,
      public Actions: BoxActions
    ) {
    }
  }

  class BoxActions {
    constructor(
      public Enumerate: string,
      public Renew: string,
      public ChangeAccess: string
    ) {
    }
  }
}