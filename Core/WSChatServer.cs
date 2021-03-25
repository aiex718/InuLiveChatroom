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
    internal class WSChatServer:IChatServer
    {
        static readonly string AnonymousUserName="吃瓜群眾";

        public event IChatServer.OnReceiveMsgEventHandler OnReceiveMsg;
        public event IChatServer.OnUserEventHandler OnUserJoin;
        public event IChatServer.OnUserEventHandler OnUserLeave;

        ConcurrentDictionary<WebSocket,string> WebSocket_Username_Dict;

        public WSChatServer()
        {
            WebSocket_Username_Dict = new ConcurrentDictionary<WebSocket, string>();
        }

        internal async Task Receive(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];

            if (WebSocket_Username_Dict.ContainsKey(webSocket)==false)
            {
                WebSocket_Username_Dict.TryAdd(webSocket,AnonymousUserName);
            }

            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                //await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);                
                var payload = JsonSerializer.Deserialize<ChatPayload>(new ArraySegment<byte>(buffer, 0, result.Count));                
                if(payload.IsValid())
                {
                    if (WebSocket_Username_Dict.ContainsKey(webSocket)&& payload.payloadType==PayloadType.Login)
                    {
                        WebSocket_Username_Dict.TryUpdate(webSocket,payload.sender,AnonymousUserName);   
                        OnUserJoin?.Invoke(this,payload.sender);
                        Console.Write($"WSChatServer, UserJoin:{payload.sender}");
                    }
                    //Transmit to other
                    if(payload.payloadType == PayloadType.Msg)
                        await SendPayloadAsync(payload);
                    OnReceiveMsg?.Invoke(this,payload);    
                }
                else
                {
                    Console.Write("Warning, bad chat payload");
                    payload.Print();
                }
                //Listen next
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await CloseWebSocket(webSocket);
        }

        public async Task SendPayloadAsync(ChatPayload payload,string username=null)
        {
            List<WebSocket> SelectedWS;
            if (String.IsNullOrEmpty(username)==false)
            {
                SelectedWS = WebSocket_Username_Dict.Where(kvp=>kvp.Value==username).Select(kvp=>kvp.Key).ToList();
            }
            else
            {
                SelectedWS = WebSocket_Username_Dict.Keys.ToList();
            }

            await SendToWs(payload,SelectedWS);
        }

        private async Task SendExcept(ChatPayload payload,IEnumerable<WebSocket> WebSockets)
        {
            var SelectedWS = WebSocket_Username_Dict.Keys.Except(WebSockets).ToList();
            await SendToWs(payload,SelectedWS);
        }

        private async Task SendToWs(ChatPayload payload,IEnumerable<WebSocket> WebSockets)
        {
            if(WebSockets.Any() && payload.IsValid())
            {
                var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(payload);

                List<WebSocket> ExceptionWs = new List<WebSocket>();

                foreach (var ws in WebSockets)
                {
                    try
                    {
                        await ws.SendAsync(jsonBytes, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        ExceptionWs.Add(ws);
                        Console.WriteLine("SendToWs Ex:{0}",ex.Message);
                    }
                }

                //Remove dc websocket
                foreach (var ws in ExceptionWs)
                {
                    await CloseWebSocket(ws);
                    Console.WriteLine("Server close ws.CloseStatus:{0}",ws.CloseStatus);
                }

            }            
        }

        private async Task CloseWebSocket(WebSocket webSocket)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.Empty, "", CancellationToken.None);
            WebSocket_Username_Dict.TryRemove(webSocket, out var username);
            if (username!=AnonymousUserName)
            {
                OnUserLeave?.Invoke(this,username);
                Console.Write($"WSChatServer, UserLeave:{username}");
            }
        }

        public IEnumerable<string> ListUser()
        {
            return WebSocket_Username_Dict.Values;
        }
    }
}
