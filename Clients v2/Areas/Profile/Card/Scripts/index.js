var AccurateAppend;
(function (AccurateAppend) {
    var Websites;
    (function (Websites) {
        var Clients;
        (function (Clients) {
            var Profile;
            (function (Profile) {
                var Card;
                (function (Card) {
                    var IndexView = (function () {
                        function IndexView(url) {
                            this.cardUrl = url;
                        }
                        IndexView.prototype.populateForm = function (data) {
                            $("#cardHolderFirstName").val(data.Primary.BillTo.FirstName);
                            $("#cardHolderLastName").val(data.Primary.BillTo.LastName);
                            $("#cardHolderBusinessName").val(data.Primary.BillTo.BusinessName);
                            $("#cardPostalCode").val(data.Primary.BillTo.PostalCode);
                            $("#cardHolderPhone").val(data.Primary.BillTo.PhoneNumber);
                            $("#cardNumber").attr("placeholder", data.DisplayValue);
                            $("#cardExpirationMonth").val(data.Primary.Expiration.substring(0, 2)).change();
                            $("#cardExpirationYear").val(data.Primary.Expiration.substring(3, 7)).change();
                        };
                        IndexView.prototype.resetForm = function () {
                            var submitButton = $("#submitBtn");
                            submitButton.html("Update");
                            submitButton.prop('disabled', false);
                        };
                        IndexView.prototype.submitForm = function (el) {
                            console.log("Submitting form");
                            var self = this;
                            var submitButton = $("#submitBtn");
                            submitButton.prop('disabled', true);
                            submitButton.html(submitButton.html() + ' <i class="fa fa-refresh fa-spin" style="font-size: 20px;"></i> ');
                            $.post(self.cardUrl, $('form').serialize(), function (e) {
                                console.log("Form processed");
                                ;
                                if (!e.Success) {
                                    var summary = $(".validation-summary-valid").find("ul");
                                    summary.empty();
                                    e.Errors.forEach(function (item) {
                                        summary.append($("<li>").text(item.ErrorMessage));
                                    });
                                    self.resetForm();
                                }
                                else {
                                    $("#alert").addClass("alert alert-success").text("Your payment details have been updated.").show();
                                }
                            });
                            return false;
                        };
                        IndexView.prototype.initialize = function (url) {
                            var self = this;
                            var channel = $.connection.callbackHub;
                            channel.client.callbackComplete = function () {
                                self.resetForm();
                            };
                            $.connection.hub.start().done(function () {
                                console.log("Connection established: " + $.connection.hub.id);
                                $("#connectionId").val($.connection.hub.id);
                                console.log("Loading data");
                                $.ajax({
                                    type: "GET",
                                    url: url,
                                    success: function (result) {
                                        if (result.Primary != null)
                                            self.populateForm(result);
                                    }
                                });
                                self.resetForm();
                            });
                        };
                        return IndexView;
                    }());
                    Card.IndexView = IndexView;
                })(Card = Profile.Card || (Profile.Card = {}));
            })(Profile = Clients.Profile || (Clients.Profile = {}));
        })(Clients = Websites.Clients || (Websites.Clients = {}));
    })(Websites = AccurateAppend.Websites || (AccurateAppend.Websites = {}));
})(AccurateAppend || (AccurateAppend = {}));
var cardIndexView;
function initializeCardView(primaryCardUrl, cardUrl) {
    $(document).ready(function () {
        cardIndexView = new AccurateAppend.Websites.Clients.Profile.Card.IndexView(cardUrl);
        cardIndexView.initialize(primaryCardUrl);
    });
}
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiaW5kZXguanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJpbmRleC50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUF3QkEsSUFBTyxjQUFjLENBZ0ZwQjtBQWhGRCxXQUFPLGNBQWM7SUFBQyxJQUFBLFFBQVEsQ0FnRjdCO0lBaEZxQixXQUFBLFFBQVE7UUFBQyxJQUFBLE9BQU8sQ0FnRnJDO1FBaEY4QixXQUFBLE9BQU87WUFBQyxJQUFBLE9BQU8sQ0FnRjdDO1lBaEZzQyxXQUFBLE9BQU87Z0JBQUMsSUFBQSxJQUFJLENBZ0ZsRDtnQkFoRjhDLFdBQUEsSUFBSTtvQkFFL0M7d0JBR0ksbUJBQVksR0FBVzs0QkFDbkIsSUFBSSxDQUFDLE9BQU8sR0FBRyxHQUFHLENBQUM7d0JBQ3ZCLENBQUM7d0JBRUQsZ0NBQVksR0FBWixVQUFhLElBQUk7NEJBQ2IsQ0FBQyxDQUFDLHNCQUFzQixDQUFDLENBQUMsR0FBRyxDQUFDLElBQUksQ0FBQyxPQUFPLENBQUMsTUFBTSxDQUFDLFNBQVMsQ0FBQyxDQUFDOzRCQUM3RCxDQUFDLENBQUMscUJBQXFCLENBQUMsQ0FBQyxHQUFHLENBQUMsSUFBSSxDQUFDLE9BQU8sQ0FBQyxNQUFNLENBQUMsUUFBUSxDQUFDLENBQUM7NEJBQzNELENBQUMsQ0FBQyx5QkFBeUIsQ0FBQyxDQUFDLEdBQUcsQ0FBQyxJQUFJLENBQUMsT0FBTyxDQUFDLE1BQU0sQ0FBQyxZQUFZLENBQUMsQ0FBQzs0QkFDbkUsQ0FBQyxDQUFDLGlCQUFpQixDQUFDLENBQUMsR0FBRyxDQUFDLElBQUksQ0FBQyxPQUFPLENBQUMsTUFBTSxDQUFDLFVBQVUsQ0FBQyxDQUFDOzRCQUN6RCxDQUFDLENBQUMsa0JBQWtCLENBQUMsQ0FBQyxHQUFHLENBQUMsSUFBSSxDQUFDLE9BQU8sQ0FBQyxNQUFNLENBQUMsV0FBVyxDQUFDLENBQUM7NEJBQzNELENBQUMsQ0FBQyxhQUFhLENBQUMsQ0FBQyxJQUFJLENBQUMsYUFBYSxFQUFFLElBQUksQ0FBQyxZQUFZ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bmludGVyZmFjZSBIdWIge1xyXG4gICAgaWQ6IHN0cmluZztcclxuICAgIHN0YXRlOiBhbnk7XHJcbiAgICBzdGFydChvcHRpb25zPzogYW55LCBjYWxsYmFjaz86ICgpID0+IGFueSk6IEpRdWVyeVByb21pc2U8YW55PjtcclxufVxyXG5cclxuaW50ZXJmYWNlIENhbGxiYWNrSHViIHtcclxuICAgIGNsaWVudDogQ2FsbGJhY2tIdWJDbGllbnQ7XHJcbn1cclxuXHJcbmludGVyZmFjZSBDYWxsYmFja0h1YkNsaWVudCB7XHJcbiAgICBjYWxsYmFja0NvbXBsZXRlOiAoKSA9PiB2b2lkO1xyXG59XHJcblxyXG5pbnRlcmZhY2UgU2lnbmFsUiB7XHJcbiAgICBjYWxsYmFja0h1YjogQ2FsbGJhY2tIdWI7XHJcbiAgICBodWI6IEh1YjtcclxufVxyXG5cclxuaW50ZXJmYWNlIEpRdWVyeVN0YXRpYyB7XHJcbiAgICBjb25uZWN0aW9uOiBTaWduYWxSO1xyXG59XHJcblxyXG5tb2R1bGUgQWNjdXJhdGVBcHBlbmQuV2Vic2l0ZXMuQ2xpZW50cy5Qcm9maWxlLkNhcmQge1xyXG5cclxuICAgIGV4cG9ydCBjbGFzcyBJbmRleFZpZXcge1xyXG4gICAgICAgIGNhcmRVcmw6IHN0cmluZztcclxuXHJcbiAgICAgICAgY29uc3RydWN0b3IodXJsOiBzdHJpbmcpIHtcclxuICAgICAgICAgICAgdGhpcy5jYXJkVXJsID0gdXJsO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcG9wdWxhdGVGb3JtKGRhdGEpIHtcclxuICAgICAgICAgICAgJChcIiNjYXJkSG9sZGVyRmlyc3ROYW1lXCIpLnZhbChkYXRhLlByaW1hcnkuQmlsbFRvLkZpcnN0TmFtZSk7XHJcbiAgICAgICAgICAgICQoXCIjY2FyZEhvbGRlckxhc3ROYW1lXCIpLnZhbChkYXRhLlByaW1hcnkuQmlsbFRvLkxhc3ROYW1lKTtcclxuICAgICAgICAgICAgJChcIiNjYXJkSG9sZGVyQnVzaW5lc3NOYW1lXCIpLnZhbChkYXRhLlByaW1hcnkuQmlsbFRvLkJ1c2luZXNzTmFtZSk7XHJcbiAgICAgICAgICAgICQoXCIjY2FyZFBvc3RhbENvZGVcIikudmFsKGRhdGEuUHJpbWFyeS5CaWxsVG8uUG9zdGFsQ29kZSk7XHJcbiAgICAgICAgICAgICQoXCIjY2FyZEhvbGRlclBob25lXCIpLnZhbChkYXRhLlByaW1hcnkuQmlsbFRvLlBob25lTnVtYmVyKTtcclxuICAgICAgICAgICAgJChcIiNjYXJkTnVtYmVyXCIpLmF0dHIoXCJwbGFjZWhvbGRlclwiLCBkYXRhLkRpc3BsYXlWYWx1ZSk7XHJcbiAgICAgICAgICAgICQoXCIjY2FyZEV4cGlyYXRpb25Nb250aFwiKS52YWwoZGF0YS5QcmltYXJ5LkV4cGlyYXRpb24uc3Vic3RyaW5nKDAsIDIpKS5jaGFuZ2UoKTtcclxuICAgICAgICAgICAgJChcIiNjYXJkRXhwaXJhdGlvblllYXJcIikudmFsKGRhdGEuUHJpbWFyeS5FeHBpcmF0aW9uLnN1YnN0cmluZygzLCA3KSkuY2hhbmdlKCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXNldEZvcm0oKSB7XHJcbiAgICAgICAgICAgIHZhciBzdWJtaXRCdXR0b24gPSAkKFwiI3N1Ym1pdEJ0blwiKTtcclxuICAgICAgICAgICAgc3VibWl0QnV0dG9uLmh0bWwoXCJVcGRhdGVcIik7XHJcbiAgICAgICAgICAgIHN1Ym1pdEJ1dHRvbi5wcm9wKCdkaXNhYmxlZCcsIGZhbHNlKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHN1Ym1pdEZvcm0oZWwpIHtcclxuICAgICAgICAgICAgY29uc29sZS5sb2coXCJTdWJtaXR0aW5nIGZvcm1cIik7XHJcblxyXG4gICAgICAgICAgICBsZXQgc2VsZiA9IHRoaXM7XHJcbiAgICAgICAgICAgIHZhciBzdWJtaXRCdXR0b24gPSAkKFwiI3N1Ym1pdEJ0blwiKTtcclxuICAgICAgICAgICAgc3VibWl0QnV0dG9uLnByb3AoJ2Rpc2FibGVkJywgdHJ1ZSk7XHJcbiAgICAgICAgICAgIHN1Ym1pdEJ1dHRvbi5odG1sKHN1Ym1pdEJ1dHRvbi5odG1sKCkgKyAnIDxpIGNsYXNzPVwiZmEgZmEtcmVmcmVzaCBmYS1zcGluXCIgc3R5bGU9XCJmb250LXNpemU6IDIwcHg7XCI+PC9pPiAnKTtcclxuXHJcbiAgICAgICAgICAgICQucG9zdChcclxuICAgICAgICAgICAgICAgIHNlbGYuY2FyZFVybCxcclxuICAgICAgICAgICAgICAgICQoJ2Zvcm0nKS5zZXJpYWxpemUoKSxcclxuICAgICAgICAgICAgICAgIGZ1bmN0aW9uIChlKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgY29uc29sZS5sb2coXCJGb3JtIHByb2Nlc3NlZFwiKTs7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCFlLlN1Y2Nlc3MpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgdmFyIHN1bW1hcnkgPSAkKFwiLnZhbGlkYXRpb24tc3VtbWFyeS12YWxpZFwiKS5maW5kKFwidWxcIik7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHN1bW1hcnkuZW1wdHkoKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZS5FcnJvcnMuZm9yRWFjaChmdW5jdGlvbiAoaXRlbSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgc3VtbWFyeS5hcHBlbmQoJChcIjxsaT5cIikudGV4dChpdGVtLkVycm9yTWVzc2FnZSkpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgc2VsZi5yZXNldEZvcm0oKTtcclxuICAgICAgICAgICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAkKFwiI2FsZXJ0XCIpLmFkZENsYXNzKFwiYWxlcnQgYWxlcnQtc3VjY2Vzc1wiKS50ZXh0KFwiWW91ciBwYXltZW50IGRldGFpbHMgaGF2ZSBiZWVuIHVwZGF0ZWQuXCIpLnNob3coKTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgcmV0dXJuIGZhbHNlO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgaW5pdGlhbGl6ZSh1cmw6IHN0cmluZykge1xyXG4gICAgICAgICAgICBsZXQgc2VsZiA9IHRoaXM7XHJcbiAgICAgICAgICAgIC8vIFJlZmVyZW5jZSB0aGUgYXV0by1nZW5lcmF0ZWQgcHJveHkgZm9yIHRoZSBodWIuXHJcbiAgICAgICAgICAgIHZhciBjaGFubmVsID0gJC5jb25uZWN0aW9uLmNhbGxiYWNrSHViO1xyXG4gICAgICAgICAgICAvLyBDcmVhdGUgYSBmdW5jdGlvbiB0aGF0IHRoZSBodWIgY2FuIGNhbGwgYmFjayB0byBzaWduYWwgY29tcGxldGlvbiBtZXNzYWdlcy5cclxuICAgICAgICAgICAgY2hhbm5lbC5jbGllbnQuY2FsbGJhY2tDb21wbGV0ZSA9IGZ1bmN0aW9uICgpIHtcclxuICAgICAgICAgICAgICAgIHNlbGYucmVzZXRGb3JtKCk7XHJcbiAgICAgICAgICAgIH07XHJcbiAgICAgICAgICAgIC8vIFN0YXJ0IHRoZSBjb25uZWN0aW9uLlxyXG4gICAgICAgICAgICAkLmNvbm5lY3Rpb24uaHViLnN0YXJ0KCkuZG9uZShmdW5jdGlvbiAoKSB7XHJcbiAgICAgICAgICAgICAgICBjb25zb2xlLmxvZyhcIkNvbm5lY3Rpb24gZXN0YWJsaXNoZWQ6IFwiICsgJC5jb25uZWN0aW9uLmh1Yi5pZCk7XHJcbiAgICAgICAgICAgICAgICAkKFwiI2Nvbm5lY3Rpb25JZFwiKS52YWwoJC5jb25uZWN0aW9uLmh1Yi5pZCk7XHJcblxyXG4gICAgICAgICAgICAgICAgY29uc29sZS5sb2coXCJMb2FkaW5nIGRhdGFcIik7XHJcbiAgICAgICAgICAgICAgICAkLmFqYXgoe1xyXG4gICAgICAgICAgICAgICAgICAgIHR5cGU6IFwiR0VUXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgdXJsOiB1cmwsXHJcbiAgICAgICAgICAgICAgICAgICAgc3VjY2VzcyhyZXN1bHQpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKHJlc3VsdC5QcmltYXJ5ICE9IG51bGwpIHNlbGYucG9wdWxhdGVGb3JtKHJlc3VsdCk7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfSk7XHJcblxyXG4gICAgICAgICAgICAgICAgc2VsZi5yZXNldEZvcm0oKTtcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgIH1cclxufVxyXG5cclxubGV0IGNhcmRJbmRleFZpZXc6IEFjY3VyYXRlQXBwZW5kLldlYnNpdGVzLkNsaWVudHMuUHJvZmlsZS5DYXJkLkluZGV4VmlldztcclxuXHJcbmZ1bmN0aW9uIGluaXRpYWxpemVDYXJkVmlldyhwcmltYXJ5Q2FyZFVybCwgY2FyZFVybCkge1xyXG4gICAgJChkb2N1bWVudCkucmVhZHkoKCkgPT4ge1xyXG4gICAgICAgIGNhcmRJbmRleFZpZXcgPSBuZXcgQWNjdXJhdGVBcHBlbmQuV2Vic2l0ZXMuQ2xpZW50cy5Qcm9maWxlLkNhcmQuSW5kZXhWaWV3KGNhcmRVcmwpO1xyXG4gICAgICAgIGNhcmRJbmRleFZpZXcuaW5pdGlhbGl6ZShwcmltYXJ5Q2FyZFVybCk7XHJcbiAgICB9KTtcclxufSJdfQ==