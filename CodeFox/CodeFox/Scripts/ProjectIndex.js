$(document).ready(function ()
{
    $("#Search").keyup(function () {
        var prefix = $("#Search").val();
        $.ajax({
            url: '/Projects/Search/',
            data: { 'Term': prefix },
            method: 'POST',
            success: function (data) {

                $('.SearchResult').html('');
                if (data != "") {
                    var html = "<ul>";
                    for (var i = 0; i < data.length; i++) {
                        html += "<div name='SelectFoundUsers' class='SelectClass list-group-item col-sm-12 col-md-12 col-lg-12'>";
                        html += data[i].Username;
                        html += "</div>";
                    }
                    html += '</ul>';
                    $('.SearchResult').append(html);
                }
                else if (prefix == "") {
                    $('.SearchResult').html('');
                }
                else {
                    $('.SearchResult').html('<p>User Not found</p>');
                }
            }
        });
    });

    $('.SelectDropDown').on('click', function (e)
    {

        var ProjectID = $(this).data("id");
        $.ajax({
            url: '/Projects/GetProject',
            data: { 'ProjectID': ProjectID },
            method: "POST",
            success: function (data)
            {
                if (data == "")
                {
                    $('#ModalText').html('Project Not Found');
                }
                else
                {
                    var Editor = ace.edit("ModalEditor");
                    Editor.setTheme("ace/theme/clouds");
                    Editor.renderer.setShowGutter(false);

                    Editor.setReadOnly(true);
                    Editor.session.setMode("ace/mode/Text");
                    Editor.$blockScrolling = Infinity;

                    ace.edit("ModalEditor").setValue(data.Location);                  
                }
            }
        });

        $('#OpenModal').modal();

        return false;
    });

    $('.ProjectArrow').on('click', function ()
    {
        var OwnedProjects = $(this).attr('id');
        if (OwnedProjects == "OwnedProjectIndex")
        {
            $(".OwnProjects").toggle();
            $(this).toggleClass('fa-caret-down');
            $(this).toggleClass('fa-caret-right');
        }
        else
        {
            $('.SharedProjects').toggle();
            $(this).toggleClass('fa-caret-down');
            $(this).toggleClass('fa-caret-right');
        }
    });


    //HELPS US TO SEE WHEN MODIFIED, SOURCE CODE: https://coderwall.com/p/uub3pw/javascript-timeago-func-e-g-8-hours-ago

});