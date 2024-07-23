var AccurateAppend;
(function (AccurateAppend) {
    var FormatString = (function () {
        function FormatString() {
        }
        FormatString.formatCurrency = function (value, decimalPlaces) {
            var formatter = new Intl.NumberFormat("en-US", {
                style: "currency",
                currency: "USD",
                minimumFractionDigits: decimalPlaces
            });
            return formatter.format(value);
        };
        FormatString.formatNumber = function (value) {
            var formatter = new Intl.NumberFormat("en-US", { maximumSignificantDigits: 3 });
            return formatter.format(value);
        };
        FormatString.formatDate = function (value) {
            value = new Date(Date.parse(value));
            var options = {
                year: "numeric",
                month: "numeric",
                day: "numeric",
                timeZone: "America/Los_Angeles"
            };
            var formatter = new Intl.DateTimeFormat("en-US", options);
            return formatter.format(value);
        };
        FormatString.formatDateTime = function (date) {
            date = new Date(Date.parse(date));
            var options = {
                year: "numeric",
                month: "numeric",
                day: "numeric",
                hour: "numeric",
                minute: "numeric",
                second: "numeric",
                timeZone: "America/Los_Angeles"
            };
            var formatter = new Intl.DateTimeFormat("en-US", options);
            return formatter.format(date);
        };
        return FormatString;
    }());
    AccurateAppend.FormatString = FormatString;
})(AccurateAppend || (AccurateAppend = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiQWNjdXJhdGVBcHBlbmQuU3RyaW5nRm9ybWF0dGluZy5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIkFjY3VyYXRlQXBwZW5kLlN0cmluZ0Zvcm1hdHRpbmcudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsSUFBTyxjQUFjLENBbURwQjtBQW5ERCxXQUFPLGNBQWM7SUFFbkI7UUFBQTtRQStDQSxDQUFDO1FBN0NRLDJCQUFjLEdBQXJCLFVBQXNCLEtBQUssRUFBRSxhQUFhO1lBQ3hDLElBQU0sU0FBUyxHQUFHLElBQUksSUFBSSxDQUFDLFlBQVksQ0FBQyxPQUFPLEVBQzdDO2dCQUNFLEtBQUssRUFBRSxVQUFVO2dCQUNqQixRQUFRLEVBQUUsS0FBSztnQkFDZixxQkFBcUIsRUFBRSxhQUFhO2FBQ3JDLENBQUMsQ0FBQztZQUNMLE9BQU8sU0FBUyxDQUFDLE1BQU0sQ0FBQyxLQUFLLENBQUMsQ0FBQztRQUNqQyxDQUFDO1FBRU0seUJBQVksR0FBbkIsVUFBb0IsS0FBSztZQUN2QixJQUFNLFNBQVMsR0FBRyxJQUFJLElBQUksQ0FBQyxZQUFZLENBQUMsT0FBTyxFQUFFLEVBQUUsd0JBQXdCLEVBQUUsQ0FBQyxFQUFFLENBQUMsQ0FBQztZQUNsRixPQUFPLFNBQVMsQ0FBQyxNQUFNLENBQUMsS0FBSyxDQUFDLENBQUM7UUFDakMsQ0FBQztRQUVNLHVCQUFVLEdBQWpCLFVBQWtCLEtBQUs7WUFDckIsS0FBSyxHQUFHLElBQUksSUFBSSxDQUFDLElBQUksQ0FBQyxLQUFLLENBQUMsS0FBSyxDQUFDLENBQUMsQ0FBQztZQUVwQyxJQUFNLE9BQU8sR0FBRztnQkFDZCxJQUFJLEVBQUUsU0FBUztnQkFDZixLQUFLLEVBQUUsU0FBUztnQkFDaEIsR0FBRyxFQUFFLFNBQVM7Z0JBQ2QsUUFBUSxFQUFFLHFCQUFxQjthQUNoQyxDQUFDO1lBRUYsSUFBTSxTQUFTLEdBQUcsSUFBSSxJQUFJLENBQUMsY0FBYyxDQUFDLE9BQU8sRUFBRSxPQUFPLENBQUMsQ0FBQztZQUM1RCxPQUFPLFNBQVMsQ0FBQyxNQUFNLENBQUMsS0FBSyxDQUFDLENBQUM7UUFDakMsQ0FBQztRQUVNLDJCQUFjLEdBQXJCLFVBQXNCLElBQUk7WUFDeEIsSUFBSSxHQUFHLElBQUksSUFBSSxDQUFDLElBQUksQ0FBQyxLQUFLLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQztZQUVsQyxJQUFNLE9BQU8sR0FBRztnQkFDZCxJQUFJLEVBQUUsU0FBUztnQkFDZixLQUFLLEVBQUUsU0FBUztnQkFDaEIsR0FBRyxFQUFFLFNBQVM7Z0JBQ2QsSUFBSSxFQUFFLFNBQVM7Z0JBQ2YsTUFBTSxFQUFFLFNBQVM7Z0JBQ2pCLE1BQU0sRUFBRSxTQUFTO2dCQUNqQixRQUFRLEVBQUUscUJBQXFCO2FBQ2hDLENBQUM7WUFFRixJQUFNLFNBQVMsR0FBRyxJQUFJLElBQUksQ0FBQyxjQUFjLENBQUMsT0FBTyxFQUFFLE9BQU8sQ0FBQyxDQUFDO1lBQzVELE9BQU8sU0FBUyxDQUFDLE1BQU0sQ0FBQyxJQUFJLENBQUMsQ0FBQztRQUNoQyxDQUFDO1FBQ0gsbUJBQUM7SUFBRCxDQUFDLEFBL0NELElBK0NDO0lBL0NZLDJCQUFZLGVBK0N4QixDQUFBO0FBRUgsQ0FBQyxFQW5ETSxjQUFjLEtBQWQsY0FBYyxRQW1EcEIiLCJzb3VyY2VzQ29udGVudCI6WyJtb2R1bGUgQWNjdXJhdGVBcHBlbmQge1xyXG5cclxuICBleHBvcnQgY2xhc3MgRm9ybWF0U3RyaW5nIHtcclxuXHJcbiAgICBzdGF0aWMgZm9ybWF0Q3VycmVuY3kodmFsdWUsIGRlY2ltYWxQbGFjZXMpIHtcclxuICAgICAgY29uc3QgZm9ybWF0dGVyID0gbmV3IEludGwuTnVtYmVyRm9ybWF0KFwiZW4tVVNcIixcclxuICAgICAgICB7XHJcbiAgICAgICAgICBzdHlsZTogXCJjdXJyZW5jeVwiLFxyXG4gICAgICAgICAgY3VycmVuY3k6IFwiVVNEXCIsXHJcbiAgICAgICAgICBtaW5pbXVtRnJhY3Rpb25EaWdpdHM6IGRlY2ltYWxQbGFjZXNcclxuICAgICAgICB9KTtcclxuICAgICAgcmV0dXJuIGZvcm1hdHRlci5mb3JtYXQodmFsdWUpO1xyXG4gICAgfVxyXG5cclxuICAgIHN0YXRpYyBmb3JtYXROdW1iZXIodmFsdWUpIHtcclxuICAgICAgY29uc3QgZm9ybWF0dGVyID0gbmV3IEludGwuTnVtYmVyRm9ybWF0KFwiZW4tVVNcIiwgeyBtYXhpbXVtU2lnbmlmaWNhbnREaWdpdHM6IDMgfSk7XHJcbiAgICAgIHJldHVybiBmb3JtYXR0ZXIuZm9ybWF0KHZhbHVlKTtcclxuICAgIH1cclxuXHJcbiAgICBzdGF0aWMgZm9ybWF0RGF0ZSh2YWx1ZSkge1xyXG4gICAgICB2YWx1ZSA9IG5ldyBEYXRlKERhdGUucGFyc2UodmFsdWUpKTtcclxuXHJcbiAgICAgIGNvbnN0IG9wdGlvbnMgPSB7XHJcbiAgICAgICAgeWVhcjogXCJudW1lcmljXCIsXHJcbiAgICAgICAgbW9udGg6IFwibnVtZXJpY1wiLFxyXG4gICAgICAgIGRheTogXCJudW1lcmljXCIsXHJcbiAgICAgICAgdGltZVpvbmU6IFwiQW1lcmljYS9Mb3NfQW5nZWxlc1wiXHJcbiAgICAgIH07XHJcblxyXG4gICAgICBjb25zdCBmb3JtYXR0ZXIgPSBuZXcgSW50bC5EYXRlVGltZUZvcm1hdChcImVuLVVTXCIsIG9wdGlvbnMpO1xyXG4gICAgICByZXR1cm4gZm9ybWF0dGVyLmZvcm1hdCh2YWx1ZSk7XHJcbiAgICB9XHJcblxyXG4gICAgc3RhdGljIGZvcm1hdERhdGVUaW1lKGRhdGUpIHtcclxuICAgICAgZGF0ZSA9IG5ldyBEYXRlKERhdGUucGFyc2UoZGF0ZSkpO1xyXG5cclxuICAgICAgY29uc3Qgb3B0aW9ucyA9IHtcclxuICAgICAgICB5ZWFyOiBcIm51bWVyaWNcIixcclxuICAgICAgICBtb250aDogXCJudW1lcmljXCIsXHJcbiAgICAgICAgZGF5OiBcIm51bWVyaWNcIixcclxuICAgICAgICBob3VyOiBcIm51bWVyaWNcIixcclxuICAgICAgICBtaW51dGU6IFwibnVtZXJpY1wiLFxyXG4gICAgICAgIHNlY29uZDogXCJudW1lcmljXCIsXHJcbiAgICAgICAgdGltZVpvbmU6IFwiQW1lcmljYS9Mb3NfQW5nZWxlc1wiXHJcbiAgICAgIH07XHJcblxyXG4gICAgICBjb25zdCBmb3JtYXR0ZXIgPSBuZXcgSW50bC5EYXRlVGltZUZvcm1hdChcImVuLVVTXCIsIG9wdGlvbnMpO1xyXG4gICAgICByZXR1cm4gZm9ybWF0dGVyLmZvcm1hdChkYXRlKTtcclxuICAgIH1cclxuICB9XHJcblxyXG59Il19