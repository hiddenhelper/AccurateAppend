var smtpRulesSummaryViewModel;
var userId;
var manifestId;
var smtpRuleLinks;
$(function () {
    smtpRulesSummaryViewModel = new SmtpRulesSummary.ViewModel(userId);
    smtpRulesSummaryViewModel.load();
    ko.applyBindings(smtpRulesSummaryViewModel);
    $("#files").kendoUpload({
        async: {
            saveUrl: "save",
        }
    });
    $("#newAutoMapRule").bind("click", function () { window.location.replace(smtpRuleLinks.NewAutoMappedRule); });
    $("#newFixedMapRule").bind("click", function () { window.location.replace(smtpRuleLinks.NewMappedRule); });
});
var SmtpRulesSummary;
(function (SmtpRulesSummary) {
    var ViewModel = (function () {
        function ViewModel(userid) {
            var _this = this;
            this.editRule = function (rule) {
                _this.currentRule(rule);
                $("#edit-rule-modal").modal("show");
            };
            this.rules = ko.observableArray();
            this.currentRule = ko.observable();
            this.userid = userid;
        }
        ViewModel.prototype.openDeleteConfirmationModal = function (rule) {
            var viewModel = smtpRulesSummaryViewModel;
            viewModel.currentRule(rule);
            $("#delete-rule-modal").modal("show");
        };
        ;
        ViewModel.prototype.delete = function (rule) {
            var viewModel = smtpRulesSummaryViewModel;
            $.ajax({
                context: this,
                type: "GET",
                url: smtpRuleLinks.Delete + "/?id=" + rule.rid(),
                success: function (data) {
                    console.log("delete() success");
                    $("#delete-rule-modal").modal("hide");
                    viewModel.load();
                    displayMessage("#globalMessage", "Rule successfully deleted", "success");
                },
                error: function (xhr) {
                    $("#error").html("<strong>Error:</strong> Unable to delete rule. Message: " + xhr.statusText).show();
                }
            });
        };
        ;
        ViewModel.prototype.save = function (rule) {
            var viewModel = smtpRulesSummaryViewModel;
            $.ajax({
                type: "POST",
                url: "" + smtpRuleLinks.Update,
                data: { json: ko.toJSON({ "Rules": viewModel.rules, UserId: userId }) },
                success: function (data) {
                    $("#edit-rule-modal").modal("hide");
                    displayMessage("#globalMessage", data.Message, "success");
                    viewModel.load();
                },
                error: function (xhr) {
                    $("#error").html("<strong>Error:</strong> Unable to save rule. Message: " + xhr.statusText).show();
                }
            });
        };
        ;
        ViewModel.prototype.load = function () {
            var viewModel = smtpRulesSummaryViewModel;
            viewModel.rules.removeAll();
            $.ajax({
                context: this,
                type: "GET",
                url: "" + smtpRuleLinks.ForCurrentUser,
                success: function (data) {
                    $.each(data, function (i, e) {
                        viewModel.rules.push(new Rule(e.UserId, e.ManifestId, e.DateAdded, e.RunOrder, e.rid, e.Terms, e.Description, (e.Default ? true : false), e.Subject, e.FileName, e.Body));
                    });
                    if (manifestId) {
                        var rule = new Rule(userId, manifestId, moment().format("MM/DD/YYYY HH:mm:ss a"), 0);
                        viewModel.rules.push(rule);
                        this.editRule(rule);
                        manifestId = null;
                        displayMessage("#edit-rule-modal #message", "Please update the Terms and Description, and click Save", "info");
                    }
                },
                error: function (xhr) {
                    $("#error").html("<strong>Error:</strong> Unable to retrieve rules. Message: " + xhr.statusText).show();
                }
            });
        };
        ;
        ViewModel.prototype.downloadManifest = function (rule) {
            window.location.replace(smtpRuleLinks.Download + "/?id=" + rule.manifestId);
        };
        ViewModel.prototype.setDefault = function (rule) {
        };
        return ViewModel;
    }());
    SmtpRulesSummary.ViewModel = ViewModel;
    var Rule = (function () {
        function Rule(userid, manifestId, dateAdded, order, rid, terms, description, isDefault, subject, fileName, body) {
            this.userid = userid;
            this.manifestId = manifestId;
            this.dateAdded = dateAdded;
            this.order = order;
            this.rid = ko.observable(rid);
            this.terms = ko.observable(terms);
            this.description = ko.observable(description);
            this.isDefault = ko.observable(isDefault);
            this.subject = ko.observable(subject);
            this.fileName = ko.observable(fileName);
            this.body = ko.observable(body);
        }
        return Rule;
    }());
    SmtpRulesSummary.Rule = Rule;
    function displayMessage(selector, message, type) {
        $(selector).removeClass().addClass("alert alert-" + type).html(message).show()
            .fadeTo(10000, 500).slideUp(500, function () { $("#globalMessage").slideUp(500); });
    }
})(SmtpRulesSummary || (SmtpRulesSummary = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiSW5kZXguanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJJbmRleC50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFJQSxJQUFJLHlCQUE4QixDQUFDO0FBQ25DLElBQUksTUFBYyxDQUFDO0FBQ25CLElBQUksVUFBa0IsQ0FBQztBQUN2QixJQUFJLGFBQWtCLENBQUM7QUFFdkIsQ0FBQyxDQUFDO0lBRUEseUJBQXlCLEdBQUcsSUFBSSxnQkFBZ0IsQ0FBQyxTQUFTLENBQUMsTUFBTSxDQUFDLENBQUM7SUFDbkUseUJBQXlCLENBQUMsSUFBSSxFQUFFLENBQUM7SUFDakMsRUFBRSxDQUFDLGFBQWEsQ0FBQyx5QkFBeUIsQ0FBQyxDQUFDO0lBRTVDLENBQUMsQ0FBQyxRQUFRLENBQUMsQ0FBQyxXQUFXLENBQUM7UUFDdEIsS0FBSyxFQUFFO1lBQ0wsT0FBTyxFQUFFLE1BQU07U0FFaEI7S0FDRixDQUFDLENBQUM7SUFFSCxDQUFDLENBQUMsaUJBQWlCLENBQUMsQ0FBQyxJQUFJLENBQUMsT0FBTyxFQUMvQixjQUFRLE1BQU0sQ0FBQyxRQUFRLENBQUMsT0FBTyxDQUFDLGFBQWEsQ0FBQyxpQkFBaUIsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUM7SUFDdkUsQ0FBQyxDQUFDLGtCQUFrQixDQUFDLENBQUMsSUFBSSxDQUFDLE9BQU8sRUFDaEMsY0FBUSxNQUFNLENBQUMsUUFBUSxDQUFDLE9BQU8sQ0FBQyxhQUFhLENBQUMsYUFBYSxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQztBQUVyRSxDQUFDLENBQUMsQ0FBQztBQUVILElBQU8sZ0JBQWdCLENBaUt0QjtBQWpLRCxXQUFPLGdCQUFnQjtJQUVyQjtRQU1FLG1CQUFZLE1BQVc7WUFBdkIsaUJBSUM7WUE4RkQsYUFBUSxHQUFHLFVBQUMsSUFBSTtnQkFDZCxLQUFJLENBQUMsV0FBVyxDQUFDLElBQUksQ0FBQyxDQUFDO2dCQUN2QixDQUFDLENBQUMsa0JBQWtCLENBQUMsQ0FBQyxLQUFL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cUJBQUksT0FzQ2hCLENBQUE7SUFFRCxTQUFTLGNBQWMsQ0FBQyxRQUFhLEVBQUUsT0FBWSxFQUFFLElBQVM7UUFDNUQsQ0FBQyxDQUFDLFFBQVEsQ0FBQyxDQUFDLFdBQVcsRUFBRSxDQUFDLFFBQVEsQ0FBQyxpQkFBZSxJQUFNLENBQUMsQ0FBQyxJQUFJLENBQUMsT0FBTyxDQUFDLENBQUMsSUFBSSxFQUFFO2FBQzNFLE1BQU0sQ0FBQyxLQUFLLEVBQUUsR0FBRyxDQUFDLENBQUMsT0FBTyxDQUFDLEdBQUcsRUFBRSxjQUFRLENBQUMsQ0FBQyxnQkFBZ0IsQ0FBQyxDQUFDLE9BQU8sQ0FBQyxHQUFHLENBQUMsQ0FBQSxDQUFDLENBQUMsQ0FBQyxDQUFDO0lBQ2pGLENBQUM7QUFFSCxDQUFDLEVBaktNLGdCQUFnQixLQUFoQixnQkFBZ0IsUUFpS3RCIiwic291cmNlc0NvbnRlbnQiOlsiLy8vIDxyZWZlcmVuY2UgcGF0aD1cIi4uLy4uLy4uLy4uL3NjcmlwdHMvdHlwaW5ncy9rZW5kby11aS9rZW5kby11aS5kLnRzXCIgLz5cclxuLy8vIDxyZWZlcmVuY2UgcGF0aD1cIi4uLy4uLy4uLy4uL3NjcmlwdHMvdHlwaW5ncy9rbm9ja291dC9rbm9ja291dC5kLnRzXCIgLz5cclxuLy8vIDxyZWZlcmVuY2UgcGF0aD1cIi4uLy4uLy4uLy4uL3NjcmlwdHMvdHlwaW5ncy9tb21lbnQvbW9tZW50LmQudHNcIiAvPlxyXG5cclxudmFyIHNtdHBSdWxlc1N1bW1hcnlWaWV3TW9kZWw6IGFueTtcclxudmFyIHVzZXJJZDogc3RyaW5nO1xyXG52YXIgbWFuaWZlc3RJZDogc3RyaW5nO1xyXG52YXIgc210cFJ1bGVMaW5rczogYW55O1xyXG5cclxuJCgoKSA9PiB7XHJcblxyXG4gIHNtdHBSdWxlc1N1bW1hcnlWaWV3TW9kZWwgPSBuZXcgU210cFJ1bGVzU3VtbWFyeS5WaWV3TW9kZWwodXNlcklkKTtcclxuICBzbXRwUnVsZXNTdW1tYXJ5Vmlld01vZGVsLmxvYWQoKTtcclxuICBrby5hcHBseUJpbmRpbmdzKHNtdHBSdWxlc1N1bW1hcnlWaWV3TW9kZWwpO1xyXG5cclxuICAkKFwiI2ZpbGVzXCIpLmtlbmRvVXBsb2FkKHtcclxuICAgIGFzeW5jOiB7XHJcbiAgICAgIHNhdmVVcmw6IFwic2F2ZVwiLFxyXG4gICAgICAvL2F1dG9VcGxvYWQ6IHRydWVcclxuICAgIH1cclxuICB9KTtcclxuXHJcbiAgJChcIiNuZXdBdXRvTWFwUnVsZVwiKS5iaW5kKFwiY2xpY2tcIixcclxuICAgICgpID0+IHsgd2luZG93LmxvY2F0aW9uLnJlcGxhY2Uoc210cFJ1bGVMaW5rcy5OZXdBdXRvTWFwcGVkUnVsZSk7IH0pO1xyXG4gICQoXCIjbmV3Rml4ZWRNYXBSdWxlXCIpLmJpbmQoXCJjbGlja1wiLFxyXG4gICAgKCkgPT4geyB3aW5kb3cubG9jYXRpb24ucmVwbGFjZShzbXRwUnVsZUxpbmtzLk5ld01hcHBlZFJ1bGUpOyB9KTtcclxuXHJcbn0pO1xyXG5cclxubW9kdWxlIFNtdHBSdWxlc1N1bW1hcnkge1xyXG5cclxuICBleHBvcnQgY2xhc3MgVmlld01vZGVsIHtcclxuXHJcbiAgICBydWxlczogS25vY2tvdXRPYnNlcnZhYmxlQXJyYXk8UnVsZT47XHJcbiAgICBjdXJyZW50UnVsZTogS25vY2tvdXRPYnNlcnZhYmxlPFJ1bGU+O1xyXG4gICAgdXNlcmlkOiBzdHJpbmc7XHJcblxyXG4gICAgY29uc3RydWN0b3IodXNlcmlkOiBhbnkpIHtcclxuICAgICAgdGhpcy5ydWxlcyA9IGtvLm9ic2VydmFibGVBcnJheSgpO1xyXG4gICAgICB0aGlzLmN1cnJlbnRSdWxlID0ga28ub2JzZXJ2YWJsZSgpO1xyXG4gICAgICB0aGlzLnVzZXJpZCA9IHVzZXJpZDtcclxuICAgIH1cclxuXHJcbiAgICBvcGVuRGVsZXRlQ29uZmlybWF0aW9uTW9kYWwocnVsZSkge1xyXG4gICAgICBjb25zdCB2aWV3TW9kZWwgPSBzbXRwUnVsZXNTdW1tYXJ5Vmlld01vZGVsO1xyXG4gICAgICB2aWV3TW9kZWwuY3VycmVudFJ1bGUocnVsZSk7XHJcbiAgICAgICQoXCIjZGVsZXRlLXJ1bGUtbW9kYWxcIikubW9kYWwoXCJzaG93XCIpO1xyXG4gICAgfTtcclxuXHJcbiAgICBkZWxldGUocnVsZSkge1xyXG4gICAgICBjb25zdCB2aWV3TW9kZWwgPSBzbXRwUnVsZXNTdW1tYXJ5Vmlld01vZGVsO1xyXG4gICAgICAkLmFqYXgoXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgY29udGV4dDogdGhpcyxcclxuICAgICAgICAgIHR5cGU6IFwiR0VUXCIsXHJcbiAgICAgICAgICB1cmw6IGAke3NtdHBSdWxlTGlua3MuRGVsZXRlfS8/aWQ9JHtydWxlLnJpZCgpfWAsXHJcbiAgICAgICAgICBzdWNjZXNzKGRhdGEpIHtcclxuICAgICAgICAgICAgY29uc29sZS5sb2coYGRlbGV0ZSgpIHN1Y2Nlc3NgKTtcclxuICAgICAgICAgICAgJChcIiNkZWxldGUtcnVsZS1tb2RhbFwiKS5tb2RhbChcImhpZGVcIik7XHJcbiAgICAgICAgICAgIHZpZXdNb2RlbC5sb2FkKCk7XHJcbiAgICAgICAgICAgIGRpc3BsYXlNZXNzYWdlKFwiI2dsb2JhbE1lc3NhZ2VcIiwgXCJSdWxlIHN1Y2Nlc3NmdWxseSBkZWxldGVkXCIsIFwic3VjY2Vzc1wiKTtcclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICBlcnJvcih4aHIpIHtcclxuICAgICAgICAgICAgJChcIiNlcnJvclwiKS5odG1sKGA8c3Ryb25nPkVycm9yOjwvc3Ryb25nPiBVbmFibGUgdG8gZGVsZXRlIHJ1bGUuIE1lc3NhZ2U6ICR7eGhyLnN0YXR1c1RleHR9YCkuc2hvdygpO1xyXG4gICAgICAgICAgfVxyXG4gICAgICAgIH0pO1xyXG4gICAgfTtcclxuXHJcbiAgICBzYXZlKHJ1bGUpIHtcclxuICAgICAgY29uc3Qgdmlld01vZGVsID0gc210cFJ1bGVzU3VtbWFyeVZpZXdNb2RlbDtcclxuICAgICAgJC5hamF4KFxyXG4gICAgICAgIHtcclxuICAgICAgICAgIHR5cGU6IFwiUE9TVFwiLFxyXG4gICAgICAgICAgdXJsOiBgJHtzbXRwUnVsZUxpbmtzLlVwZGF0ZX1gLFxyXG4gICAgICAgICAgZGF0YTogeyBqc29uOiBrby50b0pTT04oeyBcIlJ1bGVzXCI6IHZpZXdNb2RlbC5ydWxlcywgVXNlcklkOiB1c2VySWQgfSkgfSxcclxuICAgICAgICAgIHN1Y2Nlc3MoZGF0YSkge1xyXG4gICAgICAgICAgICAkKFwiI2VkaXQtcnVsZS1tb2RhbFwiKS5tb2RhbChcImhpZGVcIik7XHJcbiAgICAgICAgICAgIGRpc3BsYXlNZXNzYWdlKFwiI2dsb2JhbE1lc3NhZ2VcIiwgZGF0YS5NZXNzYWdlLCBcInN1Y2Nlc3NcIik7XHJcbiAgICAgICAgICAgIHZpZXdNb2RlbC5sb2FkKCk7XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAgZXJyb3IoeGhyKSB7XHJcbiAgICAgICAgICAgICQoXCIjZXJyb3JcIikuaHRtbChgPHN0cm9uZz5FcnJvcjo8L3N0cm9uZz4gVW5hYmxlIHRvIHNhdmUgcnVsZS4gTWVzc2FnZTogJHt4aHIuc3RhdHVzVGV4dH1gKS5zaG93KCk7XHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgfSk7XHJcbiAgICB9O1xyXG5cclxuICAgIGxvYWQoKSB7XHJcbiAgICAgIGNvbnN0IHZpZXdNb2RlbCA9IHNtdHBSdWxlc1N1bW1hcnlWaWV3TW9kZWw7XHJcbiAgICAgIHZpZXdNb2RlbC5ydWxlcy5yZW1vdmVBbGwoKTtcclxuICAgICAgJC5hamF4KFxyXG4gICAgICAgIHtcclxuICAgICAgICAgIGNvbnRleHQ6IHRoaXMsXHJcbiAgICAgICAgICB0eXBlOiBcIkdFVFwiLFxyXG4gICAgICAgICAgdXJsOiBgJHtzbXRwUnVsZUxpbmtzLkZvckN1cnJlbnRVc2VyfWAsXHJcbiAgICAgICAgICBzdWNjZXNzKGRhdGEpIHtcclxuICAgICAgICAgICAgJC5lYWNoKGRhdGEsXHJcbiAgICAgICAgICAgICAgKGksIGUpID0+IHtcclxuICAgICAgICAgICAgICAgIHZpZXdNb2RlbC5ydWxlcy5wdXNoKG5ldyBSdWxlKFxyXG4gICAgICAgICAgICAgICAgICBlLlVzZXJJZCxcclxuICAgICAgICAgICAgICAgICAgZS5NYW5pZmVzdElkLFxyXG4gICAgICAgICAgICAgICAgICBlLkRhdGVBZGRlZCxcclxuICAgICAgICAgICAgICAgICAgZS5SdW5PcmRlcixcclxuICAgICAgICAgICAgICAgICAgZS5yaWQsXHJcbiAgICAgICAgICAgICAgICAgIGUuVGVybXMsXHJcbiAgICAgICAgICAgICAgICAgIGUuRGVzY3JpcHRpb24sXHJcbiAgICAgICAgICAgICAgICAgIChlLkRlZmF1bHQgPyB0cnVlIDogZmFsc2UpLFxyXG4gICAgICAgICAgICAgICAgICBlLlN1YmplY3QsXHJcbiAgICAgICAgICAgICAgICAgIGUuRmlsZU5hbWUsXHJcbiAgICAgICAgICAgICAgICAgIGUuQm9keVxyXG4gICAgICAgICAgICAgICAgKSk7XHJcbiAgICAgICAgICAgICAgfSk7XHJcblxyXG4gICAgICAgICAgICAvLyBtYW5pZmVzdElkIGlzIHBvc3RlZCBmcm9tIER5bmFtaWNBcHBlbmQgaWYgYSBuZXcgcnVsZSBpcyBiZWluZyBjcmVhdGVkLCBhZGQgbmV3IHJ1bGUgdG8gY29sbGVjdGlvblxyXG4gICAgICAgICAgICBpZiAobWFuaWZlc3RJZCkge1xyXG4gICAgICAgICAgICAgIGNvbnN0IHJ1bGUgPSBuZXcgUnVsZSh1c2VySWQsXHJcbiAgICAgICAgICAgICAgICBtYW5pZmVzdElkLFxyXG4gICAgICAgICAgICAgICAgbW9tZW50KCkuZm9ybWF0KFwiTU0vREQvWVlZWSBISDptbTpzcyBhXCIpLFxyXG4gICAgICAgICAgICAgICAgICAgIDApO1xyXG4gICAgICAgICAgICAgIFxyXG4gICAgICAgICAgICAgIHZpZXdNb2RlbC5ydWxlcy5wdXNoKHJ1bGUpO1xyXG4gICAgICAgICAgICAgICAgdGhpcy5lZGl0UnVsZShydWxlKTtcclxuICAgICAgICAgICAgICBtYW5pZmVzdElkID0gbnVsbDtcclxuICAgICAgICAgICAgICAgIGRpc3BsYXlNZXNzYWdlKFwiI2VkaXQtcnVsZS1tb2RhbCAjbWVzc2FnZVwiLCBcIlBsZWFzZSB1cGRhdGUgdGhlIFRlcm1zIGFuZCBEZXNjcmlwdGlvbiwgYW5kIGNsaWNrIFNhdmVcIiwgXCJpbmZvXCIpO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAgZXJyb3IoeGhyKSB7XHJcbiAgICAgICAgICAgICQoXCIjZXJyb3JcIikuaHRtbChgPHN0cm9uZz5FcnJvcjo8L3N0cm9uZz4gVW5hYmxlIHRvIHJldHJpZXZlIHJ1bGVzLiBNZXNzYWdlOiAke3hoci5zdGF0dXNUZXh0fWApLnNob3coKTtcclxuICAgICAgICAgIH1cclxuICAgICAgICB9KTtcclxuICAgIH07XHJcblxyXG4gICAgZG93bmxvYWRNYW5pZmVzdChydWxlKSB7XHJcbiAgICAgIHdpbmRvdy5sb2NhdGlvbi5yZXBsYWNlKGAke3NtdHBSdWxlTGlua3MuRG93bmxvYWR9Lz9pZD0ke3J1bGUubWFuaWZlc3RJZH1gKTtcclxuICAgIH1cclxuXHJcbiAgICBlZGl0UnVsZSA9IChydWxlKSA9PiB7XHJcbiAgICAgIHRoaXMuY3VycmVudFJ1bGUocnVsZSk7XHJcbiAgICAgICQoXCIjZWRpdC1ydWxlLW1vZGFsXCIpLm1vZGFsKFwic2hvd1wiKTtcclxuICAgIH07XHJcblxyXG4gICAgc2V0RGVmYXVsdChydWxlOiBSdWxlKSB7XHJcblxyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgZXhwb3J0IGNsYXNzIFJ1bGUge1xyXG4gICAgdXNlcmlkOiBhbnk7XHJcbiAgICBtYW5pZmVzdElkOiBhbnk7XHJcbiAgICBkYXRlQWRkZWQ6IGFueTtcclxuICAgIG9yZGVyOiBhbnk7XHJcbiAgICByaWQ6IGFueTtcclxuICAgIHRlcm1zOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPjtcclxuICAgIGRlc2NyaXB0aW9uOiBLbm9ja291dE9ic2VydmFibGU8c3RyaW5nPjtcclxuICAgIGlzRGVmYXVsdDogS25vY2tvdXRPYnNlcnZhYmxlPGJvb2xlYW4+O1xyXG4gICAgc3ViamVjdDogS25vY2tvdXRPYnNlcnZhYmxlPGJvb2xlYW4+O1xyXG4gICAgZmlsZU5hbWU6IEtub2Nrb3V0T2JzZXJ2YWJsZTxib29sZWFuPjtcclxuICAgIGJvZHk6IEtub2Nrb3V0T2JzZXJ2YWJsZTxib29sZWFuPjtcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihcclxuICAgICAgdXNlcmlkOiBhbnksXHJcbiAgICAgIG1hbmlmZXN0SWQ6IGFueSxcclxuICAgICAgZGF0ZUFkZGVkOiBhbnksXHJcbiAgICAgIG9yZGVyPzogYW55LFxyXG4gICAgICByaWQ/OiBhbnksXHJcbiAgICAgIHRlcm1zPzogYW55LFxyXG4gICAgICBkZXNjcmlwdGlvbj86IGFueSxcclxuICAgICAgaXNEZWZhdWx0PzogYW55LFxyXG4gICAgICBzdWJqZWN0PzogYW55LFxyXG4gICAgICBmaWxlTmFtZT86IGFueSxcclxuICAgICAgYm9keT86IGFueVxyXG4gICAgKSB7XHJcbiAgICAgIHRoaXMudXNlcmlkID0gdXNlcmlkO1xyXG4gICAgICB0aGlzLm1hbmlmZXN0SWQgPSBtYW5pZmVzdElkO1xyXG4gICAgICB0aGlzLmRhdGVBZGRlZCA9IGRhdGVBZGRlZDtcclxuICAgICAgdGhpcy5vcmRlciA9IG9yZGVyO1xyXG4gICAgICB0aGlzLnJpZCA9IGtvLm9ic2VydmFibGUocmlkKTtcclxuICAgICAgdGhpcy50ZXJtcyA9IGtvLm9ic2VydmFibGUodGVybXMpO1xyXG4gICAgICB0aGlzLmRlc2NyaXB0aW9uID0ga28ub2JzZXJ2YWJsZShkZXNjcmlwdGlvbik7XHJcbiAgICAgIHRoaXMuaXNEZWZhdWx0ID0ga28ub2JzZXJ2YWJsZShpc0RlZmF1bHQpO1xyXG4gICAgICB0aGlzLnN1YmplY3QgPSBrby5vYnNlcnZhYmxlKHN1YmplY3QpO1xyXG4gICAgICB0aGlzLmZpbGVOYW1lID0ga28ub2JzZXJ2YWJsZShmaWxlTmFtZSk7XHJcbiAgICAgIHRoaXMuYm9keSA9IGtvLm9ic2VydmFibGUoYm9keSk7XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICBmdW5jdGlvbiBkaXNwbGF5TWVzc2FnZShzZWxlY3RvcjogYW55LCBtZXNzYWdlOiBhbnksIHR5cGU6IGFueSkge1xyXG4gICAgJChzZWxlY3RvcikucmVtb3ZlQ2xhc3MoKS5hZGRDbGFzcyhgYWxlcnQgYWxlcnQtJHt0eXBlfWApLmh0bWwobWVzc2FnZSkuc2hvdygpXHJcbiAgICAgIC5mYWRlVG8oMTAwMDAsIDUwMCkuc2xpZGVVcCg1MDAsICgpID0+IHsgJChcIiNnbG9iYWxNZXNzYWdlXCIpLnNsaWRlVXAoNTAwKSB9KTtcclxuICB9XHJcblxyXG59Il19