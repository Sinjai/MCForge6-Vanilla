$(function() {

    if(public_checked === "true") { $('#is-public').prop('checked', true); }
    if(loadalllevels_checked === "true") { $('#loadalllevels').prop('checked', true); }
    if(verifynames_checked === "true") { $('#verifynames').prop('checked', true); }
    if(verifying_checked === "true") { $('#verifying').prop('checked', true); }
    if(agreeingtorules_checked === "true") { $('#agreeingtorules').prop('checked', true); }
    if(irc_enabled_checked === "true") { $('#irc-enabled').prop('checked', true); }
    if(gc_enabled_checked === "true") { $('#gc-enabled').prop('checked', true); }

    $('[data-show-module]').click(function() {
        $("#main").hide();
        $("#" + $(this).attr("data-show-module") + '-module').show();
        $('#module-title').text($(this).attr("data-module-title")).show();
    });
    $('[data-show-all]').click(function() {
        $("#main").show();
        $(".module").hide();
        $('#module-title').text("").hide();
    });


    $('[data-group-list]').each(function() {
        var type = $(this).attr('data-group-list');

        if(type === 'option') {
            var value = $(this).attr('data-option-value');
            var current = $(this).attr('data-current-value');

            for(var i = 0; i < GROUPS.length; i++) {
                var val = "";
                var selected = "";

                if(value === 'name') {
                    val = GROUPS[i].name.toLowerCase();

                    if(val === current) {
                        selected = " selected";
                    }
                }

                $(this).append("<option value=\"" + val + "\"" + selected + ">" + GROUPS[i].name + "</option>");
            }
        }
    });

    $('#save-settings').click(function() {
        $("#form").submit();
    });
});


