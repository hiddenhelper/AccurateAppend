module AccurateAppend.DynamicAppend {

    export class Manifest {

        // global Operations
        operations: KnockoutObservableArray<Operation>;
        supressionid: KnockoutObservable<string>;

        constructor() {
            this.operations = ko.observableArray<Operation>();
            this.supressionid = ko.observable("");
        }

        // required inputfieldnames
        get inputfieldnames(): Array<string> {
            // return all fields in operations where include == true
            var fields = new Array<string>();
            $.each(this.operations(), (i, operation) => {
                $.each(_.filter(operation.inputfields, (o) => (o.metafieldName.indexOf("_") === -1)), (h, field) => {
                    fields.push(field.metafieldName);
                });
            });
            return _.uniq(fields);
        }
        // global outputfields
        get outputfields(): Array<Field> {
            // return all fields in operations where include == true
            var fields = new Array<Field>();
            $.each(this.operations(), (i, operation) => {
                $.each(_.filter(operation.outputfields, (o) => (o.include())), (h, field) => {
                    fields.push(field);
                });
            });
            return fields;
        }
        
        // Adds an Operation using a Strategy
        add(name: string, successCallback: Function, errorCallback: Function) {
            var self = this;
            $.ajax(
                {
                    type: "GET",
                    url: "/Batch/GetOperationDefintion",
                    data: { name: name },
                    success: data => {
                        // create new Operation from Definitions
                        var operation = new Operation(data);
                        // some operations require special mapping 
                        var e;
                        switch (operation.name) {
                            case OperationName.DEDUPE_PHONE:
                                try {
                                    this.map(new DedupePhoneStrategy(), operation, this.operations());
                                    this.operations.push(operation);
                                    successCallback();
                                } catch (e) {
                                    errorCallback(e);
                                }
                                break;
                            case OperationName.SET_PREF_PHONE:
                                try {
                                    this.map(new SetPrefPhoneStrategy(), operation, this.operations());
                                    this.operations.push(operation);
                                    successCallback();
                                } catch (e) {
                                    errorCallback(e);
                                }
                                break;
                            case OperationName.SET_PREF_PHONE_SINGLE_COLUMN:
                                try {
                                    this.map(new SetPrefPhoneSingleColumnStrategy(), operation, this.operations());
                                    this.operations.push(operation);
                                    successCallback();
                                } catch (e) {
                                    errorCallback(e);
                                }
                                break;
                            case OperationName.SET_PREF_ADDRESS_SINGLE_COLUMN:
                                try {
                                    this.map(new SetPrefAddressSingleColumnStrategy(), operation, this.operations());
                                    this.operations.push(operation);
                                    successCallback();
                                } catch (e) {
                                    errorCallback(e);
                                }
                                break;
                            case OperationName.SET_PREF_PHONE_COMPARE_INPUT:
                                try {
                                    this.map(new SetPrefPhoneCompareInputStrategy(), operation, this.operations());
                                    this.operations.push(operation);
                                    successCallback();
                                } catch (e) {
                                    errorCallback(e);
                                }
                                break;
                            case OperationName.SET_PREF_BASED_ON_VERIFICATION:
                                try {
                                    this.map(new SetPrefBasedOnVerificationStrategy(), operation, this.operations());
                                    this.operations.push(operation);
                                    successCallback();
                                } catch (e) {
                                    errorCallback(e);
                                }
                                break;
                            case OperationName.SET_PREF_EMAIL_VER:
                                try {
                                    this.map(new SetPrefEmailVerStrategy(), operation, this.operations());
                                    this.operations.push(operation);
                                    successCallback();
                                } catch (e) {
                                    errorCallback(e);
                                }
                                break;
                            case OperationName.NCOA48:
                                this.map(new Ncoa48Strategy(), operation, this.operations());
                                this.operations.unshift(operation);
                                break;
                            default:
                                // if NCOA is present and this operation has Address input then map it's inputs to NCOA
                                if (_.contains(_.map(this.operations(), v => v.name), OperationName.NCOA48)) {
                                    console.log("NCOA detected");
                                    this.map(new Ncoa48StrategyRemove(), operation, this.operations());
                                    this.operations.push(operation);
                                    this.map(new Ncoa48Strategy(), operation, this.operations());
                                } else {
                                    this.operations.push(operation);
                                }
                                break;
                        }
                    },
                    error: (xhr, status, error) => {
                        alert("Unable to get defintion for " + name);
                    }
                });
        }

        // Removes an Operation
        remove(name: string) {
            console.log("Removing " + name);
            // determine if oepration is in collection before removal
            var operation = _.first(_.filter(this.operations(), (o) => (o.name === <OperationName>OperationName[<string>name])));
            if (!operation) return;
            switch (operation.name) {
                case OperationName.SET_PREF_PHONE_SINGLE_COLUMN:
                    this.map(new SetPrefPhoneSingleColumnStrategyRemove(), operation, this.operations());
                    break;
                case OperationName.SET_PREF_EMAIL_VER:
                    this.map(new SetPrefEmailVerStrategyRemove(), operation, this.operations());
                    break;
                case OperationName.NCOA48:
                    this.map(new Ncoa48StrategyRemove(), operation, this.operations());
                    break;
            }
            this.operations.remove(operation);
        }

        // Builds JSON using mapped objects that exactly match legacy objects 
        toJson() {

            // set SequenceId
            $.each(this.operations(), (i, o) => {
                o.sequnceid = i;
                $.each(o.inputfields, (j, f) => {
                    f.sequnceid = o.sequnceid;
                });
                $.each(o.outputfields, (j, f) => {
                    f.sequnceid = o.sequnceid;
                });
            });

            // build mapped version of Manifest that exactly matches manifest expected by controller
            return ko.toJSON({
                "Operations": ko.utils.arrayMap(this.operations(), o => {
                    return {
                        SequenceId: o.sequnceid,
                        Name: OperationName[o.name],
                        MatchLevels: ko.utils.arrayMap(o.matchlevels, m => {
                            return {
                                Name: m.name,
                                Include: m.include
                            }
                        }),
                        QualityLevels: ko.utils.arrayMap(o.qualitylevels, m => {
                            return {
                                Name: m.name,
                                Include: m.include
                            }
                        }),
                        Sources: ko.utils.arrayMap(o.sources, m => {
                            return {
                                Name: m.name,
                                Include: m.include
                            }
                        }),
                        InputFields: ko.utils.arrayMap(o.inputfields, f => {
                            return {
                                MetaFieldName: f.metafieldName,
                                OperationParamName: f.operationparamname,
                                SequenceId: f.sequnceid,
                                Include: f.include,
                                Required: f.required
                            }
                        }),
                        OutputFields: ko.utils.arrayMap(o.outputfields, f => {
                            return {
                                ColumnTitle: f.columntitle,
                                MetaFieldName: f.metafieldName,
                                OperationParamName: f.operationparamname,
                                SequenceId: f.sequnceid,
                                Include: f.include,
                                Required: f.required
                            }
                        }),
                    }
                }),
                "OutputFields": ko.utils.arrayMap(this.outputfields, f => {
                    return {
                        xPath: f.xpath,
                        ColumnTitle: f.columntitle,
                        MetaFieldName: f.metafieldName,
                        OperationParamName: f.operationparamname,
                        SequenceId: f.sequnceid,
                        Include: f.include,
                        Required: f.required
                    }
                })
            });
        }

        // set "include" attribute in "outputfields" to true/false for given Operation
        static toggleOutputfields = (include: boolean, ...operations: Array<Operation>) => {
            $.each(operations, (i, operation) => {
                $.each(operation.outputfields, (h, field) => {
                    field.include(include);
                });
            });
        }

        // maps target Operation to global Operation collection
        private map(strategy: IMappingStrategy, target: Operation, operations: Array<Operation>) {
            return strategy.execute(target, operations);
        }
    }

    export class Operation {
        
        sequnceid: number;
        title: string;
        description: string;
        name: OperationName;
        matchlevels: Array<MatchLevel>;
        qualitylevels: Array<QualityLevel>;
        inputfields: Array<Field>;
        outputfields: Array<Field>;
        sources: Array<Source>;

        constructor(obj: any) {
            var self = this;
            self.title = obj.Title;
            self.description = obj.Description;
            self.name = <OperationName>OperationName[<string>obj.Name];
            self.matchlevels = new Array<MatchLevel>();
            self.qualitylevels = new Array<MatchLevel>();
            self.inputfields = Array<Field>();
            self.outputfields = Array<Field>();
            self.sources = Array<Source>();
            $.each(obj.MatchLevels, (i, matchlevel) => { self.matchlevels.push(new MatchLevel(matchlevel)) });
            $.each(obj.QualityLevels, (i, qualitylevel) => { self.qualitylevels.push(new QualityLevel(qualitylevel)) });
            $.each(obj.InputFields, (i, inputfield) => { self.inputfields.push(new Field(inputfield, self.name, null)) });
            $.each(obj.Sources, (i, source) => { self.sources.push(new Source(source)) });
            var previous: Field;
            $.each(obj.OutputFields, (i, outputfield) => {
                var field: Field;
                // MatchLevel and QualityLevel fields require xpath with attribute and must have the same metafieldname as it's parent field
                switch (outputfield.MetaFieldName) {
                case "MatchLevel":
                case "QualityLevel":
                        // MatchLevel and QualityLevel need to use parent field name
                        field = new Field(outputfield, self.name, previous.metafieldName);
                    break;
                default:
                    field = new Field(outputfield, self.name, Field.generateFieldId());
                    // store the previous field, used to add MatchLevel or QualityLevel above
                    previous = (outputfield.name !== FieldName.MatchLevel && outputfield.name !== FieldName.QualityLevel) ? field : null;
                }
                self.outputfields.push(field);
            });
        }

        // determines if the this operation is mapped to the input operation
        isMapped(operation: Operation) {
            return _.filter(Operation.getMetafieldnames(this.outputfields), i => ($.inArray(i, Operation.getMetafieldnames(operation.inputfields)) > -1)).length > 0;
        }

        // identifies utility operations
        isUtil() {
            switch (this.name) {
            case OperationName.DEDUPE_PHONE:
            case OperationName.SET_PREF_PHONE:
            case OperationName.SET_PREF_PHONE_SINGLE_COLUMN:
            case OperationName.SET_PREF_ADDRESS_SINGLE_COLUMN:
                return true;
            default:
                return false;
            }
        }

        // maps field in 'from' to field in 'to'
        static mapField(from: Operation, fromOperationfieldName: FieldName, to: Operation, tofieldName: FieldName) {
            // get matching field from operation.outputfields
            var fromField = _.find(from.outputfields, o => (o.name === fromOperationfieldName));
            if (!fromField) console.log("Unable to locate 'from:'" + FieldName[fromOperationfieldName] + " field in " + OperationName[from.name]);
            // get matchingfield from this.inputfields
            var toField = _.find(to.inputfields, o => (o.name === tofieldName));
            if (!toField) console.log("Unable to locate 'to':" + FieldName[fromOperationfieldName] + " field in " + OperationName[to.name]);
            // set to metafieldname to from metafieldname
            toField.metafieldName = Field.generateFieldId();
            toField.metafieldName = fromField.metafieldName;
        }

        // get metafieldnames for a collection of Field
        static getMetafieldnames(fields: Array<Field>) {
            return _.map(fields, field => field.metafieldName);
        }
    }

    export class Field {

        sequnceid: number;
        name: FieldName; // allows us to standarize names regardless of the name in the manifest, needed to work with FieldGroups
        operationparamname: string;
        columntitle: KnockoutObservable<string>;
        required: boolean;
        include: KnockoutObservable<boolean>;
        color: KnockoutObservable<string>;
        private _metafieldname: KnockoutObservable<string>;
        
        constructor(obj: any, operationName: OperationName, fieldId: string) {
            var name = <FieldName>FieldName[<string>obj.MetaFieldName];
            this.name = this.standardizeFieldName(name, operationName);

            this.operationparamname = obj.OperationParamName;
            this.columntitle = ko.observable(obj.ColumnTitle);
            this.required = obj.Required;
            this.include = ko.observable(obj.Include);
            this._metafieldname = ko.observable(fieldId || obj.MetaFieldName);
            this.color = ko.observable("black");
        }

        set metafieldName(value: string) {
            this._metafieldname(value);
        }

        get metafieldName(): string {
            switch (this.name) {
            case FieldName.MatchLevel:
            case FieldName.QualityLevel:
                return this._metafieldname() + "_" + FieldName[this.name];
            default:
                return this._metafieldname();
            }
        }

        get xpath(): string {
            switch (this.name) {
            case FieldName.MatchLevel:
            case FieldName.QualityLevel:
                return this._metafieldname() + "/@" + FieldName[this.name];
            default:
                return this._metafieldname();
            }
        }

        // some fields referenced in the incoming Manifest definition contain address components but are named in a non-standard way like ParsedAddress, ParsedCity
        // this method standardizes the naming so we can operate on groups of fields using FieldGroup
        standardizeFieldName(name: FieldName, operationName: OperationName) {
            switch (operationName) {
            case OperationName.NCOA48:
                switch (name) {
                case FieldName.StandardizedAddress:
                    return FieldName.StreetAddress;
                case FieldName.StandardizedCity:
                    return FieldName.City;
                case FieldName.StandardizedState:
                    return FieldName.State;
                case FieldName.StandardizedZip:
                    return FieldName.PostalCode;
                }
            default:
                return name;
            }
        }

        // generates a unique field name
        static generateFieldId() {
            var d = new Date().getTime();
            var uuid = "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, c => {
                var r = (d + Math.random() * 16) % 16 | 0;
                d = Math.floor(d / 16);
                return (c === "x" ? r : (r & 0x7 | 0x8)).toString(16);
            });
            return "_" + uuid;
        }
    }

    export class MatchLevel {

        name: string;
        include: KnockoutObservable<boolean>;

        constructor(obj: any) {
            this.name = obj.Name;
            this.include = ko.observable(obj.Include);
        }
    }

    export class QualityLevel {

        name: string;
        include: KnockoutObservable<boolean>;

        constructor(obj: any) {
            this.name = obj.Name;
            this.include = ko.observable(obj.Include);
        }
    }

    export class Source {

        name: string;
        include: KnockoutObservable<boolean>;

        constructor(obj: any) {
            this.name = obj.Name;
            this.include = ko.observable(obj.Include);
        }
    }

    export class FieldGroup {

        private static addressFields = [FieldName.StreetAddress, FieldName.City, FieldName.State, FieldName.PostalCode];
        private static ncoaAddressFields = [FieldName.StandardizedAddress, FieldName.StandardizedCity, FieldName.StandardizedCity, FieldName.StandardizedState, FieldName.StandardizedZip, FieldName.StandardizedAddressRange, FieldName.StandardizedStreetName];
        private static nameFields = [FieldName.FirstName, FieldName.LastName];
        private static phoneFields = [FieldName.PhoneNumber];
        private static businessNameFields = [FieldName.BusinessName];

        // returns FieldNames for a specific FieldGroup
        static getFields(name: FieldGroupName) {
            switch (name) {
            case FieldGroupName.Address:
                return FieldGroup.addressFields;
            case FieldGroupName.Name:
                return FieldGroup.nameFields;
            case FieldGroupName.Phone:
                return FieldGroup.phoneFields;
            case FieldGroupName.BusinessName:
                return FieldGroup.businessNameFields;
            default:
                throw new ReferenceError("Error: No fields exist for " + FieldGroupName[name]);
            }
        }

        // returns true if the output field collection of an operation contains the fields needed to map to a group
        // validate each member and entire funciton return false if any array is not supported
        static isSupported(name: FieldGroupName, fields: Array<Field>) {
            if (fields.length === 0) return false;
            switch (name) {
            case FieldGroupName.Address:
                return _.intersection(_.pluck(fields, "name"), FieldGroup.addressFields).length === FieldGroup.addressFields.length;
            case FieldGroupName.Name:
                return _.intersection(_.pluck(fields, "name"), FieldGroup.nameFields).length === FieldGroup.nameFields.length;
            case FieldGroupName.Phone:
                return _.intersection(_.pluck(fields, "name"), FieldGroup.phoneFields).length === FieldGroup.phoneFields.length;
            case FieldGroupName.BusinessName:
                return _.intersection(_.pluck(fields, "name"), FieldGroup.businessNameFields).length === FieldGroup.businessNameFields.length;
            default:
                return false;
            }
        }
    }
}