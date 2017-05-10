﻿
$(document).ready(function ()
{
    /**************************************************************SHARE**************************************************************/
    /*Ajax*/
    $("#AddUsername").keyup(function ()
    {
        var form = $(this);
        var prefix = $("#AddUsername").val();
        $.ajax({
            url: '/Editor/Autocomplete/',
            data: { 'term': prefix },
            method: 'POST',
            success: function (data)
            {

                $('.ShowFound').html('');
                if (data != "")
                {
                    var html = "<ul>";
                    for (var i = 0; i < data.length; i++)
                    {
                        html += "<div name='SelectFoundUsers' class='SelectClass list-group-item col-sm-12 col-md-12 col-lg-12'>";
                        html += data[i].Username;
                        html += "</div>";
                    }
                    html += '</ul>';
                    $('.ShowFound').append(html);
                }
                else if (prefix == "")
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

    $('#ShareForm').on('submit', function ()
    {
        var AddUserHtml = document.getElementsByClassName('SelectedFoundUser');
        var AddUser = AddUserHtml[0].valueOf().innerHTML;
        $.ajax({
            url: '/Editor/Share/',
            dataType: 'html',
            data: AddAntiForgeryToken({ 'Username': AddUser, 'ProjectID': ProjectID }),
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
                    var html = '<tr">';
                    html += '<td class="ShareUsername">' + AddUser + '</td>';
                    if (CurrentUser == Owner)
                    {
                        html += ' <td><span class="RemoveShare glyphicon glyphicon-remove"></span></td>';
                    }
                    html += '</tr>';
                    $('.ListOfCollaborators').append(html);
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
                data: AddAntiForgeryToken({ 'Username': RemoveUser, 'ProjectID': ProjectID }),
                method: 'POST',
                success: function (data)
                {
                    if (data == '"Success"')
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

    var first = true;
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
    // Link: http://stackoverflow.com/questions/4074199/jquery-ajax-calls-and-the-html-antiforgerytoken
    // We found a way to pass parameters to function that validate AntiForgeryTokens. 
    AddAntiForgeryToken = function (data)
    {
        data.__RequestVerificationToken = $('#ShareForm input[name=__RequestVerificationToken]').val();
        return data;
    };

    /**************************************************************REGISTER**************************************************************/

    $('#RegisterForm').on('submit', function ()
    {
        var form = $(this);
        $('#DuplicateUsernameError').html('');
        $.ajax({
            url: '/Account/Register/',
            data: form.serialize(),
            method: 'POST',
            success: function (data)
            {
                if (data == "success")
                {
                    window.location.href = 'Projects';
                }
                else if(!data.Succeded)
                {
                    var message = '<li class="text-danger">Please fill out all the input fields</li>';
                    $('#DuplicateUsernameError').append(message);
                }
                else
                {
                    var html = "";
                    for (var i = 0; i < data.Errors.length; i++) {
                        html += "<li class='text-danger'>";
                        html += data.Errors[i];
                        html += "</li>";
                    }
                    $('#DuplicateUsernameError').append(html);
                }
            }
        });
        return false;
    });
});