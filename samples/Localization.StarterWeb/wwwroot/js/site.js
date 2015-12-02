(function () {
    $("#selectLanguage select").change(function () {
        $(this).parent().submit();
    });
}());