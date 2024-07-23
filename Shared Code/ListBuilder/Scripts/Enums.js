var AccurateAppend;
(function (AccurateAppend) {
    var ListBuilder;
    (function (ListBuilder) {
        var SectionName;
        (function (SectionName) {
            SectionName[SectionName["States"] = 0] = "States";
            SectionName[SectionName["Counties"] = 1] = "Counties";
            SectionName[SectionName["Zips"] = 2] = "Zips";
            SectionName[SectionName["Cities"] = 3] = "Cities";
            SectionName[SectionName["TimeZones"] = 4] = "TimeZones";
            SectionName[SectionName["AgeRanges"] = 5] = "AgeRanges";
            SectionName[SectionName["ExactAges"] = 6] = "ExactAges";
            SectionName[SectionName["DobRanges"] = 7] = "DobRanges";
            SectionName[SectionName["Gender"] = 8] = "Gender";
            SectionName[SectionName["MaritalStatus"] = 9] = "MaritalStatus";
            SectionName[SectionName["Languages"] = 10] = "Languages";
            SectionName[SectionName["Hoh"] = 11] = "Hoh";
            SectionName[SectionName["EstIncome"] = 12] = "EstIncome";
            SectionName[SectionName["NetWorth"] = 13] = "NetWorth";
            SectionName[SectionName["HomeValue"] = 14] = "HomeValue";
            SectionName[SectionName["OwnRent"] = 15] = "OwnRent";
            SectionName[SectionName["Investments"] = 16] = "Investments";
            SectionName[SectionName["Education"] = 17] = "Education";
            SectionName[SectionName["OccupationGeneral"] = 18] = "OccupationGeneral";
            SectionName[SectionName["OccupationDetailed"] = 19] = "OccupationDetailed";
            SectionName[SectionName["Donates"] = 20] = "Donates";
            SectionName[SectionName["InterestsPurchases"] = 21] = "InterestsPurchases";
            SectionName[SectionName["InterestsReadingGeneral"] = 22] = "InterestsReadingGeneral";
            SectionName[SectionName["InterestsReadingMagazinesAndSubscriptions"] = 23] = "InterestsReadingMagazinesAndSubscriptions";
        })(SectionName = ListBuilder.SectionName || (ListBuilder.SectionName = {}));
    })(ListBuilder = AccurateAppend.ListBuilder || (AccurateAppend.ListBuilder = {}));
})(AccurateAppend || (AccurateAppend = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiRW51bXMuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJFbnVtcy50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQSxJQUFPLGNBQWMsQ0E2QnBCO0FBN0JELFdBQU8sY0FBYztJQUFDLElBQUEsV0FBVyxDQTZCaEM7SUE3QnFCLFdBQUEsV0FBVztRQUU3QixJQUFZLFdBeUJYO1FBekJELFdBQVksV0FBVztZQUNuQixpREFBTSxDQUFBO1lBQ04scURBQVEsQ0FBQTtZQUNSLDZDQUFJLENBQUE7WUFDSixpREFBTSxDQUFBO1lBQ04sdURBQVMsQ0FBQTtZQUNULHVEQUFTLENBQUE7WUFDVCx1REFBUyxDQUFBO1lBQ1QsdURBQVMsQ0FBQTtZQUNULGlEQUFNLENBQUE7WUFDTiwrREFBYSxDQUFBO1lBQ2Isd0RBQVMsQ0FBQTtZQUNULDRDQUFHLENBQUE7WUFDSCx3REFBUyxDQUFBO1lBQ1Qsc0RBQVEsQ0FBQTtZQUNSLHdEQUFTLENBQUE7WUFDVCxvREFBTyxDQUFBO1lBQ1AsNERBQVcsQ0FBQTtZQUNYLHdEQUFTLENBQUE7WUFDVCx3RUFBaUIsQ0FBQTtZQUNqQiwwRUFBa0IsQ0FBQTtZQUNsQixvREFBTyxDQUFBO1lBQ1AsMEVBQWtCLENBQUE7WUFDbEIsb0ZBQXVCLENBQUE7WUFDdkIsd0hBQXlDLENBQUE7UUFDN0MsQ0FBQyxFQXpCVyxXQUFXLEdBQVgsdUJBQVcsS0FBWCx1QkFBVyxRQXlCdEI7SUFFTCxDQUFDLEVBN0JxQixXQUFXLEdBQVgsMEJBQVcsS0FBWCwwQkFBVyxRQTZCaEM7QUFBRCxDQUFDLEVBN0JNLGNBQWMsS0FBZCxjQUFjLFFBNkJwQiIsInNvdXJjZXNDb250ZW50IjpbIm1vZHVsZSBBY2N1cmF0ZUFwcGVuZC5MaXN0QnVpbGRlciB7XHJcblxyXG4gICAgZXhwb3J0IGVudW0gU2VjdGlvbk5hbWUge1xyXG4gICAgICAgIFN0YXRlcyxcclxuICAgICAgICBDb3VudGllcyxcclxuICAgICAgICBaaXBzLFxyXG4gICAgICAgIENpdGllcyxcclxuICAgICAgICBUaW1lWm9uZXMsXHJcbiAgICAgICAgQWdlUmFuZ2VzLFxyXG4gICAgICAgIEV4YWN0QWdlcyxcclxuICAgICAgICBEb2JSYW5nZXMsXHJcbiAgICAgICAgR2VuZGVyLFxyXG4gICAgICAgIE1hcml0YWxTdGF0dXMsXHJcbiAgICAgICAgTGFuZ3VhZ2VzLFxyXG4gICAgICAgIEhvaCxcclxuICAgICAgICBFc3RJbmNvbWUsXHJcbiAgICAgICAgTmV0V29ydGgsXHJcbiAgICAgICAgSG9tZVZhbHVlLFxyXG4gICAgICAgIE93blJlbnQsXHJcbiAgICAgICAgSW52ZXN0bWVudHMsXHJcbiAgICAgICAgRWR1Y2F0aW9uLFxyXG4gICAgICAgIE9jY3VwYXRpb25HZW5lcmFsLFxyXG4gICAgICAgIE9jY3VwYXRpb25EZXRhaWxlZCxcclxuICAgICAgICBEb25hdGVzLFxyXG4gICAgICAgIEludGVyZXN0c1B1cmNoYXNlcyxcclxuICAgICAgICBJbnRlcmVzdHNSZWFkaW5nR2VuZXJhbCxcclxuICAgICAgICBJbnRlcmVzdHNSZWFkaW5nTWFnYXppbmVzQW5kU3Vic2NyaXB0aW9uc1xyXG4gICAgfVxyXG5cclxufSJdfQ==