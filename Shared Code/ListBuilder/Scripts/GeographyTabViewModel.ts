module AccurateAppend.ListBuilder {

    export class GeographyTabViewModel {

        constructor(public urls: Array<Url>) {
            // TABS
            this.statesTabInitialize();
            this.initializeTimeZoneSection("#timeZoneBody");
        }

        // loads states section
        statesTabInitialize() {
            const selector = "#collapseState #stateOptions";
            $(selector + " div").remove();
            const states = this.getStates();
            const columns = 3;
            var elementsPerColumn = Math.ceil(states.length / columns) + 1;
            for (var col = 0; col < columns; col++) {
                var div = $('<div class="col-md-4"></div>');
                $.each(states, (i, v) => {
                    if (col === 0 && (i >= elementsPerColumn)) return true;
                    if (col === 1 && (i < elementsPerColumn || i >= elementsPerColumn * 2)) return true;
                    if (col === 2 && (i < (elementsPerColumn * 2))) return true;
                    div.append(`<div class="checkbox"><label><input type='checkbox' name='states' id='${v.id}' value='${ko.toJSON(v)}' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' style='margin-right: 3px;' />${v.stateFullName} (${v.abbreviation})</label></div>`);
                });
                $(selector).append(div);
            }
        }

        // populates cities section with cities mathing the chosen state
        citiesTabOnChangeState = (event: any, vm: any) => {
            const selector = "#collapseCities";
            $(selector + " #cityCityOptions div").remove();
            if ($(event.target).val() === "") return false;
            const cities = this.getCities($(event.target).val());
            const columns = 3;
            var elementsPerColumn = Math.ceil(cities.length / columns) + 1;
            for (var col = 0; col < columns; col++) {
                var div = $('<div class="col-md-4"></div>');
                $.each(cities, (i, v) => {
                    if (col === 0 && (i >= elementsPerColumn)) return true;
                    if (col === 1 && (i < elementsPerColumn || i >= elementsPerColumn * 2)) return true;
                    if (col === 2 && (i < (elementsPerColumn * 2))) return true;
                    div.append(`<div class="checkbox"><label><input type='checkbox' name='cities' id='${v.id}' value='${ko.toJSON(v)}' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${v.name}</label></div>`);
                });
                $(selector + " #cityCityOptions").append(div);
            }
            if (cities.length > 0) {
                $(selector + " #cityCityControlGroup #cityCount").text(`Displaying ${cities.length}${cities.length > 1 ? " cities" : " city"}`);
                $(selector + " #cityCityControlGroup").show();
            }
            ko.cleanNode($(selector + " #cityCityOptions")[0]);
            ko.applyBindings(vm, $(selector + " #cityCityOptions")[0]);
        }

        // loads counties section with counties mathing the chosen state
        countiesTabOnChangeState = (event: any, vm: any) => {
            const selector = "#collapseCounty";
            $(selector + " #countyCountyOptions div").remove();
            const counties = this.getCounties($(event.target).val());
            const columns = 3;
            var elementsPerColumn = Math.ceil(counties.length / columns) + 1;
            for (var col = 0; col < columns; col++) {
                var div = $('<div class="col-md-4"></div>');
                $.each(counties, (i, v) => {
                    if (col === 0 && (i >= elementsPerColumn)) return true;
                    if (col === 1 && (i < elementsPerColumn || i >= elementsPerColumn * 2)) return true;
                    if (col === 2 && (i < (elementsPerColumn * 2))) return true;
                    div.append(`<div class="checkbox"><label><input type='checkbox' name='counties' id='${v.id}' value='${ko.toJSON(v)}' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${v.name}</label></div>`);
                });
                $(selector + " #countyCountyOptions").append(div);
            }
            if (counties.length > 0) {
                $(selector + " #countyControlGroup #countyCount").text(`Displaying ${counties.length}${counties.length > 1 ? " counties" : " county"}`);
                $(selector + " #countyControlGroup").show();
            }
            ko.cleanNode($(selector + " #countyCountyOptions")[0]);
            ko.applyBindings(vm, $(selector + " #countyCountyOptions")[0]);
        }

        // populates cities section with cities mathing the chosen state
        zipsTabOnChangeState = (event: any, vm: any) => {
            const selector = "#collapseZips";
            $(selector + " #zipZipOptions div").remove();
            if ($(event.target).val() === "") return false;
            const zips = this.getZips($(event.target).val());
            var elementsPerColumn = Math.ceil(zips.length / 3) + 1;
            for (var col = 0; col < 3; col++) {
                var div = $('<div class="col-md-4"></div>');
                $.each(zips, (i, v) => {
                    if (col === 0 && (i >= elementsPerColumn)) return true;
                    if (col === 1 && (i < elementsPerColumn || i >= elementsPerColumn * 2)) return true;
                    if (col === 2 && (i < (elementsPerColumn * 2) || i >= elementsPerColumn * 3)) return true;
                    if (col === 3 && (i < (elementsPerColumn * 3) || i >= elementsPerColumn * 4)) return true;
                    if (col === 4 && (i < (elementsPerColumn * 4))) return true;
                    div.append(`<div class="checkbox"><label><input type='checkbox' name='zips' id='${v.id}' value='${ko.toJSON(v)}' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${v.name}</label></div>`);
                });
                $(selector + " #zipZipOptions").append(div);
            }
            if (zips.length > 0) {
                $(selector + " #zipZipControlGroup #zipCount").text(`Displaying ${zips.length}${zips.length > 1 ? " zips" : " zip"}`);
                $(selector + " #zipZipControlGroup").show();
            }
            ko.cleanNode($(selector + " #zipZipOptions")[0]);
            ko.applyBindings(vm, $(selector + " #zipZipOptions")[0]);
        }

        

        // populate TimeZone section
        initializeTimeZoneSection(selector) {

            const inputs = this.getTimeZones();

            var div = $(selector);
            $.each(inputs, (i, v) => {
                div.append(`<div class="checkbox"><label><input type='checkbox' name='timeZones' id='${v.id}' value='${ko.toJSON(v)}' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${v.label}</label></div>`);
                $(selector).append(div);
            });

        }
        
        // LOOKUPS

        //* Fetch list of counties matching state */
        getCounties(state): County[] {
            var counties = new Array();
            if (!state) return counties;
            $.ajax(
                {
                    type: "GET",
                    url: this.getUrl("GetCounties") + "?state=" + state.toString(),
                    async: false,
                    success(data) {
                        $.each(data, (i, v) => { counties.push(new County(v.Name, v.Fips, new State(v.State, v.StateFIPS))); });
                    }
                });
            return counties;
        }

        //* Fetch list of zips matching city and state */
        getZips(state): Zip[] {
            var zips = new Array();
            $.ajax(
                {
                    type: "GET",
                    url: this.getUrl("GetZips") + "?state=" + state.toString(),
                    async: false,
                    success(data) {
                        $.each(data, (i, v) => { zips.push(new Zip(v.Name, new State(v.State, v.StateFIPS))); });
                    }
                });
            return zips;
        }

        //* Fetch list of cities matching state */
        getCities(state): City[] {
            var cities = new Array();
            $.ajax(
                {
                    type: "GET",
                    url: this.getUrl("GetCities") + "?state=" + state.toString(),
                    async: false,
                    success(data) {
                        $.each(data, (i, v) => { cities.push(new City(v.Name, new State(v.State, v.StateFIPS))); });
                    }
                });
            return cities;
        }

        //* Fetch list states */
        getStates(): State[] {
            var states = new Array();
            $.ajax(
                {
                    type: "GET",
                    url: this.getUrl("GetStates"),
                    async: false,
                    success(data) {
                        $.each(data, (i, v) => {
                            states.push(new State(v.Abbreviation, v.Fips, v.StateFullName));
                        });
                    }
                });
            return states;
        }
        
        getTimeZones(): ListBuilder.ValueLabel[] {
            return [
                new ListBuilder.ValueLabel("C", "Central Time Zone"),
                new ListBuilder.ValueLabel("E", "Eastern Time Zone"),
                new ListBuilder.ValueLabel("H", "Hawaii / Alaska Time Zone"),
                new ListBuilder.ValueLabel("M", "Mountain Time Zone"),
                new ListBuilder.ValueLabel("P", "Pacific Time Zone"),
                new ListBuilder.ValueLabel("U", "Unknown")
            ];
        }

        getUrl(urlName: string) {
            const match = jQuery.grep(this.urls, (n) => n.name === urlName);
            return match[0].url;
        }

    }

    export class Url {
        constructor(public name: string, public url: string) {
        }
    }

}