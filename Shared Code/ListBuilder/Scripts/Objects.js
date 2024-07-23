var AccurateAppend;
(function (AccurateAppend) {
    var ListBuilder;
    (function (ListBuilder) {
        var County = (function () {
            function County(name, fips, state) {
                this.name = name;
                this.fips = fips;
                this.state = state;
                this.id = generateUuid();
            }
            return County;
        }());
        ListBuilder.County = County;
        var Zip = (function () {
            function Zip(name, state) {
                this.name = name;
                this.state = state;
                this.id = generateUuid();
            }
            return Zip;
        }());
        ListBuilder.Zip = Zip;
        var City = (function () {
            function City(name, state) {
                this.name = name;
                this.state = state;
                this.id = generateUuid();
            }
            return City;
        }());
        ListBuilder.City = City;
        var State = (function () {
            function State(abbreviation, fips, stateFullName) {
                this.abbreviation = abbreviation;
                this.fips = fips;
                this.stateFullName = stateFullName;
                this.id = generateUuid();
            }
            return State;
        }());
        ListBuilder.State = State;
        var DobRange = (function () {
            function DobRange(start, end) {
                this.start = start;
                this.end = end;
            }
            DobRange.prototype.isValid = function () {
                if (!this.start.isValid().result)
                    return new Validation(false, "Please enter a valid start date.");
                if (!this.end.isValid().result)
                    return new Validation(false, "Please enter a valid end date.");
                var startDate = new Date(this.start.year, 1, this.start.month);
                var endDate = new Date(this.end.year, 1, this.end.month);
                if (endDate < startDate)
                    return new Validation(false, "End date must be after start date.");
                if (!this.start.isAdult())
                    return new Validation(false, "Start date and end date must be Age 18 or older.");
                return new Validation(true, "");
            };
            DobRange.prototype.toString = function () {
                return this.start + " to " + this.end;
            };
            return DobRange;
        }());
        ListBuilder.DobRange = DobRange;
        var Dob = (function () {
            function Dob(date) {
                this.year = 0;
                this.month = 0;
                var regex = /^(0[1-9]|1[0-2])-(\d{4})/;
                var m;
                if ((m = regex.exec(date)) !== null) {
                    this.month = Number(m[1]);
                    this.year = Number(m[2]);
                }
            }
            Dob.prototype.isValid = function () {
                if (this.month === 0 || this.year === 0)
                    return new Validation(false, "Year and Month must be populated");
                return new Validation(true, "");
            };
            Dob.prototype.isAdult = function () {
                var today = new Date();
                var age = today.getFullYear() - this.year;
                var m = today.getMonth() - this.month;
                if (m < 0 || (m === 0 && today.getDate() < new Date(this.year, this.month).getDate())) {
                    age--;
                }
                return age >= 18;
            };
            Dob.prototype.toString = function () {
                return ((this.month === 10 || this.month === 11 || this.month === 12) ? this.month : "0" + this.month) + "-" + this.year;
            };
            return Dob;
        }());
        ListBuilder.Dob = Dob;
        var ValueLabel = (function () {
            function ValueLabel(name, label) {
                this.name = name;
                this.label = label;
                this.id = generateUuid();
            }
            return ValueLabel;
        }());
        ListBuilder.ValueLabel = ValueLabel;
        var Validation = (function () {
            function Validation(result, message) {
                this.result = result;
                this.message = message;
            }
            return Validation;
        }());
        ListBuilder.Validation = Validation;
        function generateUuid() {
            var d = new Date().getTime();
            var uuid = "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
                var r = (d + Math.random() * 16) % 16 | 0;
                d = Math.floor(d / 16);
                return (c === "x" ? r : (r & 0x7 | 0x8)).toString(16);
            });
            return uuid;
        }
        ;
    })(ListBuilder = AccurateAppend.ListBuilder || (AccurateAppend.ListBuilder = {}));
})(AccurateAppend || (AccurateAppend = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiT2JqZWN0cy5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIk9iamVjdHMudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUEsSUFBTyxjQUFjLENBNEdwQjtBQTVHRCxXQUFPLGNBQWM7SUFBQyxJQUFBLFdBQVcsQ0E0R2hDO0lBNUdxQixXQUFBLFdBQVc7UUFFN0I7WUFHSSxnQkFBbUIsSUFBWSxFQUFTLElBQVksRUFBUyxLQUFZO2dCQUF0RCxTQUFJLEdBQUosSUFBSSxDQUFRO2dCQUFTLFNBQUksR0FBSixJQUFJLENBQVE7Z0JBQVMsVUFBSyxHQUFMLEtBQUssQ0FBTztnQkFDckUsSUFBSSxDQUFDLEVBQUUsR0FBRyxZQUFZLEVBQUUsQ0FBQztZQUM3QixDQUFDO1lBQ0wsYUFBQztRQUFELENBQUMsQUFORCxJQU1DO1FBTlksa0JBQU0sU0FNbEIsQ0FBQTtRQUVEO1lBR0ksYUFBbUIsSUFBWSxFQUFTLEtBQVk7Z0JBQWpDLFNBQUksR0FBSixJQUFJLENBQVE7Z0JBQVMsVUFBSyxHQUFMLEtBQUssQ0FBTztnQkFDaEQsSUFBSSxDQUFDLEVBQUUsR0FBRyxZQUFZLEVBQUUsQ0FBQztZQUM3QixDQUFDO1lBQ0wsVUFBQztRQUFELENBQUMsQUFORCxJQU1DO1FBTlksZUFBRyxNQU1mLENBQUE7UUFFRDtZQUdJLGNBQW1CLElBQVksRUFBUyxLQUFZO2dCQUFqQyxTQUFJLEdBQUosSUFBSSxDQUFRO2dCQUFTLFVBQUssR0FBTCxLQUFLLENBQU87Z0JBQ2hELElBQUksQ0FBQyxFQUFFLEdBQUcsWUFBWSxFQUFFLENBQUM7WUFDN0IsQ0FBQztZQUNMLFdBQUM7UUFBRCxDQUFDLEFBTkQsSUFNQztRQU5ZLGdCQUFJLE9BTWhCLENBQUE7UUFFRDtZQUdJLGVBQW1CLFlBQW9CLEVBQVMsSUFBWSxFQUFTLGFBQXNCO2dCQUF4RSxpQkFBWSxHQUFaLFlBQVksQ0FBUTtnQkFBUyxTQUFJLEdBQUosSUFBSSxDQUFRO2dCQUFTLGtCQUFhLEdBQWIsYUFBYSxDQUFT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RUFBRSxDQUFDO1lBQzdCLElBQU0sSUFBSSxHQUFHLHNDQUFzQyxDQUFDLE9BQU8sQ0FBQyxPQUFPLEVBQy9ELFVBQUEsQ0FBQztnQkFDRyxJQUFJLENBQUMsR0FBRyxDQUFDLENBQUMsR0FBRyxJQUFJLENBQUMsTUFBTSxFQUFFLEdBQUcsRUFBRSxDQUFDLEdBQUcsRUFBRSxHQUFHLENBQUMsQ0FBQztnQkFDMUMsQ0FBQyxHQUFHLElBQUksQ0FBQyxLQUFLLENBQUMsQ0FBQyxHQUFHLEVBQUUsQ0FBQyxDQUFDO2dCQUN2QixPQUFPLENBQUMsQ0FBQyxLQUFLLEdBQUcsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsR0FBRyxHQUFHLEdBQUcsR0FBRyxDQUFDLENBQUMsQ0FBQyxRQUFRLENBQUMsRUFBRSxDQUFDLENBQUM7WUFDMUQsQ0FBQyxDQUFDLENBQUM7WUFDUCxPQUFPLElBQUksQ0FBQztRQUNoQixDQUFDO1FBQUEsQ0FBQztJQUVOLENBQUMsRUE1R3FCLFdBQVcsR0FBWCwwQkFBVyxLQUFYLDBCQUFXLFFBNEdoQztBQUFELENBQUMsRUE1R00sY0FBYyxLQUFkLGNBQWMsUUE0R3BCIiwic291cmNlc0NvbnRlbnQiOlsibW9kdWxlIEFjY3VyYXRlQXBwZW5kLkxpc3RCdWlsZGVyIHtcclxuICAgIFxyXG4gICAgZXhwb3J0IGNsYXNzIENvdW50eSB7XHJcbiAgICAgICAgaWQ6IHN0cmluZztcclxuXHJcbiAgICAgICAgY29uc3RydWN0b3IocHVibGljIG5hbWU6IHN0cmluZywgcHVibGljIGZpcHM6IG51bWJlciwgcHVibGljIHN0YXRlOiBTdGF0ZSkge1xyXG4gICAgICAgICAgICB0aGlzLmlkID0gZ2VuZXJhdGVVdWlkKCk7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIGV4cG9ydCBjbGFzcyBaaXAge1xyXG4gICAgICAgIGlkOiBzdHJpbmc7XHJcblxyXG4gICAgICAgIGNvbnN0cnVjdG9yKHB1YmxpYyBuYW1lOiBzdHJpbmcsIHB1YmxpYyBzdGF0ZTogU3RhdGUpIHtcclxuICAgICAgICAgICAgdGhpcy5pZCA9IGdlbmVyYXRlVXVpZCgpO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBleHBvcnQgY2xhc3MgQ2l0eSB7XHJcbiAgICAgICAgaWQ6IHN0cmluZztcclxuXHJcbiAgICAgICAgY29uc3RydWN0b3IocHVibGljIG5hbWU6IHN0cmluZywgcHVibGljIHN0YXRlOiBTdGF0ZSkge1xyXG4gICAgICAgICAgICB0aGlzLmlkID0gZ2VuZXJhdGVVdWlkKCk7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIGV4cG9ydCBjbGFzcyBTdGF0ZSB7XHJcbiAgICAgICAgaWQ6IHN0cmluZztcclxuXHJcbiAgICAgICAgY29uc3RydWN0b3IocHVibGljIGFiYnJldmlhdGlvbjogc3RyaW5nLCBwdWJsaWMgZmlwczogbnVtYmVyLCBwdWJsaWMgc3RhdGVGdWxsTmFtZT86IHN0cmluZykge1xyXG4gICAgICAgICAgICB0aGlzLmlkID0gZ2VuZXJhdGVVdWlkKCk7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIGV4cG9ydCBjbGFzcyBEb2JSYW5nZSB7XHJcblxyXG4gICAgICAgIGNvbnN0cnVjdG9yKHB1YmxpYyBzdGFydDogRG9iLCBwdWJsaWMgZW5kOiBEb2IpIHtcclxuICAgICAgICB9XHJcbiAgICAgICAgaXNWYWxpZCgpIHtcclxuICAgICAgICAgICAgLy8gc3RhcnQgYW5kIGVuZCBhcmUgdmFsaWQgZGF0ZXNcclxuICAgICAgICAgICAgaWYgKCF0aGlzLnN0YXJ0LmlzVmFsaWQoKS5yZXN1bHQpIHJldHVybiBuZXcgVmFsaWRhdGlvbihmYWxzZSwgXCJQbGVhc2UgZW50ZXIgYSB2YWxpZCBzdGFydCBkYXRlLlwiKTtcclxuICAgICAgICAgICAgaWYgKCF0aGlzLmVuZC5pc1ZhbGlkKCkucmVzdWx0KSByZXR1cm4gbmV3IFZhbGlkYXRpb24oZmFsc2UsIFwiUGxlYXNlIGVudGVyIGEgdmFsaWQgZW5kIGRhdGUuXCIpO1xyXG4gICAgICAgICAgICBjb25zdCBzdGFydERhdGUgPSBuZXcgRGF0ZSh0aGlzLnN0YXJ0LnllYXIsIDEsIHRoaXMuc3RhcnQubW9udGgpO1xyXG4gICAgICAgICAgICBjb25zdCBlbmREYXRlID0gbmV3IERhdGUodGhpcy5lbmQueWVhciwgMSwgdGhpcy5lbmQubW9udGgpO1xyXG4gICAgICAgICAgICAvLyBzdGFydCBpcyBiZWZvcmUgZW5kXHJcbiAgICAgICAgICAgIGlmIChlbmREYXRlIDwgc3RhcnREYXRlKSByZXR1cm4gbmV3IFZhbGlkYXRpb24oZmFsc2UsIFwiRW5kIGRhdGUgbXVzdCBiZSBhZnRlciBzdGFydCBkYXRlLlwiKTtcclxuICAgICAgICAgICAgLy8gZGF0ZSByZXByZXNlbnRzIGFnZSAxOCBvciBvbGRlclxyXG4gICAgICAgICAgICBpZiAoIXRoaXMuc3RhcnQuaXNBZHVsdCgpKSByZXR1cm4gbmV3IFZhbGlkYXRpb24oZmFsc2UsIFwiU3RhcnQgZGF0ZSBhbmQgZW5kIGRhdGUgbXVzdCBiZSBBZ2UgMTggb3Igb2xkZXIuXCIpO1xyXG4gICAgICAgICAgICByZXR1cm4gbmV3IFZhbGlkYXRpb24odHJ1ZSwgXCJcIik7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIHRvU3RyaW5nKCkge1xyXG4gICAgICAgICAgICByZXR1cm4gYCR7dGhpcy5zdGFydH0gdG8gJHt0aGlzLmVuZH1gO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBleHBvcnQgY2xhc3MgRG9iIHtcclxuICAgICAgICB5ZWFyOiBudW1iZXIgPSAwO1xyXG4gICAgICAgIG1vbnRoOiBudW1iZXIgPSAwO1xyXG4gICAgICAgIGNvbnN0cnVjdG9yKGRhdGU/OiBzdHJpbmcpIHtcclxuICAgICAgICAgICAgLy8gcGFyc2Ugb25seSBpZiBkYXRlIGluIG1tLXl5eXkgZm9ybWF0XHJcbiAgICAgICAgICAgIGNvbnN0IHJlZ2V4ID0gL14oMFsxLTldfDFbMC0yXSktKFxcZHs0fSkvO1xyXG4gICAgICAgICAgICBsZXQgbTogUmVnRXhwRXhlY0FycmF5O1xyXG4gICAgICAgICAgICBpZiAoKG0gPSByZWdleC5leGVjKGRhdGUpKSAhPT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgdGhpcy5tb250aCA9IE51bWJlcihtWzFdKTtcclxuICAgICAgICAgICAgICAgIHRoaXMueWVhciA9IE51bWJlcihtWzJdKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuICAgICAgICBpc1ZhbGlkKCkge1xyXG4gICAgICAgICAgICBpZiAodGhpcy5tb250aCA9PT0gMCB8fCB0aGlzLnllYXIgPT09IDApIHJldHVybiBuZXcgVmFsaWRhdGlvbihmYWxzZSwgXCJZZWFyIGFuZCBNb250aCBtdXN0IGJlIHBvcHVsYXRlZFwiKTtcclxuICAgICAgICAgICAgcmV0dXJuIG5ldyBWYWxpZGF0aW9uKHRydWUsIFwiXCIpO1xyXG4gICAgICAgIH1cclxuICAgICAgICBpc0FkdWx0KCkge1xyXG4gICAgICAgICAgICBjb25zdCB0b2RheSA9IG5ldyBEYXRlKCk7XHJcbiAgICAgICAgICAgIGxldCBhZ2UgPSB0b2RheS5nZXRGdWxsWWVhcigpIC0gdGhpcy55ZWFyO1xyXG4gICAgICAgICAgICBjb25zdCBtID0gdG9kYXkuZ2V0TW9udGgoKSAtIHRoaXMubW9udGg7XHJcbiAgICAgICAgICAgIGlmIChtIDwgMCB8fCAobSA9PT0gMCAmJiB0b2RheS5nZXREYXRlKCkgPCBuZXcgRGF0ZSh0aGlzLnllYXIsIHRoaXMubW9udGgpLmdldERhdGUoKSkpIHtcclxuICAgICAgICAgICAgICAgIGFnZS0tO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIHJldHVybiBhZ2UgPj0gMTg7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIHRvU3RyaW5nKCkge1xyXG4gICAgICAgICAgICByZXR1cm4gYCR7KHRoaXMubW9udGggPT09IDEwIHx8IHRoaXMubW9udGggPT09IDExIHx8IHRoaXMubW9udGggPT09IDEyKSA/IHRoaXMubW9udGggOiBgMCR7dGhpcy5tb250aH1gfS0ke3RoaXMueWVhcn1gO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuICAgXHJcbiAgICBleHBvcnQgY2xhc3MgVmFsdWVMYWJlbCB7XHJcbiAgICAgICAgaWQ6IHN0cmluZztcclxuXHJcbiAgICAgICAgY29uc3RydWN0b3IocHVibGljIG5hbWU6IHN0cmluZywgcHVibGljIGxhYmVsPzogc3RyaW5nKSB7XHJcbiAgICAgICAgICAgIHRoaXMuaWQgPSBnZW5lcmF0ZVV1aWQoKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgZXhwb3J0IGNsYXNzIFZhbGlkYXRpb24ge1xyXG4gICAgICAgIGNvbnN0cnVjdG9yKHB1YmxpYyByZXN1bHQ6IGJvb2xlYW4sIHB1YmxpYyBtZXNzYWdlOiBzdHJpbmcpIHt9XHJcbiAgICB9XHJcblxyXG4gICAgZnVuY3Rpb24gZ2VuZXJhdGVVdWlkKCkge1xyXG4gICAgICAgIGxldCBkID0gbmV3IERhdGUoKS5nZXRUaW1lKCk7XHJcbiAgICAgICAgY29uc3QgdXVpZCA9IFwieHh4eHh4eHgteHh4eC00eHh4LXl4eHgteHh4eHh4eHh4eHh4XCIucmVwbGFjZSgvW3h5XS9nLFxyXG4gICAgICAgICAgICBjID0+IHtcclxuICAgICAgICAgICAgICAgIHZhciByID0gKGQgKyBNYXRoLnJhbmRvbSgpICogMTYpICUgMTYgfCAwO1xyXG4gICAgICAgICAgICAgICAgZCA9IE1hdGguZmxvb3IoZCAvIDE2KTtcclxuICAgICAgICAgICAgICAgIHJldHVybiAoYyA9PT0gXCJ4XCIgPyByIDogKHIgJiAweDcgfCAweDgpKS50b1N0cmluZygxNik7XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgIHJldHVybiB1dWlkO1xyXG4gICAgfTtcclxuXHJcbn0iXX0=