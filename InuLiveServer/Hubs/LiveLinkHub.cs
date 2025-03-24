using System.Diagnostics;
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
    public class LiveLinkHub : Hub
    {
        private readonly IChatServer chatServer;

        public LiveLinkHub(IChatServer chat)
        {
            chatServer = chat;
        }

        //called by client
        public async Task ClientSendChatPayload(ChatPayload payload)
        {
            await chatServer.ReceiveClientChatPayloadAsync(Context.ConnectionId,payload);
        }

        public override async Task OnConnectedAsync()        
        {
            await chatServer.UserConnectedAsync(Context.ConnectionId);
            await base.OnConnectedAsync();     
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await chatServer.UserDisconnectedAsync(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        
    }
}