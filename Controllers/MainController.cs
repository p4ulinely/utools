using Microsoft.AspNetCore.Mvc;
using utools.Data;

namespace utools.Controllers
{
    [ApiController]
    [Route("")]
    public class MainController : ControllerBase
    {

        [HttpGet]
        [Route("")]
        public IActionResult displayMain()
        {
            return Ok(new { api = "v1",
                api_path = "/v1/empresas",
                author = "Paulinely"});
        }
    }
}