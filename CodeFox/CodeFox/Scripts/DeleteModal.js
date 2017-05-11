$('.Delete').on('click', function (e) {

    var ProjectID = $(this).data("id");
    $.ajax({
        url: '/Projects/Delete',
        data: { 'ID': ProjectID },
        method: "GET",
        success: function (data) {
            if (data == "") {
                $('#ModalText').html('Project Not Found');
            }
            else {
                html = "<h2>Delete</h2>";
                html += "<h3>Are you sure you want to delete this project?</h3>";
                html += "<div>";
                html += "<h4>After deleting project can not be restored</h4>";
                html += "<hr/>";
                html += "<div>";


                html += " <h2 class='DeleteTitle'>Name:</h2>";
                html += "<p class='DeleteText'>" + "&#32;" + data.Name + "<dd>";

                html += " <h2 class='DeleteTitle'>Type:     </h2>";
                html += "<p class='DeleteText'>    " + data.Type + "<dd>";
                html += "</div>";

                $('.DeleteContent').html('');
                $('.DeleteContent').append(html);
            }
        }
    });

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
                    window.location = '/Projects';
                }
            }
        });
        return false;
    });

    return false;
});