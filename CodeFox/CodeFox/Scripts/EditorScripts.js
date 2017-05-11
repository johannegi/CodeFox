var ProjectID = Number($('.ProjectID').val());
var ReadMeID = Number($('.ReadMeID').val());
var CurrentUser = $('.CurrentUser').val();
var TreeIcon = $('.TreeIcon').val();

//Load ace editor
var Editor = ace.edit("Editor");
Editor.setTheme("ace/theme/twilight");
Editor.session.setMode("ace/mode/Text");
Editor.$blockScrolling = Infinity;

//start Codehub signalR
var CodeHub = $.connection.codeHub;
$.connection.hub.start();
var Silent = false;
var FileID = ReadMeID;

//When another user makes a change
CodeHub.client.onChange = function (ChangeData, Username) {
    Silent = true;
    Editor.getSession().getDocument().applyDelta(ChangeData);
    if (ChangeData.action == 'insert')
    {
        if (ChangeData.end.column == 0)
        {
            $('#EditorInfo').text(Username + ' added new line ' + (ChangeData.end.row + 1));
        }
        else
        {
            $('#EditorInfo').text(Username + ' inserted at line ' + (ChangeData.end.row + 1));
        }
    }
    if (ChangeData.action == 'remove') {
        $('#EditorInfo').text(Username + ' removed at line ' + (ChangeData.end.row + 1));
    }

    Silent = false;
}

//Sends changes with signalR to other users
$.connection.hub.start().done(function () {
    CodeHub.server.joinFile(FileID);
    ace.edit("Editor").on('change', function (Obj) {
        if (Silent) {
            return;
        }
        CodeHub.server.onChange(Obj, FileID, CurrentUser);
    });
});

//Save every 500ms after user stops writing
var TypingTimer;                //timer identifier
var DoneTypingInterval = 500;  //time in ms

//on keyup, start the countdown
$('#Editor').on("keyup", function (Obj) {
    if (TypingTimer) clearTimeout(TypingTimer);
    TypingTimer = setTimeout(DoneTyping, DoneTypingInterval);
});

//on keydown, clear the countdown
$('#Editor').on("keydown", function () {
    clearTimeout(TypingTimer);
    var Selected = $('#Tree').jstree(true).get_selected('full', true)
    Selected = Selected[0]
    if (Selected.type == 'file' || Selected.type == 'ReadMe') {
        $('#EditorInfo').text('File will save when you stop editing');
    }
});

//user is "finished typing," do something
function DoneTyping() {
    var Selected = $('#Tree').jstree(true).get_selected('full', true)
    Selected = Selected[0]
    var FileID = Number(Selected.id);
    var NewText = String(ace.edit("Editor").session.getValue());
    if (Selected.type == 'file' || Selected.type == 'ReadMe') {
        $.ajax(
            {
                url: '/Editor/SaveFile/',
                data: { 'ProjectID': ProjectID, 'FileID': Number(Selected.id), 'NewText': NewText },
                method: 'POST',
                success: function (ReturnData) {
                    $('#EditorInfo').text('File saved');
                }
            });
    }
}

