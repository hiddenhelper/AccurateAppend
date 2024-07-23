var AccurateAppend;
(function (AccurateAppend) {
    var ListBuilder;
    (function (ListBuilder) {
        var GeographyTabViewModel = (function () {
            function GeographyTabViewModel(urls) {
                var _this = this;
                this.urls = urls;
                this.citiesTabOnChangeState = function (event, vm) {
                    var selector = "#collapseCities";
                    $(selector + " #cityCityOptions div").remove();
                    if ($(event.target).val() === "")
                        return false;
                    var cities = _this.getCities($(event.target).val());
                    var columns = 3;
                    var elementsPerColumn = Math.ceil(cities.length / columns) + 1;
                    for (var col = 0; col < columns; col++) {
                        var div = $('<div class="col-md-4"></div>');
                        $.each(cities, function (i, v) {
                            if (col === 0 && (i >= elementsPerColumn))
                                return true;
                            if (col === 1 && (i < elementsPerColumn || i >= elementsPerColumn * 2))
                                return true;
                            if (col === 2 && (i < (elementsPerColumn * 2)))
                                return true;
                            div.append("<div class=\"checkbox\"><label><input type='checkbox' name='cities' id='" + v.id + "' value='" + ko.toJSON(v) + "' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;" + v.name + "</label></div>");
                        });
                        $(selector + " #cityCityOptions").append(div);
                    }
                    if (cities.length > 0) {
                        $(selector + " #cityCityControlGroup #cityCount").text("Displaying " + cities.length + (cities.length > 1 ? " cities" : " city"));
                        $(selector + " #cityCityControlGroup").show();
                    }
                    ko.cleanNode($(selector + " #cityCityOptions")[0]);
                    ko.applyBindings(vm, $(selector + " #cityCityOptions")[0]);
                };
                this.countiesTabOnChangeState = function (event, vm) {
                    var selector = "#collapseCounty";
                    $(selector + " #countyCountyOptions div").remove();
                    var counties = _this.getCounties($(event.target).val());
                    var columns = 3;
                    var elementsPerColumn = Math.ceil(counties.length / columns) + 1;
                    for (var col = 0; col < columns; col++) {
                        var div = $('<div class="col-md-4"></div>');
                        $.each(counties, function (i, v) {
                            if (col === 0 && (i >= elementsPerColumn))
                                return true;
                            if (col === 1 && (i < elementsPerColumn || i >= elementsPerColumn * 2))
                                return true;
                            if (col === 2 && (i < (elementsPerColumn * 2)))
                                return true;
                            div.append("<div class=\"checkbox\"><label><input type='checkbox' name='counties' id='" + v.id + "' value='" + ko.toJSON(v) + "' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;" + v.name + "</label></div>");
                        });
                        $(selector + " #countyCountyOptions").append(div);
                    }
                    if (counties.length > 0) {
                        $(selector + " #countyControlGroup #countyCount").text("Displaying " + counties.length + (counties.length > 1 ? " counties" : " county"));
                        $(selector + " #countyControlGroup").show();
                    }
                    ko.cleanNode($(selector + " #countyCountyOptions")[0]);
                    ko.applyBindings(vm, $(selector + " #countyCountyOptions")[0]);
                };
                this.zipsTabOnChangeState = function (event, vm) {
                    var selector = "#collapseZips";
                    $(selector + " #zipZipOptions div").remove();
                    if ($(event.target).val() === "")
                        return false;
                    var zips = _this.getZips($(event.target).val());
                    var elementsPerColumn = Math.ceil(zips.length / 3) + 1;
                    for (var col = 0; col < 3; col++) {
                        var div = $('<div class="col-md-4"></div>');
                        $.each(zips, function (i, v) {
                            if (col === 0 && (i >= elementsPerColumn))
                                return true;
                            if (col === 1 && (i < elementsPerColumn || i >= elementsPerColumn * 2))
                                return true;
                            if (col === 2 && (i < (elementsPerColumn * 2) || i >= elementsPerColumn * 3))
                                return true;
                            if (col === 3 && (i < (elementsPerColumn * 3) || i >= elementsPerColumn * 4))
                                return true;
                            if (col === 4 && (i < (elementsPerColumn * 4)))
                                return true;
                            div.append("<div class=\"checkbox\"><label><input type='checkbox' name='zips' id='" + v.id + "' value='" + ko.toJSON(v) + "' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;" + v.name + "</label></div>");
                        });
                        $(selector + " #zipZipOptions").append(div);
                    }
                    if (zips.length > 0) {
                        $(selector + " #zipZipControlGroup #zipCount").text("Displaying " + zips.length + (zips.length > 1 ? " zips" : " zip"));
                        $(selector + " #zipZipControlGroup").show();
                    }
                    ko.cleanNode($(selector + " #zipZipOptions")[0]);
                    ko.applyBindings(vm, $(selector + " #zipZipOptions")[0]);
                };
                this.statesTabInitialize();
                this.initializeTimeZoneSection("#timeZoneBody");
            }
            GeographyTabViewModel.prototype.statesTabInitialize = function () {
                var selector = "#collapseState #stateOptions";
                $(selector + " div").remove();
                var states = this.getStates();
                var columns = 3;
                var elementsPerColumn = Math.ceil(states.length / columns) + 1;
                for (var col = 0; col < columns; col++) {
                    var div = $('<div class="col-md-4"></div>');
                    $.each(states, function (i, v) {
                        if (col === 0 && (i >= elementsPerColumn))
                            return true;
                        if (col === 1 && (i < elementsPerColumn || i >= elementsPerColumn * 2))
                            return true;
                        if (col === 2 && (i < (elementsPerColumn * 2)))
                            return true;
                        div.append("<div class=\"checkbox\"><label><input type='checkbox' name='states' id='" + v.id + "' value='" + ko.toJSON(v) + "' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' style='margin-right: 3px;' />" + v.stateFullName + " (" + v.abbreviation + ")</label></div>");
                    });
                    $(selector).append(div);
                }
            };
            GeographyTabViewModel.prototype.initializeTimeZoneSection = function (selector) {
                var inputs = this.getTimeZones();
                var div = $(selector);
                $.each(inputs, function (i, v) {
                    div.append("<div class=\"checkbox\"><label><input type='checkbox' name='timeZones' id='" + v.id + "' value='" + ko.toJSON(v) + "' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;" + v.label + "</label></div>");
                    $(selector).append(div);
                });
            };
            GeographyTabViewModel.prototype.getCounties = function (state) {
                var counties = new Array();
                if (!state)
                    return counties;
                $.ajax({
                    type: "GET",
                    url: this.getUrl("GetCounties") + "?state=" + state.toString(),
                    async: false,
                    success: function (data) {
                        $.each(data, function (i, v) { counties.push(new ListBuilder.County(v.Name, v.Fips, new ListBuilder.State(v.State, v.StateFIPS))); });
                    }
                });
                return counties;
            };
            GeographyTabViewModel.prototype.getZips = function (state) {
                var zips = new Array();
                $.ajax({
                    type: "GET",
                    url: this.getUrl("GetZips") + "?state=" + state.toString(),
                    async: false,
                    success: function (data) {
                        $.each(data, function (i, v) { zips.push(new ListBuilder.Zip(v.Name, new ListBuilder.State(v.State, v.StateFIPS))); });
                    }
                });
                return zips;
            };
            GeographyTabViewModel.prototype.getCities = function (state) {
                var cities = new Array();
                $.ajax({
                    type: "GET",
                    url: this.getUrl("GetCities") + "?state=" + state.toString(),
                    async: false,
                    success: function (data) {
                        $.each(data, function (i, v) { cities.push(new ListBuilder.City(v.Name, new ListBuilder.State(v.State, v.StateFIPS))); });
                    }
                });
                return cities;
            };
            GeographyTabViewModel.prototype.getStates = function () {
                var states = new Array();
                $.ajax({
                    type: "GET",
                    url: this.getUrl("GetStates"),
                    async: false,
                    success: function (data) {
                        $.each(data, function (i, v) {
                            states.push(new ListBuilder.State(v.Abbreviation, v.Fips, v.StateFullName));
                        });
                    }
                });
                return states;
            };
            GeographyTabViewModel.prototype.getTimeZones = function () {
                return [
                    new ListBuilder.ValueLabel("C", "Central Time Zone"),
                    new ListBuilder.ValueLabel("E", "Eastern Time Zone"),
                    new ListBuilder.ValueLabel("H", "Hawaii / Alaska Time Zone"),
                    new ListBuilder.ValueLabel("M", "Mountain Time Zone"),
                    new ListBuilder.ValueLabel("P", "Pacific Time Zone"),
                    new ListBuilder.ValueLabel("U", "Unknown")
                ];
            };
            GeographyTabViewModel.prototype.getUrl = function (urlName) {
                var match = jQuery.grep(this.urls, function (n) { return n.name === urlName; });
                return match[0].url;
            };
            return GeographyTabViewModel;
        }());
        ListBuilder.GeographyTabViewModel = GeographyTabViewModel;
        var Url = (function () {
            function Url(name, url) {
                this.name = name;
                this.url = url;
            }
            return Url;
        }());
        ListBuilder.Url = Url;
    })(ListBuilder = AccurateAppend.ListBuilder || (AccurateAppend.ListBuilder = {}));
})(AccurateAppend || (AccurateAppend = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiR2VvZ3JhcGh5VGFiVmlld01vZGVsLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiR2VvZ3JhcGh5VGFiVmlld01vZGVsLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLElBQU8sY0FBYyxDQWtOcEI7QUFsTkQsV0FBTyxjQUFjO0lBQUMsSUFBQSxXQUFXLENBa05oQztJQWxOcUIsV0FBQSxXQUFXO1FBRTdCO1lBRUksK0JBQW1CLElBQWdCO2dCQUFuQyxpQkFJQztnQkFKa0IsU0FBSSxHQUFKLElBQUksQ0FBWTtnQkEwQm5DLDJCQUFzQixHQUFHLFVBQUMsS0FBVSxFQUFFLEVBQU87b0JBQ3pDLElBQU0sUUFBUSxHQUFHLGlCQUFpQixDQUFDO29CQUNuQyxDQUFDLENBQUMsUUFBUSxHQUFHLHVCQUF1QixDQUFDLENBQUMsTUFBTSxFQUFFLENBQUM7b0JBQy9DLElBQUksQ0FBQyxDQUFDLEtBQUssQ0FBQyxNQUFNLENBQUMsQ0FBQyxHQUFHLEVBQUUsS0FBSyxFQUFFO3dCQUFFLE9BQU8sS0FBSyxDQUFDO29CQUMvQyxJQUFNLE1BQU0sR0FBRyxLQUFJLENBQUMsU0FBUyxDQUFDLENBQUMsQ0FBQyxLQUFLLENBQUMsTUFBTSxDQUFDLENBQUMsR0FBRyxFQUFFLENBQUMsQ0FBQztvQkFDckQsSUFBTSxPQUFPLEdBQUcsQ0FBQyxDQUFDO29CQUNsQixJQUFJLGlCQUFpQixHQUFHLElBQUksQ0FBQyxJQUFJLENBQUMsTUFBTSxDQUFDLE1BQU0sR0FBRyxPQUFPLENBQUMsR0FBRyxDQUFDLENBQUM7b0JBQy9ELEtBQUssSUFBSSxHQUFHLEdBQUcsQ0FBQyxFQUFFLEdBQUcsR0FBRyxPQUFPLEVBQUUsR0FBRyxFQUFFLEVBQUU7d0JBQ3BDLElBQUksR0FBRyxHQUFHLENBQUMsQ0FBQyw4QkFBOEIsQ0FBQyxDQUFDO3dCQUM1QyxDQUFDLENBQUMsSUFBSSxDQUFDLE1BQU0sRUFBRSxVQUFDLENBQUMsRUFBRSxDQUFDOzRCQUNoQixJQUFJLEdBQUcs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RCxJQUFJLFdBQVcsQ0FBQyxVQUFVLENBQUMsR0FBRyxFQUFFLFNBQVMsQ0FBQztpQkFDN0MsQ0FBQztZQUNOLENBQUM7WUFFRCxzQ0FBTSxHQUFOLFVBQU8sT0FBZTtnQkFDbEIsSUFBTSxLQUFLLEdBQUcsTUFBTSxDQUFDLElBQUksQ0FBQyxJQUFJLENBQUMsSUFBSSxFQUFFLFVBQUMsQ0FBQyxJQUFLLE9BQUEsQ0FBQyxDQUFDLElBQUksS0FBSyxPQUFPLEVBQWxCLENBQWtCLENBQUMsQ0FBQztnQkFDaEUsT0FBTyxLQUFLLENBQUMsQ0FBQyxDQUFDLENBQUMsR0FBRyxDQUFDO1lBQ3hCLENBQUM7WUFFTCw0QkFBQztRQUFELENBQUMsQUF6TUQsSUF5TUM7UUF6TVksaUNBQXFCLHdCQXlNakMsQ0FBQTtRQUVEO1lBQ0ksYUFBbUIsSUFBWSxFQUFTLEdBQVc7Z0JBQWhDLFNBQUksR0FBSixJQUFJLENBQVE7Z0JBQVMsUUFBRyxHQUFILEdBQUcsQ0FBUTtZQUNuRCxDQUFDO1lBQ0wsVUFBQztRQUFELENBQUMsQUFIRCxJQUdDO1FBSFksZUFBRyxNQUdmLENBQUE7SUFFTCxDQUFDLEVBbE5xQixXQUFXLEdBQVgsMEJBQVcsS0FBWCwwQkFBVyxRQWtOaEM7QUFBRCxDQUFDLEVBbE5NLGNBQWMsS0FBZCxjQUFjLFFBa05wQiIsInNvdXJjZXNDb250ZW50IjpbIm1vZHVsZSBBY2N1cmF0ZUFwcGVuZC5MaXN0QnVpbGRlciB7XHJcblxyXG4gICAgZXhwb3J0IGNsYXNzIEdlb2dyYXBoeVRhYlZpZXdNb2RlbCB7XHJcblxyXG4gICAgICAgIGNvbnN0cnVjdG9yKHB1YmxpYyB1cmxzOiBBcnJheTxVcmw+KSB7XHJcbiAgICAgICAgICAgIC8vIFRBQlNcclxuICAgICAgICAgICAgdGhpcy5zdGF0ZXNUYWJJbml0aWFsaXplKCk7XHJcbiAgICAgICAgICAgIHRoaXMuaW5pdGlhbGl6ZVRpbWVab25lU2VjdGlvbihcIiN0aW1lWm9uZUJvZHlcIik7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAvLyBsb2FkcyBzdGF0ZXMgc2VjdGlvblxyXG4gICAgICAgIHN0YXRlc1RhYkluaXRpYWxpemUoKSB7XHJcbiAgICAgICAgICAgIGNvbnN0IHNlbGVjdG9yID0gXCIjY29sbGFwc2VTdGF0ZSAjc3RhdGVPcHRpb25zXCI7XHJcbiAgICAgICAgICAgICQoc2VsZWN0b3IgKyBcIiBkaXZcIikucmVtb3ZlKCk7XHJcbiAgICAgICAgICAgIGNvbnN0IHN0YXRlcyA9IHRoaXMuZ2V0U3RhdGVzKCk7XHJcbiAgICAgICAgICAgIGNvbnN0IGNvbHVtbnMgPSAzO1xyXG4gICAgICAgICAgICB2YXIgZWxlbWVudHNQZXJDb2x1bW4gPSBNYXRoLmNlaWwoc3RhdGVzLmxlbmd0aCAvIGNvbHVtbnMpICsgMTtcclxuICAgICAgICAgICAgZm9yICh2YXIgY29sID0gMDsgY29sIDwgY29sdW1uczsgY29sKyspIHtcclxuICAgICAgICAgICAgICAgIHZhciBkaXYgPSAkKCc8ZGl2IGNsYXNzPVwiY29sLW1kLTRcIj48L2Rpdj4nKTtcclxuICAgICAgICAgICAgICAgICQuZWFjaChzdGF0ZXMsIChpLCB2KSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKGNvbCA9PT0gMCAmJiAoaSA+PSBlbGVtZW50c1BlckNvbHVtbikpIHJldHVybiB0cnVlO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChjb2wgPT09IDEgJiYgKGkgPCBlbGVtZW50c1BlckNvbHVtbiB8fCBpID49IGVsZW1lbnRzUGVyQ29sdW1uICogMikpIHJldHVybiB0cnVlO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChjb2wgPT09IDIgJiYgKGkgPCAoZWxlbWVudHNQZXJDb2x1bW4gKiAyKSkpIHJldHVybiB0cnVlO1xyXG4gICAgICAgICAgICAgICAgICAgIGRpdi5hcHBlbmQoYDxkaXYgY2xhc3M9XCJjaGVja2JveFwiPjxsYWJlbD48aW5wdXQgdHlwZT0nY2hlY2tib3gnIG5hbWU9J3N0YXRlcycgaWQ9JyR7di5pZH0nIHZhbHVlPScke2tvLnRvSlNPTih2KX0nIGRhdGEtYmluZD0nY2xpY2s6IGZ1bmN0aW9uIChkYXRhLCBldmVudCkgeyBsaXN0Q3JpdGVyaWFWaWV3TW9kZWwudG9nZ2xlKGV2ZW50KTsgcmV0dXJuIHRydWU7IH0nIHN0eWxlPSdtYXJnaW4tcmlnaHQ6IDNweDsnIC8+JHt2LnN0YXRlRnVsbE5hbWV9ICgke3YuYWJicmV2aWF0aW9ufSk8L2xhYmVsPjwvZGl2PmApO1xyXG4gICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICAkKHNlbGVjdG9yKS5hcHBlbmQoZGl2KTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8gcG9wdWxhdGVzIGNpdGllcyBzZWN0aW9uIHdpdGggY2l0aWVzIG1hdGhpbmcgdGhlIGNob3NlbiBzdGF0ZVxyXG4gICAgICAgIGNpdGllc1RhYk9uQ2hhbmdlU3RhdGUgPSAoZXZlbnQ6IGFueSwgdm06IGFueSkgPT4ge1xyXG4gICAgICAgICAgICBjb25zdCBzZWxlY3RvciA9IFwiI2NvbGxhcHNlQ2l0aWVzXCI7XHJcbiAgICAgICAgICAgICQoc2VsZWN0b3IgKyBcIiAjY2l0eUNpdHlPcHRpb25zIGRpdlwiKS5yZW1vdmUoKTtcclxuICAgICAgICAgICAgaWYgKCQoZXZlbnQudGFyZ2V0KS52YWwoKSA9PT0gXCJcIikgcmV0dXJuIGZhbHNlO1xyXG4gICAgICAgICAgICBjb25zdCBjaXRpZXMgPSB0aGlzLmdldENpdGllcygkKGV2ZW50LnRhcmdldCkudmFsKCkpO1xyXG4gICAgICAgICAgICBjb25zdCBjb2x1bW5zID0gMztcclxuICAgICAgICAgICAgdmFyIGVsZW1lbnRzUGVyQ29sdW1uID0gTWF0aC5jZWlsKGNpdGllcy5sZW5ndGggLyBjb2x1bW5zKSArIDE7XHJcbiAgICAgICAgICAgIGZvciAodmFyIGNvbCA9IDA7IGNvbCA8IGNvbHVtbnM7IGNvbCsrKSB7XHJcbiAgICAgICAgICAgICAgICB2YXIgZGl2ID0gJCgnPGRpdiBjbGFzcz1cImNvbC1tZC00XCI+PC9kaXY+Jyk7XHJcbiAgICAgICAgICAgICAgICAkLmVhY2goY2l0aWVzLCAoaSwgdikgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChjb2wgPT09IDAgJiYgKGkgPj0gZWxlbWVudHNQZXJDb2x1bW4pKSByZXR1cm4gdHJ1ZTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoY29sID09PSAxICYmIChpIDwgZWxlbWVudHNQZXJDb2x1bW4gfHwgaSA+PSBlbGVtZW50c1BlckNvbHVtbiAqIDIpKSByZXR1cm4gdHJ1ZTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoY29sID09PSAyICYmIChpIDwgKGVsZW1lbnRzUGVyQ29sdW1uICogMikpKSByZXR1cm4gdHJ1ZTtcclxuICAgICAgICAgICAgICAgICAgICBkaXYuYXBwZW5kKGA8ZGl2IGNsYXNzPVwiY2hlY2tib3hcIj48bGFiZWw+PGlucHV0IHR5cGU9J2NoZWNrYm94JyBuYW1lPSdjaXRpZXMnIGlkPScke3YuaWR9JyB2YWx1ZT0nJHtrby50b0pTT04odil9JyBkYXRhLWJpbmQ9J2NsaWNrOiBmdW5jdGlvbiAoZGF0YSwgZXZlbnQpIHsgbGlzdENyaXRlcmlhVmlld01vZGVsLnRvZ2dsZShldmVudCk7IHJldHVybiB0cnVlOyB9JyAvPiZuYnNwOyR7di5uYW1lfTwvbGFiZWw+PC9kaXY+YCk7XHJcbiAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgICQoc2VsZWN0b3IgKyBcIiAjY2l0eUNpdHlPcHRpb25zXCIpLmFwcGVuZChkaXYpO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIGlmIChjaXRpZXMubGVuZ3RoID4gMCkge1xyXG4gICAgICAgICAgICAgICAgJChzZWxlY3RvciArIFwiICNjaXR5Q2l0eUNvbnRyb2xHcm91cCAjY2l0eUNvdW50XCIpLnRleHQoYERpc3BsYXlpbmcgJHtjaXRpZXMubGVuZ3RofSR7Y2l0aWVzLmxlbmd0aCA+IDEgPyBcIiBjaXRpZXNcIiA6IFwiIGNpdHlcIn1gKTtcclxuICAgICAgICAgICAgICAgICQoc2VsZWN0b3IgKyBcIiAjY2l0eUNpdHlDb250cm9sR3JvdXBcIikuc2hvdygpO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIGtvLmNsZWFuTm9kZSgkKHNlbGVjdG9yICsgXCIgI2NpdHlDaXR5T3B0aW9uc1wiKVswXSk7XHJcbiAgICAgICAgICAgIGtvLmFwcGx5QmluZGluZ3Modm0sICQoc2VsZWN0b3IgKyBcIiAjY2l0eUNpdHlPcHRpb25zXCIpWzBdKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIC8vIGxvYWRzIGNvdW50aWVzIHNlY3Rpb24gd2l0aCBjb3VudGllcyBtYXRoaW5nIHRoZSBjaG9zZW4gc3RhdGVcclxuICAgICAgICBjb3VudGllc1RhYk9uQ2hhbmdlU3RhdGUgPSAoZXZlbnQ6IGFueSwgdm06IGFueSkgPT4ge1xyXG4gICAgICAgICAgICBjb25zdCBzZWxlY3RvciA9IFwiI2NvbGxhcHNlQ291bnR5XCI7XHJcbiAgICAgICAgICAgICQoc2VsZWN0b3IgKyBcIiAjY291bnR5Q291bnR5T3B0aW9ucyBkaXZcIikucmVtb3ZlKCk7XHJcbiAgICAgICAgICAgIGNvbnN0IGNvdW50aWVzID0gdGhpcy5nZXRDb3VudGllcygkKGV2ZW50LnRhcmdldCkudmFsKCkpO1xyXG4gICAgICAgICAgICBjb25zdCBjb2x1bW5zID0gMztcclxuICAgICAgICAgICAgdmFyIGVsZW1lbnRzUGVyQ29sdW1uID0gTWF0aC5jZWlsKGNvdW50aWVzLmxlbmd0aCAvIGNvbHVtbnMpICsgMTtcclxuICAgICAgICAgICAgZm9yICh2YXIgY29sID0gMDsgY29sIDwgY29sdW1uczsgY29sKyspIHtcclxuICAgICAgICAgICAgICAgIHZhciBkaXYgPSAkKCc8ZGl2IGNsYXNzPVwiY29sLW1kLTRcIj48L2Rpdj4nKTtcclxuICAgICAgICAgICAgICAgICQuZWFjaChjb3VudGllcywgKGksIHYpID0+IHtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoY29sID09PSAwICYmIChpID49IGVsZW1lbnRzUGVyQ29sdW1uKSkgcmV0dXJuIHRydWU7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKGNvbCA9PT0gMSAmJiAoaSA8IGVsZW1lbnRzUGVyQ29sdW1uIHx8IGkgPj0gZWxlbWVudHNQZXJDb2x1bW4gKiAyKSkgcmV0dXJuIHRydWU7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKGNvbCA9PT0gMiAmJiAoaSA8IChlbGVtZW50c1BlckNvbHVtbiAqIDIpKSkgcmV0dXJuIHRydWU7XHJcbiAgICAgICAgICAgICAgICAgICAgZGl2LmFwcGVuZChgPGRpdiBjbGFzcz1cImNoZWNrYm94XCI+PGxhYmVsPjxpbnB1dCB0eXBlPSdjaGVja2JveCcgbmFtZT0nY291bnRpZXMnIGlkPScke3YuaWR9JyB2YWx1ZT0nJHtrby50b0pTT04odil9JyBkYXRhLWJpbmQ9J2NsaWNrOiBmdW5jdGlvbiAoZGF0YSwgZXZlbnQpIHsgbGlzdENyaXRlcmlhVmlld01vZGVsLnRvZ2dsZShldmVudCk7IHJldHVybiB0cnVlOyB9JyAvPiZuYnNwOyR7di5uYW1lfTwvbGFiZWw+PC9kaXY+YCk7XHJcbiAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgICQoc2VsZWN0b3IgKyBcIiAjY291bnR5Q291bnR5T3B0aW9uc1wiKS5hcHBlbmQoZGl2KTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICBpZiAoY291bnRpZXMubGVuZ3RoID4gMCkge1xyXG4gICAgICAgICAgICAgICAgJChzZWxlY3RvciArIFwiICNjb3VudHlDb250cm9sR3JvdXAgI2NvdW50eUNvdW50XCIpLnRleHQoYERpc3BsYXlpbmcgJHtjb3VudGllcy5sZW5ndGh9JHtjb3VudGllcy5sZW5ndGggPiAxID8gXCIgY291bnRpZXNcIiA6IFwiIGNvdW50eVwifWApO1xyXG4gICAgICAgICAgICAgICAgJChzZWxlY3RvciArIFwiICNjb3VudHlDb250cm9sR3JvdXBcIikuc2hvdygpO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIGtvLmNsZWFuTm9kZSgkKHNlbGVjdG9yICsgXCIgI2NvdW50eUNvdW50eU9wdGlvbnNcIilbMF0pO1xyXG4gICAgICAgICAgICBrby5hcHBseUJpbmRpbmdzKHZtLCAkKHNlbGVjdG9yICsgXCIgI2NvdW50eUNvdW50eU9wdGlvbnNcIilbMF0pO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8gcG9wdWxhdGVzIGNpdGllcyBzZWN0aW9uIHdpdGggY2l0aWVzIG1hdGhpbmcgdGhlIGNob3NlbiBzdGF0ZVxyXG4gICAgICAgIHppcHNUYWJPbkNoYW5nZVN0YXRlID0gKGV2ZW50OiBhbnksIHZtOiBhbnkpID0+IHtcclxuICAgICAgICAgICAgY29uc3Qgc2VsZWN0b3IgPSBcIiNjb2xsYXBzZVppcHNcIjtcclxuICAgICAgICAgICAgJChzZWxlY3RvciArIFwiICN6aXBaaXBPcHRpb25zIGRpdlwiKS5yZW1vdmUoKTtcclxuICAgICAgICAgICAgaWYgKCQoZXZlbnQudGFyZ2V0KS52YWwoKSA9PT0gXCJcIikgcmV0dXJuIGZhbHNlO1xyXG4gICAgICAgICAgICBjb25zdCB6aXBzID0gdGhpcy5nZXRaaXBzKCQoZXZlbnQudGFyZ2V0KS52YWwoKSk7XHJcbiAgICAgICAgICAgIHZhciBlbGVtZW50c1BlckNvbHVtbiA9IE1hdGguY2VpbCh6aXBzLmxlbmd0aCAvIDMpICsgMTtcclxuICAgICAgICAgICAgZm9yICh2YXIgY29sID0gMDsgY29sIDwgMzsgY29sKyspIHtcclxuICAgICAgICAgICAgICAgIHZhciBkaXYgPSAkKCc8ZGl2IGNsYXNzPVwiY29sLW1kLTRcIj48L2Rpdj4nKTtcclxuICAgICAgICAgICAgICAgICQuZWFjaCh6aXBzLCAoaSwgdikgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChjb2wgPT09IDAgJiYgKGkgPj0gZWxlbWVudHNQZXJDb2x1bW4pKSByZXR1cm4gdHJ1ZTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoY29sID09PSAxICYmIChpIDwgZWxlbWVudHNQZXJDb2x1bW4gfHwgaSA+PSBlbGVtZW50c1BlckNvbHVtbiAqIDIpKSByZXR1cm4gdHJ1ZTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoY29sID09PSAyICYmIChpIDwgKGVsZW1lbnRzUGVyQ29sdW1uICogMikgfHwgaSA+PSBlbGVtZW50c1BlckNvbHVtbiAqIDMpKSByZXR1cm4gdHJ1ZTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoY29sID09PSAzICYmIChpIDwgKGVsZW1lbnRzUGVyQ29sdW1uICogMykgfHwgaSA+PSBlbGVtZW50c1BlckNvbHVtbiAqIDQpKSByZXR1cm4gdHJ1ZTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoY29sID09PSA0ICYmIChpIDwgKGVsZW1lbnRzUGVyQ29sdW1uICogNCkpKSByZXR1cm4gdHJ1ZTtcclxuICAgICAgICAgICAgICAgICAgICBkaXYuYXBwZW5kKGA8ZGl2IGNsYXNzPVwiY2hlY2tib3hcIj48bGFiZWw+PGlucHV0IHR5cGU9J2NoZWNrYm94JyBuYW1lPSd6aXBzJyBpZD0nJHt2LmlkfScgdmFsdWU9JyR7a28udG9KU09OKHYpfScgZGF0YS1iaW5kPSdjbGljazogZnVuY3Rpb24gKGRhdGEsIGV2ZW50KSB7IGxpc3RDcml0ZXJpYVZpZXdNb2RlbC50b2dnbGUoZXZlbnQpOyByZXR1cm4gdHJ1ZTsgfScgLz4mbmJzcDske3YubmFtZX08L2xhYmVsPjwvZGl2PmApO1xyXG4gICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICAkKHNlbGVjdG9yICsgXCIgI3ppcFppcE9wdGlvbnNcIikuYXBwZW5kKGRpdik7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgaWYgKHppcHMubGVuZ3RoID4gMCkge1xyXG4gICAgICAgICAgICAgICAgJChzZWxlY3RvciArIFwiICN6aXBaaXBDb250cm9sR3JvdXAgI3ppcENvdW50XCIpLnRleHQoYERpc3BsYXlpbmcgJHt6aXBzLmxlbmd0aH0ke3ppcHMubGVuZ3RoID4gMSA/IFwiIHppcHNcIiA6IFwiIHppcFwifWApO1xyXG4gICAgICAgICAgICAgICAgJChzZWxlY3RvciArIFwiICN6aXBaaXBDb250cm9sR3JvdXBcIikuc2hvdygpO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIGtvLmNsZWFuTm9kZSgkKHNlbGVjdG9yICsgXCIgI3ppcFppcE9wdGlvbnNcIilbMF0pO1xyXG4gICAgICAgICAgICBrby5hcHBseUJpbmRpbmdzKHZtLCAkKHNlbGVjdG9yICsgXCIgI3ppcFppcE9wdGlvbnNcIilbMF0pO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgXHJcblxyXG4gICAgICAgIC8vIHBvcHVsYXRlIFRpbWVab25lIHNlY3Rpb25cclxuICAgICAgICBpbml0aWFsaXplVGltZVpvbmVTZWN0aW9uKHNlbGVjdG9yKSB7XHJcblxyXG4gICAgICAgICAgICBjb25zdCBpbnB1dHMgPSB0aGlzLmdldFRpbWVab25lcygpO1xyXG5cclxuICAgICAgICAgICAgdmFyIGRpdiA9ICQoc2VsZWN0b3IpO1xyXG4gICAgICAgICAgICAkLmVhY2goaW5wdXRzLCAoaSwgdikgPT4ge1xyXG4gICAgICAgICAgICAgICAgZGl2LmFwcGVuZChgPGRpdiBjbGFzcz1cImNoZWNrYm94XCI+PGxhYmVsPjxpbnB1dCB0eXBlPSdjaGVja2JveCcgbmFtZT0ndGltZVpvbmVzJyBpZD0nJHt2LmlkfScgdmFsdWU9JyR7a28udG9KU09OKHYpfScgZGF0YS1iaW5kPSdjbGljazogZnVuY3Rpb24gKGRhdGEsIGV2ZW50KSB7IGxpc3RDcml0ZXJpYVZpZXdNb2RlbC50b2dnbGUoZXZlbnQpOyByZXR1cm4gdHJ1ZTsgfScgLz4mbmJzcDske3YubGFiZWx9PC9sYWJlbD48L2Rpdj5gKTtcclxuICAgICAgICAgICAgICAgICQoc2VsZWN0b3IpLmFwcGVuZChkaXYpO1xyXG4gICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgfVxyXG4gICAgICAgIFxyXG4gICAgICAgIC8vIExPT0tVUFNcclxuXHJcbiAgICAgICAgLy8qIEZldGNoIGxpc3Qgb2YgY291bnRpZXMgbWF0Y2hpbmcgc3RhdGUgKi9cclxuICAgICAgICBnZXRDb3VudGllcyhzdGF0ZSk6IENvdW50eVtdIHtcclxuICAgICAgICAgICAgdmFyIGNvdW50aWVzID0gbmV3IEFycmF5KCk7XHJcbiAgICAgICAgICAgIGlmICghc3RhdGUpIHJldHVybiBjb3VudGllcztcclxuICAgICAgICAgICAgJC5hamF4KFxyXG4gICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgIHR5cGU6IFwiR0VUXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgdXJsOiB0aGlzLmdldFVybChcIkdldENvdW50aWVzXCIpICsgXCI/c3RhdGU9XCIgKyBzdGF0ZS50b1N0cmluZygpLFxyXG4gICAgICAgICAgICAgICAgICAgIGFzeW5jOiBmYWxzZSxcclxuICAgICAgICAgICAgICAgICAgICBzdWNjZXNzKGRhdGEpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgJC5lYWNoKGRhdGEsIChpLCB2KSA9PiB7IGNvdW50aWVzLnB1c2gobmV3IENvdW50eSh2Lk5hbWUsIHYuRmlwcywgbmV3IFN0YXRlKHYuU3RhdGUsIHYuU3RhdGVGSVBTKSkpOyB9KTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgcmV0dXJuIGNvdW50aWVzO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8qIEZldGNoIGxpc3Qgb2YgemlwcyBtYXRjaGluZyBjaXR5IGFuZCBzdGF0ZSAqL1xyXG4gICAgICAgIGdldFppcHMoc3RhdGUpOiBaaXBbXSB7XHJcbiAgICAgICAgICAgIHZhciB6aXBzID0gbmV3IEFycmF5KCk7XHJcbiAgICAgICAgICAgICQuYWpheChcclxuICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICB0eXBlOiBcIkdFVFwiLFxyXG4gICAgICAgICAgICAgICAgICAgIHVybDogdGhpcy5nZXRVcmwoXCJHZXRaaXBzXCIpICsgXCI/c3RhdGU9XCIgKyBzdGF0ZS50b1N0cmluZygpLFxyXG4gICAgICAgICAgICAgICAgICAgIGFzeW5jOiBmYWxzZSxcclxuICAgICAgICAgICAgICAgICAgICBzdWNjZXNzKGRhdGEpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgJC5lYWNoKGRhdGEsIChpLCB2KSA9PiB7IHppcHMucHVzaChuZXcgWmlwKHYuTmFtZSwgbmV3IFN0YXRlKHYuU3RhdGUsIHYuU3RhdGVGSVBTKSkpOyB9KTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgcmV0dXJuIHppcHM7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAvLyogRmV0Y2ggbGlzdCBvZiBjaXRpZXMgbWF0Y2hpbmcgc3RhdGUgKi9cclxuICAgICAgICBnZXRDaXRpZXMoc3RhdGUpOiBDaXR5W10ge1xyXG4gICAgICAgICAgICB2YXIgY2l0aWVzID0gbmV3IEFycmF5KCk7XHJcbiAgICAgICAgICAgICQuYWpheChcclxuICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICB0eXBlOiBcIkdFVFwiLFxyXG4gICAgICAgICAgICAgICAgICAgIHVybDogdGhpcy5nZXRVcmwoXCJHZXRDaXRpZXNcIikgKyBcIj9zdGF0ZT1cIiArIHN0YXRlLnRvU3RyaW5nKCksXHJcbiAgICAgICAgICAgICAgICAgICAgYXN5bmM6IGZhbHNlLFxyXG4gICAgICAgICAgICAgICAgICAgIHN1Y2Nlc3MoZGF0YSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAkLmVhY2goZGF0YSwgKGksIHYpID0+IHsgY2l0aWVzLnB1c2gobmV3IENpdHkodi5OYW1lLCBuZXcgU3RhdGUodi5TdGF0ZSwgdi5TdGF0ZUZJUFMpKSk7IH0pO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICByZXR1cm4gY2l0aWVzO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8qIEZldGNoIGxpc3Qgc3RhdGVzICovXHJcbiAgICAgICAgZ2V0U3RhdGVzKCk6IFN0YXRlW10ge1xyXG4gICAgICAgICAgICB2YXIgc3RhdGVzID0gbmV3IEFycmF5KCk7XHJcbiAgICAgICAgICAgICQuYWpheChcclxuICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICB0eXBlOiBcIkdFVFwiLFxyXG4gICAgICAgICAgICAgICAgICAgIHVybDogdGhpcy5nZXRVcmwoXCJHZXRTdGF0ZXNcIiksXHJcbiAgICAgICAgICAgICAgICAgICAgYXN5bmM6IGZhbHNlLFxyXG4gICAgICAgICAgICAgICAgICAgIHN1Y2Nlc3MoZGF0YSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAkLmVhY2goZGF0YSwgKGksIHYpID0+IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHN0YXRlcy5wdXNoKG5ldyBTdGF0ZSh2LkFiYnJldmlhdGlvbiwgdi5GaXBzLCB2LlN0YXRlRnVsbE5hbWUpKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgIHJldHVybiBzdGF0ZXM7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIFxyXG4gICAgICAgIGdldFRpbWVab25lcygpOiBMaXN0QnVpbGRlci5WYWx1ZUxhYmVsW10ge1xyXG4gICAgICAgICAgICByZXR1cm4gW1xyXG4gICAgICAgICAgICAgICAgbmV3IExpc3RCdWlsZGVyLlZhbHVlTGFiZWwoXCJDXCIsIFwiQ2VudHJhbCBUaW1lIFpvbmVcIiksXHJcbiAgICAgICAgICAgICAgICBuZXcgTGlzdEJ1aWxkZXIuVmFsdWVMYWJlbChcIkVcIiwgXCJFYXN0ZXJuIFRpbWUgWm9uZVwiKSxcclxuICAgICAgICAgICAgICAgIG5ldyBMaXN0QnVpbGRlci5WYWx1ZUxhYmVsKFwiSFwiLCBcIkhhd2FpaSAvIEFsYXNrYSBUaW1lIFpvbmVcIiksXHJcbiAgICAgICAgICAgICAgICBuZXcgTGlzdEJ1aWxkZXIuVmFsdWVMYWJlbChcIk1cIiwgXCJNb3VudGFpbiBUaW1lIFpvbmVcIiksXHJcbiAgICAgICAgICAgICAgICBuZXcgTGlzdEJ1aWxkZXIuVmFsdWVMYWJlbChcIlBcIiwgXCJQYWNpZmljIFRpbWUgWm9uZVwiKSxcclxuICAgICAgICAgICAgICAgIG5ldyBMaXN0QnVpbGRlci5WYWx1ZUxhYmVsKFwiVVwiLCBcIlVua25vd25cIilcclxuICAgICAgICAgICAgXTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGdldFVybCh1cmxOYW1lOiBzdHJpbmcpIHtcclxuICAgICAgICAgICAgY29uc3QgbWF0Y2ggPSBqUXVlcnkuZ3JlcCh0aGlzLnVybHMsIChuKSA9PiBuLm5hbWUgPT09IHVybE5hbWUpO1xyXG4gICAgICAgICAgICByZXR1cm4gbWF0Y2hbMF0udXJsO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICB9XHJcblxyXG4gICAgZXhwb3J0IGNsYXNzIFVybCB7XHJcbiAgICAgICAgY29uc3RydWN0b3IocHVibGljIG5hbWU6IHN0cmluZywgcHVibGljIHVybDogc3RyaW5nKSB7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxufSJdfQ==