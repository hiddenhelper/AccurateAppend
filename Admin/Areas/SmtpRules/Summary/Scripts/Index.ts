/// <reference path="../../../../scripts/typings/kendo-ui/kendo-ui.d.ts" />
/// <reference path="../../../../scripts/typings/knockout/knockout.d.ts" />
/// <reference path="../../../../scripts/typings/moment/moment.d.ts" />

var smtpRulesSummaryViewModel: any;
var userId: string;
var manifestId: string;
var smtpRuleLinks: any;

$(() => {

  smtpRulesSummaryViewModel = new SmtpRulesSummary.ViewModel(userId);
  smtpRulesSummaryViewModel.load();
  ko.applyBindings(smtpRulesSummaryViewModel);

  $("#files").kendoUpload({
    async: {
      saveUrl: "save",
      //autoUpload: true
    }
  });

  $("#newAutoMapRule").bind("click",
    () => { window.location.replace(smtpRuleLinks.NewAutoMappedRule); });
  $("#newFixedMapRule").bind("click",
    () => { window.location.replace(smtpRuleLinks.NewMappedRule); });

});

module SmtpRulesSummary {

  export class ViewModel {

    rules: KnockoutObservableArray<Rule>;
    currentRule: KnockoutObservable<Rule>;
    userid: string;

    constructor(userid: any) {
      this.rules = ko.observableArray();
      this.currentRule = ko.observable();
      this.userid = userid;
    }

    openDeleteConfirmationModal(rule) {
      const viewModel = smtpRulesSummaryViewModel;
      viewModel.currentRule(rule);
      $("#delete-rule-modal").modal("show");
    };

    delete(rule) {
      const viewModel = smtpRulesSummaryViewModel;
      $.ajax(
        {
          context: this,
          type: "GET",
          url: `${smtpRuleLinks.Delete}/?id=${rule.rid()}`,
          success(data) {
            console.log(`delete() success`);
            $("#delete-rule-modal").modal("hide");
            viewModel.load();
            displayMessage("#globalMessage", "Rule successfully deleted", "success");
          },
          error(xhr) {
            $("#error").html(`<strong>Error:</strong> Unable to delete rule. Message: ${xhr.statusText}`).show();
          }
        });
    };

    save(rule) {
      const viewModel = smtpRulesSummaryViewModel;
      $.ajax(
        {
          type: "POST",
          url: `${smtpRuleLinks.Update}`,
          data: { json: ko.toJSON({ "Rules": viewModel.rules, UserId: userId }) },
          success(data) {
            $("#edit-rule-modal").modal("hide");
            displayMessage("#globalMessage", data.Message, "success");
            viewModel.load();
          },
          error(xhr) {
            $("#error").html(`<strong>Error:</strong> Unable to save rule. Message: ${xhr.statusText}`).show();
          }
        });
    };

    load() {
      const viewModel = smtpRulesSummaryViewModel;
      viewModel.rules.removeAll();
      $.ajax(
        {
          context: this,
          type: "GET",
          url: `${smtpRuleLinks.ForCurrentUser}`,
          success(data) {
            $.each(data,
              (i, e) => {
                viewModel.rules.push(new Rule(
                  e.UserId,
                  e.ManifestId,
                  e.DateAdded,
                  e.RunOrder,
                  e.rid,
                  e.Terms,
                  e.Description,
                  (e.Default ? true : false),
                  e.Subject,
                  e.FileName,
                  e.Body
                ));
              });

            // manifestId is posted from DynamicAppend if a new rule is being created, add new rule to collection
            if (manifestId) {
              const rule = new Rule(userId,
                manifestId,
                moment().format("MM/DD/YYYY HH:mm:ss a"),
                    0);
              
              viewModel.rules.push(rule);
                this.editRule(rule);
              manifestId = null;
                displayMessage("#edit-rule-modal #message", "Please update the Terms and Description, and click Save", "info");
            }
          },
          error(xhr) {
            $("#error").html(`<strong>Error:</strong> Unable to retrieve rules. Message: ${xhr.statusText}`).show();
          }
        });
    };

    downloadManifest(rule) {
      window.location.replace(`${smtpRuleLinks.Download}/?id=${rule.manifestId}`);
    }

    editRule = (rule) => {
      this.currentRule(rule);
      $("#edit-rule-modal").modal("show");
    };

    setDefault(rule: Rule) {

    }
  }

  export class Rule {
    userid: any;
    manifestId: any;
    dateAdded: any;
    order: any;
    rid: any;
    terms: KnockoutObservable<string>;
    description: KnockoutObservable<string>;
    isDefault: KnockoutObservable<boolean>;
    subject: KnockoutObservable<boolean>;
    fileName: KnockoutObservable<boolean>;
    body: KnockoutObservable<boolean>;

    constructor(
      userid: any,
      manifestId: any,
      dateAdded: any,
      order?: any,
      rid?: any,
      terms?: any,
      description?: any,
      isDefault?: any,
      subject?: any,
      fileName?: any,
      body?: any
    ) {
      this.userid = userid;
      this.manifestId = manifestId;
      this.dateAdded = dateAdded;
      this.order = order;
      this.rid = ko.observable(rid);
      this.terms = ko.observable(terms);
      this.description = ko.observable(description);
      this.isDefault = ko.observable(isDefault);
      this.subject = ko.observable(subject);
      this.fileName = ko.observable(fileName);
      this.body = ko.observable(body);
    }
  }

  function displayMessage(selector: any, message: any, type: any) {
    $(selector).removeClass().addClass(`alert alert-${type}`).html(message).show()
      .fadeTo(10000, 500).slideUp(500, () => { $("#globalMessage").slideUp(500) });
  }

}