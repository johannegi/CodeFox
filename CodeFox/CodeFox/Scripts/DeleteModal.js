$(document).ready(function ()
{
    $('.Delete').on('click', function ()
    {
        // We get the ProjectID from the data-id attribute.
        var ProjectID = $(this).data("id");
        // Ajax request sent.
        $.ajax({
            url: '/Projects/Delete',
            data: { 'ID': ProjectID },
            method: "GET",
            success: function (Data)
            {
                if (Data == "")
                {
                    $('#ModalText').html('Project Not Found');
                }
                else
                {
                    // We build the modal and append it into the tag where .DeleteContent is.
                    Html = "<h2>Delete</h2>";
                    Html += "<h3>Are you sure you want to delete this project?</h3>";
                    Html += "<div>";
                    Html += "<h4>After deleting project can not be restored</h4>";
                    Html += "<hr/>";
                    Html += "<div>";


                    Html += " <h2 class='DeleteTitle'>Name:</h2>";
                    Html += "<p class='DeleteText'>" + "&#32;" + Data.Name + "<dd>";

                    Html += " <h2 class='DeleteTitle'>Type:</h2>";
                    Html += "<p class='DeleteText'>" + Data.Type + "<dd>";
                    Html += "</div>";
                   
                    $('.DeleteContent').html('');
                    $('.DeleteContent').append(Html);
                }
            }
        });
        // Then we open the modal
        $('#OpenModalDelete').modal();
        $('.DeleteProject').on('click', function ()
        {
            $.ajax({
                url: '/Projects/Delete',
                data: { 'ProjectID': ProjectID },
                method: "POST",
                success: function (Data)
                {
                    if (Data == "")
                    {
                        $('#ModalText').html('Project Not Found');
                    }
                    else
                    {
                        // We redirect the user, to refresh the site so that 
                        // the projects update.
                        window.location = '/Projects';
                    }
                }
            });
            return false;
        });

        return false;
    });
});
