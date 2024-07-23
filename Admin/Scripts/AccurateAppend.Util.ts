/// <reference path="typings/moment/moment.d.ts" />
/// <reference path="typings/jquery/jquery.d.ts" />

module AccurateAppend {

    export module Util {

        export function newGuid() {

            var d = new Date().getTime();
            var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
                var r = (d + Math.random() * 16) % 16 | 0;
                d = Math.floor(d / 16);
                return (c == 'x' ? r : (r & 0x7 | 0x8)).toString(16);
            });
            return uuid;

        }

        export function getUrlParam(name) {
            var results = new RegExp('[\?&]' + name + '=([^&#]*)', 'i').exec(window.location.href);
            if (results == null) {
                return null;
            }
            else {
                return decodeURIComponent(results[1]) || 0;
            }
        }
        

    }

}