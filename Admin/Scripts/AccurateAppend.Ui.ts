
module AccurateAppend.Ui {

    export enum DateRangeValue {
        Last24Hours,
        Today,
        Yesterday,
        Last7Days,
        ThisMonth,
        LastMonth,
        Last30Days,
        Last60Days,
        Last90Days,
        CurrentMonth,
        PreviousToLastMonth,
        All,
        Custom
    }

    class DateRangeValueDescription {
        public static getDecription(dateRangeValue: DateRangeValue) {
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
        }
    }

    export class DateRangeWidgetSettings {
        constructor(
            // options to be included in select
            public dateRangeOptions: Array<DateRangeValue>,
            // default value to be chosen in select
            public defaultValue: DateRangeValue,
            // callbacks to be executed when change is raised
            public callBacks: Array<any>
            ) { }
    }

    // renders control that renders date ragne DD along with date pickers for custom date range
    export class DateRangeWidget {

        private _startDate: Date;
        private _endDate: Date;
        private _dateRangeValue: DateRangeValue;
        private _elementId: string;
        private _settings: any;

        constructor(elementId: string, settings: DateRangeWidgetSettings) {
            this._settings = settings;
            this._elementId = elementId;
            this._dateRangeValue = settings.defaultValue;
            this.addControls(this._dateRangeValue);
            this.setEvents();
            this.sync(settings.defaultValue);
            this.displayDebug();
        }

        // public properties
        public getStartDate() { return this._startDate; }
        public getEndDate() { return this._endDate; }
        public getDateRangeValue() { return this._dateRangeValue; }
        // sets value of select, syncs date pickers
        public setDateRangeValue(value: DateRangeValue) {
            this._dateRangeValue = value;
            var $select = this.$getDateRangeSelect();
            $select.val(DateRangeValue[value]);
            this.sync(this._dateRangeValue);
            this.runCallBacks(this._settings.callBacks);
        }
        public refresh() {
            this.runCallBacks(this._settings.callBacks);
        }

        // adds raw controls to DOM
        private addControls(value: DateRangeValue) {
            //start.max(end.value());
            //end.min(start.value());
            var root = $('#' + this._elementId);
            // append date pickers
            var $datePickerPanel = $('<span id="' + this._elementId + '_datePickerPanel"></span>');
            $datePickerPanel.hide();
            var $startDatePicker = $('<input id="' + this._elementId + '_startDate' + '"/>');
            $datePickerPanel.append($startDatePicker).append('<label style="padding: 0 5px 0 5px;">to</label>');
            var endDatePicker = $('<input id="' + this._elementId + '_endDate' + '"/>');
            $datePickerPanel.append(endDatePicker);
            $startDatePicker.kendoDatePicker({ format: "yyyy-MM-dd", value: this._startDate });
            endDatePicker.kendoDatePicker({ format: "yyyy-MM-dd", value: this._endDate });
            root.append($datePickerPanel);
            // append select
            var $select = $('<select id="' + this._elementId + '_dateRange' + '" class="form-control" style="width: 150px;display: inline;margin-left: 5px;"></select>');
            $.each(this._settings.dateRangeOptions, (i, v) => { $select.append($("<option " + (value === v ? "selected" : "") + "/>").val(DateRangeValue[v]).text(DateRangeValueDescription.getDecription(v))); });
            root.append($select);
        }

        // set events 
        private setEvents() {
            var $root = $('#' + this._elementId);
            var select = $root.find('select');
            select.change(() => {
                console.log('dateRange change firing');
                var v: string = $('#' + this._elementId + '_dateRange').val();
                this._dateRangeValue = DateRangeValue[v];
                this.sync(this._dateRangeValue);
                this.runCallBacks(this._settings.callBacks);
                this.displayDebug();
            });
            var start = this.getStartPicker();
            start.bind("change", () => {
                console.log('startdate change firing');
                var startPicker = this.getStartPicker();
                var endPicker = this.getEndPicker();
                if (startPicker.value() >= endPicker.value()) {
                    this.messageShow('Start date must be before end date.');
                    startPicker.value(moment(endPicker.value()).add(-1, 'day').format('YYYY-MM-DD'));
                } else {
                    this._startDate = startPicker.value();
                }
                this.runCallBacks(this._settings.callBacks);
                this.displayDebug();
            });
            var end = this.getEndPicker();
            end.bind("change", () => {
                console.log('enddate change firing');
                var startPicker = this.getStartPicker();
                var endPicker = this.getEndPicker();
                if (startPicker.value() >= endPicker.value()) {
                    this.messageShow('End date must be after start date.');
                    endPicker.value(moment(startPicker.value()).add(1, 'day').format('YYYY-MM-DD'));
                } else {
                    this._endDate = endPicker.value();
                }
                this.runCallBacks(this._settings.callBacks);
                this.displayDebug();
            });
        }

