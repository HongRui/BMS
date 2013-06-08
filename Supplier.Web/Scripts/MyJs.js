
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
    var Wins = window.open(strUrl, strName, "width=" + window.screen.availWidth + ",height=" + (window.screen.availHeight - 55) + ",menubar=no,toolbar=no,scrollbars=yes,status=no,titlebar=no,resizable=yes,location=no");
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
function closeWin() {
    if (parent) {
        parent.$.fn.colorbox.close();
    } else {
    window.close();
    }
}
function ExportExcel(grid) {
    var url = grid.jqGrid('getGridParam', 'url');
    var postdata = grid.jqGrid('getGridParam', 'postData');
    var exportform = $("#exportForm");
    if (exportform.length == 0) {
        $('body').append("<div style='display:none'><form method='post' id='exportForm' action='" + url + "'></form></div>");
        exportform = $("#exportForm");
    }
    exportform.empty();
    exportform.append("<input name='toexcel' value='true'/>");
    $.each(postdata, function (n, value) { exportform.append("<input name='" + n + "' value='" + value + "'/>"); });
    exportform.submit();
}