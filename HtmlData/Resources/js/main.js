$(function() {

    if(public_checked.toLowerCase() === "true") { $('#is-public').prop('checked', true); }
    if(loadalllevels_checked.toLowerCase() === "true") { $('#loadalllevels').prop('checked', true); }
    if(verifynames_checked.toLowerCase() === "true") { $('#verifynames').prop('checked', true); }
    if(verifying_checked.toLowerCase() === "true") { $('#verifying').prop('checked', true); }
    if(agreeingtorules_checked.toLowerCase() === "true") { $('#agreeingtorules').prop('checked', true); }
    if(irc_enabled_checked.toLowerCase() === "true") { $('#irc-enabled').prop('checked', true); }
    if(gc_enabled_checked.toLowerCase() === "true") { $('#gc-enabled').prop('checked', true); }
    if(usingconsole_checked.toLowerCase() === "true") { $('#usingconsole').prop('checked', true); }
    if(check_for_core_updates_checked.toLowerCase() === "true") { $('#check-for-core-updates').prop('checked', true); }
    if(check_misc_updates_checked.toLowerCase() === "true") { $('#check-misc-updates').prop('checked', true); }
    if(allow_patch_updates_checked.toLowerCase() === "true") { $('#allow-patch-updates').prop('checked', true); }
    if(allow_update_checked.toLowerCase() === "true") { $('#allow-update').prop('checked', true); }
    if(silent_update_checked.toLowerCase() === "true") { $('#silent-update').prop('checked', true); }
    if(ask_before_core_checked.toLowerCase() === "true") { $('#ask-before-core').prop('checked', true); }
    if(ask_before_misc_checked.toLowerCase() === "true") { $('#ask-before-misc').prop('checked', true); }
    if(silent_core_update_checked.toLowerCase() === "true") { $('#silent-core-update').prop('checked', true); }
    if(mysql_pooling_checked.toLowerCase() === "true") { $('#mysql-pooling').prop('checked', true); }
    if(sqlite_pooling_checked.toLowerCase() === "true") { $('#sqlite-pooling').prop('checked', true); }
    if(sqlite_inmemory_checked.toLowerCase() === "true") { $('#sqlite-inmemory').prop('checked', true); }
    if(database_queuing_checked.toLowerCase() === "true") { $('#database-queuing').prop('checked', true); }

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

    var $dbselect = $('[data-db-select]');

    $dbselect.find("option[value='" + $dbselect.attr('data-current-value') + "']").prop("selected", true);

    $("#database-" + $dbselect.attr('data-current-value')).show();

    $dbselect.change(function() {
        $('.db-item').hide();
        $("#database-" + $(this).val()).show();
    });
});


