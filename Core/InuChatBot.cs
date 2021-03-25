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
    internal class InuChatBot
    {
        static readonly string HelpMsg="輸入 #語音內容 來說話，!who 查看線上使用者";
        IChatServer ChatServer;
        GoogleTTSService TTSService;

        public InuChatBot()
        {
            TTSService = new GoogleTTSService();
            TTSService.StartSpeech();
        }

        public void Attach(IChatServer chatServer)
        {
            ChatServer = chatServer;
            ChatServer.OnUserJoin += OnUserJoin;
            ChatServer.OnUserLeave += OnUserLeave;
            ChatServer.OnReceiveMsg += OnReceiveMsg;
        }

        public void Detach()
        {
            if(ChatServer!=null)
            {
                ChatServer.OnUserJoin -= OnUserJoin;
                ChatServer.OnUserLeave -= OnUserLeave;
                ChatServer.OnReceiveMsg -= OnReceiveMsg;
                ChatServer=null;
            }
        }

        async void OnUserJoin(object sender, string username)
        {
            var response = GenerateResponse(username+" 加入聊天室");
            await ChatServer.SendPayloadAsync(response);

            var help = GenerateResponse(HelpMsg);
            await ChatServer.SendPayloadAsync(help,username);
        }

        async void OnUserLeave(object sender, string username)
        {
            var response = GenerateResponse(username+" 離開聊天室");
            await ChatServer.SendPayloadAsync(response);
        }

        async void OnReceiveMsg(object sender, ChatPayload payload)
        {
            if (payload.message.StartsWith('#'))
            {
                TTSService.QueueText(payload.message.Replace("#",""));
            }

            else if (payload.message.StartsWith("!help"))
            {
                var help = GenerateResponse(HelpMsg);
                await ChatServer.SendPayloadAsync(help,payload.sender);
            }

            else if (payload.message.StartsWith("!who"))
            {
                var users =  ChatServer.ListUser().ToList();
                var who = GenerateResponse($"現在線上人數為{users.Count}人 ,已登入使用者:{users.Aggregate((i, j) => i + ',' + j)}");
                await ChatServer.SendPayloadAsync(who,payload.sender);
            }
            
            else if (payload.message.Contains("bot"))
            {
                var response = GenerateResponse("汪 ∪･ω･∪");
                await ChatServer.SendPayloadAsync(response);
            }
        }

        static ChatPayload GenerateResponse(string msg)
        {
            return new ChatPayload()
            {
                sender="狗狗機器人",
                type="Info",
                color="gray",
                message=msg
            };
        }
    }
}