var TreeData;
$(document).ready(function () {
    $.ajax(
{
    url: '/Editor/GetTreeJson/',
    data: { 'ProjectID': ProjectID },
    method: 'POST',
    success: function (ReturnData) {
        TreeData = ReturnData;
        //Load JsTree
        $('#Tree').jstree({
            "core": {
                "animation": 0,
                "check_callback": true,
                "data": TreeData
            },
            "types": {
                "#": {
                    "valid_children": ["root"]
                },
                "root": {
                    "icon": TreeIcon,
                    "valid_children": ["default", "file", "ReadMe"]
                },
                "default": {
                    "valid_children": ["default", "file"]
                },
                "file": {
                    "icon": "glyphicon glyphicon-file",
                    "valid_children": []
                },
                "ReadMe": {
                    "icon": "glyphicon glyphicon-file",
                    "valid_children": []
                }
            },
            "plugins": ["contextmenu", "dnd", "types"],
            "contextmenu": {
                "items": function (Node) {
                    return {
                        "Rename": {
                            "separator_before": false,
                            "separator_after": false,
                            "label": "Rename",
                            //Right click on node to rename function
                            "action": function () {
                                var NewName = prompt("Please enter new name", "");
                                if (NewName != null && NewName != "") {
                                    //Rename File
                                    if (Node.type == 'file') {
                                        $.ajax(
                                        {
                                            url: '/Editor/ChangeFileName/',
                                            data: { 'ProjectID': ProjectID, 'FileID': Number(Node.id), 'NewName': NewName },
                                            method: 'POST',
                                            success: function (ReturnData) {
                                                $('#EditorInfo').text(Node.text + ' Renamed to ' + ReturnData.Name + '.' + ReturnData.Type);
                                                $("#Tree").jstree('set_text', Node, (ReturnData.Name + '.' + ReturnData.Type));
                                                ace.edit("Editor").setValue(ReturnData.Location);
                                                var Modelist = ace.require("ace/ext/modelist")
                                                var FilePath = ReturnData.Name + '.' + ReturnData.Type;
                                                var Mode = Modelist.getModeForPath(FilePath).mode;
                                                ace.edit("Editor").session.setMode(Mode);
                                            }
                                        });
                                    }
                                        //Rename Folder
                                    else if (Node.type != 'ReadMe') {
                                        $.ajax(
                                        {
                                            url: '/Editor/ChangeFolderName/',
                                            data: { 'ProjectID': ProjectID, 'FolderID': Number(Node.id), 'NewName': NewName },
                                            method: 'POST',
                                            success: function (ReturnData) {
                                                $('#EditorInfo').text(Node.text + ' Renamed to ' + ReturnData.Name);
                                                $("#Tree").jstree('set_text', Node, (ReturnData.Name));
                                            }
                                        });
                                    }
                                }
                            }
                        },
                        //Right click on node to remove function
                        "Remove": {
                            "separator_before": false,
                            "separator_after": false,
                            "label": "Remove",
                            "action": function () {
                                if (Node.id == ReadMeID) {
                                    alert("You can't delete the read me file")
                                }
                                else if (Node.id == 'Project') {
                                    alert("You can't delete the project root")
                                }
                                else {
                                    //Delete File
                                    if (Node.type == 'file' && confirm("Are you sure you want to delete " + Node.text + " ?") == true) {
                                        $.ajax(
                                            {
                                                url: '/Editor/DeleteFile/',
                                                data: { 'FileID': Number(Node.id), 'ProjectID': ProjectID },
                                                method: 'POST',
                                                success: function (ReturnData) {
                                                    $('#EditorInfo').text(Node.text + ' deleted');
                                                    $("#Tree").jstree('delete_node', Node);
                                                }
                                            });
                                    }
                                        //Delete Folder
                                    else if (confirm("Are you sure you want to delete " + Node.text + " and every file inside it ?") == true) {
                                        $.ajax(
                                            {
                                                url: '/Editor/DeleteFolder/',
                                                data: { 'FolderID': Number(Node.id), 'ProjectID': ProjectID },
                                                method: 'POST',
                                                success: function (ReturnData) {
                                                    $('#EditorInfo').text(Node.text + ' deleted');
                                                    $("#Tree").jstree('delete_node', Node);
                                                }
                                            });
                                    }
                                }
                            }
                        }
                    };
                }
            }
        });
    }
});
});



var Parent = 0;
var newParent = 0;
var Pos = 0;
var newPos = 0;

//When JsTree is loaded
$("#Tree").on("loaded.jstree", function () {
    $('#Tree').jstree(true).select_node(String(ReadMeID));
    $("#Tree").on("select_node.jstree", function (Event, Node) {
        //When node is selected
        var Selected = $('#Tree').jstree(true).get_selected('full', true)
        Selected = Selected[0];
        CodeHub.server.leaveFile(FileID);
        FileID = Selected.id;
        CodeHub.server.joinFile(FileID);

        //Open new file when selected
        if (Selected.type == 'file' || Selected.type == 'ReadMe') {
            ace.edit("Editor").setReadOnly(false);
            $.ajax(
                {
                    url: '/Editor/OpenNewFile/',
                    data: { 'FileID': Number(Selected.id) },
                    method: 'POST',
                    success: function (ReturnData) {
                        Silent = true;
                        ace.edit("Editor").setValue(ReturnData.Location);
                        var Modelist = ace.require("ace/ext/modelist")
                        var FilePath = ReturnData.Name + '.' + ReturnData.Type;
                        var Mode = Modelist.getModeForPath(FilePath).mode;
                        ace.edit("Editor").session.setMode(Mode);
                        Silent = false;
                    }
                });
        }
        else {
            ace.edit("Editor").setValue('');
            ace.edit("Editor").setReadOnly(true);
        }

    });
    //When node is dragged and moved
    $("#Tree").bind('move_node.jstree', function (Event, Data) {
        if (Data.parent != Data.old_parent) {
            //Move File
            if (Data.node.type == 'file') {
                var NewFolder = null;
                if (Data.parent != 'Project') {
                    NewFolder = Number(Data.parent)
                }
                $.ajax(
                {
                    url: '/Editor/MoveFile/',
                    data: { 'ProjectID': ProjectID, 'FileID': Number(Data.node.id), 'NewFolderID': NewFolder },
                    method: 'POST',
                    success: function () {
                        var NewParent = $('#Tree').jstree(true).get_node(Data.parent)
                        $('#EditorInfo').text(Data.node.text + ' Moved to folder ' + NewParent.text);
                    },
                });
            }
                //Move Folder
            else if (Data.node.type != 'ReadMe') {
                var NewFolder = null;
                if (Data.parent != 'Project') {
                    NewFolder = Number(Data.parent)
                }
                $.ajax(
                {
                    url: '/Editor/MoveFolder/',
                    data: { 'ProjectID': ProjectID, 'FolderID': Number(Data.node.id), 'NewFolderID': NewFolder },
                    method: 'POST',
                    success: function () {
                        var NewParent = $('#Tree').jstree(true).get_node(Data.parent)
                        $('#EditorInfo').text(Data.node.text + ' Moved to folder ' + NewParent.text);
                    }
                });
            }
        }
    });
});