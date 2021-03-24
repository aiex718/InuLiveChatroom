using System;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using InuLiveServer.Core;
using InuLiveServer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace InuLiveServer.Hubs
{
    public class ChatHub : Hub,IChatServer
    {
        static ConcurrentDictionary<string,string> Cid_Username_Dict = new ConcurrentDictionary<string, string>();
        public event IChatServer.OnReceiveMsgEventHandler OnReceiveMsg;
        public event IChatServer.OnUserEventHandler OnUserJoin;
        public event IChatServer.OnUserEventHandler OnUserLeave;
        
        public async Task SendMessage(string user, string message)
        {
            var payload = JsonSerializer.Deserialize<ChatPayload>(message);
            if (payload!=null&& payload.IsValid())
            {
                if(payload.payloadType==PayloadType.Msg)
                    await Clients.All.SendAsync("ReceiveMessage", user, message);
                    
                OnReceiveMsg?.Invoke(this,payload);
            }
        }

        public override async Task OnConnectedAsync()        
        {
            var userName = Context.User.Identity.Name;
            var cid = Context.ConnectionId;
            if(String.IsNullOrEmpty(cid)==false &&String.IsNullOrEmpty(userName)==false )            
            {
                Cid_Username_Dict.TryAdd(Context.ConnectionId,userName);
                OnUserJoin?.Invoke(this,userName);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var cid = Context.ConnectionId;
            if(String.IsNullOrEmpty(cid)==false && Cid_Username_Dict.Keys.Contains(cid))            
            {
                Cid_Username_Dict.TryRemove(Context.ConnectionId,out var userName); 
                OnUserLeave?.Invoke(this,userName);    
            }

            await base.OnDisconnectedAsync(exception);
        }

        async Task IChatServer.SendAsync(ChatPayload payload, string username = null)
        {
            var message = JsonSerializer.Serialize(payload);
            IEnumerable<string> cids;

            if (String.IsNullOrEmpty(username)==false)
            {
                cids = Cid_Username_Dict.Where(kvp=>kvp.Value==username).Select(kvp=>kvp.Key).ToList();
                foreach(var cid in cids)
                {
                    await Clients.Client(cid).SendAsync("ReceiveMessage", payload.sender, message);
                }
            }
            else
            {
                await Clients.All.SendAsync("ReceiveMessage", payload.sender, message);
            }
        }

        IEnumerable<string> IChatServer.ListUser()
        {
            return Cid_Username_Dict.Values;
        }
    }
}