module AccurateAppend.ListBuilder {

    export class DemographicsTabViewModel {

        constructor() {
            this.initializeAgeRangesSection("#ageRangesBody");
            this.initializeGenderSection("#genderBody");
            this.initializeMaritalStatusSection("#maritalStatusBody");
            this.initializeHeadOfHouseholdSection("#headOfHouseholdBody");
            this.initializeLanguageSection("#languageBody");
        }

        initializeAgeRangesSection(selector) {

            const inputs = this.getAgeRanges();

            var div = $(selector);
            $.each(inputs, (i, v) => {
                div.append(`<label class="checkbox-inline"><input type='checkbox' name='ageRanges' id='${v.id}' value='${ko.toJSON(v)}' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${v.label}</label>`);
                $(selector).append(div);
            });

        }

        initializeGenderSection(selector) {

            const inputs = this.getGenders();

            var div = $(selector);
            $.each(inputs, (i, v) => {
                div.append(`<label class="checkbox-inline"><input type='checkbox' name='gender' id='${v.id}' value='${ko.toJSON(v)}' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${v.label}</label>`);
                $(selector).append(div);
            });

        }

        initializeMaritalStatusSection(selector) {

            const inputs = this.getMaritalStatus();

            var div = $(selector);
            $.each(inputs, (i, v) => {
                div.append(`<label class="checkbox-inline"><input type='checkbox' name='maritalStatus' id='${v.id}' value='${ko.toJSON(v)}' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${v.label}</label>`);
                $(selector).append(div);
            });

        }

        initializeHeadOfHouseholdSection(selector) {

            const inputs = this.getHeadOfHousehold();

            var div = $(selector);
            $.each(inputs, (i, v) => {
                div.append(`<label class="checkbox-inline"><input type='checkbox' name='hoh' id='${v.id}' value='${ko.toJSON(v)}' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${v.label}</label>`);
                $(selector).append(div);
            });

        }

        initializeLanguageSection(selector) {

            const inputs = this.getLanguages();

            const cols = 3;
            var elementsPerColumn = Math.ceil(inputs.length / cols) + 1;
            for (var col = 0; col < cols; col++) {
                var div = $('<div class="col-md-4"></div>');
                $.each(inputs, (i, v) => {
                    if (col === 0 && (i >= elementsPerColumn)) return true;
                    if (col === 1 && (i < elementsPerColumn || i >= elementsPerColumn * 2)) return true;
                    if (col === 2 && (i < (elementsPerColumn * 2) || i >= elementsPerColumn * 3)) return true;
                    if (col === 3 && (i < (elementsPerColumn * 3) || i >= elementsPerColumn * 4)) return true;
                    if (col === 4 && (i < (elementsPerColumn * 4))) return true;
                    div.append(`<div class="checkbox"><label><input type='checkbox' name='languages' id='${v.id}' value='${ko.toJSON(v)}' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${v.label}</label></div>`);
                });
                $(selector).append(div);
            }

        }

        // LOOKUPS

        getAgeRanges() {
            return [
                new ListBuilder.ValueLabel("A", "18-24"), new ListBuilder.ValueLabel("B", "25-34"), new ListBuilder.ValueLabel("C", "35-44"), new ListBuilder.ValueLabel("D", "45-44"),
                new ListBuilder.ValueLabel("E", "55-64"), new ListBuilder.ValueLabel("F", "65-74"), new ListBuilder.ValueLabel("G", "75+")
            ];
        }

        getMaritalStatus() {
            return [
                new ListBuilder.ValueLabel("M", "Married"), new ListBuilder.ValueLabel("S", "Single"), new ListBuilder.ValueLabel("A", "Inferred Married"), new ListBuilder.ValueLabel("B", "Inferred Single"), new ListBuilder.ValueLabel("", "Unknown")
            ];
        }

        getHeadOfHousehold() {
            return [
                new ListBuilder.ValueLabel("1", "1"),
                new ListBuilder.ValueLabel("2", "2"),
                new ListBuilder.ValueLabel("3", "3"),
                new ListBuilder.ValueLabel("4", "4"),
                new ListBuilder.ValueLabel("5", "5"),
                new ListBuilder.ValueLabel("5", "6")
            ];
        }

        getGenders() {
            return [
                new ListBuilder.ValueLabel("M", "Male"), new ListBuilder.ValueLabel("F", "Female"), new ListBuilder.ValueLabel("U", "Unknown")
            ];
        }

