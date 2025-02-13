var JobsDateRangeWidget;
var NBDateRangeWidget;
var jobProcessingSummaryViewModel;
var links;
$(function () {
    JobsDateRangeWidget = new AccurateAppend.Ui.DateRangeWidget("jobsDateRange", new AccurateAppend.Ui.DateRangeWidgetSettings([
        AccurateAppend.Ui.DateRangeValue.Last24Hours,
        AccurateAppend.Ui.DateRangeValue.Last7Days,
        AccurateAppend.Ui.DateRangeValue.Last30Days,
        AccurateAppend.Ui.DateRangeValue.Last60Days,
        AccurateAppend.Ui.DateRangeValue.LastMonth,
        AccurateAppend.Ui.DateRangeValue.Custom
    ], AccurateAppend.Ui.DateRangeValue.Last24Hours, [
        jobProcessingSummaryViewModel.renderCompleteGrid
    ]));
    NBDateRangeWidget = new AccurateAppend.Ui.DateRangeWidget("nbDateRange", new AccurateAppend.Ui.DateRangeWidgetSettings([
        AccurateAppend.Ui.DateRangeValue.Last24Hours,
        AccurateAppend.Ui.DateRangeValue.Last7Days,
        AccurateAppend.Ui.DateRangeValue.Last30Days,
        AccurateAppend.Ui.DateRangeValue.Custom
    ], AccurateAppend.Ui.DateRangeValue.Last7Days, [
        jobProcessingSummaryViewModel.renderNationBuilderCompleteGrid
    ]));
    NBDateRangeWidget.refresh();
});
var AccurateAppend;
(function (AccurateAppend) {
    var JobProcessing;
    (function (JobProcessing) {
        var Summary;
        (function (Summary) {
            var ViewModel = (function () {
                function ViewModel() {
                }
                ViewModel.prototype.renderCompleteGrid = function () {
                };
                ViewModel.prototype.renderInProcessGrid = function () {
                };
                ViewModel.prototype.renderCompleteGridGlobal = function () {
                };
                ViewModel.prototype.renderCompleteGridForSingleUser = function () {
                };
                ViewModel.prototype.renderNationBuilderCompleteGrid = function () {
                };
                return ViewModel;
            }());
            Summary.ViewModel = ViewModel;
        })(Summary = JobProcessing.Summary || (JobProcessing.Summary = {}));
    })(JobProcessing = AccurateAppend.JobProcessing || (AccurateAppend.JobProcessing = {}));
})(AccurateAppend || (AccurateAppend = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiSW5kZXguanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJJbmRleC50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFPQSxJQUFJLG1CQUFzRCxDQUFDO0FBQzNELElBQUksaUJBQW9ELENBQUM7QUFDekQsSUFBSSw2QkFBNkUsQ0FBQztBQUdsRixJQUFJLEtBQVUsQ0FBQztBQUVmLENBQUMsQ0FBQztJQUVBLG1CQUFtQixHQUFHLElBQUksY0FBYyxDQUFDLEVBQUUsQ0FBQyxlQUFlLENBQUMsZUFBZSxFQUN6RSxJQUFJLGNBQWMsQ0FBQyxFQUFFLENBQUMsdUJBQXVCLENBQzNDO1FBQ0UsY0FBYyxDQUFDLEVBQUUsQ0FBQyxjQUFjLENBQUMsV0FBVztRQUM1QyxjQUFjLENBQUMsRUFBRSxDQUFDLGNBQWMsQ0FBQyxTQUFTO1FBQzFDLGNBQWMsQ0FBQyxFQUFFLENBQUMsY0FBYyxDQUFDLFVBQVU7UUFDM0MsY0FBYyxDQUFDLEVBQUUsQ0FBQyxjQUFjLENBQUMsVUFBVTtRQUMzQyxjQUFjLENBQUMsRUFBRSxDQUFDLGNBQWMsQ0FBQyxTQUFTO1FBQzFDLGNBQWMsQ0FBQyxFQUFFLENBQUMsY0FBYyxDQUFDLE1BQU07S0FDeEMsRUFDRCxjQUFjLENBQUMsRUFBRSxDQUFDLGNBQWMsQ0FBQyxXQUFXLEVBQzVDO1FBQ0UsNkJBQTZCLENBQUMsa0JBQWtCO0tBQ2pELENBQUMsQ0FBQyxDQUFDO0lBRVIsaUJBQWlCLEdBQUcsSUFBSSxjQUFjLENBQUMsRUFBRSxDQUFDLGVBQWUsQ0FBQyxhQUFhLEVBQ3JFLElBQUksY0FBYyxDQUFDLEVBQUUsQ0FBQyx1QkFBdUIsQ0FDM0M7UUFDRSxjQUFjLENBQUMsRUFBRSxDQUFDLGNBQWMsQ0FBQyxXQUFXO1FBQzVDLGNBQWMsQ0FBQyxFQUFFLENBQUMsY0FBYyxDQUFDLFNBQVM7UUFDMUMsY0FBYyxDQUFDLEVBQUUsQ0FBQyxjQUFjLENBQUMsVUFBVTtRQUMzQyxjQUFjLENBQUMsRUFBRSxDQUFDLGNBQWMsQ0FBQyxNQUFNO0tBQ3hDLEVBQ0QsY0FBYyxDQUFDLEVBQUUsQ0FBQyxjQUFjLENBQUMsU0FBUyxFQUMxQztRQUNFLDZCQUE2QixDQUFDLCtCQUErQjtLQUM5RCxDQUFDLENBQUMsQ0FBQztJQUNSLGlCQUFpQixDQUFDLE9BQU8sRUFBRSxDQUFDO0FBRTlCLENBQUMsQ0FBQyxDQUFDO0FBRUgsSUFBTyxjQUFjLENBdUJwQjtBQXZCRCxXQUFPLGNBQWM7SUFBQyxJQUFBLGFBQWEsQ0F1QmxDO0lBdkJxQixXQUFBLGFBQWE7UUFBQyxJQUFBLE9BQU8sQ0F1QjFDO1FBdkJtQyxXQUFBLE9BQU87WUFLekM7Z0JBQUE7Z0JBaUJBLENBQUM7Z0JBZkMsc0NBQWtCLEdBQWxCO2dCQUNBLENBQUM7Z0JBRUQsdUNBQW1CLEdBQW5CO2dCQUNBLENBQUM7Z0JBRUQsNENBQXdCLEdBQXhCO2dCQUNBLENBQUM7Z0JBRUQsbURBQStCLEdBQS9CO2dCQUNBLENBQUM7Z0JBRUQsbURBQStCLEdBQS9CO2dCQUNBLENBQUM7Z0JBRUgsZ0JBQUM7WUFBRCxDQUFDLEFBakJELElBaUJDO1lBakJZLGlCQUFTLFlBaUJyQixDQUFBO1FBQ0gsQ0FBQyxFQXZCbUMsT0FBTyxHQUFQLHFCQUFPLEtBQVAscUJBQU8sUUF1QjFDO0lBQUQsQ0FBQyxFQXZCcUIsYUFBYSxHQUFiLDRCQUFhLEtBQWIsNEJBQWEsUUF1QmxDO0FBQUQsQ0FBQyxFQXZCTSxjQUFjLEtBQWQsY0FBYyxRQXVCcEIiLCJzb3VyY2VzQ29udGVudCI6WyIvLy8gPHJlZmVyZW5jZSBwYXRoPVwiLi4vLi4vLi4vLi4vc2NyaXB0cy90eXBpbmdzL21vbWVudC9tb21lbnQuZC50c1wiIC8+XHJcbi8vLyA8cmVmZXJlbmNlIHBhdGg9XCIuLi8uLi8uLi8uLi9zY3JpcHRzL3R5cGluZ3Mva2VuZG8tdWkva2VuZG8tdWkuZC50c1wiIC8+XHJcbi8vLyA8cmVmZXJlbmNlIHBhdGg9XCJKb2JNb2RlbC50c1wiIC8+XHJcbi8vLyA8cmVmZXJlbmNlIHBhdGg9XCJKb2JSZXBvcnRNb2RlbC50c1wiIC8+XHJcbi8vLyA8cmVmZXJlbmNlIHBhdGg9XCJOYXRpb25CdWlsZGVyTW9kZWwudHNcIiAvPlxyXG4vLy8gPHJlZmVyZW5jZSBwYXRoPVwiT3BlcmF0aW9uUmVwb3J0TW9kZWwudHNcIiAvPlxyXG5cclxudmFyIEpvYnNEYXRlUmFuZ2VXaWRnZXQ6IEFjY3VyYXRlQXBwZW5kLlVpLkRhdGVSYW5nZVdpZGdldDtcclxudmFyIE5CRGF0ZVJhbmdlV2lkZ2V0OiBBY2N1cmF0ZUFwcGVuZC5VaS5EYXRlUmFuZ2VXaWRnZXQ7XHJcbnZhciBqb2JQcm9jZXNzaW5nU3VtbWFyeVZpZXdNb2RlbDogQWNjdXJhdGVBcHBlbmQuSm9iUHJvY2Vzc2luZy5TdW1tYXJ5LlZpZXdNb2RlbDtcclxuLy8gYW1iaWVudCBkZWNsYXJhdGlvbiwgc2V0IGluIFZpZXdcclxuLy8gXHJcbnZhciBsaW5rczogYW55O1xyXG5cclxuJCgoKSA9PiB7XHJcblxyXG4gIEpvYnNEYXRlUmFuZ2VXaWRnZXQgPSBuZXcgQWNjdXJhdGVBcHBlbmQuVWkuRGF0ZVJhbmdlV2lkZ2V0KFwiam9ic0RhdGVSYW5nZVwiLFxyXG4gICAgbmV3IEFjY3VyYXRlQXBwZW5kLlVpLkRhdGVSYW5nZVdpZGdldFNldHRpbmdzKFxyXG4gICAgICBbXHJcbiAgICAgICAgQWNjdXJhdGVBcHBlbmQuVWkuRGF0ZVJhbmdlVmFsdWUuTGFzdDI0SG91cnMsXHJcbiAgICAgICAgQWNjdXJhdGVBcHBlbmQuVWkuRGF0ZVJhbmdlVmFsdWUuTGFzdDdEYXlzLFxyXG4gICAgICAgIEFjY3VyYXRlQXBwZW5kLlVpLkRhdGVSYW5nZVZhbHVlLkxhc3QzMERheXMsXHJcbiAgICAgICAgQWNjdXJhdGVBcHBlbmQuVWkuRGF0ZVJhbmdlVmFsdWUuTGFzdDYwRGF5cyxcclxuICAgICAgICBBY2N1cmF0ZUFwcGVuZC5VaS5EYXRlUmFuZ2VWYWx1ZS5MYXN0TW9udGgsXHJcbiAgICAgICAgQWNjdXJhdGVBcHBlbmQuVWkuRGF0ZVJhbmdlVmFsdWUuQ3VzdG9tXHJcbiAgICAgIF0sXHJcbiAgICAgIEFjY3VyYXRlQXBwZW5kLlVpLkRhdGVSYW5nZVZhbHVlLkxhc3QyNEhvdXJzLFxyXG4gICAgICBbXHJcbiAgICAgICAgam9iUHJvY2Vzc2luZ1N1bW1hcnlWaWV3TW9kZWwucmVuZGVyQ29tcGxldGVHcmlkXHJcbiAgICAgIF0pKTtcclxuXHJcbiAgTkJEYXRlUmFuZ2VXaWRnZXQgPSBuZXcgQWNjdXJhdGVBcHBlbmQuVWkuRGF0ZVJhbmdlV2lkZ2V0KFwibmJEYXRlUmFuZ2VcIixcclxuICAgIG5ldyBBY2N1cmF0ZUFwcGVuZC5VaS5EYXRlUmFuZ2VXaWRnZXRTZXR0aW5ncyhcclxuICAgICAgW1xyXG4gICAgICAgIEFjY3VyYXRlQXBwZW5kLlVpLkRhdGVSYW5nZVZhbHVlLkxhc3QyNEhvdXJzLFxyXG4gICAgICAgIEFjY3VyYXRlQXBwZW5kLlVpLkRhdGVSYW5nZVZhbHVlLkxhc3Q3RGF5cyxcclxuICAgICAgICBBY2N1cmF0ZUFwcGVuZC5VaS5EYXRlUmFuZ2VWYWx1ZS5MYXN0MzBEYXlzLFxyXG4gICAgICAgIEFjY3VyYXRlQXBwZW5kLlVpLkRhdGVSYW5nZVZhbHVlLkN1c3RvbVxyXG4gICAgICBdLFxyXG4gICAgICBBY2N1cmF0ZUFwcGVuZC5VaS5EYXRlUmFuZ2VWYWx1ZS5MYXN0N0RheXMsXHJcbiAgICAgIFtcclxuICAgICAgICBqb2JQcm9jZXNzaW5nU3VtbWFyeVZpZXdNb2RlbC5yZW5kZXJOYXRpb25CdWlsZGVyQ29tcGxldGVHcmlkXHJcbiAgICAgIF0pKTtcclxuICBOQkRhdGVSYW5nZVdpZGdldC5yZWZyZXNoKCk7XHJcblxyXG59KTtcclxuXHJcbm1vZHVsZSBBY2N1cmF0ZUFwcGVuZC5Kb2JQcm9jZXNzaW5nLlN1bW1hcnkge1xyXG5cclxuICAvLyBuZXcgbWV0aG9kcyB3aXRoIHRoZSBzYW1lIG5hbWVzIHNob3VsZCBiZSBjcmVhdGVkIG9uIHRoZSBUUyBjbGFzc1xyXG4gIC8vIGNvcHkgdGhlIGd1dHMgb2YgdGhlIEpTIG1ldGhvZCBpbnRvIHRoZSBuZXcgVFMgY2xhc3MgbWV0aG9kXHJcblxyXG4gIGV4cG9ydCBjbGFzcyBWaWV3TW9kZWwge1xyXG4gICAgICBcclxuICAgIHJlbmRlckNvbXBsZXRlR3JpZCgpIHtcclxuICAgIH1cclxuXHJcbiAgICByZW5kZXJJblByb2Nlc3NHcmlkKCkge1xyXG4gICAgfVxyXG5cclxuICAgIHJlbmRlckNvbXBsZXRlR3JpZEdsb2JhbCgpIHtcclxuICAgIH1cclxuXHJcbiAgICByZW5kZXJDb21wbGV0ZUdyaWRGb3JTaW5nbGVVc2VyKCkge1xyXG4gICAgfVxyXG5cclxuICAgIHJlbmRlck5hdGlvbkJ1aWxkZXJDb21wbGV0ZUdyaWQoKSB7XHJcbiAgICB9XHJcblxyXG4gIH1cclxufSJdfQ==