var AccurateAppend;
(function (AccurateAppend) {
    var DynamicAppend;
    (function (DynamicAppend) {
        var Manifest = (function () {
            function Manifest() {
                this.operations = ko.observableArray();
                this.supressionid = ko.observable("");
            }
            Object.defineProperty(Manifest.prototype, "inputfieldnames", {
                get: function () {
                    var fields = new Array();
                    $.each(this.operations(), function (i, operation) {
                        $.each(_.filter(operation.inputfields, function (o) { return (o.metafieldName.indexOf("_") === -1); }), function (h, field) {
                            fields.push(field.metafieldName);
                        });
                    });
                    return _.uniq(fields);
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(Manifest.prototype, "outputfields", {
                get: function () {
                    var fields = new Array();
                    $.each(this.operations(), function (i, operation) {
                        $.each(_.filter(operation.outputfields, function (o) { return (o.include()); }), function (h, field) {
                            fields.push(field);
                        });
                    });
                    return fields;
                },
                enumerable: true,
                configurable: true
            });
            Manifest.prototype.add = function (name, successCallback, errorCallback) {
                var _this = this;
                var self = this;
                $.ajax({
                    type: "GET",
                    url: "/Batch/GetOperationDefintion",
                    data: { name: name },
                    success: function (data) {
                        var operation = new Operation(data);
                        var e;
                        switch (operation.name) {
                            case DynamicAppend.OperationName.DEDUPE_PHONE:
                                try {
                                    _this.map(new DynamicAppend.DedupePhoneStrategy(), operation, _this.operations());
                                    _this.operations.push(operation);
                                    successCallback();
                                }
                                catch (e) {
                                    errorCallback(e);
                                }
                                break;
                            case DynamicAppend.OperationName.SET_PREF_PHONE:
                                try {
                                    _this.map(new DynamicAppend.SetPrefPhoneStrategy(), operation, _this.operations());
                                    _this.operations.push(operation);
                                    successCallback();
                                }
                                catch (e) {
                                    errorCallback(e);
                                }
                                break;
                            case DynamicAppend.OperationName.SET_PREF_PHONE_SINGLE_COLUMN:
                                try {
                                    _this.map(new DynamicAppend.SetPrefPhoneSingleColumnStrategy(), operation, _this.operations());
                                    _this.operations.push(operation);
                                    successCallback();
                                }
                                catch (e) {
                                    errorCallback(e);
                                }
                                break;
                            case DynamicAppend.OperationName.SET_PREF_ADDRESS_SINGLE_COLUMN:
                                try {
                                    _this.map(new DynamicAppend.SetPrefAddressSingleColumnStrategy(), operation, _this.operations());
                                    _this.operations.push(operation);
                                    successCallback();
                                }
                                catch (e) {
                                    errorCallback(e);
                                }
                                break;
                            case DynamicAppend.OperationName.SET_PREF_PHONE_COMPARE_INPUT:
                                try {
                                    _this.map(new DynamicAppend.SetPrefPhoneCompareInputStrategy(), operation, _this.operations());
                                    _this.operations.push(operation);
                                    successCallback();
                                }
                                catch (e) {
                                    errorCallback(e);
                                }
                                break;
                            case DynamicAppend.OperationName.SET_PREF_BASED_ON_VERIFICATION:
                                try {
                                    _this.map(new DynamicAppend.SetPrefBasedOnVerificationStrategy(), operation, _this.operations());
                                    _this.operations.push(operation);
                                    successCallback();
                                }
                                catch (e) {
                                    errorCallback(e);
                                }
                                break;
                            case DynamicAppend.OperationName.SET_PREF_EMAIL_VER:
                                try {
                                    _this.map(new DynamicAppend.SetPrefEmailVerStrategy(), operation, _this.operations());
                                    _this.operations.push(operation);
                                    successCallback();
                                }
                                catch (e) {
                                    errorCallback(e);
                                }
                                break;
                            case DynamicAppend.OperationName.NCOA48:
                                _this.map(new DynamicAppend.Ncoa48Strategy(), operation, _this.operations());
                                _this.operations.unshift(operation);
                                break;
                            default:
                                if (_.contains(_.map(_this.operations(), function (v) { return v.name; }), DynamicAppend.OperationName.NCOA48)) {
                                    console.log("NCOA detected");
                                    _this.map(new DynamicAppend.Ncoa48StrategyRemove(), operation, _this.operations());
                                    _this.operations.push(operation);
                                    _this.map(new DynamicAppend.Ncoa48Strategy(), operation, _this.operations());
                                }
                                else {
                                    _this.operations.push(operation);
                                }
                                break;
                        }
                    },
                    error: function (xhr, status, error) {
                        alert("Unable to get defintion for " + name);
                    }
                });
            };
            Manifest.prototype.remove = function (name) {
                console.log("Removing " + name);
                var operation = _.first(_.filter(this.operations(), function (o) { return (o.name === DynamicAppend.OperationName[name]); }));
                if (!operation)
                    return;
                switch (operation.name) {
                    case DynamicAppend.OperationName.SET_PREF_PHONE_SINGLE_COLUMN:
                        this.map(new DynamicAppend.SetPrefPhoneSingleColumnStrategyRemove(), operation, this.operations());
                        break;
                    case DynamicAppend.OperationName.SET_PREF_EMAIL_VER:
                        this.map(new DynamicAppend.SetPrefEmailVerStrategyRemove(), operation, this.operations());
                        break;
                    case DynamicAppend.OperationName.NCOA48:
                        this.map(new DynamicAppend.Ncoa48StrategyRemove(), operation, this.operations());
                        break;
                }
                this.operations.remove(operation);
            };
            Manifest.prototype.toJson = function () {
                $.each(this.operations(), function (i, o) {
                    o.sequnceid = i;
                    $.each(o.inputfields, function (j, f) {
                        f.sequnceid = o.sequnceid;
                    });
                    $.each(o.outputfields, function (j, f) {
                        f.sequnceid = o.sequnceid;
                    });
                });
                return ko.toJSON({
                    "Operations": ko.utils.arrayMap(this.operations(), function (o) {
                        return {
                            SequenceId: o.sequnceid,
                            Name: DynamicAppend.OperationName[o.name],
                            MatchLevels: ko.utils.arrayMap(o.matchlevels, function (m) {
                                return {
                                    Name: m.name,
                                    Include: m.include
                                };
                            }),
                            QualityLevels: ko.utils.arrayMap(o.qualitylevels, function (m) {
                                return {
                                    Name: m.name,
                                    Include: m.include
                                };
                            }),
                            Sources: ko.utils.arrayMap(o.sources, function (m) {
                                return {
                                    Name: m.name,
                                    Include: m.include
                                };
                            }),
                            InputFields: ko.utils.arrayMap(o.inputfields, function (f) {
                                return {
                                    MetaFieldName: f.metafieldName,
                                    OperationParamName: f.operationparamname,
                                    SequenceId: f.sequnceid,
                                    Include: f.include,
                                    Required: f.required
                                };
                            }),
                            OutputFields: ko.utils.arrayMap(o.outputfields, function (f) {
                                return {
                                    ColumnTitle: f.columntitle,
                                    MetaFieldName: f.metafieldName,
                                    OperationParamName: f.operationparamname,
                                    SequenceId: f.sequnceid,
                                    Include: f.include,
                                    Required: f.required
                                };
                            }),
                        };
                    }),
                    "OutputFields": ko.utils.arrayMap(this.outputfields, function (f) {
                        return {
                            xPath: f.xpath,
                            ColumnTitle: f.columntitle,
                            MetaFieldName: f.metafieldName,
                            OperationParamName: f.operationparamname,
                            SequenceId: f.sequnceid,
                            Include: f.include,
                            Required: f.required
                        };
                    })
                });
            };
            Manifest.prototype.map = function (strategy, target, operations) {
                return strategy.execute(target, operations);
            };
            Manifest.toggleOutputfields = function (include) {
                var operations = [];
                for (var _i = 1; _i < arguments.length; _i++) {
                    operations[_i - 1] = arguments[_i];
                }
                $.each(operations, function (i, operation) {
                    $.each(operation.outputfields, function (h, field) {
                        field.include(include);
                    });
                });
            };
            return Manifest;
        }());
        DynamicAppend.Manifest = Manifest;
        var Operation = (function () {
            function Operation(obj) {
                var self = this;
                self.title = obj.Title;
                self.description = obj.Description;
                self.name = DynamicAppend.OperationName[obj.Name];
                self.matchlevels = new Array();
                self.qualitylevels = new Array();
                self.inputfields = Array();
                self.outputfields = Array();
                self.sources = Array();
                $.each(obj.MatchLevels, function (i, matchlevel) { self.matchlevels.push(new MatchLevel(matchlevel)); });
                $.each(obj.QualityLevels, function (i, qualitylevel) { self.qualitylevels.push(new QualityLevel(qualitylevel)); });
                $.each(obj.InputFields, function (i, inputfield) { self.inputfields.push(new Field(inputfield, self.name, null)); });
                $.each(obj.Sources, function (i, source) { self.sources.push(new Source(source)); });
                var previous;
                $.each(obj.OutputFields, function (i, outputfield) {
                    var field;
                    switch (outputfield.MetaFieldName) {
                        case "MatchLevel":
                        case "QualityLevel":
                            field = new Field(outputfield, self.name, previous.metafieldName);
                            break;
                        default:
                            field = new Field(outputfield, self.name, Field.generateFieldId());
                            previous = (outputfield.name !== DynamicAppend.FieldName.MatchLevel && outputfield.name !== DynamicAppend.FieldName.QualityLevel) ? field : null;
                    }
                    self.outputfields.push(field);
                });
            }
            Operation.prototype.isMapped = function (operation) {
                return _.filter(Operation.getMetafieldnames(this.outputfields), function (i) { return ($.inArray(i, Operation.getMetafieldnames(operation.inputfields)) > -1); }).length > 0;
            };
            Operation.prototype.isUtil = function () {
                switch (this.name) {
                    case DynamicAppend.OperationName.DEDUPE_PHONE:
                    case DynamicAppend.OperationName.SET_PREF_PHONE:
                    case DynamicAppend.OperationName.SET_PREF_PHONE_SINGLE_COLUMN:
                    case DynamicAppend.OperationName.SET_PREF_ADDRESS_SINGLE_COLUMN:
                        return true;
                    default:
                        return false;
                }
            };
            Operation.mapField = function (from, fromOperationfieldName, to, tofieldName) {
                var fromField = _.find(from.outputfields, function (o) { return (o.name === fromOperationfieldName); });
                if (!fromField)
                    console.log("Unable to locate 'from:'" + DynamicAppend.FieldName[fromOperationfieldName] + " field in " + DynamicAppend.OperationName[from.name]);
                var toField = _.find(to.inputfields, function (o) { return (o.name === tofieldName); });
                if (!toField)
                    console.log("Unable to locate 'to':" + DynamicAppend.FieldName[fromOperationfieldName] + " field in " + DynamicAppend.OperationName[to.name]);
                toField.metafieldName = Field.generateFieldId();
                toField.metafieldName = fromField.metafieldName;
            };
            Operation.getMetafieldnames = function (fields) {
                return _.map(fields, function (field) { return field.metafieldName; });
            };
            return Operation;
        }());
        DynamicAppend.Operation = Operation;
        var Field = (function () {
            function Field(obj, operationName, fieldId) {
                var name = DynamicAppend.FieldName[obj.MetaFieldName];
                this.name = this.standardizeFieldName(name, operationName);
                this.operationparamname = obj.OperationParamName;
                this.columntitle = ko.observable(obj.ColumnTitle);
                this.required = obj.Required;
                this.include = ko.observable(obj.Include);
                this._metafieldname = ko.observable(fieldId || obj.MetaFieldName);
                this.color = ko.observable("black");
            }
            Object.defineProperty(Field.prototype, "metafieldName", {
                get: function () {
                    switch (this.name) {
                        case DynamicAppend.FieldName.MatchLevel:
                        case DynamicAppend.FieldName.QualityLevel:
                            return this._metafieldname() + "_" + DynamicAppend.FieldName[this.name];
                        default:
                            return this._metafieldname();
                    }
                },
                set: function (value) {
                    this._metafieldname(value);
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(Field.prototype, "xpath", {
                get: function () {
                    switch (this.name) {
                        case DynamicAppend.FieldName.MatchLevel:
                        case DynamicAppend.FieldName.QualityLevel:
                            return this._metafieldname() + "/@" + DynamicAppend.FieldName[this.name];
                        default:
                            return this._metafieldname();
                    }
                },
                enumerable: true,
                configurable: true
            });
            Field.prototype.standardizeFieldName = function (name, operationName) {
                switch (operationName) {
                    case DynamicAppend.OperationName.NCOA48:
                        switch (name) {
                            case DynamicAppend.FieldName.StandardizedAddress:
                                return DynamicAppend.FieldName.StreetAddress;
                            case DynamicAppend.FieldName.StandardizedCity:
                                return DynamicAppend.FieldName.City;
                            case DynamicAppend.FieldName.StandardizedState:
                                return DynamicAppend.FieldName.State;
                            case DynamicAppend.FieldName.StandardizedZip:
                                return DynamicAppend.FieldName.PostalCode;
                        }
                    default:
                        return name;
                }
            };
            Field.generateFieldId = function () {
                var d = new Date().getTime();
                var uuid = "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
                    var r = (d + Math.random() * 16) % 16 | 0;
                    d = Math.floor(d / 16);
                    return (c === "x" ? r : (r & 0x7 | 0x8)).toString(16);
                });
                return "_" + uuid;
            };
            return Field;
        }());
        DynamicAppend.Field = Field;
        var MatchLevel = (function () {
            function MatchLevel(obj) {
                this.name = obj.Name;
                this.include = ko.observable(obj.Include);
            }
            return MatchLevel;
        }());
        DynamicAppend.MatchLevel = MatchLevel;
        var QualityLevel = (function () {
            function QualityLevel(obj) {
                this.name = obj.Name;
                this.include = ko.observable(obj.Include);
            }
            return QualityLevel;
        }());
        DynamicAppend.QualityLevel = QualityLevel;
        var Source = (function () {
            function Source(obj) {
                this.name = obj.Name;
                this.include = ko.observable(obj.Include);
            }
            return Source;
        }());
        DynamicAppend.Source = Source;
        var FieldGroup = (function () {
            function FieldGroup() {
            }
            FieldGroup.getFields = function (name) {
                switch (name) {
                    case DynamicAppend.FieldGroupName.Address:
                        return FieldGroup.addressFields;
                    case DynamicAppend.FieldGroupName.Name:
                        return FieldGroup.nameFields;
                    case DynamicAppend.FieldGroupName.Phone:
                        return FieldGroup.phoneFields;
                    case DynamicAppend.FieldGroupName.BusinessName:
                        return FieldGroup.businessNameFields;
                    default:
                        throw new ReferenceError("Error: No fields exist for " + DynamicAppend.FieldGroupName[name]);
                }
            };
            FieldGroup.isSupported = function (name, fields) {
                if (fields.length === 0)
                    return false;
                switch (name) {
                    case DynamicAppend.FieldGroupName.Address:
                        return _.intersection(_.pluck(fields, "name"), FieldGroup.addressFields).length === FieldGroup.addressFields.length;
                    case DynamicAppend.FieldGroupName.Name:
                        return _.intersection(_.pluck(fields, "name"), FieldGroup.nameFields).length === FieldGroup.nameFields.length;
                    case DynamicAppend.FieldGroupName.Phone:
                        return _.intersection(_.pluck(fields, "name"), FieldGroup.phoneFields).length === FieldGroup.phoneFields.length;
                    case DynamicAppend.FieldGroupName.BusinessName:
                        return _.intersection(_.pluck(fields, "name"), FieldGroup.businessNameFields).length === FieldGroup.businessNameFields.length;
                    default:
                        return false;
                }
            };
            FieldGroup.addressFields = [DynamicAppend.FieldName.StreetAddress, DynamicAppend.FieldName.City, DynamicAppend.FieldName.State, DynamicAppend.FieldName.PostalCode];
            FieldGroup.ncoaAddressFields = [DynamicAppend.FieldName.StandardizedAddress, DynamicAppend.FieldName.StandardizedCity, DynamicAppend.FieldName.StandardizedCity, DynamicAppend.FieldName.StandardizedState, DynamicAppend.FieldName.StandardizedZip, DynamicAppend.FieldName.StandardizedAddressRange, DynamicAppend.FieldName.StandardizedStreetName];
            FieldGroup.nameFields = [DynamicAppend.FieldName.FirstName, DynamicAppend.FieldName.LastName];
            FieldGroup.phoneFields = [DynamicAppend.FieldName.PhoneNumber];
            FieldGroup.businessNameFields = [DynamicAppend.FieldName.BusinessName];
            return FieldGroup;
        }());
        DynamicAppend.FieldGroup = FieldGroup;
    })(DynamicAppend = AccurateAppend.DynamicAppend || (AccurateAppend.DynamicAppend = {}));
})(AccurateAppend || (AccurateAppend = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiQWNjdXJhdGVBcHBlbmQuRHluYW1pY0FwcGVuZC5PYmplY3RzLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiQWNjdXJhdGVBcHBlbmQuRHluYW1pY0FwcGVuZC5PYmplY3RzLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLElBQU8sY0FBYyxDQWdlcEI7QUFoZUQsV0FBTyxjQUFjO0lBQUMsSUFBQSxhQUFhLENBZ2VsQztJQWhlcUIsV0FBQSxhQUFhO1FBRS9CO1lBTUk7Z0JBQ0ksSUFBSSxDQUFDLFVBQVUsR0FBRyxFQUFFLENBQUMsZUFBZSxFQUFhLENBQUM7Z0JBQ2xELElBQUksQ0FBQyxZQUFZLEdBQUcsRUFBRSxDQUFDLFVBQVUsQ0FBQyxFQUFFLENBQUMsQ0FBQztZQUMxQyxDQUFDO1lBR0Qsc0JBQUkscUNBQWU7cUJBQW5CO29CQUVJLElBQUksTUFBTSxHQUFHLElBQUksS0FBSyxFQUFVLENBQUM7b0JBQ2pDLENBQUMsQ0FBQyxJQUFJLENBQUMsSUFBSSxDQUFDLFVBQVUsRUFBRSxFQUFFLFVBQUMsQ0FBQyxFQUFFLFNBQVM7d0JBQ25DLENBQUMsQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLE1BQU0sQ0FBQyxTQUFTLENBQUMsV0FBVyxFQUFFLFVBQUMsQ0FBQyxJQUFLLE9BQUEsQ0FBQyxDQUFDLENBQUMsYUFBYSxDQUFDLE9BQU8sQ0FBQyxHQUFHLENBQUMsS0FBSyxDQUFDLENBQUMsQ0FBQyxFQUFyQyxDQUFxQyxDQUFDLEVBQUUsVUFBQyxDQUFDLEVBQUUsS0FBSzs0QkFDM0YsTUFBTSxDQUFDLElBQUksQ0FBQyxLQUFLLENBQUMsYUFBYSxDQUFDLENBQUM7d0JBQ3JDLENBQUMsQ0FBQyxDQUFDO29CQUNQLENBQUMsQ0FBQyxDQUFDO29CQUNILE9BQU8sQ0FBQyxDQUFDLElBQUksQ0FBQyxNQUFNLENBQUMsQ0FBQztnQkFDMUIsQ0FBQzs7O2VBQUE7WUFFRCxzQkFBSSxrQ0FBWTtxQkFBaEI7b0JBRUksSUFBSSxNQUFNLEdB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UyxDQUFDLElBQUksRUFBRSxjQUFBLFNBQVMsQ0FBQyxLQUFLLEVBQUUsY0FBQSxTQUFTLENBQUMsVUFBVSxDQUFDLENBQUM7WUFDakcsNEJBQWlCLEdBQUcsQ0FBQyxjQUFBLFNBQVMsQ0FBQyxtQkFBbUIsRUFBRSxjQUFBLFNBQVMsQ0FBQyxnQkFBZ0IsRUFBRSxjQUFBLFNBQVMsQ0FBQyxnQkFBZ0IsRUFBRSxjQUFBLFNBQVMsQ0FBQyxpQkFBaUIsRUFBRSxjQUFBLFNBQVMsQ0FBQyxlQUFlLEVBQUUsY0FBQSxTQUFTLENBQUMsd0JBQXdCLEVBQUUsY0FBQSxTQUFTLENBQUMsc0JBQXNCLENBQUMsQ0FBQztZQUMxTyxxQkFBVSxHQUFHLENBQUMsY0FBQSxTQUFTLENBQUMsU0FBUyxFQUFFLGNBQUEsU0FBUyxDQUFDLFFBQVEsQ0FBQyxDQUFDO1lBQ3ZELHNCQUFXLEdBQUcsQ0FBQyxjQUFBLFNBQVMsQ0FBQyxXQUFXLENBQUMsQ0FBQztZQUN0Qyw2QkFBa0IsR0FBRyxDQUFDLGNBQUEsU0FBUyxDQUFDLFlBQVksQ0FBQyxDQUFDO1lBbUNqRSxpQkFBQztTQUFBLEFBekNELElBeUNDO1FBekNZLHdCQUFVLGFBeUN0QixDQUFBO0lBQ0wsQ0FBQyxFQWhlcUIsYUFBYSxHQUFiLDRCQUFhLEtBQWIsNEJBQWEsUUFnZWxDO0FBQUQsQ0FBQyxFQWhlTSxjQUFjLEtBQWQsY0FBYyxRQWdlcEIiLCJzb3VyY2VzQ29udGVudCI6WyJtb2R1bGUgQWNjdXJhdGVBcHBlbmQuRHluYW1pY0FwcGVuZCB7XHJcblxyXG4gICAgZXhwb3J0IGNsYXNzIE1hbmlmZXN0IHtcclxuXHJcbiAgICAgICAgLy8gZ2xvYmFsIE9wZXJhdGlvbnNcclxuICAgICAgICBvcGVyYXRpb25zOiBLbm9ja291dE9ic2VydmFibGVBcnJheTxPcGVyYXRpb24+O1xyXG4gICAgICAgIHN1cHJlc3Npb25pZDogS25vY2tvdXRPYnNlcnZhYmxlPHN0cmluZz47XHJcblxyXG4gICAgICAgIGNvbnN0cnVjdG9yKCkge1xyXG4gICAgICAgICAgICB0aGlzLm9wZXJhdGlvbnMgPSBrby5vYnNlcnZhYmxlQXJyYXk8T3BlcmF0aW9uPigpO1xyXG4gICAgICAgICAgICB0aGlzLnN1cHJlc3Npb25pZCA9IGtvLm9ic2VydmFibGUoXCJcIik7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAvLyByZXF1aXJlZCBpbnB1dGZpZWxkbmFtZXNcclxuICAgICAgICBnZXQgaW5wdXRmaWVsZG5hbWVzKCk6IEFycmF5PHN0cmluZz4ge1xyXG4gICAgICAgICAgICAvLyByZXR1cm4gYWxsIGZpZWxkcyBpbiBvcGVyYXRpb25zIHdoZXJlIGluY2x1ZGUgPT0gdHJ1ZVxyXG4gICAgICAgICAgICB2YXIgZmllbGRzID0gbmV3IEFycmF5PHN0cmluZz4oKTtcclxuICAgICAgICAgICAgJC5lYWNoKHRoaXMub3BlcmF0aW9ucygpLCAoaSwgb3BlcmF0aW9uKSA9PiB7XHJcbiAgICAgICAgICAgICAgICAkLmVhY2goXy5maWx0ZXIob3BlcmF0aW9uLmlucHV0ZmllbGRzLCAobykgPT4gKG8ubWV0YWZpZWxkTmFtZS5pbmRleE9mKFwiX1wiKSA9PT0gLTEpKSwgKGgsIGZpZWxkKSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgZmllbGRzLnB1c2goZmllbGQubWV0YWZpZWxkTmFtZSk7XHJcbiAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgIHJldHVybiBfLnVuaXEoZmllbGRzKTtcclxuICAgICAgICB9XHJcbiAgICAgICAgLy8gZ2xvYmFsIG91dHB1dGZpZWxkc1xyXG4gICAgICAgIGdldCBvdXRwdXRmaWVsZHMoKTogQXJyYXk8RmllbGQ+IHtcclxuICAgICAgICAgICAgLy8gcmV0dXJuIGFsbCBmaWVsZHMgaW4gb3BlcmF0aW9ucyB3aGVyZSBpbmNsdWRlID09IHRydWVcclxuICAgICAgICAgICAgdmFyIGZpZWxkcyA9IG5ldyBBcnJheTxGaWVsZD4oKTtcclxuICAgICAgICAgICAgJC5lYWNoKHRoaXMub3BlcmF0aW9ucygpLCAoaSwgb3BlcmF0aW9uKSA9PiB7XHJcbiAgICAgICAgICAgICAgICAkLmVhY2goXy5maWx0ZXIob3BlcmF0aW9uLm91dHB1dGZpZWxkcywgKG8pID0+IChvLmluY2x1ZGUoKSkpLCAoaCwgZmllbGQpID0+IHtcclxuICAgICAgICAgICAgICAgICAgICBmaWVsZHMucHVzaChmaWVsZCk7XHJcbiAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgIHJldHVybiBmaWVsZHM7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIFxyXG4gICAgICAgIC8vIEFkZHMgYW4gT3BlcmF0aW9uIHVzaW5nIGEgU3RyYXRlZ3lcclxuICAgICAgICBhZGQobmFtZTogc3RyaW5nLCBzdWNjZXNzQ2FsbGJhY2s6IEZ1bmN0aW9uLCBlcnJvckNhbGxiYWNrOiBGdW5jdGlvbikge1xyXG4gICAgICAgICAgICB2YXIgc2VsZiA9IHRoaXM7XHJcbiAgICAgICAgICAgICQuYWpheChcclxuICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICB0eXBlOiBcIkdFVFwiLFxyXG4gICAgICAgICAgICAgICAgICAgIHVybDogXCIvQmF0Y2gvR2V0T3BlcmF0aW9uRGVmaW50aW9uXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgZGF0YTogeyBuYW1lOiBuYW1lIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgc3VjY2VzczogZGF0YSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIC8vIGNyZWF0ZSBuZXcgT3BlcmF0aW9uIGZyb20gRGVmaW5pdGlvbnNcclxuICAgICAgICAgICAgICAgICAgICAgICAgdmFyIG9wZXJhdGlvbiA9IG5ldyBPcGVyYXRpb24oZGF0YSk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIC8vIHNvbWUgb3BlcmF0aW9ucyByZXF1aXJlIHNwZWNpYWwgbWFwcGluZyBcclxuICAgICAgICAgICAgICAgICAgICAgICAgdmFyIGU7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHN3aXRjaCAob3BlcmF0aW9uLm5hbWUpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNhc2UgT3BlcmF0aW9uTmFtZS5ERURVUEVfUEhPTkU6XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdHJ5IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5tYXAobmV3IERlZHVwZVBob25lU3RyYXRlZ3koKSwgb3BlcmF0aW9uLCB0aGlzLm9wZXJhdGlvbnMoKSk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMub3BlcmF0aW9ucy5wdXNoKG9wZXJhdGlvbik7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHN1Y2Nlc3NDYWxsYmFjaygpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0gY2F0Y2ggKGUpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgZXJyb3JDYWxsYmFjayhlKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgYnJlYWs7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjYXNlIE9wZXJhdGlvbk5hbWUuU0VUX1BSRUZfUEhPTkU6XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdHJ5IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5tYXAobmV3IFNldFByZWZQaG9uZVN0cmF0ZWd5KCksIG9wZXJhdGlvbiwgdGhpcy5vcGVyYXRpb25zKCkpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLm9wZXJhdGlvbnMucHVzaChvcGVyYXRpb24pO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBzdWNjZXNzQ2FsbGJhY2soKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB9IGNhdGNoIChlKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGVycm9yQ2FsbGJhY2soZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGJyZWFrO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgY2FzZSBPcGVyYXRpb25OYW1lLlNFVF9QUkVGX1BIT05FX1NJTkdMRV9DT0xVTU46XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdHJ5IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5tYXAobmV3IFNldFByZWZQaG9uZVNpbmdsZUNvbHVtblN0cmF0ZWd5KCksIG9wZXJhdGlvbiwgdGhpcy5vcGVyYXRpb25zKCkpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLm9wZXJhdGlvbnMucHVzaChvcGVyYXRpb24pO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBzdWNjZXNzQ2FsbGJhY2soKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB9IGNhdGNoIChlKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGVycm9yQ2FsbGJhY2soZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGJyZWFrO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgY2FzZSBPcGVyYXRpb25OYW1lLlNFVF9QUkVGX0FERFJFU1NfU0lOR0xFX0NPTFVNTjpcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB0cnkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLm1hcChuZXcgU2V0UHJlZkFkZHJlc3NTaW5nbGVDb2x1bW5TdHJhdGVneSgpLCBvcGVyYXRpb24sIHRoaXMub3BlcmF0aW9ucygpKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5vcGVyYXRpb25zLnB1c2gob3BlcmF0aW9uKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgc3VjY2Vzc0NhbGxiYWNrKCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfSBjYXRjaCAoZSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBlcnJvckNhbGxiYWNrKGUpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBicmVhaztcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNhc2UgT3BlcmF0aW9uTmFtZS5TRVRfUFJFRl9QSE9ORV9DT01QQVJFX0lOUFVUOlxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRyeSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMubWFwKG5ldyBTZXRQcmVmUGhvbmVDb21wYXJlSW5wdXRTdHJhdGVneSgpLCBvcGVyYXRpb24sIHRoaXMub3BlcmF0aW9ucygpKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5vcGVyYXRpb25zLnB1c2gob3BlcmF0aW9uKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgc3VjY2Vzc0NhbGxiYWNrKCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfSBjYXRjaCAoZSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBlcnJvckNhbGxiYWNrKGUpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBicmVhaztcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNhc2UgT3BlcmF0aW9uTmFtZS5TRVRfUFJFRl9CQVNFRF9PTl9WRVJJRklDQVRJT046XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdHJ5IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5tYXAobmV3IFNldFByZWZCYXNlZE9uVmVyaWZpY2F0aW9uU3RyYXRlZ3koKSwgb3BlcmF0aW9uLCB0aGlzLm9wZXJhdGlvbnMoKSk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMub3BlcmF0aW9ucy5wdXNoKG9wZXJhdGlvbik7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHN1Y2Nlc3NDYWxsYmFjaygpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0gY2F0Y2ggKGUpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgZXJyb3JDYWxsYmFjayhlKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgYnJlYWs7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjYXNlIE9wZXJhdGlvbk5hbWUuU0VUX1BSRUZfRU1BSUxfVkVSOlxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRyeSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMubWFwKG5ldyBTZXRQcmVmRW1haWxWZXJTdHJhdGVneSgpLCBvcGVyYXRpb24sIHRoaXMub3BlcmF0aW9ucygpKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5vcGVyYXRpb25zLnB1c2gob3BlcmF0aW9uKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgc3VjY2Vzc0NhbGxiYWNrKCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfSBjYXRjaCAoZSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBlcnJvckNhbGxiYWNrKGUpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBicmVhaztcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNhc2UgT3BlcmF0aW9uTmFtZS5OQ09BNDg6XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5tYXAobmV3IE5jb2E0OFN0cmF0ZWd5KCksIG9wZXJhdGlvbiwgdGhpcy5vcGVyYXRpb25zKCkpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMub3BlcmF0aW9ucy51bnNoaWZ0KG9wZXJhdGlvbik7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgYnJlYWs7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBkZWZhdWx0OlxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIC8vIGlmIE5DT0EgaXMgcHJlc2VudCBhbmQgdGhpcyBvcGVyYXRpb24gaGFzIEFkZHJlc3MgaW5wdXQgdGhlbiBtYXAgaXQncyBpbnB1dHMgdG8gTkNPQVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGlmIChfLmNvbnRhaW5zKF8ubWFwKHRoaXMub3BlcmF0aW9ucygpLCB2ID0+IHYubmFtZSksIE9wZXJhdGlvbk5hbWUuTkNPQTQ4KSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBjb25zb2xlLmxvZyhcIk5DT0EgZGV0ZWN0ZWRcIik7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMubWFwKG5ldyBOY29hNDhTdHJhdGVneVJlbW92ZSgpLCBvcGVyYXRpb24sIHRoaXMub3BlcmF0aW9ucygpKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5vcGVyYXRpb25zLnB1c2gob3BlcmF0aW9uKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5tYXAobmV3IE5jb2E0OFN0cmF0ZWd5KCksIG9wZXJhdGlvbiwgdGhpcy5vcGVyYXRpb25zKCkpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMub3BlcmF0aW9ucy5wdXNoKG9wZXJhdGlvbik7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGJyZWFrO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICBlcnJvcjogKHhociwgc3RhdHVzLCBlcnJvcikgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBhbGVydChcIlVuYWJsZSB0byBnZXQgZGVmaW50aW9uIGZvciBcIiArIG5hbWUpO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8gUmVtb3ZlcyBhbiBPcGVyYXRpb25cclxuICAgICAgICByZW1vdmUobmFtZTogc3RyaW5nKSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKFwiUmVtb3ZpbmcgXCIgKyBuYW1lKTtcclxuICAgICAgICAgICAgLy8gZGV0ZXJtaW5lIGlmIG9lcHJhdGlvbiBpcyBpbiBjb2xsZWN0aW9uIGJlZm9yZSByZW1vdmFsXHJcbiAgICAgICAgICAgIHZhciBvcGVyYXRpb24gPSBfLmZpcnN0KF8uZmlsdGVyKHRoaXMub3BlcmF0aW9ucygpLCAobykgPT4gKG8ubmFtZSA9PT0gPE9wZXJhdGlvbk5hbWU+T3BlcmF0aW9uTmFtZVs8c3RyaW5nPm5hbWVdKSkpO1xyXG4gICAgICAgICAgICBpZiAoIW9wZXJhdGlvbikgcmV0dXJuO1xyXG4gICAgICAgICAgICBzd2l0Y2ggKG9wZXJhdGlvbi5uYW1lKSB7XHJcbiAgICAgICAgICAgICAgICBjYXNlIE9wZXJhdGlvbk5hbWUuU0VUX1BSRUZfUEhPTkVfU0lOR0xFX0NPTFVNTjpcclxuICAgICAgICAgICAgICAgICAgICB0aGlzLm1hcChuZXcgU2V0UHJlZlBob25lU2luZ2xlQ29sdW1uU3RyYXRlZ3lSZW1vdmUoKSwgb3BlcmF0aW9uLCB0aGlzLm9wZXJhdGlvbnMoKSk7XHJcbiAgICAgICAgICAgICAgICAgICAgYnJlYWs7XHJcbiAgICAgICAgICAgICAgICBjYXNlIE9wZXJhdGlvbk5hbWUuU0VUX1BSRUZfRU1BSUxfVkVSOlxyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMubWFwKG5ldyBTZXRQcmVmRW1haWxWZXJTdHJhdGVneVJlbW92ZSgpLCBvcGVyYXRpb24sIHRoaXMub3BlcmF0aW9ucygpKTtcclxuICAgICAgICAgICAgICAgICAgICBicmVhaztcclxuICAgICAgICAgICAgICAgIGNhc2UgT3BlcmF0aW9uTmFtZS5OQ09BNDg6XHJcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5tYXAobmV3IE5jb2E0OFN0cmF0ZWd5UmVtb3ZlKCksIG9wZXJhdGlvbiwgdGhpcy5vcGVyYXRpb25zKCkpO1xyXG4gICAgICAgICAgICAgICAgICAgIGJyZWFrO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIHRoaXMub3BlcmF0aW9ucy5yZW1vdmUob3BlcmF0aW9uKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIC8vIEJ1aWxkcyBKU09OIHVzaW5nIG1hcHBlZCBvYmplY3RzIHRoYXQgZXhhY3RseSBtYXRjaCBsZWdhY3kgb2JqZWN0cyBcclxuICAgICAgICB0b0pzb24oKSB7XHJcblxyXG4gICAgICAgICAgICAvLyBzZXQgU2VxdWVuY2VJZFxyXG4gICAgICAgICAgICAkLmVhY2godGhpcy5vcGVyYXRpb25zKCksIChpLCBvKSA9PiB7XHJcbiAgICAgICAgICAgICAgICBvLnNlcXVuY2VpZCA9IGk7XHJcbiAgICAgICAgICAgICAgICAkLmVhY2goby5pbnB1dGZpZWxkcywgKGosIGYpID0+IHtcclxuICAgICAgICAgICAgICAgICAgICBmLnNlcXVuY2VpZCA9IG8uc2VxdW5jZWlkO1xyXG4gICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICAkLmVhY2goby5vdXRwdXRmaWVsZHMsIChqLCBmKSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgZi5zZXF1bmNlaWQgPSBvLnNlcXVuY2VpZDtcclxuICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgIC8vIGJ1aWxkIG1hcHBlZCB2ZXJzaW9uIG9mIE1hbmlmZXN0IHRoYXQgZXhhY3RseSBtYXRjaGVzIG1hbmlmZXN0IGV4cGVjdGVkIGJ5IGNvbnRyb2xsZXJcclxuICAgICAgICAgICAgcmV0dXJuIGtvLnRvSlNPTih7XHJcbiAgICAgICAgICAgICAgICBcIk9wZXJhdGlvbnNcIjoga28udXRpbHMuYXJyYXlNYXAodGhpcy5vcGVyYXRpb25zKCksIG8gPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIFNlcXVlbmNlSWQ6IG8uc2VxdW5jZWlkLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBOYW1lOiBPcGVyYXRpb25OYW1lW28ubmFtZV0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIE1hdGNoTGV2ZWxzOiBrby51dGlscy5hcnJheU1hcChvLm1hdGNobGV2ZWxzLCBtID0+IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgTmFtZTogbS5uYW1lLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIEluY2x1ZGU6IG0uaW5jbHVkZVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9KSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgUXVhbGl0eUxldmVsczoga28udXRpbHMuYXJyYXlNYXAoby5xdWFsaXR5bGV2ZWxzLCBtID0+IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgTmFtZTogbS5uYW1lLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIEluY2x1ZGU6IG0uaW5jbHVkZVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9KSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgU291cmNlczoga28udXRpbHMuYXJyYXlNYXAoby5zb3VyY2VzLCBtID0+IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgTmFtZTogbS5uYW1lLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIEluY2x1ZGU6IG0uaW5jbHVkZVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9KSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgSW5wdXRGaWVsZHM6IGtvLnV0aWxzLmFycmF5TWFwKG8uaW5wdXRmaWVsZHMsIGYgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBNZXRhRmllbGROYW1lOiBmLm1ldGFmaWVsZE5hbWUsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgT3BlcmF0aW9uUGFyYW1OYW1lOiBmLm9wZXJhdGlvbnBhcmFtbmFtZSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBTZXF1ZW5jZUlkOiBmLnNlcXVuY2VpZCxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBJbmNsdWRlOiBmLmluY2x1ZGUsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgUmVxdWlyZWQ6IGYucmVxdWlyZWRcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgfSksXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIE91dHB1dEZpZWxkczoga28udXRpbHMuYXJyYXlNYXAoby5vdXRwdXRmaWVsZHMsIGYgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBDb2x1bW5UaXRsZTogZi5jb2x1bW50aXRsZSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBNZXRhRmllbGROYW1lOiBmLm1ldGFmaWVsZE5hbWUsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgT3BlcmF0aW9uUGFyYW1OYW1lOiBmLm9wZXJhdGlvbnBhcmFtbmFtZSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBTZXF1ZW5jZUlkOiBmLnNlcXVuY2VpZCxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBJbmNsdWRlOiBmLmluY2x1ZGUsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgUmVxdWlyZWQ6IGYucmVxdWlyZWRcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgfSksXHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfSksXHJcbiAgICAgICAgICAgICAgICBcIk91dHB1dEZpZWxkc1wiOiBrby51dGlscy5hcnJheU1hcCh0aGlzLm91dHB1dGZpZWxkcywgZiA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgeFBhdGg6IGYueHBhdGgsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIENvbHVtblRpdGxlOiBmLmNvbHVtbnRpdGxlLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBNZXRhRmllbGROYW1lOiBmLm1ldGFmaWVsZE5hbWUsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIE9wZXJhdGlvblBhcmFtTmFtZTogZi5vcGVyYXRpb25wYXJhbW5hbWUsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIFNlcXVlbmNlSWQ6IGYuc2VxdW5jZWlkLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBJbmNsdWRlOiBmLmluY2x1ZGUsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIFJlcXVpcmVkOiBmLnJlcXVpcmVkXHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfSlcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAvLyBzZXQgXCJpbmNsdWRlXCIgYXR0cmlidXRlIGluIFwib3V0cHV0ZmllbGRzXCIgdG8gdHJ1ZS9mYWxzZSBmb3IgZ2l2ZW4gT3BlcmF0aW9uXHJcbiAgICAgICAgc3RhdGljIHRvZ2dsZU91dHB1dGZpZWxkcyA9IChpbmNsdWRlOiBib29sZWFuLCAuLi5vcGVyYXRpb25zOiBBcnJheTxPcGVyYXRpb24+KSA9PiB7XHJcbiAgICAgICAgICAgICQuZWFjaChvcGVyYXRpb25zLCAoaSwgb3BlcmF0aW9uKSA9PiB7XHJcbiAgICAgICAgICAgICAgICAkLmVhY2gob3BlcmF0aW9uLm91dHB1dGZpZWxkcywgKGgsIGZpZWxkKSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgZmllbGQuaW5jbHVkZShpbmNsdWRlKTtcclxuICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIC8vIG1hcHMgdGFyZ2V0IE9wZXJhdGlvbiB0byBnbG9iYWwgT3BlcmF0aW9uIGNvbGxlY3Rpb25cclxuICAgICAgICBwcml2YXRlIG1hcChzdHJhdGVneTogSU1hcHBpbmdTdHJhdGVneSwgdGFyZ2V0OiBPcGVyYXRpb24sIG9wZXJhdGlvbnM6IEFycmF5PE9wZXJhdGlvbj4pIHtcclxuICAgICAgICAgICAgcmV0dXJuIHN0cmF0ZWd5LmV4ZWN1dGUodGFyZ2V0LCBvcGVyYXRpb25zKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgZXhwb3J0IGNsYXNzIE9wZXJhdGlvbiB7XHJcbiAgICAgICAgXHJcbiAgICAgICAgc2VxdW5jZWlkOiBudW1iZXI7XHJcbiAgICAgICAgdGl0bGU6IHN0cmluZztcclxuICAgICAgICBkZXNjcmlwdGlvbjogc3RyaW5nO1xyXG4gICAgICAgIG5hbWU6IE9wZXJhdGlvbk5hbWU7XHJcbiAgICAgICAgbWF0Y2hsZXZlbHM6IEFycmF5PE1hdGNoTGV2ZWw+O1xyXG4gICAgICAgIHF1YWxpdHlsZXZlbHM6IEFycmF5PFF1YWxpdHlMZXZlbD47XHJcbiAgICAgICAgaW5wdXRmaWVsZHM6IEFycmF5PEZpZWxkPjtcclxuICAgICAgICBvdXRwdXRmaWVsZHM6IEFycmF5PEZpZWxkPjtcclxuICAgICAgICBzb3VyY2VzOiBBcnJheTxTb3VyY2U+O1xyXG5cclxuICAgICAgICBjb25zdHJ1Y3RvcihvYmo6IGFueSkge1xyXG4gICAgICAgICAgICB2YXIgc2VsZiA9IHRoaXM7XHJcbiAgICAgICAgICAgIHNlbGYudGl0bGUgPSBvYmouVGl0bGU7XHJcbiAgICAgICAgICAgIHNlbGYuZGVzY3JpcHRpb24gPSBvYmouRGVzY3JpcHRpb247XHJcbiAgICAgICAgICAgIHNlbGYubmFtZSA9IDxPcGVyYXRpb25OYW1lPk9wZXJhdGlvbk5hbWVbPHN0cmluZz5vYmouTmFtZV07XHJcbiAgICAgICAgICAgIHNlbGYubWF0Y2hsZXZlbHMgPSBuZXcgQXJyYXk8TWF0Y2hMZXZlbD4oKTtcclxuICAgICAgICAgICAgc2VsZi5xdWFsaXR5bGV2ZWxzID0gbmV3IEFycmF5PE1hdGNoTGV2ZWw+KCk7XHJcbiAgICAgICAgICAgIHNlbGYuaW5wdXRmaWVsZHMgPSBBcnJheTxGaWVsZD4oKTtcclxuICAgICAgICAgICAgc2VsZi5vdXRwdXRmaWVsZHMgPSBBcnJheTxGaWVsZD4oKTtcclxuICAgICAgICAgICAgc2VsZi5zb3VyY2VzID0gQXJyYXk8U291cmNlPigpO1xyXG4gICAgICAgICAgICAkLmVhY2gob2JqLk1hdGNoTGV2ZWxzLCAoaSwgbWF0Y2hsZXZlbCkgPT4geyBzZWxmLm1hdGNobGV2ZWxzLnB1c2gobmV3IE1hdGNoTGV2ZWwobWF0Y2hsZXZlbCkpIH0pO1xyXG4gICAgICAgICAgICAkLmVhY2gob2JqLlF1YWxpdHlMZXZlbHMsIChpLCBxdWFsaXR5bGV2ZWwpID0+IHsgc2VsZi5xdWFsaXR5bGV2ZWxzLnB1c2gobmV3IFF1YWxpdHlMZXZlbChxdWFsaXR5bGV2ZWwpKSB9KTtcclxuICAgICAgICAgICAgJC5lYWNoKG9iai5JbnB1dEZpZWxkcywgKGksIGlucHV0ZmllbGQpID0+IHsgc2VsZi5pbnB1dGZpZWxkcy5wdXNoKG5ldyBGaWVsZChpbnB1dGZpZWxkLCBzZWxmLm5hbWUsIG51bGwpKSB9KTtcclxuICAgICAgICAgICAgJC5lYWNoKG9iai5Tb3VyY2VzLCAoaSwgc291cmNlKSA9PiB7IHNlbGYuc291cmNlcy5wdXNoKG5ldyBTb3VyY2Uoc291cmNlKSkgfSk7XHJcbiAgICAgICAgICAgIHZhciBwcmV2aW91czogRmllbGQ7XHJcbiAgICAgICAgICAgICQuZWFjaChvYmouT3V0cHV0RmllbGRzLCAoaSwgb3V0cHV0ZmllbGQpID0+IHtcclxuICAgICAgICAgICAgICAgIHZhciBmaWVsZDogRmllbGQ7XHJcbiAgICAgICAgICAgICAgICAvLyBNYXRjaExldmVsIGFuZCBRdWFsaXR5TGV2ZWwgZmllbGRzIHJlcXVpcmUgeHBhdGggd2l0aCBhdHRyaWJ1dGUgYW5kIG11c3QgaGF2ZSB0aGUgc2FtZSBtZXRhZmllbGRuYW1lIGFzIGl0J3MgcGFyZW50IGZpZWxkXHJcbiAgICAgICAgICAgICAgICBzd2l0Y2ggKG91dHB1dGZpZWxkLk1ldGFGaWVsZE5hbWUpIHtcclxuICAgICAgICAgICAgICAgIGNhc2UgXCJNYXRjaExldmVsXCI6XHJcbiAgICAgICAgICAgICAgICBjYXNlIFwiUXVhbGl0eUxldmVsXCI6XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIC8vIE1hdGNoTGV2ZWwgYW5kIFF1YWxpdHlMZXZlbCBuZWVkIHRvIHVzZSBwYXJlbnQgZmllbGQgbmFtZVxyXG4gICAgICAgICAgICAgICAgICAgICAgICBmaWVsZCA9IG5ldyBGaWVsZChvdXRwdXRmaWVsZCwgc2VsZi5uYW1lLCBwcmV2aW91cy5tZXRhZmllbGROYW1lKTtcclxuICAgICAgICAgICAgICAgICAgICBicmVhaztcclxuICAgICAgICAgICAgICAgIGRlZmF1bHQ6XHJcbiAgICAgICAgICAgICAgICAgICAgZmllbGQgPSBuZXcgRmllbGQob3V0cHV0ZmllbGQsIHNlbGYubmFtZSwgRmllbGQuZ2VuZXJhdGVGaWVsZElkKCkpO1xyXG4gICAgICAgICAgICAgICAgICAgIC8vIHN0b3JlIHRoZSBwcmV2aW91cyBmaWVsZCwgdXNlZCB0byBhZGQgTWF0Y2hMZXZlbCBvciBRdWFsaXR5TGV2ZWwgYWJvdmVcclxuICAgICAgICAgICAgICAgICAgICBwcmV2aW91cyA9IChvdXRwdXRmaWVsZC5uYW1lICE9PSBGaWVsZE5hbWUuTWF0Y2hMZXZlbCAmJiBvdXRwdXRmaWVsZC5uYW1lICE9PSBGaWVsZE5hbWUuUXVhbGl0eUxldmVsKSA/IGZpZWxkIDogbnVsbDtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIHNlbGYub3V0cHV0ZmllbGRzLnB1c2goZmllbGQpO1xyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIC8vIGRldGVybWluZXMgaWYgdGhlIHRoaXMgb3BlcmF0aW9uIGlzIG1hcHBlZCB0byB0aGUgaW5wdXQgb3BlcmF0aW9uXHJcbiAgICAgICAgaXNNYXBwZWQob3BlcmF0aW9uOiBPcGVyYXRpb24pIHtcclxuICAgICAgICAgICAgcmV0dXJuIF8uZmlsdGVyKE9wZXJhdGlvbi5nZXRNZXRhZmllbGRuYW1lcyh0aGlzLm91dHB1dGZpZWxkcyksIGkgPT4gKCQuaW5BcnJheShpLCBPcGVyYXRpb24uZ2V0TWV0YWZpZWxkbmFtZXMob3BlcmF0aW9uLmlucHV0ZmllbGRzKSkgPiAtMSkpLmxlbmd0aCA+IDA7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAvLyBpZGVudGlmaWVzIHV0aWxpdHkgb3BlcmF0aW9uc1xyXG4gICAgICAgIGlzVXRpbCgpIHtcclxuICAgICAgICAgICAgc3dpdGNoICh0aGlzLm5hbWUpIHtcclxuICAgICAgICAgICAgY2FzZSBPcGVyYXRpb25OYW1lLkRFRFVQRV9QSE9ORTpcclxuICAgICAgICAgICAgY2FzZSBPcGVyYXRpb25OYW1lLlNFVF9QUkVGX1BIT05FOlxyXG4gICAgICAgICAgICBjYXNlIE9wZXJhdGlvbk5hbWUuU0VUX1BSRUZfUEhPTkVfU0lOR0xFX0NPTFVNTjpcclxuICAgICAgICAgICAgY2FzZSBPcGVyYXRpb25OYW1lLlNFVF9QUkVGX0FERFJFU1NfU0lOR0xFX0NPTFVNTjpcclxuICAgICAgICAgICAgICAgIHJldHVybiB0cnVlO1xyXG4gICAgICAgICAgICBkZWZhdWx0OlxyXG4gICAgICAgICAgICAgICAgcmV0dXJuIGZhbHNlO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAvLyBtYXBzIGZpZWxkIGluICdmcm9tJyB0byBmaWVsZCBpbiAndG8nXHJcbiAgICAgICAgc3RhdGljIG1hcEZpZWxkKGZyb206IE9wZXJhdGlvbiwgZnJvbU9wZXJhdGlvbmZpZWxkTmFtZTogRmllbGROYW1lLCB0bzogT3BlcmF0aW9uLCB0b2ZpZWxkTmFtZTogRmllbGROYW1lKSB7XHJcbiAgICAgICAgICAgIC8vIGdldCBtYXRjaGluZyBmaWVsZCBmcm9tIG9wZXJhdGlvbi5vdXRwdXRmaWVsZHNcclxuICAgICAgICAgICAgdmFyIGZyb21GaWVsZCA9IF8uZmluZChmcm9tLm91dHB1dGZpZWxkcywgbyA9PiAoby5uYW1lID09PSBmcm9tT3BlcmF0aW9uZmllbGROYW1lKSk7XHJcbiAgICAgICAgICAgIGlmICghZnJvbUZpZWxkKSBjb25zb2xlLmxvZyhcIlVuYWJsZSB0byBsb2NhdGUgJ2Zyb206J1wiICsgRmllbGROYW1lW2Zyb21PcGVyYXRpb25maWVsZE5hbWVdICsgXCIgZmllbGQgaW4gXCIgKyBPcGVyYXRpb25OYW1lW2Zyb20ubmFtZV0pO1xyXG4gICAgICAgICAgICAvLyBnZXQgbWF0Y2hpbmdmaWVsZCBmcm9tIHRoaXMuaW5wdXRmaWVsZHNcclxuICAgICAgICAgICAgdmFyIHRvRmllbGQgPSBfLmZpbmQodG8uaW5wdXRmaWVsZHMsIG8gPT4gKG8ubmFtZSA9PT0gdG9maWVsZE5hbWUpKTtcclxuICAgICAgICAgICAgaWYgKCF0b0ZpZWxkKSBjb25zb2xlLmxvZyhcIlVuYWJsZSB0byBsb2NhdGUgJ3RvJzpcIiArIEZpZWxkTmFtZVtmcm9tT3BlcmF0aW9uZmllbGROYW1lXSArIFwiIGZpZWxkIGluIFwiICsgT3BlcmF0aW9uTmFtZVt0by5uYW1lXSk7XHJcbiAgICAgICAgICAgIC8vIHNldCB0byBtZXRhZmllbGRuYW1lIHRvIGZyb20gbWV0YWZpZWxkbmFtZVxyXG4gICAgICAgICAgICB0b0ZpZWxkLm1ldGFmaWVsZE5hbWUgPSBGaWVsZC5nZW5lcmF0ZUZpZWxkSWQoKTtcclxuICAgICAgICAgICAgdG9GaWVsZC5tZXRhZmllbGROYW1lID0gZnJvbUZpZWxkLm1ldGFmaWVsZE5hbWU7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAvLyBnZXQgbWV0YWZpZWxkbmFtZXMgZm9yIGEgY29sbGVjdGlvbiBvZiBGaWVsZFxyXG4gICAgICAgIHN0YXRpYyBnZXRNZXRhZmllbGRuYW1lcyhmaWVsZHM6IEFycmF5PEZpZWxkPikge1xyXG4gICAgICAgICAgICByZXR1cm4gXy5tYXAoZmllbGRzLCBmaWVsZCA9PiBmaWVsZC5tZXRhZmllbGROYW1lKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgZXhwb3J0IGNsYXNzIEZpZWxkIHtcclxuXHJcbiAgICAgICAgc2VxdW5jZWlkOiBudW1iZXI7XHJcbiAgICAgICAgbmFtZTogRmllbGROYW1lOyAvLyBhbGxvd3MgdXMgdG8gc3RhbmRhcml6ZSBuYW1lcyByZWdhcmRsZXNzIG9mIHRoZSBuYW1lIGluIHRoZSBtYW5pZmVzdCwgbmVlZGVkIHRvIHdvcmsgd2l0aCBGaWVsZEdyb3Vwc1xyXG4gICAgICAgIG9wZXJhdGlvbnBhcmFtbmFtZTogc3RyaW5nO1xyXG4gICAgICAgIGNvbHVtbnRpdGxlOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPjtcclxuICAgICAgICByZXF1aXJlZDogYm9vbGVhbjtcclxuICAgICAgICBpbmNsdWRlOiBLbm9ja291dE9ic2VydmFibGU8Ym9vbGVhbj47XHJcbiAgICAgICAgY29sb3I6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+O1xyXG4gICAgICAgIHByaXZhdGUgX21ldGFmaWVsZG5hbWU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxzdHJpbmc+O1xyXG4gICAgICAgIFxyXG4gICAgICAgIGNvbnN0cnVjdG9yKG9iajogYW55LCBvcGVyYXRpb25OYW1lOiBPcGVyYXRpb25OYW1lLCBmaWVsZElkOiBzdHJpbmcpIHtcclxuICAgICAgICAgICAgdmFyIG5hbWUgPSA8RmllbGROYW1lPkZpZWxkTmFtZVs8c3RyaW5nPm9iai5NZXRhRmllbGROYW1lXTtcclxuICAgICAgICAgICAgdGhpcy5uYW1lID0gdGhpcy5zdGFuZGFyZGl6ZUZpZWxkTmFtZShuYW1lLCBvcGVyYXRpb25OYW1lKTtcclxuXHJcbiAgICAgICAgICAgIHRoaXMub3BlcmF0aW9ucGFyYW1uYW1lID0gb2JqLk9wZXJhdGlvblBhcmFtTmFtZTtcclxuICAgICAgICAgICAgdGhpcy5jb2x1bW50aXRsZSA9IGtvLm9ic2VydmFibGUob2JqLkNvbHVtblRpdGxlKTtcclxuICAgICAgICAgICAgdGhpcy5yZXF1aXJlZCA9IG9iai5SZXF1aXJlZDtcclxuICAgICAgICAgICAgdGhpcy5pbmNsdWRlID0ga28ub2JzZXJ2YWJsZShvYmouSW5jbHVkZSk7XHJcbiAgICAgICAgICAgIHRoaXMuX21ldGFmaWVsZG5hbWUgPSBrby5vYnNlcnZhYmxlKGZpZWxkSWQgfHwgb2JqLk1ldGFGaWVsZE5hbWUpO1xyXG4gICAgICAgICAgICB0aGlzLmNvbG9yID0ga28ub2JzZXJ2YWJsZShcImJsYWNrXCIpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgc2V0IG1ldGFmaWVsZE5hbWUodmFsdWU6IHN0cmluZykge1xyXG4gICAgICAgICAgICB0aGlzLl9tZXRhZmllbGRuYW1lKHZhbHVlKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGdldCBtZXRhZmllbGROYW1lKCk6IHN0cmluZyB7XHJcbiAgICAgICAgICAgIHN3aXRjaCAodGhpcy5uYW1lKSB7XHJcbiAgICAgICAgICAgIGNhc2UgRmllbGROYW1lLk1hdGNoTGV2ZWw6XHJcbiAgICAgICAgICAgIGNhc2UgRmllbGROYW1lLlF1YWxpdHlMZXZlbDpcclxuICAgICAgICAgICAgICAgIHJldHVybiB0aGlzLl9tZXRhZmllbGRuYW1lKCkgKyBcIl9cIiArIEZpZWxkTmFtZVt0aGlzLm5hbWVdO1xyXG4gICAgICAgICAgICBkZWZhdWx0OlxyXG4gICAgICAgICAgICAgICAgcmV0dXJuIHRoaXMuX21ldGFmaWVsZG5hbWUoKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgZ2V0IHhwYXRoKCk6IHN0cmluZyB7XHJcbiAgICAgICAgICAgIHN3aXRjaCAodGhpcy5uYW1lKSB7XHJcbiAgICAgICAgICAgIGNhc2UgRmllbGROYW1lLk1hdGNoTGV2ZWw6XHJcbiAgICAgICAgICAgIGNhc2UgRmllbGROYW1lLlF1YWxpdHlMZXZlbDpcclxuICAgICAgICAgICAgICAgIHJldHVybiB0aGlzLl9tZXRhZmllbGRuYW1lKCkgKyBcIi9AXCIgKyBGaWVsZE5hbWVbdGhpcy5uYW1lXTtcclxuICAgICAgICAgICAgZGVmYXVsdDpcclxuICAgICAgICAgICAgICAgIHJldHVybiB0aGlzLl9tZXRhZmllbGRuYW1lKCk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIC8vIHNvbWUgZmllbGRzIHJlZmVyZW5jZWQgaW4gdGhlIGluY29taW5nIE1hbmlmZXN0IGRlZmluaXRpb24gY29udGFpbiBhZGRyZXNzIGNvbXBvbmVudHMgYnV0IGFyZSBuYW1lZCBpbiBhIG5vbi1zdGFuZGFyZCB3YXkgbGlrZSBQYXJzZWRBZGRyZXNzLCBQYXJzZWRDaXR5XHJcbiAgICAgICAgLy8gdGhpcyBtZXRob2Qgc3RhbmRhcmRpemVzIHRoZSBuYW1pbmcgc28gd2UgY2FuIG9wZXJhdGUgb24gZ3JvdXBzIG9mIGZpZWxkcyB1c2luZyBGaWVsZEdyb3VwXHJcbiAgICAgICAgc3RhbmRhcmRpemVGaWVsZE5hbWUobmFtZTogRmllbGROYW1lLCBvcGVyYXRpb25OYW1lOiBPcGVyYXRpb25OYW1lKSB7XHJcbiAgICAgICAgICAgIHN3aXRjaCAob3BlcmF0aW9uTmFtZSkge1xyXG4gICAgICAgICAgICBjYXNlIE9wZXJhdGlvbk5hbWUuTkNPQTQ4OlxyXG4gICAgICAgICAgICAgICAgc3dpdGNoIChuYW1lKSB7XHJcbiAgICAgICAgICAgICAgICBjYXNlIEZpZWxkTmFtZS5TdGFuZGFyZGl6ZWRBZGRyZXNzOlxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiBGaWVsZE5hbWUuU3RyZWV0QWRkcmVzcztcclxuICAgICAgICAgICAgICAgIGNhc2UgRmllbGROYW1lLlN0YW5kYXJkaXplZENpdHk6XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIEZpZWxkTmFtZS5DaXR5O1xyXG4gICAgICAgICAgICAgICAgY2FzZSBGaWVsZE5hbWUuU3RhbmRhcmRpemVkU3RhdGU6XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIEZpZWxkTmFtZS5TdGF0ZTtcclxuICAgICAgICAgICAgICAgIGNhc2UgRmllbGROYW1lLlN0YW5kYXJkaXplZFppcDpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gRmllbGROYW1lLlBvc3RhbENvZGU7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIGRlZmF1bHQ6XHJcbiAgICAgICAgICAgICAgICByZXR1cm4gbmFtZTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8gZ2VuZXJhdGVzIGEgdW5pcXVlIGZpZWxkIG5hbWVcclxuICAgICAgICBzdGF0aWMgZ2VuZXJhdGVGaWVsZElkKCkge1xyXG4gICAgICAgICAgICB2YXIgZCA9IG5ldyBEYXRlKCkuZ2V0VGltZSgpO1xyXG4gICAgICAgICAgICB2YXIgdXVpZCA9IFwieHh4eHh4eHgteHh4eC00eHh4LXl4eHgteHh4eHh4eHh4eHh4XCIucmVwbGFjZSgvW3h5XS9nLCBjID0+IHtcclxuICAgICAgICAgICAgICAgIHZhciByID0gKGQgKyBNYXRoLnJhbmRvbSgpICogMTYpICUgMTYgfCAwO1xyXG4gICAgICAgICAgICAgICAgZCA9IE1hdGguZmxvb3IoZCAvIDE2KTtcclxuICAgICAgICAgICAgICAgIHJldHVybiAoYyA9PT0gXCJ4XCIgPyByIDogKHIgJiAweDcgfCAweDgpKS50b1N0cmluZygxNik7XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICByZXR1cm4gXCJfXCIgKyB1dWlkO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBleHBvcnQgY2xhc3MgTWF0Y2hMZXZlbCB7XHJcblxyXG4gICAgICAgIG5hbWU6IHN0cmluZztcclxuICAgICAgICBpbmNsdWRlOiBLbm9ja291dE9ic2VydmFibGU8Ym9vbGVhbj47XHJcblxyXG4gICAgICAgIGNvbnN0cnVjdG9yKG9iajogYW55KSB7XHJcbiAgICAgICAgICAgIHRoaXMubmFtZSA9IG9iai5OYW1lO1xyXG4gICAgICAgICAgICB0aGlzLmluY2x1ZGUgPSBrby5vYnNlcnZhYmxlKG9iai5JbmNsdWRlKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgZXhwb3J0IGNsYXNzIFF1YWxpdHlMZXZlbCB7XHJcblxyXG4gICAgICAgIG5hbWU6IHN0cmluZztcclxuICAgICAgICBpbmNsdWRlOiBLbm9ja291dE9ic2VydmFibGU8Ym9vbGVhbj47XHJcblxyXG4gICAgICAgIGNvbnN0cnVjdG9yKG9iajogYW55KSB7XHJcbiAgICAgICAgICAgIHRoaXMubmFtZSA9IG9iai5OYW1lO1xyXG4gICAgICAgICAgICB0aGlzLmluY2x1ZGUgPSBrby5vYnNlcnZhYmxlKG9iai5JbmNsdWRlKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgZXhwb3J0IGNsYXNzIFNvdXJjZSB7XHJcblxyXG4gICAgICAgIG5hbWU6IHN0cmluZztcclxuICAgICAgICBpbmNsdWRlOiBLbm9ja291dE9ic2VydmFibGU8Ym9vbGVhbj47XHJcblxyXG4gICAgICAgIGNvbnN0cnVjdG9yKG9iajogYW55KSB7XHJcbiAgICAgICAgICAgIHRoaXMubmFtZSA9IG9iai5OYW1lO1xyXG4gICAgICAgICAgICB0aGlzLmluY2x1ZGUgPSBrby5vYnNlcnZhYmxlKG9iai5JbmNsdWRlKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgZXhwb3J0IGNsYXNzIEZpZWxkR3JvdXAge1xyXG5cclxuICAgICAgICBwcml2YXRlIHN0YXRpYyBhZGRyZXNzRmllbGRzID0gW0ZpZWxkTmFtZS5TdHJlZXRBZGRyZXNzLCBGaWVsZE5hbWUuQ2l0eSwgRmllbGROYW1lLlN0YXRlLCBGaWVsZE5hbWUuUG9zdGFsQ29kZV07XHJcbiAgICAgICAgcHJpdmF0ZSBzdGF0aWMgbmNvYUFkZHJlc3NGaWVsZHMgPSBbRmllbGROYW1lLlN0YW5kYXJkaXplZEFkZHJlc3MsIEZpZWxkTmFtZS5TdGFuZGFyZGl6ZWRDaXR5LCBGaWVsZE5hbWUuU3RhbmRhcmRpemVkQ2l0eSwgRmllbGROYW1lLlN0YW5kYXJkaXplZFN0YXRlLCBGaWVsZE5hbWUuU3RhbmRhcmRpemVkWmlwLCBGaWVsZE5hbWUuU3RhbmRhcmRpemVkQWRkcmVzc1JhbmdlLCBGaWVsZE5hbWUuU3RhbmRhcmRpemVkU3RyZWV0TmFtZV07XHJcbiAgICAgICAgcHJpdmF0ZSBzdGF0aWMgbmFtZUZpZWxkcyA9IFtGaWVsZE5hbWUuRmlyc3ROYW1lLCBGaWVsZE5hbWUuTGFzdE5hbWVdO1xyXG4gICAgICAgIHByaXZhdGUgc3RhdGljIHBob25lRmllbGRzID0gW0ZpZWxkTmFtZS5QaG9uZU51bWJlcl07XHJcbiAgICAgICAgcHJpdmF0ZSBzdGF0aWMgYnVzaW5lc3NOYW1lRmllbGRzID0gW0ZpZWxkTmFtZS5CdXNpbmVzc05hbWVdO1xyXG5cclxuICAgICAgICAvLyByZXR1cm5zIEZpZWxkTmFtZXMgZm9yIGEgc3BlY2lmaWMgRmllbGRHcm91cFxyXG4gICAgICAgIHN0YXRpYyBnZXRGaWVsZHMobmFtZTogRmllbGRHcm91cE5hbWUpIHtcclxuICAgICAgICAgICAgc3dpdGNoIChuYW1lKSB7XHJcbiAgICAgICAgICAgIGNhc2UgRmllbGRHcm91cE5hbWUuQWRkcmVzczpcclxuICAgICAgICAgICAgICAgIHJldHVybiBGaWVsZEdyb3VwLmFkZHJlc3NGaWVsZHM7XHJcbiAgICAgICAgICAgIGNhc2UgRmllbGRHcm91cE5hbWUuTmFtZTpcclxuICAgICAgICAgICAgICAgIHJldHVybiBGaWVsZEdyb3VwLm5hbWVGaWVsZHM7XHJcbiAgICAgICAgICAgIGNhc2UgRmllbGRHcm91cE5hbWUuUGhvbmU6XHJcbiAgICAgICAgICAgICAgICByZXR1cm4gRmllbGRHcm91cC5waG9uZUZpZWxkcztcclxuICAgICAgICAgICAgY2FzZSBGaWVsZEdyb3VwTmFtZS5CdXNpbmVzc05hbWU6XHJcbiAgICAgICAgICAgICAgICByZXR1cm4gRmllbGRHcm91cC5idXNpbmVzc05hbWVGaWVsZHM7XHJcbiAgICAgICAgICAgIGRlZmF1bHQ6XHJcbiAgICAgICAgICAgICAgICB0aHJvdyBuZXcgUmVmZXJlbmNlRXJyb3IoXCJFcnJvcjogTm8gZmllbGRzIGV4aXN0IGZvciBcIiArIEZpZWxkR3JvdXBOYW1lW25hbWVdKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8gcmV0dXJucyB0cnVlIGlmIHRoZSBvdXRwdXQgZmllbGQgY29sbGVjdGlvbiBvZiBhbiBvcGVyYXRpb24gY29udGFpbnMgdGhlIGZpZWxkcyBuZWVkZWQgdG8gbWFwIHRvIGEgZ3JvdXBcclxuICAgICAgICAvLyB2YWxpZGF0ZSBlYWNoIG1lbWJlciBhbmQgZW50aXJlIGZ1bmNpdG9uIHJldHVybiBmYWxzZSBpZiBhbnkgYXJyYXkgaXMgbm90IHN1cHBvcnRlZFxyXG4gICAgICAgIHN0YXRpYyBpc1N1cHBvcnRlZChuYW1lOiBGaWVsZEdyb3VwTmFtZSwgZmllbGRzOiBBcnJheTxGaWVsZD4pIHtcclxuICAgICAgICAgICAgaWYgKGZpZWxkcy5sZW5ndGggPT09IDApIHJldHVybiBmYWxzZTtcclxuICAgICAgICAgICAgc3dpdGNoIChuYW1lKSB7XHJcbiAgICAgICAgICAgIGNhc2UgRmllbGRHcm91cE5hbWUuQWRkcmVzczpcclxuICAgICAgICAgICAgICAgIHJldHVybiBfLmludGVyc2VjdGlvbihfLnBsdWNrKGZpZWxkcywgXCJuYW1lXCIpLCBGaWVsZEdyb3VwLmFkZHJlc3NGaWVsZHMpLmxlbmd0aCA9PT0gRmllbGRHcm91cC5hZGRyZXNzRmllbGRzLmxlbmd0aDtcclxuICAgICAgICAgICAgY2FzZSBGaWVsZEdyb3VwTmFtZS5OYW1lOlxyXG4gICAgICAgICAgICAgICAgcmV0dXJuIF8uaW50ZXJzZWN0aW9uKF8ucGx1Y2soZmllbGRzLCBcIm5hbWVcIiksIEZpZWxkR3JvdXAubmFtZUZpZWxkcykubGVuZ3RoID09PSBGaWVsZEdyb3VwLm5hbWVGaWVsZHMubGVuZ3RoO1xyXG4gICAgICAgICAgICBjYXNlIEZpZWxkR3JvdXBOYW1lLlBob25lOlxyXG4gICAgICAgICAgICAgICAgcmV0dXJuIF8uaW50ZXJzZWN0aW9uKF8ucGx1Y2soZmllbGRzLCBcIm5hbWVcIiksIEZpZWxkR3JvdXAucGhvbmVGaWVsZHMpLmxlbmd0aCA9PT0gRmllbGRHcm91cC5waG9uZUZpZWxkcy5sZW5ndGg7XHJcbiAgICAgICAgICAgIGNhc2UgRmllbGRHcm91cE5hbWUuQnVzaW5lc3NOYW1lOlxyXG4gICAgICAgICAgICAgICAgcmV0dXJuIF8uaW50ZXJzZWN0aW9uKF8ucGx1Y2soZmllbGRzLCBcIm5hbWVcIiksIEZpZWxkR3JvdXAuYnVzaW5lc3NOYW1lRmllbGRzKS5sZW5ndGggPT09IEZpZWxkR3JvdXAuYnVzaW5lc3NOYW1lRmllbGRzLmxlbmd0aDtcclxuICAgICAgICAgICAgZGVmYXVsdDpcclxuICAgICAgICAgICAgICAgIHJldHVybiBmYWxzZTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuICAgIH1cclxufSJdfQ==