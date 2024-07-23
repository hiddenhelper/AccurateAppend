module AccurateAppend.DynamicAppend {
    
    // target = operation being mapped
    // operations = global collection of operations not including the target
    export interface IMappingStrategy {
        execute(target: Operation, operations: Array<Operation>): void;
    }
    
    export class DedupePhoneStrategy implements IMappingStrategy {
        execute(target: Operation, operations: Array<Operation>): void {
            // validate, return error message if strategy can't be completed
            // there need to be at least two operations with a phonenumber in their output
            var phoneOperations = _.filter(operations, operation => FieldGroup.isSupported(FieldGroupName.Phone, operation.outputfields) && !operation.isUtil());
            if(phoneOperations.length < 2) throw new Error("Unable to add this operation. 2 or more operations that output phone number must proceed.");
            // map 
            var index = 1;
            $.each(phoneOperations, (i, o) => {
                var phoneDedupeField = <FieldName>FieldName["PhoneNumber" + index];
                Operation.mapField(o, FieldName.PhoneNumber, target, phoneDedupeField);
                index += 1;
            });
            // remove unmapped input fields from dedupe operation
            var fields = target.inputfields;
            for (var i = fields.length - 1; i >= 0; i--) {
                if (fields[i].metafieldName.indexOf("_") === -1) fields.splice(i, 1);
            }
        }
    }

    export class SetPrefPhoneStrategy implements IMappingStrategy {
        execute(target: Operation, operations: Array<Operation>): void {
            // validate, return error message if strategy can't be completed
            // there need to be at least two operations with a phonenumber in their output
            var phoneOperations = _.filter(operations, operation =>
                FieldGroup.isSupported(FieldGroupName.Phone, operation.outputfields) && !operation.isUtil()
            );
            if (phoneOperations.length < 2) throw new Error("Unable to add this operation. 2 or more operations that output phone number must proceed.");

            var index = 1;
            $.each(phoneOperations, (i, o) => {
                // map the output of Phone operations to the next available input of the dedupe operation
                var phoneDedupeField = <FieldName>FieldName["PhoneNumber" + index];
                Operation.mapField(o, FieldName.PhoneNumber, target, phoneDedupeField);
                index += 1;
            });
            // remove unmapped input fields
            var fields = target.inputfields;
            for (var i = fields.length - 1; i >= 0; i--) {
                if (fields[i].metafieldName.indexOf("_") === -1) fields.splice(i, 1);
            }
        }
    }

    export class SetPrefPhoneCompareInputStrategy implements IMappingStrategy {
        execute(target: Operation, operations: Array<Operation>): void {
            // validate, return error message if strategy can't be completed
            var phoneOperations = _.filter(operations, operation =>
                FieldGroup.isSupported(FieldGroupName.Phone, operation.outputfields) && !operation.isUtil()
            );
            
            // validation rules
            if (phoneOperations.length < 1) throw new Error("Unable to add this operation. Operation requires at least 1 phone operation.");
            if (phoneOperations.length > 5) throw new Error("Unable to add this operation. Operation is limited to 5 or fewer output phone numbers.");

            var index = 1;
            $.each(phoneOperations, (i, o) => {
                // map the output of Phone operations to the next available input of the dedupe operation
                var phoneDedupeField = <FieldName>FieldName["PhoneNumber" + index];
                Operation.mapField(o, FieldName.PhoneNumber, target, phoneDedupeField);
                index += 1;
            });
            // remove unmapped input fields
            var fields = target.inputfields;
            for (var i = fields.length - 1; i >= 0; i--) {
                if (fields[i].metafieldName.indexOf("_") === -1 && fields[i].metafieldName !== "PhoneNumber") fields.splice(i, 1);
            }
        }
    }

    export class SetPrefEmailVerStrategy implements IMappingStrategy {
        execute(target: Operation, operations: Array<Operation>): void {
            // EMAIL_VER_DELIVERABLE needs to be in the operations collection
            var emailVerificationOperation = _.filter(operations, operation => operation.name === OperationName.EMAIL_VER_DELIVERABLE );
            if (emailVerificationOperation.length !== 1) throw new Error("Unable to add this operation. Operation requires email verification.");
            
            var index = 1;
            $.each(emailVerificationOperation, (i, o) => {
                // map outputs of EMAIL_VER_DELIVERABLE to this operation SET_PREF_EMAIL_VER
                Operation.mapField(o, FieldName.IsValid, target, FieldName.IsValid);
                Operation.mapField(o, FieldName.StatusCode, target, FieldName.StatusCode);
                index += 1;
                // uncheck outputfields
                Manifest.toggleOutputfields(false, o);
            });
        }
    }
    export class SetPrefEmailVerStrategyRemove implements IMappingStrategy {
        execute(target: Operation, operations: Array<Operation>): void {
            // find email verification operation this was mapped to, recheck it's outputs
            var emailVerificationOperation = _.first(_.filter(operations, operation => operation.name === OperationName.EMAIL_VER_DELIVERABLE));
            Manifest.toggleOutputfields(true, emailVerificationOperation);
        }
    }

    export class SetPrefBasedOnVerificationStrategy implements IMappingStrategy {
        execute(target: Operation, operations: Array<Operation>): void {
            // validate, return error message if strategy can't be completed
            var phoneOperations = _.filter(operations, operation =>
                FieldGroup.isSupported(FieldGroupName.Phone, operation.outputfields) && !operation.isUtil()
            );
            // validation rules
            // ensure PHONE_VER has been selected
            var phoneVerPresent = _.find(operations, operation => operation.name === OperationName.PHONE_VER);
            if (!phoneVerPresent) throw new Error("Unable to add this operation. Verify Connection Status (C1-C7) must be selected first.");
            if (phoneOperations.length < 1) throw new Error("Unable to add this operation. Operation requires at least 1 phone operation.");
            if (phoneOperations.length > 5) throw new Error("Unable to add this operation. Operation is limited to 5 or fewer output phone numbers.");

            // map PhoneVerificationStatus in PHONE_VER outputfields to dedupe operation
            var phoneVerOperation = _.first(_.filter(operations, operation => operation.name === OperationName.PHONE_VER ));
            Operation.mapField(phoneVerOperation, FieldName.PhoneVerificationStatus, target, FieldName.PhoneVerificationStatus);
            
            var index = 1;
            $.each(phoneOperations, (i, o) => {
                // map the output of Phone operations to the next available input of the dedupe operation
                var phoneDedupeField = <FieldName>FieldName["PhoneNumber" + index];
                Operation.mapField(o, FieldName.PhoneNumber, target, phoneDedupeField);
                index += 1;
            });
            // remove unmapped input fields
            var fields = target.inputfields;
            for (var i = fields.length - 1; i >= 0; i--) {
                if (fields[i].metafieldName.indexOf("_") === -1 && fields[i].metafieldName !== "PhoneNumber") fields.splice(i, 1);
            }
        }
    }

    export class SetPrefPhoneSingleColumnStrategy implements IMappingStrategy {
        execute(target: Operation, operations: Array<Operation>): void {
            // validate, return error message if stratgey can't be completed
            // there need to be at least two operations with a phonenumber in their output
            var phoneOperations = _.filter(operations, operation => FieldGroup.isSupported(FieldGroupName.Phone, operation.outputfields) && !operation.isUtil());
            if (phoneOperations.length < 2) throw new Error("Unable to add this operation. 2 or more operations that output phone number must preceed.");
            var index = 1;
            $.each(phoneOperations, (i, o) => {
                // map the output of Phone oeprations to the next available input of the dedupe operation
                var phoneDedupeField = <FieldName>FieldName["PhoneNumber" + index];
                Operation.mapField(o, FieldName.PhoneNumber, target, phoneDedupeField);
                index += 1;
                // dis-include outputfields in oeprations containing PhoneNumber
                Manifest.toggleOutputfields(false, o);
            });
            // rmeove unmapped input fields
            var fields = target.inputfields;
            for (var i = fields.length - 1; i >= 0; i--) {
                if (fields[i].metafieldName.indexOf("_") === -1) fields.splice(i, 1);
            }
        }
    }
    export class SetPrefPhoneSingleColumnStrategyRemove implements IMappingStrategy {
        execute(target: Operation, operations: Array<Operation>): void {
            // find phone operations this operation was mapped to
            $.each(operations, (i, o) => {
                if (o.isMapped(target)) Manifest.toggleOutputfields(true, o);
            });
        }
    }
    
    export class Ncoa48Strategy implements IMappingStrategy {
        execute(target: Operation, operations: Array<Operation>): void {
            $.each(operations, (i, o) => {
                if (o.isUtil()) return true;
                console.log("Mapping " + OperationName[o.name]);
                //if (i === 0) return true;
                // find operations whose input supports 'Address' and map thier inputs to NCOA output
                if (FieldGroup.isSupported(FieldGroupName.Address, o.inputfields)) {
                    $.each(FieldGroup.getFields(FieldGroupName.Address), (i, fieldNameToMap) => {
                        Operation.mapField(target, fieldNameToMap, o, fieldNameToMap);
                    });
                }
            });
        }
    }
    export class Ncoa48StrategyRemove implements IMappingStrategy {
        execute(target: Operation, operations: Array<Operation>): void {
            // find Operations that were mapped to this 
            $.each(operations, (i, o) => {
                if (o.isUtil()) return true;
                if (o.name === OperationName.NCOA48) return true;
                // find operations whose input supports 'Address', these were maped to NCOA
                if (FieldGroup.isSupported(FieldGroupName.Address, o.inputfields)) {
                    var addressFields = FieldGroup.getFields(FieldGroupName.Address);
                    $.each(_.filter(o.inputfields, o => (_.indexOf(addressFields, o.name) !== -1)), (i, f) => {
                        console.log(OperationName[o.name] + ": unmapping " + f.metafieldName + " to " + FieldName[f.name]);
                        f.metafieldName = FieldName[f.name];
                    });
                }
            });
        }
    }

    export class SetPrefAddressSingleColumnStrategy implements IMappingStrategy {
        execute(target: Operation, operations: Array<Operation>): void {
            // validate, return error message if stratgey can't be completed
            // there need to be at least two operations with a phonenumber in their output
            var phoneOperations = _.filter(operations, operation => FieldGroup.isSupported(FieldGroupName.Address, operation.outputfields) && !operation.isUtil());
            if (phoneOperations.length < 2) throw new Error("Unable to add this operation. 2 or more operations that output phone number must precede.");
            var index = 1;
            $.each(phoneOperations, (i, o) => {
                // map the output of Phone oeprations to the next available input of the dedupe operation
                var dedupeField = <FieldName>FieldName["StreetAddress" + index];
                Operation.mapField(o, FieldName.StreetAddress, target, dedupeField);
                index += 1;
                // dis-include outputfields in oeprations containing PhoneNumber
                Manifest.toggleOutputfields(false, o);
            });
            // rmeove unmapped input fields
            var fields = target.inputfields;
            for (var i = fields.length - 1; i >= 0; i--) {
                if (fields[i].metafieldName.indexOf("_") === -1) fields.splice(i, 1);
            }
        }
    }
    export class SetPrefAddressSingleColumnStrategyRemove implements IMappingStrategy {
        execute(target: Operation, operations: Array<Operation>): void {
            // find phone operations this operation was mapped to
            $.each(operations, (i, o) => {
                if (o.isMapped(target)) Manifest.toggleOutputfields(true, o);
            });
        }
    }
}