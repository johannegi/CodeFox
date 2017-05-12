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
            url: '/Projects/GetReadMe',
            data: { 'ProjectID': ProjectID },
            method: "POST",
            success: function (data)
            {
                if (data == "") {
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

        $('#OpenModalDetails').modal();

        return false;
    });

    $('.LeaveProjectDropDown').on('click', function ()
    {
        $(this).addClass('LeaveSelected');
        $('#OpenModalLeave').modal('show');
    });

    $(document).on('click', '.CloseLeave', function (e) {
        e.preventDefault();
        $('.LeaveSelected').removeClass('LeaveSelected');
        $('#OpenModalLeave').modal('hide');
    });

    $("#OpenModalLeave").on('hidden.bs.modal', function () {
        $('.LeaveSelected').removeClass('LeaveSelected');
    });

    $(document).on('click', ".LeaveProject", function (e) {

        var ProjectID = $('.LeaveSelected').attr('id');
        $('.LeaveSelectedDropDown').removeClass('.LeaveSelected')
        $.ajax({
            url: '/Projects/LeaveProject',
            data: { 'ProjectID': ProjectID },
            method: "POST",
            success: function (data) {
                window.location = '/Projects';
            }
        });
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


    //Gives us time difference, we modified the code a little bit so that it
    //can read the datformat we provided, SOURCE CODE: https://coderwall.com/p/uub3pw/javascript-timeago-func-e-g-8-hours-ago
    (function timeAgo(selector) {

        var templates = {
            prefix: "",
            suffix: " ago",
            seconds: "less than a minute",
            minute: "about a minute",
            minutes: "%d minutes",
            hour: "about an hour",
            hours: "about %d hours",
            day: "a day",
            days: "%d days",
            month: "about a month",
            months: "%d months",
            year: "about a year",
            years: "%d years"
        };
        var template = function (t, n) {
            return templates[t] && templates[t].replace(/%d/i, Math.abs(Math.round(n)));
        };

        var timer = function (time) {
            if (!time)
                return;
            time = time.replace(/\.\d+/, ""); // remove milliseconds
            time = time.replace(/-/, "/").replace(/-/, "/");
            time = time.replace(/T/, " ").replace(/Z/, " GMT");
            time = time.replace(/([\+\-]\d\d)\:?(\d\d)/, " $1$2"); // -04:00 -> -0400
            time = new Date(time * 1000 || time);

            var now = new Date();
            var seconds = ((now.getTime() - time) * .001) >> 0;
            var minutes = seconds / 60;
            var hours = minutes / 60;
            var days = hours / 24;
            var years = days / 365;

            return templates.prefix + (
                    seconds < 45 && template('seconds', seconds) ||
                    seconds < 90 && template('minute', 1) ||
                    minutes < 45 && template('minutes', minutes) ||
                    minutes < 90 && template('hour', 1) ||
                    hours < 24 && template('hours', hours) ||
                    hours < 42 && template('day', 1) ||
                    days < 30 && template('days', days) ||
                    days < 45 && template('month', 1) ||
                    days < 365 && template('months', days / 30) ||
                    years < 1.5 && template('year', 1) ||
                    template('years', years)
                    ) + templates.suffix;
        };

        var elements = document.getElementsByClassName('timeago');
        for (var i in elements) {
            var $this = elements[i];
            if (typeof $this === 'object') {
                $this.innerHTML = timer($this.getAttribute('title') || $this.getAttribute('datetime'));
            }
        }
        // update time every minute
        setTimeout(timeAgo, 60000);

    })();
});