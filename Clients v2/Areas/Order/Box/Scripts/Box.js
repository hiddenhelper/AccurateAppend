var viewModel;
$(document).ready(function () {
    viewModel = new AccurateAppend.Order.Box.ViewModel(forCurrentUser);
    viewModel.init();
});
var AccurateAppend;
(function (AccurateAppend) {
    var Order;
    (function (Order) {
        var Box;
        (function (Box) {
            var ViewModel = (function () {
                function ViewModel(forCurrentUser) {
                    this.forCurrentUser = forCurrentUser;
                }
                ViewModel.prototype.init = function () {
                    this.loadRegistrationDetails();
                };
                ViewModel.prototype.loadRegistrationDetails = function () {
                    $.ajax({
                        context: this,
                        url: forCurrentUser,
                        dataType: "json",
                        type: "GET",
                        success: function (result) {
                            var _this = this;
                            if (result.HttpStatus === 500) {
                                AccurateAppend.Util.displayMessage("globalMessage", result.Message, "warning");
                                return;
                            }
                            var data = result.data;
                            $.each(data, function () {
                                _this.registration = new BoxRegistration(data.Name, data.Id, data.UserId, data.DateRegistered, data.IsActive, new BoxActions(data.Actions.Enumerate, data.Actions.Renew, data.Actions.ChangeAccess));
                            });
                            this.loadTreeView(this.registration);
                        }
                    });
                };
                ViewModel.prototype.loadTreeView = function (registration) {
                    var ds = new kendo.data.HierarchicalDataSource({
                        transport: {
                            read: {
                                url: registration.Actions.Enumerate,
                                dataType: "json"
                            }
                        },
                        schema: {
                            model: {
                                id: "NodeId",
                                hasChildren: "HasChildren"
                            }
                        }
                    });
                    $("#treeview").kendoTreeView({
                        dataSource: ds,
                        dataTextField: "Name",
                        select: function (e) {
                            var node = e.node;
                            var treeview = $("#treeview").data("kendoTreeView");
                            var dataItem = treeview.dataItem(node);
                            $.ajax({
                                context: this,
                                url: dataItem["Actions"].Details,
                                dataType: "json",
                                type: "GET",
                                success: function (result) {
                                    $("#fileDetailPane").show();
                                    $("#details").html(kendo.template($("#file-detail").html())(result));
                                }
                            });
                        }
                    });
                };
                return ViewModel;
            }());
            Box.ViewModel = ViewModel;
            var BoxRegistration = (function () {
                function BoxRegistration(Name, Id, UserId, DateRegistered, IsActive, Actions) {
                    this.Name = Name;
                    this.Id = Id;
                    this.UserId = UserId;
                    this.DateRegistered = DateRegistered;
                    this.IsActive = IsActive;
                    this.Actions = Actions;
                }
                return BoxRegistration;
            }());
            var BoxActions = (function () {
                function BoxActions(Enumerate, Renew, ChangeAccess) {
                    this.Enumerate = Enumerate;
                    this.Renew = Renew;
                    this.ChangeAccess = ChangeAccess;
                }
                return BoxActions;
            }());
        })(Box = Order.Box || (Order.Box = {}));
    })(Order = AccurateAppend.Order || (AccurateAppend.Order = {}));
})(AccurateAppend || (AccurateAppend = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiQm94LmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiQm94LnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUtBLElBQUksU0FBYyxDQUFDO0FBRW5CLENBQUMsQ0FBQyxRQUFRLENBQUMsQ0FBQyxLQUFLLENBQUM7SUFFaEIsU0FBUyxHQUFHLElBQUksY0FBYyxDQUFDLEtBQUssQ0FBQyxHQUFHLENBQUMsU0FBUyxDQUFDLGNBQWMsQ0FBQyxDQUFDO0lBQ25FLFNBQVMsQ0FBQyxJQUFJLEVBQUUsQ0FBQztBQUVuQixDQUFDLENBQUMsQ0FBQztBQUVILElBQU8sY0FBYyxDQXFHcEI7QUFyR0QsV0FBTyxjQUFjO0lBQUMsSUFBQSxLQUFLLENBcUcxQjtJQXJHcUIsV0FBQSxLQUFLO1FBQUMsSUFBQSxHQUFHLENBcUc5QjtRQXJHMkIsV0FBQSxHQUFHO1lBQzdCO2dCQUlFLG1CQUFtQixjQUFzQjtvQkFBdEIsbUJBQWMsR0FBZCxjQUFjLENBQVE7Z0JBQ3pDLENBQUM7Z0JBRUQsd0JBQUksR0FBSjtvQkFDRSxJQUFJLENBQUMsdUJBQXVCLEVBQUUsQ0FBQztnQkFDakMsQ0FBQztnQkFFRCwyQ0FBdUIsR0FBdkI7b0JBQ0UsQ0FBQyxDQUFDLElBQUksQ0FBQzt3QkFDTCxPQUFPLEVBQUUsSUFBSTt3QkFDYixHQUFHLEVBQUUsY0FBYzt3QkFDbkIsUUFBUSxFQUFFLE1BQU07d0JBQ2hCLElBQUksRUFBRSxLQUFLO3dCQUNYLE9BQU8sWUFBQyxNQUFNOzRCQUFkLGlCQXFCQzs0QkFwQkMsSUFBSSxNQUFNLENBQUMsVUFBVSxLQUFLLEdBQUcsRUFBRTtnQ0FDN0IsZUFBQSxJQUFJLENBQUMsY0FBYyxDQUFDLGVBQWUsRUFBRSxNQUFNLENBQUMsT0FBTyxFQUFFLFNBQVMsQ0FBQyxDQUFDO2dDQUNoRSxPQUFPOzZCQUNSOzRCQUNELElBQU0sSUFBSSxHQUFHLE1BQU0sQ0FBQyxJQUFJLENBQUM7NEJBQ3pCLENBQUMsQ0FBQyxJQUFJLENBQUMsSUFBSSxFQUNU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WixZQUFZLENBQVE7Z0JBRTdCLENBQUM7Z0JBQ0gsaUJBQUM7WUFBRCxDQUFDLEFBUEQsSUFPQztRQUNILENBQUMsRUFyRzJCLEdBQUcsR0FBSCxTQUFHLEtBQUgsU0FBRyxRQXFHOUI7SUFBRCxDQUFDLEVBckdxQixLQUFLLEdBQUwsb0JBQUssS0FBTCxvQkFBSyxRQXFHMUI7QUFBRCxDQUFDLEVBckdNLGNBQWMsS0FBZCxjQUFjLFFBcUdwQiIsInNvdXJjZXNDb250ZW50IjpbIi8vLyA8cmVmZXJlbmNlIHBhdGg9XCIuLi8uLi8uLi8uLi9TY3JpcHRzL3R5cGluZ3Mva2VuZG8tdWkva2VuZG8tdWkuZC50c1wiIC8+XHJcbi8vLyA8cmVmZXJlbmNlIHBhdGg9XCIuLi8uLi8uLi8uLi9TY3JpcHRzL3R5cGluZ3MvSHR0cFN0YXR1c0NvZGUudHNcIiAvPlxyXG4vLy8gPHJlZmVyZW5jZSBwYXRoPVwiLi4vLi4vLi4vLi4vc2NyaXB0cy9hcHAvc2hhcmVkL3V0aWwudHNcIiAvPlxyXG5cclxuZGVjbGFyZSBsZXQgZm9yQ3VycmVudFVzZXI6IHN0cmluZztcclxudmFyIHZpZXdNb2RlbDogYW55O1xyXG5cclxuJChkb2N1bWVudCkucmVhZHkoKCkgPT4ge1xyXG5cclxuICB2aWV3TW9kZWwgPSBuZXcgQWNjdXJhdGVBcHBlbmQuT3JkZXIuQm94LlZpZXdNb2RlbChmb3JDdXJyZW50VXNlcik7XHJcbiAgdmlld01vZGVsLmluaXQoKTtcclxuXHJcbn0pO1xyXG5cclxubW9kdWxlIEFjY3VyYXRlQXBwZW5kLk9yZGVyLkJveCB7XHJcbiAgZXhwb3J0IGNsYXNzIFZpZXdNb2RlbCB7XHJcblxyXG4gICAgcmVnaXN0cmF0aW9uOiBCb3hSZWdpc3RyYXRpb247XHJcblxyXG4gICAgY29uc3RydWN0b3IocHVibGljIGZvckN1cnJlbnRVc2VyOiBzdHJpbmcpIHtcclxuICAgIH1cclxuXHJcbiAgICBpbml0KCkge1xyXG4gICAgICB0aGlzLmxvYWRSZWdpc3RyYXRpb25EZXRhaWxzKCk7XHJcbiAgICB9XHJcblxyXG4gICAgbG9hZFJlZ2lzdHJhdGlvbkRldGFpbHMoKSB7XHJcbiAgICAgICQuYWpheCh7XHJcbiAgICAgICAgY29udGV4dDogdGhpcyxcclxuICAgICAgICB1cmw6IGZvckN1cnJlbnRVc2VyLFxyXG4gICAgICAgIGRhdGFUeXBlOiBcImpzb25cIixcclxuICAgICAgICB0eXBlOiBcIkdFVFwiLFxyXG4gICAgICAgIHN1Y2Nlc3MocmVzdWx0KSB7XHJcbiAgICAgICAgICBpZiAocmVzdWx0Lkh0dHBTdGF0dXMgPT09IDUwMCkge1xyXG4gICAgICAgICAgICBVdGlsLmRpc3BsYXlNZXNzYWdlKFwiZ2xvYmFsTWVzc2FnZVwiLCByZXN1bHQuTWVzc2FnZSwgXCJ3YXJuaW5nXCIpO1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgICBjb25zdCBkYXRhID0gcmVzdWx0LmRhdGE7XHJcbiAgICAgICAgICAkLmVhY2goZGF0YSxcclxuICAgICAgICAgICAgKCkgPT4ge1xyXG4gICAgICAgICAgICAgIHRoaXMucmVnaXN0cmF0aW9uID0gbmV3IEJveFJlZ2lzdHJhdGlvbihcclxuICAgICAgICAgICAgICAgIGRhdGEuTmFtZSxcclxuICAgICAgICAgICAgICAgIGRhdGEuSWQsXHJcbiAgICAgICAgICAgICAgICBkYXRhLlVzZXJJZCxcclxuICAgICAgICAgICAgICAgIGRhdGEuRGF0ZVJlZ2lzdGVyZWQsXHJcbiAgICAgICAgICAgICAgICBkYXRhLklzQWN0aXZlLFxyXG4gICAgICAgICAgICAgICAgbmV3IEJveEFjdGlvbnMoXHJcbiAgICAgICAgICAgICAgICAgIGRhdGEuQWN0aW9ucy5FbnVtZXJhdGUsXHJcbiAgICAgICAgICAgICAgICAgIGRhdGEuQWN0aW9ucy5SZW5ldyxcclxuICAgICAgICAgICAgICAgICAgZGF0YS5BY3Rpb25zLkNoYW5nZUFjY2VzcylcclxuICAgICAgICAgICAgICApO1xyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICAgIHRoaXMubG9hZFRyZWVWaWV3KHRoaXMucmVnaXN0cmF0aW9uKTtcclxuICAgICAgICB9XHJcbiAgICAgIH0pO1xyXG4gICAgfVxyXG5cclxuICAgIGxvYWRUcmVlVmlldyhyZWdpc3RyYXRpb246IGFueSkge1xyXG4gICAgICBjb25zdCBkcyA9IG5ldyBrZW5kby5kYXRhLkhpZXJhcmNoaWNhbERhdGFTb3VyY2Uoe1xyXG4gICAgICAgIHRyYW5zcG9ydDoge1xyXG4gICAgICAgICAgcmVhZDoge1xyXG4gICAgICAgICAgICB1cmw6IHJlZ2lzdHJhdGlvbi5BY3Rpb25zLkVudW1lcmF0ZSxcclxuICAgICAgICAgICAgZGF0YVR5cGU6IFwianNvblwiXHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgfSxcclxuICAgICAgICBzY2hlbWE6IHtcclxuICAgICAgICAgIG1vZGVsOiB7XHJcbiAgICAgICAgICAgIGlkOiBcIk5vZGVJZFwiLFxyXG4gICAgICAgICAgICBoYXNDaGlsZHJlbjogXCJIYXNDaGlsZHJlblwiXHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG4gICAgICB9KTtcclxuXHJcbiAgICAgICQoXCIjdHJlZXZpZXdcIikua2VuZG9UcmVlVmlldyh7XHJcbiAgICAgICAgZGF0YVNvdXJjZTogZHMsXHJcbiAgICAgICAgZGF0YVRleHRGaWVsZDogXCJOYW1lXCIsXHJcbiAgICAgICAgc2VsZWN0IChlKSB7XHJcbiAgICAgICAgICBjb25zdCBub2RlID0gZS5ub2RlO1xyXG4gICAgICAgICAgY29uc3QgdHJlZXZpZXcgPSAkKFwiI3RyZWV2aWV3XCIpLmRhdGEoXCJrZW5kb1RyZWVWaWV3XCIpO1xyXG4gICAgICAgICAgY29uc3QgZGF0YUl0ZW0gPSB0cmVldmlldy5kYXRhSXRlbShub2RlKTtcclxuICAgICAgICAgICQuYWpheCh7XHJcbiAgICAgICAgICAgIGNvbnRleHQ6IHRoaXMsXHJcbiAgICAgICAgICAgIHVybDogZGF0YUl0ZW1bXCJBY3Rpb25zXCJdLkRldGFpbHMsXHJcbiAgICAgICAgICAgIGRhdGFUeXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgdHlwZTogXCJHRVRcIixcclxuICAgICAgICAgICAgc3VjY2VzcyhyZXN1bHQpIHtcclxuICAgICAgICAgICAgICAkKFwiI2ZpbGVEZXRhaWxQYW5lXCIpLnNob3coKTtcclxuICAgICAgICAgICAgICAkKFwiI2RldGFpbHNcIikuaHRtbChrZW5kby50ZW1wbGF0ZSgkKFwiI2ZpbGUtZGV0YWlsXCIpLmh0bWwoKSkocmVzdWx0KSk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIH0pO1xyXG4gICAgICAgIH1cclxuICAgICAgfSk7XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICBjbGFzcyBCb3hSZWdpc3RyYXRpb24ge1xyXG4gICAgY29uc3RydWN0b3IoXHJcbiAgICAgIHB1YmxpYyBOYW1lOiBzdHJpbmcsXHJcbiAgICAgIHB1YmxpYyBJZDogbnVtYmVyLFxyXG4gICAgICBwdWJsaWMgVXNlcklkOiBzdHJpbmcsXHJcbiAgICAgIHB1YmxpYyBEYXRlUmVnaXN0ZXJlZDogc3RyaW5nLFxyXG4gICAgICBwdWJsaWMgSXNBY3RpdmU6IGJvb2xlYW4sXHJcbiAgICAgIHB1YmxpYyBBY3Rpb25zOiBCb3hBY3Rpb25zXHJcbiAgICApIHtcclxuICAgIH1cclxuICB9XHJcblxyXG4gIGNsYXNzIEJveEFjdGlvbnMge1xyXG4gICAgY29uc3RydWN0b3IoXHJcbiAgICAgIHB1YmxpYyBFbnVtZXJhdGU6IHN0cmluZyxcclxuICAgICAgcHVibGljIFJlbmV3OiBzdHJpbmcsXHJcbiAgICAgIHB1YmxpYyBDaGFuZ2VBY2Nlc3M6IHN0cmluZ1xyXG4gICAgKSB7XHJcbiAgICB9XHJcbiAgfVxyXG59Il19