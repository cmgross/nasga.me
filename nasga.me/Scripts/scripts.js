$(document).ready(function () {
    $("#profileForm").submit(function () {
        if (!$("#profileForm").valid()) return false;
        $.blockUI({
            message: "Saving profile",
            css: {
                border: 'none',
                padding: '15px',
                backgroundColor: '#000',
                '-webkit-border-radius': '10px',
                '-moz-border-radius': '10px',
                opacity: .5,
                color: '#fff'
            }
        });
    });
    $("#personalRecords").click(function() {
        $.blockUI({
            message: "Loading Records",
            css: {
                border: 'none',
                padding: '15px',
                backgroundColor: '#000',
                '-webkit-border-radius': '10px',
                '-moz-border-radius': '10px',
                opacity: .5,
                color: '#fff'
            }
        });
    });
    //var $table = $('.table');
    //var $fixedColumn = $table.clone().insertBefore($table).addClass('fixed-column');

    //$fixedColumn.find('th:not(:first-child),td:not(:first-child)').remove();

    //$fixedColumn.find('tr').each(function (i, elem) {
    //    $(this).height($table.find('tr:eq(' + i + ')').height());
    //});
});
