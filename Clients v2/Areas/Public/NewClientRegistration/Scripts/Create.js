var newClientRegistrationView;
var AccurateAppend;
(function (AccurateAppend) {
    var Websites;
    (function (Websites) {
        var Clients;
        (function (Clients) {
            var Public;
            (function (Public) {
                var NewClientRegistrationView = (function () {
                    function NewClientRegistrationView() {
                    }
                    NewClientRegistrationView.prototype.initialize = function (viewModel) {
                        this.ViewModel = viewModel;
                        $('#ckUseBillingAddress').change(function () {
                            if ($(this).is(":checked")) {
                                $("#CardHolderFirstName").val($("#FirstName").val());
                                $("#CardHolderLastName").val($("#LastName").val());
                                $("#CardAddress").val($("#Address").val());
                                $("#CardCity").val($("#City").val());
                                $("#CardStateAbbreviation").val($("#StateAbbreviation").val());
                                $("#CardStatePlainText").val($("#StatePlainText").val());
                                $("#CardCountry").val($("#Country").val());
                                $("#CardPostalCode").val($("#PostalCode").val());
                                $("#CardHolderPhone").val($("#Phone").val());
                                $("#CardNumber").focus();
                            }
                            else {
                                $("#CardHolderFirstName").val("");
                                $("#CardHolderLastName").val("");
                                $("#CardAddress").val("");
                                $("#CardCity").val("");
                                $("#CardPostalCode").val("");
                                $("#CardStateAbbreviation").val("");
                                $("#CardStatePlainText").val("");
                                $("#CardCountry").val("");
                            }
                        });
                        $("#lnkTerms").click(function () {
                            $("#tos-modal").modal('show');
                        });
                        if (viewModel.Model.CardCountry == viewModel.UnitedStates || viewModel.Model.CardCountry == viewModel.Canada || viewModel.Model.CardCountry == null) {
                            $("input[name=CardStatePlainText]").hide();
                            $("select[name=CardStateAbbreviation]").next().show();
                        }
                        else {
                            $("input[name=CardStatePlainText]").show();
                            $("select[name=CardStateAbbreviation]").next().hide();
                        }
                        $("select[name=CardCountry]").change(function () {
                            if ($(this).val() == viewModel.UnitedStates || $(this).val() == viewModel.Canada) {
                                $("input[name=CardStatePlainText]").hide();
                                $("select[name=CardStateAbbreviation]").val('');
                                $("select[name=CardStateAbbreviation]").next().show();
                            }
                            else {
                                $("input[name=CardStatePlainText]").show();
                                $("select[name=CardStateAbbreviation]").next().hide();
                            }
                        });
                        if (viewModel.Model.Country == viewModel.UnitedStates || viewModel.Model.Country == viewModel.Canada || viewModel.Model.Country == null) {
                            $("input[name=StatePlainText]").hide();
                            $("select[name=StateAbbreviation]").next().show();
                        }
                        else {
                            $("input[name=StatePlainText]").show();
                            $("select[name=StateAbbreviation]").next().hide();
                        }
                        $("select[name=Country]").change(function () {
                            if ($(this).val() == viewModel.UnitedStates || $(this).val() == viewModel.Canada) {
                                $("input[name=StatePlainText]").hide();
                                $("select[name=StateAbbreviation]").val('');
                                $("select[name=StateAbbreviation]").next().show();
                            }
                            else {
                                $("input[name=StatePlainText]").show();
                                $("select[name=StateAbbreviation]").next().hide();
                            }
                        });
                    };
                    return NewClientRegistrationView;
                }());
                Public.NewClientRegistrationView = NewClientRegistrationView;
            })(Public = Clients.Public || (Clients.Public = {}));
        })(Clients = Websites.Clients || (Websites.Clients = {}));
    })(Websites = AccurateAppend.Websites || (AccurateAppend.Websites = {}));
})(AccurateAppend || (AccurateAppend = {}));
$(document).ready(function () {
    (new AccurateAppend.Websites.Clients.Public.NewClientRegistrationView()).initialize(viewModel);
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiQ3JlYXRlLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiQ3JlYXRlLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLElBQUkseUJBQTJGLENBQUM7QUFFaEcsSUFBTyxjQUFjLENBdUZwQjtBQXZGRCxXQUFPLGNBQWM7SUFBQyxJQUFBLFFBQVEsQ0F1RjdCO0lBdkZxQixXQUFBLFFBQVE7UUFBQyxJQUFBLE9BQU8sQ0F1RnJDO1FBdkY4QixXQUFBLE9BQU87WUFBQyxJQUFBLE1BQU0sQ0F1RjVDO1lBdkZzQyxXQUFBLE1BQU07Z0JBT3pDO29CQUFBO29CQStFQSxDQUFDO29CQTVFRyw4Q0FBVSxHQUFWLFVBQVcsU0FBMEM7d0JBQ2pELElBQUksQ0FBQyxTQUFTLEdBQUcsU0FBUyxDQUFDO3dCQUUzQixDQUFDLENBQUMsc0JBQXNCLENBQUMsQ0FBQyxNQUFNLENBQUM7NEJBQzdCLElBQUksQ0FBQyxDQUFDLElBQUksQ0FBQyxDQUFDLEVBQUUsQ0FBQyxVQUFVLENBQUMsRUFBRTtnQ0FDeEIsQ0FBQyxDQUFDLHNCQUFzQixDQUFDLENBQUMsR0FBRyxDQUFDLENBQUMsQ0FBQyxZQUFZLENBQUMsQ0FBQyxHQUFHLEVBQUUsQ0FBQyxDQUFDO2dDQUNyRCxDQUFDLENBQUMscUJBQXFCLENBQUMsQ0FBQyxHQUFHLENBQUMsQ0FBQyxDQUFDLFdBQVcsQ0FBQyxDQUFDLEdBQUcsRUFBRSxDQUFDLENBQUM7Z0NBQ25ELENBQUMsQ0FBQyxjQUFjLENBQUMsQ0FBQyxHQUFHLENBQUMsQ0FBQyxDQUFDLFVBQVUsQ0FBQyxDQUFDLEdBQUcsRUFBRSxDQUFDLENBQUM7Z0NBQzNDLENBQUMsQ0FBQyxXQUFXLENBQUMsQ0FBQyxHQUFHLENBQUMsQ0FBQyxDQUFDLE9BQU8sQ0FBQyxDQUFDLEdBQUcsRUFBRSxDQUFDLENBQUM7Z0NBQ3JDLENBQUMsQ0FBQyx3QkFBd0IsQ0FBQyxDQUFDLEdBQUcsQ0FBQyxDQUFDLENBQUMsb0JBQW9CLENBQUMsQ0FBQyxHQUFHLEVB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Q0FBQyxDQUFDLDRCQUE0QixDQUFDLENBQUMsSUFBSSxFQUFFLENBQUM7Z0NBQ3ZDLENBQUMsQ0FBQyxnQ0FBZ0MsQ0FBQyxDQUFDLElBQUksRUFBRSxDQUFDLElBQUksRUFBRSxDQUFDOzZCQUNyRDt3QkFDTCxDQUFDLENBQUMsQ0FBQztvQkFDUCxDQUFDO29CQUNMLGdDQUFDO2dCQUFELENBQUMsQUEvRUQsSUErRUM7Z0JBL0VZLGdDQUF5Qiw0QkErRXJDLENBQUE7WUFDTCxDQUFDLEVBdkZzQyxNQUFNLEdBQU4sY0FBTSxLQUFOLGNBQU0sUUF1RjVDO1FBQUQsQ0FBQyxFQXZGOEIsT0FBTyxHQUFQLGdCQUFPLEtBQVAsZ0JBQU8sUUF1RnJDO0lBQUQsQ0FBQyxFQXZGcUIsUUFBUSxHQUFSLHVCQUFRLEtBQVIsdUJBQVEsUUF1RjdCO0FBQUQsQ0FBQyxFQXZGTSxjQUFjLEtBQWQsY0FBYyxRQXVGcEI7QUFLRCxDQUFDLENBQUMsUUFBUSxDQUFDLENBQUMsS0FBSyxDQUFDO0lBQ2QsQ0FBQyxJQUFJLGNBQWMsQ0FBQyxRQUFRLENBQUMsT0FBTyxDQUFDLE1BQU0sQ0FBQyx5QkFBeUIsRUFBRSxDQUFDLENBQUMsVUFBVSxDQUFDLFNBQVMsQ0FBQyxDQUFDO0FBQ25HLENBQUMsQ0FBQyxDQUFDIiwic291cmNlc0NvbnRlbnQiOlsibGV0IG5ld0NsaWVudFJlZ2lzdHJhdGlvblZpZXc6IEFjY3VyYXRlQXBwZW5kLldlYnNpdGVzLkNsaWVudHMuUHVibGljLk5ld0NsaWVudFJlZ2lzdHJhdGlvblZpZXc7XHJcblxyXG5tb2R1bGUgQWNjdXJhdGVBcHBlbmQuV2Vic2l0ZXMuQ2xpZW50cy5QdWJsaWMge1xyXG4gICAgZXhwb3J0IGludGVyZmFjZSBJTmV3Q2xpZW50UmVnaXN0cmF0aW9uVmlld01vZGVsIHtcclxuICAgICAgICBNb2RlbDogYW55LFxyXG4gICAgICAgIENhbmFkYTogc3RyaW5nLFxyXG4gICAgICAgIFVuaXRlZFN0YXRlczogc3RyaW5nXHJcbiAgICB9XHJcblxyXG4gICAgZXhwb3J0IGNsYXNzIE5ld0NsaWVudFJlZ2lzdHJhdGlvblZpZXcge1xyXG4gICAgICAgIFZpZXdNb2RlbDogSU5ld0NsaWVudFJlZ2lzdHJhdGlvblZpZXdNb2RlbDtcclxuXHJcbiAgICAgICAgaW5pdGlhbGl6ZSh2aWV3TW9kZWw6IElOZXdDbGllbnRSZWdpc3RyYXRpb25WaWV3TW9kZWwpIHtcclxuICAgICAgICAgICAgdGhpcy5WaWV3TW9kZWwgPSB2aWV3TW9kZWw7XHJcblxyXG4gICAgICAgICAgICAkKCcjY2tVc2VCaWxsaW5nQWRkcmVzcycpLmNoYW5nZShmdW5jdGlvbiAoKSB7XHJcbiAgICAgICAgICAgICAgICBpZiAoJCh0aGlzKS5pcyhcIjpjaGVja2VkXCIpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgJChcIiNDYXJkSG9sZGVyRmlyc3ROYW1lXCIpLnZhbCgkKFwiI0ZpcnN0TmFtZVwiKS52YWwoKSk7XHJcbiAgICAgICAgICAgICAgICAgICAgJChcIiNDYXJkSG9sZGVyTGFzdE5hbWVcIikudmFsKCQoXCIjTGFzdE5hbWVcIikudmFsKCkpO1xyXG4gICAgICAgICAgICAgICAgICAgICQoXCIjQ2FyZEFkZHJlc3NcIikudmFsKCQoXCIjQWRkcmVzc1wiKS52YWwoKSk7XHJcbiAgICAgICAgICAgICAgICAgICAgJChcIiNDYXJkQ2l0eVwiKS52YWwoJChcIiNDaXR5XCIpLnZhbCgpKTtcclxuICAgICAgICAgICAgICAgICAgICAkKFwiI0NhcmRTdGF0ZUFiYnJldmlhdGlvblwiKS52YWwoJChcIiNTdGF0ZUFiYnJldmlhdGlvblwiKS52YWwoKSk7XHJcbiAgICAgICAgICAgICAgICAgICAgJChcIiNDYXJkU3RhdGVQbGFpblRleHRcIikudmFsKCQoXCIjU3RhdGVQbGFpblRleHRcIikudmFsKCkpO1xyXG4gICAgICAgICAgICAgICAgICAgICQoXCIjQ2FyZENvdW50cnlcIikudmFsKCQoXCIjQ291bnRyeVwiKS52YWwoKSk7XHJcbiAgICAgICAgICAgICAgICAgICAgJChcIiNDYXJkUG9zdGFsQ29kZVwiKS52YWwoJChcIiNQb3N0YWxDb2RlXCIpLnZhbCgpKTtcclxuICAgICAgICAgICAgICAgICAgICAkKFwiI0NhcmRIb2xkZXJQaG9uZVwiKS52YWwoJChcIiNQaG9uZVwiKS52YWwoKSk7XHJcbiAgICAgICAgICAgICAgICAgICAgJChcIiNDYXJkTnVtYmVyXCIpLmZvY3VzKCk7XHJcbiAgICAgICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgICAgICQoXCIjQ2FyZEhvbGRlckZpcnN0TmFtZVwiKS52YWwoXCJcIik7XHJcbiAgICAgICAgICAgICAgICAgICAgJChcIiNDYXJkSG9sZGVyTGFzdE5hbWVcIikudmFsKFwiXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgICQoXCIjQ2FyZEFkZHJlc3NcIikudmFsKFwiXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgICQoXCIjQ2FyZENpdHlcIikudmFsKFwiXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgICQoXCIjQ2FyZFBvc3RhbENvZGVcIikudmFsKFwiXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgICQoXCIjQ2FyZFN0YXRlQWJicmV2aWF0aW9uXCIpLnZhbChcIlwiKTtcclxuICAgICAgICAgICAgICAgICAgICAkKFwiI0NhcmRTdGF0ZVBsYWluVGV4dFwiKS52YWwoXCJcIik7XHJcbiAgICAgICAgICAgICAgICAgICAgJChcIiNDYXJkQ291bnRyeVwiKS52YWwoXCJcIik7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH0pO1xyXG5cclxuICAgICAgICAgICAgJChcIiNsbmtUZXJtc1wiKS5jbGljayhmdW5jdGlvbiAoKSB7XHJcbiAgICAgICAgICAgICAgICAkKFwiI3Rvcy1tb2RhbFwiKS5tb2RhbCgnc2hvdycpO1xyXG4gICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgIC8vY2FyZCBjb3VudHJ5IHRvZ2dsaW5nIGJhc2VkIG9uIG5vcnRoIGFtZXJpY2Egb3Igbm90XHJcblxyXG4gICAgICAgICAgICBpZiAodmlld01vZGVsLk1vZGVsLkNhcmRDb3VudHJ5ID09IHZpZXdNb2RlbC5Vbml0ZWRTdGF0ZXMgfHwgdmlld01vZGVsLk1vZGVsLkNhcmRDb3VudHJ5ID09IHZpZXdNb2RlbC5DYW5hZGEgfHwgdmlld01vZGVsLk1vZGVsLkNhcmRDb3VudHJ5ID09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICQoXCJpbnB1dFtuYW1lPUNhcmRTdGF0ZVBsYWluVGV4dF1cIikuaGlkZSgpO1xyXG4gICAgICAgICAgICAgICAgJChcInNlbGVjdFtuYW1lPUNhcmRTdGF0ZUFiYnJldmlhdGlvbl1cIikubmV4dCgpLnNob3coKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICBlbHNlIHtcclxuICAgICAgICAgICAgICAgICQoXCJpbnB1dFtuYW1lPUNhcmRTdGF0ZVBsYWluVGV4dF1cIikuc2hvdygpO1xyXG4gICAgICAgICAgICAgICAgJChcInNlbGVjdFtuYW1lPUNhcmRTdGF0ZUFiYnJldmlhdGlvbl1cIikubmV4dCgpLmhpZGUoKTtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgJChcInNlbGVjdFtuYW1lPUNhcmRDb3VudHJ5XVwiKS5jaGFuZ2UoZnVuY3Rpb24gKCkge1xyXG4gICAgICAgICAgICAgICAgaWYgKCQodGhpcykudmFsKCkgPT0gdmlld01vZGVsLlVuaXRlZFN0YXRlcyB8fCAkKHRoaXMpLnZhbCgpID09IHZpZXdNb2RlbC5DYW5hZGEpIHtcclxuICAgICAgICAgICAgICAgICAgICAkKFwiaW5wdXRbbmFtZT1DYXJkU3RhdGVQbGFpblRleHRdXCIpLmhpZGUoKTtcclxuICAgICAgICAgICAgICAgICAgICAkKFwic2VsZWN0W25hbWU9Q2FyZFN0YXRlQWJicmV2aWF0aW9uXVwiKS52YWwoJycpO1xyXG4gICAgICAgICAgICAgICAgICAgICQoXCJzZWxlY3RbbmFtZT1DYXJkU3RhdGVBYmJyZXZpYXRpb25dXCIpLm5leHQoKS5zaG93KCk7XHJcbiAgICAgICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgICAgICQoXCJpbnB1dFtuYW1lPUNhcmRTdGF0ZVBsYWluVGV4dF1cIikuc2hvdygpO1xyXG4gICAgICAgICAgICAgICAgICAgICQoXCJzZWxlY3RbbmFtZT1DYXJkU3RhdGVBYmJyZXZpYXRpb25dXCIpLm5leHQoKS5oaWRlKCk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH0pO1xyXG5cclxuXHJcbiAgICAgICAgICAgIC8vcGVyc29uYWwgY291bnRyeSB0b2dnbGluZyBiYXNlZCBvbiBub3J0aCBhbWVyaWNhIG9yIG5vdFxyXG5cclxuICAgICAgICAgICAgaWYgKHZpZXdNb2RlbC5Nb2RlbC5Db3VudHJ5ID09IHZpZXdNb2RlbC5Vbml0ZWRTdGF0ZXMgfHwgdmlld01vZGVsLk1vZGVsLkNvdW50cnkgPT0gdmlld01vZGVsLkNhbmFkYSB8fCB2aWV3TW9kZWwuTW9kZWwuQ291bnRyeSA9PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAkKFwiaW5wdXRbbmFtZT1TdGF0ZVBsYWluVGV4dF1cIikuaGlkZSgpO1xyXG4gICAgICAgICAgICAgICAgJChcInNlbGVjdFtuYW1lPVN0YXRlQWJicmV2aWF0aW9uXVwiKS5uZXh0KCkuc2hvdygpO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgJChcImlucHV0W25hbWU9U3RhdGVQbGFpblRleHRdXCIpLnNob3coKTtcclxuICAgICAgICAgICAgICAgICQoXCJzZWxlY3RbbmFtZT1TdGF0ZUFiYnJldmlhdGlvbl1cIikubmV4dCgpLmhpZGUoKTtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgJChcInNlbGVjdFtuYW1lPUNvdW50cnldXCIpLmNoYW5nZShmdW5jdGlvbiAoKSB7XHJcbiAgICAgICAgICAgICAgICBpZiAoJCh0aGlzKS52YWwoKSA9PSB2aWV3TW9kZWwuVW5pdGVkU3RhdGVzIHx8ICQodGhpcykudmFsKCkgPT0gdmlld01vZGVsLkNhbmFkYSkge1xyXG4gICAgICAgICAgICAgICAgICAgICQoXCJpbnB1dFtuYW1lPVN0YXRlUGxhaW5UZXh0XVwiKS5oaWRlKCk7XHJcbiAgICAgICAgICAgICAgICAgICAgJChcInNlbGVjdFtuYW1lPVN0YXRlQWJicmV2aWF0aW9uXVwiKS52YWwoJycpO1xyXG4gICAgICAgICAgICAgICAgICAgICQoXCJzZWxlY3RbbmFtZT1TdGF0ZUFiYnJldmlhdGlvbl1cIikubmV4dCgpLnNob3coKTtcclxuICAgICAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICAgICAgJChcImlucHV0W25hbWU9U3RhdGVQbGFpblRleHRdXCIpLnNob3coKTtcclxuICAgICAgICAgICAgICAgICAgICAkKFwic2VsZWN0W25hbWU9U3RhdGVBYmJyZXZpYXRpb25dXCIpLm5leHQoKS5oaWRlKCk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxufVxyXG5cclxuXHJcblxyXG5cclxuJChkb2N1bWVudCkucmVhZHkoZnVuY3Rpb24gKCkge1xyXG4gICAgKG5ldyBBY2N1cmF0ZUFwcGVuZC5XZWJzaXRlcy5DbGllbnRzLlB1YmxpYy5OZXdDbGllbnRSZWdpc3RyYXRpb25WaWV3KCkpLmluaXRpYWxpemUodmlld01vZGVsKTtcclxufSk7XHJcblxyXG5cclxuXHJcblxyXG4iXX0=