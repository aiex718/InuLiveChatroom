using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InuLiveServer.Hubs;
using InuLiveServer.Models;
using Microsoft.AspNetCore.SignalR;

namespace InuLiveServer.Core
{
    public class SignalRSyncServer:IVideoSyncServer
    {
        readonly IHubContext<LiveLinkHub> hubContext;
        static readonly DateTime startTime = DateTime.Now;
        static TimeSpan syncOffset { get; set; } = TimeSpan.Zero;
        static bool IsPaused { get; set; } = false;
        static TimeSpan pauseTime { get; set; } = TimeSpan.Zero;

        public SignalRSyncServer(IHubContext<LiveLinkHub> hub)
        {
            hubContext=hub;
        }

        public TimeSpan GetCurrentTime()
        {
            return DateTime.Now - startTime - syncOffset;
        }

        
        public async Task VideoPause()
        {
            if (IsPaused == false)
            {
                IsPaused = true;
                pauseTime = GetCurrentTime();
                await SyncAllClientAsync();
            }
        }

        public async Task VideoResume()
        {
            if(IsPaused)
            {
                IsPaused = false;
                await SetCurrentTimeAsync(pauseTime);
            }
        }

        public async Task SetOffsetTimeAsync(TimeSpan t)
        {
            syncOffset -= t;
            await SyncAllClientAsync();
        }

        public async Task SetCurrentTimeAsync(TimeSpan t)
        {
            syncOffset = DateTime.Now - startTime - t;
            await SyncAllClientAsync();
        }

        public async Task SyncClientAsync(string cid)
        {
            var payload = GenerateVideoCtrlPayload();
            await hubContext.Clients.Client(cid).SendAsync("ServerSendVideoCtrlPayload", payload);
        }

        public async Task SyncAllClientAsync()
        {
            var payload = GenerateVideoCtrlPayload();
            await hubContext.Clients.All.SendAsync("ServerSendVideoCtrlPayload", payload);
        }

        VideoCtrlPayload GenerateVideoCtrlPayload()
        {
            return new VideoCtrlPayload
            {
                paused = IsPaused,
                currentTime = GetCurrentTime().TotalSeconds
            };
        }

    }
}