using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwtInJsApps.Web.Controllers
{
    [ApiController]
    [Route("[CONTROLLER]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DummyController:ControllerBase
    {
        public DummyController()
        {

        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("it worked!");
        }
    }
}
