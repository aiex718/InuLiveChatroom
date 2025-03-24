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
using Microsoft.VisualBasic;
using System.Text;

namespace InuLiveServer.Core
{
    internal class InuChatBot: IHostedService
    {
        static readonly string WelcomeMessage = "輸入/help 顯示幫助訊息";
        IChatServer chatServer;
        IVideoSyncServer videoSyncServer;

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

        public InuChatBot(IChatServer chat, IVideoSyncServer sync)
        {
            videoSyncServer = sync;
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

            var help = GenerateResponse(WelcomeMessage);
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
            if(payload.payloadType!=PayloadType.Cmd)
                return; //only handle command

            if (payload.message == "/help")
            {
                var help = GenerateResponse(GetHelpMessage());
                await chatServer.SendChatPayloadToClientAsync(cid,help);
            }
            else if (payload.message == "/who")
            {
                var users =  chatServer.ListUser().ToList();
                var response = GenerateResponse($"現在線上人數為{users.Count}人 ,已登入使用者:{users.Aggregate((i, j) => i + ',' + j)}");
                await chatServer.SendChatPayloadToClientAsync(cid,response);
            }
            else if(payload.message.StartsWith("/sync"))
            {
                var args = payload.message.Split(' ');

                if(args.Length == 1)
                    await videoSyncServer.SyncClientAsync(cid);
                else if(args.Length == 2 && args[1] == "all")
                    await videoSyncServer.SyncAllClientAsync();
                else
                {
                    var response = GenerateResponse($"參數錯誤 {args[1]}");
                    await chatServer.SendChatPayloadToClientAsync(cid,response);
                }
            }
            else if(payload.message == "/playtime")
            {
                var time = videoSyncServer.GetCurrentTime();
                var response = GenerateResponse($"目前播放時間:{time}");
                await chatServer.SendChatPayloadToClientAsync(cid,response);
            }
            else if(payload.message == "/play")
            {
                await videoSyncServer.VideoResume();
            }
            else if(payload.message == "/pause")
            {
                await videoSyncServer.VideoPause();
            }
            else if(payload.message.StartsWith("/offset"))
            {
                var args = payload.message.Split(' ');
                if(args.Length == 2)
                {
                    if(double.TryParse(args[1],out double offset))
                        await videoSyncServer.SetOffsetTimeAsync(TimeSpan.FromSeconds(offset));
                    else
                    {
                        var response = GenerateResponse($"參數錯誤 {args[1]}");
                        await chatServer.SendChatPayloadToClientAsync(cid,response);
                    }
                }
            }
            else if(payload.message.StartsWith("/seek"))
            {
                var args = payload.message.Split(' ');
                if(args.Length == 2)
                {
                    TimeSpan time;
                    if(TimeSpan.TryParse(args[1],out time))
                        await videoSyncServer.SetCurrentTimeAsync(time);
                    else
                    {
                        var response = GenerateResponse($"參數錯誤 {args[1]}");
                        await chatServer.SendChatPayloadToClientAsync(cid,response);
                    }
                }
            }
            else if (payload.message == "/bot")
            {
                var response = GenerateResponse("汪 ∪･ω･∪");
                await chatServer.SendChatPayloadToAllAsync(response);
            }
            else
            {
                var response = GenerateResponse($"聽不懂{payload.message}是什麼（∪･ω･∪）＜ 汪");
                await chatServer.SendChatPayloadToClientAsync(cid,response);
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

        static string GetHelpMessage()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("輸入/help 顯示幫助訊息");
            sb.AppendLine("輸入/who 查看線上使用者");
            sb.AppendLine("輸入/sync 同步影片播放時間");
            sb.AppendLine("輸入/sync all 同步所有使用者影片播放時間");
            sb.AppendLine("輸入/playtime 查看目前影片播放時間");
            sb.AppendLine("輸入/play 播放影片");
            sb.AppendLine("輸入/pause 暫停影片");
            sb.AppendLine("輸入/offset [秒數] 設定影片時間偏移");
            sb.AppendLine("輸入/seek [時間(00:00:00)] 設定影片時間");
            sb.AppendLine("輸入/bot 呼叫狗狗機器人");
            return sb.ToString();
        }

    }
}