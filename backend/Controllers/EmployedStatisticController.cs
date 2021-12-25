using ExcelTool.Commands.EmployedStatistics;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ExcelTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployedStatisticController : ControllerBase
    {
        private readonly IMediator mediator;

        public EmployedStatisticController(IMediator mediator)
        {
            this.mediator = mediator;
        }


        [HttpPost("uploadOther")]
        public async Task<IActionResult> UploadOther([FromForm] EmployedStatisticUploadOtherCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
    }
}