        // runs callbacks after events are fired
        private runCallBacks(callbacks: Array<any>) {
            $.each(callbacks, (i, o) => { o.call(); });
        }

        // sync's date pickers and date range select
        private sync(dateRangeValue) {
            var dateRangeResult = this.getStartEndForRange(dateRangeValue);
            this._startDate = dateRangeResult.startDate;
            this._endDate = dateRangeResult.endDate;
            var startPicker = this.getStartPicker();
            startPicker.value(dateRangeResult.startDate);
            var endPicker = this.getEndPicker();
            endPicker.value(dateRangeResult.endDate);
            if (this._dateRangeValue === DateRangeValue.Custom) {
                $('#' + this._elementId + '_datePickerPanel').show();
            } else {
                $('#' + this._elementId + '_datePickerPanel').hide();
            }

        }

        // helpers 
        private messageShow(message) {
            $('#message').addClass('alert alert-danger').show().text(message);
            window.setTimeout(() => { $('#message').hide(); }, 10000);
        }
        private messageHide() {
            $('#message').hide();
        }
        private getStartPicker() {
            return $('#' + this._elementId + '_startDate').data('kendoDatePicker');
        }
        private getEndPicker() {
            return $('#' + this._elementId + '_endDate').data('kendoDatePicker');
        }
        private $getDateRangeSelect() {
            return $('#' + this._elementId + '_dateRange');
        }
        private displayDebug() {
            console.log("startdate=" + this._startDate);
            console.log("enddate=" + this._endDate);
        }

        // returns start and end date in local for a given DateRangeValue
        private getStartEndForRange(dateRangeValue: DateRangeValue) {
            switch (dateRangeValue) {
                case DateRangeValue.Last24Hours:
                    return new DateRangeResult(
                        moment().subtract(24, 'hours').local().toDate(),
                        moment().local().toDate()
                        );
                case DateRangeValue.Today:
                    return new DateRangeResult(
                        moment().local().startOf('day').toDate(),
                        moment().local().toDate()
                        );
                case DateRangeValue.Yesterday:
                    return new DateRangeResult(
                        moment().local().subtract(1, 'days').startOf('day').toDate(),
                        moment().local().startOf('day').toDate()
                        );
                case DateRangeValue.Last7Days:
                    return new DateRangeResult(
                        moment().local().subtract(7, 'days').toDate(),
                        moment().local().toDate()
                        );
                case DateRangeValue.Last30Days:
                    return new DateRangeResult(
                        moment().local().subtract(30, 'days').toDate(),
                        moment().local().toDate()
                        );
                case DateRangeValue.Last60Days:
                    return new DateRangeResult(
                        moment().local().subtract(60, 'days').toDate(),
                        moment().local().toDate()
                        );
                case DateRangeValue.Last90Days:
                    return new DateRangeResult(
                        moment().local().subtract(90, 'days').toDate(),
                        moment().local().toDate()
                        );
                case DateRangeValue.ThisMonth:
                    return new DateRangeResult(
                        moment().local().startOf('month').toDate(),
                        moment().local().toDate()
                        );
                case DateRangeValue.LastMonth:
                    return new DateRangeResult(
                        moment().local().subtract(1, 'months').startOf('month').toDate(),
                        moment().local().startOf('month').subtract('days', 1).toDate()
                        );
                case DateRangeValue.All:
                    return new DateRangeResult(
                        moment().local().subtract(1, 'years').startOf('month').toDate(),
                        moment().local().toDate()
                        );
                case DateRangeValue.Custom:
                    return new DateRangeResult(
                        moment().local().startOf('day').toDate(),
                        moment().local().toDate()
                        );
            }
            return new DateRangeResult(null, null);
        }

    }

    class DateRangeResult {
        private _startDate: Date;
        private _endDate: Date;
        constructor(startDate: Date, endDate: Date) {
            this._startDate = startDate;
            this._endDate = endDate;
        }
        public get startDate(): Date {
            return this._startDate;
        }
        public get endDate(): Date {
            return this._endDate;
        }

    }

    export class ApplicationId {
        public static load() {
            if ($("#ApplicationId") != null) {
                var v = $.cookie('ApplicationId');
                if (v != '') {
                    $('#ApplicationId option[value=' + $.cookie('ApplicationId') + ']').attr('selected', 'selected');
                }
            }
        }
        public static set() {
            if ($("#ApplicationId") != null) {
                $.cookie('ApplicationId', $('#ApplicationId option:selected').val());    
            }
        }
    }

}
