using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using InuLiveServer.Hubs;
using InuLiveServer.Models;
using Microsoft.AspNetCore.SignalR;
using static InuLiveServer.Core.IChatServer;

namespace InuLiveServer.Core
{
    public class SignalRChatServer:IChatServer
    {
        static readonly String DefaultUserName="AnonymousUser";

        ConcurrentDictionary<string,string> Cid_Username_Dict;
        IHubContext<ChatHub> _hubContext;

        public event OnReceiveMsgEventHandler OnReceiveMsg;
        public event OnUserEventHandler OnUserJoin;
        public event OnUserEventHandler OnUserLeave;

        public SignalRChatServer()
        {
            Cid_Username_Dict = new ConcurrentDictionary<string, string>();
        }

        public void ConnectToHub(IHubContext<ChatHub> hubContext)
        {
            _hubContext=hubContext;
        }

        public async Task OnSendMessage(HubCallerContext context,string user, string message)
        {
            var payload = JsonSerializer.Deserialize<ChatPayload>(message);
            if (payload!=null&& payload.IsValid())
            {
                if(payload.payloadType==PayloadType.Login)
                {
                    var userName = payload.sender;
                    var cid = context.ConnectionId;

                    if(Cid_Username_Dict.TryUpdate(cid,userName,DefaultUserName))
                    {
                        OnUserJoin?.Invoke(this,userName);
                        Console.WriteLine($"{userName} login cid:{cid}");
                    }
                }

                if(payload.payloadType==PayloadType.Msg)
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveMessage", user, message);                
                    Console.WriteLine($"{user} say :{message}");
                }
                OnReceiveMsg?.Invoke(this,payload);
            }
        }

        public async Task OnConnectedAsync(HubCallerContext context)
        {
            var cid = context.ConnectionId;
            if(String.IsNullOrEmpty(cid)==false)            
            {
                Cid_Username_Dict.TryAdd(context.ConnectionId,DefaultUserName);                
                Console.WriteLine($"AnonymousUser connected cid:{cid}");
            }
   
        }

        public async Task OnDisconnectedAsync(HubCallerContext context, Exception ex)
        {
            var cid = context.ConnectionId;
            if(String.IsNullOrEmpty(cid)==false && Cid_Username_Dict.Keys.Contains(cid))            
            {
                Cid_Username_Dict.TryRemove(context.ConnectionId,out var userName);
                Console.WriteLine($"{userName} leave cid:{cid}");
                OnUserLeave?.Invoke(this,userName);    
            }        
        }
        

        public async Task SendPayloadAsync(ChatPayload payload, string username = null)
        {
            var message = JsonSerializer.Serialize(payload);
            IEnumerable<string> cids;

            if (String.IsNullOrEmpty(username)==false)
            {
                cids = Cid_Username_Dict.Where(kvp=>kvp.Value==username).Select(kvp=>kvp.Key).ToList();
                foreach(var cid in cids)
                {
                    await _hubContext.Clients.Client(cid).SendAsync("ReceiveMessage", payload.sender, message);
                }
            }
            else
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", payload.sender, message);
            }
        }

        public IEnumerable<string> ListUser()
        {
            return Cid_Username_Dict.Values;
        }
    }
}