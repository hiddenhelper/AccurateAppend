var AccurateAppend;
(function (AccurateAppend) {
    var Ui;
    (function (Ui) {
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
        })(DateRangeValue = Ui.DateRangeValue || (Ui.DateRangeValue = {}));
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
        Ui.DateRangeWidgetSettings = DateRangeWidgetSettings;
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
        Ui.DateRangeWidget = DateRangeWidget;
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
        Ui.ApplicationId = ApplicationId;
    })(Ui = AccurateAppend.Ui || (AccurateAppend.Ui = {}));
})(AccurateAppend || (AccurateAppend = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiQWNjdXJhdGVBcHBlbmQuVWkuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJBY2N1cmF0ZUFwcGVuZC5VaS50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFDQSxJQUFPLGNBQWMsQ0E4U3BCO0FBOVNELFdBQU8sY0FBYztJQUFDLElBQUEsRUFBRSxDQThTdkI7SUE5U3FCLFdBQUEsRUFBRTtRQUVwQixJQUFZLGNBY1g7UUFkRCxXQUFZLGNBQWM7WUFDdEIsaUVBQVcsQ0FBQTtZQUNYLHFEQUFLLENBQUE7WUFDTCw2REFBUyxDQUFBO1lBQ1QsNkRBQVMsQ0FBQTtZQUNULDZEQUFTLENBQUE7WUFDVCw2REFBUyxDQUFBO1lBQ1QsK0RBQVUsQ0FBQTtZQUNWLCtEQUFVLENBQUE7WUFDViwrREFBVSxDQUFBO1lBQ1YsbUVBQVksQ0FBQTtZQUNaLGtGQUFtQixDQUFBO1lBQ25CLGtEQUFHLENBQUE7WUFDSCx3REFBTSxDQUFBO1FBQ1YsQ0FBQyxFQWRXLGNBQWMsR0FBZCxpQkFBYyxLQUFkLGlCQUFjLFFBY3pCO1FBRUQ7WUFBQTtZQWlDQSxDQUFDO1lBaENpQix1Q0FBYSxHQUEzQixVQUE0QixjQUE4QjtnQkFDdEQsUUFBUSxjQUFjLEVBQUU7b0JBQ3BCLEtBQUssY0FBYyxDQUFDLFdBQVc7d0JBQzNCLE9BQU8sZUFBZSxDQUFDO29CQUMzQixLQUFLLGNBQWMsQ0FBQyxLQUFLO3dCQUNyQixPQUFPLE9BQU8sQ0FBQztvQkFDbkIsS0FBSyxjQUFjLENBQUMsU0FBUzt3QkFDekIsT0FBTyxXQUFXLENBQUM7b0JBQ3ZCLEtBQUssY0FBYyxDQUFDLFNBQVM7d0JBQ3pCLE9BQU8sYUFBYSxDQUFDO29CQUN6QixLQUFLLGNBQWMsQ0FBQyxTQUFTO3dCQUN6QixPQUFPLFlBQVksQ0FBQztvQkFDeEIsS0FBSyxjQUFjLENBQUMsU0FBUzt3QkFDekIsT0FBTyxZQUFZLENBQUM7b0JBQ3hCLEtBQUssY0FBYyxDQUFDLFVBQVU7d0JBQzFCLE9BQU8sY0FBYyxDQUFDO29CQUMxQixLQUFLLGNBQWMsQ0FBQyxVQUFVO3dCQUMxQixPQUFPLGNBQWMsQ0FBQztvQkFDMUIsS0FBSyxjQUFjLENBQUMsVUFBVTt3QkFDMUIsT0FBTyxjQUFjLENBQUM7b0JBQzFCLEtBQUssY0FBYyxDQUFDLFlBQVk7d0JBQzVCLE9BQU8sZUFBZSxDQUFDO29CQUMzQixLQUFLLGNBQWMsQ0FBQyxtQkFBbUI7d0JBQ25DLE9BQU8sd0JBQXdCLENBQUM7b0JBQ3BDLEtBQUssY0FBYyxDQUFDLEdBQUc7d0JBQ25CLE9BQU8sS0FBSyxDQUFDO29CQUNqQixLQUFLLGNBQWMsQ0FBQyxNQUFNO3dCQUN0QixPQUFPLFFBQVEsQ0FBQztvQkFDcEI7d0JBQ0ksT0FBTyxTQUFTLENBQUM7aUJBQ3hCO1lBQ0wsQ0FBQztZQUNMLGdDQUFDO1FBQUQsQ0FBQyxBQWpDRCxJQWlDQztRQUVEO1lBQ0ksaUNBRVcsZ0JBQXVDLEVBRXZDLFlBQTRCLEVBRTVCLFNBQXFCO2dCQUpyQixxQkFBZ0IsR0FBaEIsZ0JBQWdCLENBQXVCO2dCQUV2QyxpQkFBWSxHQUFaLFlBQVksQ0FBZ0I7Z0JBRTVCLGNBQVMsR0FBVCxTQUFTLENBQVk7WUFDeEIsQ0FBQztZQUNiLDhCQUFDO1FBQUQsQ0FBQyxBQVRELElBU0M7UUFUWSwwQkFBdUIsMEJBU25DLENBQUE7UUFHRDtZQVFJLHlCQUFZLFNBQWlCLEVBQUUsUUFBaUM7Z0JBQzVELElBQUksQ0FBQyxTQUFTLEdBQUcsUUFBUSxDQUFDO2dCQUMxQixJQUFJLENBQUMsVUFBVSxHQUFHLFNBQVMsQ0FBQztnQkFDNUIsSUFBSSxDQUFDLGVBQWUsR0FBRyxRQUFRLENBQUMsWUFBWSxDQUFDO2dCQUM3QyxJQUFJLENBQUMsV0FBVyxDQUFDLElBQUksQ0FBQyxlQUFlLENBQUMsQ0FBQztnQkFDdkMsSUFBSSxDQUFDLFNBQVMsRUFBRSxDQUFDO2dCQUNqQixJQUFJLENBQUMsSUFBSSxDQUFDLFFBQVEsQ0FBQyxZQUFZLENBQUMsQ0FBQztnQkFDakMsSUFBSSxDQUFDLFlBQVksRUFBRSxDQUFDO1lBQ3hCLENBQUM7WUFHTSxzQ0FBWSxHQUFuQixjQUF3QixPQUFPLElBQUksQ0FBQyxVQUFVLENBQUMsQ0FBQyxDQUFDO1lBQzFDLG9DQUFVLEdBQWpCLGNBQXNCLE9BQU8sSUFBSSxDQUFDLFFBQVEsQ0FBQyxDQUFDLENBQUM7WUFDdEMsMkNBQWlCLEdBQXhCLGNBQTZCLE9BQU8sSUFBSSxDQUFDLGVBQWUsQ0FBQyxDQUFDLENBQUM7WUFFcEQsMkNBQWlCLEdBQXhCLFVBQXlCLEtBQXFCO2dCQUMxQyxJQUFJLENBQUMsZUFBZSxHQUFHLEtBQUssQ0FBQztnQkFDN0IsSUFBSSxPQUFPLEdBQUcsSUFBSSxDQUFDLG1CQUFtQixFQUFFLENBQUM7Z0JBQ3pDLE9BQU8sQ0FBQyxHQUFHLENBQUMsY0FBYyxDQUFDLEtBQUssQ0FBQyxDQUFDLENBQUM7Z0JBQ25DLElBQUksQ0FBQyxJQUFJLENBQUMsSUFBSSxDQUFDLGVBQWUsQ0FBQyxDQUFDO2dCQUNoQyxJQUFJLENBQUMsWUFBWSxDQUFDLElBQUksQ0FBQyxTQUFTLENBQUMsU0FBUyxDQUFDLENBQUM7WUFDaEQsQ0FBQztZQUNNLGlDQUFPLEdBQWQ7Z0JBQ0ksSUFBSSxDQUFDLFlBQVksQ0FBQyxJQUFJLENBQUMsU0FBUyxDQUFDLFNBQVMsQ0FBQyxDQUFDO1lBQ2hELENBQUM7WUFHTyxxQ0FBVyxHQUFuQixVQUFvQixLQUFxQjtnQkFHckMsSUFBSSxJQUFJLEdBQUcsQ0FBQyxDQUFDLEdBQUcsR0FBRyxJQUFJLENBQUMsVUFBVSxDQUFDLENBQUM7Z0JBRXBDLElBQUksZ0JBQWdCLEdBQUcsQ0FBQyxDQUFDLFlBQVksR0FBRyxJQUFJLENBQUMsVUFBVSxHQUFHLDJCQUEyQixDQUFDLENBQUM7Z0JBQ3ZGLGdCQUFnQixDQUFDLElBQUksRUFBRSxDQUFDO2dCQUN4QixJQUFJLGdCQUFnQixHQUFHLENBQUMsQ0FBQyxhQUFhLEdBQUcsSUFBSSxDQUFDLFVBQVUsR0FBRyxZQUFZLEdBQUcsS0FBSyxDQUFDLENBQUM7Z0JBQ2pGLGdCQUFnQixDQUFDLE1BQU0sQ0FBQyxnQkFBZ0IsQ0FBQyxDQUFDLE1BQU0sQ0FBQyxpREFBaUQsQ0FBQyxDQUFDO2dCQUNwRyxJQUFJLGFBQWEsR0FBRyxDQUFDLENBQUMsYUFBYSxHQUFHLElBQUksQ0FBQyxVQUFVLEdBQUcsVUFBVSxHQUFHLEtBQUssQ0FBQyxDQUFDO2dCQUM1RSxnQkFBZ0IsQ0FBQyxNQUFNLENBQUMsYUFBYSxDQUFDLENBQUM7Z0JBQ3ZDLGdCQUFnQixDQUFDLGVBQWUsQ0FBQyxFQUFFLE1BQU0sRUFBRSxZQUFZLEVBQUUsS0FBSyxFQUFFLElBQUksQ0FBQyxVQUFVLEVBQUUsQ0FBQyxDQUFDO2dCQUNuRixhQUFhLENBQUMsZUFBZSxDQUFDLEVBQUUsTUFBTSxFQUFFLFlBQVksRUFBRSxLQUFLLEVBQUUsSUFBSSxDQUFDLFFBQVEsRUFBRSxDQUFDLENBQUM7Z0JBQzlFLElBQUksQ0FBQyxNQUFNLENBQUMsZ0JBQWdCLENBQUMsQ0FBQztnQkFFOUIsSUFBSSxPQUFPLEdBQUcsQ0FBQyxDQUFDLGNBQWMsR0FBRyxJQUFJLENBQUMsVUFBVSxHQUFHLFlBQVksR0FBRyx5RkFBeUYsQ0FBQyxDQUFDO2dCQUM3SixDQUFDLENBQUMsSUFBSSxDQUFDLElBQUksQ0FBQyxTQUFTLENBQUMsZ0JBQWdCLEVBQUUsVUFBQyxDQUFDLEVBQUUsQ0FBQyxJQUFPLE9BQU8sQ0FBQyxNQUFNLENBQUMsQ0FBQyxDQUFDLFVBQVUsR0FBRyxDQUFDLEtBQUssS0FBSyxDQUFDLENBQUMsQ0FBQyxDQUFDLFVBQVUsQ0FBQyxDQUFDLENBQUMsRUFBRSxDQUFDLEdBQUcsSUFBSSxDQUFDLENBQUMsR0FBRyxDQUFDLGNBQWMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLElBQUksQ0FBQyx5QkFBeUIsQ0FBQyxhQUFhLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUM7Z0JBQ3ZNLElBQUksQ0FBQyxNQUFNLENBQUMsT0FBTyxDQUFDLENBQUM7WUFDekIsQ0FBQztZQUdPLG1DQUFTLEdBQWpCO2dCQUFBLGlCQXVDQztnQkF0Q0csSUFBSSxLQUFLLEdBQUcsQ0FBQyxDQUFDLEdBQUcsR0FBRyxJQUFJLENBQUMsVUFBVSxDQUFDLENBQUM7Z0JBQ3JDLElBQUksTUFBTSxHQUFHLEtBQUssQ0FBQyxJQUFJLENBQUMsUUFBUSxDQUFDLENBQUM7Z0JBQ2xDLE1BQU0sQ0FBQyxNQUFNLENBQUM7b0JBQ1YsT0FBTyxDQUFDLEdBQUcsQ0FBQyx5QkFBeUIsQ0FBQyxDQUFDO29CQUN2QyxJQUFJLENBQUMsR0FBVyxDQUFDLENBQUMsR0FBRyxHQUFHLEtBQUksQ0FBQyxVQUFVLEdBQUcsWUFBWSxDQUFDLENBQUMsR0FBRyxFQUFFLENBQUM7b0JBQzlELEtBQUksQ0FBQyxlQUFlLEdBQUcsY0FBYyxDQUFDLENBQUMsQ0FBQyxDQUFDO29CQUN6QyxLQUFJLENBQUMsSUFBSSxDQUFDLEtBQUksQ0FBQyxlQUFlLENBQUMsQ0FBQztvQkFDaEMsS0FBSSxDQUFDLFlBQVksQ0FBQyxLQUFJLENBQUMsU0FBUyxDQUFDLFNBQVMsQ0FBQyxDQUFDO29CQUM1QyxLQUFJLENBQUMsWUFBWSxFQUFFLENBQUM7Z0JBQ3hCLENBQUMsQ0FBQyxDQUFDO2dCQUNILElBQUksS0FBSyxHQUFHLElBQUksQ0FBQyxjQUFjLEVBQUUsQ0FBQztnQkFDbEMsS0FBSyxDQUFDLElBQUksQ0FBQyxRQUFRLEVBQUU7b0JBQ2pCLE9BQU8sQ0FBQyxHQUFHLENBQUMseUJBQXlCLENBQUMsQ0FBQztvQkFDdkMsSUFBSSxXQUFXLEdBQUcsS0FBSSxDQUFDLGNBQWMsRUFBRSxDQUFDO29CQUN4QyxJQUFJLFNBQVMsR0FBRyxLQUFJLENBQUMsWUFBWSxFQUFFLENBQUM7b0JBQ3BDLElBQUksV0FBVyxDQUFDLEtBQUssRUFBRSxJQUFJLFNBQVMsQ0FBQyxLQUFLLEVBQUUsRUFBRTt3QkFDMUMsS0FBSSxDQUFDLFdBQVcsQ0FBQyxxQ0FBcUMsQ0FBQyxDQUFDO3dCQUN4RCxXQUFXLENBQUMsS0FBSyxDQUFDLE1BQU0sQ0FBQyxTQUFTLENBQUMsS0FBSyxFQUFFLENBQUMsQ0FBQyxHQUFHLENBQUMsQ0FBQyxDQUFDLEVBQUUsS0FBSyxDQUFDLENBQUMsTUFBTSxDQUFDLFlBQVksQ0FBQyxDQUFDLENBQUM7cUJBQ3BGO3lCQUFNO3dCQUNILEtBQUksQ0FBQyxVQUFVLEdBQUcsV0FBVyxDQUFDLEtBQUssRUFBRSxDQUFDO3FCQUN6QztvQkFDRCxLQUFJLENBQUMsWUFBWSxDQUFDLEtBQUksQ0FBQyxTQUFTLENBQUMsU0FBUyxDQUFDLENBQUM7b0JBQzVDLEtBQUksQ0FBQyxZQUFZLEVBQUUsQ0FBQztnQkFDeEIsQ0FBQyxDQUFDLENBQUM7Z0JBQ0gsSUFBSSxHQUFHLEdBQUcsSUFBSSxDQUFDLFlBQVksRUFBRSxDQUFDO2dCQUM5QixHQUFHLENBQUMsSUFBSSxDQUFDLFFBQVEsRUFBRTtvQkFDZixPQUFPLENBQUMsR0FBRyxDQUFDLHVCQUF1QixDQUFDLENBQUM7b0JBQ3JDLElBQUksV0FBVyxHQUFHLEtBQUksQ0FBQyxjQUFjLEVBQUUsQ0FBQztvQkFDeEMsSUFBSSxTQUFTLEdBQUcsS0FBSSxDQUFDLFlBQVksRUFBRSxDQUFDO29CQUNwQyxJQUFJLFdBQVcsQ0FBQyxLQUFLLEVBQUUsSUFBSSxTQUFTLENBQUMsS0FBSyxFQUFFLEVBQUU7d0JBQzFDLEtBQUksQ0FBQyxXQUFXLENBQUMsb0NBQW9DLENBQUMsQ0FBQzt3QkFDdkQsU0FBUyxDQUFDLEtBQUssQ0FBQyxNQUFNLENBQUMsV0FBVyxDQUFDLEtBQUssRUFBRSxDQUFDLENBQUMsR0FBRyxDQUFDLENBQUMsRUFBRSxLQUFLLENBQUMsQ0FBQyxNQUFNLENBQUMsWUFBWSxDQUFDLENBQUMsQ0FBQztxQkFDbkY7eUJBQU07d0JBQ0gsS0FBSSxDQUFDLFFBQVEsR0FBRyxTQUFTLENBQUMsS0FBSyxFQUFFLENBQUM7cUJBQ3JDO29CQUNELEtBQUksQ0FBQyxZQUFZLENBQUMsS0FBSSxDQUFDLFNBQVMsQ0FBQyxTQUFTLENBQUMsQ0FBQztvQkFDNUMsS0FBSSxDQUFDLFlBQVksRUFBRSxDQUFDO2dCQUN4QixDQUFDLENBQUMsQ0FBQztZQUNQLENBQUM7WUFHTyxzQ0FBWSxHQUFwQixVQUFxQixTQUFxQjtnQkFDdEMsQ0FBQyxDQUFDLElBQUksQ0FBQyxTQUFTLEVBQUUsVUFBQyxDQUFDLEVBQUUsQ0FBQyxJQUFPLENBQUMsQ0FBQyxJQUFJLEVBQUUsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDO1lBQy9DLENBQUM7WUFHTyw4QkFBSSxHQUFaLFVBQWEsY0FBYztnQkFDdkIsSUFBSSxlQUFlLEdBQUcsSUFBSSxDQUFDLG1CQUFtQixDQUFDLGNBQWMsQ0FBQyxDQUFDO2dCQUMvRCxJQUFJLENBQUMsVUFBVSxHQUFHLGVBQWUsQ0FBQyxTQUFTLENBQUM7Z0JBQzVDLElBQUksQ0FBQyxRQUFRLEdBQUcsZUFBZSxDQUFDLE9BQU8sQ0FBQztnQkFDeEMsSUFBSSxXQUFXLEdBQUcsSUFBSSxDQUFDLGNBQWMsRUFBRSxDQUFDO2dCQUN4QyxXQUFXLENBQUMsS0FBSyxDQUFDLGVBQWUsQ0FBQyxTQUFTLENBQUMsQ0FBQztnQkFDN0MsSUFBSSxTQUFTLEdBQUcsSUFBSSxDQUFDLFlBQVksRUFBRSxDQUFDO2dCQUNwQyxTQUFTLENBQUMsS0FBSyxDQUFDLGVBQWUsQ0FBQyxPQUFPLENBQUMsQ0FBQztnQkFDekMsSUFBSSxJQUFJLENBQUMsZUFBZSxLQUFLLGNBQWMsQ0FBQyxNQUFNLEVBQUU7b0JBQ2hELENBQUMsQ0FBQyxHQUFHLEdBQUcsSUFBSSxDQUFDLFVBQVUsR0FBRyxrQkFBa0IsQ0FBQyxDQUFDLElBQUksRUFBRSxDQUFDO2lCQUN4RDtxQkFBTTtvQkFDSCxDQUFDLENBQUMsR0FBRyxHQUFHLElBQUksQ0FBQyxVQUFVLEdBQUcsa0JBQWtCLENBQUMsQ0FBQyxJQUFJLEVBQUUsQ0FBQztpQkFDeEQ7WUFFTCxDQUFDO1lBR08scUNBQVcsR0FBbkIsVUFBb0IsT0FBTztnQkFDdkIsQ0FBQyxDQUFDLFVBQVUsQ0FBQyxDQUFDLFFBQVEsQ0FBQyxvQkFBb0IsQ0FBQyxDQUFDLElBQUksRUFBRSxDQUFDLElBQUksQ0FBQyxPQUFPLENBQUMsQ0FBQztnQkFDbEUsTUFBTSxDQUFDLFVBQVUsQ0FBQyxjQUFRLENBQUMsQ0FBQyxVQUFVLENBQUMsQ0FBQyxJQUFJLEVBQUUsQ0FBQyxDQUFDLENBQUMsRUFBRSxLQUFLLENBQUMsQ0FBQztZQUM5RCxDQUFDO1lBQ08scUNBQVcsR0FBbkI7Z0JBQ0ksQ0FBQyxDQUFDLFVBQVUsQ0FBQyxDQUFDLElBQUksRUFBRSxDQUFDO1lBQ3pCLENBQUM7WUFDTyx3Q0FBYyxHQUF0QjtnQkFDSSxPQUFPLENBQUMsQ0FBQyxHQUFHLEdBQUcsSUFBSSxDQUFDLFVBQVUsR0FBRyxZQUFZLENBQUMsQ0FBQyxJQUFJLENBQUMsaUJBQWlCLENBQUMsQ0FBQztZQUMzRSxDQUFDO1lBQ08sc0NBQVksR0FBcEI7Z0JBQ0ksT0FBTyxDQUFDLENBQUMsR0FBRyxHQUFHLElBQUksQ0FBQyxVQUFVLEdBQUcsVUFBVSxDQUFDLENBQUMsSUFBSSxDQUFDLGlCQUFpQixDQUFDLENBQUM7WUFDekUsQ0FBQztZQUNPLDZDQUFtQixHQUEzQjtnQkFDSSxPQUFPLENBQUMsQ0FBQyxHQUFHLEdBQUcsSUFBSSxDQUFDLFVBQVUsR0FBRyxZQUFZLENBQUMsQ0FBQztZQUNuRCxDQUFDO1lBQ08sc0NBQVksR0FBcEI7Z0JBQ0ksT0FBTyxDQUFDLEdBQUcsQ0FBQyxZQUFZLEdBQUcsSUFBSSxDQUFDLFVBQVUsQ0FBQyxDQUFDO2dCQUM1QyxPQUFPLENBQUMsR0FBRyxDQUFDLFVBQVUsR0FBRyxJQUFJLENBQUMsUUFBUSxDQUFDLENBQUM7WUFDNUMsQ0FBQztZQUdPLDZDQUFtQixHQUEzQixVQUE0QixjQUE4QjtnQkFDdEQsUUFBUSxjQUFjLEVBQUU7b0JBQ3BCLEtBQUssY0FBYyxDQUFDLFdBQVc7d0JBQzNCLE9BQU8sSUFBSSxlQUFlLENBQ3RCLE1BQU0sRUFBRSxDQUFDLFFBQVEsQ0FBQyxFQUFFLEVBQUUsT0FBTyxDQUFDLENBQUMsS0FBSyxFQUFFLENBQUMsTUFBTSxFQUFFLEVBQy9DLE1BQU0sRUFBRSxDQUFDLEtBQUssRUFBRSxDQUFDLE1BQU0sRUFBRSxDQUN4QixDQUFDO29CQUNWLEtBQUssY0FBYyxDQUFDLEtBQUs7d0JBQ3JCLE9BQU8sSUFBSSxlQUFlLENBQ3RCLE1BQU0sRUFBRSxDQUFDLEtBQUssRUFBRSxDQUFDLE9BQU8sQ0FBQyxLQUFLLENBQUMsQ0FBQyxNQUFNLEVBQUUsRUFDeEMsTUFBTSxFQUFFLENBQUMsS0FBSyxFQUFFLENBQUMsTUFBTSxFQUFFLENBQ3hCLENBQUM7b0JBQ1YsS0FBSyxjQUFjLENBQUMsU0FBUzt3QkFDekIsT0FBTyxJQUFJLGVBQWUsQ0FDdEIsTUFBTSxFQUFFLENBQUMsS0FBSyxFQUFFLENBQUMsUUFBUSxDQUFDLENBQUMsRUFBRSxNQUFNLENBQUMsQ0FBQyxPQUFPLENBQUMsS0FBSyxDQUFDLENBQUMsTUFBTSxFQUFFLEVBQzVELE1BQU0sRUFBRSxDQUFDLEtBQUssRUFBRSxDQUFDLE9BQU8sQ0FBQyxLQUFLLENBQUMsQ0FBQyxNQUFNLEVBQUUsQ0FDdkMsQ0FBQztvQkFDVixLQUFLLGNBQWMsQ0FBQyxTQUFTO3dCQUN6QixPQUFPLElBQUksZUFBZSxDQUN0QixNQUFNLEVBQUUsQ0FBQyxLQUFLLEVBQUUsQ0FBQyxRQUFRLENBQUMsQ0FBQyxFQUFFLE1BQU0sQ0FBQyxDQUFDLE1BQU0sRUFBRSxFQUM3QyxNQUFNLEVBQUUsQ0FBQyxLQUFLLEVBQUUsQ0FBQyxNQUFNLEVBQUUsQ0FDeEIsQ0FBQztvQkFDVixLQUFLLGNBQWMsQ0FBQyxVQUFVO3dCQUMxQixPQUFPLElBQUksZUFBZSxDQUN0QixNQUFNLEVBQUUsQ0FBQyxLQUFLLEVBQUUsQ0FBQyxRQUFRLENBQUMsRUFBRSxFQUFFLE1BQU0sQ0FBQyxDQUFDLE1BQU0sRUFBRSxFQUM5QyxNQUFNLEVBQUUsQ0FBQyxLQUFLLEVBQUUsQ0FBQyxNQUFNLEVBQUUsQ0FDeEIsQ0FBQztvQkFDVixLQUFLLGNBQWMsQ0FBQyxVQUFVO3dCQUMxQixPQUFPLElBQUksZUFBZSxDQUN0QixNQUFNLEVBQUUsQ0FBQyxLQUFLLEVBQUUsQ0FBQyxRQUFRLENBQUMsRUFBRSxFQUFFLE1BQU0sQ0FBQyxDQUFDLE1BQU0sRUFBRSxFQUM5QyxNQUFNLEVBQUUsQ0FBQyxLQUFLLEVBQUUsQ0FBQyxNQUFNLEVBQUUsQ0FDeEIsQ0FBQztvQkFDVixLQUFLLGNBQWMsQ0FBQyxVQUFVO3dCQUMxQixPQUFPLElBQUksZUFBZSxDQUN0QixNQUFNLEVBQUUsQ0FBQyxLQUFLLEVBQUUsQ0FBQyxRQUFRLENBQUMsRUFBRSxFQUFFLE1BQU0sQ0FBQyxDQUFDLE1BQU0sRUFBRSxFQUM5QyxNQUFNLEVBQUUsQ0FBQyxLQUFLLEVBQUUsQ0FBQyxNQUFNLEVBQUUsQ0FDeEIsQ0FBQztvQkFDVixLQUFLLGNBQWMsQ0FBQyxTQUFTO3dCQUN6QixPQUFPLElBQUksZUFBZSxDQUN0QixNQUFNLEVBQUUsQ0FBQyxLQUFLLEVBQUUsQ0FBQyxPQUFPLENBQUMsT0FBTyxDQUFDLENBQUMsTUFBTSxFQUFFLEVBQzFDLE1BQU0sRUFBRSxDQUFDLEtBQUssRUFBRSxDQUFDLE1BQU0sRUFBRSxDQUN4QixDQUFDO29CQUNWLEtBQUssY0FBYyxDQUFDLFNBQVM7d0JBQ3pCLE9BQU8sSUFBSSxlQUFlLENBQ3RCLE1BQU0sRUFBRSxDQUFDLEtBQUssRUFBRSxDQUFDLFFBQVEsQ0FBQyxDQUFDLEVBQUUsUUFBUSxDQUFDLENBQUMsT0FBTyxDQUFDLE9BQU8sQ0FBQyxDQUFDLE1BQU0sRUFBRSxFQUNoRSxNQUFNLEVBQUUsQ0FBQyxLQUFLLEVBQUUsQ0FBQyxPQUFPLENBQUMsT0FBTyxDQUFDLENBQUMsUUFBUSxDQUFDLE1BQU0sRUFBRSxDQUFDLENBQUMsQ0FBQyxNQUFNLEVBQUUsQ0FDN0QsQ0FBQztvQkFDVixLQUFLLGNBQWMsQ0FBQyxHQUFHO3dCQUNuQixPQUFPLElBQUksZUFBZSxDQUN0QixNQUFNLEVBQUUsQ0FBQyxLQUFLLEVBQUUsQ0FBQyxRQUFRLENBQUMsQ0FBQyxFQUFFLE9BQU8sQ0FBQyxDQUFDLE9BQU8sQ0FBQyxPQUFPLENBQUMsQ0FBQyxNQUFNLEVBQUUsRUFDL0QsTUFBTSxFQUFFLENBQUMsS0FBSyxFQUFFLENBQUMsTUFBTSxFQUFFLENBQ3hCLENBQUM7b0JBQ1YsS0FBSyxjQUFjLENBQUMsTUFBTTt3QkFDdEIsT0FBTyxJQUFJLGVBQWUsQ0FDdEIsTUFBTSxFQUFFLENBQUMsS0FBSyxFQUFFLENBQUMsT0FBTyxDQUFDLEtBQUssQ0FBQyxDQUFDLE1BQU0sRUFBRSxFQUN4QyxNQUFNLEVBQUUsQ0FBQyxLQUFLLEVBQUUsQ0FBQyxNQUFNLEVBQUUsQ0FDeEIsQ0FBQztpQkFDYjtnQkFDRCxPQUFPLElBQUksZUFBZSxDQUFDLElBQUksRUFBRSxJQUFJLENBQUMsQ0FBQztZQUMzQyxDQUFDO1lBRUwsc0JBQUM7UUFBRCxDQUFDLEFBM01ELElBMk1DO1FBM01ZLGtCQUFlLGtCQTJNM0IsQ0FBQTtRQUVEO1lBR0kseUJBQVksU0FBZSxFQUFFLE9BQWE7Z0JBQ3RDLElBQUksQ0FBQyxVQUFVLEdBQUcsU0FBUyxDQUFDO2dCQUM1QixJQUFJLENBQUMsUUFBUSxHQUFHLE9BQU8sQ0FBQztZQUM1QixDQUFDO1lBQ0Qsc0JBQVcsc0NBQVM7cUJBQXBCO29CQUNJLE9BQU8sSUFBSSxDQUFDLFVBQVUsQ0FBQztnQkFDM0IsQ0FBQzs7O2VBQUE7WUFDRCxzQkFBVyxvQ0FBTztxQkFBbEI7b0JBQ0ksT0FBTyxJQUFJLENBQUMsUUFBUSxDQUFDO2dCQUN6QixDQUFDOzs7ZUFBQTtZQUVMLHNCQUFDO1FBQUQsQ0FBQyxBQWRELElBY0M7UUFFRDtZQUFBO1lBY0EsQ0FBQztZQWJpQixrQkFBSSxHQUFsQjtnQkFDSSxJQUFJLENBQUMsQ0FBQyxnQkFBZ0IsQ0FBQyxJQUFJLElBQUksRUFBRTtvQkFDN0IsSUFBSSxDQUFDLEdBQUcsQ0FBQyxDQUFDLE1BQU0sQ0FBQyxlQUFlLENBQUMsQ0FBQztvQkFDbEMsSUFBSSxDQUFDLElBQUksRUFBRSxFQUFFO3dCQUNULENBQUMsQ0FBQyw4QkFBOEIsR0FBRyxDQUFDLENBQUMsTUFBTSxDQUFDLGVBQWUsQ0FBQyxHQUFHLEdBQUcsQ0FBQyxDQUFDLElBQUksQ0FBQyxVQUFVLEVBQUUsVUFBVSxDQUFDLENBQUM7cUJBQ3BHO2lCQUNKO1lBQ0wsQ0FBQztZQUNhLGlCQUFHLEdBQWpCO2dCQUNJLElBQUksQ0FBQyxDQUFDLGdCQUFnQixDQUFDLElBQUksSUFBSSxFQUFFO29CQUM3QixDQUFDLENBQUMsTUFBTSxDQUFDLGVBQWUsRUFBRSxDQUFDLENBQUMsZ0NBQWdDLENBQUMsQ0FBQyxHQUFHLEVBQUUsQ0FBQyxDQUFDO2lCQUN4RTtZQUNMLENBQUM7WUFDTCxvQkFBQztRQUFELENBQUMsQUFkRCxJQWNDO1FBZFksZ0JBQWEsZ0JBY3pCLENBQUE7SUFFTCxDQUFDLEVBOVNxQixFQUFFLEdBQUYsaUJBQUUsS0FBRixpQkFBRSxRQThTdkI7QUFBRCxDQUFDLEVBOVNNLGNBQWMsS0FBZCxjQUFjLFFBOFNwQiIsInNvdXJjZXNDb250ZW50IjpbIlxyXG5tb2R1bGUgQWNjdXJhdGVBcHBlbmQuVWkge1xyXG5cclxuICAgIGV4cG9ydCBlbnVtIERhdGVSYW5nZVZhbHVlIHtcclxuICAgICAgICBMYXN0MjRIb3VycyxcclxuICAgICAgICBUb2RheSxcclxuICAgICAgICBZZXN0ZXJkYXksXHJcbiAgICAgICAgTGFzdDdEYXlzLFxyXG4gICAgICAgIFRoaXNNb250aCxcclxuICAgICAgICBMYXN0TW9udGgsXHJcbiAgICAgICAgTGFzdDMwRGF5cyxcclxuICAgICAgICBMYXN0NjBEYXlzLFxyXG4gICAgICAgIExhc3Q5MERheXMsXHJcbiAgICAgICAgQ3VycmVudE1vbnRoLFxyXG4gICAgICAgIFByZXZpb3VzVG9MYXN0TW9udGgsXHJcbiAgICAgICAgQWxsLFxyXG4gICAgICAgIEN1c3RvbVxyXG4gICAgfVxyXG5cclxuICAgIGNsYXNzIERhdGVSYW5nZVZhbHVlRGVzY3JpcHRpb24ge1xyXG4gICAgICAgIHB1YmxpYyBzdGF0aWMgZ2V0RGVjcmlwdGlvbihkYXRlUmFuZ2VWYWx1ZTogRGF0ZVJhbmdlVmFsdWUpIHtcclxuICAgICAgICAgICAgc3dpdGNoIChkYXRlUmFuZ2VWYWx1ZSkge1xyXG4gICAgICAgICAgICAgICAgY2FzZSBEYXRlUmFuZ2VWYWx1ZS5MYXN0MjRIb3VyczpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gXCJMYXN0IDI0IEhvdXJzXCI7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLlRvZGF5OlxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiBcIlRvZGF5XCI7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLlllc3RlcmRheTpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gXCJZZXN0ZXJkYXlcIjtcclxuICAgICAgICAgICAgICAgIGNhc2UgRGF0ZVJhbmdlVmFsdWUuTGFzdDdEYXlzOlxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiBcIkxhc3QgNyBEYXlzXCI7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLlRoaXNNb250aDpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gXCJUaGlzIE1vbnRoXCI7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLkxhc3RNb250aDpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gXCJMYXN0IE1vbnRoXCI7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLkxhc3QzMERheXM6XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIFwiTGFzdCAzMCBEYXlzXCI7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLkxhc3Q2MERheXM6XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIFwiTGFzdCA2MCBEYXlzXCI7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLkxhc3Q5MERheXM6XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIFwiTGFzdCA5MCBEYXlzXCI7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLkN1cnJlbnRNb250aDpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gXCJDdXJyZW50IE1vbnRoXCI7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLlByZXZpb3VzVG9MYXN0TW9udGg6XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIFwiUHJldmlvdXMgVG8gTGFzdCBNb250aFwiO1xyXG4gICAgICAgICAgICAgICAgY2FzZSBEYXRlUmFuZ2VWYWx1ZS5BbGw6XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIFwiQWxsXCI7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLkN1c3RvbTpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gXCJDdXN0b21cIjtcclxuICAgICAgICAgICAgICAgIGRlZmF1bHQ6XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIHVuZGVmaW5lZDtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBleHBvcnQgY2xhc3MgRGF0ZVJhbmdlV2lkZ2V0U2V0dGluZ3Mge1xyXG4gICAgICAgIGNvbnN0cnVjdG9yKFxyXG4gICAgICAgICAgICAvLyBvcHRpb25zIHRvIGJlIGluY2x1ZGVkIGluIHNlbGVjdFxyXG4gICAgICAgICAgICBwdWJsaWMgZGF0ZVJhbmdlT3B0aW9uczogQXJyYXk8RGF0ZVJhbmdlVmFsdWU+LFxyXG4gICAgICAgICAgICAvLyBkZWZhdWx0IHZhbHVlIHRvIGJlIGNob3NlbiBpbiBzZWxlY3RcclxuICAgICAgICAgICAgcHVibGljIGRlZmF1bHRWYWx1ZTogRGF0ZVJhbmdlVmFsdWUsXHJcbiAgICAgICAgICAgIC8vIGNhbGxiYWNrcyB0byBiZSBleGVjdXRlZCB3aGVuIGNoYW5nZSBpcyByYWlzZWRcclxuICAgICAgICAgICAgcHVibGljIGNhbGxCYWNrczogQXJyYXk8YW55PlxyXG4gICAgICAgICAgICApIHsgfVxyXG4gICAgfVxyXG5cclxuICAgIC8vIHJlbmRlcnMgY29udHJvbCB0aGF0IHJlbmRlcnMgZGF0ZSByYWduZSBERCBhbG9uZyB3aXRoIGRhdGUgcGlja2VycyBmb3IgY3VzdG9tIGRhdGUgcmFuZ2VcclxuICAgIGV4cG9ydCBjbGFzcyBEYXRlUmFuZ2VXaWRnZXQge1xyXG5cclxuICAgICAgICBwcml2YXRlIF9zdGFydERhdGU6IERhdGU7XHJcbiAgICAgICAgcHJpdmF0ZSBfZW5kRGF0ZTogRGF0ZTtcclxuICAgICAgICBwcml2YXRlIF9kYXRlUmFuZ2VWYWx1ZTogRGF0ZVJhbmdlVmFsdWU7XHJcbiAgICAgICAgcHJpdmF0ZSBfZWxlbWVudElkOiBzdHJpbmc7XHJcbiAgICAgICAgcHJpdmF0ZSBfc2V0dGluZ3M6IGFueTtcclxuXHJcbiAgICAgICAgY29uc3RydWN0b3IoZWxlbWVudElkOiBzdHJpbmcsIHNldHRpbmdzOiBEYXRlUmFuZ2VXaWRnZXRTZXR0aW5ncykge1xyXG4gICAgICAgICAgICB0aGlzLl9zZXR0aW5ncyA9IHNldHRpbmdzO1xyXG4gICAgICAgICAgICB0aGlzLl9lbGVtZW50SWQgPSBlbGVtZW50SWQ7XHJcbiAgICAgICAgICAgIHRoaXMuX2RhdGVSYW5nZVZhbHVlID0gc2V0dGluZ3MuZGVmYXVsdFZhbHVlO1xyXG4gICAgICAgICAgICB0aGlzLmFkZENvbnRyb2xzKHRoaXMuX2RhdGVSYW5nZVZhbHVlKTtcclxuICAgICAgICAgICAgdGhpcy5zZXRFdmVudHMoKTtcclxuICAgICAgICAgICAgdGhpcy5zeW5jKHNldHRpbmdzLmRlZmF1bHRWYWx1ZSk7XHJcbiAgICAgICAgICAgIHRoaXMuZGlzcGxheURlYnVnKCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAvLyBwdWJsaWMgcHJvcGVydGllc1xyXG4gICAgICAgIHB1YmxpYyBnZXRTdGFydERhdGUoKSB7IHJldHVybiB0aGlzLl9zdGFydERhdGU7IH1cclxuICAgICAgICBwdWJsaWMgZ2V0RW5kRGF0ZSgpIHsgcmV0dXJuIHRoaXMuX2VuZERhdGU7IH1cclxuICAgICAgICBwdWJsaWMgZ2V0RGF0ZVJhbmdlVmFsdWUoKSB7IHJldHVybiB0aGlzLl9kYXRlUmFuZ2VWYWx1ZTsgfVxyXG4gICAgICAgIC8vIHNldHMgdmFsdWUgb2Ygc2VsZWN0LCBzeW5jcyBkYXRlIHBpY2tlcnNcclxuICAgICAgICBwdWJsaWMgc2V0RGF0ZVJhbmdlVmFsdWUodmFsdWU6IERhdGVSYW5nZVZhbHVlKSB7XHJcbiAgICAgICAgICAgIHRoaXMuX2RhdGVSYW5nZVZhbHVlID0gdmFsdWU7XHJcbiAgICAgICAgICAgIHZhciAkc2VsZWN0ID0gdGhpcy4kZ2V0RGF0ZVJhbmdlU2VsZWN0KCk7XHJcbiAgICAgICAgICAgICRzZWxlY3QudmFsKERhdGVSYW5nZVZhbHVlW3ZhbHVlXSk7XHJcbiAgICAgICAgICAgIHRoaXMuc3luYyh0aGlzLl9kYXRlUmFuZ2VWYWx1ZSk7XHJcbiAgICAgICAgICAgIHRoaXMucnVuQ2FsbEJhY2tzKHRoaXMuX3NldHRpbmdzLmNhbGxCYWNrcyk7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIHB1YmxpYyByZWZyZXNoKCkge1xyXG4gICAgICAgICAgICB0aGlzLnJ1bkNhbGxCYWNrcyh0aGlzLl9zZXR0aW5ncy5jYWxsQmFja3MpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8gYWRkcyByYXcgY29udHJvbHMgdG8gRE9NXHJcbiAgICAgICAgcHJpdmF0ZSBhZGRDb250cm9scyh2YWx1ZTogRGF0ZVJhbmdlVmFsdWUpIHtcclxuICAgICAgICAgICAgLy9zdGFydC5tYXgoZW5kLnZhbHVlKCkpO1xyXG4gICAgICAgICAgICAvL2VuZC5taW4oc3RhcnQudmFsdWUoKSk7XHJcbiAgICAgICAgICAgIHZhciByb290ID0gJCgnIycgKyB0aGlzLl9lbGVtZW50SWQpO1xyXG4gICAgICAgICAgICAvLyBhcHBlbmQgZGF0ZSBwaWNrZXJzXHJcbiAgICAgICAgICAgIHZhciAkZGF0ZVBpY2tlclBhbmVsID0gJCgnPHNwYW4gaWQ9XCInICsgdGhpcy5fZWxlbWVudElkICsgJ19kYXRlUGlja2VyUGFuZWxcIj48L3NwYW4+Jyk7XHJcbiAgICAgICAgICAgICRkYXRlUGlja2VyUGFuZWwuaGlkZSgpO1xyXG4gICAgICAgICAgICB2YXIgJHN0YXJ0RGF0ZVBpY2tlciA9ICQoJzxpbnB1dCBpZD1cIicgKyB0aGlzLl9lbGVtZW50SWQgKyAnX3N0YXJ0RGF0ZScgKyAnXCIvPicpO1xyXG4gICAgICAgICAgICAkZGF0ZVBpY2tlclBhbmVsLmFwcGVuZCgkc3RhcnREYXRlUGlja2VyKS5hcHBlbmQoJzxsYWJlbCBzdHlsZT1cInBhZGRpbmc6IDAgNXB4IDAgNXB4O1wiPnRvPC9sYWJlbD4nKTtcclxuICAgICAgICAgICAgdmFyIGVuZERhdGVQaWNrZXIgPSAkKCc8aW5wdXQgaWQ9XCInICsgdGhpcy5fZWxlbWVudElkICsgJ19lbmREYXRlJyArICdcIi8+Jyk7XHJcbiAgICAgICAgICAgICRkYXRlUGlja2VyUGFuZWwuYXBwZW5kKGVuZERhdGVQaWNrZXIpO1xyXG4gICAgICAgICAgICAkc3RhcnREYXRlUGlja2VyLmtlbmRvRGF0ZVBpY2tlcih7IGZvcm1hdDogXCJ5eXl5LU1NLWRkXCIsIHZhbHVlOiB0aGlzLl9zdGFydERhdGUgfSk7XHJcbiAgICAgICAgICAgIGVuZERhdGVQaWNrZXIua2VuZG9EYXRlUGlja2VyKHsgZm9ybWF0OiBcInl5eXktTU0tZGRcIiwgdmFsdWU6IHRoaXMuX2VuZERhdGUgfSk7XHJcbiAgICAgICAgICAgIHJvb3QuYXBwZW5kKCRkYXRlUGlja2VyUGFuZWwpO1xyXG4gICAgICAgICAgICAvLyBhcHBlbmQgc2VsZWN0XHJcbiAgICAgICAgICAgIHZhciAkc2VsZWN0ID0gJCgnPHNlbGVjdCBpZD1cIicgKyB0aGlzLl9lbGVtZW50SWQgKyAnX2RhdGVSYW5nZScgKyAnXCIgY2xhc3M9XCJmb3JtLWNvbnRyb2xcIiBzdHlsZT1cIndpZHRoOiAxNTBweDtkaXNwbGF5OiBpbmxpbmU7bWFyZ2luLWxlZnQ6IDVweDtcIj48L3NlbGVjdD4nKTtcclxuICAgICAgICAgICAgJC5lYWNoKHRoaXMuX3NldHRpbmdzLmRhdGVSYW5nZU9wdGlvbnMsIChpLCB2KSA9PiB7ICRzZWxlY3QuYXBwZW5kKCQoXCI8b3B0aW9uIFwiICsgKHZhbHVlID09PSB2ID8gXCJzZWxlY3RlZFwiIDogXCJcIikgKyBcIi8+XCIpLnZhbChEYXRlUmFuZ2VWYWx1ZVt2XSkudGV4dChEYXRlUmFuZ2VWYWx1ZURlc2NyaXB0aW9uLmdldERlY3JpcHRpb24odikpKTsgfSk7XHJcbiAgICAgICAgICAgIHJvb3QuYXBwZW5kKCRzZWxlY3QpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8gc2V0IGV2ZW50cyBcclxuICAgICAgICBwcml2YXRlIHNldEV2ZW50cygpIHtcclxuICAgICAgICAgICAgdmFyICRyb290ID0gJCgnIycgKyB0aGlzLl9lbGVtZW50SWQpO1xyXG4gICAgICAgICAgICB2YXIgc2VsZWN0ID0gJHJvb3QuZmluZCgnc2VsZWN0Jyk7XHJcbiAgICAgICAgICAgIHNlbGVjdC5jaGFuZ2UoKCkgPT4ge1xyXG4gICAgICAgICAgICAgICAgY29uc29sZS5sb2coJ2RhdGVSYW5nZSBjaGFuZ2UgZmlyaW5nJyk7XHJcbiAgICAgICAgICAgICAgICB2YXIgdjogc3RyaW5nID0gJCgnIycgKyB0aGlzLl9lbGVtZW50SWQgKyAnX2RhdGVSYW5nZScpLnZhbCgpO1xyXG4gICAgICAgICAgICAgICAgdGhpcy5fZGF0ZVJhbmdlVmFsdWUgPSBEYXRlUmFuZ2VWYWx1ZVt2XTtcclxuICAgICAgICAgICAgICAgIHRoaXMuc3luYyh0aGlzLl9kYXRlUmFuZ2VWYWx1ZSk7XHJcbiAgICAgICAgICAgICAgICB0aGlzLnJ1bkNhbGxCYWNrcyh0aGlzLl9zZXR0aW5ncy5jYWxsQmFja3MpO1xyXG4gICAgICAgICAgICAgICAgdGhpcy5kaXNwbGF5RGVidWcoKTtcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgIHZhciBzdGFydCA9IHRoaXMuZ2V0U3RhcnRQaWNrZXIoKTtcclxuICAgICAgICAgICAgc3RhcnQuYmluZChcImNoYW5nZVwiLCAoKSA9PiB7XHJcbiAgICAgICAgICAgICAgICBjb25zb2xlLmxvZygnc3RhcnRkYXRlIGNoYW5nZSBmaXJpbmcnKTtcclxuICAgICAgICAgICAgICAgIHZhciBzdGFydFBpY2tlciA9IHRoaXMuZ2V0U3RhcnRQaWNrZXIoKTtcclxuICAgICAgICAgICAgICAgIHZhciBlbmRQaWNrZXIgPSB0aGlzLmdldEVuZFBpY2tlcigpO1xyXG4gICAgICAgICAgICAgICAgaWYgKHN0YXJ0UGlja2VyLnZhbHVlKCkgPj0gZW5kUGlja2VyLnZhbHVlKCkpIHtcclxuICAgICAgICAgICAgICAgICAgICB0aGlzLm1lc3NhZ2VTaG93KCdTdGFydCBkYXRlIG11c3QgYmUgYmVmb3JlIGVuZCBkYXRlLicpO1xyXG4gICAgICAgICAgICAgICAgICAgIHN0YXJ0UGlja2VyLnZhbHVlKG1vbWVudChlbmRQaWNrZXIudmFsdWUoKSkuYWRkKC0xLCAnZGF5JykuZm9ybWF0KCdZWVlZLU1NLUREJykpO1xyXG4gICAgICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICAgICB0aGlzLl9zdGFydERhdGUgPSBzdGFydFBpY2tlci52YWx1ZSgpO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgdGhpcy5ydW5DYWxsQmFja3ModGhpcy5fc2V0dGluZ3MuY2FsbEJhY2tzKTtcclxuICAgICAgICAgICAgICAgIHRoaXMuZGlzcGxheURlYnVnKCk7XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICB2YXIgZW5kID0gdGhpcy5nZXRFbmRQaWNrZXIoKTtcclxuICAgICAgICAgICAgZW5kLmJpbmQoXCJjaGFuZ2VcIiwgKCkgPT4ge1xyXG4gICAgICAgICAgICAgICAgY29uc29sZS5sb2coJ2VuZGRhdGUgY2hhbmdlIGZpcmluZycpO1xyXG4gICAgICAgICAgICAgICAgdmFyIHN0YXJ0UGlja2VyID0gdGhpcy5nZXRTdGFydFBpY2tlcigpO1xyXG4gICAgICAgICAgICAgICAgdmFyIGVuZFBpY2tlciA9IHRoaXMuZ2V0RW5kUGlja2VyKCk7XHJcbiAgICAgICAgICAgICAgICBpZiAoc3RhcnRQaWNrZXIudmFsdWUoKSA+PSBlbmRQaWNrZXIudmFsdWUoKSkge1xyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMubWVzc2FnZVNob3coJ0VuZCBkYXRlIG11c3QgYmUgYWZ0ZXIgc3RhcnQgZGF0ZS4nKTtcclxuICAgICAgICAgICAgICAgICAgICBlbmRQaWNrZXIudmFsdWUobW9tZW50KHN0YXJ0UGlja2VyLnZhbHVlKCkpLmFkZCgxLCAnZGF5JykuZm9ybWF0KCdZWVlZLU1NLUREJykpO1xyXG4gICAgICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICAgICB0aGlzLl9lbmREYXRlID0gZW5kUGlja2VyLnZhbHVlKCk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB0aGlzLnJ1bkNhbGxCYWNrcyh0aGlzLl9zZXR0aW5ncy5jYWxsQmFja3MpO1xyXG4gICAgICAgICAgICAgICAgdGhpcy5kaXNwbGF5RGVidWcoKTtcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAvLyBydW5zIGNhbGxiYWNrcyBhZnRlciBldmVudHMgYXJlIGZpcmVkXHJcbiAgICAgICAgcHJpdmF0ZSBydW5DYWxsQmFja3MoY2FsbGJhY2tzOiBBcnJheTxhbnk+KSB7XHJcbiAgICAgICAgICAgICQuZWFjaChjYWxsYmFja3MsIChpLCBvKSA9PiB7IG8uY2FsbCgpOyB9KTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIC8vIHN5bmMncyBkYXRlIHBpY2tlcnMgYW5kIGRhdGUgcmFuZ2Ugc2VsZWN0XHJcbiAgICAgICAgcHJpdmF0ZSBzeW5jKGRhdGVSYW5nZVZhbHVlKSB7XHJcbiAgICAgICAgICAgIHZhciBkYXRlUmFuZ2VSZXN1bHQgPSB0aGlzLmdldFN0YXJ0RW5kRm9yUmFuZ2UoZGF0ZVJhbmdlVmFsdWUpO1xyXG4gICAgICAgICAgICB0aGlzLl9zdGFydERhdGUgPSBkYXRlUmFuZ2VSZXN1bHQuc3RhcnREYXRlO1xyXG4gICAgICAgICAgICB0aGlzLl9lbmREYXRlID0gZGF0ZVJhbmdlUmVzdWx0LmVuZERhdGU7XHJcbiAgICAgICAgICAgIHZhciBzdGFydFBpY2tlciA9IHRoaXMuZ2V0U3RhcnRQaWNrZXIoKTtcclxuICAgICAgICAgICAgc3RhcnRQaWNrZXIudmFsdWUoZGF0ZVJhbmdlUmVzdWx0LnN0YXJ0RGF0ZSk7XHJcbiAgICAgICAgICAgIHZhciBlbmRQaWNrZXIgPSB0aGlzLmdldEVuZFBpY2tlcigpO1xyXG4gICAgICAgICAgICBlbmRQaWNrZXIudmFsdWUoZGF0ZVJhbmdlUmVzdWx0LmVuZERhdGUpO1xyXG4gICAgICAgICAgICBpZiAodGhpcy5fZGF0ZVJhbmdlVmFsdWUgPT09IERhdGVSYW5nZVZhbHVlLkN1c3RvbSkge1xyXG4gICAgICAgICAgICAgICAgJCgnIycgKyB0aGlzLl9lbGVtZW50SWQgKyAnX2RhdGVQaWNrZXJQYW5lbCcpLnNob3coKTtcclxuICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICQoJyMnICsgdGhpcy5fZWxlbWVudElkICsgJ19kYXRlUGlja2VyUGFuZWwnKS5oaWRlKCk7XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAvLyBoZWxwZXJzIFxyXG4gICAgICAgIHByaXZhdGUgbWVzc2FnZVNob3cobWVzc2FnZSkge1xyXG4gICAgICAgICAgICAkKCcjbWVzc2FnZScpLmFkZENsYXNzKCdhbGVydCBhbGVydC1kYW5nZXInKS5zaG93KCkudGV4dChtZXNzYWdlKTtcclxuICAgICAgICAgICAgd2luZG93LnNldFRpbWVvdXQoKCkgPT4geyAkKCcjbWVzc2FnZScpLmhpZGUoKTsgfSwgMTAwMDApO1xyXG4gICAgICAgIH1cclxuICAgICAgICBwcml2YXRlIG1lc3NhZ2VIaWRlKCkge1xyXG4gICAgICAgICAgICAkKCcjbWVzc2FnZScpLmhpZGUoKTtcclxuICAgICAgICB9XHJcbiAgICAgICAgcHJpdmF0ZSBnZXRTdGFydFBpY2tlcigpIHtcclxuICAgICAgICAgICAgcmV0dXJuICQoJyMnICsgdGhpcy5fZWxlbWVudElkICsgJ19zdGFydERhdGUnKS5kYXRhKCdrZW5kb0RhdGVQaWNrZXInKTtcclxuICAgICAgICB9XHJcbiAgICAgICAgcHJpdmF0ZSBnZXRFbmRQaWNrZXIoKSB7XHJcbiAgICAgICAgICAgIHJldHVybiAkKCcjJyArIHRoaXMuX2VsZW1lbnRJZCArICdfZW5kRGF0ZScpLmRhdGEoJ2tlbmRvRGF0ZVBpY2tlcicpO1xyXG4gICAgICAgIH1cclxuICAgICAgICBwcml2YXRlICRnZXREYXRlUmFuZ2VTZWxlY3QoKSB7XHJcbiAgICAgICAgICAgIHJldHVybiAkKCcjJyArIHRoaXMuX2VsZW1lbnRJZCArICdfZGF0ZVJhbmdlJyk7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIHByaXZhdGUgZGlzcGxheURlYnVnKCkge1xyXG4gICAgICAgICAgICBjb25zb2xlLmxvZyhcInN0YXJ0ZGF0ZT1cIiArIHRoaXMuX3N0YXJ0RGF0ZSk7XHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKFwiZW5kZGF0ZT1cIiArIHRoaXMuX2VuZERhdGUpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8gcmV0dXJucyBzdGFydCBhbmQgZW5kIGRhdGUgaW4gbG9jYWwgZm9yIGEgZ2l2ZW4gRGF0ZVJhbmdlVmFsdWVcclxuICAgICAgICBwcml2YXRlIGdldFN0YXJ0RW5kRm9yUmFuZ2UoZGF0ZVJhbmdlVmFsdWU6IERhdGVSYW5nZVZhbHVlKSB7XHJcbiAgICAgICAgICAgIHN3aXRjaCAoZGF0ZVJhbmdlVmFsdWUpIHtcclxuICAgICAgICAgICAgICAgIGNhc2UgRGF0ZVJhbmdlVmFsdWUuTGFzdDI0SG91cnM6XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIG5ldyBEYXRlUmFuZ2VSZXN1bHQoXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIG1vbWVudCgpLnN1YnRyYWN0KDI0LCAnaG91cnMnKS5sb2NhbCgpLnRvRGF0ZSgpLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoKS5sb2NhbCgpLnRvRGF0ZSgpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICk7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLlRvZGF5OlxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiBuZXcgRGF0ZVJhbmdlUmVzdWx0KFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoKS5sb2NhbCgpLnN0YXJ0T2YoJ2RheScpLnRvRGF0ZSgpLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoKS5sb2NhbCgpLnRvRGF0ZSgpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICk7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLlllc3RlcmRheTpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gbmV3IERhdGVSYW5nZVJlc3VsdChcclxuICAgICAgICAgICAgICAgICAgICAgICAgbW9tZW50KCkubG9jYWwoKS5zdWJ0cmFjdCgxLCAnZGF5cycpLnN0YXJ0T2YoJ2RheScpLnRvRGF0ZSgpLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoKS5sb2NhbCgpLnN0YXJ0T2YoJ2RheScpLnRvRGF0ZSgpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICk7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLkxhc3Q3RGF5czpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gbmV3IERhdGVSYW5nZVJlc3VsdChcclxuICAgICAgICAgICAgICAgICAgICAgICAgbW9tZW50KCkubG9jYWwoKS5zdWJ0cmFjdCg3LCAnZGF5cycpLnRvRGF0ZSgpLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoKS5sb2NhbCgpLnRvRGF0ZSgpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICk7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLkxhc3QzMERheXM6XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIG5ldyBEYXRlUmFuZ2VSZXN1bHQoXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIG1vbWVudCgpLmxvY2FsKCkuc3VidHJhY3QoMzAsICdkYXlzJykudG9EYXRlKCksXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIG1vbWVudCgpLmxvY2FsKCkudG9EYXRlKClcclxuICAgICAgICAgICAgICAgICAgICAgICAgKTtcclxuICAgICAgICAgICAgICAgIGNhc2UgRGF0ZVJhbmdlVmFsdWUuTGFzdDYwRGF5czpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gbmV3IERhdGVSYW5nZVJlc3VsdChcclxuICAgICAgICAgICAgICAgICAgICAgICAgbW9tZW50KCkubG9jYWwoKS5zdWJ0cmFjdCg2MCwgJ2RheXMnKS50b0RhdGUoKSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgbW9tZW50KCkubG9jYWwoKS50b0RhdGUoKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICApO1xyXG4gICAgICAgICAgICAgICAgY2FzZSBEYXRlUmFuZ2VWYWx1ZS5MYXN0OTBEYXlzOlxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiBuZXcgRGF0ZVJhbmdlUmVzdWx0KFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoKS5sb2NhbCgpLnN1YnRyYWN0KDkwLCAnZGF5cycpLnRvRGF0ZSgpLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoKS5sb2NhbCgpLnRvRGF0ZSgpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICk7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLlRoaXNNb250aDpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gbmV3IERhdGVSYW5nZVJlc3VsdChcclxuICAgICAgICAgICAgICAgICAgICAgICAgbW9tZW50KCkubG9jYWwoKS5zdGFydE9mKCdtb250aCcpLnRvRGF0ZSgpLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoKS5sb2NhbCgpLnRvRGF0ZSgpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICk7XHJcbiAgICAgICAgICAgICAgICBjYXNlIERhdGVSYW5nZVZhbHVlLkxhc3RNb250aDpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gbmV3IERhdGVSYW5nZVJlc3VsdChcclxuICAgICAgICAgICAgICAgICAgICAgICAgbW9tZW50KCkubG9jYWwoKS5zdWJ0cmFjdCgxLCAnbW9udGhzJykuc3RhcnRPZignbW9udGgnKS50b0RhdGUoKSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgbW9tZW50KCkubG9jYWwoKS5zdGFydE9mKCdtb250aCcpLnN1YnRyYWN0KCdkYXlzJywgMSkudG9EYXRlKClcclxuICAgICAgICAgICAgICAgICAgICAgICAgKTtcclxuICAgICAgICAgICAgICAgIGNhc2UgRGF0ZVJhbmdlVmFsdWUuQWxsOlxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiBuZXcgRGF0ZVJhbmdlUmVzdWx0KFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoKS5sb2NhbCgpLnN1YnRyYWN0KDEsICd5ZWFycycpLnN0YXJ0T2YoJ21vbnRoJykudG9EYXRlKCksXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIG1vbWVudCgpLmxvY2FsKCkudG9EYXRlKClcclxuICAgICAgICAgICAgICAgICAgICAgICAgKTtcclxuICAgICAgICAgICAgICAgIGNhc2UgRGF0ZVJhbmdlVmFsdWUuQ3VzdG9tOlxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiBuZXcgRGF0ZVJhbmdlUmVzdWx0KFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoKS5sb2NhbCgpLnN0YXJ0T2YoJ2RheScpLnRvRGF0ZSgpLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoKS5sb2NhbCgpLnRvRGF0ZSgpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgcmV0dXJuIG5ldyBEYXRlUmFuZ2VSZXN1bHQobnVsbCwgbnVsbCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgIH1cclxuXHJcbiAgICBjbGFzcyBEYXRlUmFuZ2VSZXN1bHQge1xyXG4gICAgICAgIHByaXZhdGUgX3N0YXJ0RGF0ZTogRGF0ZTtcclxuICAgICAgICBwcml2YXRlIF9lbmREYXRlOiBEYXRlO1xyXG4gICAgICAgIGNvbnN0cnVjdG9yKHN0YXJ0RGF0ZTogRGF0ZSwgZW5kRGF0ZTogRGF0ZSkge1xyXG4gICAgICAgICAgICB0aGlzLl9zdGFydERhdGUgPSBzdGFydERhdGU7XHJcbiAgICAgICAgICAgIHRoaXMuX2VuZERhdGUgPSBlbmREYXRlO1xyXG4gICAgICAgIH1cclxuICAgICAgICBwdWJsaWMgZ2V0IHN0YXJ0RGF0ZSgpOiBEYXRlIHtcclxuICAgICAgICAgICAgcmV0dXJuIHRoaXMuX3N0YXJ0RGF0ZTtcclxuICAgICAgICB9XHJcbiAgICAgICAgcHVibGljIGdldCBlbmREYXRlKCk6IERhdGUge1xyXG4gICAgICAgICAgICByZXR1cm4gdGhpcy5fZW5kRGF0ZTtcclxuICAgICAgICB9XHJcblxyXG4gICAgfVxyXG5cclxuICAgIGV4cG9ydCBjbGFzcyBBcHBsaWNhdGlvbklkIHtcclxuICAgICAgICBwdWJsaWMgc3RhdGljIGxvYWQoKSB7XHJcbiAgICAgICAgICAgIGlmICgkKFwiI0FwcGxpY2F0aW9uSWRcIikgIT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgdmFyIHYgPSAkLmNvb2tpZSgnQXBwbGljYXRpb25JZCcpO1xyXG4gICAgICAgICAgICAgICAgaWYgKHYgIT0gJycpIHtcclxuICAgICAgICAgICAgICAgICAgICAkKCcjQXBwbGljYXRpb25JZCBvcHRpb25bdmFsdWU9JyArICQuY29va2llKCdBcHBsaWNhdGlvbklkJykgKyAnXScpLmF0dHIoJ3NlbGVjdGVkJywgJ3NlbGVjdGVkJyk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9XHJcbiAgICAgICAgcHVibGljIHN0YXRpYyBzZXQoKSB7XHJcbiAgICAgICAgICAgIGlmICgkKFwiI0FwcGxpY2F0aW9uSWRcIikgIT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgJC5jb29raWUoJ0FwcGxpY2F0aW9uSWQnLCAkKCcjQXBwbGljYXRpb25JZCBvcHRpb246c2VsZWN0ZWQnKS52YWwoKSk7ICAgIFxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxufVxyXG4iXX0=