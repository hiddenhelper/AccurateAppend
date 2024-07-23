var AccurateAppend;
(function (AccurateAppend) {
    var Websites;
    (function (Websites) {
        var Clients;
        (function (Clients) {
            var Shared;
            (function (Shared) {
                var MessageType;
                (function (MessageType) {
                    MessageType[MessageType["Error"] = 0] = "Error";
                    MessageType[MessageType["Info"] = 1] = "Info";
                    MessageType[MessageType["Warning"] = 2] = "Warning";
                    MessageType[MessageType["Success"] = 3] = "Success";
                })(MessageType || (MessageType = {}));
                var NotificationHelper = (function () {
                    function NotificationHelper(targetElementName) {
                        if (targetElementName.indexOf('#') != 0)
                            targetElementName = '#' + targetElementName;
                        this.targetElement = $(targetElementName);
                        this.targetElement.hide();
                    }
                    NotificationHelper.prototype.clearMessage = function () {
                        this.targetElement.hide();
                        this.targetElement.html('');
                        this.targetElement.removeClass();
                    };
                    NotificationHelper.prototype.showMessage = function (message, messageType) {
                        if (messageType === void 0) { messageType = MessageType.Info; }
                        this.targetElement.removeClass();
                        this.targetElement.addClass('alert');
                        this.targetElement.html(message);
                        this.targetElement.addClass(this.getMessageCss(messageType));
                        this.targetElement.show();
                    };
                    NotificationHelper.prototype.showError = function (message) {
                        this.showMessage(message, MessageType.Error);
                    };
                    NotificationHelper.prototype.showInfo = function (message) {
                        this.showMessage(message, MessageType.Info);
                    };
                    NotificationHelper.prototype.showWarning = function (message) {
                        this.showMessage(message, MessageType.Warning);
                    };
                    NotificationHelper.prototype.showSuccess = function (message) {
                        this.showMessage(message, MessageType.Success);
                    };
                    NotificationHelper.prototype.getMessageCss = function (messageType) {
                        switch (messageType) {
                            case MessageType.Error:
                                return 'alert-danger';
                            case MessageType.Info:
                                return 'alert-info';
                            case MessageType.Success:
                                return 'alert-success';
                            case MessageType.Warning:
                                return 'alert-warning';
                            default:
                                return 'alert-info';
                        }
                    };
                    return NotificationHelper;
                }());
                Shared.NotificationHelper = NotificationHelper;
            })(Shared = Clients.Shared || (Clients.Shared = {}));
        })(Clients = Websites.Clients || (Websites.Clients = {}));
    })(Websites = AccurateAppend.Websites || (AccurateAppend.Websites = {}));
})(AccurateAppend || (AccurateAppend = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiTm90aWZpY2F0aW9uSGVscGVyLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiTm90aWZpY2F0aW9uSGVscGVyLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLElBQU8sY0FBYyxDQTZFcEI7QUE3RUQsV0FBTyxjQUFjO0lBQUMsSUFBQSxRQUFRLENBNkU3QjtJQTdFcUIsV0FBQSxRQUFRO1FBQUMsSUFBQSxPQUFPLENBNkVyQztRQTdFOEIsV0FBQSxPQUFPO1lBQUMsSUFBQSxNQUFNLENBNkU1QztZQTdFc0MsV0FBQSxNQUFNO2dCQUd6QyxJQUFLLFdBTUo7Z0JBTkQsV0FBSyxXQUFXO29CQUVaLCtDQUFLLENBQUE7b0JBQ0wsNkNBQUksQ0FBQTtvQkFDSixtREFBTyxDQUFBO29CQUNQLG1EQUFPLENBQUE7Z0JBQ1gsQ0FBQyxFQU5JLFdBQVcsS0FBWCxXQUFXLFFBTWY7Z0JBR0Q7b0JBU0ksNEJBQVksaUJBQXlCO3dCQUNqQyxJQUFJLGlCQUFpQixDQUFDLE9BQU8sQ0FBQyxHQUFHLENBQUMsSUFBSSxDQUFDOzRCQUNuQyxpQkFBaUIsR0FBRyxHQUFHLEdBQUcsaUJBQWlCLENBQUM7d0JBRWhELElBQUksQ0FBQyxhQUFhLEdBQUcsQ0FBQyxDQUFDLGlCQUFpQixDQUFDLENBQUM7d0JBQzFDLElBQUksQ0FBQyxhQUFhLENBQUMsSUFBSSxFQUFFLENBQUM7b0JBQzlCLENBQUM7b0JBR0QseUNBQVksR0FBWjt3QkFDSSxJQUFJLENBQUMsYUFBYSxDQUFDLElBQUksRUFBRSxDQUFDO3dCQUMxQixJQUFJLENBQUMsYUFBYSxDQUFDLElBQUksQ0FBQyxFQUFFLENBQUMsQ0FBQzt3QkFDNUIsSUFBSSxDQUFDLGFBQWEsQ0FBQyxXQUFXLEVBQUUsQ0FBQztvQkFDckMsQ0FBQztvQkFHRCx3Q0FBVyxHQUFYLFVBQVksT0FBZSxFQUFFLFdBQThCO3dCQUE5Qiw0QkFBQSxFQUFBLGNBQWMsV0FBVyxDQUFDLElBQUk7d0JBRXZELElBQUksQ0FBQyxhQUFhLENBQUMsV0FBVyxFQUFFLENBQUM7d0JBQ2pDLElBQUksQ0FBQyxhQUFhLENBQUMsUUFBUSxDQUFDLE9BQU8sQ0FBQyxDQUFDO3dCQUNyQyxJQUFJLENBQUMsYUFBYSxDQUFDLElBQUksQ0FBQyxPQUFPLENBQUMsQ0FBQzt3QkFDakMsSUFBSSxDQUFDLGFBQWEsQ0FBQyxRQUFRLENBQUMsSUFBSSxDQUFDLGFBQWEsQ0FBQyxXQUFXLENBQUMsQ0FBQyxDQUFDO3dCQUM3RCxJQUFJLENBQUMsYUFBYSxDQUFDLElBQUksRUFBRSxDQUFDO29CQUM5QixDQUFDO29CQUVELHNDQUFTLEdBQVQsVUFBVSxPQUFlO3dCQUNyQixJQUFJLENBQUMsV0FBVyxDQUFDLE9BQU8sRUFBRSxXQUFXLENBQUMsS0FBSyxDQUFDLENBQUM7b0JBQ2pELENBQUM7b0JBRUQscUNBQVEsR0FBUixVQUFTLE9BQWU7d0JBQ3BCLElBQUksQ0FBQyxXQUFXLENBQUMsT0FBTyxFQUFFLFdBQVcsQ0FBQyxJQUFJLENBQUMsQ0FBQztvQkFDaEQsQ0FBQztvQkFFRCx3Q0FBVyxHQUFYLFVBQVksT0FBZTt3QkFDdkIsSUFBSSxDQUFDLFdBQVcsQ0FBQyxPQUFPLEVBQUUsV0FBVyxDQUFDLE9BQU8sQ0FBQyxDQUFDO29CQUNuRCxDQUFDO29CQUVELHdDQUFXLEdBQVgsVUFBWSxPQUFlO3dCQUN2QixJQUFJLENBQUMsV0FBVyxDQUFDLE9BQU8sRUFBRSxXQUFXLENBQUMsT0FBTyxDQUFDLENBQUM7b0JBQ25ELENBQUM7b0JBRUQsMENBQWEsR0FBYixVQUFjLFdBQXdCO3dCQUNsQyxRQUFRLFdBQVcsRUFBRTs0QkFDakIsS0FBSyxXQUFXLENBQUMsS0FBSztnQ0FDbEIsT0FBTyxjQUFjLENBQUM7NEJBQzFCLEtBQUssV0FBVyxDQUFDLElBQUk7Z0NBQ2pCLE9BQU8sWUFBWSxDQUFDOzRCQUN4QixLQUFLLFdBQVcsQ0FBQyxPQUFPO2dDQUNwQixPQUFPLGVBQWUsQ0FBQzs0QkFDM0IsS0FBSyxXQUFXLENBQUMsT0FBTztnQ0FDcEIsT0FBTyxlQUFlLENBQUM7NEJBQzNCO2dDQUNJLE9BQU8sWUFBWSxDQUFDO3lCQUMzQjtvQkFDTCxDQUFDO29CQUNMLHlCQUFDO2dCQUFELENBQUMsQUFoRUQsSUFnRUM7Z0JBaEVZLHlCQUFrQixxQkFnRTlCLENBQUE7WUFDTCxDQUFDLEVBN0VzQyxNQUFNLEdBQU4sY0FBTSxLQUFOLGNBQU0sUUE2RTVDO1FBQUQsQ0FBQyxFQTdFOEIsT0FBTyxHQUFQLGdCQUFPLEtBQVAsZ0JBQU8sUUE2RXJDO0lBQUQsQ0FBQyxFQTdFcUIsUUFBUSxHQUFSLHVCQUFRLEtBQVIsdUJBQVEsUUE2RTdCO0FBQUQsQ0FBQyxFQTdFTSxjQUFjLEtBQWQsY0FBYyxRQTZFcEIiLCJzb3VyY2VzQ29udGVudCI6WyJtb2R1bGUgQWNjdXJhdGVBcHBlbmQuV2Vic2l0ZXMuQ2xpZW50cy5TaGFyZWQge1xyXG5cclxuICAgIC8vbWVzc2FnZSB0eXBlIGVudW1lcmF0aW9uIGZvciBub3RpZmljYXRpb25cclxuICAgIGVudW0gTWVzc2FnZVR5cGVcclxuICAgIHtcclxuICAgICAgICBFcnJvcixcclxuICAgICAgICBJbmZvLFxyXG4gICAgICAgIFdhcm5pbmcsXHJcbiAgICAgICAgU3VjY2Vzc1xyXG4gICAgfVxyXG5cclxuICAgIC8vdGhpcyBjbGFzcyBhbGxvd3MgdHMgc2NyaXB0cyB0byBlbWJlZCBub3RpZmljYXRpb24gZWxlbWVudCB0byB0aGVpciBwYWdlcyBlYXNpbHlcclxuICAgIGV4cG9ydCBjbGFzcyBOb3RpZmljYXRpb25IZWxwZXJcclxuICAgIHtcclxuICAgICAgICB0YXJnZXRFbGVtZW50OiBKUXVlcnk7XHJcblxyXG4gICAgICAgIC8vVE9ETzogYSBuaWNlIHRvIGhhdmUgZmVhdHVyZSB0byBtYWtlIG5vdGlmaWNhdGlvbiBkaXNtaXNzYWJsZSBieSB1c2VycyBieSBhZGRpbmcgYSBzbWFsbCBjbG9zZSBidXR0b25cclxuICAgICAgICAvL2Rpc21pc2FibGU6IGJvb2xlYW47XHJcblxyXG4gICAgICAgIC8vaW5pdGlhbGl6ZSB1c2luZyB0aGUgZWxlbWVudCBpZFxyXG4gICAgICAgIC8vcGFzcyBlbGVtZW50IGlkIHdpdGggb3Igd2l0aG91dCB0aGUgIyBzaWduXHJcbiAgICAgICAgY29uc3RydWN0b3IodGFyZ2V0RWxlbWVudE5hbWU6IHN0cmluZykge1xyXG4gICAgICAgICAgICBpZiAodGFyZ2V0RWxlbWVudE5hbWUuaW5kZXhPZignIycpICE9IDApXHJcbiAgICAgICAgICAgICAgICB0YXJnZXRFbGVtZW50TmFtZSA9ICcjJyArIHRhcmdldEVsZW1lbnROYW1lO1xyXG5cclxuICAgICAgICAgICAgdGhpcy50YXJnZXRFbGVtZW50ID0gJCh0YXJnZXRFbGVtZW50TmFtZSk7XHJcbiAgICAgICAgICAgIHRoaXMudGFyZ2V0RWxlbWVudC5oaWRlKCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAvL2NsZWFycyB0aGUgY29udGVudCBvZiB0aGUgbm90aWZpY2F0aW9uIGFuZCBoaWRlcyB0aGUgZWxlbWVudFxyXG4gICAgICAgIGNsZWFyTWVzc2FnZSgpIHtcclxuICAgICAgICAgICAgdGhpcy50YXJnZXRFbGVtZW50LmhpZGUoKTtcclxuICAgICAgICAgICAgdGhpcy50YXJnZXRFbGVtZW50Lmh0bWwoJycpO1xyXG4gICAgICAgICAgICB0aGlzLnRhcmdldEVsZW1lbnQucmVtb3ZlQ2xhc3MoKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIC8vZGlzcGxheXMgdGhlIG1lc3NhZ2Ugc3R5bGVkIGFzIHRoZSBtZXNzYWdlIHR5cGVcclxuICAgICAgICBzaG93TWVzc2FnZShtZXNzYWdlOiBzdHJpbmcsIG1lc3NhZ2VUeXBlID0gTWVzc2FnZVR5cGUuSW5mbylcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHRoaXMudGFyZ2V0RWxlbWVudC5yZW1vdmVDbGFzcygpO1xyXG4gICAgICAgICAgICB0aGlzLnRhcmdldEVsZW1lbnQuYWRkQ2xhc3MoJ2FsZXJ0Jyk7XHJcbiAgICAgICAgICAgIHRoaXMudGFyZ2V0RWxlbWVudC5odG1sKG1lc3NhZ2UpO1xyXG4gICAgICAgICAgICB0aGlzLnRhcmdldEVsZW1lbnQuYWRkQ2xhc3ModGhpcy5nZXRNZXNzYWdlQ3NzKG1lc3NhZ2VUeXBlKSk7XHJcbiAgICAgICAgICAgIHRoaXMudGFyZ2V0RWxlbWVudC5zaG93KCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBzaG93RXJyb3IobWVzc2FnZTogc3RyaW5nKSB7XHJcbiAgICAgICAgICAgIHRoaXMuc2hvd01lc3NhZ2UobWVzc2FnZSwgTWVzc2FnZVR5cGUuRXJyb3IpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgc2hvd0luZm8obWVzc2FnZTogc3RyaW5nKSB7XHJcbiAgICAgICAgICAgIHRoaXMuc2hvd01lc3NhZ2UobWVzc2FnZSwgTWVzc2FnZVR5cGUuSW5mbyk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBzaG93V2FybmluZyhtZXNzYWdlOiBzdHJpbmcpIHtcclxuICAgICAgICAgICAgdGhpcy5zaG93TWVzc2FnZShtZXNzYWdlLCBNZXNzYWdlVHlwZS5XYXJuaW5nKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHNob3dTdWNjZXNzKG1lc3NhZ2U6IHN0cmluZykge1xyXG4gICAgICAgICAgICB0aGlzLnNob3dNZXNzYWdlKG1lc3NhZ2UsIE1lc3NhZ2VUeXBlLlN1Y2Nlc3MpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgZ2V0TWVzc2FnZUNzcyhtZXNzYWdlVHlwZTogTWVzc2FnZVR5cGUpIHtcclxuICAgICAgICAgICAgc3dpdGNoIChtZXNzYWdlVHlwZSkge1xyXG4gICAgICAgICAgICAgICAgY2FzZSBNZXNzYWdlVHlwZS5FcnJvcjpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gJ2FsZXJ0LWRhbmdlcic7XHJcbiAgICAgICAgICAgICAgICBjYXNlIE1lc3NhZ2VUeXBlLkluZm86XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuICdhbGVydC1pbmZvJztcclxuICAgICAgICAgICAgICAgIGNhc2UgTWVzc2FnZVR5cGUuU3VjY2VzczpcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gJ2FsZXJ0LXN1Y2Nlc3MnO1xyXG4gICAgICAgICAgICAgICAgY2FzZSBNZXNzYWdlVHlwZS5XYXJuaW5nOlxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiAnYWxlcnQtd2FybmluZyc7XHJcbiAgICAgICAgICAgICAgICBkZWZhdWx0OlxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiAnYWxlcnQtaW5mbyc7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9XHJcbiAgICB9XHJcbn0iXX0=