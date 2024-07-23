// abient declaration used to tell TS about the existance of the variable
declare let lookupUrls: Array<AccurateAppend.ListBuilder.Url>;
declare let requestId: string;

module AccurateAppend.ListBuilder {

  export class ViewModel {

    listCriteriaViewModel: ListCriteriaViewModel;

    // tab specific view models
    geographyTabViewModel: GeographyTabViewModel;
    demographicsTabViewModel: DemographicsTabViewModel;
    financesTabViewModel: FinancesTabViewModel;
    interestsTabViewModel: InterestsTabViewModel;

    listDownloadUri = ko.observable("");
    listDefintionDownloadUri = ko.observable("");

    constructor(public urls: Array<Url>, requestid: string) {
      this.listCriteriaViewModel = new ListCriteriaViewModel(urls, requestid);
      // tabs
      this.geographyTabViewModel = new GeographyTabViewModel(urls);
      this.demographicsTabViewModel = new DemographicsTabViewModel();
      this.financesTabViewModel = new FinancesTabViewModel();
      this.interestsTabViewModel = new InterestsTabViewModel();
    }

    downloadList() {
        $("#listCriteriaDisplay").find("#message").remove();
      const count = this.listCriteriaViewModel.count();
      if (count > 200000 && count < 600000) {
        $("#listCriteriaDisplay")
          .prepend(
            `<div style="padding: 8px;" class="alert alert-warning" id="message">Larger lists can take up to 5 minutes to build. Don't close your browser.</div>`);
        return false;
      } else if (this.listCriteriaViewModel.count() >= 600000) {
        $("#listCriteriaDisplay")
          .prepend(
            `<div style="padding: 8px;" class="alert alert-danger" id="message">List to large to download. Please add criteria or contact support.</div>`);
        return false;
      }
      $("#listBuildWaiting").show();
      $("#downloadList").attr("disabled", "disabled").find("[class='fa fa-refresh fa-spin']").show();
      $.post("/ListBuilder/BuildList/Query",
        { listCriteria: ko.toJSON(this.listCriteriaViewModel) },
        data => {
          if (data.HttpStatusCodeResult === 200) {
            this.listDownloadUri(data.ListDownloadUri);
            this.listDefintionDownloadUri(data.ListDefintionDownloadUri);
            window.location.replace(this.listDownloadUri());
          } else {
            $("#listCriteriaDisplay")
              .prepend(`<div style="padding: 8px;" class="alert alert-warning" id="message">${data.Message}</div>`);
          }
          $("#listBuildWaiting").hide();
          $("#downloadList").removeAttr("disabled").find("[class='fa fa-refresh fa-spin']").hide();
        });
      return true;
    }

    downloadDefintion() {
      window.location.replace(this.listDefintionDownloadUri());
    }

    enhanceList() {
      $("#listCriteriaDisplay").find("#message").remove();
      if (this.listCriteriaViewModel.count() > 400000) {
        $("#listCriteriaDisplay")
          .prepend(
            `<div style="padding: 8px;" class="alert alert-danger" id="message">List to large to enhance. Please add criteria or contact support.</div>`);
        return false;
      }
      $("#listCriteriaDisplay").find("#listBuildWaiting").show();
      $("#enhanceList").attr("disabled", "disabled").find("[class='fa fa-refresh fa-spin']").show();
      $.post("/ListBuilder/BuildList/Create",
        { listCriteria: ko.toJSON(this.listCriteriaViewModel) },
        data => {
          if (data.HttpStatusCodeResult === 200) {
            $("#listCriteriaDisplay")
              .prepend(
                `<div style="padding: 8px;" class="alert alert-success" id="message">List being generated. You will be redirected shortly.</div>`);
            window.location.replace(data.DownloadUri);
          } else {
            $("#listCriteriaDisplay")
              .prepend(`<div style="padding: 8px;" class="alert alert-warning" id="message">${data.Message}</div>`);
          }
          $("#listCriteriaDisplay").find("#listBuildWaiting").hide();
          $("#listCriteriaDisplay").find("#enhanceList").removeAttr("disabled").find("[class='fa fa-refresh fa-spin']")
            .hide();
        });
      return true;
    }

    generateUuid() {
      let d = new Date().getTime();
      const uuid = "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g,
        c => {
          var r = (d + Math.random() * 16) % 16 | 0;
          d = Math.floor(d / 16);
          return (c === "x" ? r : (r & 0x7 | 0x8)).toString(16);
        });
      return uuid;
    };

    numberWithCommas(x) {
      return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
  }

}

let viewModel1: AccurateAppend.ListBuilder.ViewModel;

$(() => {

  viewModel1 = new AccurateAppend.ListBuilder.ViewModel(lookupUrls, requestId);
  //if ($("#listCriteria").val()) viewModel1.listCriteriaViewModel.fromJson($("#listCriteria").val());
  ko.applyBindings(viewModel1);

  $("#dob_start").kendoMaskedTextBox({ mask: "00-0000" });
  $("#dob_end").kendoMaskedTextBox({ mask: "00-0000" });

});