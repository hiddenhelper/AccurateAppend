var AccurateAppend;
(function (AccurateAppend) {
    var DatePicker;
    (function (DatePicker) {
        var DateRangeValue;
        (function (DateRangeValue) {
            DateRangeValue[DateRangeValue["Last24Hours"] = 0] = "Last24Hours";
            DateRangeValue[DateRangeValue["Today"] = 1] = "Today";
            DateRangeValue[DateRangeValue["Yesterday"] = 2] = "Yesterday";
            DateRangeValue[DateRangeValue["Last7Days"] = 3] = "Last7Days";
            DateRangeValue[DateRangeValue["ThisMonth"] = 4] = "ThisMonth";
            DateRangeValue[DateRangeValue["LastMonth"] = 5] = "LastMonth";
            DateRangeValue[DateRangeValue["Last30Days"] = 6] = "Last30Days";
            DateRangeValue[DateRangeValue["Last60Days"] = 7] = "Last60Days";
            DateRangeValue[DateRangeValue["Last90Days"] = 8] = "Last90Days";
            DateRangeValue[DateRangeValue["CurrentMonth"] = 9] = "CurrentMonth";
            DateRangeValue[DateRangeValue["PreviousToLastMonth"] = 10] = "PreviousToLastMonth";
            DateRangeValue[DateRangeValue["All"] = 11] = "All";
            DateRangeValue[DateRangeValue["Custom"] = 12] = "Custom";
        })(DateRangeValue = DatePicker.DateRangeValue || (DatePicker.DateRangeValue = {}));
        var DateRangeValueDescription = (function () {
            function DateRangeValueDescription() {
            }
            DateRangeValueDescription.getDecription = function (dateRangeValue) {
                switch (dateRangeValue) {
                    case DateRangeValue.Last24Hours:
                        return "Last 24 Hours";
                    case DateRangeValue.Today:
                        return "Today";
                    case DateRangeValue.Yesterday:
                        return "Yesterday";
                    case DateRangeValue.Last7Days:
                        return "Last 7 Days";
                    case DateRangeValue.ThisMonth:
                        return "This Month";
                    case DateRangeValue.LastMonth:
                        return "Last Month";
                    case DateRangeValue.Last30Days:
                        return "Last 30 Days";
                    case DateRangeValue.Last60Days:
                        return "Last 60 Days";
                    case DateRangeValue.Last90Days:
                        return "Last 90 Days";
                    case DateRangeValue.CurrentMonth:
                        return "Current Month";
                    case DateRangeValue.PreviousToLastMonth:
                        return "Previous To Last Month";
                    case DateRangeValue.All:
                        return "All";
                    case DateRangeValue.Custom:
                        return "Custom";
                    default:
                        return undefined;
                }
            };
            return DateRangeValueDescription;
        }());
        var DateRangeWidgetSettings = (function () {
            function DateRangeWidgetSettings(dateRangeOptions, defaultValue, callBacks) {
                this.dateRangeOptions = dateRangeOptions;
                this.defaultValue = defaultValue;
                this.callBacks = callBacks;
            }
            return DateRangeWidgetSettings;
        }());
        DatePicker.DateRangeWidgetSettings = DateRangeWidgetSettings;
        var DateRangeWidget = (function () {
            function DateRangeWidget(elementId, settings) {
                this._settings = settings;
                this._elementId = elementId;
                this._dateRangeValue = settings.defaultValue;
                this.addControls(this._dateRangeValue);
                this.setEvents();
                this.sync(settings.defaultValue);
                this.displayDebug();
            }
            DateRangeWidget.prototype.getStartDate = function () { return this._startDate; };
            DateRangeWidget.prototype.getEndDate = function () { return this._endDate; };
            DateRangeWidget.prototype.getDateRangeValue = function () { return this._dateRangeValue; };
            DateRangeWidget.prototype.setDateRangeValue = function (value) {
                this._dateRangeValue = value;
                var $select = this.$getDateRangeSelect();
                $select.val(DateRangeValue[value]);
                this.sync(this._dateRangeValue);
                this.runCallBacks(this._settings.callBacks);
            };
            DateRangeWidget.prototype.refresh = function () {
                this.runCallBacks(this._settings.callBacks);
            };
            DateRangeWidget.prototype.addControls = function (value) {
                var root = $('#' + this._elementId);
                var $datePickerPanel = $('<span id="' + this._elementId + '_datePickerPanel"></span>');
                $datePickerPanel.hide();
                var $startDatePicker = $('<input id="' + this._elementId + '_startDate' + '"/>');
                $datePickerPanel.append($startDatePicker).append('<label style="padding: 0 5px 0 5px;">to</label>');
                var endDatePicker = $('<input id="' + this._elementId + '_endDate' + '"/>');
                $datePickerPanel.append(endDatePicker);
                $startDatePicker.kendoDatePicker({ format: "yyyy-MM-dd", value: this._startDate });
                endDatePicker.kendoDatePicker({ format: "yyyy-MM-dd", value: this._endDate });
                root.append($datePickerPanel);
                var $select = $('<select id="' + this._elementId + '_dateRange' + '" class="form-control" style="width: 150px;display: inline;margin-left: 5px;"></select>');
                $.each(this._settings.dateRangeOptions, function (i, v) { $select.append($("<option " + (value === v ? "selected" : "") + "/>").val(DateRangeValue[v]).text(DateRangeValueDescription.getDecription(v))); });
                root.append($select);
            };
            DateRangeWidget.prototype.setEvents = function () {
                var _this = this;
                var $root = $('#' + this._elementId);
                var select = $root.find('select');
                select.change(function () {
                    console.log('dateRange change firing');
                    var v = $('#' + _this._elementId + '_dateRange').val();
                    _this._dateRangeValue = DateRangeValue[v];
                    _this.sync(_this._dateRangeValue);
                    _this.runCallBacks(_this._settings.callBacks);
                    _this.displayDebug();
                });
                var start = this.getStartPicker();
                start.bind("change", function () {
                    console.log('startdate change firing');
                    var startPicker = _this.getStartPicker();
                    var endPicker = _this.getEndPicker();
                    if (startPicker.value() >= endPicker.value()) {
                        _this.messageShow('Start date must be before end date.');
                        startPicker.value(moment(endPicker.value()).add(-1, 'day').format('YYYY-MM-DD'));
                    }
                    else {
                        _this._startDate = startPicker.value();
                    }
                    _this.runCallBacks(_this._settings.callBacks);
                    _this.displayDebug();
                });
                var end = this.getEndPicker();
                end.bind("change", function () {
                    console.log('enddate change firing');
                    var startPicker = _this.getStartPicker();
                    var endPicker = _this.getEndPicker();
                    if (startPicker.value() >= endPicker.value()) {
                        _this.messageShow('End date must be after start date.');
                        endPicker.value(moment(startPicker.value()).add(1, 'day').format('YYYY-MM-DD'));
                    }
                    else {
                        _this._endDate = endPicker.value();
                    }
                    _this.runCallBacks(_this._settings.callBacks);
                    _this.displayDebug();
                });
            };
            DateRangeWidget.prototype.runCallBacks = function (callbacks) {
                $.each(callbacks, function (i, o) { o.call(); });
            };
            DateRangeWidget.prototype.sync = function (dateRangeValue) {
                var dateRangeResult = this.getStartEndForRange(dateRangeValue);
                this._startDate = dateRangeResult.startDate;
                this._endDate = dateRangeResult.endDate;
                var startPicker = this.getStartPicker();
                startPicker.value(dateRangeResult.startDate);
                var endPicker = this.getEndPicker();
                endPicker.value(dateRangeResult.endDate);
                if (this._dateRangeValue === DateRangeValue.Custom) {
                    $('#' + this._elementId + '_datePickerPanel').show();
                }
                else {
                    $('#' + this._elementId + '_datePickerPanel').hide();
                }
            };
            DateRangeWidget.prototype.messageShow = function (message) {
                $('#message').addClass('alert alert-danger').show().text(message);
                window.setTimeout(function () { $('#message').hide(); }, 10000);
            };
            DateRangeWidget.prototype.messageHide = function () {
                $('#message').hide();
            };
            DateRangeWidget.prototype.getStartPicker = function () {
                return $('#' + this._elementId + '_startDate').data('kendoDatePicker');
            };
            DateRangeWidget.prototype.getEndPicker = function () {
                return $('#' + this._elementId + '_endDate').data('kendoDatePicker');
            };
            DateRangeWidget.prototype.$getDateRangeSelect = function () {
                return $('#' + this._elementId + '_dateRange');
            };
            DateRangeWidget.prototype.displayDebug = function () {
                console.log("startdate=" + this._startDate);
                console.log("enddate=" + this._endDate);
            };
            DateRangeWidget.prototype.getStartEndForRange = function (dateRangeValue) {
                switch (dateRangeValue) {
                    case DateRangeValue.Last24Hours:
                        return new DateRangeResult(moment().subtract(24, 'hours').local().toDate(), moment().local().toDate());
                    case DateRangeValue.Today:
                        return new DateRangeResult(moment().local().startOf('day').toDate(), moment().local().toDate());
                    case DateRangeValue.Yesterday:
                        return new DateRangeResult(moment().local().subtract(1, 'days').startOf('day').toDate(), moment().local().startOf('day').toDate());
                    case DateRangeValue.Last7Days:
                        return new DateRangeResult(moment().local().subtract(7, 'days').toDate(), moment().local().toDate());
                    case DateRangeValue.Last30Days:
                        return new DateRangeResult(moment().local().subtract(30, 'days').toDate(), moment().local().toDate());
                    case DateRangeValue.Last60Days:
                        return new DateRangeResult(moment().local().subtract(60, 'days').toDate(), moment().local().toDate());
                    case DateRangeValue.Last90Days:
                        return new DateRangeResult(moment().local().subtract(90, 'days').toDate(), moment().local().toDate());
                    case DateRangeValue.ThisMonth:
                        return new DateRangeResult(moment().local().startOf('month').toDate(), moment().local().toDate());
                    case DateRangeValue.LastMonth:
                        return new DateRangeResult(moment().local().subtract(1, 'months').startOf('month').toDate(), moment().local().startOf('month').subtract('days', 1).toDate());
                    case DateRangeValue.All:
                        return new DateRangeResult(moment().local().subtract(1, 'years').startOf('month').toDate(), moment().local().toDate());
                    case DateRangeValue.Custom:
                        return new DateRangeResult(moment().local().startOf('day').toDate(), moment().local().toDate());
                }
                return new DateRangeResult(null, null);
            };
            return DateRangeWidget;
        }());
        DatePicker.DateRangeWidget = DateRangeWidget;
        var DateRangeResult = (function () {
            function DateRangeResult(startDate, endDate) {
                this._startDate = startDate;
                this._endDate = endDate;
            }
            Object.defineProperty(DateRangeResult.prototype, "startDate", {
                get: function () {
                    return this._startDate;
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(DateRangeResult.prototype, "endDate", {
                get: function () {
                    return this._endDate;
                },
                enumerable: true,
                configurable: true
            });
            return DateRangeResult;
        }());
        var ApplicationId = (function () {
            function ApplicationId() {
            }
            ApplicationId.load = function () {
                if ($("#ApplicationId") != null) {
                    var v = $.cookie('ApplicationId');
                    if (v != '') {
                        $('#ApplicationId option[value=' + $.cookie('ApplicationId') + ']').attr('selected', 'selected');
                    }
                }
            };
            ApplicationId.set = function () {
                if ($("#ApplicationId") != null) {
                    $.cookie('ApplicationId', $('#ApplicationId option:selected').val());
                }
            };
            return ApplicationId;
        }());
        DatePicker.ApplicationId = ApplicationId;
    })(DatePicker = AccurateAppend.DatePicker || (AccurateAppend.DatePicker = {}));
})(AccurateAppend || (AccurateAppend = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiQWNjdXJhdGVBcHBlbmQuRGF0ZVBpY2tlci5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIkFjY3VyYXRlQXBwZW5kLkRhdGVQaWNrZXIudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQ0EsSUFBTyxjQUFjLENBOFNwQjtBQTlTRCxXQUFPLGNBQWM7SUFBQyxJQUFBLFVBQVUsQ0E4Uy9CO0lBOVNxQixXQUFBLFVBQVU7UUFFNUIsSUFBWSxjQWNYO1FBZEQsV0FBWSxjQUFjO1lBQ3RCLGlFQUFXLENBQUE7WUFDWCxxREFBSyxDQUFBO1lBQ0wsNkRBQVMsQ0FBQTtZQUNULDZEQUFTLENBQUE7WUFDVCw2REFBUyxDQUFBO1lBQ1QsNkRBQVMsQ0FBQTtZQUNULCtEQUFVLENBQUE7WUFDViwrREFBVSxDQUFBO1lBQ1YsK0RBQVUsQ0FBQTtZQUNWLG1FQUFZLENBQUE7WUFDWixrRkFBbUIsQ0FBQTtZQUNuQixrREFBRyxDQUFBO1lBQ0gsd0RBQU0sQ0FBQTtRQUNWLENBQUMsRUFkVyxjQUFjLEdBQWQseUJBQWMsS0FBZCx5QkFBYyxRQWN6QjtRQUVEO1lBQUE7WUFpQ0EsQ0FBQztZQWhDaUIsdUNBQWEsR0FBM0IsVUFBNEIsY0FBOEI7Z0JBQ3RELFFBQVEsY0FBYyxFQUFFO29CQUNwQixLQUFLLGNBQWMsQ0FBQyxXQUFXO3dCQUMzQixPQUFPLGVBQWUsQ0FBQztvQkFDM0IsS0FBSyxjQUFjLENBQUMsS0FBSzt3QkFDckIsT0FBTyxPQUFPLENBQUM7b0JBQ25CLEtBQUssY0FBYyxDQUFDLFNBQVM7d0JBQ3pCLE9BQU8sV0FBVyxDQUFDO29CQUN2QixLQUFLLGNBQWMsQ0FBQyxTQUFTO3dCQUN6QixPQUFPLGFBQWEsQ0FBQztvQkFDekIsS0FBSyxjQUFjLENBQUMsU0FBUzt3QkFDekIsT0FBTyxZQUFZLENBQUM7b0JBQ3hCLEtBQUssY0FBYyxDQUFDLFNBQVM7d0JBQ3pCLE9BQU8sWUFBWSxDQUFDO29CQUN4QixLQUFLLGNBQWMsQ0FBQyxVQUFVO3dCQUMx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QyxJQUFJLENBQUMsVUFBVSxFQUFFLFVBQVUsQ0FBQyxDQUFDO3FCQUNwRztpQkFDSjtZQUNMLENBQUM7WUFDYSxpQkFBRyxHQUFqQjtnQkFDSSxJQUFJLENBQUMsQ0FBQyxnQkFBZ0IsQ0FBQyxJQUFJLElBQUksRUFBRTtvQkFDN0IsQ0FBQyxDQUFDLE1BQU0sQ0FBQyxlQUFlLEVBQUUsQ0FBQyxDQUFDLGdDQUFnQyxDQUFDLENBQUMsR0FBRyxFQUFFLENBQUMsQ0FBQztpQkFDeEU7WUFDTCxDQUFDO1lBQ0wsb0JBQUM7UUFBRCxDQUFDLEFBZEQsSUFjQztRQWRZLHdCQUFhLGdCQWN6QixDQUFBO0lBRUwsQ0FBQyxFQTlTcUIsVUFBVSxHQUFWLHlCQUFVLEtBQVYseUJBQVUsUUE4Uy9CO0FBQUQsQ0FBQyxFQTlTTSxjQUFjLEtBQWQsY0FBYyxRQThTcEIiLCJzb3VyY2VzQ29udGVudCI6WyJcclxubW9kdWxlIEFjY3VyYXRlQXBwZW5kLkRhdGVQaWNrZXIge1xyXG5cclxuICAgIGV4cG9ydCBlbnVtIERhdGVSYW5nZVZhbHVlIHtcclxuICAgICAgICBMYXN0MjRIb3VycyxcclxuICAgICAgICBUb2RheSxcclxuICAgICAgICBZZXN0ZXJkYXksXHJcbiAgICAgICAgTGFzdDdEYXlzLFxyXG4gICAgICAgIFRoaXNNb250aCxcclxuICAgICAgICBMYXN0TW9udGgsXHJcbiAgICAgICAgTGFzdDMwRGF5cyxcclxuICAgICAgICBMYXN0NjBEYXlzLFxyXG4gICAgICAgIExhc3Q5MERheXMsXHJcbiAgICAgICAgQ3VycmVudE1vbnRoLFxyXG4gICAgICAgIFByZXZpb3VzVG9MYXN0TW9udGgsXHJcbiAgICAgICAgQWxsLFxyXG4gICAgICAgIEN1c3RvbVxyXG4gICAgfVxyXG5cclxuICAgIGNsYXNzIERhdGVSYW5nZVZhbHVlRGVzY3JpcHRpb24ge1xyXG4gICAgICAgIHB1YmxpYyBzdGF0aWMgZ2V0RGVjcmlwdGlvbihkYXRlUmFuZ2VWYWx1ZTogRGF0ZVJhbmdlVmFsdWUpIHtcclxuICAgICAgICAgICAgc3dpdGNoIChkYXRlUmFuZ2VWYWx1ZSkge1xyXG4gICAgICAgICAgICAgICAgY2FzZSBEYXRlUmFuZ2VWYWx1ZS5MYXN0MjRIb3VyczpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gXCJMYXN0IDI0IEhvdXJzXCI7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLlRvZGF5OlxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiBcIlRvZGF5XCI7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLlllc3RlcmRheTpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gXCJZZXN0ZXJkYXlcIjtcclxuICAgICAgICAgICAgICAgIGNhc2UgRGF0ZVJhbmdlVmFsdWUuTGFzdDdEYXlzOlxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiBcIkxhc3QgNyBEYXlzXCI7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLlRoaXNNb250aDpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gXCJUaGlzIE1vbnRoXCI7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLkxhc3RNb250aDpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gXCJMYXN0IE1vbnRoXCI7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLkxhc3QzMERheXM6XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIFwiTGFzdCAzMCBEYXlzXCI7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLkxhc3Q2MERheXM6XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIFwiTGFzdCA2MCBEYXlzXCI7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLkxhc3Q5MERheXM6XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIFwiTGFzdCA5MCBEYXlzXCI7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLkN1cnJlbnRNb250aDpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gXCJDdXJyZW50IE1vbnRoXCI7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLlByZXZpb3VzVG9MYXN0TW9udGg6XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIFwiUHJldmlvdXMgVG8gTGFzdCBNb250aFwiO1xyXG4gICAgICAgICAgICAgICAgY2FzZSBEYXRlUmFuZ2VWYWx1ZS5BbGw6XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIFwiQWxsXCI7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLkN1c3RvbTpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gXCJDdXN0b21cIjtcclxuICAgICAgICAgICAgICAgIGRlZmF1bHQ6XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIHVuZGVmaW5lZDtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBleHBvcnQgY2xhc3MgRGF0ZVJhbmdlV2lkZ2V0U2V0dGluZ3Mge1xyXG4gICAgICAgIGNvbnN0cnVjdG9yKFxyXG4gICAgICAgICAgICAvLyBvcHRpb25zIHRvIGJlIGluY2x1ZGVkIGluIHNlbGVjdFxyXG4gICAgICAgICAgICBwdWJsaWMgZGF0ZVJhbmdlT3B0aW9uczogQXJyYXk8RGF0ZVJhbmdlVmFsdWU+LFxyXG4gICAgICAgICAgICAvLyBkZWZhdWx0IHZhbHVlIHRvIGJlIGNob3NlbiBpbiBzZWxlY3RcclxuICAgICAgICAgICAgcHVibGljIGRlZmF1bHRWYWx1ZTogRGF0ZVJhbmdlVmFsdWUsXHJcbiAgICAgICAgICAgIC8vIGNhbGxiYWNrcyB0byBiZSBleGVjdXRlZCB3aGVuIGNoYW5nZSBpcyByYWlzZWRcclxuICAgICAgICAgICAgcHVibGljIGNhbGxCYWNrczogQXJyYXk8YW55PlxyXG4gICAgICAgICAgICApIHsgfVxyXG4gICAgfVxyXG5cclxuICAgIC8vIHJlbmRlcnMgY29udHJvbCB0aGF0IHJlbmRlcnMgZGF0ZSByYWduZSBERCBhbG9uZyB3aXRoIGRhdGUgcGlja2VycyBmb3IgY3VzdG9tIGRhdGUgcmFuZ2VcclxuICAgIGV4cG9ydCBjbGFzcyBEYXRlUmFuZ2VXaWRnZXQge1xyXG5cclxuICAgICAgICBwcml2YXRlIF9zdGFydERhdGU6IERhdGU7XHJcbiAgICAgICAgcHJpdmF0ZSBfZW5kRGF0ZTogRGF0ZTtcclxuICAgICAgICBwcml2YXRlIF9kYXRlUmFuZ2VWYWx1ZTogRGF0ZVJhbmdlVmFsdWU7XHJcbiAgICAgICAgcHJpdmF0ZSBfZWxlbWVudElkOiBzdHJpbmc7XHJcbiAgICAgICAgcHJpdmF0ZSBfc2V0dGluZ3M6IGFueTtcclxuXHJcbiAgICAgICAgY29uc3RydWN0b3IoZWxlbWVudElkOiBzdHJpbmcsIHNldHRpbmdzOiBEYXRlUmFuZ2VXaWRnZXRTZXR0aW5ncykge1xyXG4gICAgICAgICAgICB0aGlzLl9zZXR0aW5ncyA9IHNldHRpbmdzO1xyXG4gICAgICAgICAgICB0aGlzLl9lbGVtZW50SWQgPSBlbGVtZW50SWQ7XHJcbiAgICAgICAgICAgIHRoaXMuX2RhdGVSYW5nZVZhbHVlID0gc2V0dGluZ3MuZGVmYXVsdFZhbHVlO1xyXG4gICAgICAgICAgICB0aGlzLmFkZENvbnRyb2xzKHRoaXMuX2RhdGVSYW5nZVZhbHVlKTtcclxuICAgICAgICAgICAgdGhpcy5zZXRFdmVudHMoKTtcclxuICAgICAgICAgICAgdGhpcy5zeW5jKHNldHRpbmdzLmRlZmF1bHRWYWx1ZSk7XHJcbiAgICAgICAgICAgIHRoaXMuZGlzcGxheURlYnVnKCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAvLyBwdWJsaWMgcHJvcGVydGllc1xyXG4gICAgICAgIHB1YmxpYyBnZXRTdGFydERhdGUoKSB7IHJldHVybiB0aGlzLl9zdGFydERhdGU7IH1cclxuICAgICAgICBwdWJsaWMgZ2V0RW5kRGF0ZSgpIHsgcmV0dXJuIHRoaXMuX2VuZERhdGU7IH1cclxuICAgICAgICBwdWJsaWMgZ2V0RGF0ZVJhbmdlVmFsdWUoKSB7IHJldHVybiB0aGlzLl9kYXRlUmFuZ2VWYWx1ZTsgfVxyXG4gICAgICAgIC8vIHNldHMgdmFsdWUgb2Ygc2VsZWN0LCBzeW5jcyBkYXRlIHBpY2tlcnNcclxuICAgICAgICBwdWJsaWMgc2V0RGF0ZVJhbmdlVmFsdWUodmFsdWU6IERhdGVSYW5nZVZhbHVlKSB7XHJcbiAgICAgICAgICAgIHRoaXMuX2RhdGVSYW5nZVZhbHVlID0gdmFsdWU7XHJcbiAgICAgICAgICAgIHZhciAkc2VsZWN0ID0gdGhpcy4kZ2V0RGF0ZVJhbmdlU2VsZWN0KCk7XHJcbiAgICAgICAgICAgICRzZWxlY3QudmFsKERhdGVSYW5nZVZhbHVlW3ZhbHVlXSk7XHJcbiAgICAgICAgICAgIHRoaXMuc3luYyh0aGlzLl9kYXRlUmFuZ2VWYWx1ZSk7XHJcbiAgICAgICAgICAgIHRoaXMucnVuQ2FsbEJhY2tzKHRoaXMuX3NldHRpbmdzLmNhbGxCYWNrcyk7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIHB1YmxpYyByZWZyZXNoKCkge1xyXG4gICAgICAgICAgICB0aGlzLnJ1bkNhbGxCYWNrcyh0aGlzLl9zZXR0aW5ncy5jYWxsQmFja3MpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8gYWRkcyByYXcgY29udHJvbHMgdG8gRE9NXHJcbiAgICAgICAgcHJpdmF0ZSBhZGRDb250cm9scyh2YWx1ZTogRGF0ZVJhbmdlVmFsdWUpIHtcclxuICAgICAgICAgICAgLy9zdGFydC5tYXgoZW5kLnZhbHVlKCkpO1xyXG4gICAgICAgICAgICAvL2VuZC5taW4oc3RhcnQudmFsdWUoKSk7XHJcbiAgICAgICAgICAgIHZhciByb290ID0gJCgnIycgKyB0aGlzLl9lbGVtZW50SWQpO1xyXG4gICAgICAgICAgICAvLyBhcHBlbmQgZGF0ZSBwaWNrZXJzXHJcbiAgICAgICAgICAgIHZhciAkZGF0ZVBpY2tlclBhbmVsID0gJCgnPHNwYW4gaWQ9XCInICsgdGhpcy5fZWxlbWVudElkICsgJ19kYXRlUGlja2VyUGFuZWxcIj48L3NwYW4+Jyk7XHJcbiAgICAgICAgICAgICRkYXRlUGlja2VyUGFuZWwuaGlkZSgpO1xyXG4gICAgICAgICAgICB2YXIgJHN0YXJ0RGF0ZVBpY2tlciA9ICQoJzxpbnB1dCBpZD1cIicgKyB0aGlzLl9lbGVtZW50SWQgKyAnX3N0YXJ0RGF0ZScgKyAnXCIvPicpO1xyXG4gICAgICAgICAgICAkZGF0ZVBpY2tlclBhbmVsLmFwcGVuZCgkc3RhcnREYXRlUGlja2VyKS5hcHBlbmQoJzxsYWJlbCBzdHlsZT1cInBhZGRpbmc6IDAgNXB4IDAgNXB4O1wiPnRvPC9sYWJlbD4nKTtcclxuICAgICAgICAgICAgdmFyIGVuZERhdGVQaWNrZXIgPSAkKCc8aW5wdXQgaWQ9XCInICsgdGhpcy5fZWxlbWVudElkICsgJ19lbmREYXRlJyArICdcIi8+Jyk7XHJcbiAgICAgICAgICAgICRkYXRlUGlja2VyUGFuZWwuYXBwZW5kKGVuZERhdGVQaWNrZXIpO1xyXG4gICAgICAgICAgICAkc3RhcnREYXRlUGlja2VyLmtlbmRvRGF0ZVBpY2tlcih7IGZvcm1hdDogXCJ5eXl5LU1NLWRkXCIsIHZhbHVlOiB0aGlzLl9zdGFydERhdGUgfSk7XHJcbiAgICAgICAgICAgIGVuZERhdGVQaWNrZXIua2VuZG9EYXRlUGlja2VyKHsgZm9ybWF0OiBcInl5eXktTU0tZGRcIiwgdmFsdWU6IHRoaXMuX2VuZERhdGUgfSk7XHJcbiAgICAgICAgICAgIHJvb3QuYXBwZW5kKCRkYXRlUGlja2VyUGFuZWwpO1xyXG4gICAgICAgICAgICAvLyBhcHBlbmQgc2VsZWN0XHJcbiAgICAgICAgICAgIHZhciAkc2VsZWN0ID0gJCgnPHNlbGVjdCBpZD1cIicgKyB0aGlzLl9lbGVtZW50SWQgKyAnX2RhdGVSYW5nZScgKyAnXCIgY2xhc3M9XCJmb3JtLWNvbnRyb2xcIiBzdHlsZT1cIndpZHRoOiAxNTBweDtkaXNwbGF5OiBpbmxpbmU7bWFyZ2luLWxlZnQ6IDVweDtcIj48L3NlbGVjdD4nKTtcclxuICAgICAgICAgICAgJC5lYWNoKHRoaXMuX3NldHRpbmdzLmRhdGVSYW5nZU9wdGlvbnMsIChpLCB2KSA9PiB7ICRzZWxlY3QuYXBwZW5kKCQoXCI8b3B0aW9uIFwiICsgKHZhbHVlID09PSB2ID8gXCJzZWxlY3RlZFwiIDogXCJcIikgKyBcIi8+XCIpLnZhbChEYXRlUmFuZ2VWYWx1ZVt2XSkudGV4dChEYXRlUmFuZ2VWYWx1ZURlc2NyaXB0aW9uLmdldERlY3JpcHRpb24odikpKTsgfSk7XHJcbiAgICAgICAgICAgIHJvb3QuYXBwZW5kKCRzZWxlY3QpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8gc2V0IGV2ZW50cyBcclxuICAgICAgICBwcml2YXRlIHNldEV2ZW50cygpIHtcclxuICAgICAgICAgICAgdmFyICRyb290ID0gJCgnIycgKyB0aGlzLl9lbGVtZW50SWQpO1xyXG4gICAgICAgICAgICB2YXIgc2VsZWN0ID0gJHJvb3QuZmluZCgnc2VsZWN0Jyk7XHJcbiAgICAgICAgICAgIHNlbGVjdC5jaGFuZ2UoKCkgPT4ge1xyXG4gICAgICAgICAgICAgICAgY29uc29sZS5sb2coJ2RhdGVSYW5nZSBjaGFuZ2UgZmlyaW5nJyk7XHJcbiAgICAgICAgICAgICAgICB2YXIgdjogc3RyaW5nID0gJCgnIycgKyB0aGlzLl9lbGVtZW50SWQgKyAnX2RhdGVSYW5nZScpLnZhbCgpO1xyXG4gICAgICAgICAgICAgICAgdGhpcy5fZGF0ZVJhbmdlVmFsdWUgPSBEYXRlUmFuZ2VWYWx1ZVt2XTtcclxuICAgICAgICAgICAgICAgIHRoaXMuc3luYyh0aGlzLl9kYXRlUmFuZ2VWYWx1ZSk7XHJcbiAgICAgICAgICAgICAgICB0aGlzLnJ1bkNhbGxCYWNrcyh0aGlzLl9zZXR0aW5ncy5jYWxsQmFja3MpO1xyXG4gICAgICAgICAgICAgICAgdGhpcy5kaXNwbGF5RGVidWcoKTtcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgIHZhciBzdGFydCA9IHRoaXMuZ2V0U3RhcnRQaWNrZXIoKTtcclxuICAgICAgICAgICAgc3RhcnQuYmluZChcImNoYW5nZVwiLCAoKSA9PiB7XHJcbiAgICAgICAgICAgICAgICBjb25zb2xlLmxvZygnc3RhcnRkYXRlIGNoYW5nZSBmaXJpbmcnKTtcclxuICAgICAgICAgICAgICAgIHZhciBzdGFydFBpY2tlciA9IHRoaXMuZ2V0U3RhcnRQaWNrZXIoKTtcclxuICAgICAgICAgICAgICAgIHZhciBlbmRQaWNrZXIgPSB0aGlzLmdldEVuZFBpY2tlcigpO1xyXG4gICAgICAgICAgICAgICAgaWYgKHN0YXJ0UGlja2VyLnZhbHVlKCkgPj0gZW5kUGlja2VyLnZhbHVlKCkpIHtcclxuICAgICAgICAgICAgICAgICAgICB0aGlzLm1lc3NhZ2VTaG93KCdTdGFydCBkYXRlIG11c3QgYmUgYmVmb3JlIGVuZCBkYXRlLicpO1xyXG4gICAgICAgICAgICAgICAgICAgIHN0YXJ0UGlja2VyLnZhbHVlKG1vbWVudChlbmRQaWNrZXIudmFsdWUoKSkuYWRkKC0xLCAnZGF5JykuZm9ybWF0KCdZWVlZLU1NLUREJykpO1xyXG4gICAgICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICAgICB0aGlzLl9zdGFydERhdGUgPSBzdGFydFBpY2tlci52YWx1ZSgpO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgdGhpcy5ydW5DYWxsQmFja3ModGhpcy5fc2V0dGluZ3MuY2FsbEJhY2tzKTtcclxuICAgICAgICAgICAgICAgIHRoaXMuZGlzcGxheURlYnVnKCk7XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICB2YXIgZW5kID0gdGhpcy5nZXRFbmRQaWNrZXIoKTtcclxuICAgICAgICAgICAgZW5kLmJpbmQoXCJjaGFuZ2VcIiwgKCkgPT4ge1xyXG4gICAgICAgICAgICAgICAgY29uc29sZS5sb2coJ2VuZGRhdGUgY2hhbmdlIGZpcmluZycpO1xyXG4gICAgICAgICAgICAgICAgdmFyIHN0YXJ0UGlja2VyID0gdGhpcy5nZXRTdGFydFBpY2tlcigpO1xyXG4gICAgICAgICAgICAgICAgdmFyIGVuZFBpY2tlciA9IHRoaXMuZ2V0RW5kUGlja2VyKCk7XHJcbiAgICAgICAgICAgICAgICBpZiAoc3RhcnRQaWNrZXIudmFsdWUoKSA+PSBlbmRQaWNrZXIudmFsdWUoKSkge1xyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMubWVzc2FnZVNob3coJ0VuZCBkYXRlIG11c3QgYmUgYWZ0ZXIgc3RhcnQgZGF0ZS4nKTtcclxuICAgICAgICAgICAgICAgICAgICBlbmRQaWNrZXIudmFsdWUobW9tZW50KHN0YXJ0UGlja2VyLnZhbHVlKCkpLmFkZCgxLCAnZGF5JykuZm9ybWF0KCdZWVlZLU1NLUREJykpO1xyXG4gICAgICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICAgICB0aGlzLl9lbmREYXRlID0gZW5kUGlja2VyLnZhbHVlKCk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB0aGlzLnJ1bkNhbGxCYWNrcyh0aGlzLl9zZXR0aW5ncy5jYWxsQmFja3MpO1xyXG4gICAgICAgICAgICAgICAgdGhpcy5kaXNwbGF5RGVidWcoKTtcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAvLyBydW5zIGNhbGxiYWNrcyBhZnRlciBldmVudHMgYXJlIGZpcmVkXHJcbiAgICAgICAgcHJpdmF0ZSBydW5DYWxsQmFja3MoY2FsbGJhY2tzOiBBcnJheTxhbnk+KSB7XHJcbiAgICAgICAgICAgICQuZWFjaChjYWxsYmFja3MsIChpLCBvKSA9PiB7IG8uY2FsbCgpOyB9KTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIC8vIHN5bmMncyBkYXRlIHBpY2tlcnMgYW5kIGRhdGUgcmFuZ2Ugc2VsZWN0XHJcbiAgICAgICAgcHJpdmF0ZSBzeW5jKGRhdGVSYW5nZVZhbHVlKSB7XHJcbiAgICAgICAgICAgIHZhciBkYXRlUmFuZ2VSZXN1bHQgPSB0aGlzLmdldFN0YXJ0RW5kRm9yUmFuZ2UoZGF0ZVJhbmdlVmFsdWUpO1xyXG4gICAgICAgICAgICB0aGlzLl9zdGFydERhdGUgPSBkYXRlUmFuZ2VSZXN1bHQuc3RhcnREYXRlO1xyXG4gICAgICAgICAgICB0aGlzLl9lbmREYXRlID0gZGF0ZVJhbmdlUmVzdWx0LmVuZERhdGU7XHJcbiAgICAgICAgICAgIHZhciBzdGFydFBpY2tlciA9IHRoaXMuZ2V0U3RhcnRQaWNrZXIoKTtcclxuICAgICAgICAgICAgc3RhcnRQaWNrZXIudmFsdWUoZGF0ZVJhbmdlUmVzdWx0LnN0YXJ0RGF0ZSk7XHJcbiAgICAgICAgICAgIHZhciBlbmRQaWNrZXIgPSB0aGlzLmdldEVuZFBpY2tlcigpO1xyXG4gICAgICAgICAgICBlbmRQaWNrZXIudmFsdWUoZGF0ZVJhbmdlUmVzdWx0LmVuZERhdGUpO1xyXG4gICAgICAgICAgICBpZiAodGhpcy5fZGF0ZVJhbmdlVmFsdWUgPT09IERhdGVSYW5nZVZhbHVlLkN1c3RvbSkge1xyXG4gICAgICAgICAgICAgICAgJCgnIycgKyB0aGlzLl9lbGVtZW50SWQgKyAnX2RhdGVQaWNrZXJQYW5lbCcpLnNob3coKTtcclxuICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICQoJyMnICsgdGhpcy5fZWxlbWVudElkICsgJ19kYXRlUGlja2VyUGFuZWwnKS5oaWRlKCk7XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAvLyBoZWxwZXJzIFxyXG4gICAgICAgIHByaXZhdGUgbWVzc2FnZVNob3cobWVzc2FnZSkge1xyXG4gICAgICAgICAgICAkKCcjbWVzc2FnZScpLmFkZENsYXNzKCdhbGVydCBhbGVydC1kYW5nZXInKS5zaG93KCkudGV4dChtZXNzYWdlKTtcclxuICAgICAgICAgICAgd2luZG93LnNldFRpbWVvdXQoKCkgPT4geyAkKCcjbWVzc2FnZScpLmhpZGUoKTsgfSwgMTAwMDApO1xyXG4gICAgICAgIH1cclxuICAgICAgICBwcml2YXRlIG1lc3NhZ2VIaWRlKCkge1xyXG4gICAgICAgICAgICAkKCcjbWVzc2FnZScpLmhpZGUoKTtcclxuICAgICAgICB9XHJcbiAgICAgICAgcHJpdmF0ZSBnZXRTdGFydFBpY2tlcigpIHtcclxuICAgICAgICAgICAgcmV0dXJuICQoJyMnICsgdGhpcy5fZWxlbWVudElkICsgJ19zdGFydERhdGUnKS5kYXRhKCdrZW5kb0RhdGVQaWNrZXInKTtcclxuICAgICAgICB9XHJcbiAgICAgICAgcHJpdmF0ZSBnZXRFbmRQaWNrZXIoKSB7XHJcbiAgICAgICAgICAgIHJldHVybiAkKCcjJyArIHRoaXMuX2VsZW1lbnRJZCArICdfZW5kRGF0ZScpLmRhdGEoJ2tlbmRvRGF0ZVBpY2tlcicpO1xyXG4gICAgICAgIH1cclxuICAgICAgICBwcml2YXRlICRnZXREYXRlUmFuZ2VTZWxlY3QoKSB7XHJcbiAgICAgICAgICAgIHJldHVybiAkKCcjJyArIHRoaXMuX2VsZW1lbnRJZCArICdfZGF0ZVJhbmdlJyk7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIHByaXZhdGUgZGlzcGxheURlYnVnKCkge1xyXG4gICAgICAgICAgICBjb25zb2xlLmxvZyhcInN0YXJ0ZGF0ZT1cIiArIHRoaXMuX3N0YXJ0RGF0ZSk7XHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKFwiZW5kZGF0ZT1cIiArIHRoaXMuX2VuZERhdGUpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8gcmV0dXJucyBzdGFydCBhbmQgZW5kIGRhdGUgaW4gbG9jYWwgZm9yIGEgZ2l2ZW4gRGF0ZVJhbmdlVmFsdWVcclxuICAgICAgICBwcml2YXRlIGdldFN0YXJ0RW5kRm9yUmFuZ2UoZGF0ZVJhbmdlVmFsdWU6IERhdGVSYW5nZVZhbHVlKSB7XHJcbiAgICAgICAgICAgIHN3aXRjaCAoZGF0ZVJhbmdlVmFsdWUpIHtcclxuICAgICAgICAgICAgICAgIGNhc2UgRGF0ZVJhbmdlVmFsdWUuTGFzdDI0SG91cnM6XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIG5ldyBEYXRlUmFuZ2VSZXN1bHQoXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIG1vbWVudCgpLnN1YnRyYWN0KDI0LCAnaG91cnMnKS5sb2NhbCgpLnRvRGF0ZSgpLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoKS5sb2NhbCgpLnRvRGF0ZSgpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICk7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLlRvZGF5OlxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiBuZXcgRGF0ZVJhbmdlUmVzdWx0KFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoKS5sb2NhbCgpLnN0YXJ0T2YoJ2RheScpLnRvRGF0ZSgpLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoKS5sb2NhbCgpLnRvRGF0ZSgpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICk7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLlllc3RlcmRheTpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gbmV3IERhdGVSYW5nZVJlc3VsdChcclxuICAgICAgICAgICAgICAgICAgICAgICAgbW9tZW50KCkubG9jYWwoKS5zdWJ0cmFjdCgxLCAnZGF5cycpLnN0YXJ0T2YoJ2RheScpLnRvRGF0ZSgpLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoKS5sb2NhbCgpLnN0YXJ0T2YoJ2RheScpLnRvRGF0ZSgpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICk7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLkxhc3Q3RGF5czpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gbmV3IERhdGVSYW5nZVJlc3VsdChcclxuICAgICAgICAgICAgICAgICAgICAgICAgbW9tZW50KCkubG9jYWwoKS5zdWJ0cmFjdCg3LCAnZGF5cycpLnRvRGF0ZSgpLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoKS5sb2NhbCgpLnRvRGF0ZSgpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICk7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLkxhc3QzMERheXM6XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIG5ldyBEYXRlUmFuZ2VSZXN1bHQoXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIG1vbWVudCgpLmxvY2FsKCkuc3VidHJhY3QoMzAsICdkYXlzJykudG9EYXRlKCksXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIG1vbWVudCgpLmxvY2FsKCkudG9EYXRlKClcclxuICAgICAgICAgICAgICAgICAgICAgICAgKTtcclxuICAgICAgICAgICAgICAgIGNhc2UgRGF0ZVJhbmdlVmFsdWUuTGFzdDYwRGF5czpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gbmV3IERhdGVSYW5nZVJlc3VsdChcclxuICAgICAgICAgICAgICAgICAgICAgICAgbW9tZW50KCkubG9jYWwoKS5zdWJ0cmFjdCg2MCwgJ2RheXMnKS50b0RhdGUoKSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgbW9tZW50KCkubG9jYWwoKS50b0RhdGUoKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICApO1xyXG4gICAgICAgICAgICAgICAgY2FzZSBEYXRlUmFuZ2VWYWx1ZS5MYXN0OTBEYXlzOlxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiBuZXcgRGF0ZVJhbmdlUmVzdWx0KFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoKS5sb2NhbCgpLnN1YnRyYWN0KDkwLCAnZGF5cycpLnRvRGF0ZSgpLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoKS5sb2NhbCgpLnRvRGF0ZSgpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICk7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLlRoaXNNb250aDpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gbmV3IERhdGVSYW5nZVJlc3VsdChcclxuICAgICAgICAgICAgICAgICAgICAgICAgbW9tZW50KCkubG9jYWwoKS5zdGFydE9mKCdtb250aCcpLnRvRGF0ZSgpLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoKS5sb2NhbCgpLnRvRGF0ZSgpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICk7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLkxhc3RNb250aDpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gbmV3IERhdGVSYW5nZVJlc3VsdChcclxuICAgICAgICAgICAgICAgICAgICAgICAgbW9tZW50KCkubG9jYWwoKS5zdWJ0cmFjdCgxLCAnbW9udGhzJykuc3RhcnRPZignbW9udGgnKS50b0RhdGUoKSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgbW9tZW50KCkubG9jYWwoKS5zdGFydE9mKCdtb250aCcpLnN1YnRyYWN0KCdkYXlzJywgMSkudG9EYXRlKClcclxuICAgICAgICAgICAgICAgICAgICAgICAgKTtcclxuICAgICAgICAgICAgICAgIGNhc2UgRGF0ZVJhbmdlVmFsdWUuQWxsOlxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiBuZXcgRGF0ZVJhbmdlUmVzdWx0KFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoKS5sb2NhbCgpLnN1YnRyYWN0KDEsICd5ZWFycycpLnN0YXJ0T2YoJ21vbnRoJykudG9EYXRlKCksXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIG1vbWVudCgpLmxvY2FsKCkudG9EYXRlKClcclxuICAgICAgICAgICAgICAgICAgICAgICAgKTtcclxuICAgICAgICAgICAgICAgIGNhc2UgRGF0ZVJhbmdlVmFsdWUuQ3VzdG9tOlxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiBuZXcgRGF0ZVJhbmdlUmVzdWx0KFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoKS5sb2NhbCgpLnN0YXJ0T2YoJ2RheScpLnRvRGF0ZSgpLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoKS5sb2NhbCgpLnRvRGF0ZSgpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgcmV0dXJuIG5ldyBEYXRlUmFuZ2VSZXN1bHQobnVsbCwgbnVsbCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgIH1cclxuXHJcbiAgICBjbGFzcyBEYXRlUmFuZ2VSZXN1bHQge1xyXG4gICAgICAgIHByaXZhdGUgX3N0YXJ0RGF0ZTogRGF0ZTtcclxuICAgICAgICBwcml2YXRlIF9lbmREYXRlOiBEYXRlO1xyXG4gICAgICAgIGNvbnN0cnVjdG9yKHN0YXJ0RGF0ZTogRGF0ZSwgZW5kRGF0ZTogRGF0ZSkge1xyXG4gICAgICAgICAgICB0aGlzLl9zdGFydERhdGUgPSBzdGFydERhdGU7XHJcbiAgICAgICAgICAgIHRoaXMuX2VuZERhdGUgPSBlbmREYXRlO1xyXG4gICAgICAgIH1cclxuICAgICAgICBwdWJsaWMgZ2V0IHN0YXJ0RGF0ZSgpOiBEYXRlIHtcclxuICAgICAgICAgICAgcmV0dXJuIHRoaXMuX3N0YXJ0RGF0ZTtcclxuICAgICAgICB9XHJcbiAgICAgICAgcHVibGljIGdldCBlbmREYXRlKCk6IERhdGUge1xyXG4gICAgICAgICAgICByZXR1cm4gdGhpcy5fZW5kRGF0ZTtcclxuICAgICAgICB9XHJcblxyXG4gICAgfVxyXG5cclxuICAgIGV4cG9ydCBjbGFzcyBBcHBsaWNhdGlvbklkIHtcclxuICAgICAgICBwdWJsaWMgc3RhdGljIGxvYWQoKSB7XHJcbiAgICAgICAgICAgIGlmICgkKFwiI0FwcGxpY2F0aW9uSWRcIikgIT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgdmFyIHYgPSAkLmNvb2tpZSgnQXBwbGljYXRpb25JZCcpO1xyXG4gICAgICAgICAgICAgICAgaWYgKHYgIT0gJycpIHtcclxuICAgICAgICAgICAgICAgICAgICAkKCcjQXBwbGljYXRpb25JZCBvcHRpb25bdmFsdWU9JyArICQuY29va2llKCdBcHBsaWNhdGlvbklkJykgKyAnXScpLmF0dHIoJ3NlbGVjdGVkJywgJ3NlbGVjdGVkJyk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9XHJcbiAgICAgICAgcHVibGljIHN0YXRpYyBzZXQoKSB7XHJcbiAgICAgICAgICAgIGlmICgkKFwiI0FwcGxpY2F0aW9uSWRcIikgIT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgJC5jb29raWUoJ0FwcGxpY2F0aW9uSWQnLCAkKCcjQXBwbGljYXRpb25JZCBvcHRpb246c2VsZWN0ZWQnKS52YWwoKSk7ICAgIFxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxufVxyXG4iXX0=