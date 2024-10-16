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
        static StreamInfo _streamInfo { get; set; } = new StreamInfo()
        {
            title = "InuLiveDemo",
            subtitle = "´ú¸Õ¼v¤ù",
            game = "Big Buck Bunny",
        };

        [HttpGet]
        [Route("api/streaminfo/")]
        public ActionResult<StreamInfo> GetStreamInfo()
        {
            return _streamInfo;
        }

        [HttpPost]
        [Route("api/streaminfo/")]
        public ActionResult<bool> PostStreamInfo([FromBody] StreamInfo streamInfo)
        {
            if (streamInfo == null || streamInfo.IsValid() == false)
                return false;
            else
            {
                if(_streamInfo == null)
                    _streamInfo=new StreamInfo();
                
                streamInfo.CopyTo(_streamInfo);
            }
            return true;
        }

        [HttpDelete]
        [Route("api/streaminfo/")]
        public ActionResult<bool> DeleteStreamInfo()
        {
            _streamInfo = null;
            return true;
        }

    }
}
