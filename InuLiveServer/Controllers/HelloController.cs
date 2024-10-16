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
    public class HelloController : Controller
    {

        [HttpGet]
        [Route("api/hello/")]
        public ActionResult<string> GetHello()
        {
            return "Hello";
        }

    }
}
