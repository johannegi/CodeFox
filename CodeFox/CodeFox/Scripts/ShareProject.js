
$(document).ready(function ()
{
    /*Ajax*/
    $("#AddUsername").keyup(function ()
    {
        var Prefix = $("#AddUsername").val();
        $.ajax({
            url: '/Editor/Autocomplete/',
            data: { 'Term': Prefix },
            method: 'POST',
            success: function (Data)
            {

                $('.ShowFound').html('');
                if (Data != "")
                {
                    var Html = "<ul>";
                    for (var i = 0; i < Data.length; i++)
                    {
                        Html += "<div name='SelectFoundUsers' class='SelectClass list-group-item col-sm-12 col-md-12 col-lg-12'>";
                        Html += Data[i].Username;
                        Html += "</div>";
                    }
                    Html += '</ul>';
                    $('.ShowFound').append(Html);
                }
                else if (Prefix == "")
                {
                    $('.ShowFound').html('');
                }
                else {
                    $('.ShowFound').html('<p>User Not found</p>');
                }
            }
        });
    });

    var ProjectID = $('.ModelProjectID').val();
    var Owner = $('.ModelOwner').val();
    var CurrentUser = $('.CurrentUser').val();

    $('#ShareSubmitButton').on('click', function ()
    {
        var AddUserHtml = document.getElementsByClassName('SelectedFoundUser');
        var AddUser = AddUserHtml[0].valueOf().innerHTML;
        $.ajax({
            url: '/Editor/Share/',
            dataType: 'html',
            data: { 'Username': AddUser, 'ProjectID': ProjectID },
            method: 'POST',
            success: function (data)
            {
                if (data == '"User is already a collaborator"')
                {
                    $('.ShowFound').html('User is already a collaborator');
                }
                else
                {
                    $('.ShowFound').html('');
                    var Html = '<tr">';
                    Html += '<td class="ShareUsername">' + AddUser + '</td>';
                    if (CurrentUser == Owner)
                    {
                        Html += ' <td><span class="RemoveShare glyphicon glyphicon-remove"></span></td>';
                    }
                    Html += '</tr>';
                    $('.ListOfCollaborators').append(Html);
                    $('.ShowFound').html('Collaborator added!');
                }
                $('.SelectedFoundUser').removeClass('SelectedFoundUser');
                document.getElementById("ShareSubmitButton").disabled = true;
                $(".ShowFound").val('');
            }
        });
        return false;
    })

    $('#CollabTable').on('click', 'span', function ()
    {
        var RemoveUserHtml = $(this).parent().prev();
        var RemoveUser = RemoveUserHtml.html();
        var CurrentRow = $(this).parent().parent();
        if (Owner == CurrentUser)
        {
            $.ajax({
                url: '/Editor/DeleteShare/',
                dataType: 'html',
                data: { 'Username': RemoveUser, 'ProjectID': ProjectID },
                method: 'POST',
                success: function (Data)
                {
                    if (Data == '"Success"')
                    {
                        CurrentRow.remove();
                    }
                    else
                    {
                        alert('Failed');
                    }
                }
            });
        }
        return false;
    })


    /***********************************************************************************/
    /*Other*/
    $('div.ShowFound').on('click', '.SelectClass', function ()
    {
        var ElementArray = document.getElementsByClassName('SelectedFoundUser');
        if ($(this).hasClass("SelectedFoundUser"))
        {
            $('.SelectedFoundUser').removeClass('SelectedFoundUser');
            document.getElementById("ShareSubmitButton").disabled = true;
        }
        else
        {
            if (ElementArray.length >= 1) {
                $('.SelectedFoundUser').removeClass('SelectedFoundUser');
            }
            $(this).addClass('SelectedFoundUser');
            document.getElementById("ShareSubmitButton").disabled = false;
        }
    });
});