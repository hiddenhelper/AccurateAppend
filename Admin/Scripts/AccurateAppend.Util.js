var AccurateAppend;
(function (AccurateAppend) {
    var Util;
    (function (Util) {
        function newGuid() {
            var d = new Date().getTime();
            var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = (d + Math.random() * 16) % 16 | 0;
                d = Math.floor(d / 16);
                return (c == 'x' ? r : (r & 0x7 | 0x8)).toString(16);
            });
            return uuid;
        }
        Util.newGuid = newGuid;
        function getUrlParam(name) {
            var results = new RegExp('[\?&]' + name + '=([^&#]*)', 'i').exec(window.location.href);
            if (results == null) {
                return null;
            }
            else {
                return decodeURIComponent(results[1]) || 0;
            }
        }
        Util.getUrlParam = getUrlParam;
    })(Util = AccurateAppend.Util || (AccurateAppend.Util = {}));
})(AccurateAppend || (AccurateAppend = {}));
//# sourceMappingURL=AccurateAppend.Util.js.map