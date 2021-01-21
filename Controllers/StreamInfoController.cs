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
        [HttpGet]
        [Route("api/streaminfo/")]
        public ActionResult<StreamInfo> GetStreamInfo()
        {
            return Program.streamInfo;
        }
    }
}
