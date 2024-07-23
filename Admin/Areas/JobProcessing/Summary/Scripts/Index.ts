/// <reference path="../../../../scripts/typings/moment/moment.d.ts" />
/// <reference path="../../../../scripts/typings/kendo-ui/kendo-ui.d.ts" />
/// <reference path="JobModel.ts" />
/// <reference path="JobReportModel.ts" />
/// <reference path="NationBuilderModel.ts" />
/// <reference path="OperationReportModel.ts" />

var JobsDateRangeWidget: AccurateAppend.Ui.DateRangeWidget;
var NBDateRangeWidget: AccurateAppend.Ui.DateRangeWidget;
var jobProcessingSummaryViewModel: AccurateAppend.JobProcessing.Summary.ViewModel;
// ambient declaration, set in View
// 
var links: any;

$(() => {

  JobsDateRangeWidget = new AccurateAppend.Ui.DateRangeWidget("jobsDateRange",
    new AccurateAppend.Ui.DateRangeWidgetSettings(
      [
        AccurateAppend.Ui.DateRangeValue.Last24Hours,
        AccurateAppend.Ui.DateRangeValue.Last7Days,
        AccurateAppend.Ui.DateRangeValue.Last30Days,
        AccurateAppend.Ui.DateRangeValue.Last60Days,
        AccurateAppend.Ui.DateRangeValue.LastMonth,
        AccurateAppend.Ui.DateRangeValue.Custom
      ],
      AccurateAppend.Ui.DateRangeValue.Last24Hours,
      [
        jobProcessingSummaryViewModel.renderCompleteGrid
      ]));

  NBDateRangeWidget = new AccurateAppend.Ui.DateRangeWidget("nbDateRange",
    new AccurateAppend.Ui.DateRangeWidgetSettings(
      [
        AccurateAppend.Ui.DateRangeValue.Last24Hours,
        AccurateAppend.Ui.DateRangeValue.Last7Days,
        AccurateAppend.Ui.DateRangeValue.Last30Days,
        AccurateAppend.Ui.DateRangeValue.Custom
      ],
      AccurateAppend.Ui.DateRangeValue.Last7Days,
      [
        jobProcessingSummaryViewModel.renderNationBuilderCompleteGrid
      ]));
  NBDateRangeWidget.refresh();

});

module AccurateAppend.JobProcessing.Summary {

  // new methods with the same names should be created on the TS class
  // copy the guts of the JS method into the new TS class method

  export class ViewModel {
      
    renderCompleteGrid() {
    }

    renderInProcessGrid() {
    }

    renderCompleteGridGlobal() {
    }

    renderCompleteGridForSingleUser() {
    }

    renderNationBuilderCompleteGrid() {
    }

  }
}