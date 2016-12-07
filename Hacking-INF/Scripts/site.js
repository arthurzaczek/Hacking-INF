var jsHelper = {};
var editor = {};
jsHelper.initHighlightJS = function () {
    $('pre code.highlight').each(function (i, block) {
        console.log("highlightBlock")
        hljs.highlightBlock(block);
    });
};

jsHelper.initEditor = function (code) {
    if (code) {
        editor = ace.edit("src-editor");
        editor.setOptions({
            enableBasicAutocompletion: true,
            enableSnippets: true,
            enableLiveAutocompletion: false
        });
        editor.setTheme("ace/theme/monokai");
        editor.getSession().setMode("ace/mode/c_cpp");
        editor.setValue(code);
    }
}

jsHelper.getCode = function () {
    return editor.getValue();
}

jsHelper.showTab = function (tab) {
    $('a[href=#' + tab + ']').tab('show');
}

jsHelper.showWaitDialog = function () {
    $("#pleaseWaitDialog").modal();
}

jsHelper.hideWaitDialog = function () {
    $("#pleaseWaitDialog").modal();
}