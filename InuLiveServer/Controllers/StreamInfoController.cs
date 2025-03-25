using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using InuLiveServer.Models;

namespace InuLiveServer.Controllers
{
    [ApiController]
    public class StreamInfoController : Controller
    {

        // "http://inuuu.duckdns.org:8081/live/livestream.flv",
        // "http://inuuu.duckdns.org:8081/live/livestream.m3u8"

        // static readonly StreamInfo DefaultStreamInfo = null;

        // static readonly StreamInfo DefaultStreamInfo = new StreamInfo()
        // {
        //     title = "以努的狗窩",
        //     subtitle = "實況準備中",
        //     game = null,
        //     urls = new List<string>{
        //         "http://inuuu.duckdns.org:8081/live/livestream.flv",
	    //         "http://inuuu.duckdns.org:8081/live/livestream.m3u8"
        //     },
        //     isLive = null
        // };
        static readonly StreamInfo DefaultStreamInfo = new StreamInfo()
        {
            title = "InuLiveDemo",
            subtitle = "BigBuckBunny",
            game = "Movie",
            urls = new List<string>{
                "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"
            },
            isLive = false
        };

        static StreamInfo streamInfo { get; set; }  = DefaultStreamInfo;

        [HttpGet]
        [Route("api/streaminfo/")]
        public ActionResult<StreamInfo> GetStreamInfo()
        {
            return streamInfo;
        }

        [HttpGet]
        [Route("api/streaminfo/set")]
        public ActionResult<bool> SetStreamTitle([FromQuery]string title, [FromQuery]string subtitle, [FromQuery]string game)
        {
            var val=new StreamInfo();
            val.title=title;
            val.subtitle=subtitle;
            val.game=game;
            val.isLive=streamInfo?.isLive;
            val.urls=streamInfo?.urls;
            
            streamInfo = val;
            return true;
        }

        [HttpGet]
        [Route("api/streaminfo/set/default")]
        public ActionResult<bool> SetStreamToDefault()
        {
            streamInfo = DefaultStreamInfo;
            return true;
        }

        [HttpPost]
        [Route("api/streaminfo/")]
        public ActionResult<bool> PostStreamInfo([FromBody] StreamInfo val)
        {
            if (val == null || val.IsValid() == false)
                return false;
            else
            {
                streamInfo = val;
                return true;
            }
        }

        [HttpDelete]
        [Route("api/streaminfo/")]
        public ActionResult<bool> DeleteStreamInfo()
        {
            streamInfo = null;
            return true;
        }

    }
}
