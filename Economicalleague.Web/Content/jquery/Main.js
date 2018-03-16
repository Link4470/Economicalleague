//jQuery扩展
$.extend({
    //改写$.get方法，对Url添加时间戳，确保每次请求，数据都是最新的
    //添加没有权限的处理
    get: function (url, data, callback, type) {
        //var host = 'http://localhost:8008/';
        // shift arguments if data argument was omited
        if (jQuery.isFunction(data)) {
            type = type || callback;
            callback = data;
            data = null;
        }
        //url = host + url;
        if (url.indexOf("?") > 0) {
            url += "&Timestamp=" + this.getTime();
        } else {
            url += "?Timestamp=" + this.getTime();
        }
        return jQuery.ajax({
            headers: { 'token': $.cookie('LoginToken') },
            type: "GET",
            url: url,
            data: data,
            success: function (rdata) {
                try {
                    callback(rdata);
                } catch (e) {

                }

            },
            error: function (response, textStatus, errorThrown) {
                if (response.status == 401) {
                    window.location = '/Login/Index';
                } else {
                    // 调用外部的error
                    //error && error(response, textStatus, errorThrown);
                }
            },
            dataType: type,
        });
    },
    //改写$.post方法，添加没有权限的处理
    post: function (url, data, callback, type) {
        //var host = 'http://localhost:8008/';
        //url = host + url;
        // shift arguments if data argument was omited
        if (jQuery.isFunction(data)) {
            type = type || callback;
            callback = data;
            data = {};
        }
        return jQuery.ajax({
            headers: { 'token': $.cookie('LoginToken')},
            type: "POST",
            url: url,
            data: data,
            success: function (rdata) {
                try {
                    callback(rdata);
                }
                catch (e) {
                }
            },
            error: function (response, textStatus, errorThrown) {
                if (response.status == 401) {
                    window.location = 'Login/Index';
                } else {
                    // 调用外部的error
                    //error && error(response, textStatus, errorThrown);
                }
            },
            dataType: type
        });
    },
    //返回时间的数值表示形式
    getTime: function () {
        ///<summary>
        ///获取时间戳
        ///</summary>
        return (new Date()).valueOf();
    },
    //仿C#的String.Format方法
    format: function (source, params) {
        /// <summary>
        ///     将指定字符串中的一个或多个格式项替换为指定对象的字符串表示形式。
        /// </summary>
        /// <param name="source" type="String">
        ///     复合格式字符串
        /// </param>
        /// <param name="params" type="String">
        ///     要设置格式的对象
        /// </param>
        /// <returns type="String">format 的副本，其中的任何格式项均替换为 arg0 的字符串表示形式。</returns>
        if (arguments.length == 1)
            return function () {
                var args = $.makeArray(arguments);
                args.unshift(source);
                return $.validator.format.apply(this, args);
            };
        if (arguments.length > 2 && params.constructor != Array) {
            params = $.makeArray(arguments).slice(1);
        }
        if (params.constructor != Array) {
            params = [params];
        }
        $.each(params, function (i, n) {
            source = source.replace(new RegExp("\\{" + i + "\\}", "g"), n);
        });
        return source;
    }
});
function Substring(source, length) {
    var result = null;
    if (source == null || source.length == 0) {
        return result;
    }
    if (source.length <= length) {
        result = source;
    }
    else {
        result = source.substr(0, length) + "...";
    }
    return result;
}