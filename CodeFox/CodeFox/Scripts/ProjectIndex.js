$(document).ready(function ()
{
    // SelectDropDown is the class on the readme link in project index page.
    $('.SelectDropDown').on('click', function (e) 
    {
        var ProjectID = $(this).data("id");
        $.ajax({
            // Fetches the readme file on specific project ID.
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
                    // Loads Ace editor.
                    var Editor = ace.edit("ModalEditor"); 
                    Editor.setTheme("ace/theme/clouds");
                    Editor.renderer.setShowGutter(false);

                    Editor.setReadOnly(true);
                    Editor.session.setMode("ace/mode/Text");
                    Editor.$blockScrolling = Infinity;
                    // Readme file content inserted into Ace editor.
                    ace.edit("ModalEditor").setValue(data.Location);            
                }
            }
        });
        // Opens the modal window with ID. 
        $('#OpenModalDetails').modal();

        return false;
    });
    // On click, add the class "LeaveSelected" and show the modal.
    $('.LeaveProjectDropDown').on('click', function ()
    {
        $(this).addClass('LeaveSelected');
        $('#OpenModalLeave').modal('show');
    });
    // We had to open and close the modal manually because we had to add and remove the class "LeaveSelected" 
    // to get the projectID.
    // Because this is in a modal we have to use this technique.
    // We remove the selected elements and hide the modal. 
    $(document).on('click', '.CloseLeave', function ()
    {
        $('.LeaveSelected').removeClass('LeaveSelected');
        $('#OpenModalLeave').modal('hide');
    });
    // When the user clicks outside the modal, we will remove the class "LeaveSelected".
    $("#OpenModalLeave").on('hidden.bs.modal', function ()
    {
        $('.LeaveSelected').removeClass('LeaveSelected');
    });
    // This is the functionality that allows the user to leave project.
    $(document).on('click', ".LeaveProject", function ()
    {
        
        var ProjectID = $('.LeaveSelected').attr('id');
        $('.LeaveSelectedDropDown').removeClass('.LeaveSelected')
        $.ajax({
            // Ajax request to leave project, handled int the ProjectsController
            url: '/Projects/LeaveProject',
            data: { 'ProjectID': ProjectID },
            method: "POST",
            success: function (data) {
                window.location = '/Projects';
            }
        });
        return false;
    });
    // This isthe functionality that allows the user to toggle the projects 
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
    (function timeAgo(selector)
    {

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