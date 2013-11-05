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
    $("#personalRecords").click(function () {
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
    var $table = $('.table');
    var $fixedColumn = $table.clone().insertBefore($table).addClass('fixed-column');

    $fixedColumn.find('th:not(:first-child),td:not(:first-child)').remove();

    $fixedColumn.find('tr').each(function (i, elem) {
        $(this).height($table.find('tr:eq(' + i + ')').height());
    });



    //var nameSearchCache = {};
    //var nameSearchXhr;
    //if (term in nameSearchCache) {
    //    return nameSearchCache[term];
    //}
    //if (nameSearchXhr != null) {
    //    nameSearchXhr.abort();
    //}

    var cache = {};
   
    $('#Name').typeahead({
        minLength: 4,
        source: function (term, process) {
            var url = "/Search/GetNames";
            var cacheHandle = $('#Name').val().toLowerCase();
            if (typeof (cache[cacheHandle]) != "undefined") {
                console.log(cache[cacheHandle]);
                return process(cache[cacheHandle]);
            } else {
                $.ajax({
                    dataType: "json",
                    cache: true,
                    url: url,
                    data: { term: term },
                    success: function (data) {
                        cache[cacheHandle] = data;
                        console.log(data);
                        return process(data);
                    }
                });
            }
        }
    });
});
