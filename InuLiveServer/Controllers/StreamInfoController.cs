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
        static StreamInfo _streamInfo { get; set; }

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
