//var refreshToken = {
    
//    timeKey: "kaizen-TimeKey",
//    getTokenTime: function () {
//        var t = localStorage.getItem(refreshToken.timeKey);
//        if (t != null)
//            return new Date(t);
//        return null;
//    },
//    setTokenTime: function (secs) {
//        var d = new Date();
//        d.setSeconds(d.getSeconds() + secs);
//        localStorage.setItem(refreshToken.timeKey, d);
//    },
//    refreshToken: function () {
//        console.log('refresh fn');
//        var last = new Date(refreshToken.lastAjaxTime);
//        last.setSeconds(last.getSeconds() + 10);
//        if (last < new Date()) {
//            console.log('your session is expired....');
//        }

//        var t = refreshToken.getTokenTime();
//        t.setSeconds(t.getSeconds() + 60);
//        if (t < new Date()) {
//            clearInterval(refreshToken.timer)
//            refreshToken.removeToken();
//            return;
//        }
//        if (refreshToken.getTokenTime() < new Date()) {
//            refreshToken.setTokenTime(660);
//            var res = refreshToken.get("/api/Secure/RefreshToken");
//            //console.log('refershTokenResponse:' + res);
//            if (res != null && res.split(".").length === 3) {
//                refreshToken.saveToken(res);
//            }
//        }

//    },
//    call: function (_type, _url, _data, _dataType) {
//        if (_data) {
//            if (_type !== "GET") {
//                _data = JSON.stringify(_data);
//            };
//        };
//        if (!_dataType) {
//            _dataType = "json";
//        };
//        var result = null;
//        $.ajax({
//            url: _url,
//            data: _data,
//            type: _type,
//            dataType: _dataType,
//            contentType: 'application/json; charset=utf-8',
//            async: false,
//            cache: false,
//            success: function (res) {
//                result = res;
//            },
//            error: function (jqXHR, textStatus, errorThrown) {
//                alert(textStatus + ':' + jqXHR.status + ':' + errorThrown);
//            }
//        });
//        return result;
//    },
//    post: function (url, data) {
//        var res = refreshToken.call("POST", url, data)
//        return res;
//    },
//    get: function (url, data) {
//        var res = refreshToken.call("GET", url, data)
//        return res;
//    },
//    timer: null,
//    startup: function () {
//        if (refreshToken.getTokenTime() != null) {
//            refreshToken.timer = setInterval(function () { refreshToken.refreshToken(); }, 5000);
//        }
//    },
//    lastAjaxTime: new Date(),
//};

//$(function () {
//    //refreshToken.startup();
//    //$(document).bind("ajaxSend", function () {
//    //    refreshToken.lastAjaxTime = new Date();
//    //    console.log('ajaxSend');
//    //}).bind("ajaxComplete", function () {
//    //    console.log('ajaxComplete');
//    //});
//});
