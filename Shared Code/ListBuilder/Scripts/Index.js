var AccurateAppend;
(function (AccurateAppend) {
    var ListBuilder;
    (function (ListBuilder) {
        var ViewModel = (function () {
            function ViewModel(urls, requestid) {
                this.urls = urls;
                this.listDownloadUri = ko.observable("");
                this.listDefintionDownloadUri = ko.observable("");
                this.listCriteriaViewModel = new ListBuilder.ListCriteriaViewModel(urls, requestid);
                this.geographyTabViewModel = new ListBuilder.GeographyTabViewModel(urls);
                this.demographicsTabViewModel = new ListBuilder.DemographicsTabViewModel();
                this.financesTabViewModel = new ListBuilder.FinancesTabViewModel();
                this.interestsTabViewModel = new ListBuilder.InterestsTabViewModel();
            }
            ViewModel.prototype.downloadList = function () {
                var _this = this;
                $("#listCriteriaDisplay").find("#message").remove();
                var count = this.listCriteriaViewModel.count();
                if (count > 200000 && count < 600000) {
                    $("#listCriteriaDisplay")
                        .prepend("<div style=\"padding: 8px;\" class=\"alert alert-warning\" id=\"message\">Larger lists can take up to 5 minutes to build. Don't close your browser.</div>");
                    return false;
                }
                else if (this.listCriteriaViewModel.count() >= 600000) {
                    $("#listCriteriaDisplay")
                        .prepend("<div style=\"padding: 8px;\" class=\"alert alert-danger\" id=\"message\">List to large to download. Please add criteria or contact support.</div>");
                    return false;
                }
                $("#listBuildWaiting").show();
                $("#downloadList").attr("disabled", "disabled").find("[class='fa fa-refresh fa-spin']").show();
                $.post("/ListBuilder/BuildList/Query", { listCriteria: ko.toJSON(this.listCriteriaViewModel) }, function (data) {
                    if (data.HttpStatusCodeResult === 200) {
                        _this.listDownloadUri(data.ListDownloadUri);
                        _this.listDefintionDownloadUri(data.ListDefintionDownloadUri);
                        window.location.replace(_this.listDownloadUri());
                    }
                    else {
                        $("#listCriteriaDisplay")
                            .prepend("<div style=\"padding: 8px;\" class=\"alert alert-warning\" id=\"message\">" + data.Message + "</div>");
                    }
                    $("#listBuildWaiting").hide();
                    $("#downloadList").removeAttr("disabled").find("[class='fa fa-refresh fa-spin']").hide();
                });
                return true;
            };
            ViewModel.prototype.downloadDefintion = function () {
                window.location.replace(this.listDefintionDownloadUri());
            };
            ViewModel.prototype.enhanceList = function () {
                $("#listCriteriaDisplay").find("#message").remove();
                if (this.listCriteriaViewModel.count() > 400000) {
                    $("#listCriteriaDisplay")
                        .prepend("<div style=\"padding: 8px;\" class=\"alert alert-danger\" id=\"message\">List to large to enhance. Please add criteria or contact support.</div>");
                    return false;
                }
                $("#listCriteriaDisplay").find("#listBuildWaiting").show();
                $("#enhanceList").attr("disabled", "disabled").find("[class='fa fa-refresh fa-spin']").show();
                $.post("/ListBuilder/BuildList/Create", { listCriteria: ko.toJSON(this.listCriteriaViewModel) }, function (data) {
                    if (data.HttpStatusCodeResult === 200) {
                        $("#listCriteriaDisplay")
                            .prepend("<div style=\"padding: 8px;\" class=\"alert alert-success\" id=\"message\">List being generated. You will be redirected shortly.</div>");
                        window.location.replace(data.DownloadUri);
                    }
                    else {
                        $("#listCriteriaDisplay")
                            .prepend("<div style=\"padding: 8px;\" class=\"alert alert-warning\" id=\"message\">" + data.Message + "</div>");
                    }
                    $("#listCriteriaDisplay").find("#listBuildWaiting").hide();
                    $("#listCriteriaDisplay").find("#enhanceList").removeAttr("disabled").find("[class='fa fa-refresh fa-spin']")
                        .hide();
                });
                return true;
            };
            ViewModel.prototype.generateUuid = function () {
                var d = new Date().getTime();
                var uuid = "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
                    var r = (d + Math.random() * 16) % 16 | 0;
                    d = Math.floor(d / 16);
                    return (c === "x" ? r : (r & 0x7 | 0x8)).toString(16);
                });
                return uuid;
            };
            ;
            ViewModel.prototype.numberWithCommas = function (x) {
                return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            };
            return ViewModel;
        }());
        ListBuilder.ViewModel = ViewModel;
    })(ListBuilder = AccurateAppend.ListBuilder || (AccurateAppend.ListBuilder = {}));
})(AccurateAppend || (AccurateAppend = {}));
var viewModel1;
$(function () {
    viewModel1 = new AccurateAppend.ListBuilder.ViewModel(lookupUrls, requestId);
    ko.applyBindings(viewModel1);
    $("#dob_start").kendoMaskedTextBox({ mask: "00-0000" });
    $("#dob_end").kendoMaskedTextBox({ mask: "00-0000" });
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiSW5kZXguanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJJbmRleC50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFJQSxJQUFPLGNBQWMsQ0EwR3BCO0FBMUdELFdBQU8sY0FBYztJQUFDLElBQUEsV0FBVyxDQTBHaEM7SUExR3FCLFdBQUEsV0FBVztRQUUvQjtZQWFFLG1CQUFtQixJQUFnQixFQUFFLFNBQWlCO2dCQUFuQyxTQUFJLEdBQUosSUFBSSxDQUFZO2dCQUhuQyxvQkFBZSxHQUFHLEVBQUUsQ0FBQyxVQUFVLENBQUMsRUFBRSxDQUFDLENBQUM7Z0JBQ3BDLDZCQUF3QixHQUFHLEVBQUUsQ0FBQyxVQUFVLENBQUMsRUFBRSxDQUFDLENBQUM7Z0JBRzNDLElBQUksQ0FBQyxxQkFBcUIsR0FBRyxJQUFJLFlBQUEscUJBQXFCLENBQUMsSUFBSSxFQUFFLFNBQVMsQ0FBQyxDQUFDO2dCQUV4RSxJQUFJLENBQUMscUJBQXFCLEdBQUcsSUFBSSxZQUFBLHFCQUFxQixDQUFDLElBQUksQ0FBQyxDQUFDO2dCQUM3RCxJQUFJLENBQUMsd0JBQXdCLEdBQUcsSUFBSSxZQUFBLHdCQUF3QixFQUFFLENBQUM7Z0JBQy9ELElBQUksQ0FBQyxvQkFBb0IsR0FBRyxJQUFJLFlBQUEsb0JBQW9CLEVBQUUsQ0FBQztnQkFDdkQsSUFBSSxDQUFDLHFCQUFxQixHQUFHLElBQUksWUFBQSxxQkFBcUIsRUFBRSxDQUFDO1lBQzNELENBQUM7WUFFRCxnQ0FBWSxHQUFaO2dCQUFBLGlCQStCQztnQkE5QkcsQ0FBQyxDQUFDLHNCQUFzQixDQUFDLENBQUMsSUFBSSxDQUFDLFVBQVUsQ0FBQyxDQUFDLE1BQU0sRUFBRSxDQUFDO2dCQUN0RCxJQUFNLEtBQUssR0FBRyxJQUFJLENBQUMscUJBQXFCLENBQUMsS0FBSyxFQUFFLENBQUM7Z0JBQ2pELElBQUksS0FBSyxHQUFHLE1BQU0sSUFBSSxLQUFLLEdBQUcsTUFBTSxFQUFFO29CQUNwQyxDQUFDLENBQUMsc0JBQXNCLENBQUM7eUJBQ3RCLE9BQU8sQ0FDTiwySkFBcUosQ0FBQyxDQUFDO29CQUMzSixPQUFPLEtBQUssQ0FBQztpQkFDZDtxQkFBTSxJQUFJLElBQUksQ0FBQyxxQkFBcUIsQ0FBQyxLQUFLLEVBQUUsSUFBSSxNQUFNLEVBQUU7b0JBQ3ZELENBQUMsQ0FBQyxzQkFBc0IsQ0FBQzt5QkFDdEIsT0FBTyxDQUNOLG1KQUE2SSxDQUFDLENBQUM7b0JBQ25KLE9BQU8sS0FBSyxDQUFDO2lCQUNkO2dCQUNELENBQUMsQ0FBQyxtQkFBbUIsQ0FBQyxDQUFDLElBQUksRUFBRSxDQUFDO2dCQUM5QixDQUFDLENBQUMsZUFBZSxDQUFDLENBQUMsSUFBSSxDQUFDLFVBQVUsRUFBRSxVQUFVLENBQUMsQ0FBQyxJQUFJLENBQUMsaUNBQWlDLENBQUMsQ0FBQyxJQUFJLEVBQUUsQ0FBQztnQkFDL0YsQ0FBQyxDQUFDLElBQUksQ0FBQyw4QkFBOEIsRUFDbkMsRUFBRSxZQUFZLEVBQUUsRUFBRSxDQUFDLE1BQU0sQ0FBQyxJQUFJLENBQUMscUJBQXFCLENBQUMsRUFBRSxFQUN2RCxVQUFBLElBQUk7b0JBQ0YsSUFBSSxJQUFJLENBQUMsb0JBQW9CLEtBQUssR0FBRyxFQUFFO3dCQUNyQyxLQUFJLENBQUMsZUFBZSxDQUFDLElBQUksQ0FBQyxlQUFlLENBQUMsQ0FBQzt3QkFDM0MsS0FBSSxDQUFDLHdCQUF3QixDQUFDLElBQUksQ0FBQyx3QkFBd0IsQ0FBQyxDQUFDO3dCQUM3RCxNQUFNLENBQUMsUUFBUSxDQUFDLE9BQU8sQ0FBQyxLQUFJLENBQUMsZUFBZSxFQUFFLENBQUMsQ0FBQztxQkFDakQ7eUJBQU07d0JBQ0wsQ0FBQyxDQUFDLHNCQUFzQixDQUFDOzZCQUN0QixPQUFPLENBQUMsK0VBQXVFLElBQUksQ0FBQyxPQUFPLFdBQVEsQ0FBQyxDQUFDO3FCQUN6RztvQkFDRCxDQUFDLENBQUMsbUJBQW1CLENBQUMsQ0FBQyxJQUFJLEVBQUUsQ0FBQztvQkFDOUIsQ0FBQyxDQUFDLGVBQWUsQ0FBQyxDQUFDLFVBQVUsQ0FBQyxVQUFVLENBQUMsQ0FBQyxJQUFJLENBQUMsaUNBQWlDLENBQUMsQ0FBQyxJQUFJLEVBQUUsQ0FBQztnQkFDM0YsQ0FBQyxDQUFDLENBQUM7Z0JBQ0wsT0FBTyxJQUFJLENBQUM7WUFDZCxDQUFDO1lBRUQscUNBQWlCLEdBQWpCO2dCQUNFLE1BQU0sQ0FBQyxRQUFRLENBQUMsT0FBTyxDQUFDLElBQUksQ0FBQyx3QkFBd0IsRUFBRSxDQUFDLENBQUM7WUFDM0QsQ0FBQztZQUVELCtCQUFXLEdBQVg7Z0JBQ0UsQ0FBQyxDQUFDLHNCQUFzQixDQUFDLENBQUMsSUFBSSxDQUFDLFVBQVUsQ0FBQyxDQUFDLE1BQU0sRUFBRSxDQUFDO2dCQUNwRCxJQUFJLElBQUksQ0FBQyxxQkFBcUIsQ0FBQyxLQUFLLEVBQUUsR0FBRyxNQUFNLEVBQUU7b0JBQy9DLENBQUMsQ0FBQyxzQkFBc0IsQ0FBQzt5QkFDdEIsT0FBTyxDQUNOLGtKQUE0SSxDQUFDLENBQUM7b0JBQ2xKLE9BQU8sS0FBSyxDQUFDO2lCQUNkO2dCQUNELENBQUMsQ0FBQyxzQkFBc0IsQ0FBQyxDQUFDLElBQUksQ0FBQyxtQkFBbUIsQ0FBQyxDQUFDLElBQUksRUFBRSxDQUFDO2dCQUMzRCxDQUFDLENBQUMsY0FBYyxDQUFDLENBQUMsSUFBSSxDQUFDLFVBQVUsRUFBRSxVQUFVLENBQUMsQ0FBQyxJQUFJLENBQUMsaUNBQWlDLENBQUMsQ0FBQyxJQUFJLEVBQUUsQ0FBQztnQkFDOUYsQ0FBQyxDQUFDLElBQUksQ0FBQywrQkFBK0IsRUFDcEMsRUFBRSxZQUFZLEVBQUUsRUFBRSxDQUFDLE1BQU0sQ0FBQyxJQUFJLENBQUMscUJBQXFCLENBQUMsRUFBRSxFQUN2RCxVQUFBLElBQUk7b0JBQ0YsSUFBSSxJQUFJLENBQUMsb0JBQW9CLEtBQUssR0FBRyxFQUFFO3dCQUNyQyxDQUFDLENBQUMsc0JBQXNCLENBQUM7NkJBQ3RCLE9BQU8sQ0FDTix1SUFBaUksQ0FBQyxDQUFDO3dCQUN2SSxNQUFNLENBQUMsUUFBUSxDQUFDLE9BQU8sQ0FBQyxJQUFJLENBQUMsV0FBVyxDQUFDLENBQUM7cUJBQzNDO3lCQUFNO3dCQUNMLENBQUMsQ0FBQyxzQkFBc0IsQ0FBQzs2QkFDdEIsT0FBTyxDQUFDLCtFQUF1RSxJQUFJLENBQUMsT0FBTyxXQUFRLENBQUMsQ0FBQztxQkFDekc7b0JBQ0QsQ0FBQyxDQUFDLHNCQUFzQixDQUFDLENBQUMsSUFBSSxDQUFDLG1CQUFtQixDQUFDLENBQUMsSUFBSSxFQUFFLENBQUM7b0JBQzNELENBQUMsQ0FBQyxzQkFBc0IsQ0FBQyxDQUFDLElBQUksQ0FBQyxjQUFjLENBQUMsQ0FBQyxVQUFVLENBQUMsVUFBVSxDQUFDLENBQUMsSUFBSSxDQUFDLGlDQUFpQyxDQUFDO3lCQUMxRyxJQUFJLEVBQUUsQ0FBQztnQkFDWixDQUFDLENBQUMsQ0FBQztnQkFDTCxPQUFPLElBQUksQ0FBQztZQUNkLENBQUM7WUFFRCxnQ0FBWSxHQUFaO2dCQUNFLElBQUksQ0FBQyxHQUFHLElBQUksSUFBSSxFQUFFLENBQUMsT0FBTyxFQUFFLENBQUM7Z0JBQzdCLElBQU0sSUFBSSxHQUFHLHNDQUFzQyxDQUFDLE9BQU8sQ0FBQyxPQUFPLEVBQ2pFLFVBQUEsQ0FBQztvQkFDQyxJQUFJLENBQUMsR0FBRyxDQUFDLENBQUMsR0FBRyxJQUFJLENBQUMsTUFBTSxFQUFFLEdBQUcsRUFBRSxDQUFDLEdBQUcsRUFBRSxHQUFHLENBQUMsQ0FBQztvQkFDMUMsQ0FBQyxHQUFHLElBQUksQ0FBQyxLQUFLLENBQUMsQ0FBQyxHQUFHLEVBQUUsQ0FBQyxDQUFDO29CQUN2QixPQUFPLENBQUMsQ0FBQyxLQUFLLEdBQUcsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsR0FBRyxHQUFHLEdBQUcsR0FBRyxDQUFDLENBQUMsQ0FBQyxRQUFRLENBQUMsRUFBRSxDQUFDLENBQUM7Z0JBQ3hELENBQUMsQ0FBQyxDQUFDO2dCQUNMLE9BQU8sSUFBSSxDQUFDO1lBQ2QsQ0FBQztZQUFBLENBQUM7WUFFRixvQ0FBZ0IsR0FBaEIsVUFBaUIsQ0FBQztnQkFDaEIsT0FBTyxDQUFDLENBQUMsUUFBUSxFQUFFLENBQUMsT0FBTyxDQUFDLHVCQUF1QixFQUFFLEdBQUcsQ0FBQyxDQUFDO1lBQzVELENBQUM7WUFDSCxnQkFBQztRQUFELENBQUMsQUF0R0QsSUFzR0M7UUF0R1kscUJBQVMsWUFzR3JCLENBQUE7SUFFSCxDQUFDLEVBMUdxQixXQUFXLEdBQVgsMEJBQVcsS0FBWCwwQkFBVyxRQTBHaEM7QUFBRCxDQUFDLEVBMUdNLGNBQWMsS0FBZCxjQUFjLFFBMEdwQjtBQUVELElBQUksVUFBZ0QsQ0FBQztBQUVyRCxDQUFDLENBQUM7SUFFQSxVQUFVLEdBQUcsSUFBSSxjQUFjLENBQUMsV0FBVyxDQUFDLFNBQVMsQ0FBQyxVQUFVLEVBQUUsU0FBUyxDQUFDLENBQUM7SUFFN0UsRUFBRSxDQUFDLGFBQWEsQ0FBQyxVQUFVLENBQUMsQ0FBQztJQUU3QixDQUFDLENBQUMsWUFBWSxDQUFDLENBQUMsa0JBQWtCLENBQUMsRUFBRSxJQUFJLEVBQUUsU0FBUyxFQUFFLENBQUMsQ0FBQztJQUN4RCxDQUFDLENBQUMsVUFBVSxDQUFDLENBQUMsa0JBQWtCLENBQUMsRUFBRSxJQUFJLEVBQUUsU0FBUyxFQUFFLENBQUMsQ0FBQztBQUV4RCxDQUFDLENBQUMsQ0FBQyIsInNvdXJjZXNDb250ZW50IjpbIi8vIGFiaWVudCBkZWNsYXJhdGlvbiB1c2VkIHRvIHRlbGwgVFMgYWJvdXQgdGhlIGV4aXN0YW5jZSBvZiB0aGUgdmFyaWFibGVcclxuZGVjbGFyZSBsZXQgbG9va3VwVXJsczogQXJyYXk8QWNjdXJhdGVBcHBlbmQuTGlzdEJ1aWxkZXIuVXJsPjtcclxuZGVjbGFyZSBsZXQgcmVxdWVzdElkOiBzdHJpbmc7XHJcblxyXG5tb2R1bGUgQWNjdXJhdGVBcHBlbmQuTGlzdEJ1aWxkZXIge1xyXG5cclxuICBleHBvcnQgY2xhc3MgVmlld01vZGVsIHtcclxuXHJcbiAgICBsaXN0Q3JpdGVyaWFWaWV3TW9kZWw6IExpc3RDcml0ZXJpYVZpZXdNb2RlbDtcclxuXHJcbiAgICAvLyB0YWIgc3BlY2lmaWMgdmlldyBtb2RlbHNcclxuICAgIGdlb2dyYXBoeVRhYlZpZXdNb2RlbDogR2VvZ3JhcGh5VGFiVmlld01vZGVsO1xyXG4gICAgZGVtb2dyYXBoaWNzVGFiVmlld01vZGVsOiBEZW1vZ3JhcGhpY3NUYWJWaWV3TW9kZWw7XHJcbiAgICBmaW5hbmNlc1RhYlZpZXdNb2RlbDogRmluYW5jZXNUYWJWaWV3TW9kZWw7XHJcbiAgICBpbnRlcmVzdHNUYWJWaWV3TW9kZWw6IEludGVyZXN0c1RhYlZpZXdNb2RlbDtcclxuXHJcbiAgICBsaXN0RG93bmxvYWRVcmkgPSBrby5vYnNlcnZhYmxlKFwiXCIpO1xyXG4gICAgbGlzdERlZmludGlvbkRvd25sb2FkVXJpID0ga28ub2JzZXJ2YWJsZShcIlwiKTtcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihwdWJsaWMgdXJsczogQXJyYXk8VXJsPiwgcmVxdWVzdGlkOiBzdHJpbmcpIHtcclxuICAgICAgdGhpcy5saXN0Q3JpdGVyaWFWaWV3TW9kZWwgPSBuZXcgTGlzdENyaXRlcmlhVmlld01vZGVsKHVybHMsIHJlcXVlc3RpZCk7XHJcbiAgICAgIC8vIHRhYnNcclxuICAgICAgdGhpcy5nZW9ncmFwaHlUYWJWaWV3TW9kZWwgPSBuZXcgR2VvZ3JhcGh5VGFiVmlld01vZGVsKHVybHMpO1xyXG4gICAgICB0aGlzLmRlbW9ncmFwaGljc1RhYlZpZXdNb2RlbCA9IG5ldyBEZW1vZ3JhcGhpY3NUYWJWaWV3TW9kZWwoKTtcclxuICAgICAgdGhpcy5maW5hbmNlc1RhYlZpZXdNb2RlbCA9IG5ldyBGaW5hbmNlc1RhYlZpZXdNb2RlbCgpO1xyXG4gICAgICB0aGlzLmludGVyZXN0c1RhYlZpZXdNb2RlbCA9IG5ldyBJbnRlcmVzdHNUYWJWaWV3TW9kZWwoKTtcclxuICAgIH1cclxuXHJcbiAgICBkb3dubG9hZExpc3QoKSB7XHJcbiAgICAgICAgJChcIiNsaXN0Q3JpdGVyaWFEaXNwbGF5XCIpLmZpbmQoXCIjbWVzc2FnZVwiKS5yZW1vdmUoKTtcclxuICAgICAgY29uc3QgY291bnQgPSB0aGlzLmxpc3RDcml0ZXJpYVZpZXdNb2RlbC5jb3VudCgpO1xyXG4gICAgICBpZiAoY291bnQgPiAyMDAwMDAgJiYgY291bnQgPCA2MDAwMDApIHtcclxuICAgICAgICAkKFwiI2xpc3RDcml0ZXJpYURpc3BsYXlcIilcclxuICAgICAgICAgIC5wcmVwZW5kKFxyXG4gICAgICAgICAgICBgPGRpdiBzdHlsZT1cInBhZGRpbmc6IDhweDtcIiBjbGFzcz1cImFsZXJ0IGFsZXJ0LXdhcm5pbmdcIiBpZD1cIm1lc3NhZ2VcIj5MYXJnZXIgbGlzdHMgY2FuIHRha2UgdXAgdG8gNSBtaW51dGVzIHRvIGJ1aWxkLiBEb24ndCBjbG9zZSB5b3VyIGJyb3dzZXIuPC9kaXY+YCk7XHJcbiAgICAgICAgcmV0dXJuIGZhbHNlO1xyXG4gICAgICB9IGVsc2UgaWYgKHRoaXMubGlzdENyaXRlcmlhVmlld01vZGVsLmNvdW50KCkgPj0gNjAwMDAwKSB7XHJcbiAgICAgICAgJChcIiNsaXN0Q3JpdGVyaWFEaXNwbGF5XCIpXHJcbiAgICAgICAgICAucHJlcGVuZChcclxuICAgICAgICAgICAgYDxkaXYgc3R5bGU9XCJwYWRkaW5nOiA4cHg7XCIgY2xhc3M9XCJhbGVydCBhbGVydC1kYW5nZXJcIiBpZD1cIm1lc3NhZ2VcIj5MaXN0IHRvIGxhcmdlIHRvIGRvd25sb2FkLiBQbGVhc2UgYWRkIGNyaXRlcmlhIG9yIGNvbnRhY3Qgc3VwcG9ydC48L2Rpdj5gKTtcclxuICAgICAgICByZXR1cm4gZmFsc2U7XHJcbiAgICAgIH1cclxuICAgICAgJChcIiNsaXN0QnVpbGRXYWl0aW5nXCIpLnNob3coKTtcclxuICAgICAgJChcIiNkb3dubG9hZExpc3RcIikuYXR0cihcImRpc2FibGVkXCIsIFwiZGlzYWJsZWRcIikuZmluZChcIltjbGFzcz0nZmEgZmEtcmVmcmVzaCBmYS1zcGluJ11cIikuc2hvdygpO1xyXG4gICAgICAkLnBvc3QoXCIvTGlzdEJ1aWxkZXIvQnVpbGRMaXN0L1F1ZXJ5XCIsXHJcbiAgICAgICAgeyBsaXN0Q3JpdGVyaWE6IGtvLnRvSlNPTih0aGlzLmxpc3RDcml0ZXJpYVZpZXdNb2RlbCkgfSxcclxuICAgICAgICBkYXRhID0+IHtcclxuICAgICAgICAgIGlmIChkYXRhLkh0dHBTdGF0dXNDb2RlUmVzdWx0ID09PSAyMDApIHtcclxuICAgICAgICAgICAgdGhpcy5saXN0RG93bmxvYWRVcmkoZGF0YS5MaXN0RG93bmxvYWRVcmkpO1xyXG4gICAgICAgICAgICB0aGlzLmxpc3REZWZpbnRpb25Eb3dubG9hZFVyaShkYXRhLkxpc3REZWZpbnRpb25Eb3dubG9hZFVyaSk7XHJcbiAgICAgICAgICAgIHdpbmRvdy5sb2NhdGlvbi5yZXBsYWNlKHRoaXMubGlzdERvd25sb2FkVXJpKCkpO1xyXG4gICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgJChcIiNsaXN0Q3JpdGVyaWFEaXNwbGF5XCIpXHJcbiAgICAgICAgICAgICAgLnByZXBlbmQoYDxkaXYgc3R5bGU9XCJwYWRkaW5nOiA4cHg7XCIgY2xhc3M9XCJhbGVydCBhbGVydC13YXJuaW5nXCIgaWQ9XCJtZXNzYWdlXCI+JHtkYXRhLk1lc3NhZ2V9PC9kaXY+YCk7XHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgICAkKFwiI2xpc3RCdWlsZFdhaXRpbmdcIikuaGlkZSgpO1xyXG4gICAgICAgICAgJChcIiNkb3dubG9hZExpc3RcIikucmVtb3ZlQXR0cihcImRpc2FibGVkXCIpLmZpbmQoXCJbY2xhc3M9J2ZhIGZhLXJlZnJlc2ggZmEtc3BpbiddXCIpLmhpZGUoKTtcclxuICAgICAgICB9KTtcclxuICAgICAgcmV0dXJuIHRydWU7XHJcbiAgICB9XHJcblxyXG4gICAgZG93bmxvYWREZWZpbnRpb24oKSB7XHJcbiAgICAgIHdpbmRvdy5sb2NhdGlvbi5yZXBsYWNlKHRoaXMubGlzdERlZmludGlvbkRvd25sb2FkVXJpKCkpO1xyXG4gICAgfVxyXG5cclxuICAgIGVuaGFuY2VMaXN0KCkge1xyXG4gICAgICAkKFwiI2xpc3RDcml0ZXJpYURpc3BsYXlcIikuZmluZChcIiNtZXNzYWdlXCIpLnJlbW92ZSgpO1xyXG4gICAgICBpZiAodGhpcy5saXN0Q3JpdGVyaWFWaWV3TW9kZWwuY291bnQoKSA+IDQwMDAwMCkge1xyXG4gICAgICAgICQoXCIjbGlzdENyaXRlcmlhRGlzcGxheVwiKVxyXG4gICAgICAgICAgLnByZXBlbmQoXHJcbiAgICAgICAgICAgIGA8ZGl2IHN0eWxlPVwicGFkZGluZzogOHB4O1wiIGNsYXNzPVwiYWxlcnQgYWxlcnQtZGFuZ2VyXCIgaWQ9XCJtZXNzYWdlXCI+TGlzdCB0byBsYXJnZSB0byBlbmhhbmNlLiBQbGVhc2UgYWRkIGNyaXRlcmlhIG9yIGNvbnRhY3Qgc3VwcG9ydC48L2Rpdj5gKTtcclxuICAgICAgICByZXR1cm4gZmFsc2U7XHJcbiAgICAgIH1cclxuICAgICAgJChcIiNsaXN0Q3JpdGVyaWFEaXNwbGF5XCIpLmZpbmQoXCIjbGlzdEJ1aWxkV2FpdGluZ1wiKS5zaG93KCk7XHJcbiAgICAgICQoXCIjZW5oYW5jZUxpc3RcIikuYXR0cihcImRpc2FibGVkXCIsIFwiZGlzYWJsZWRcIikuZmluZChcIltjbGFzcz0nZmEgZmEtcmVmcmVzaCBmYS1zcGluJ11cIikuc2hvdygpO1xyXG4gICAgICAkLnBvc3QoXCIvTGlzdEJ1aWxkZXIvQnVpbGRMaXN0L0NyZWF0ZVwiLFxyXG4gICAgICAgIHsgbGlzdENyaXRlcmlhOiBrby50b0pTT04odGhpcy5saXN0Q3JpdGVyaWFWaWV3TW9kZWwpIH0sXHJcbiAgICAgICAgZGF0YSA9PiB7XHJcbiAgICAgICAgICBpZiAoZGF0YS5IdHRwU3RhdHVzQ29kZVJlc3VsdCA9PT0gMjAwKSB7XHJcbiAgICAgICAgICAgICQoXCIjbGlzdENyaXRlcmlhRGlzcGxheVwiKVxyXG4gICAgICAgICAgICAgIC5wcmVwZW5kKFxyXG4gICAgICAgICAgICAgICAgYDxkaXYgc3R5bGU9XCJwYWRkaW5nOiA4cHg7XCIgY2xhc3M9XCJhbGVydCBhbGVydC1zdWNjZXNzXCIgaWQ9XCJtZXNzYWdlXCI+TGlzdCBiZWluZyBnZW5lcmF0ZWQuIFlvdSB3aWxsIGJlIHJlZGlyZWN0ZWQgc2hvcnRseS48L2Rpdj5gKTtcclxuICAgICAgICAgICAgd2luZG93LmxvY2F0aW9uLnJlcGxhY2UoZGF0YS5Eb3dubG9hZFVyaSk7XHJcbiAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAkKFwiI2xpc3RDcml0ZXJpYURpc3BsYXlcIilcclxuICAgICAgICAgICAgICAucHJlcGVuZChgPGRpdiBzdHlsZT1cInBhZGRpbmc6IDhweDtcIiBjbGFzcz1cImFsZXJ0IGFsZXJ0LXdhcm5pbmdcIiBpZD1cIm1lc3NhZ2VcIj4ke2RhdGEuTWVzc2FnZX08L2Rpdj5gKTtcclxuICAgICAgICAgIH1cclxuICAgICAgICAgICQoXCIjbGlzdENyaXRlcmlhRGlzcGxheVwiKS5maW5kKFwiI2xpc3RCdWlsZFdhaXRpbmdcIikuaGlkZSgpO1xyXG4gICAgICAgICAgJChcIiNsaXN0Q3JpdGVyaWFEaXNwbGF5XCIpLmZpbmQoXCIjZW5oYW5jZUxpc3RcIikucmVtb3ZlQXR0cihcImRpc2FibGVkXCIpLmZpbmQoXCJbY2xhc3M9J2ZhIGZhLXJlZnJlc2ggZmEtc3BpbiddXCIpXHJcbiAgICAgICAgICAgIC5oaWRlKCk7XHJcbiAgICAgICAgfSk7XHJcbiAgICAgIHJldHVybiB0cnVlO1xyXG4gICAgfVxyXG5cclxuICAgIGdlbmVyYXRlVXVpZCgpIHtcclxuICAgICAgbGV0IGQgPSBuZXcgRGF0ZSgpLmdldFRpbWUoKTtcclxuICAgICAgY29uc3QgdXVpZCA9IFwieHh4eHh4eHgteHh4eC00eHh4LXl4eHgteHh4eHh4eHh4eHh4XCIucmVwbGFjZSgvW3h5XS9nLFxyXG4gICAgICAgIGMgPT4ge1xyXG4gICAgICAgICAgdmFyIHIgPSAoZCArIE1hdGgucmFuZG9tKCkgKiAxNikgJSAxNiB8IDA7XHJcbiAgICAgICAgICBkID0gTWF0aC5mbG9vcihkIC8gMTYpO1xyXG4gICAgICAgICAgcmV0dXJuIChjID09PSBcInhcIiA/IHIgOiAociAmIDB4NyB8IDB4OCkpLnRvU3RyaW5nKDE2KTtcclxuICAgICAgICB9KTtcclxuICAgICAgcmV0dXJuIHV1aWQ7XHJcbiAgICB9O1xyXG5cclxuICAgIG51bWJlcldpdGhDb21tYXMoeCkge1xyXG4gICAgICByZXR1cm4geC50b1N0cmluZygpLnJlcGxhY2UoL1xcQig/PShcXGR7M30pKyg/IVxcZCkpL2csIFwiLFwiKTtcclxuICAgIH1cclxuICB9XHJcblxyXG59XHJcblxyXG5sZXQgdmlld01vZGVsMTogQWNjdXJhdGVBcHBlbmQuTGlzdEJ1aWxkZXIuVmlld01vZGVsO1xyXG5cclxuJCgoKSA9PiB7XHJcblxyXG4gIHZpZXdNb2RlbDEgPSBuZXcgQWNjdXJhdGVBcHBlbmQuTGlzdEJ1aWxkZXIuVmlld01vZGVsKGxvb2t1cFVybHMsIHJlcXVlc3RJZCk7XHJcbiAgLy9pZiAoJChcIiNsaXN0Q3JpdGVyaWFcIikudmFsKCkpIHZpZXdNb2RlbDEubGlzdENyaXRlcmlhVmlld01vZGVsLmZyb21Kc29uKCQoXCIjbGlzdENyaXRlcmlhXCIpLnZhbCgpKTtcclxuICBrby5hcHBseUJpbmRpbmdzKHZpZXdNb2RlbDEpO1xyXG5cclxuICAkKFwiI2RvYl9zdGFydFwiKS5rZW5kb01hc2tlZFRleHRCb3goeyBtYXNrOiBcIjAwLTAwMDBcIiB9KTtcclxuICAkKFwiI2RvYl9lbmRcIikua2VuZG9NYXNrZWRUZXh0Qm94KHsgbWFzazogXCIwMC0wMDAwXCIgfSk7XHJcblxyXG59KTsiXX0=