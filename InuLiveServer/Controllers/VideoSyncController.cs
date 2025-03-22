using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace InuLiveServer.Controllers
{
    [ApiController]
    public class VideoSyncController : Controller
    {
        static TimeSpan syncOffset { get; set; } = TimeSpan.Zero;

        [HttpGet]
        [Route("api/videosync")]
        public ActionResult<double> GetVideoSyncSeconds()
        {
            return (DateTime.Now - Program.startTime - syncOffset).TotalSeconds;
        }

        [HttpGet]
        [Route("api/videosync/set")]
        public ActionResult<bool> SetVideoSyncSeconds([FromQuery]double t)
        {
            syncOffset = DateTime.Now - Program.startTime - TimeSpan.FromSeconds(t);
            return true;
        }

        [HttpGet]
        [Route("api/videosync/offset")]
        public ActionResult<bool> OffsetVideoSyncSeconds([FromQuery]double t)
        {
            syncOffset -= TimeSpan.FromSeconds(t);
            return true;
        }

        [HttpGet]
        [Route("api/videosync/reload")]
        public ActionResult<bool> VideoSyncReload()
        {
            syncOffset = DateTime.Now-Program.startTime;
            return true;
        }
    }
}