using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.WebData;

namespace TiktokTools.Web
{
    public class ChatHub : Hub
    {
        public void Send(string id, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.Client(id).addNewMessageToPage(message);
        }
    }
}