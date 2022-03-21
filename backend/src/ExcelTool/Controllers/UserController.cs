using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ExcelTool.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("Profile")]
        public async Task<IActionResult> Get()
        {
            var a = HttpContext.User;
            return Ok(new { Id = "admin" });;
        }
    }
}
