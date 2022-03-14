using ExcelTool.Infrastructures.JTW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ExcelTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private IAuthenticateService _authService;

        public TokenController(IAuthenticateService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost, Route("Request")]
        public IActionResult RequestToken([FromBody] LoginDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request");
            }
            string token;
            if (_authService.IsAuthenticated(request, out token))
            {
                return Ok(new TokenRespondDto() { Token = token });
            }
            return BadRequest("Invalid Request");
        }
    }
}
