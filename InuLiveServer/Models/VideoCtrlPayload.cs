using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InuLiveServer.Models
{
    public class VideoCtrlPayload
    {
        public bool paused { get; set; }
        public double currentTime { get; set; }
    }
}