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
        static readonly string HelpMsg="輸入 #語音內容 來說話";
        WSChatServer wsChatServer;
        GoogleTTSService TTSService;

        public InuChatBot(WSChatServer chatServer)
        {
            TTSService = new GoogleTTSService();
            TTSService.StartSpeech();

            wsChatServer = chatServer;
            wsChatServer.OnUserJoin += OnUserJoin;
            wsChatServer.OnUserLeave += OnUserLeave;
            wsChatServer.OnReceiveMsg += OnReceiveMsg;
        }

        async void OnUserJoin(object sender, string username)
        {
            var response = GenerateResponse(username+" 加入聊天室");
            await wsChatServer.Send(response);

            var help = GenerateResponse(HelpMsg);
            await wsChatServer.Send(help,username);
        }

        async void OnUserLeave(object sender, string username)
        {
            var response = GenerateResponse(username+" 離開聊天室");
            await wsChatServer.Send(response);
        }

        async void OnReceiveMsg(object sender, ChatPayload payload)
        {
            if (payload.message.StartsWith('#'))
            {
                TTSService.QueueText(payload.message.Replace("#",""));
            }

            if (payload.message.Contains("@help"))
            {
                var help = GenerateResponse(HelpMsg);
                await wsChatServer.Send(help,payload.sender);
            }
            
            if (payload.message.Contains("bot"))
            {
                var response = GenerateResponse("汪 ∪･ω･∪");
                await wsChatServer.Send(response);
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