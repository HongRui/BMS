﻿
function OpenSmallWindow(strUrl, strName) {
    return window.open(strUrl, strName, "width=400,height=300,top=" + (window.screen.availHeight - 55 - 300) / 2 + ",left=" + (window.screen.availWidth - 400) / 2 + ",menubar=no,toolbar=no,scrollbars=yes,status=no,titlebar=no,resizable=yes,location=no");
}


function OpenMiddleWindow(strUrl, strName) {
    return window.open(strUrl, strName, "width=640,height=480,top=" + (window.screen.availHeight - 55 - 480) / 2 + ",left=" + (window.screen.availWidth - 640) / 2 + ",menubar=no,toolbar=no,scrollbars=yes,status=no,titlebar=no,resizable=yes,location=no");
}
function OpenLargeWindow(strUrl, strName) {
    return window.open(strUrl, strName, "width=960,height=600,top=" + (window.screen.availHeight - 55 - 600) / 2 + ",left=" + (window.screen.availWidth - 800) / 2 + ",menubar=no,toolbar=no,scrollbars=yes,status=no,titlebar=no,resizable=yes,location=no");
}
function OpenFullWindow(strUrl, strName) {
    if (strName == undefined) strName =(new  Date()).getTime();
    var p = "width=" + window.screen.availWidth + ",height=" + (window.screen.availHeight - 55) + ",menubar=no,toolbar=no,scrollbars=yes,status=no,titlebar=no,resizable=yes,location=no";
    var Wins = window.open(strUrl, strName,p);
    Wins.moveTo(-3, 0);
    return Wins;
}
function OpenCustomWindow(strUrl, strName, iWidth, iHeight) {
    return window.open(strUrl, strName, "width=" + iWidth + ",height=" + iHeight + ",top=" + (window.screen.availHeight - iHeight - 55) / 2 + ",left=" + (window.screen.availWidth - iWidth) / 2 + ",menubar=no,toolbar=no,scrollbars=yes,status=no,titlebar=no,resizable=yes,location=no");
}
function OpenPrintWindow(strUrl, strName) {
    var Wins = window.open(strUrl, strName, "width=" + window.screen.availWidth + ",height=" + (window.screen.availHeight - 100) + ",menubar=yes,toolbar=no,scrollbars=yes,status=no,titlebar=no,resizable=yes,location=no");
    Wins.moveTo(-3, 0);
    return Wins;
}
function OpenNormalWindow(strUrl, strName) {
    var Wins = window.open(strUrl, strName, "width=" + window.screen.availWidth + ",height=" + (window.screen.height - 55));
    Wins.moveTo(3, 3);
    return Wins;
}
function PopWindow(strUrl, width, height) {
    if (width==undefined) {
        width = 640;
    }
    if (height == undefined) {
        height = window.screen.availHeight - 150;
    }
    window.open(strUrl, "", "width="+width+",height="+height+",top=" + (window.screen.availHeight - 55 - height) / 2 + ",left=" + (window.screen.availWidth - width) / 2 + ",menubar=no,toolbar=no,scrollbars=yes,status=no,titlebar=no,resizable=yes,location=no");
}
function closeWin() {
    if (parent) {
        parent.$.fn.colorbox.close();
    } else {
    window.close();
    }
}
function Date2String(jsondate) {
    if (jsondate == null) return "";
    jsondate = jsondate.replace("/Date(", "").replace(")/", "");
    if (jsondate.indexOf("+") > 0) {
    jsondate = jsondate.substring(0, jsondate.indexOf("+"));
    }
    else if (jsondate.indexOf("-") > 0) {
    jsondate = jsondate.substring(0, jsondate.indexOf("-"));
    }
     var date = new Date(parseInt(jsondate, 10));
     var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
     var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
     return date.getFullYear() + "-" + month + "-" + currentDate;

}
function Time2String(jsondate) {
    if (jsondate == null) return "";
    jsondate = jsondate.replace("/Date(", "").replace(")/", "");
    if (jsondate.indexOf("+") > 0) {
        jsondate = jsondate.substring(0, jsondate.indexOf("+"));
    }
    else if (jsondate.indexOf("-") > 0) {
        jsondate = jsondate.substring(0, jsondate.indexOf("-"));
    }
    var date = new Date(parseInt(jsondate));
    var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
    var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
    var hour = date.getHours()  < 10 ? "0" + date.getHours() : date.getHours();
    var minute = date.getMinutes < 10 ? "0" + date.getMinutes() : date.getMinutes();
    return date.getFullYear() + "-" + month + "-" + currentDate+ " " + hour+":"+minute;

}
function AddHead(str, list, sidx, sord) {
    for (var i = 0; i < list.length; i++) {
        var item = list[i];
        if (item.orderby == undefined) {
            str += "<th>" + item.title + "</th>";
        } else if (item.orderby == sidx) {
            if (sord == "desc") {
                str += "<th><a  href='javascript:SearchClick(\"" + sidx + "\",\"\")'>" + item.title + "<i class='icon-arrow-down'></i></th>";
            } else {
                str += "<th><a  href='javascript:SearchClick(\"" + sidx + "\",\"desc\")'>" + item.title + "<i class='icon-arrow-up'></i></th>";
            }
        } else {
            str += "<th><a href='javascript:SearchClick(\"" + item.orderby + "\",\"\")'>" + item.title + "</th>";
        }
    }
    return str;
}


//回车换为tab
var isIe = (document.all) ? true : false;
function catchKeyDown(evt) {
    evt = (evt) ? evt : ((window.event) ? window.event : "");
    var key = isIe ? evt.keyCode : evt.which;
    var el = evt.srcElement || evt.target;
    if (evt.keyCode == 13) {
        var el = evt.srcElement || evt.target;
        if (el.tagName.toLowerCase() == "input" && el.type != "submit") {
            nextCtl(el).focus();
            return false;
            if (isIe) {
                e.preventDefault();
                windows.event.keyCode = 9;
            } else {
                nextCtl(el).focus();
                //evt.preventDefault();
                return false;
            }
        }
    }
}
function nextCtl(ctl) {
    var form = ctl.form;
    for (var i = 0; i < form.elements.length - 1; i++) {
        if (ctl == form.elements[i]) {
            return form.elements[i + 1];
        }
    }
    return ctl;
}
function GetUrlArgs() {
    var args = new Object(); //声明一个空对象 
    var query = window.location.search.substring(1); // 取查询字符串，如从 http://www.snowpeak.org/testjs.htm?a1=v1&amp;a2=&amp;a3=v3#anchor 中截出 a1=v1&amp;a2=&amp;a3=v3。
    var pairs = query.split("&amp;"); // 以 &amp; 符分开成数组 
    for (var i = 0; i < pairs.length; i++) {
        var pos = pairs[i].indexOf('='); // 查找 "name=value" 对 
        if (pos == -1) continue; // 若不成对，则跳出循环继续下一对 
        var argname = pairs[i].substring(0, pos); // 取参数名 
        var value = pairs[i].substring(pos + 1); // 取参数值 
        value = decodeURIComponent(value); // 若需要，则解码 
        args[argname] = value; // 存成对象的一个属性 
    }
    return args; // 返回此对象 
}