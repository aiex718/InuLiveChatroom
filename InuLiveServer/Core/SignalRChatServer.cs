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
        static readonly String DefaultUserName="一位吃瓜群眾";

        ConcurrentDictionary<string,string> Cid_Username_Dict;
        readonly IHubContext<LiveLinkHub> hubContext;

        public event OnReceiveChatPayloadEventHandler OnReceiveChatPayload;
        public event OnUserEventHandler OnUserJoin;
        public event OnUserEventHandler OnUserLeave;

        public SignalRChatServer(IHubContext<LiveLinkHub> hub)
        {
            Cid_Username_Dict = new ConcurrentDictionary<string, string>();
            hubContext=hub;
        }

        public async Task ReceiveClientChatPayloadAsync(string cid, ChatPayload payload)
        {
            if (payload!=null&& payload.IsValid())
            {
                if(payload.payloadType==PayloadType.Login)
                {
                    var userName = payload.nickname;
                    if(string.IsNullOrEmpty(userName)==false && 
                        Cid_Username_Dict.TryUpdate(cid,userName,DefaultUserName))
                    {
                        OnUserJoin?.Invoke(this,cid);
                        Console.WriteLine($"{userName} login cid:{cid}");
                    }
                }
                else if(payload.payloadType==PayloadType.Msg)
                {
                    var userName = GetUserName(cid);
                    await SendChatPayloadToAllAsync(payload);             
                    Console.WriteLine($"{payload.nickname} say :{payload.message}");
                }
                else if(payload.payloadType==PayloadType.Cmd)
                {
                    var userName = GetUserName(cid);
                    if(userName!=DefaultUserName)
                        await SendChatPayloadToClientAsync(cid,payload);
                    Console.WriteLine($"{payload.nickname} send command :{payload.message}");
                }

                OnReceiveChatPayload?.Invoke(this,cid,payload);
            }
        }

        public async Task UserConnectedAsync(string cid)
        {
            if(String.IsNullOrEmpty(cid)==false)            
            {
                Cid_Username_Dict.TryAdd(cid,DefaultUserName);                
                Console.WriteLine($"{DefaultUserName} connected cid:{cid}");
            }
   
        }

        public async Task UserDisconnectedAsync(string cid)
        {
            if(String.IsNullOrEmpty(cid)==false && Cid_Username_Dict.Keys.Contains(cid))            
            {
                OnUserLeave?.Invoke(this,cid);
                Cid_Username_Dict.TryRemove(cid,out var userName);
                Console.WriteLine($"{userName} leave cid:{cid}");
            }        
        }

        public async Task SendChatPayloadToClientAsync(string cid, ChatPayload payload)
        {
            if (String.IsNullOrEmpty(cid)==false)
                await hubContext.Clients.Client(cid).SendAsync("ServerSendChatPayload", payload);
        }

        public async Task SendChatPayloadToAllAsync(ChatPayload payload)
        {
            await hubContext.Clients.All.SendAsync("ServerSendChatPayload", payload);
        }

        public IEnumerable<string> ListUser()
        {
            return Cid_Username_Dict.Values;
        }

        public string GetUserName(string cid)
        {
            string userName=null;
            Cid_Username_Dict.TryGetValue(cid,out userName);
            return userName;
        }
    }
}