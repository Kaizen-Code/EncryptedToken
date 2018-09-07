var ajaxHelper = {
    call: function (_type, _url, _data, _dataType) {
        if (_data) {
            if (_type !== "GET") {
                _data = JSON.stringify(_data);
            };
        };
        if (!_dataType) {
            _dataType = "json";
        };
        var result = null;
        $.ajax({
            url: _url,
            data: _data,
            type: _type,
            dataType: _dataType,
            contentType: 'application/json; charset=utf-8',
            async: false,
            cache: false,
            success: function (res) {
                result = res;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ':' + jqXHR.status + ':' + errorThrown);
            }
        });
        return result;
    },
    post: function (url, data) {
        var res = ajaxHelper.call("POST", url, data)
        return res;
    },
    get: function (url, data) {
        var res = ajaxHelper.call("GET", url, data)
        return res;
    },
    getPartialView: function (url, data) {
        var res = ajaxHelper.call("GET", url, data, "html")
        return res;
    },
    parseForm: function (_formRef) {
        var serialized = _formRef.serializeArray();
        var s = '';
        var data = {};
        for (s in serialized) {
            data[serialized[s]['name']] = serialized[s]['value']
        }
        return data;
    },
   
};

var secureMvc = {
    Navigation: null,
    Content: null,
    parseForm: function (_field) {
        var formRef = $(_field).parents("form");
        return ajaxHelper.parseForm(formRef);
    },
    ShowContent: function () {
        var result = ajaxHelper.getPartialView("/Secure/Navigation");
        secureMvc.Navigation.html(result);
    },

    ShowPermissions: function () {
        var result = ajaxHelper.getPartialView("/Secure/PermissionList");
        secureMvc.Content.html(result);
    },
    permissionId: null,
    showPermissionDetail: function (id) {
        secureMvc.permissionId = id;
        var result = ajaxHelper.getPartialView("/Secure/PermissionDetail/" + id);
        secureMvc.Content.html(result);
    },
    editPermission: function (id) {
        var result = ajaxHelper.getPartialView("/Secure/EditPermission/" + id);
        secureMvc.Content.html(result);
    },
    savePermission: function (field) {
        var jsdata = secureMvc.parseForm(field)
        var result = ajaxHelper.post("/Secure/SavePermission", jsdata);
        if (result != null) {
            secureMvc.ShowPermissions();
        }
        return false;
    },
    removeUsers: function () {
        var data = {};
        data.id = secureMvc.permissionId;
        data.items = $("#AllowedItems").val();
        if (data.id == null || data.items == null) {
            return false;
        }
        var result = ajaxHelper.post("/Secure/DismissUserRoles", data);
        if (result == true) {
            secureMvc.showPermissionDetail(secureMvc.permissionId);
        }
    },
    assignUsers: function () {
        var data = {};
        data.id = secureMvc.permissionId;
        data.items = $("#NotAllowedItems").val();
        if (data.id == null || data.items == null) {
            return false;
        }
        var result = ajaxHelper.post("/Secure/AssignUserRoles", data);
        if (result == true) {
            secureMvc.showPermissionDetail(secureMvc.permissionId);
        }
    },
    
    ShowUsers: function () {
        var result = ajaxHelper.getPartialView("/Secure/UserList");
        secureMvc.Content.html(result);
    },
    userId: null,
    showUserDetail: function (id) {
        secureMvc.userId = id;
        var result = ajaxHelper.getPartialView("/Secure/UserDetail/" + id);
        secureMvc.Content.html(result);
    },
    removePermission: function () {
        var data = {};
        data.id = secureMvc.userId;
        data.items = $("#AllowedItems").val();
        if (data.id == null || data.items == null) {
            return false;
        }
        var result = ajaxHelper.post("/Secure/DeniedPermissions", data);
        if (result == true) {
            secureMvc.showUserDetail(secureMvc.userId);
        }
    },
    assignPermission: function () {
        var data = {};
        data.id = secureMvc.userId;
        data.items = $("#NotAllowedItems").val();
        if (data.id == null || data.items == null) {
            return false;
        }
        var result = ajaxHelper.post("/Secure/AssignPermissions", data);
        if (result == true) {
            secureMvc.showUserDetail(secureMvc.userId);
        }
    },




    showLoginHistory: function () {
        var result = ajaxHelper.getPartialView("/Secure/LoginHistory");
        secureMvc.Content.html(result);
    },
    showLoggers: function () {
        var result = ajaxHelper.getPartialView("/Secure/Loggers");
        secureMvc.Content.html(result);
    },
    
}

$(function () {
    secureMvc.Content = $("#SecureContent");
    secureMvc.Navigation = $("#SecureNavigation");

    secureMvc.ShowContent();

});