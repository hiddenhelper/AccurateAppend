using System;
using Microsoft.AspNet.SignalR;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Provides the SignalR hub used for context callbacks.
    /// </summary>
    public class CallbackHub : Hub
    {
        /// <summary>
        /// Communicates to the client browser that an asynchronous wait process has completed.
        /// </summary>
        /// <param name="connectionId">The identifier of the connection to send the callback to.</param>
        /// <param name="publicKey">The pubic key identifier used to uniquely identify the asynchronous operation.</param>
        /// <param name="message">The message (generally human readable) that describes the result of the operation.</param>
        public void OnCallbackComplete(String connectionId, Guid publicKey, String message)
        {
            this.Clients.Client(connectionId).callbackComplete(publicKey.ToString(), message);
        }

        /// <summary>
        /// Communicates to the client browser that an asynchronous wait process has completed.
        /// </summary>
        /// <param name="connectionId">The identifier of the connection to send the callback to.</param>
        public void OnCallbackComplete(String connectionId)
        {
            this.Clients.Client(connectionId).callbackComplete();
        }

        // uncomment this to eventually test connect callback issues (if any)
        //public override Task OnConnected()
        //{
        //    var t1 = Groups.Add(Context.ConnectionId, Context.ConnectionId);
        //    var t2 = base.OnConnected();

        //    return Task.WhenAll(t1, t2);
        //}
    }
}