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
    $("#searchForm").submit(function () {
        if (!$("#searchForm").valid()) return false;
        $.blockUI({
            message: "Searching",
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

    //var cache = {};
   
    $('#Name').typeahead({
        minLength: 4,
        source: function (term, process) {
            var url = "/Search/GetNames";
            var year = $('#Year').val();
            var athleteClass = $('#AthleteClass').val();
            //var cacheHandle = $('#Name').val().toLowerCase();
            //if (typeof (cache[cacheHandle]) != "undefined") {
            //    console.log(cache[cacheHandle]);
            //    return process(cache[cacheHandle]);
            //} else {
                $.ajax({
                    dataType: "json",
                    cache: true,
                    url: url,
                    data: { term: term, year: year, athleteClass: athleteClass },
                    beforeSend: function () {
                        $.blockUI({
                            message: "Loading",
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
                    },
                    success: function (data) {
                        //cache[cacheHandle] = data;
                        //console.log(data);
                        return process(data);
                    },
                    complete: function () {
                        $.unblockUI();
                    }
                });
            //}
        }
    });
});
