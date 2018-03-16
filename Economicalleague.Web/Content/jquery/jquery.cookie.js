jQuery.cookie = function(h, m, j) {
    if (typeof m != "undefined") {
        j = j || {};
        if (m === null) {
            m = "";
            j.expires = -1;
        }
        var f = "";
        if (j.expires && (typeof j.expires == "number" || j.expires.toUTCString)) {
            var d;
            if (typeof j.expires == "number") {
                d = new Date();
                d.setTime(d.getTime() + (j.expires * 24 * 60 * 60 * 1000));
            } else {
                d = j.expires;
            }
            f = "; expires=" + d.toUTCString();
        }
        var k = j.path ? "; path=" + j.path : "";
        var e = j.domain ? "; domain=" + j.domain : "";
        var l = j.secure ? "; secure" : "";
        document.cookie = [h, "=", encodeURIComponent(m), f, k, e, l].join("");
    } else {
        var c = null;
        if (document.cookie && document.cookie != "") {
            var b = document.cookie.split(";");
            for (var g = 0;
                g < b.length;
                g++) {
                var a = jQuery.trim(b[g]);
                if (a.substring(0, h.length + 1) == (h + "=")) {
                    c = decodeURIComponent(a.substring(h.length + 1));
                    break;
                }
            }
        }
        return c;
    }
};