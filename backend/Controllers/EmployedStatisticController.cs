using ExcelTool.Commands.EmployedStatistics;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
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


        [HttpPost("UploadOther")]
        public async Task<IActionResult> UploadOther([FromForm] EmployedStatisticUploadOtherCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("ExportOther")]
        public async Task<IActionResult> ExportOther([FromBody] EmployedStatisticOtherExport command)
        {
            var result = await mediator.Send(command);
            return File(result.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Guid.NewGuid().ToString() + ".xlsx");
        }
    }
}
