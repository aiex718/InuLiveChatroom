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
        // push
        // rtmp://inuliveserver-srs.bndsbja0b7eje2gu.eastasia.azurecontainer.io/live/livestream
        // http://inuliveserver-srs.bndsbja0b7eje2gu.eastasia.azurecontainer.io:1985/rtc/v1/whip/?app=live&stream=livestream
        // pull
        // http://inuliveserver-srs.bndsbja0b7eje2gu.eastasia.azurecontainer.io:8080/live/livestream.flv
        // http://inuliveserver-srs.bndsbja0b7eje2gu.eastasia.azurecontainer.io:8080/live/livestream.m3u8
        // http://inuliveserver-srs.bndsbja0b7eje2gu.eastasia.azurecontainer.io:1985/rtc/v1/whep/?app=live&stream=livestream

        // static readonly StreamInfo DefaultStreamInfo = null;

        static readonly StreamInfo DefaultStreamInfo = new StreamInfo()
        {
            title = "InuLive",
            subtitle = "Big Buck Bunny",
            game = "DEMO",
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
