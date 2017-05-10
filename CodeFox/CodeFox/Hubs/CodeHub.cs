using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace CodeFox.Hubs
{
    public class CodeHub : Hub
    {
        public void LeaveFile(int FileID)
        {
            Groups.Remove(Context.ConnectionId, Convert.ToString(FileID));
        }

        public void JoinFile(int FileID)
        {
            Groups.Add(Context.ConnectionId, Convert.ToString(FileID));
        }

        public void OnChange(object ChangeData, int FileID)
        {
            Clients.Group(Convert.ToString(FileID), Context.ConnectionId).OnChange(ChangeData);
        }
    }
}