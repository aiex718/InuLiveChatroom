using System.Linq;
using System.Net;
using System.IO;
using System.Net.WebSockets;
using InuLiveServer.Models;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace InuLiveServer.Core
{
    internal class InuChatBot: IHostedService
    {
        static readonly string HelpMsg="輸入/who 查看線上使用者";
        IChatServer chatServer;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("InuChatBot started");
            await Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("InuChatBot stopped");
            return Task.CompletedTask;
        }

        public InuChatBot(IChatServer chat)
        {
            chatServer = chat;

            chatServer.OnUserJoin += OnUserJoin;
            chatServer.OnUserLeave += OnUserLeave;
            chatServer.OnReceiveChatPayload += OnReceiveChatPayload;
        }
        

        async void OnUserJoin(object sender, string cid)
        {
            var username = chatServer.GetUserName(cid);
            var response = GenerateResponse(username+" 加入聊天室");
            await chatServer.SendChatPayloadToAllAsync(response);

            var help = GenerateResponse(HelpMsg);
            await chatServer.SendChatPayloadToClientAsync(cid,help);
        }

        async void OnUserLeave(object sender, string cid)
        {
            var username = chatServer.GetUserName(cid);
            var response = GenerateResponse(username+" 離開聊天室");
            await chatServer.SendChatPayloadToAllAsync(response);
        }

        async void OnReceiveChatPayload(object sender,string cid, ChatPayload payload)
        {
            if (payload.message == "/help")
            {
                var help = GenerateResponse(HelpMsg);
                await chatServer.SendChatPayloadToClientAsync(cid,help);
            }
            else if (payload.message == "/who")
            {
                var users =  chatServer.ListUser().ToList();
                var who = GenerateResponse($"現在線上人數為{users.Count}人 ,已登入使用者:{users.Aggregate((i, j) => i + ',' + j)}");
                await chatServer.SendChatPayloadToClientAsync(cid,who);
            }
            else if(payload.message.StartsWith("/sync"))
            {

            }
            else if (payload.message.Contains("bot"))
            {
                var response = GenerateResponse("汪 ∪･ω･∪");
                await chatServer.SendChatPayloadToAllAsync(response);
            }
        }

        static ChatPayload GenerateResponse(string msg)
        {
            return new ChatPayload()
            {
                nickname="狗狗機器人",
                type="Info",
                color="gray",
                message=msg
            };
        }

    }
}