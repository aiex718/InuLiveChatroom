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
    public class ChatHub : Hub
    {
        
        public async Task SendMessage(string user, string message)
        {
            await Program.chatServer.OnSendMessage(Context,user,message);
        }

        public override async Task OnConnectedAsync()        
        {
            await Program.chatServer.OnConnectedAsync(Context);
            await base.OnConnectedAsync();     
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Program.chatServer.OnDisconnectedAsync(Context, exception);
            await base.OnDisconnectedAsync(exception);
        }
        
    }
}