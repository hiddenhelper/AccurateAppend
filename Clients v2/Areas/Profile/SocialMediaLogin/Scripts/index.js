var AccurateAppend;
(function (AccurateAppend) {
    var Websites;
    (function (Websites) {
        var Clients;
        (function (Clients) {
            var Profile;
            (function (Profile) {
                var SocialMediaLogin;
                (function (SocialMediaLogin) {
                    var IndexView = (function () {
                        function IndexView() {
                            this.notification = new Clients.Shared.NotificationHelper('message');
                        }
                        IndexView.prototype.removeLink = function (url, e) {
                            var self = this;
                            this.notification.clearMessage();
                            e.preventDefault();
                            this.showAjaxInProgress(e.target);
                            $.ajax({
                                url: url,
                                dataType: 'json',
                                type: 'DELETE',
                                success: function (result) {
                                    if (result.HttpStatusCodeResult === 500) {
                                        self.notification.showError('Error occured removing social medial login');
                                    }
                                    else {
                                        if (result.Success === true) {
                                            self.notification.showSuccess(result.Message);
                                            var grid = $("#grid").data("kendoGrid");
                                            grid.dataSource.read();
                                        }
                                        else {
                                            self.notification.showWarning(result.Message);
                                        }
                                    }
                                },
                                complete: function () {
                                    self.clearAjaxInProgress(e.target);
                                }
                            });
                            return false;
                        };
                        IndexView.prototype.showAjaxInProgress = function (el) {
                            $(el).addClass('ajax-in-progress');
                            $(el).attr('disabled', 'true');
                        };
                        IndexView.prototype.clearAjaxInProgress = function (el) {
                            $(el).removeClass('ajax-in-progress');
                            $(el).removeAttr("disabled");
                        };
                        IndexView.prototype.initialize = function (url) {
                            $("#grid").kendoGrid({
                                autoBind: true,
                                dataSource: {
                                    type: "json",
                                    schema: {
                                        type: "json",
                                        data: "Data",
                                        total: function (response) {
                                            return response.length;
                                        }
                                    },
                                    change: function () {
                                        if (this.data().length <= 0) {
                                            $("#no-logins-message").show();
                                            $("#grid").hide();
                                        }
                                        else {
                                            $("#no-logins-message").hide();
                                            $("#grid").show();
                                        }
                                    },
                                    transport: {
                                        read: function (options) {
                                            $.ajax({
                                                url: url,
                                                dataType: 'json',
                                                type: 'GET',
                                                data: {},
                                                success: function (result) {
                                                    if (result.HttpStatusCodeResult === 500) {
                                                        console.log("Read returned 500 status.");
                                                    }
                                                    else {
                                                        if (result.length <= 0) {
                                                            $("#no-logins-message").show();
                                                            $("#grid").hide();
                                                        }
                                                        else {
                                                            $("#no-logins-message").hide();
                                                            $("#grid").show();
                                                        }
                                                        options.success({ Data: result });
                                                    }
                                                }
                                            });
                                        }
                                    }
                                },
                                scrollable: false,
                                pageable: false,
                                columns: [
                                    {
                                        field: "Name",
                                        title: "Name"
                                    },
                                    {
                                        field: "ProviderName",
                                        title: "Login From"
                                    },
                                    {
                                        template: "<button class=\"btn btn-primary\" onClick=\"socialMediaLoginIndexView.removeLink(\'#= Actions.Remove #\',event)\">remove</button>"
                                    }
                                ]
                            });
                        };
                        return IndexView;
                    }());
                    SocialMediaLogin.IndexView = IndexView;
                })(SocialMediaLogin = Profile.SocialMediaLogin || (Profile.SocialMediaLogin = {}));
            })(Profile = Clients.Profile || (Clients.Profile = {}));
        })(Clients = Websites.Clients || (Websites.Clients = {}));
    })(Websites = AccurateAppend.Websites || (AccurateAppend.Websites = {}));
})(AccurateAppend || (AccurateAppend = {}));
var socialMediaLoginIndexView;
function initializeSocialMediaLoginView(url) {
    $(document).ready(function () {
        socialMediaLoginIndexView = new AccurateAppend.Websites.Clients.Profile.SocialMediaLogin.IndexView();
        socialMediaLoginIndexView.initialize(url);
    });
}
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiaW5kZXguanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJpbmRleC50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFJQSxJQUFPLGNBQWMsQ0FtSXBCO0FBbklELFdBQU8sY0FBYztJQUFDLElBQUEsUUFBUSxDQW1JN0I7SUFuSXFCLFdBQUEsUUFBUTtRQUFDLElBQUEsT0FBTyxDQW1JckM7UUFuSThCLFdBQUEsT0FBTztZQUFDLElBQUEsT0FBTyxDQW1JN0M7WUFuSXNDLFdBQUEsT0FBTztnQkFBQyxJQUFBLGdCQUFnQixDQW1JOUQ7Z0JBbkk4QyxXQUFBLGdCQUFnQjtvQkFFM0Q7d0JBSUk7NEJBQ0ksSUFBSSxDQUFDLFlBQVksR0FBRyxJQUFJLE9BQU8sQ0FBQyxNQUFNLENBQUMsa0JBQWtCLENBQUMsU0FBUyxDQUFDLENBQUM7d0JBQ3pFLENBQUM7d0JBR0QsOEJBQVUsR0FBVixVQUFXLEdBQUcsRUFBQyxDQUFDOzRCQUVaLElBQUksSUFBSSxHQUFHLElBQUksQ0FBQzs0QkFDaEIsSUFBSSxDQUFDLFlBQVksQ0FBQyxZQUFZLEVBQUUsQ0FBQzs0QkFDakMsQ0FBQyxDQUFDLGNBQWMsRUFBRSxDQUFDOzRCQUVuQixJQUFJLENBQUMsa0JBQWtCLENBQUMsQ0FBQyxDQUFDLE1BQU0sQ0FBQyxDQUFDOzRCQUVsQyxDQUFDLENBQUMsSUFBSSxDQUFDO2dDQUNILEdBQUcsRUFBRSxHQUFHO2dDQUNSLFFBQVEsRUFBRSxNQUFNO2dDQUNoQixJQUFJLEVBQUUsUUFBUTtnQ0FDZCxPQUFPLFlBQUMsTUFBTTtvQ0FDVixJQUFJLE1BQU0sQ0FBQyxvQkFBb0IsS0FBSyxHQUFHLEVBQUU7d0NBQ3JDLElBQUksQ0FBQyxZQUFZLENBQUMsU0FBUyxDQUFDLDRDQUE0QyxDQUFDLENBQUM7cUNBQzdFO3lDQUNJO3dDQUNELElBQUksTUFBTSxDQUFDLE9BQU8sS0FBSyxJQUFJLEVBQUU7NENBQ3pCLElBQUksQ0FBQyxZQUFZLENBQUMsV0FBVyxDQUFDLE1BQU0sQ0FB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Y2UgcGF0aD1cIi4uLy4uLy4uLy4uL1NjcmlwdHMvdHlwaW5ncy9odHRwc3RhdHVzY29kZS50c1wiIC8+XHJcbi8vLyA8cmVmZXJlbmNlIHBhdGg9XCIuLi8uLi8uLi8uLi92aWV3cy9zaGFyZWQvc2NyaXB0cy9ub3RpZmljYXRpb25oZWxwZXIudHNcIiAvPlxyXG4vLy8gPHJlZmVyZW5jZSBwYXRoPVwiLi4vLi4vLi4vLi4vU2NyaXB0cy90eXBpbmdzL2tlbmRvLXVpL2tlbmRvLXVpLmQudHNcIiAvPlxyXG5cclxubW9kdWxlIEFjY3VyYXRlQXBwZW5kLldlYnNpdGVzLkNsaWVudHMuUHJvZmlsZS5Tb2NpYWxNZWRpYUxvZ2luIHtcclxuXHJcbiAgICBleHBvcnQgY2xhc3MgSW5kZXhWaWV3IHtcclxuXHJcbiAgICAgICAgbm90aWZpY2F0aW9uOiBDbGllbnRzLlNoYXJlZC5Ob3RpZmljYXRpb25IZWxwZXI7XHJcblxyXG4gICAgICAgIGNvbnN0cnVjdG9yKCkge1xyXG4gICAgICAgICAgICB0aGlzLm5vdGlmaWNhdGlvbiA9IG5ldyBDbGllbnRzLlNoYXJlZC5Ob3RpZmljYXRpb25IZWxwZXIoJ21lc3NhZ2UnKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIC8vYW4gZXZlbnQgaGFuZGxlciBmb3IgcmVtb3ZlIGJ1dHRvblxyXG4gICAgICAgIHJlbW92ZUxpbmsodXJsLGUpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBsZXQgc2VsZiA9IHRoaXM7XHJcbiAgICAgICAgICAgIHRoaXMubm90aWZpY2F0aW9uLmNsZWFyTWVzc2FnZSgpO1xyXG4gICAgICAgICAgICBlLnByZXZlbnREZWZhdWx0KCk7XHJcblxyXG4gICAgICAgICAgICB0aGlzLnNob3dBamF4SW5Qcm9ncmVzcyhlLnRhcmdldCk7XHJcbiAgICAgICAgICAgIFxyXG4gICAgICAgICAgICAkLmFqYXgoe1xyXG4gICAgICAgICAgICAgICAgdXJsOiB1cmwsXHJcbiAgICAgICAgICAgICAgICBkYXRhVHlwZTogJ2pzb24nLFxyXG4gICAgICAgICAgICAgICAgdHlwZTogJ0RFTEVURScsXHJcbiAgICAgICAgICAgICAgICBzdWNjZXNzKHJlc3VsdCkge1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChyZXN1bHQuSHR0cFN0YXR1c0NvZGVSZXN1bHQgPT09IDUwMCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBzZWxmLm5vdGlmaWNhdGlvbi5zaG93RXJyb3IoJ0Vycm9yIG9jY3VyZWQgcmVtb3Zpbmcgc29jaWFsIG1lZGlhbCBsb2dpbicpO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICBlbHNlIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKHJlc3VsdC5TdWNjZXNzID09PSB0cnVlKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBzZWxmLm5vdGlmaWNhdGlvbi5zaG93U3VjY2VzcyhyZXN1bHQuTWVzc2FnZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB2YXIgZ3JpZCA9ICQoXCIjZ3JpZFwiKS5kYXRhKFwia2VuZG9HcmlkXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgZ3JpZC5kYXRhU291cmNlLnJlYWQoKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICBlbHNlIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHNlbGYubm90aWZpY2F0aW9uLnNob3dXYXJuaW5nKHJlc3VsdC5NZXNzYWdlKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgY29tcGxldGUoKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgc2VsZi5jbGVhckFqYXhJblByb2dyZXNzKGUudGFyZ2V0KTtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfSk7XHJcblxyXG4gICAgICAgICAgICByZXR1cm4gZmFsc2U7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAvL1RPRE8gOiBtb3ZlIHRoaXMgbWV0aG9kIHRvIGEgc2hhcmUgYXJlYSBmb3Igb3RoZXIgc2NyaXB0cyB0byB1c2VcclxuICAgICAgICAvL2Rpc2FibGVzIGFuZCBhZGRzIGEgbG9hZGluZyBnaWYgd2hpbGUgYW4gYWpheCBjYWxsIGlzIGlucHJvZ3Jlc3NcclxuICAgICAgICBzaG93QWpheEluUHJvZ3Jlc3MoZWwpIHtcclxuICAgICAgICAgICAgJChlbCkuYWRkQ2xhc3MoJ2FqYXgtaW4tcHJvZ3Jlc3MnKTtcclxuICAgICAgICAgICAgJChlbCkuYXR0cignZGlzYWJsZWQnLCAndHJ1ZScpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy9UT0RPIDogbW92ZSB0aGlzIG1ldGhvZCB0byBhIHNoYXJlIGFyZWEgZm9yIG90aGVyIHNjcmlwdHMgdG8gdXNlXHJcbiAgICAgICAgLy9jbGVhcnMgdGhlIGxvYWRpbmcgZ2lmIGFuZCByZS1lbmFibGVzIHRoZSBidXR0b24gYWZ0ZXIgYW4gYWpheCBjYWxsIGlzIGNvbXBsZXRlZCBcclxuICAgICAgICBjbGVhckFqYXhJblByb2dyZXNzKGVsKSB7XHJcbiAgICAgICAgICAgICQoZWwpLnJlbW92ZUNsYXNzKCdhamF4LWluLXByb2dyZXNzJyk7XHJcbiAgICAgICAgICAgICQoZWwpLnJlbW92ZUF0dHIoXCJkaXNhYmxlZFwiKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIC8vaW5pdGlhbGl6ZSB0aGUga2VuZG8gZ3JpZFxyXG4gICAgICAgIGluaXRpYWxpemUodXJsKSB7XHJcblxyXG4gICAgICAgICAgICAkKFwiI2dyaWRcIikua2VuZG9HcmlkKHtcclxuICAgICAgICAgICAgICAgIGF1dG9CaW5kOiB0cnVlLFxyXG4gICAgICAgICAgICAgICAgZGF0YVNvdXJjZToge1xyXG4gICAgICAgICAgICAgICAgICAgIHR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgICAgICAgIHNjaGVtYToge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB0eXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgZGF0YTogXCJEYXRhXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHRvdGFsKHJlc3BvbnNlKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICByZXR1cm4gcmVzcG9uc2UubGVuZ3RoO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICBjaGFuZ2U6IGZ1bmN0aW9uICgpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKHRoaXMuZGF0YSgpLmxlbmd0aCA8PSAwKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKFwiI25vLWxvZ2lucy1tZXNzYWdlXCIpLnNob3coKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICQoXCIjZ3JpZFwiKS5oaWRlKCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKFwiI25vLWxvZ2lucy1tZXNzYWdlXCIpLmhpZGUoKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICQoXCIjZ3JpZFwiKS5zaG93KCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgIHRyYW5zcG9ydDoge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByZWFkKG9wdGlvbnMpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICQuYWpheCh7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdXJsOiB1cmwsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgZGF0YVR5cGU6ICdqc29uJyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB0eXBlOiAnR0VUJyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBkYXRhOiB7fSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBzdWNjZXNzKHJlc3VsdCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAocmVzdWx0Lkh0dHBTdGF0dXNDb2RlUmVzdWx0ID09PSA1MDApIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNvbnNvbGUubG9nKFwiUmVhZCByZXR1cm5lZCA1MDAgc3RhdHVzLlwiKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGlmIChyZXN1bHQubGVuZ3RoIDw9IDApIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKFwiI25vLWxvZ2lucy1tZXNzYWdlXCIpLnNob3coKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKFwiI2dyaWRcIikuaGlkZSgpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKFwiI25vLWxvZ2lucy1tZXNzYWdlXCIpLmhpZGUoKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKFwiI2dyaWRcIikuc2hvdygpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIG9wdGlvbnMuc3VjY2Vzcyh7IERhdGE6IHJlc3VsdCB9KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgIHNjcm9sbGFibGU6IGZhbHNlLFxyXG4gICAgICAgICAgICAgICAgXHJcbiAgICAgICAgICAgICAgICBwYWdlYWJsZTogZmFsc2UsXHJcbiAgICAgICAgICAgICAgICBjb2x1bW5zOiBbXHJcbiAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBmaWVsZDogXCJOYW1lXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHRpdGxlOiBcIk5hbWVcIlxyXG4gICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBmaWVsZDogXCJQcm92aWRlck5hbWVcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgdGl0bGU6IFwiTG9naW4gRnJvbVwiXHJcbiAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHRlbXBsYXRlOiBcIjxidXR0b24gY2xhc3M9XFxcImJ0biBidG4tcHJpbWFyeVxcXCIgb25DbGljaz1cXFwic29jaWFsTWVkaWFMb2dpbkluZGV4Vmlldy5yZW1vdmVMaW5rKFxcJyM9IEFjdGlvbnMuUmVtb3ZlICNcXCcsZXZlbnQpXFxcIj5yZW1vdmU8L2J1dHRvbj5cIlxyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIF1cclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBcclxuICAgIH1cclxufVxyXG5cclxubGV0IHNvY2lhbE1lZGlhTG9naW5JbmRleFZpZXc6IEFjY3VyYXRlQXBwZW5kLldlYnNpdGVzLkNsaWVudHMuUHJvZmlsZS5Tb2NpYWxNZWRpYUxvZ2luLkluZGV4VmlldztcclxuXHJcbmZ1bmN0aW9uIGluaXRpYWxpemVTb2NpYWxNZWRpYUxvZ2luVmlldyh1cmwpIHtcclxuICAgICQoZG9jdW1lbnQpLnJlYWR5KCgpID0+IHtcclxuICAgICAgICBzb2NpYWxNZWRpYUxvZ2luSW5kZXhWaWV3ID0gbmV3IEFjY3VyYXRlQXBwZW5kLldlYnNpdGVzLkNsaWVudHMuUHJvZmlsZS5Tb2NpYWxNZWRpYUxvZ2luLkluZGV4VmlldygpO1xyXG4gICAgICAgIHNvY2lhbE1lZGlhTG9naW5JbmRleFZpZXcuaW5pdGlhbGl6ZSh1cmwpO1xyXG4gICAgfSk7XHJcbn0iXX0=