using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InuLiveServer.Core
{
    public interface IVideoSyncServer
    {
        //bool IsAutoSyncEnabled { get; }
        //bool AutoSyncCtrl(bool en);

        TimeSpan GetCurrentTime();
        Task VideoPause();
        Task VideoResume();

        Task SetOffsetTimeAsync(TimeSpan t);
        Task SetCurrentTimeAsync(TimeSpan t);
        Task SyncClientAsync(string cid);
        Task SyncAllClientAsync();
    }
}