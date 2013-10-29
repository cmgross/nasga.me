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
});