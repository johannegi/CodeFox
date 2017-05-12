$(document).ready(function () {
    $('.Delete').on('click', function (e) {
        // We get the ProjectID from the data-id attribute.
        var ProjectID = $(this).data("id");
        // Ajax request sent.
        $.ajax({
            url: '/Projects/Delete',
            data: { 'ID': ProjectID },
            method: "GET",
            success: function (data) {
                if (data == "") {
                    $('#ModalText').html('Project Not Found');
                }
                else {
                    // We build the modal and append it into the tag where .DeleteContent is.
                    html = "<h2>Delete</h2>";
                    html += "<h3>Are you sure you want to delete this project?</h3>";
                    html += "<div>";
                    html += "<h4>After deleting project can not be restored</h4>";
                    html += "<hr/>";
                    html += "<div>";


                    html += " <h2 class='DeleteTitle'>Name:</h2>";
                    html += "<p class='DeleteText'>" + "&#32;" + data.Name + "<dd>";

                    html += " <h2 class='DeleteTitle'>Type:</h2>";
                    html += "<p class='DeleteText'>" + data.Type + "<dd>";
                    html += "</div>";
                   
                    $('.DeleteContent').html('');
                    $('.DeleteContent').append(html);
                }
            }
        });
        // Then we open the modal
        $('#OpenModalDelete').modal();
        $('#DeleteForm').on('submit', function (e) {
            $.ajax({
                url: '/Projects/Delete',
                data: { 'ProjectID': ProjectID },
                method: "POST",
                success: function (data) {
                    if (data == "") {
                        $('#ModalText').html('Project Not Found');
                    }
                    else {
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
