var AccurateAppend;
(function (AccurateAppend) {
    var DynamicAppend;
    (function (DynamicAppend) {
        var DedupePhoneStrategy = (function () {
            function DedupePhoneStrategy() {
            }
            DedupePhoneStrategy.prototype.execute = function (target, operations) {
                var phoneOperations = _.filter(operations, function (operation) { return DynamicAppend.FieldGroup.isSupported(DynamicAppend.FieldGroupName.Phone, operation.outputfields) && !operation.isUtil(); });
                if (phoneOperations.length < 2)
                    throw new Error("Unable to add this operation. 2 or more operations that output phone number must proceed.");
                var index = 1;
                $.each(phoneOperations, function (i, o) {
                    var phoneDedupeField = DynamicAppend.FieldName["PhoneNumber" + index];
                    DynamicAppend.Operation.mapField(o, DynamicAppend.FieldName.PhoneNumber, target, phoneDedupeField);
                    index += 1;
                });
                var fields = target.inputfields;
                for (var i = fields.length - 1; i >= 0; i--) {
                    if (fields[i].metafieldName.indexOf("_") === -1)
                        fields.splice(i, 1);
                }
            };
            return DedupePhoneStrategy;
        }());
        DynamicAppend.DedupePhoneStrategy = DedupePhoneStrategy;
        var SetPrefPhoneStrategy = (function () {
            function SetPrefPhoneStrategy() {
            }
            SetPrefPhoneStrategy.prototype.execute = function (target, operations) {
                var phoneOperations = _.filter(operations, function (operation) {
                    return DynamicAppend.FieldGroup.isSupported(DynamicAppend.FieldGroupName.Phone, operation.outputfields) && !operation.isUtil();
                });
                if (phoneOperations.length < 2)
                    throw new Error("Unable to add this operation. 2 or more operations that output phone number must proceed.");
                var index = 1;
                $.each(phoneOperations, function (i, o) {
                    var phoneDedupeField = DynamicAppend.FieldName["PhoneNumber" + index];
                    DynamicAppend.Operation.mapField(o, DynamicAppend.FieldName.PhoneNumber, target, phoneDedupeField);
                    index += 1;
                });
                var fields = target.inputfields;
                for (var i = fields.length - 1; i >= 0; i--) {
                    if (fields[i].metafieldName.indexOf("_") === -1)
                        fields.splice(i, 1);
                }
            };
            return SetPrefPhoneStrategy;
        }());
        DynamicAppend.SetPrefPhoneStrategy = SetPrefPhoneStrategy;
        var SetPrefPhoneCompareInputStrategy = (function () {
            function SetPrefPhoneCompareInputStrategy() {
            }
            SetPrefPhoneCompareInputStrategy.prototype.execute = function (target, operations) {
                var phoneOperations = _.filter(operations, function (operation) {
                    return DynamicAppend.FieldGroup.isSupported(DynamicAppend.FieldGroupName.Phone, operation.outputfields) && !operation.isUtil();
                });
                if (phoneOperations.length < 1)
                    throw new Error("Unable to add this operation. Operation requires at least 1 phone operation.");
                if (phoneOperations.length > 5)
                    throw new Error("Unable to add this operation. Operation is limited to 5 or fewer output phone numbers.");
                var index = 1;
                $.each(phoneOperations, function (i, o) {
                    var phoneDedupeField = DynamicAppend.FieldName["PhoneNumber" + index];
                    DynamicAppend.Operation.mapField(o, DynamicAppend.FieldName.PhoneNumber, target, phoneDedupeField);
                    index += 1;
                });
                var fields = target.inputfields;
                for (var i = fields.length - 1; i >= 0; i--) {
                    if (fields[i].metafieldName.indexOf("_") === -1 && fields[i].metafieldName !== "PhoneNumber")
                        fields.splice(i, 1);
                }
            };
            return SetPrefPhoneCompareInputStrategy;
        }());
        DynamicAppend.SetPrefPhoneCompareInputStrategy = SetPrefPhoneCompareInputStrategy;
        var SetPrefEmailVerStrategy = (function () {
            function SetPrefEmailVerStrategy() {
            }
            SetPrefEmailVerStrategy.prototype.execute = function (target, operations) {
                var emailVerificationOperation = _.filter(operations, function (operation) { return operation.name === DynamicAppend.OperationName.EMAIL_VER_DELIVERABLE; });
                if (emailVerificationOperation.length !== 1)
                    throw new Error("Unable to add this operation. Operation requires email verification.");
                var index = 1;
                $.each(emailVerificationOperation, function (i, o) {
                    DynamicAppend.Operation.mapField(o, DynamicAppend.FieldName.IsValid, target, DynamicAppend.FieldName.IsValid);
                    DynamicAppend.Operation.mapField(o, DynamicAppend.FieldName.StatusCode, target, DynamicAppend.FieldName.StatusCode);
                    index += 1;
                    DynamicAppend.Manifest.toggleOutputfields(false, o);
                });
            };
            return SetPrefEmailVerStrategy;
        }());
        DynamicAppend.SetPrefEmailVerStrategy = SetPrefEmailVerStrategy;
        var SetPrefEmailVerStrategyRemove = (function () {
            function SetPrefEmailVerStrategyRemove() {
            }
            SetPrefEmailVerStrategyRemove.prototype.execute = function (target, operations) {
                var emailVerificationOperation = _.first(_.filter(operations, function (operation) { return operation.name === DynamicAppend.OperationName.EMAIL_VER_DELIVERABLE; }));
                DynamicAppend.Manifest.toggleOutputfields(true, emailVerificationOperation);
            };
            return SetPrefEmailVerStrategyRemove;
        }());
        DynamicAppend.SetPrefEmailVerStrategyRemove = SetPrefEmailVerStrategyRemove;
        var SetPrefBasedOnVerificationStrategy = (function () {
            function SetPrefBasedOnVerificationStrategy() {
            }
            SetPrefBasedOnVerificationStrategy.prototype.execute = function (target, operations) {
                var phoneOperations = _.filter(operations, function (operation) {
                    return DynamicAppend.FieldGroup.isSupported(DynamicAppend.FieldGroupName.Phone, operation.outputfields) && !operation.isUtil();
                });
                var phoneVerPresent = _.find(operations, function (operation) { return operation.name === DynamicAppend.OperationName.PHONE_VER; });
                if (!phoneVerPresent)
                    throw new Error("Unable to add this operation. Verify Connection Status (C1-C7) must be selected first.");
                if (phoneOperations.length < 1)
                    throw new Error("Unable to add this operation. Operation requires at least 1 phone operation.");
                if (phoneOperations.length > 5)
                    throw new Error("Unable to add this operation. Operation is limited to 5 or fewer output phone numbers.");
                var phoneVerOperation = _.first(_.filter(operations, function (operation) { return operation.name === DynamicAppend.OperationName.PHONE_VER; }));
                DynamicAppend.Operation.mapField(phoneVerOperation, DynamicAppend.FieldName.PhoneVerificationStatus, target, DynamicAppend.FieldName.PhoneVerificationStatus);
                var index = 1;
                $.each(phoneOperations, function (i, o) {
                    var phoneDedupeField = DynamicAppend.FieldName["PhoneNumber" + index];
                    DynamicAppend.Operation.mapField(o, DynamicAppend.FieldName.PhoneNumber, target, phoneDedupeField);
                    index += 1;
                });
                var fields = target.inputfields;
                for (var i = fields.length - 1; i >= 0; i--) {
                    if (fields[i].metafieldName.indexOf("_") === -1 && fields[i].metafieldName !== "PhoneNumber")
                        fields.splice(i, 1);
                }
            };
            return SetPrefBasedOnVerificationStrategy;
        }());
        DynamicAppend.SetPrefBasedOnVerificationStrategy = SetPrefBasedOnVerificationStrategy;
        var SetPrefPhoneSingleColumnStrategy = (function () {
            function SetPrefPhoneSingleColumnStrategy() {
            }
            SetPrefPhoneSingleColumnStrategy.prototype.execute = function (target, operations) {
                var phoneOperations = _.filter(operations, function (operation) { return DynamicAppend.FieldGroup.isSupported(DynamicAppend.FieldGroupName.Phone, operation.outputfields) && !operation.isUtil(); });
                if (phoneOperations.length < 2)
                    throw new Error("Unable to add this operation. 2 or more operations that output phone number must preceed.");
                var index = 1;
                $.each(phoneOperations, function (i, o) {
                    var phoneDedupeField = DynamicAppend.FieldName["PhoneNumber" + index];
                    DynamicAppend.Operation.mapField(o, DynamicAppend.FieldName.PhoneNumber, target, phoneDedupeField);
                    index += 1;
                    DynamicAppend.Manifest.toggleOutputfields(false, o);
                });
                var fields = target.inputfields;
                for (var i = fields.length - 1; i >= 0; i--) {
                    if (fields[i].metafieldName.indexOf("_") === -1)
                        fields.splice(i, 1);
                }
            };
            return SetPrefPhoneSingleColumnStrategy;
        }());
        DynamicAppend.SetPrefPhoneSingleColumnStrategy = SetPrefPhoneSingleColumnStrategy;
        var SetPrefPhoneSingleColumnStrategyRemove = (function () {
            function SetPrefPhoneSingleColumnStrategyRemove() {
            }
            SetPrefPhoneSingleColumnStrategyRemove.prototype.execute = function (target, operations) {
                $.each(operations, function (i, o) {
                    if (o.isMapped(target))
                        DynamicAppend.Manifest.toggleOutputfields(true, o);
                });
            };
            return SetPrefPhoneSingleColumnStrategyRemove;
        }());
        DynamicAppend.SetPrefPhoneSingleColumnStrategyRemove = SetPrefPhoneSingleColumnStrategyRemove;
        var Ncoa48Strategy = (function () {
            function Ncoa48Strategy() {
            }
            Ncoa48Strategy.prototype.execute = function (target, operations) {
                $.each(operations, function (i, o) {
                    if (o.isUtil())
                        return true;
                    console.log("Mapping " + DynamicAppend.OperationName[o.name]);
                    if (DynamicAppend.FieldGroup.isSupported(DynamicAppend.FieldGroupName.Address, o.inputfields)) {
                        $.each(DynamicAppend.FieldGroup.getFields(DynamicAppend.FieldGroupName.Address), function (i, fieldNameToMap) {
                            DynamicAppend.Operation.mapField(target, fieldNameToMap, o, fieldNameToMap);
                        });
                    }
                });
            };
            return Ncoa48Strategy;
        }());
        DynamicAppend.Ncoa48Strategy = Ncoa48Strategy;
        var Ncoa48StrategyRemove = (function () {
            function Ncoa48StrategyRemove() {
            }
            Ncoa48StrategyRemove.prototype.execute = function (target, operations) {
                $.each(operations, function (i, o) {
                    if (o.isUtil())
                        return true;
                    if (o.name === DynamicAppend.OperationName.NCOA48)
                        return true;
                    if (DynamicAppend.FieldGroup.isSupported(DynamicAppend.FieldGroupName.Address, o.inputfields)) {
                        var addressFields = DynamicAppend.FieldGroup.getFields(DynamicAppend.FieldGroupName.Address);
                        $.each(_.filter(o.inputfields, function (o) { return (_.indexOf(addressFields, o.name) !== -1); }), function (i, f) {
                            console.log(DynamicAppend.OperationName[o.name] + ": unmapping " + f.metafieldName + " to " + DynamicAppend.FieldName[f.name]);
                            f.metafieldName = DynamicAppend.FieldName[f.name];
                        });
                    }
                });
            };
            return Ncoa48StrategyRemove;
        }());
        DynamicAppend.Ncoa48StrategyRemove = Ncoa48StrategyRemove;
        var SetPrefAddressSingleColumnStrategy = (function () {
            function SetPrefAddressSingleColumnStrategy() {
            }
            SetPrefAddressSingleColumnStrategy.prototype.execute = function (target, operations) {
                var phoneOperations = _.filter(operations, function (operation) { return DynamicAppend.FieldGroup.isSupported(DynamicAppend.FieldGroupName.Address, operation.outputfields) && !operation.isUtil(); });
                if (phoneOperations.length < 2)
                    throw new Error("Unable to add this operation. 2 or more operations that output phone number must precede.");
                var index = 1;
                $.each(phoneOperations, function (i, o) {
                    var dedupeField = DynamicAppend.FieldName["StreetAddress" + index];
                    DynamicAppend.Operation.mapField(o, DynamicAppend.FieldName.StreetAddress, target, dedupeField);
                    index += 1;
                    DynamicAppend.Manifest.toggleOutputfields(false, o);
                });
                var fields = target.inputfields;
                for (var i = fields.length - 1; i >= 0; i--) {
                    if (fields[i].metafieldName.indexOf("_") === -1)
                        fields.splice(i, 1);
                }
            };
            return SetPrefAddressSingleColumnStrategy;
        }());
        DynamicAppend.SetPrefAddressSingleColumnStrategy = SetPrefAddressSingleColumnStrategy;
        var SetPrefAddressSingleColumnStrategyRemove = (function () {
            function SetPrefAddressSingleColumnStrategyRemove() {
            }
            SetPrefAddressSingleColumnStrategyRemove.prototype.execute = function (target, operations) {
                $.each(operations, function (i, o) {
                    if (o.isMapped(target))
                        DynamicAppend.Manifest.toggleOutputfields(true, o);
                });
            };
            return SetPrefAddressSingleColumnStrategyRemove;
        }());
        DynamicAppend.SetPrefAddressSingleColumnStrategyRemove = SetPrefAddressSingleColumnStrategyRemove;
    })(DynamicAppend = AccurateAppend.DynamicAppend || (AccurateAppend.DynamicAppend = {}));
})(AccurateAppend || (AccurateAppend = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiQWNjdXJhdGVBcHBlbmQuRHluYW1pY0FwcGVuZC5TdHJhdGVnaWVzLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiQWNjdXJhdGVBcHBlbmQuRHluYW1pY0FwcGVuZC5TdHJhdGVnaWVzLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLElBQU8sY0FBYyxDQXNPcEI7QUF0T0QsV0FBTyxjQUFjO0lBQUMsSUFBQSxhQUFhLENBc09sQztJQXRPcUIsV0FBQSxhQUFhO1FBUS9CO1lBQUE7WUFtQkEsQ0FBQztZQWxCRyxxQ0FBTyxHQUFQLFVBQVEsTUFBaUIsRUFBRSxVQUE0QjtnQkFHbkQsSUFBSSxlQUFlLEdBQUcsQ0FBQyxDQUFDLE1BQU0sQ0FBQyxVQUFVLEVBQUUsVUFBQSxTQUFTLElBQUksT0FBQSxjQUFBLFVBQVUsQ0FBQyxXQUFXLENBQUMsY0FBQSxjQUFjLENBQUMsS0FBSyxFQUFFLFNBQVMsQ0FBQyxZQUFZLENBQUMsSUFBSSxDQUFDLFNBQVMsQ0FBQyxNQUFNLEVBQUUsRUFBM0YsQ0FBMkYsQ0FBQyxDQUFDO2dCQUNySixJQUFHLGVBQWUsQ0FBQyxNQUFNLEdBQUcsQ0FBQztvQkFBRSxNQUFNLElBQUksS0FBSyxDQUFDLDJGQUEyRixDQUFDLENBQUM7Z0JBRTVJLElBQUksS0FBSyxHQUFHLENBQUMsQ0FBQztnQkFDZCxDQUFDLENBQUMsSUFBSSxDQUFDLGVBQWUsRUFBRSxVQUFDLENBQUMsRUFBRSxDQUFDO29CQUN6QixJQUFJLGdCQUFnQixHQUFjLGNBQUEsU0FBUyxDQUFDLGFBQWEsR0FBRyxLQUFLLENBQUMsQ0FBQztvQkFDbkUsY0FBQSxTQUFTLENBQUMsUUFBUSxDQUFDLENBQUMsRUFBRSxjQUFBLFNBQVMsQ0FBQyxXQUFXLEVBQUUsTUFBTSxFQUFFLGdCQUFnQixDQUFDLENBQUM7b0JBQ3ZFLEtBQUssSUFBSSxDQUFDLENBQUM7Z0JBQ2YsQ0FBQyxDQUFDLENBQUM7Z0JBRUgsSUFBSSxNQUFNLEdBQUcsTUFBTSxDQUFDLFdB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WUFPQSxDQUFDO1lBTkcsMERBQU8sR0FBUCxVQUFRLE1BQWlCLEVBQUUsVUFBNEI7Z0JBRW5ELENBQUMsQ0FBQyxJQUFJLENBQUMsVUFBVSxFQUFFLFVBQUMsQ0FBQyxFQUFFLENBQUM7b0JBQ3BCLElBQUksQ0FBQyxDQUFDLFFBQVEsQ0FBQyxNQUFNLENBQUM7d0JBQUUsY0FBQSxRQUFRLENBQUMsa0JBQWtCLENBQUMsSUFBSSxFQUFFLENBQUMsQ0FBQyxDQUFDO2dCQUNqRSxDQUFDLENBQUMsQ0FBQztZQUNQLENBQUM7WUFDTCwrQ0FBQztRQUFELENBQUMsQUFQRCxJQU9DO1FBUFksc0RBQXdDLDJDQU9wRCxDQUFBO0lBQ0wsQ0FBQyxFQXRPcUIsYUFBYSxHQUFiLDRCQUFhLEtBQWIsNEJBQWEsUUFzT2xDO0FBQUQsQ0FBQyxFQXRPTSxjQUFjLEtBQWQsY0FBYyxRQXNPcEIiLCJzb3VyY2VzQ29udGVudCI6WyJtb2R1bGUgQWNjdXJhdGVBcHBlbmQuRHluYW1pY0FwcGVuZCB7XHJcbiAgICBcclxuICAgIC8vIHRhcmdldCA9IG9wZXJhdGlvbiBiZWluZyBtYXBwZWRcclxuICAgIC8vIG9wZXJhdGlvbnMgPSBnbG9iYWwgY29sbGVjdGlvbiBvZiBvcGVyYXRpb25zIG5vdCBpbmNsdWRpbmcgdGhlIHRhcmdldFxyXG4gICAgZXhwb3J0IGludGVyZmFjZSBJTWFwcGluZ1N0cmF0ZWd5IHtcclxuICAgICAgICBleGVjdXRlKHRhcmdldDogT3BlcmF0aW9uLCBvcGVyYXRpb25zOiBBcnJheTxPcGVyYXRpb24+KTogdm9pZDtcclxuICAgIH1cclxuICAgIFxyXG4gICAgZXhwb3J0IGNsYXNzIERlZHVwZVBob25lU3RyYXRlZ3kgaW1wbGVtZW50cyBJTWFwcGluZ1N0cmF0ZWd5IHtcclxuICAgICAgICBleGVjdXRlKHRhcmdldDogT3BlcmF0aW9uLCBvcGVyYXRpb25zOiBBcnJheTxPcGVyYXRpb24+KTogdm9pZCB7XHJcbiAgICAgICAgICAgIC8vIHZhbGlkYXRlLCByZXR1cm4gZXJyb3IgbWVzc2FnZSBpZiBzdHJhdGVneSBjYW4ndCBiZSBjb21wbGV0ZWRcclxuICAgICAgICAgICAgLy8gdGhlcmUgbmVlZCB0byBiZSBhdCBsZWFzdCB0d28gb3BlcmF0aW9ucyB3aXRoIGEgcGhvbmVudW1iZXIgaW4gdGhlaXIgb3V0cHV0XHJcbiAgICAgICAgICAgIHZhciBwaG9uZU9wZXJhdGlvbnMgPSBfLmZpbHRlcihvcGVyYXRpb25zLCBvcGVyYXRpb24gPT4gRmllbGRHcm91cC5pc1N1cHBvcnRlZChGaWVsZEdyb3VwTmFtZS5QaG9uZSwgb3BlcmF0aW9uLm91dHB1dGZpZWxkcykgJiYgIW9wZXJhdGlvbi5pc1V0aWwoKSk7XHJcbiAgICAgICAgICAgIGlmKHBob25lT3BlcmF0aW9ucy5sZW5ndGggPCAyKSB0aHJvdyBuZXcgRXJyb3IoXCJVbmFibGUgdG8gYWRkIHRoaXMgb3BlcmF0aW9uLiAyIG9yIG1vcmUgb3BlcmF0aW9ucyB0aGF0IG91dHB1dCBwaG9uZSBudW1iZXIgbXVzdCBwcm9jZWVkLlwiKTtcclxuICAgICAgICAgICAgLy8gbWFwIFxyXG4gICAgICAgICAgICB2YXIgaW5kZXggPSAxO1xyXG4gICAgICAgICAgICAkLmVhY2gocGhvbmVPcGVyYXRpb25zLCAoaSwgbykgPT4ge1xyXG4gICAgICAgICAgICAgICAgdmFyIHBob25lRGVkdXBlRmllbGQgPSA8RmllbGROYW1lPkZpZWxkTmFtZVtcIlBob25lTnVtYmVyXCIgKyBpbmRleF07XHJcbiAgICAgICAgICAgICAgICBPcGVyYXRpb24ubWFwRmllbGQobywgRmllbGROYW1lLlBob25lTnVtYmVyLCB0YXJnZXQsIHBob25lRGVkdXBlRmllbGQpO1xyXG4gICAgICAgICAgICAgICAgaW5kZXggKz0gMTtcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgIC8vIHJlbW92ZSB1bm1hcHBlZCBpbnB1dCBmaWVsZHMgZnJvbSBkZWR1cGUgb3BlcmF0aW9uXHJcbiAgICAgICAgICAgIHZhciBmaWVsZHMgPSB0YXJnZXQuaW5wdXRmaWVsZHM7XHJcbiAgICAgICAgICAgIGZvciAodmFyIGkgPSBmaWVsZHMubGVuZ3RoIC0gMTsgaSA+PSAwOyBpLS0pIHtcclxuICAgICAgICAgICAgICAgIGlmIChmaWVsZHNbaV0ubWV0YWZpZWxkTmFtZS5pbmRleE9mKFwiX1wiKSA9PT0gLTEpIGZpZWxkcy5zcGxpY2UoaSwgMSk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgZXhwb3J0IGNsYXNzIFNldFByZWZQaG9uZVN0cmF0ZWd5IGltcGxlbWVudHMgSU1hcHBpbmdTdHJhdGVneSB7XHJcbiAgICAgICAgZXhlY3V0ZSh0YXJnZXQ6IE9wZXJhdGlvbiwgb3BlcmF0aW9uczogQXJyYXk8T3BlcmF0aW9uPik6IHZvaWQge1xyXG4gICAgICAgICAgICAvLyB2YWxpZGF0ZSwgcmV0dXJuIGVycm9yIG1lc3NhZ2UgaWYgc3RyYXRlZ3kgY2FuJ3QgYmUgY29tcGxldGVkXHJcbiAgICAgICAgICAgIC8vIHRoZXJlIG5lZWQgdG8gYmUgYXQgbGVhc3QgdHdvIG9wZXJhdGlvbnMgd2l0aCBhIHBob25lbnVtYmVyIGluIHRoZWlyIG91dHB1dFxyXG4gICAgICAgICAgICB2YXIgcGhvbmVPcGVyYXRpb25zID0gXy5maWx0ZXIob3BlcmF0aW9ucywgb3BlcmF0aW9uID0+XHJcbiAgICAgICAgICAgICAgICBGaWVsZEdyb3VwLmlzU3VwcG9ydGVkKEZpZWxkR3JvdXBOYW1lLlBob25lLCBvcGVyYXRpb24ub3V0cHV0ZmllbGRzKSAmJiAhb3BlcmF0aW9uLmlzVXRpbCgpXHJcbiAgICAgICAgICAgICk7XHJcbiAgICAgICAgICAgIGlmIChwaG9uZU9wZXJhdGlvbnMubGVuZ3RoIDwgMikgdGhyb3cgbmV3IEVycm9yKFwiVW5hYmxlIHRvIGFkZCB0aGlzIG9wZXJhdGlvbi4gMiBvciBtb3JlIG9wZXJhdGlvbnMgdGhhdCBvdXRwdXQgcGhvbmUgbnVtYmVyIG11c3QgcHJvY2VlZC5cIik7XHJcblxyXG4gICAgICAgICAgICB2YXIgaW5kZXggPSAxO1xyXG4gICAgICAgICAgICAkLmVhY2gocGhvbmVPcGVyYXRpb25zLCAoaSwgbykgPT4ge1xyXG4gICAgICAgICAgICAgICAgLy8gbWFwIHRoZSBvdXRwdXQgb2YgUGhvbmUgb3BlcmF0aW9ucyB0byB0aGUgbmV4dCBhdmFpbGFibGUgaW5wdXQgb2YgdGhlIGRlZHVwZSBvcGVyYXRpb25cclxuICAgICAgICAgICAgICAgIHZhciBwaG9uZURlZHVwZUZpZWxkID0gPEZpZWxkTmFtZT5GaWVsZE5hbWVbXCJQaG9uZU51bWJlclwiICsgaW5kZXhdO1xyXG4gICAgICAgICAgICAgICAgT3BlcmF0aW9uLm1hcEZpZWxkKG8sIEZpZWxkTmFtZS5QaG9uZU51bWJlciwgdGFyZ2V0LCBwaG9uZURlZHVwZUZpZWxkKTtcclxuICAgICAgICAgICAgICAgIGluZGV4ICs9IDE7XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAvLyByZW1vdmUgdW5tYXBwZWQgaW5wdXQgZmllbGRzXHJcbiAgICAgICAgICAgIHZhciBmaWVsZHMgPSB0YXJnZXQuaW5wdXRmaWVsZHM7XHJcbiAgICAgICAgICAgIGZvciAodmFyIGkgPSBmaWVsZHMubGVuZ3RoIC0gMTsgaSA+PSAwOyBpLS0pIHtcclxuICAgICAgICAgICAgICAgIGlmIChmaWVsZHNbaV0ubWV0YWZpZWxkTmFtZS5pbmRleE9mKFwiX1wiKSA9PT0gLTEpIGZpZWxkcy5zcGxpY2UoaSwgMSk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgZXhwb3J0IGNsYXNzIFNldFByZWZQaG9uZUNvbXBhcmVJbnB1dFN0cmF0ZWd5IGltcGxlbWVudHMgSU1hcHBpbmdTdHJhdGVneSB7XHJcbiAgICAgICAgZXhlY3V0ZSh0YXJnZXQ6IE9wZXJhdGlvbiwgb3BlcmF0aW9uczogQXJyYXk8T3BlcmF0aW9uPik6IHZvaWQge1xyXG4gICAgICAgICAgICAvLyB2YWxpZGF0ZSwgcmV0dXJuIGVycm9yIG1lc3NhZ2UgaWYgc3RyYXRlZ3kgY2FuJ3QgYmUgY29tcGxldGVkXHJcbiAgICAgICAgICAgIHZhciBwaG9uZU9wZXJhdGlvbnMgPSBfLmZpbHRlcihvcGVyYXRpb25zLCBvcGVyYXRpb24gPT5cclxuICAgICAgICAgICAgICAgIEZpZWxkR3JvdXAuaXNTdXBwb3J0ZWQoRmllbGRHcm91cE5hbWUuUGhvbmUsIG9wZXJhdGlvbi5vdXRwdXRmaWVsZHMpICYmICFvcGVyYXRpb24uaXNVdGlsKClcclxuICAgICAgICAgICAgKTtcclxuICAgICAgICAgICAgXHJcbiAgICAgICAgICAgIC8vIHZhbGlkYXRpb24gcnVsZXNcclxuICAgICAgICAgICAgaWYgKHBob25lT3BlcmF0aW9ucy5sZW5ndGggPCAxKSB0aHJvdyBuZXcgRXJyb3IoXCJVbmFibGUgdG8gYWRkIHRoaXMgb3BlcmF0aW9uLiBPcGVyYXRpb24gcmVxdWlyZXMgYXQgbGVhc3QgMSBwaG9uZSBvcGVyYXRpb24uXCIpO1xyXG4gICAgICAgICAgICBpZiAocGhvbmVPcGVyYXRpb25zLmxlbmd0aCA+IDUpIHRocm93IG5ldyBFcnJvcihcIlVuYWJsZSB0byBhZGQgdGhpcyBvcGVyYXRpb24uIE9wZXJhdGlvbiBpcyBsaW1pdGVkIHRvIDUgb3IgZmV3ZXIgb3V0cHV0IHBob25lIG51bWJlcnMuXCIpO1xyXG5cclxuICAgICAgICAgICAgdmFyIGluZGV4ID0gMTtcclxuICAgICAgICAgICAgJC5lYWNoKHBob25lT3BlcmF0aW9ucywgKGksIG8pID0+IHtcclxuICAgICAgICAgICAgICAgIC8vIG1hcCB0aGUgb3V0cHV0IG9mIFBob25lIG9wZXJhdGlvbnMgdG8gdGhlIG5leHQgYXZhaWxhYmxlIGlucHV0IG9mIHRoZSBkZWR1cGUgb3BlcmF0aW9uXHJcbiAgICAgICAgICAgICAgICB2YXIgcGhvbmVEZWR1cGVGaWVsZCA9IDxGaWVsZE5hbWU+RmllbGROYW1lW1wiUGhvbmVOdW1iZXJcIiArIGluZGV4XTtcclxuICAgICAgICAgICAgICAgIE9wZXJhdGlvbi5tYXBGaWVsZChvLCBGaWVsZE5hbWUuUGhvbmVOdW1iZXIsIHRhcmdldCwgcGhvbmVEZWR1cGVGaWVsZCk7XHJcbiAgICAgICAgICAgICAgICBpbmRleCArPSAxO1xyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgLy8gcmVtb3ZlIHVubWFwcGVkIGlucHV0IGZpZWxkc1xyXG4gICAgICAgICAgICB2YXIgZmllbGRzID0gdGFyZ2V0LmlucHV0ZmllbGRzO1xyXG4gICAgICAgICAgICBmb3IgKHZhciBpID0gZmllbGRzLmxlbmd0aCAtIDE7IGkgPj0gMDsgaS0tKSB7XHJcbiAgICAgICAgICAgICAgICBpZiAoZmllbGRzW2ldLm1ldGFmaWVsZE5hbWUuaW5kZXhPZihcIl9cIikgPT09IC0xICYmIGZpZWxkc1tpXS5tZXRhZmllbGROYW1lICE9PSBcIlBob25lTnVtYmVyXCIpIGZpZWxkcy5zcGxpY2UoaSwgMSk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgZXhwb3J0IGNsYXNzIFNldFByZWZFbWFpbFZlclN0cmF0ZWd5IGltcGxlbWVudHMgSU1hcHBpbmdTdHJhdGVneSB7XHJcbiAgICAgICAgZXhlY3V0ZSh0YXJnZXQ6IE9wZXJhdGlvbiwgb3BlcmF0aW9uczogQXJyYXk8T3BlcmF0aW9uPik6IHZvaWQge1xyXG4gICAgICAgICAgICAvLyBFTUFJTF9WRVJfREVMSVZFUkFCTEUgbmVlZHMgdG8gYmUgaW4gdGhlIG9wZXJhdGlvbnMgY29sbGVjdGlvblxyXG4gICAgICAgICAgICB2YXIgZW1haWxWZXJpZmljYXRpb25PcGVyYXRpb24gPSBfLmZpbHRlcihvcGVyYXRpb25zLCBvcGVyYXRpb24gPT4gb3BlcmF0aW9uLm5hbWUgPT09IE9wZXJhdGlvbk5hbWUuRU1BSUxfVkVSX0RFTElWRVJBQkxFICk7XHJcbiAgICAgICAgICAgIGlmIChlbWFpbFZlcmlmaWNhdGlvbk9wZXJhdGlvbi5sZW5ndGggIT09IDEpIHRocm93IG5ldyBFcnJvcihcIlVuYWJsZSB0byBhZGQgdGhpcyBvcGVyYXRpb24uIE9wZXJhdGlvbiByZXF1aXJlcyBlbWFpbCB2ZXJpZmljYXRpb24uXCIpO1xyXG4gICAgICAgICAgICBcclxuICAgICAgICAgICAgdmFyIGluZGV4ID0gMTtcclxuICAgICAgICAgICAgJC5lYWNoKGVtYWlsVmVyaWZpY2F0aW9uT3BlcmF0aW9uLCAoaSwgbykgPT4ge1xyXG4gICAgICAgICAgICAgICAgLy8gbWFwIG91dHB1dHMgb2YgRU1BSUxfVkVSX0RFTElWRVJBQkxFIHRvIHRoaXMgb3BlcmF0aW9uIFNFVF9QUkVGX0VNQUlMX1ZFUlxyXG4gICAgICAgICAgICAgICAgT3BlcmF0aW9uLm1hcEZpZWxkKG8sIEZpZWxkTmFtZS5Jc1ZhbGlkLCB0YXJnZXQsIEZpZWxkTmFtZS5Jc1ZhbGlkKTtcclxuICAgICAgICAgICAgICAgIE9wZXJhdGlvbi5tYXBGaWVsZChvLCBGaWVsZE5hbWUuU3RhdHVzQ29kZSwgdGFyZ2V0LCBGaWVsZE5hbWUuU3RhdHVzQ29kZSk7XHJcbiAgICAgICAgICAgICAgICBpbmRleCArPSAxO1xyXG4gICAgICAgICAgICAgICAgLy8gdW5jaGVjayBvdXRwdXRmaWVsZHNcclxuICAgICAgICAgICAgICAgIE1hbmlmZXN0LnRvZ2dsZU91dHB1dGZpZWxkcyhmYWxzZSwgbyk7XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuICAgIGV4cG9ydCBjbGFzcyBTZXRQcmVmRW1haWxWZXJTdHJhdGVneVJlbW92ZSBpbXBsZW1lbnRzIElNYXBwaW5nU3RyYXRlZ3kge1xyXG4gICAgICAgIGV4ZWN1dGUodGFyZ2V0OiBPcGVyYXRpb24sIG9wZXJhdGlvbnM6IEFycmF5PE9wZXJhdGlvbj4pOiB2b2lkIHtcclxuICAgICAgICAgICAgLy8gZmluZCBlbWFpbCB2ZXJpZmljYXRpb24gb3BlcmF0aW9uIHRoaXMgd2FzIG1hcHBlZCB0bywgcmVjaGVjayBpdCdzIG91dHB1dHNcclxuICAgICAgICAgICAgdmFyIGVtYWlsVmVyaWZpY2F0aW9uT3BlcmF0aW9uID0gXy5maXJzdChfLmZpbHRlcihvcGVyYXRpb25zLCBvcGVyYXRpb24gPT4gb3BlcmF0aW9uLm5hbWUgPT09IE9wZXJhdGlvbk5hbWUuRU1BSUxfVkVSX0RFTElWRVJBQkxFKSk7XHJcbiAgICAgICAgICAgIE1hbmlmZXN0LnRvZ2dsZU91dHB1dGZpZWxkcyh0cnVlLCBlbWFpbFZlcmlmaWNhdGlvbk9wZXJhdGlvbik7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIGV4cG9ydCBjbGFzcyBTZXRQcmVmQmFzZWRPblZlcmlmaWNhdGlvblN0cmF0ZWd5IGltcGxlbWVudHMgSU1hcHBpbmdTdHJhdGVneSB7XHJcbiAgICAgICAgZXhlY3V0ZSh0YXJnZXQ6IE9wZXJhdGlvbiwgb3BlcmF0aW9uczogQXJyYXk8T3BlcmF0aW9uPik6IHZvaWQge1xyXG4gICAgICAgICAgICAvLyB2YWxpZGF0ZSwgcmV0dXJuIGVycm9yIG1lc3NhZ2UgaWYgc3RyYXRlZ3kgY2FuJ3QgYmUgY29tcGxldGVkXHJcbiAgICAgICAgICAgIHZhciBwaG9uZU9wZXJhdGlvbnMgPSBfLmZpbHRlcihvcGVyYXRpb25zLCBvcGVyYXRpb24gPT5cclxuICAgICAgICAgICAgICAgIEZpZWxkR3JvdXAuaXNTdXBwb3J0ZWQoRmllbGRHcm91cE5hbWUuUGhvbmUsIG9wZXJhdGlvbi5vdXRwdXRmaWVsZHMpICYmICFvcGVyYXRpb24uaXNVdGlsKClcclxuICAgICAgICAgICAgKTtcclxuICAgICAgICAgICAgLy8gdmFsaWRhdGlvbiBydWxlc1xyXG4gICAgICAgICAgICAvLyBlbnN1cmUgUEhPTkVfVkVSIGhhcyBiZWVuIHNlbGVjdGVkXHJcbiAgICAgICAgICAgIHZhciBwaG9uZVZlclByZXNlbnQgPSBfLmZpbmQob3BlcmF0aW9ucywgb3BlcmF0aW9uID0+IG9wZXJhdGlvbi5uYW1lID09PSBPcGVyYXRpb25OYW1lLlBIT05FX1ZFUik7XHJcbiAgICAgICAgICAgIGlmICghcGhvbmVWZXJQcmVzZW50KSB0aHJvdyBuZXcgRXJyb3IoXCJVbmFibGUgdG8gYWRkIHRoaXMgb3BlcmF0aW9uLiBWZXJpZnkgQ29ubmVjdGlvbiBTdGF0dXMgKEMxLUM3KSBtdXN0IGJlIHNlbGVjdGVkIGZpcnN0LlwiKTtcclxuICAgICAgICAgICAgaWYgKHBob25lT3BlcmF0aW9ucy5sZW5ndGggPCAxKSB0aHJvdyBuZXcgRXJyb3IoXCJVbmFibGUgdG8gYWRkIHRoaXMgb3BlcmF0aW9uLiBPcGVyYXRpb24gcmVxdWlyZXMgYXQgbGVhc3QgMSBwaG9uZSBvcGVyYXRpb24uXCIpO1xyXG4gICAgICAgICAgICBpZiAocGhvbmVPcGVyYXRpb25zLmxlbmd0aCA+IDUpIHRocm93IG5ldyBFcnJvcihcIlVuYWJsZSB0byBhZGQgdGhpcyBvcGVyYXRpb24uIE9wZXJhdGlvbiBpcyBsaW1pdGVkIHRvIDUgb3IgZmV3ZXIgb3V0cHV0IHBob25lIG51bWJlcnMuXCIpO1xyXG5cclxuICAgICAgICAgICAgLy8gbWFwIFBob25lVmVyaWZpY2F0aW9uU3RhdHVzIGluIFBIT05FX1ZFUiBvdXRwdXRmaWVsZHMgdG8gZGVkdXBlIG9wZXJhdGlvblxyXG4gICAgICAgICAgICB2YXIgcGhvbmVWZXJPcGVyYXRpb24gPSBfLmZpcnN0KF8uZmlsdGVyKG9wZXJhdGlvbnMsIG9wZXJhdGlvbiA9PiBvcGVyYXRpb24ubmFtZSA9PT0gT3BlcmF0aW9uTmFtZS5QSE9ORV9WRVIgKSk7XHJcbiAgICAgICAgICAgIE9wZXJhdGlvbi5tYXBGaWVsZChwaG9uZVZlck9wZXJhdGlvbiwgRmllbGROYW1lLlBob25lVmVyaWZpY2F0aW9uU3RhdHVzLCB0YXJnZXQsIEZpZWxkTmFtZS5QaG9uZVZlcmlmaWNhdGlvblN0YXR1cyk7XHJcbiAgICAgICAgICAgIFxyXG4gICAgICAgICAgICB2YXIgaW5kZXggPSAxO1xyXG4gICAgICAgICAgICAkLmVhY2gocGhvbmVPcGVyYXRpb25zLCAoaSwgbykgPT4ge1xyXG4gICAgICAgICAgICAgICAgLy8gbWFwIHRoZSBvdXRwdXQgb2YgUGhvbmUgb3BlcmF0aW9ucyB0byB0aGUgbmV4dCBhdmFpbGFibGUgaW5wdXQgb2YgdGhlIGRlZHVwZSBvcGVyYXRpb25cclxuICAgICAgICAgICAgICAgIHZhciBwaG9uZURlZHVwZUZpZWxkID0gPEZpZWxkTmFtZT5GaWVsZE5hbWVbXCJQaG9uZU51bWJlclwiICsgaW5kZXhdO1xyXG4gICAgICAgICAgICAgICAgT3BlcmF0aW9uLm1hcEZpZWxkKG8sIEZpZWxkTmFtZS5QaG9uZU51bWJlciwgdGFyZ2V0LCBwaG9uZURlZHVwZUZpZWxkKTtcclxuICAgICAgICAgICAgICAgIGluZGV4ICs9IDE7XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAvLyByZW1vdmUgdW5tYXBwZWQgaW5wdXQgZmllbGRzXHJcbiAgICAgICAgICAgIHZhciBmaWVsZHMgPSB0YXJnZXQuaW5wdXRmaWVsZHM7XHJcbiAgICAgICAgICAgIGZvciAodmFyIGkgPSBmaWVsZHMubGVuZ3RoIC0gMTsgaSA+PSAwOyBpLS0pIHtcclxuICAgICAgICAgICAgICAgIGlmIChmaWVsZHNbaV0ubWV0YWZpZWxkTmFtZS5pbmRleE9mKFwiX1wiKSA9PT0gLTEgJiYgZmllbGRzW2ldLm1ldGFmaWVsZE5hbWUgIT09IFwiUGhvbmVOdW1iZXJcIikgZmllbGRzLnNwbGljZShpLCAxKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBleHBvcnQgY2xhc3MgU2V0UHJlZlBob25lU2luZ2xlQ29sdW1uU3RyYXRlZ3kgaW1wbGVtZW50cyBJTWFwcGluZ1N0cmF0ZWd5IHtcclxuICAgICAgICBleGVjdXRlKHRhcmdldDogT3BlcmF0aW9uLCBvcGVyYXRpb25zOiBBcnJheTxPcGVyYXRpb24+KTogdm9pZCB7XHJcbiAgICAgICAgICAgIC8vIHZhbGlkYXRlLCByZXR1cm4gZXJyb3IgbWVzc2FnZSBpZiBzdHJhdGdleSBjYW4ndCBiZSBjb21wbGV0ZWRcclxuICAgICAgICAgICAgLy8gdGhlcmUgbmVlZCB0byBiZSBhdCBsZWFzdCB0d28gb3BlcmF0aW9ucyB3aXRoIGEgcGhvbmVudW1iZXIgaW4gdGhlaXIgb3V0cHV0XHJcbiAgICAgICAgICAgIHZhciBwaG9uZU9wZXJhdGlvbnMgPSBfLmZpbHRlcihvcGVyYXRpb25zLCBvcGVyYXRpb24gPT4gRmllbGRHcm91cC5pc1N1cHBvcnRlZChGaWVsZEdyb3VwTmFtZS5QaG9uZSwgb3BlcmF0aW9uLm91dHB1dGZpZWxkcykgJiYgIW9wZXJhdGlvbi5pc1V0aWwoKSk7XHJcbiAgICAgICAgICAgIGlmIChwaG9uZU9wZXJhdGlvbnMubGVuZ3RoIDwgMikgdGhyb3cgbmV3IEVycm9yKFwiVW5hYmxlIHRvIGFkZCB0aGlzIG9wZXJhdGlvbi4gMiBvciBtb3JlIG9wZXJhdGlvbnMgdGhhdCBvdXRwdXQgcGhvbmUgbnVtYmVyIG11c3QgcHJlY2VlZC5cIik7XHJcbiAgICAgICAgICAgIHZhciBpbmRleCA9IDE7XHJcbiAgICAgICAgICAgICQuZWFjaChwaG9uZU9wZXJhdGlvbnMsIChpLCBvKSA9PiB7XHJcbiAgICAgICAgICAgICAgICAvLyBtYXAgdGhlIG91dHB1dCBvZiBQaG9uZSBvZXByYXRpb25zIHRvIHRoZSBuZXh0IGF2YWlsYWJsZSBpbnB1dCBvZiB0aGUgZGVkdXBlIG9wZXJhdGlvblxyXG4gICAgICAgICAgICAgICAgdmFyIHBob25lRGVkdXBlRmllbGQgPSA8RmllbGROYW1lPkZpZWxkTmFtZVtcIlBob25lTnVtYmVyXCIgKyBpbmRleF07XHJcbiAgICAgICAgICAgICAgICBPcGVyYXRpb24ubWFwRmllbGQobywgRmllbGROYW1lLlBob25lTnVtYmVyLCB0YXJnZXQsIHBob25lRGVkdXBlRmllbGQpO1xyXG4gICAgICAgICAgICAgICAgaW5kZXggKz0gMTtcclxuICAgICAgICAgICAgICAgIC8vIGRpcy1pbmNsdWRlIG91dHB1dGZpZWxkcyBpbiBvZXByYXRpb25zIGNvbnRhaW5pbmcgUGhvbmVOdW1iZXJcclxuICAgICAgICAgICAgICAgIE1hbmlmZXN0LnRvZ2dsZU91dHB1dGZpZWxkcyhmYWxzZSwgbyk7XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAvLyBybWVvdmUgdW5tYXBwZWQgaW5wdXQgZmllbGRzXHJcbiAgICAgICAgICAgIHZhciBmaWVsZHMgPSB0YXJnZXQuaW5wdXRmaWVsZHM7XHJcbiAgICAgICAgICAgIGZvciAodmFyIGkgPSBmaWVsZHMubGVuZ3RoIC0gMTsgaSA+PSAwOyBpLS0pIHtcclxuICAgICAgICAgICAgICAgIGlmIChmaWVsZHNbaV0ubWV0YWZpZWxkTmFtZS5pbmRleE9mKFwiX1wiKSA9PT0gLTEpIGZpZWxkcy5zcGxpY2UoaSwgMSk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9XHJcbiAgICB9XHJcbiAgICBleHBvcnQgY2xhc3MgU2V0UHJlZlBob25lU2luZ2xlQ29sdW1uU3RyYXRlZ3lSZW1vdmUgaW1wbGVtZW50cyBJTWFwcGluZ1N0cmF0ZWd5IHtcclxuICAgICAgICBleGVjdXRlKHRhcmdldDogT3BlcmF0aW9uLCBvcGVyYXRpb25zOiBBcnJheTxPcGVyYXRpb24+KTogdm9pZCB7XHJcbiAgICAgICAgICAgIC8vIGZpbmQgcGhvbmUgb3BlcmF0aW9ucyB0aGlzIG9wZXJhdGlvbiB3YXMgbWFwcGVkIHRvXHJcbiAgICAgICAgICAgICQuZWFjaChvcGVyYXRpb25zLCAoaSwgbykgPT4ge1xyXG4gICAgICAgICAgICAgICAgaWYgKG8uaXNNYXBwZWQodGFyZ2V0KSkgTWFuaWZlc3QudG9nZ2xlT3V0cHV0ZmllbGRzKHRydWUsIG8pO1xyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcbiAgICBcclxuICAgIGV4cG9ydCBjbGFzcyBOY29hNDhTdHJhdGVneSBpbXBsZW1lbnRzIElNYXBwaW5nU3RyYXRlZ3kge1xyXG4gICAgICAgIGV4ZWN1dGUodGFyZ2V0OiBPcGVyYXRpb24sIG9wZXJhdGlvbnM6IEFycmF5PE9wZXJhdGlvbj4pOiB2b2lkIHtcclxuICAgICAgICAgICAgJC5lYWNoKG9wZXJhdGlvbnMsIChpLCBvKSA9PiB7XHJcbiAgICAgICAgICAgICAgICBpZiAoby5pc1V0aWwoKSkgcmV0dXJuIHRydWU7XHJcbiAgICAgICAgICAgICAgICBjb25zb2xlLmxvZyhcIk1hcHBpbmcgXCIgKyBPcGVyYXRpb25OYW1lW28ubmFtZV0pO1xyXG4gICAgICAgICAgICAgICAgLy9pZiAoaSA9PT0gMCkgcmV0dXJuIHRydWU7XHJcbiAgICAgICAgICAgICAgICAvLyBmaW5kIG9wZXJhdGlvbnMgd2hvc2UgaW5wdXQgc3VwcG9ydHMgJ0FkZHJlc3MnIGFuZCBtYXAgdGhpZXIgaW5wdXRzIHRvIE5DT0Egb3V0cHV0XHJcbiAgICAgICAgICAgICAgICBpZiAoRmllbGRHcm91cC5pc1N1cHBvcnRlZChGaWVsZEdyb3VwTmFtZS5BZGRyZXNzLCBvLmlucHV0ZmllbGRzKSkge1xyXG4gICAgICAgICAgICAgICAgICAgICQuZWFjaChGaWVsZEdyb3VwLmdldEZpZWxkcyhGaWVsZEdyb3VwTmFtZS5BZGRyZXNzKSwgKGksIGZpZWxkTmFtZVRvTWFwKSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIE9wZXJhdGlvbi5tYXBGaWVsZCh0YXJnZXQsIGZpZWxkTmFtZVRvTWFwLCBvLCBmaWVsZE5hbWVUb01hcCk7XHJcbiAgICAgICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuICAgIGV4cG9ydCBjbGFzcyBOY29hNDhTdHJhdGVneVJlbW92ZSBpbXBsZW1lbnRzIElNYXBwaW5nU3RyYXRlZ3kge1xyXG4gICAgICAgIGV4ZWN1dGUodGFyZ2V0OiBPcGVyYXRpb24sIG9wZXJhdGlvbnM6IEFycmF5PE9wZXJhdGlvbj4pOiB2b2lkIHtcclxuICAgICAgICAgICAgLy8gZmluZCBPcGVyYXRpb25zIHRoYXQgd2VyZSBtYXBwZWQgdG8gdGhpcyBcclxuICAgICAgICAgICAgJC5lYWNoKG9wZXJhdGlvbnMsIChpLCBvKSA9PiB7XHJcbiAgICAgICAgICAgICAgICBpZiAoby5pc1V0aWwoKSkgcmV0dXJuIHRydWU7XHJcbiAgICAgICAgICAgICAgICBpZiAoby5uYW1lID09PSBPcGVyYXRpb25OYW1lLk5DT0E0OCkgcmV0dXJuIHRydWU7XHJcbiAgICAgICAgICAgICAgICAvLyBmaW5kIG9wZXJhdGlvbnMgd2hvc2UgaW5wdXQgc3VwcG9ydHMgJ0FkZHJlc3MnLCB0aGVzZSB3ZXJlIG1hcGVkIHRvIE5DT0FcclxuICAgICAgICAgICAgICAgIGlmIChGaWVsZEdyb3VwLmlzU3VwcG9ydGVkKEZpZWxkR3JvdXBOYW1lLkFkZHJlc3MsIG8uaW5wdXRmaWVsZHMpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgdmFyIGFkZHJlc3NGaWVsZHMgPSBGaWVsZEdyb3VwLmdldEZpZWxkcyhGaWVsZEdyb3VwTmFtZS5BZGRyZXNzKTtcclxuICAgICAgICAgICAgICAgICAgICAkLmVhY2goXy5maWx0ZXIoby5pbnB1dGZpZWxkcywgbyA9PiAoXy5pbmRleE9mKGFkZHJlc3NGaWVsZHMsIG8ubmFtZSkgIT09IC0xKSksIChpLCBmKSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGNvbnNvbGUubG9nKE9wZXJhdGlvbk5hbWVbby5uYW1lXSArIFwiOiB1bm1hcHBpbmcgXCIgKyBmLm1ldGFmaWVsZE5hbWUgKyBcIiB0byBcIiArIEZpZWxkTmFtZVtmLm5hbWVdKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZi5tZXRhZmllbGROYW1lID0gRmllbGROYW1lW2YubmFtZV07XHJcbiAgICAgICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBleHBvcnQgY2xhc3MgU2V0UHJlZkFkZHJlc3NTaW5nbGVDb2x1bW5TdHJhdGVneSBpbXBsZW1lbnRzIElNYXBwaW5nU3RyYXRlZ3kge1xyXG4gICAgICAgIGV4ZWN1dGUodGFyZ2V0OiBPcGVyYXRpb24sIG9wZXJhdGlvbnM6IEFycmF5PE9wZXJhdGlvbj4pOiB2b2lkIHtcclxuICAgICAgICAgICAgLy8gdmFsaWRhdGUsIHJldHVybiBlcnJvciBtZXNzYWdlIGlmIHN0cmF0Z2V5IGNhbid0IGJlIGNvbXBsZXRlZFxyXG4gICAgICAgICAgICAvLyB0aGVyZSBuZWVkIHRvIGJlIGF0IGxlYXN0IHR3byBvcGVyYXRpb25zIHdpdGggYSBwaG9uZW51bWJlciBpbiB0aGVpciBvdXRwdXRcclxuICAgICAgICAgICAgdmFyIHBob25lT3BlcmF0aW9ucyA9IF8uZmlsdGVyKG9wZXJhdGlvbnMsIG9wZXJhdGlvbiA9PiBGaWVsZEdyb3VwLmlzU3VwcG9ydGVkKEZpZWxkR3JvdXBOYW1lLkFkZHJlc3MsIG9wZXJhdGlvbi5vdXRwdXRmaWVsZHMpICYmICFvcGVyYXRpb24uaXNVdGlsKCkpO1xyXG4gICAgICAgICAgICBpZiAocGhvbmVPcGVyYXRpb25zLmxlbmd0aCA8IDIpIHRocm93IG5ldyBFcnJvcihcIlVuYWJsZSB0byBhZGQgdGhpcyBvcGVyYXRpb24uIDIgb3IgbW9yZSBvcGVyYXRpb25zIHRoYXQgb3V0cHV0IHBob25lIG51bWJlciBtdXN0IHByZWNlZGUuXCIpO1xyXG4gICAgICAgICAgICB2YXIgaW5kZXggPSAxO1xyXG4gICAgICAgICAgICAkLmVhY2gocGhvbmVPcGVyYXRpb25zLCAoaSwgbykgPT4ge1xyXG4gICAgICAgICAgICAgICAgLy8gbWFwIHRoZSBvdXRwdXQgb2YgUGhvbmUgb2VwcmF0aW9ucyB0byB0aGUgbmV4dCBhdmFpbGFibGUgaW5wdXQgb2YgdGhlIGRlZHVwZSBvcGVyYXRpb25cclxuICAgICAgICAgICAgICAgIHZhciBkZWR1cGVGaWVsZCA9IDxGaWVsZE5hbWU+RmllbGROYW1lW1wiU3RyZWV0QWRkcmVzc1wiICsgaW5kZXhdO1xyXG4gICAgICAgICAgICAgICAgT3BlcmF0aW9uLm1hcEZpZWxkKG8sIEZpZWxkTmFtZS5TdHJlZXRBZGRyZXNzLCB0YXJnZXQsIGRlZHVwZUZpZWxkKTtcclxuICAgICAgICAgICAgICAgIGluZGV4ICs9IDE7XHJcbiAgICAgICAgICAgICAgICAvLyBkaXMtaW5jbHVkZSBvdXRwdXRmaWVsZHMgaW4gb2VwcmF0aW9ucyBjb250YWluaW5nIFBob25lTnVtYmVyXHJcbiAgICAgICAgICAgICAgICBNYW5pZmVzdC50b2dnbGVPdXRwdXRmaWVsZHMoZmFsc2UsIG8pO1xyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgLy8gcm1lb3ZlIHVubWFwcGVkIGlucHV0IGZpZWxkc1xyXG4gICAgICAgICAgICB2YXIgZmllbGRzID0gdGFyZ2V0LmlucHV0ZmllbGRzO1xyXG4gICAgICAgICAgICBmb3IgKHZhciBpID0gZmllbGRzLmxlbmd0aCAtIDE7IGkgPj0gMDsgaS0tKSB7XHJcbiAgICAgICAgICAgICAgICBpZiAoZmllbGRzW2ldLm1ldGFmaWVsZE5hbWUuaW5kZXhPZihcIl9cIikgPT09IC0xKSBmaWVsZHMuc3BsaWNlKGksIDEpO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG4gICAgZXhwb3J0IGNsYXNzIFNldFByZWZBZGRyZXNzU2luZ2xlQ29sdW1uU3RyYXRlZ3lSZW1vdmUgaW1wbGVtZW50cyBJTWFwcGluZ1N0cmF0ZWd5IHtcclxuICAgICAgICBleGVjdXRlKHRhcmdldDogT3BlcmF0aW9uLCBvcGVyYXRpb25zOiBBcnJheTxPcGVyYXRpb24+KTogdm9pZCB7XHJcbiAgICAgICAgICAgIC8vIGZpbmQgcGhvbmUgb3BlcmF0aW9ucyB0aGlzIG9wZXJhdGlvbiB3YXMgbWFwcGVkIHRvXHJcbiAgICAgICAgICAgICQuZWFjaChvcGVyYXRpb25zLCAoaSwgbykgPT4ge1xyXG4gICAgICAgICAgICAgICAgaWYgKG8uaXNNYXBwZWQodGFyZ2V0KSkgTWFuaWZlc3QudG9nZ2xlT3V0cHV0ZmllbGRzKHRydWUsIG8pO1xyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcbn0iXX0=