        getLanguages() {
            return [
                new ListBuilder.ValueLabel("A1", "Afrikaans"),
                new ListBuilder.ValueLabel("A2", "Albanian"),
                new ListBuilder.ValueLabel("A3", "Amharic"),
                new ListBuilder.ValueLabel("A4", "Arabic"),
                new ListBuilder.ValueLabel("A5", "Armenian"),
                new ListBuilder.ValueLabel("A6", "Ashanti"),
                new ListBuilder.ValueLabel("A7", "Azeri"),
                new ListBuilder.ValueLabel("B1", "Bantu"),
                new ListBuilder.ValueLabel("B2", "Basque"),
                new ListBuilder.ValueLabel("B3", "Bengali"),
                new ListBuilder.ValueLabel("B4", "Bulgarian"),
                new ListBuilder.ValueLabel("B5", "Burmese"),
                new ListBuilder.ValueLabel("C1", "Chinese (Mandarin, Cantonese)"),
                new ListBuilder.ValueLabel("C2", "Comorian"),
                new ListBuilder.ValueLabel("C3", "Czech"),
                new ListBuilder.ValueLabel("D1", "Danish"),
                new ListBuilder.ValueLabel("D2", "Dutch"),
                new ListBuilder.ValueLabel("D3", "Dzongha"),
                new ListBuilder.ValueLabel("E1", "English"),
                new ListBuilder.ValueLabel("E2", "Estonian"),
                new ListBuilder.ValueLabel("F1", "Farsi"),
                new ListBuilder.ValueLabel("F2", "Finnish"),
                new ListBuilder.ValueLabel("F3", "French"),
                new ListBuilder.ValueLabel("G1", "Georgian"),
                new ListBuilder.ValueLabel("G2", "German"),
                new ListBuilder.ValueLabel("G3", "Ga"),
                new ListBuilder.ValueLabel("G4", "Greek"),
                new ListBuilder.ValueLabel("H1", "Hausa"),
                new ListBuilder.ValueLabel("H2", "Hebrew"),
                new ListBuilder.ValueLabel("H3", "Hindi"),
                new ListBuilder.ValueLabel("H4", "Hungarian"),
                new ListBuilder.ValueLabel("I1", "Icelandic"),
                new ListBuilder.ValueLabel("I2", "Indonesian"),
                new ListBuilder.ValueLabel("I3", "Italian"),
                new ListBuilder.ValueLabel("J1", "Japanese"),
                new ListBuilder.ValueLabel("K1", "Kazakh"),
                new ListBuilder.ValueLabel("K2", "Khmer"),
                new ListBuilder.ValueLabel("K3", "Kirghiz"),
                new ListBuilder.ValueLabel("K4", "Korean"),
                new ListBuilder.ValueLabel("L1", "Laotian (Include Hmong)"),
                new ListBuilder.ValueLabel("L2", "Latvian"),
                new ListBuilder.ValueLabel("L3", "Lithuanian"),
                new ListBuilder.ValueLabel("M1", "Macedonian"),
                new ListBuilder.ValueLabel("M2", "Malagasy"),
                new ListBuilder.ValueLabel("M3", "Malay"),
                new ListBuilder.ValueLabel("M4", "Moldavian"),
                new ListBuilder.ValueLabel("M5", "Mongolian"),
                new ListBuilder.ValueLabel("N1", "Nepali"),
                new ListBuilder.ValueLabel("N2", "Norwegian"),
                new ListBuilder.ValueLabel("O1", "Oromo"),
                new ListBuilder.ValueLabel("P1", "Pashto"),
                new ListBuilder.ValueLabel("P2", "Polish"),
                new ListBuilder.ValueLabel("P3", "Portuguese"),
                new ListBuilder.ValueLabel("R1", "Romanian"),
                new ListBuilder.ValueLabel("R2", "Russian"),
                new ListBuilder.ValueLabel("S1", "Samoan"),
                new ListBuilder.ValueLabel("S2", "Serbo-Croatian"),
                new ListBuilder.ValueLabel("S3", "Sinhalese"),
                new ListBuilder.ValueLabel("S4", "Slovakian"),
                new ListBuilder.ValueLabel("S5", "Slovenian"),
                new ListBuilder.ValueLabel("S6", "Somali"),
                new ListBuilder.ValueLabel("S7", "Sotho"),
                new ListBuilder.ValueLabel("S8", "Spanish"),
                new ListBuilder.ValueLabel("S9", "Swahili"),
                new ListBuilder.ValueLabel("SA", "Swazi"),
                new ListBuilder.ValueLabel("SB", "Swedish"),
                new ListBuilder.ValueLabel("T1", "Tagalog"),
                new ListBuilder.ValueLabel("T2", "Tajik"),
                new ListBuilder.ValueLabel("T3", "Thai"),
                new ListBuilder.ValueLabel("T4", "Tibetan"),
                new ListBuilder.ValueLabel("T5", "Tongan"),
                new ListBuilder.ValueLabel("T6", "Turkish"),
                new ListBuilder.ValueLabel("T7", "Turkmeni"),
                new ListBuilder.ValueLabel("T8", "Tswana"),
                new ListBuilder.ValueLabel("UX", "Unknown"),
                new ListBuilder.ValueLabel("U1", "Urdu"),
                new ListBuilder.ValueLabel("U2", "Uzbeki"),
                new ListBuilder.ValueLabel("V1", "Vietnamese"),
                new ListBuilder.ValueLabel("X1", "Xhosa"),
                new ListBuilder.ValueLabel("Z1", "Zulu")
            ];
        }
         
    }

}