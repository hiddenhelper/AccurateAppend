using System;
using Microsoft.AspNet.SignalR;

namespace AccurateAppend.Websites.Admin
{
    public class CallbackHub : Hub
    {
        public void Send(String message)
        {
            this.Clients.All.addNewMessageToPage(message);
        }
    }
}