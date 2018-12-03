using Microsoft.AspNetCore.Mvc;

namespace AsyncApi.Worker.Controllers
{
    [Route("api/[controller]")]
    public class PingController : Controller
    {
        [HttpGet]
        public string Get()
        {
            return "response";
        }
    }
}
