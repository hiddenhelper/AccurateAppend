module AccurateAppend.ListBuilder {

    export class InterestsTabViewModel {

        constructor() {
            this.initializePurchasesSection("#purchasesBody");
            this.initializeSportsSection("#sportsBody");
            this.initializeFitnessSection("#fitnessBody");
            this.initializeOutdoorsSection("#outdoorsGeneralBody");
            this.initializeReadingGeneralSection("#readingGeneralBody");
            this.initializeReadingPreferencesSection("#readingMagazinesBody");
        }

        // TABS

        initializePurchasesSection(selector) {

            const inputs = this.getInterestsPurchases();

            // can be encapsulated in a common funtion
            const cols = 2;
            var elementsPerColumn = Math.ceil(inputs.length / cols) + 1;
            for (var col = 0; col < cols; col++) {
                var div = $('<div class="col-md-5"></div>');
                $.each(inputs, (i, v) => {
                    if (col === 0 && (i >= elementsPerColumn)) return true;
                    if (col === 1 && (i < elementsPerColumn || i >= elementsPerColumn * 2)) return true;
                    div.append(`<div class="checkbox"><label><input type='checkbox' name='interestsPurchases' id='${v.id}' value='${ko.toJSON(v)}' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${v.label}</label></div>`);
                });
                $(selector).append(div);
            }

        }

        initializeSportsSection(selector) {

            const inputs = this.getInterestsSports();

            // can be encapsulated in a common funtion
            const cols = 2;
            var elementsPerColumn = Math.ceil(inputs.length / cols) + 1;
            for (var col = 0; col < cols; col++) {
                var div = $('<div class="col-md-5" style="padding-left: 0;"></div>');
                $.each(inputs, (i, v) => {
                    if (col === 0 && (i >= elementsPerColumn)) return true;
                    if (col === 1 && (i < elementsPerColumn || i >= elementsPerColumn * 2)) return true;
                    div.append(`<div class="checkbox"><label><input type='checkbox' name='interestsSports' id='${v.id}' value='${ko.toJSON(v)}' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${v.label}</label></div>`);
                });
                $(selector).append(div);
            }

        }

        initializeFitnessSection(selector) {

            const inputs = this.getInterestsFitness();

            var div = $(selector);
            $.each(inputs, (i, v) => {
                div.append(`<div class="checkbox"><label><input type='checkbox' name='interestsFitness' id='${v.id}' value='${ko.toJSON(v)}' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${v.label}</label></div>`);
                $(selector).append(div);
            });

        }

        initializeOutdoorsSection(selector) {

            const inputs = this.getInterestsOutdoors();

            var div = $(selector);
            $.each(inputs, (i, v) => {
                div.append(`<div class="checkbox"><label><input type='checkbox' name='interestsFitness' id='${v.id}' value='${ko.toJSON(v)}' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${v.label}</label></div>`);
                $(selector).append(div);
            });

        }

        initializeReadingGeneralSection(selector) {

            const inputs = this.getInterestsReadingGeneral();

            var div = $(selector);
            $.each(inputs, (i, v) => {
                div.append(`<div class="checkbox"><label><input type='checkbox' name='interestsReadingGeneral' id='${v.id}' value='${ko.toJSON(v)}' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${v.label}</label></div>`);
                $(selector).append(div);
            });

        }

        initializeReadingPreferencesSection(selector) {

            const inputs = this.getInterestsReadingPreferences();

            // can be encapsulated in a common funtion
            const cols = 2;
            var elementsPerColumn = Math.ceil(inputs.length / cols) + 1;
            for (var col = 0; col < cols; col++) {
                var div = $('<div class="col-md-5" style="padding-left: 0;"></div>');
                $.each(inputs, (i, v) => {
                    if (col === 0 && (i >= elementsPerColumn)) return true;
                    if (col === 1 && (i < elementsPerColumn || i >= elementsPerColumn * 2)) return true;
                    div.append(`<div class="checkbox"><label><input type='checkbox' name='interestsReadingMagazinesAndSubscriptions' id='${v.id}' value='${ko.toJSON(v)}' data-bind='click: function (data, event) { listCriteriaViewModel.toggle(event); return true; }' />&nbsp;${v.label}</label></div>`);
                });
                $(selector).append(div);
            }

        }

        // LOOKUPS

        getInterestsPurchases(): ListBuilder.ValueLabel[] {
            return [
                new ListBuilder.ValueLabel("buysMaleMerchandise", "Male Merchandise"),
                new ListBuilder.ValueLabel("buysFemaleMerchandise", "Female Merchandise"),
                new ListBuilder.ValueLabel("buysHobbyMerchandise", "Hobby Merchandise"),
                new ListBuilder.ValueLabel("buysGardeningFarmSupplies", "Gardening Farm Supplies"),
                new ListBuilder.ValueLabel("buysGourmetFoodAccessories", "Gourmet Food Accessories"),
                new ListBuilder.ValueLabel("buysMailOrder", "Mail Order"),
                new ListBuilder.ValueLabel("buysOnline", "Online"),
                new ListBuilder.ValueLabel("buysWomensApparel", "Womens Apparel"),
                new ListBuilder.ValueLabel("buysWomensApparelPetite", "Womens Apparel Petite"),
                new ListBuilder.ValueLabel("buysWomensApparelPlusSizes", "Womens Apparel Plus Sizes"),
                new ListBuilder.ValueLabel("buysWomensApparelYoung", "Womens Apparel Young"),
                new ListBuilder.ValueLabel("buysMensApparel", "Mens Apparel"),
                new ListBuilder.ValueLabel("buysMensApparelBigAndTall", "Mens Apparel Big And Tall"),
                new ListBuilder.ValueLabel("buysMensApparelYoung", "Mens Apparel Young"),
                new ListBuilder.ValueLabel("buysChildrensApparel", "Childrens Apparel"),
                new ListBuilder.ValueLabel("buysChildrensApparelInfantsToddlers", "Childrens Apparel Infants Toddlers"),
                new ListBuilder.ValueLabel("buysChildrenLearningToys", "Children Learning Toys"),
                new ListBuilder.ValueLabel("buysChildrensBabyCare", "Childrens Baby Care"),
                new ListBuilder.ValueLabel("buysChildrensSchoolSupplies", "Childrens School Supplies"),
                new ListBuilder.ValueLabel("buysChildrensGeneral", "Childrens General"),
                new ListBuilder.ValueLabel("buysChildrensInterests", "Childrens Interests"),
                new ListBuilder.ValueLabel("buysBooks", "Books")
            ];
        }

        getInterestsSports(): ListBuilder.ValueLabel[] {
            return [
                new ListBuilder.ValueLabel("golf", "Golf"),
                new ListBuilder.ValueLabel("boatingSailing", "Boating/Sailing"),
                new ListBuilder.ValueLabel("hunting", "Hunting"),
                new ListBuilder.ValueLabel("fishing", "Fishing"),
                new ListBuilder.ValueLabel("interestedInTennis", "Tennis"),
                new ListBuilder.ValueLabel("interestedInSnowskiing", "Snow Skiing"),
                new ListBuilder.ValueLabel("interestedInMotorcycling", "Motorcycling"),
                new ListBuilder.ValueLabel("interestedInNascar", "Nascar"),
                new ListBuilder.ValueLabel("interestedInScubaDiving", "Scuba Diving"),
                new ListBuilder.ValueLabel("interestedInSportsandLeisure", "Sports and Leisure"),
                new ListBuilder.ValueLabel("interestedInSpectatorSportsAutoMotorcycleRacing", "Auto & Motorcycle Racing"),
                new ListBuilder.ValueLabel("interestedInSpectatorSportsTVSports", "TV Sports"),
                new ListBuilder.ValueLabel("interestedInSpectatorSportsFootball", "Football"),
                new ListBuilder.ValueLabel("interestedInSpectatorSportsBaseball", "Baseball"),
                new ListBuilder.ValueLabel("interestedInSpectatorSportsBasketball", "Basketball"),
                new ListBuilder.ValueLabel("interestedInSpectatorSportsHockey", "Hockey"),
                new ListBuilder.ValueLabel("interestedInSpectatorSportsSoccer", "Soccer")
            ];
        }

        getInterestsFitness(): ListBuilder.ValueLabel[] {
            return [
                new ListBuilder.ValueLabel("interestedInExerciseHealthGrouping", "Health - General"),
                new ListBuilder.ValueLabel("interestedInExerciseRunningJogging", "Jogging"),
                new ListBuilder.ValueLabel("interestedInExerciseWalking", "Walking"),
                new ListBuilder.ValueLabel("interestedInExerciseAerobic", "Aerobic"),
                new ListBuilder.ValueLabel("interestedInDietingWeightLoss", "Dieting & Weight Loss")
            ];
        }

        getInterestsOutdoors(): ListBuilder.ValueLabel[] {
            return [
                new ListBuilder.ValueLabel("interestedInCampingHiking", "Camping Hiking"),
                new ListBuilder.ValueLabel("interestedInHuntingShooting", "Hunting Shooting"),
                new ListBuilder.ValueLabel("interestedInSportsGrouping", "Sports - General"),
                new ListBuilder.ValueLabel("interestedInOutdoorsGrouping", "Outdoors - General")
            ];
        }

        getInterestsReadingGeneral(): ListBuilder.ValueLabel[] {
            return [
                new ListBuilder.ValueLabel("interestedInBooksAndMagazines", "Books and Magazines"),
                new ListBuilder.ValueLabel("interestedInBooksAndMusicBooks", "Books and MusicBooks"),
                new ListBuilder.ValueLabel("interestedInBooksAndMusicBooksAudio", "Books and Music Books Audio"),
                new ListBuilder.ValueLabel("interestedInReadingGeneral", "Reading General"),
                new ListBuilder.ValueLabel("interestedInReadingScienceFiction", "Reading Science Fiction"),
                new ListBuilder.ValueLabel("interestedInReadingMagazines", "Reading Magazines"),
                new ListBuilder.ValueLabel("interestedInReadingAudioBooks", "Reading Audio Books")
            ];
        }

        getInterestsReadingPreferences(): ListBuilder.ValueLabel[] {
            return [
                new ListBuilder.ValueLabel("interestedInHistoryMilitary", "History Military"),
                new ListBuilder.ValueLabel("interestedInCurrentAffairsPolitics", "Current Affairs Politics"),
                new ListBuilder.ValueLabel("interestedInReligiousInspirational", "Religious Inspirational"),
                new ListBuilder.ValueLabel("interestedInScienceSpace", "Science Space"),
                new ListBuilder.ValueLabel("interestedInMagazines", "Magazines"),
                new ListBuilder.ValueLabel("interestedInEducationOnline", "Education Online"),
                new ListBuilder.ValueLabel("interestedInGaming", "Gaming"),
                new ListBuilder.ValueLabel("interestedInComputingHomeOfficeGeneral", "Computing Home Office General"),
                new ListBuilder.ValueLabel("interestedInDVDsVideos", "DVDs Videos"),
                new ListBuilder.ValueLabel("interestedInElectronicsandComputingTVVideoMovieWatcher", "TV Video Movie Watcher"),
                new ListBuilder.ValueLabel("interestedInElectronicsComputingAndHomeOffice", "Electronics Computing and Home Office"),
                new ListBuilder.ValueLabel("interestedInHighEndAppliances", "High End Appliances"),
                //new ValueLabel("intendToPurchaseHDTVSatelliteDish", ""),
                new ListBuilder.ValueLabel("interestedInMusicHomeStereo", "Music Home Stereo"),
                new ListBuilder.ValueLabel("interestedInMusicPlayer", "Music Player"),
                new ListBuilder.ValueLabel("interestedInMusicCollector", "Music Collector"),
                new ListBuilder.ValueLabel("interestedInMusicAvidListener", "Music Avid Listener"),
                new ListBuilder.ValueLabel("interestedInMovieCollector", "Movie Collector"),
                new ListBuilder.ValueLabel("interestedInTVCable", "TV Cable"),
                new ListBuilder.ValueLabel("interestedInGamesVideoGames", "Games Video Games"),
                new ListBuilder.ValueLabel("interestedInTVSatelliteDish", "TV Satellite Dish"),
                new ListBuilder.ValueLabel("interestedInComputers", "Computers"),
                new ListBuilder.ValueLabel("interestedInGamesComputerGames", "Games Computer Games"),
                new ListBuilder.ValueLabel("interestedInConsumerElectronics", "Consumer Electronics"),
                //new ValueLabel("interestedInElectronicsComputersGrouping", ""),
                new ListBuilder.ValueLabel("interestedInTelecommunications", "Telecommunications"),
                new ListBuilder.ValueLabel("interestedInTheaterPerformingArts", "Theater Performing Arts"),
                new ListBuilder.ValueLabel("interestedInArts", "Arts"),
                new ListBuilder.ValueLabel("interestedInMusicalinstruments", "Musical Instruments"),
                new ListBuilder.ValueLabel("interestedInGamesBoardGamesPuzzles", "Board Games and Puzzles"),
                new ListBuilder.ValueLabel("interestedInGamingCasino", "Gaming Casino")
            ];
        }

    }

}