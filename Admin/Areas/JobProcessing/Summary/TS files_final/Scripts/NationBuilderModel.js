var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var AccurateAppend;
(function (AccurateAppend) {
    var JobProcessing;
    (function (JobProcessing) {
        var Summary;
        (function (Summary) {
            var NationBuilderModel = (function (_super) {
                __extends(NationBuilderModel, _super);
                function NationBuilderModel(Email, JobId, links) {
                    return _super.call(this, Email, JobId, links) || this;
                }
                NationBuilderModel.resume = function (pushid) {
                    $.confirm({
                        text: "Are you sure you want to resume this push?",
                        confirm: function (button) {
                            console.log('calling resume for ' + pushid);
                            $.getJSON("<%= Url.BuildFor<ResumeController>().ToResume() %>?id=" +
                                pushid, function (result) {
                                console.log('resume returning status = ' +
                                    result.HttpStatusCode +
                                    ', message = ' +
                                    result.Message);
                                if (result.HttpStatusCode !== 200) {
                                    $('#nationbuilderpushes')
                                        .prepend('<div class="alert alert-danger" style="display: none; margin-bottom: 20px;">' +
                                        result.Message +
                                        '</div>');
                                }
                                else {
                                    this.renderNationBuilderInprocessGrid();
                                    this.renderNationBuilderCompleteGrid();
                                    $('#nationbuilderpushes .alert').remove();
                                }
                            });
                        },
                        cancel: function (button) {
                        },
                        confirmButton: "Yes",
                        cancelButton: "Close"
                    });
                };
                NationBuilderModel.prototype.cancel = function (pushid) {
                    $.confirm({
                        text: "Are you sure you want to cancel this push?",
                        confirm: function (button) {
                            console.log('calling confirm for ' + pushid);
                            $.getJSON("<%= Url.BuildFor<CancelController>().ToCancel() %>?id=" + pushid, function (result) {
                                console.log('confirm returning status = ' +
                                    result.HttpStatusCode +
                                    ', message = ' +
                                    result.Message);
                                if (result.HttpStatusCode !== 200) {
                                    $('#nationbuilderpushes')
                                        .prepend('<div class="alert alert-danger" style="display: none; margin-bottom: 20px;">' +
                                        result.Message +
                                        '</div>');
                                }
                                else {
                                    this.renderNationBuilderInprocessGrid();
                                    this.renderNationBuilderCompleteGrid();
                                    $('#nationbuilderpushes .alert').remove();
                                }
                            });
                        },
                        cancel: function (button) {
                        },
                        confirmButton: "Yes",
                        cancelButton: "Close"
                    });
                };
                return NationBuilderModel;
            }(AccurateAppend.JobProcessing.Summary.ParentModel));
            Summary.NationBuilderModel = NationBuilderModel;
        })(Summary = JobProcessing.Summary || (JobProcessing.Summary = {}));
    })(JobProcessing = AccurateAppend.JobProcessing || (AccurateAppend.JobProcessing = {}));
})(AccurateAppend || (AccurateAppend = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiTmF0aW9uQnVpbGRlck1vZGVsLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiTmF0aW9uQnVpbGRlck1vZGVsLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiI7Ozs7Ozs7Ozs7Ozs7QUFNQSxJQUFPLGNBQWMsQ0EwRXBCO0FBMUVELFdBQU8sY0FBYztJQUFDLElBQUEsYUFBYSxDQTBFbEM7SUExRXFCLFdBQUEsYUFBYTtRQUFDLElBQUEsT0FBTyxDQTBFMUM7UUExRW1DLFdBQUEsT0FBTztZQUN2QztnQkFBd0Msc0NBQWdEO2dCQUVwRiw0QkFBWSxLQUFLLEVBQUUsS0FBSyxFQUFFLEtBQUs7MkJBQzNCLGtCQUFNLEtBQUssRUFBRSxLQUFLLEVBQUUsS0FBSyxDQUFDO2dCQUM5QixDQUFDO2dCQUVNLHlCQUFNLEdBQWIsVUFBYyxNQUFXO29CQUNyQixDQUFDLENBQUMsT0FBTyxDQUFDO3dCQUNOLElBQUksRUFBRSw0Q0FBNEM7d0JBQ2xELE9BQU8sRUFBRSxVQUFVLE1BQU07NEJBQ3JCLE9BQU8sQ0FBQyxHQUFHLENBQUMscUJBQXFCLEdBQUcsTUFBTSxDQUFDLENBQUM7NEJBQzVDLENBQUMsQ0FBQyxPQUFPLENBQ0wsd0RBQXdEO2dDQUN4RCxNQUFNLEVBQ04sVUFBVSxNQUFNO2dDQUNaLE9BQU8sQ0FBQyxHQUFHLENBQUMsNEJBQTRCO29DQUNwQyxNQUFNLENBQUMsY0FBYztvQ0FDckIsY0FBYztvQ0FDZCxNQUFNLENBQUMsT0FBTyxDQUFDLENBQUM7Z0NBQ3BCLElBQUksTUFBTSxDQUFDLGNBQWMsS0FBSyxHQUFHLEVBQUU7b0NBQy9CLENBQUMsQ0FBQyxzQkFBc0IsQ0FBQzt5Q0FDcEIsT0FBTyxDQUNKLDhFQUE4RTt3Q0FDOUUsTUFBTSxDQUFDLE9BQU87d0NBQ2QsUUFBUSxDQUFDLENBQUM7aUNBQ3JCO3FDQUFNO29DQUNILElBQUksQ0FBQyxnQ0FBZ0MsRUFBRSxDQUFDO29DQUN4QyxJQUFJLENBQUMsK0JBQStCLEVBQUUsQ0FBQztvQ0FDdkMsQ0FBQyxDQUFDLDZCQUE2QixDQUFDLENBQUMsTUFBTSxFQUFFLENBQUM7aUNBQzdDOzRCQUNMLENBQUMsQ0FBQyxDQUFDO3dCQUNYLENBQUM7d0JBQ0QsTUFBTSxFQUFFLFVBQVUsTUFBTTt3QkFFeEIsQ0FBQzt3QkFDRCxhQUFhLEVBQUUsS0FBSzt3QkFDcEIsWUFBWSxFQUFFLE9BQU87cUJBQ3hCLENBQUMsQ0FBQztnQkFDUCxDQUFDO2dCQUVELG1DQUFNLEdBQU4sVUFBTyxNQUFXO29CQUNkLENBQUMsQ0FBQyxPQUFPLENBQUM7d0JBQ04sSUFBSSxFQUFFLDRDQUE0Qzt3QkFDbEQsT0FBTyxFQUFFLFVBQVUsTUFBTTs0QkFDckIsT0FBTyxDQUFDLEdBQUcsQ0FBQyxzQkFBc0IsR0FBRyxNQUFNLENBQUMsQ0FBQzs0QkFDN0MsQ0FBQyxDQUFDLE9BQU8sQ0FDTCx3REFBd0QsR0FBRyxNQUFNLEVBQ2pFLFVBQVUsTUFBTTtnQ0FDWixPQUFPLENBQUMsR0FBRyxDQUFDLDZCQUE2QjtvQ0FDckMsTUFBTSxDQUFDLGNBQWM7b0NBQ3JCLGNBQWM7b0NBQ2QsTUFBTSxDQUFDLE9BQU8sQ0FBQyxDQUFDO2dDQUNwQixJQUFJLE1BQU0sQ0FBQyxjQUFjLEtBQUssR0FBRyxFQUFFO29DQUMvQixDQUFDLENBQUMsc0JBQXNCLENBQUM7eUNBQ3BCLE9BQU8sQ0FDSiw4RUFBOEU7d0NBQzlFLE1BQU0sQ0FBQyxPQUFPO3dDQUNkLFFBQVEsQ0FBQyxDQUFDO2lDQUNyQjtxQ0FBTTtvQ0FDSCxJQUFJLENBQUMsZ0NBQWdDLEVBQUUsQ0FBQztvQ0FDeEMsSUFBSSxDQUFDLCtCQUErQixFQUFFLENBQUM7b0NBQ3ZDLENBQUMsQ0FBQyw2QkFBNkIsQ0FBQyxDQUFDLE1BQU0sRUFBRSxDQUFDO2lDQUM3Qzs0QkFDTCxDQUFDLENBQUMsQ0FBQzt3QkFDWCxDQUFDO3dCQUNELE1BQU0sRUFBRSxVQUFVLE1BQU07d0JBRXhCLENBQUM7d0JBQ0QsYUFBYSxFQUFFLEtBQUs7d0JBQ3BCLFlBQVksRUFBRSxPQUFPO3FCQUN4QixDQUFDLENBQUM7Z0JBQ1AsQ0FBQztnQkFDTCx5QkFBQztZQUFELENBQUMsQUF4RUQsQ0FBd0MsY0FBYyxDQUFDLGFBQWEsQ0FBQyxPQUFPLENBQUMsV0FBVyxHQXdFdkY7WUF4RVksMEJBQWtCLHFCQXdFOUIsQ0FBQTtRQUNMLENBQUMsRUExRW1DLE9BQU8sR0FBUCxxQkFBTyxLQUFQLHFCQUFPLFFBMEUxQztJQUFELENBQUMsRUExRXFCLGFBQWEsR0FBYiw0QkFBYSxLQUFiLDRCQUFhLFFBMEVsQztBQUFELENBQUMsRUExRU0sY0FBYyxLQUFkLGNBQWMsUUEwRXBCIiwic291cmNlc0NvbnRlbnQiOlsiLy8vIDxyZWZlcmVuY2UgcGF0aD1cIi4uLy4uLy4uLy4uL3NjcmlwdHMvdHlwaW5ncy9qcXVlcnkvanF1ZXJ5LmQudHNcIiAvPlxyXG4vLy8gPHJlZmVyZW5jZSBwYXRoPVwiLi4vLi4vLi4vLi4vc2NyaXB0cy90eXBpbmdzL21vbWVudC9tb21lbnQuZC50c1wiIC8+XHJcbi8vLyA8cmVmZXJlbmNlIHBhdGg9XCIuLi8uLi8uLi8uLi9zY3JpcHRzL3R5cGluZ3Mva2VuZG8tdWkva2VuZG8tdWkuZC50c1wiIC8+XHJcblxyXG4vLy8gPHJlZmVyZW5jZSBwYXRoPVwiUGFyZW50TW9kZWwudHNcIiAvPlxyXG5cclxubW9kdWxlIEFjY3VyYXRlQXBwZW5kLkpvYlByb2Nlc3NpbmcuU3VtbWFyeSB7XHJcbiAgICBleHBvcnQgY2xhc3MgTmF0aW9uQnVpbGRlck1vZGVsIGV4dGVuZHMgQWNjdXJhdGVBcHBlbmQuSm9iUHJvY2Vzc2luZy5TdW1tYXJ5LlBhcmVudE1vZGVsIHtcclxuXHJcbiAgICAgICAgY29uc3RydWN0b3IoRW1haWwsIEpvYklkLCBsaW5rcykge1xyXG4gICAgICAgICAgICBzdXBlcihFbWFpbCwgSm9iSWQsIGxpbmtzKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHN0YXRpYyByZXN1bWUocHVzaGlkOiBhbnkpIHtcclxuICAgICAgICAgICAgJC5jb25maXJtKHtcclxuICAgICAgICAgICAgICAgIHRleHQ6IFwiQXJlIHlvdSBzdXJlIHlvdSB3YW50IHRvIHJlc3VtZSB0aGlzIHB1c2g/XCIsXHJcbiAgICAgICAgICAgICAgICBjb25maXJtOiBmdW5jdGlvbiAoYnV0dG9uKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgY29uc29sZS5sb2coJ2NhbGxpbmcgcmVzdW1lIGZvciAnICsgcHVzaGlkKTtcclxuICAgICAgICAgICAgICAgICAgICAkLmdldEpTT04oXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIFwiPCU9IFVybC5CdWlsZEZvcjxSZXN1bWVDb250cm9sbGVyPigpLlRvUmVzdW1lKCkgJT4/aWQ9XCIgKyAvLyBUT0RPOiBORUVEUyBUTyBDT01FIEZST00gSk9CIFNVTU1BUllcclxuICAgICAgICAgICAgICAgICAgICAgICAgcHVzaGlkLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBmdW5jdGlvbiAocmVzdWx0KSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjb25zb2xlLmxvZygncmVzdW1lIHJldHVybmluZyBzdGF0dXMgPSAnICtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICByZXN1bHQuSHR0cFN0YXR1c0NvZGUgK1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICcsIG1lc3NhZ2UgPSAnICtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICByZXN1bHQuTWVzc2FnZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAocmVzdWx0Lkh0dHBTdGF0dXNDb2RlICE9PSAyMDApIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKCcjbmF0aW9uYnVpbGRlcnB1c2hlcycpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIC5wcmVwZW5kKFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJzxkaXYgY2xhc3M9XCJhbGVydCBhbGVydC1kYW5nZXJcIiBzdHlsZT1cImRpc3BsYXk6IG5vbmU7IG1hcmdpbi1ib3R0b206IDIwcHg7XCI+JyArXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICByZXN1bHQuTWVzc2FnZSArXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAnPC9kaXY+Jyk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMucmVuZGVyTmF0aW9uQnVpbGRlcklucHJvY2Vzc0dyaWQoKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLnJlbmRlck5hdGlvbkJ1aWxkZXJDb21wbGV0ZUdyaWQoKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKCcjbmF0aW9uYnVpbGRlcnB1c2hlcyAuYWxlcnQnKS5yZW1vdmUoKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgY2FuY2VsOiBmdW5jdGlvbiAoYnV0dG9uKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgLy8gZG8gc29tZXRoaW5nXHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgY29uZmlybUJ1dHRvbjogXCJZZXNcIixcclxuICAgICAgICAgICAgICAgIGNhbmNlbEJ1dHRvbjogXCJDbG9zZVwiXHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgY2FuY2VsKHB1c2hpZDogYW55KSB7XHJcbiAgICAgICAgICAgICQuY29uZmlybSh7XHJcbiAgICAgICAgICAgICAgICB0ZXh0OiBcIkFyZSB5b3Ugc3VyZSB5b3Ugd2FudCB0byBjYW5jZWwgdGhpcyBwdXNoP1wiLFxyXG4gICAgICAgICAgICAgICAgY29uZmlybTogZnVuY3Rpb24gKGJ1dHRvbikge1xyXG4gICAgICAgICAgICAgICAgICAgIGNvbnNvbGUubG9nKCdjYWxsaW5nIGNvbmZpcm0gZm9yICcgKyBwdXNoaWQpO1xyXG4gICAgICAgICAgICAgICAgICAgICQuZ2V0SlNPTihcclxuICAgICAgICAgICAgICAgICAgICAgICAgXCI8JT0gVXJsLkJ1aWxkRm9yPENhbmNlbENvbnRyb2xsZXI+KCkuVG9DYW5jZWwoKSAlPj9pZD1cIiArIHB1c2hpZCwgLy8gVE9ETzogTkVFRFMgVE8gQ09NRSBGUk9NIEpPQiBTVU1NQVJZXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGZ1bmN0aW9uIChyZXN1bHQpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNvbnNvbGUubG9nKCdjb25maXJtIHJldHVybmluZyBzdGF0dXMgPSAnICtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICByZXN1bHQuSHR0cFN0YXR1c0NvZGUgK1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICcsIG1lc3NhZ2UgPSAnICtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICByZXN1bHQuTWVzc2FnZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAocmVzdWx0Lkh0dHBTdGF0dXNDb2RlICE9PSAyMDApIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKCcjbmF0aW9uYnVpbGRlcnB1c2hlcycpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIC5wcmVwZW5kKFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJzxkaXYgY2xhc3M9XCJhbGVydCBhbGVydC1kYW5nZXJcIiBzdHlsZT1cImRpc3BsYXk6IG5vbmU7IG1hcmdpbi1ib3R0b206IDIwcHg7XCI+JyArXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICByZXN1bHQuTWVzc2FnZSArXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAnPC9kaXY+Jyk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMucmVuZGVyTmF0aW9uQnVpbGRlcklucHJvY2Vzc0dyaWQoKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLnJlbmRlck5hdGlvbkJ1aWxkZXJDb21wbGV0ZUdyaWQoKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKCcjbmF0aW9uYnVpbGRlcnB1c2hlcyAuYWxlcnQnKS5yZW1vdmUoKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgY2FuY2VsOiBmdW5jdGlvbiAoYnV0dG9uKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgLy8gZG8gc29tZXRoaW5nXHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgY29uZmlybUJ1dHRvbjogXCJZZXNcIixcclxuICAgICAgICAgICAgICAgIGNhbmNlbEJ1dHRvbjogXCJDbG9zZVwiXHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxufSJdfQ==